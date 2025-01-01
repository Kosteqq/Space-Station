using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace SpaceStation.Core
{
    [Serializable, GeneratePropertyBag]
    public abstract partial class CacheControllerAction<TController> : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<TController> Controller;

        protected override Status OnStart()
        {
            if (Controller.Value != null)
            {
                return Status.Success;
            }

            Debug.Log(typeof(TController).Name);

            if (Target.Value.TryGetComponent<TController>(out var controller))
            {
                Controller.Value = controller;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}
