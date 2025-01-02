using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace SpaceStation.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Assign Current Task",
        story: "Assign [task] from [controller]",
        category: "Action",
        id: "23ac507b9d6cf589c9dcd962ecfa4d7b")]
    public partial class AssignCurrentTaskAction : Action
    {
        [SerializeReference] public BlackboardVariable<Task> Task;
        [SerializeReference] public BlackboardVariable<TaskController> Controller;
        
        protected override Status OnStart()
        {
            if (Controller.Value == null)
            {
                LogFailure("Controller is null!", true);
                return Status.Failure;
            }
            
            Task.Value = Controller.Value.GetTask();
            
            return Status.Success;
        }
    }
}

