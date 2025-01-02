using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace SpaceStation.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Assign Task Type",
        story: "Asign [TaskType] from [Task]",
        category: "Action",
        id: "e52e220abca14d083258a68bc3a6fce1")]
    public partial class AssignTaskTypeAction : Action
    {
        [SerializeReference] public BlackboardVariable<TaskType> TaskType;
        [SerializeReference] public BlackboardVariable<Task> Task;

        protected override Status OnStart()
        {
            if (Task.Value == null)
            {
                LogFailure("Task is null!", true);
                return Status.Failure;
            }

            TaskType.Value = Task.Value.Type;
            return Status.Success;
        }
    }
}    
