using UnityEngine;

namespace SpaceStation.AI
{
    public class TaskDispatcher : MonoBehaviour
    {
        [SerializeField]
        private Task _task;
        
        public Task GetTask()
        {
            var instance = Instantiate(_task);
            
            instance.OnReleased += () => Debug.Log("Task released");
            instance.OnReleased += () => Destroy(instance);
            
            return instance;
        }
    }
}