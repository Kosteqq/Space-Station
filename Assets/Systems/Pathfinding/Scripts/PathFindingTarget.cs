using UnityEngine;

namespace SpaceStation.PathFinding
{
    public class PathFindingTarget : MonoBehaviour
    {
        private void Start()
        {
            var manager = FindAnyObjectByType<PathFindingManager>();
            manager.RegisterTarget(this);
        }
    }
}