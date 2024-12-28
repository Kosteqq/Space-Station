using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        [SerializeField]
        internal float _gridCellSize = 1f;

        [SerializeField]
        internal Vector2 _gridBoundsCenter = Vector2.zero;

        [SerializeField]
        internal Vector2 _gridBoundsSize = Vector2.one;
        
        private GridCell[,] _grid;
        
        internal GridCell[,] Grid => _grid;
        
        private void Start()
        {
            RegenerateGrid();
        }

        private void Update()
        {
            
        }

        internal void RegenerateGrid()
        {
            var size = GetGridSize();

            var minCorner = GetMinCorner() + new Vector2(_gridCellSize, _gridCellSize) / 2f;
            _grid = new GridCell[size.x, size.y];

            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    _grid[x, y] = new GridCell
                    {
                        GridPosition = new Vector2Int(x, y),
                        WorldPosition = minCorner + new Vector2(x, y) * _gridCellSize,
                        // Direction = new Vector2(0.5f, 0.5f),
                    };
                }
            }
        }

        internal void BakeToPosition(Vector3 p_worldPosition)
        {
            if (!IsPointInsideGrid(p_worldPosition))
            {
                Debug.LogError($"Position {p_worldPosition} is outside grid!");
                return;
            }

            Debug.Log($"Baking to position {p_worldPosition} {IsPointInsideGrid(p_worldPosition)}");

            var targetLocalPosition = p_worldPosition.XZ() - GetMinCorner();
            var targetPosition = new Vector2Int(Mathf.FloorToInt(targetLocalPosition.x / _gridCellSize), Mathf.FloorToInt(targetLocalPosition.y / _gridCellSize));

            for (var y = 0; y < _grid.GetLength(1); y++)
            {
                for (var x = 0; x < _grid.GetLength(0); x++)
                {
                    ref var cell = ref _grid[x, y];
                    cell.Weight = -1;
                }
            }

            // HACKME: Primitive method, optimize it
            while (true)
            {
                var any = false;
                
                for (var y = 0; y < _grid.GetLength(1); y++)
                {
                    for (var x = 0; x < _grid.GetLength(0); x++)
                    {
                        ref var cell = ref _grid[x, y];

                        if (cell.Weight >= 0)
                        {
                            continue;
                        }
                        
                        if (targetPosition == cell.GridPosition)
                        {
                            cell.Weight = 0;
                            any = true;
                            continue;
                        }

                        var neighourWeight = int.MaxValue;

                        if (x > 0 && _grid[x - 1, y].Weight != -1)
                        {
                            neighourWeight = Mathf.Min(neighourWeight, _grid[x - 1, y].Weight);
                        }

                        if (x < _grid.GetLength(0) - 1 && _grid[x + 1, y].Weight != -1)
                        {
                            neighourWeight = Mathf.Min(neighourWeight, _grid[x + 1, y].Weight);
                        }

                        if (y > 0 && _grid[x, y - 1].Weight != -1)
                        {
                            neighourWeight = Mathf.Min(neighourWeight, _grid[x, y - 1].Weight);
                        }

                        if (y < _grid.GetLength(1) - 1  &&_grid[x, y + 1].Weight != -1)
                        {
                            neighourWeight = Mathf.Min(neighourWeight, _grid[x, y + 1].Weight);
                        }

                        if (neighourWeight != int.MaxValue)
                        {
                            var delta = targetPosition - cell.GridPosition;
                            cell.TargetDistance = delta.magnitude;
                            cell.Weight = neighourWeight + 1;
                            any = true;
                        }
                    }
                }
                
                if (!any)
                {
                    break;
                }
            }
        }

        private void WriteToNeighboursWeight(Vector2Int p_cellPosition, float p_cellWeight)
        {
            var gridSize = GetGridSize();
            
            for (var y = p_cellPosition.y - 1; y <= p_cellPosition.y + 1; y++)
            {
                if (y < 0 || y >= gridSize.y)
                {
                    continue;
                }
             
                for (var x = p_cellPosition.x - 1; x <= p_cellPosition.x + 1; x++)
                {
                    if (x < 0 || x >= gridSize.x)
                    {
                        continue;
                    }

                    if (p_cellPosition == new Vector2Int(x, y))
                    {
                        continue;
                    }
                    
                    var delta = new Vector2Int(x, y) - p_cellPosition;
                    var weight = p_cellWeight + (delta.magnitude > 1 ?  1.5f : 1f);

                    if (_grid[x, y].Weight == -1f || _grid[x, y].Weight > weight)
                    {
                        ref var neighbour = ref _grid[x, y];
                        // neighbour.Weight = weight;
                    }
                }   
            }
        }

        internal GridCell? GetLowestWeightNeighbour(Vector2Int p_cellPosition)
        {
            var lowestWeight = (float)int.MaxValue;
            var lowestWeigthNeighbour = p_cellPosition;

            var gridSize = GetGridSize();

            for (var y = p_cellPosition.y - 1; y <= p_cellPosition.y + 1; y++)
            {
                if (y < 0 || y >= gridSize.y)
                {
                    continue;
                }
             
                for (var x = p_cellPosition.x - 1; x <= p_cellPosition.x + 1; x++)
                {
                    if (x < 0 || x >= gridSize.x)
                    {
                        continue;
                    }

                    if (p_cellPosition == new Vector2Int(x, y))
                    {
                        continue;
                    }

                    if (_grid[x, y].Weight >= 0 && _grid[x, y].Weight <= lowestWeight)
                    {
                        lowestWeight = _grid[x, y].Weight;
                        lowestWeigthNeighbour = new Vector2Int(x, y);
                    }
                }   
            }

            if (lowestWeigthNeighbour == p_cellPosition)
            {
                return null;
            }

            return _grid[lowestWeigthNeighbour.x, lowestWeigthNeighbour.y];
        }

        private bool IsPointInsideGrid(Vector3 p_worldPosition)
        {
            var minCorner = GetMinCorner();
            var maxCorner = GetMaxCorner();

            return minCorner.x < p_worldPosition.x
                   && maxCorner.x > p_worldPosition.x
                   && minCorner.y < p_worldPosition.z
                   && maxCorner.y > p_worldPosition.z;
        }

        private Vector2Int GetGridSize()
        {
            return new Vector2Int(
                Mathf.FloorToInt(_gridBoundsSize.x / _gridCellSize),
                Mathf.FloorToInt(_gridBoundsSize.y / _gridCellSize));
        }

        private Vector2 GetMinCorner()
        {
            return _gridBoundsCenter - _gridBoundsSize / 2f;
        }

        private Vector2 GetMaxCorner()
        {
            var size = GetGridSize();
            var corner = GetMinCorner();

            return corner + (Vector2)size * _gridCellSize;
        }
    }

    internal struct GridCell
    {
        public Vector2Int GridPosition;
        public Vector2 WorldPosition;
        public int Weight;
        public float TargetDistance;
    }
}
