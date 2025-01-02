using System.Collections.Generic;
using SpaceStation.Core;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.PathFinding
{
    public class PathFindingObjectController : SystemController<PathFindingManager>
    {
        public List<Vector2> FindPath()
        {
            return SystemManager.GetPathToTarget(this);
        }
    }
}