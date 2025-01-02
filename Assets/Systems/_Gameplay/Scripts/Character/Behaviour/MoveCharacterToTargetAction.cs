using SpaceStation.PathFinding;
using System;
using SpaceStation.Movement;
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
        [SerializeReference] public BlackboardVariable<GameplayCharacterController> CharacterController;
        [SerializeReference] public BlackboardVariable<PathFindingTarget> Target;

        private MovementTask _task;
        
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
            
            _task = CharacterController.Value.MoveTo(Target.Value);
            
            return GetStatusFromTask(_task);
        }

        protected override Status OnUpdate()
        {
            return GetStatusFromTask(_task);
        }

        private static Status GetStatusFromTask(MovementTask p_task)
        {
            switch (p_task.CurrentStatus.Value)
            {
                case MovementTask.Status.Running:
                    return Status.Running;
                case MovementTask.Status.Success:
                    return Status.Success;
                case MovementTask.Status.Undefined:
                case MovementTask.Status.Failure:
                default:
                    return Status.Failure;
            }
        }
    }
}
