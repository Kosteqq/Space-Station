using UnityEngine;

namespace SpaceStation.AI
{
    public class TaskController : MonoBehaviour
    {
        private ITaskDispatcher _defaultDispatcher;
        private ITaskDispatcher _overridingDispatcher;
        
        private Task _currentTask;

        private void OnDestroy()
        {
            ReleaseCurrentTask();
        }

        public void SetDefaultDispatcher(ITaskDispatcher p_dispatcher)
        {
            _defaultDispatcher = p_dispatcher;
        }

        public void OverrideDispatcher(ITaskDispatcher p_dispatcher)
        {
            _overridingDispatcher = p_dispatcher;
        }

        public void ReleaseCurrentTask()
        {
            _currentTask?.Release();
            _currentTask = null;
        }
        
        public Task GetTask()
        {
            if (_currentTask != null)
            {
                return _currentTask;
            }
            
            var dispatcher = _overridingDispatcher == null ? _defaultDispatcher : _overridingDispatcher;
            _currentTask = dispatcher?.GetTask();

            if (_currentTask == null)
            {
                return null;
            }
            
            _currentTask.OnReleased += () => _currentTask = null;
            return _currentTask;
        }
    }
}