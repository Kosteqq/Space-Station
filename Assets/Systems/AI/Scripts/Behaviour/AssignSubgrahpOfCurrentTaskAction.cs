using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace SpaceStation.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "AssignCurrentTask",
        story: "Assign [subgraph] from [controller] current task",
        category: "Action",
        id: "23ac507b9d6cf589c9dcd962ecfa4d7b")]
    public partial class AssignSubgrahpOfCurrentTaskAction : Action
    {
        [SerializeReference] public BlackboardVariable<BehaviorGraph> Subgraph;
        [SerializeReference] public BlackboardVariable<TaskController> Controller;
        
        protected override Status OnStart()
        {
            var task = Controller.Value.GetTask();
            Subgraph.Value = task.BehaviorGraph;
            
            return Status.Success;
        }
    }
}

