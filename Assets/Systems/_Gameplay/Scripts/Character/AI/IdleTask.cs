using SpaceStation.AI;
using SpaceStation.PathFinding;

namespace SpaceStation.Gameplay.Character
{
    public abstract class IdleTask : Task
    {
        public sealed override TaskType Type => TaskType.Idle;
        
        public abstract PathFindingTarget Target { get; set; }
    }
}