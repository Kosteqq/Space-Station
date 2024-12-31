using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace SpaceStation.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "CacheTaskController",
        story: "Cache [controller]",
        category: "Action",
        id: "5d480582bb77ead7bcd150cd2ea1fcd0")]
    public class CacheTaskControllerAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<TaskController> Controller;

        protected override Status OnStart()
        {
            if (Controller.Value != null)
            {
                return Status.Success;
            }

            if (Target.Value.TryGetComponent<TaskController>(out var controller))
            {
                Controller.Value = controller;
                return Status.Success;
            }
            
            return Status.Failure;
        }
    }
}
