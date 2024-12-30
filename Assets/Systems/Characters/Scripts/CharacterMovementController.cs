using System.Collections.Generic;
using SpaceStation.Core;
using SpaceStation.PathFinding;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.Characters
{
    public class CharacterMovementController : GameController
    {
        private PathFindingManager _pathFindingManager;
        private PathFindingObjectController _pathFindingController;
        
        public PathFindingTarget TargetPoint;
        public float Speed;

        private List<Vector2> _pathPoints;
        private Vector3 _prevTargetPoint;
        private Vector2 _prevPoint;
        private Vector2 _destPoint;

        public override void StartGame()
        {
            _pathFindingManager = FindAnyObjectByType<PathFindingManager>();
            _pathFindingController = GetComponent<PathFindingObjectController>();
        }

        private void Update()
        {
            if (_prevTargetPoint != TargetPoint.transform.position && _pathFindingManager != null)
            {
                _prevTargetPoint = TargetPoint.transform.position;
                _pathFindingManager.BakeToTaret(TargetPoint);
                _pathFindingController.FindPath();
                _pathPoints = new List<Vector2>(_pathFindingController.Path);
                _prevPoint = _pathPoints[0];
                _destPoint = _pathPoints[0];
            }
            
            if (_pathPoints.IsEmpty())
            {
                return;
            }
            
            if (transform.position.XZ() == _destPoint)
            {
                _prevPoint = _destPoint;
                _destPoint = GetNextDestPoint();
            }

            var delta = _destPoint - transform.position.XZ();
            var distance = delta.magnitude;

            var speed = Speed * Time.deltaTime;
            speed = Mathf.Min(speed, distance);

            Debug.Log(speed);

            delta = delta.normalized;
            transform.position += new Vector3(delta.x, 0f, delta.y) * speed;
        }

        private Vector2 GetNextDestPoint()
        {
            if (_pathPoints.IsEmpty()
                || _pathPoints.IsLast(_prevPoint))
            {
                return transform.position.XZ();
            }

            var prevPointIndex = _pathPoints.IndexOf(_prevPoint);
            return _pathPoints[prevPointIndex + 1];
        }
    }
}
