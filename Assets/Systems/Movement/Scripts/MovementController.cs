using System.Collections.Generic;
using SpaceStation.Core;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.Movement
{
    public class MovementController : GameController
    {
        public float Speed;
        
        public bool Success { get; private set; }

        private List<Vector2> _pathPoints;
        private Vector3 _prevTargetPoint;
        private Vector2 _prevPoint;
        private Vector2 _destPoint;

        public override void InitializeGame()
        {
            base.InitializeGame();
        }

        public override void StartGame()
        {
            base.StartGame();
            
            Success = false;
            _pathPoints = new List<Vector2>();
        }

        public void FollowPath(List<Vector2> p_points)
        {
            _pathPoints = p_points;
            _prevPoint = _pathPoints[0];
            _destPoint = _pathPoints[0];
            Success = false;
        }

        private void Update()
        {
            if (_pathPoints.IsEmpty() || Success)
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

            if (distance == 0)
            {
                Success = true;
                return;
            }

            var speed = Speed * Time.deltaTime;
            speed = Mathf.Min(speed, distance);

            // Debug.Log(speed);

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
