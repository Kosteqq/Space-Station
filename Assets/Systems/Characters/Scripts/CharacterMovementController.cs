using System.Collections.Generic;
using SpaceStation.PathFinding;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.Characters
{
    public class CharacterMovementController : MonoBehaviour
    {
        public Transform TargetPoint;
        public float Speed;
        public List<Vector2> PathPoints;

        private PathFindingManager _manager;
        private Vector3 _prevTargetPoint;
        private Vector2 _prevPoint;
        private Vector2 _destPoint;

        private void Start()
        {
            _manager = FindAnyObjectByType<PathFindingManager>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            for (var i = 0; i < PathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(
                    new Vector3(PathPoints[i].x, transform.position.y, PathPoints[i].y),
                    new Vector3(PathPoints[i + 1].x, transform.position.y, PathPoints[i + 1].y));
            }

            if (Application.isPlaying)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, new Vector3(_destPoint.x, transform.position.y, _destPoint.y));
            }
        }

        private void Update()
        {
            if (_prevTargetPoint != TargetPoint.transform.position && _manager != null)
            {
                _prevTargetPoint = TargetPoint.transform.position;
                PathPoints = _manager.GetPathToPoint(transform.position.XZ(), TargetPoint.transform.position.XZ());
                _prevPoint = PathPoints[0];
                _destPoint = PathPoints[0];
            }
            
            if (PathPoints.IsEmpty())
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
            if (PathPoints.IsEmpty()
                || PathPoints.IsLast(_prevPoint))
            {
                return transform.position.XZ();
            }

            var prevPointIndex = PathPoints.IndexOf(_prevPoint);
            return PathPoints[prevPointIndex + 1];
        }
    }
}
