using System;
using System.Collections.Generic;
using System.Linq;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.PathFinding
{
    public class PathFindingManager : MonoBehaviour
    {
        internal struct GridCell
        {
            public Vector2Int GridPosition;
            public Vector2 WorldPosition;
            public int Weight;
            public float StraightTargetDistance;
        }
        
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
            if (_grid == null)
            {
                RegenerateGrid();
            }
        }

        internal void RegisterTarget(PathFindingTarget p_target)
        {
            // HACKME: Waiting for code architecture to resolve dependency hell
            if (_grid == null)
            {
                RegenerateGrid();
            }
            
            BakeToTaret(p_target);
        }

        public List<Vector2> GetPathToTarget(PathFindingObjectController p_object)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Pathfinding");
            
            var srcCellPosition = GetCellFromWorldPosition(p_object.transform.position.XZ());

            var path = new List<Vector2Int>();
            path.Add(srcCellPosition);
            
            while (true)
            {
                var bestCell = TryGetLowestWeightNeighbour(path[^1]);

                if (!bestCell.HasValue)
                {
                    Debug.LogError("Failed in searching path. Returning empty path!");
                    return new List<Vector2>();
                }
                
                path.Add(bestCell.Value);

                // Check is target
                if (_grid[bestCell.Value.x, bestCell.Value.y].Weight == 0)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return path.Select(pathCell => _grid[pathCell.x, pathCell.y].WorldPosition).ToList();
                }
            }
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
                    };
                }
            }
        }

        internal void BakeToTaret(PathFindingTarget p_target)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Baking grid");
            if (!IsPointInsideGrid(p_target.transform.position.XZ()))
            {
                UnityEngine.Profiling.Profiler.EndSample();
                Debug.LogError($"Position {p_target.transform.position} is outside grid!");
                return;
            }

            Debug.Log($"Baking to position {p_target.transform.position}");

            var cellPosition = GetCellFromWorldPosition(p_target.transform.position.XZ());

            for (var y = 0; y < _grid.GetLength(1); y++)
            {
                for (var x = 0; x < _grid.GetLength(0); x++)
                {
                    ref var cell = ref _grid[x, y];
                    cell.Weight = int.MaxValue;
                }
            }

            var openPoints = new Queue<Vector2Int>(_grid.Length);
            var discradedPoints = new HashSet<Vector2Int>(_grid.Length);
            
            openPoints.Enqueue(cellPosition);
            discradedPoints.Add(cellPosition);
            
            _grid[cellPosition.x, cellPosition.y].Weight = 0;

            while (!openPoints.IsEmpty())
            {
                var point = openPoints.Dequeue();
                var parentCellWeight = _grid[point.x, point.y].Weight;
                
                ForEachCellNeighbour(point, false, (ref GridCell cell) =>
                {
                    if (!discradedPoints.Contains(cell.GridPosition))
                    {
                        cell.Weight = parentCellWeight + 1;
                        openPoints.Enqueue(cell.GridPosition);
                        discradedPoints.Add(cell.GridPosition);
                    }
                });
            }

            UnityEngine.Profiling.Profiler.EndSample();
        }

        internal delegate void GridCellAction(ref GridCell p_cell);

        private void ForEachCellNeighbour(Vector2Int p_cellPosition, bool p_includeDiagonals, GridCellAction p_action)
        {
            var offsets = new List<Vector2Int>(8)
            {
                new(0, 1),
                new(-1, 0),
                new(1, 0),
                new(0, -1)
            };

            if (p_includeDiagonals)
            {
                offsets.AddRange(new []
                {
                    new Vector2Int(-1, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(-1, -1),
                    new Vector2Int(1, -1)
                });
            }

            foreach (var offset in offsets)
            {
                if (IsCellPositionValid(p_cellPosition + offset))
                {
                    ref var cell = ref _grid[p_cellPosition.x + offset.x, p_cellPosition.y + offset.y];
                    p_action.Invoke(ref cell);
                }
            }
        }

        private Vector2Int GetCellFromWorldPosition(Vector2 p_worldPosition)
        {
            var targetLocalPosition = p_worldPosition - GetMinCorner();
            return new Vector2Int(
                Mathf.FloorToInt(targetLocalPosition.x / _gridCellSize),
                Mathf.FloorToInt(targetLocalPosition.y / _gridCellSize));
        }

        private bool IsCellPositionValid(Vector2Int p_position)
        {
            return p_position.x >= 0 && p_position.x < _grid.GetLength(0)
                && p_position.y >= 0 && p_position.y < _grid.GetLength(1);
        }

        internal Vector2Int? TryGetLowestWeightNeighbour(Vector2Int p_cellPosition)
        {
            var lowestWeight = (float)int.MaxValue;
            Vector2Int? lowestWeigthNeighbour = null;
            
            ForEachCellNeighbour(p_cellPosition, true, (ref GridCell cell) =>
            {
                if (lowestWeight > cell.Weight)
                {
                    lowestWeight = cell.Weight;
                    lowestWeigthNeighbour = cell.GridPosition;
                }
            });

            if (!lowestWeigthNeighbour.HasValue)
            {
                Debug.LogError($"Failed to find neighbour of {p_cellPosition}");
                return null;
            }

            return lowestWeigthNeighbour;
        }

        private bool IsPointInsideGrid(Vector2 p_worldPosition)
        {
            var minCorner = GetMinCorner();
            var maxCorner = GetMaxCorner();

            return minCorner.x < p_worldPosition.x
                   && maxCorner.x > p_worldPosition.x
                   && minCorner.y < p_worldPosition.y
                   && maxCorner.y > p_worldPosition.y;
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
}
