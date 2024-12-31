using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace SpaceStation.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "ReleaseCurrentTask",
        story: "Release [controller] current task",
        category: "Action",
        id: "2017eeaaf7420949b0462111275bb1cf")]
    public partial class ReleaseCurrentTaskAction : Action
    {
        [SerializeReference] public BlackboardVariable<TaskController> Controller;

        protected override Status OnStart()
        {
            Controller.Value.ReleaseCurrentTask();
            return Status.Success;
        }
    }
}

