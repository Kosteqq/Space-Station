using SpaceStation.AI;
using System;
using SpaceStation.PathFinding;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace SpaceStation.Gameplay.Character
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Assign Idle Task Variables",
        story: "Assign Idle [Task] Variables",
        category: "Action",
        id: "69e20caba75140fe571d0b8d4205d0bc")]
    public partial class AssignIdleTaskVariablesAction : Action
    {
        [SerializeReference] public BlackboardVariable Task2;
        [SerializeReference] public BlackboardVariable<Task> Task;
        [SerializeReference] public BlackboardVariable<PathFindingTarget> Target;

        protected override Status OnStart()
        {
            if (Task.Value == null)
            {
                LogFailure("Task is null!", true);
                return Status.Failure;
            }

            if (Task.Value is not IdleTask idleTask)
            {
                LogFailure("Task is null!", true);
                return Status.Failure;
            }

            Target.Value = idleTask.Target;

            return Status.Success;
        }
    }
}

