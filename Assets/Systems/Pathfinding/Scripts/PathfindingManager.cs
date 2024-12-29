using System;
using System.Collections.Generic;
using System.Linq;
using SpaceStation.Utils;
using Unity.VisualScripting.FullSerializer;
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

        private struct OpenSearchPoint
        {
            public readonly Vector2Int Point;
            public readonly Vector2[] PrevPoints;
            public readonly float StraigthDistance;
            public readonly float PathDistance;
            public readonly float TotalDistance;

            public OpenSearchPoint(in GridCell p_cell, in OpenSearchPoint? p_prevPoint)
            {
                Point = p_cell.GridPosition;
                PrevPoints = new[] { p_cell.WorldPosition };
                StraigthDistance = p_cell.StraightTargetDistance;
                PathDistance = 0f;
                TotalDistance = p_cell.StraightTargetDistance;

                if (p_prevPoint.HasValue)
                {
                    PrevPoints = new Vector2[p_prevPoint.Value.PrevPoints.Length + 1];
                    p_prevPoint.Value.PrevPoints.CopyTo(PrevPoints, 0);
                    PrevPoints[^1] = p_cell.WorldPosition;

                    var delta = (Point - p_prevPoint.Value.Point).magnitude;
                    PathDistance = p_prevPoint.Value.PathDistance + delta;
                    TotalDistance += PathDistance;
                }
            }
        }

        public List<Vector2> GetPathToPoint(Vector2 p_srcPosition, Vector2 p_targetPosition)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Pathfinding");
            var srcCellPosition = GetCellFromWorldPosition(p_srcPosition);
            var targetCellPosition = GetCellFromWorldPosition(p_targetPosition);
            
            BakeToPosition(p_targetPosition);

            var srcCell = _grid[srcCellPosition.x, srcCellPosition.y];
            var targetCell = _grid[targetCellPosition.x, targetCellPosition.y];
            
            var openPoints = new List<OpenSearchPoint>(_grid.Length);
            var discardedPoints = new HashSet<Vector2Int>(_grid.Length);
            openPoints.Add(new OpenSearchPoint(srcCell, null));
            discardedPoints.Add(srcCell.GridPosition);
            
            while (true)
            {
                if (openPoints.IsEmpty())
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return new List<Vector2>();
                }
                
                var bestDistance = float.MaxValue;
                var bestStartIndex = 0;

                for (var pointIndex = 0; pointIndex < openPoints.Count; pointIndex++)
                {
                    var point = openPoints[pointIndex];
                    if (bestDistance > point.TotalDistance)
                    {
                        bestStartIndex = pointIndex;
                        bestDistance = point.TotalDistance;
                    }
                }

                var startPoint = openPoints[bestStartIndex];
                openPoints.RemoveAt(bestStartIndex);

                if (startPoint.Point == targetCell.GridPosition)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return startPoint.PrevPoints.ToList();
                }

                ForEachCellNeighbour(startPoint.Point, cell =>
                {
                    if (!discardedPoints.Contains(cell.GridPosition))
                    {
                        discardedPoints.Add(cell.GridPosition);
                        openPoints.Add(new OpenSearchPoint(cell, startPoint));
                    }
                });
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
                        // Direction = new Vector2(0.5f, 0.5f),
                    };
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

        internal void BakeToPosition(Vector2 p_worldPosition)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Baking grid");
            if (!IsPointInsideGrid(p_worldPosition))
            {
                UnityEngine.Profiling.Profiler.EndSample();
                Debug.LogError($"Position {p_worldPosition} is outside grid!");
                return;
            }

            Debug.Log($"Baking to position {p_worldPosition} {IsPointInsideGrid(p_worldPosition)}");

            var targetPosition = GetCellFromWorldPosition(p_worldPosition);

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
                            cell.StraightTargetDistance = delta.magnitude;
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
            UnityEngine.Profiling.Profiler.EndSample();
        }

        private void ForEachCellNeighbour(Vector2Int p_cellPosition, Action<GridCell> p_action)
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

                    if (p_cellPosition != new Vector2Int(x, y))
                    {
                        p_action.Invoke(_grid[x, y]);
                    }
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

    internal struct GridCell
    {
        public Vector2Int GridPosition;
        public Vector2 WorldPosition;
        public int Weight;
        public float StraightTargetDistance;
    }
}
