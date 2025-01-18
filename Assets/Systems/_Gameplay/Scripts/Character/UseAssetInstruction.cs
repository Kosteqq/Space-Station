using System;
using SpaceStation.AI.Goap;
using SpaceStation.EnviroAsset;
using SpaceStation.Movement;
using UnityEngine;
using Action = SpaceStation.AI.Goap.Action;

namespace SpaceStation.Gameplay.Character
{
    public class EvnrioActions<T>
        where T : EnviroAssetController
    {
        // sensor
        [CustomStateName("Is_By_" + nameof(T) + "_Attribute")]
        public sealed class IsByOwnEnviroAssetState : BlackboardStateDefinition
        { }
        
        // world sensor
        [CustomStateName("Is_Any_" + nameof(T) + "_Asset_Available")]
        public sealed class IsAnyAvailableAssetState : BlackboardStateDefinition
        { }
        
        // sensor
        [CustomStateName("Is_Owning_" + nameof(T) + "_Asset")]
        public sealed class IsOwningAssetState : BlackboardStateDefinition
        { }
        
        // sensor
        [CustomStateName("Is_Carrying_" + nameof(T) + "_Asset")]
        public sealed class IsCarryingAssetState : BlackboardStateDefinition
        { }
        
        public static class UseAssetAction
        {
            public static Action.Status Start(in Action.Context p_context)
            {
                if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                    return Action.Status.Failure;
            
                if (!assetUser.TryOwnAsset<T>())
                    return Action.Status.Failure;
            
                return Action.Status.Success;
            }
        }

        public static class GoToAssetAction
        {
            public static Action.Status Start(in Action.Context p_context)
            {
                if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                    return Action.Status.Failure;

                if (!p_context.Object.TryGetComponent<GameplayCharacterController>(out var gameplayCharacter))
                    return Action.Status.Failure;

                var foodAsset = assetUser.GetOwninggAsset<T>();
                
                if (foodAsset == null)
                    return Action.Status.Failure;

                var movementTask = gameplayCharacter.MoveTo(foodAsset.PathFindingTarget);

                var status = GetStatusFromMovement(movementTask.CurrentStatus.Value);

                return status;
            }

            public static Action.Status Run(in Action.Context p_context)
            {
                if (!p_context.Object.TryGetComponent<MovementController>(out var movementController))
                    return Action.Status.Failure;

                var movementTask = movementController.CurrentTask;
                movementTask ??= movementController.PrevTask;

                var status = GetStatusFromMovement(movementTask.CurrentStatus.Value);

                return status;
            }

            private static Action.Status GetStatusFromMovement(MovementTask.Status p_status)
            {
                switch (p_status)
                {
                    case MovementTask.Status.Success:
                        return Action.Status.Success;
                    
                    case MovementTask.Status.Running:
                        return Action.Status.Running;
                    
                    default:
                    case MovementTask.Status.Undefined:
                    case MovementTask.Status.Failure:
                        return Action.Status.Failure;
                }
            }
        }

        public static class CarryAssetAction
        {
            public static Action.Status Start(in Action.Context p_context)
            {
                if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                    return Action.Status.Failure;
            
                if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                    return Action.Status.Failure;

                var asset = assetUser.GetOwninggAsset<T>();
            
                if (asset == null)
                    return Action.Status.Failure;

                // Set in by sensor/monitor
                aiController.Blackboard.Set<IsCarryingAssetState>(true);
            
                asset.transform.SetParent(p_context.Object.transform);
                asset.transform.localPosition = Vector3.zero;
                return Action.Status.Success;
            }
        }

        public static class DropAssetAction
        {
            public static Action.Status Start(in Action.Context p_context)
            {
                if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                    return Action.Status.Failure;
            
                if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                    return Action.Status.Failure;

                var asset = assetUser.GetOwninggAsset<T>();
            
                if (asset == null)
                    return Action.Status.Failure;

                // Set in by sensor/monitor
                aiController.Blackboard.Set<IsCarryingAssetState>(false);
            
                asset.transform.SetParent(null, true);
                return Action.Status.Success;
            }
        }

        public static class PutAssetAction
        {
        }
        
        public static void Register(AiManager p_manager, bool p_usable)
        {
            p_manager.BuildAction()
                .WithName($"Own {typeof(T).Name} Asset")
                .WithPrecondition<IsAnyAvailableAssetState>(true)
                .WithEffect<IsOwningAssetState>(true)
                .WithStartCallback(UseAssetAction.Start)
                .Build();
            
            p_manager.BuildAction()
                .WithName($"Go To {typeof(T).Name} Asset")
                .WithPrecondition<IsOwningAssetState>(true)
                .WithEffect<IsByOwnEnviroAssetState>(true)
                // .WithTempEffect<IsByEnviroAssetState>(true)
                .WithStartCallback(GoToAssetAction.Start)
                .WithRunCallback(GoToAssetAction.Run)
                .Build();
            
            p_manager.BuildAction()
                .WithName($"Carry {typeof(T).Name} Asset")
                .WithPrecondition<IsOwningAssetState>(true)
                .WithPrecondition<IsByOwnEnviroAssetState>(true)
                .WithEffect<IsCarryingAssetState>(true)
                .WithStartCallback(CarryAssetAction.Start)
                .Build();
            
            p_manager.BuildAction()
                .WithName($"Drop {typeof(T).Name} Asset")
                .WithPrecondition<IsOwningAssetState>(true)
                .WithPrecondition<IsCarryingAssetState>(true)
                .WithEffect<IsCarryingAssetState>(false)
                .WithStartCallback(CarryAssetAction.Start)
                .Build();

            if (p_usable)
            {
                p_manager.BuildAction()
                    .WithName($"Use {typeof(T).Name} Asset")
                    .WithPrecondition<IsOwningAssetState>(true)
                    .WithPrecondition<IsByOwnEnviroAssetState>(true)
                    .WithPrecondition<EvnrioActions<FoodEnviroAsset>.IsCarryingAssetState>(true)
                    .WithEffect<IsCriticalHungryStateDefinition>(false)
                    .WithEffect<IsHungryStateDefinition>(false)
                    .WithStartCallback((in Action.Context p_context) =>
                    {
                        if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                            return Action.Status.Failure;
            
                        if (!p_context.Object.TryGetComponent<GameplayCharacterController>(out var gameplayCharacter))
                            return Action.Status.Failure;

                        var food = assetUser.GetOwninggAsset<FoodEnviroAsset>();
                        assetUser.RemoveOwn(food);
            
                        if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                            return Action.Status.Failure;
                        
                        aiController.Blackboard.Set<EvnrioActions<FoodEnviroAsset>.IsCarryingAssetState>(false);
                        
                        UnityEngine.Object.Destroy(food);
                        assetUser.RemoveAllOwnings();
                        gameplayCharacter.Hungry.Value -= 20;
                        return Action.Status.Success;
                    })
                    .Build();
            }
        }
    }
}