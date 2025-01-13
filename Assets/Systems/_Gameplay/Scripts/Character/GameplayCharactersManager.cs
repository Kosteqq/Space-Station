using SpaceStation.AI;
using SpaceStation.Core;
using SpaceStation.PathFinding;
using UnityEngine;

namespace SpaceStation.Gameplay.Character
{
    public class GameplayCharactersManager : GameSystemManager
    {
        [SerializeField]
        private PathFindingTarget[] _idleTargets;
        
        public ITaskDispatcher DefaultAiTaskDispatcher { get; private set; }
        
        public override void Initialize()
        {
            DefaultAiTaskDispatcher = new IdleAiTaskDispatcher(_idleTargets);
        }
    }

    public class IdleAiTaskDispatcher : ITaskDispatcher
    {
        private readonly PathFindingTarget[] _idleTargets;
        
        internal IdleAiTaskDispatcher(PathFindingTarget[] p_idleTargets)
        {
            _idleTargets = p_idleTargets;
        }
        
        public Task GetTask()
        {
            var task = ScriptableObject.CreateInstance<DefaultIdleTask>();

            task.Target = _idleTargets[Random.Range(0, _idleTargets.Length)];
            // task.OnReleased += () => Object.Destroy(task);
            
            return task;
        }
    }

    public sealed class DefaultIdleTask : IdleTask
    {
        public override PathFindingTarget Target { get; set; }
    }
}