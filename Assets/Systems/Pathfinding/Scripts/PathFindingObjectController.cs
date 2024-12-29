using System.Collections.Generic;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.PathFinding
{
    public class PathFindingObjectController : MonoBehaviour
    {
        private PathFindingManager _manager;
        private List<Vector2> _path;

        public List<Vector2> Path => _path;
        
        internal void Start()
        {
            _manager = FindAnyObjectByType<PathFindingManager>();
        }

        public void FindPath()
        {
            _path = _manager.GetPathToTarget(this);
        }

        private void OnDrawGizmos()
        {
            if (_path == null)
            {
                return;
            }
            
            Gizmos.color = Color.yellow;

            for (var i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawLine(
                    new Vector3(_path[i].x, 0f, _path[i].y),
                    new Vector3(_path[i + 1].x, 0f, _path[i + 1].y));
            }
        }
    }
}