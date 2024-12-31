using Unity.Behavior;
using UnityEngine;
using Action = System.Action;

namespace SpaceStation.AI
{
    [CreateAssetMenu(menuName = "Create Task", fileName = "Task", order = 0)]
    public class Task : ScriptableObject
    {
        [SerializeField]
        private BehaviorGraph _behaviorGraph;
        
        public BehaviorGraph BehaviorGraph { get; private set; }

        public event Action OnReleased;
        
        private void Awake()
        {
            if (Application.isPlaying)
            {
                BehaviorGraph = Instantiate(_behaviorGraph);
            }
        }

        public void Release()
        {
            // Success, Failure??
            OnReleased?.Invoke();
        }
    }
}