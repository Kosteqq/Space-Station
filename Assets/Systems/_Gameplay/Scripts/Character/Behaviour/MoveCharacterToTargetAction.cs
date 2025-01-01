using SpaceStation.PathFinding;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace SpaceStation.Gameplay.Character
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Move Chatacter To Target",
        story: "Set [CharacterController] [Target]",
        category: "Action",
        id: "b3b0b1d25c70105216ef7babd957eeb7")]
    public partial class MoveCharacterToTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<CharacterController> CharacterController;
        [SerializeReference] public BlackboardVariable<PathFindingTarget> Target;

        protected override Status OnStart()
        {
            if (CharacterController.Value == null)
            {
                LogFailure("Failed move to target. The character controller is null!!", true);
                return Status.Failure;
            }
            
            if (Target.Value == null)
            {
                LogFailure("Failed move to target. The Target is null!!", true);
                return Status.Failure;
            }
            
            CharacterController.Value.MoveTo(Target.Value);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return Status.Running;
        }
    }
}
