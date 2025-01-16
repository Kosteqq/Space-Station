using System;
using System.Collections.Generic;
using SpaceStation.Core;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.Movement
{
    public class MovementTask
    {
        public enum Status
        {
            Undefined,
            Running,
            Success,
            Failure,
        }

        public readonly ObservableProperty<Status> CurrentStatus = new(Status.Undefined);
    }

    public class MovementController : SystemController<MovementManager>
    {
        private float _currentSpeed;
        
        private MovementTask _currentTask;
        private MovementTask _prevTask;
        private Queue<Vector2> _path;
        private float _stopDistance;

        private Vector2? _targetPoint;

        public MovementTask CurrentTask => _currentTask;
        public MovementTask PrevTask => _prevTask;

        public override void StartGame()
        {
            base.StartGame();
            
            _currentSpeed = SystemManager.DefaultSpeed;
        }

        public MovementTask FollowPath(Vector2 p_target, IReadOnlyCollection<Vector2> p_path)
        {
            return FollowPath(
                p_target,
                p_path,
                SystemManager.DefaultStopDistance);
        }
        
        public MovementTask FollowPath(Vector2 p_target, IReadOnlyCollection<Vector2> p_path, float p_stopDistance)
        {
            _path = new Queue<Vector2>(p_path);
            _stopDistance = p_stopDistance;

            if (_currentTask != null)
            {
                _prevTask = _currentTask;
                _currentTask.CurrentStatus.Value = MovementTask.Status.Failure;
            }
            
            _currentTask = new MovementTask();
            _currentTask.CurrentStatus.Value = MovementTask.Status.Running;

            if (_path == null || _path.IsEmpty())
            {
                Debug.LogError("The path is null or empty!");
                _currentTask.CurrentStatus.Value = MovementTask.Status.Failure;
            }

            return _currentTask;
        }

        private void Update()
        {
            if (!IsMovementFollowPath())
            {
                return;
            }

            if (!_targetPoint.HasValue)
            {
                if (!TryAcequireNextPoint())
                {
                    _currentTask.CurrentStatus.Value = MovementTask.Status.Success;
                    _prevTask = _currentTask;
                    _currentTask = null;
                    return;
                }
            }

            MoveToTargetPoint();
        }

        private bool IsMovementFollowPath()
        {
            return _currentTask != null && _currentTask.CurrentStatus.Value == MovementTask.Status.Running;
        }
        
        private bool TryAcequireNextPoint()
        {
             var success = _path.TryDequeue(out var point);
             _targetPoint = success ? point : null;

             return success;
        }

        private void MoveToTargetPoint()
        {
            var positionDelta = _targetPoint.Value - transform.position.XZ();
            var distanceToPoint = positionDelta.magnitude;
            var reachedPoint = distanceToPoint < _stopDistance;

            if (reachedPoint)
            {
                _targetPoint = null;
                return;
            }
            
            var movement = CalculateMoveDistance(positionDelta);
            transform.position += new Vector3(movement.x, 0f, movement.y);
        }
        
        private Vector2 CalculateMoveDistance(Vector2 p_positionDelta)
        {
            var distance = p_positionDelta.magnitude;
            var direction = p_positionDelta.normalized;
            
            var moveDistance = Mathf.Min(_currentSpeed * Time.deltaTime, distance);
                
            return direction * moveDistance;
        }

        private void OnDrawGizmosSelected()
        {
            if (!IsMovementFollowPath())
            {
                return;
            }

            Gizmos.color = Color.yellow;
            
            if (_targetPoint.HasValue)
            {
                Gizmos.DrawLine(transform.position, _targetPoint.Value.AsXZ(transform.position.y));
            }

            var prevPoint = _targetPoint;
            
            foreach (var point in _path)
            {
                if (prevPoint.HasValue)
                {
                    Gizmos.DrawLine(prevPoint.Value.AsXZ(transform.position.y), point.AsXZ(transform.position.y));
                }

                prevPoint = point;
            }
        }
    }
}
