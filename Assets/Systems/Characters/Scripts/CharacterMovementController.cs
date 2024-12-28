using System.Collections.Generic;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.Characters
{
    public class CharacterMovementController : MonoBehaviour
    {
        public float _speed;
        public List<Vector2> _pathPoints;

        private Vector2 _prevPoint;
        private Vector2 _destPoint;

        private void Awake()
        {
            _prevPoint = _pathPoints[0];
            _destPoint = _pathPoints[0];
            transform.position = new Vector3(_pathPoints[0].x, transform.position.y, _pathPoints[0].y);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            for (var i = 0; i < _pathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(
                    new Vector3(_pathPoints[i].x, 0f, _pathPoints[i].y),
                    new Vector3(_pathPoints[i + 1].x, 0f, _pathPoints[i + 1].y));
            }

            if (Application.isPlaying)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, new Vector3(_destPoint.x, transform.position.y, _destPoint.y));
            }
        }

        private void Update()
        {
            if (transform.position.XZ() == _destPoint)
            {
                _prevPoint = _destPoint;
                _destPoint = GetNextDestPoint();
            }

            var delta = _destPoint - transform.position.XZ();
            var distance = delta.magnitude;

            var speed = _speed * Time.deltaTime;
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
