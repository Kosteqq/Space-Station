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
            Debug.Log($"Baking to position {p_worldPosition}");
        }

        internal GridCell[,] CreateGrid()
        {
            var sizeX = Mathf.FloorToInt(_gridBoundsSize.x / _gridCellSize);
            var sizeY = Mathf.FloorToInt(_gridBoundsSize.y / _gridCellSize);

            var minCorner = _gridBoundsCenter - _gridBoundsSize / 2f + new Vector2(_gridCellSize, _gridCellSize) / 2f;
            var grid = new GridCell[sizeX, sizeY];

            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
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
