using System.Collections.Generic;
using SpaceStation.Core;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.PathFinding
{
    public class PathFindingObjectController : SystemSubcontroller<PathFindingManager>
    {
        private List<Vector2> _path;

        public List<Vector2> Path => _path;

        public void FindPath()
        {
            _path = SystemManager.GetPathToTarget(this);
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