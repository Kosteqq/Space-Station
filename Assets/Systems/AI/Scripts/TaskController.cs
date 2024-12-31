using UnityEngine;

namespace SpaceStation.AI
{
    public class TaskController : MonoBehaviour
    {
        [SerializeField]
        private TaskDispatcher _defaultDispatcher;
        private TaskDispatcher _overridingDispatcher;
        
        private Task _currentTask;

        private void OnDestroy()
        {
            ReleaseCurrentTask();
        }

        public void ReleaseCurrentTask()
        {
            _currentTask?.Release();
            _currentTask = null;
        }

        public void ChangeOverridingDispatcher(TaskDispatcher p_dispatcher)
        {
            _overridingDispatcher = p_dispatcher;
        }
        
        public Task GetTask()
        {
            if (_currentTask == null)
            {
                var dispatcher = _overridingDispatcher == null ? _defaultDispatcher : _overridingDispatcher;
                
                _currentTask = dispatcher?.GetTask();
                
                if (_currentTask != null)
                {
                    _currentTask.OnReleased += () => _currentTask = null;
                }
            }
            
            return _currentTask;
        }
    }
}