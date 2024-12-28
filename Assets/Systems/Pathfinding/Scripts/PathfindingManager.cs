using System;
using System.Collections.Generic;
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
        
        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        internal void BakeToPosition(Vector3 p_worldPosition)
        {
            Debug.Log($"Baking to position {p_worldPosition} {IsPointInsideGrid(p_worldPosition)}");
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

        private Vector2Int GetGridCellSize()
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
            var size = GetGridCellSize();
            var corner = GetMinCorner();

            return corner + (Vector2)size * _gridCellSize;
        }

        internal GridCell[,] CreateGrid()
        {
            var size = GetGridCellSize();

            var minCorner = GetMinCorner() + new Vector2(_gridCellSize, _gridCellSize) / 2f;
            var grid = new GridCell[size.x, size.y];

            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    grid[x, y] = new GridCell
                    {
                        GridPosition = new Vector2(x, y),
                        WorldPosition = minCorner + new Vector2(x, y) * _gridCellSize,
                        Direction = new Vector2(0.5f, 0.5f),
                    };
                }
            }

            return grid;
        }
    }

    internal struct GridCell
    {
        public Vector2 GridPosition;
        public Vector2 WorldPosition;
        public Vector2 Direction;
    }
}
