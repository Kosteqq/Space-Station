using System;
using SpaceStation.AI;
using SpaceStation.AI.Goap;
using SpaceStation.Core;
using SpaceStation.EnviroAsset;
using SpaceStation.Movement;
using SpaceStation.PathFinding;
using SpaceStation.Utils;
using UnityEngine;
using Action = SpaceStation.AI.Goap.Action;

namespace SpaceStation.Gameplay.Character
{
    public sealed class IsCriticalHungryStateDefinition : BlackboardStateDefinition
    {
        public override string Name => "Is_Critical_Hungry";
    }
    
    public sealed class HasTableStateDefinition : BlackboardStateDefinition
    {
        public override string Name => "Has_Table";
    }
    
    public sealed class IsByTableTargetStateDefinition : BlackboardStateDefinition
    {
        public override string Name => "Is_By_Table";
    }
    
    public sealed class HasOwnFoodStateDefinition : BlackboardStateDefinition
    {
        public override string Name => "Has_Own_Food";
    }
    
    public sealed class HasFoodInHandStateDefinition : BlackboardStateDefinition
    {
        public override string Name => "Has_Food_In_Hand";
    }
    
    public sealed class IsByFoodTargetStateDefinition : BlackboardStateDefinition
    {
        public override string Name => "Is_By_Food_Target";
    }

    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(PathFindingObjectController))]
    [RequireComponent(typeof(TaskController))]
    [RequireComponent(typeof(EnviroAssetUserController))]
    [RequireComponent(typeof(AiController))]
    public class GameplayCharacterController : SystemController<GameplayCharactersManager>
    {
        private MovementController _movementController;
        private PathFindingObjectController _pathFindingObjectController;
        private TaskController _taskController;
        private EnviroAssetUserController _enviroAssetUserController;
        private AiController _aiController;

        private PathFindingManager _pathFindingManager; // TEMP

        public ObservableProperty<float> Hungry;

        public override void InitializeGame()
        {
             base.InitializeGame();
            
            _movementController = GetComponent<MovementController>();
            _pathFindingObjectController = GetComponent<PathFindingObjectController>();
            _taskController = GetComponent<TaskController>();
            _enviroAssetUserController = GetComponent<EnviroAssetUserController>();
            _aiController = GetComponent<AiController>();
            
            _pathFindingManager = GameManager.GetSystem<PathFindingManager>();
            
            _taskController.SetDefaultDispatcher(SystemManager.DefaultAiTaskDispatcher);

            Hungry = new();
            Hungry.OnValueChange += value =>
            {
                _aiController.Blackboard.Set<IsCriticalHungryStateDefinition>(value >= 55);
            };
        }

        public override void StartGame()
        {
            base.StartGame();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Fast Eat")
                .WithPrecondition<HasFoodInHandStateDefinition>(true)
                .WithPrecondition<IsByTableTargetStateDefinition>(true)
                .WithEffect<IsCriticalHungryStateDefinition>(false)
                .WithStartCallback(Eat)
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Go to table")
                .WithPrecondition<HasTableStateDefinition>(true)
                .WithPrecondition<IsByTableTargetStateDefinition>(false)
                .WithEffect<IsByTableTargetStateDefinition>(true)
                .WithEffect<IsByFoodTargetStateDefinition>(false)
                .WithStartCallback(StartGoToAsset<EatPlaceEnviroAsset>)
                .WithRunCallback(UpdateGoToAsset<EatPlaceEnviroAsset>)
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Find Table")
                .WithEffect<HasTableStateDefinition>(true)
                .WithStartCallback(FindAsset<EatPlaceEnviroAsset>)
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Find food")
                // .WithPrecondition<HasOwnFoodStateDefinition>(false)
                .WithEffect<HasOwnFoodStateDefinition>(true)
                .WithStartCallback(FindAsset<FoodEnviroAsset>)
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Go to food")
                .WithPrecondition<HasOwnFoodStateDefinition>(true)
                .WithPrecondition<IsByFoodTargetStateDefinition>(false)
                .WithEffect<IsByTableTargetStateDefinition>(false)
                .WithEffect<IsByFoodTargetStateDefinition>(true)
                .WithStartCallback(StartGoToAsset<FoodEnviroAsset>)
                .WithRunCallback(UpdateGoToAsset<FoodEnviroAsset>)
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Take Food")
                .WithPrecondition<HasOwnFoodStateDefinition>(true)
                .WithPrecondition<IsByFoodTargetStateDefinition>(true)
                .WithEffect<HasFoodInHandStateDefinition>(true)
                .WithRunCallback(TakeFood)
                .Build();

            _aiController.SystemManager.BuildGoal()
                .WithName("Eat critical")
                .WithActivationCondition<IsCriticalHungryStateDefinition>(true)
                .WithSatisfyCondition<IsCriticalHungryStateDefinition>(false)
                .WithPriority(10)
                .Build();

            _aiController.SystemManager.BuildGoal()
                .WithName("Idle")
                .WithPriority(-10)
                .Build();
            
            _aiController.Blackboard.Set<IsCriticalHungryStateDefinition>(true);
            _aiController.CreatePlan();
        }

        private static Action.Status FindAsset<TAsset>(in Action.Context p_context)
            where TAsset : EnviroAssetController
        {
            if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                return Action.Status.Failure;
            
            if (!assetUser.TryUseAsset<TAsset>())
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                return Action.Status.Failure;
            
            
            if (typeof(TAsset) == typeof(EatPlaceEnviroAsset))
                aiController.Blackboard.Set<HasTableStateDefinition>(true);
            if (typeof(TAsset) == typeof(FoodEnviroAsset))
                aiController.Blackboard.Set<HasOwnFoodStateDefinition>(true);
            
            return Action.Status.Success;
        }

        private Action.Status StartGoToAsset<TAsset>(in Action.Context p_context)
            where TAsset : EnviroAssetController
        {
            if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<GameplayCharacterController>(out var gameplayCharacter))
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                return Action.Status.Failure;

            var foodAsset = assetUser.GetUsingAsset<TAsset>();
            if (foodAsset == null)
                return Action.Status.Failure;

            var movementTask = gameplayCharacter.MoveTo(foodAsset.PathFindingTarget);

            switch (movementTask.CurrentStatus.Value)
            {
                case MovementTask.Status.Running:
                    return Action.Status.Running;
                case MovementTask.Status.Success:
                    if (typeof(TAsset) == typeof(EatPlaceEnviroAsset))
                        aiController.Blackboard.Set<IsByTableTargetStateDefinition>(true);
                    if (typeof(TAsset) == typeof(FoodEnviroAsset))
                        aiController.Blackboard.Set<IsByFoodTargetStateDefinition>(true);
                    return Action.Status.Success;
                case MovementTask.Status.Undefined:
                case MovementTask.Status.Failure:
                default:
                    return Action.Status.Failure;
            }
        }

        private Action.Status UpdateGoToAsset<TAsset>(in Action.Context p_context)
            where TAsset : EnviroAssetController
        {
            if (!p_context.Object.TryGetComponent<MovementController>(out var movementController))
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                return Action.Status.Failure;

            var movementTask = movementController.CurrentTask;
            movementTask ??= movementController.PrevTask;


            switch (movementTask.CurrentStatus.Value)
            {
                case MovementTask.Status.Running:
                    return Action.Status.Running;
                case MovementTask.Status.Success:
                    if (typeof(TAsset) == typeof(EatPlaceEnviroAsset))
                        aiController.Blackboard.Set<IsByTableTargetStateDefinition>(true);
                    if (typeof(TAsset) == typeof(FoodEnviroAsset))
                        aiController.Blackboard.Set<IsByFoodTargetStateDefinition>(true);
                    return Action.Status.Success;
                case MovementTask.Status.Undefined:
                case MovementTask.Status.Failure:
                default:
                    return Action.Status.Failure;
            }
        }

        private Action.Status TakeFood(in Action.Context p_context)
        {
            if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                return Action.Status.Failure;

            var foodAsset = assetUser.GetUsingAsset<FoodEnviroAsset>();
            
            if (foodAsset == null)
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                return Action.Status.Failure;

            aiController.Blackboard.Set<HasFoodInHandStateDefinition>(true);
            
            foodAsset.transform.SetParent(gameObject.transform);
            foodAsset.transform.localPosition = Vector3.zero;
            return Action.Status.Success;
        }

        private Action.Status Eat(in Action.Context p_context)
        {
            if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<GameplayCharacterController>(out var gameplayCharacter))
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                return Action.Status.Failure;
            
            aiController.Blackboard.Set<HasFoodInHandStateDefinition>(false);
            aiController.Blackboard.Set<IsByTableTargetStateDefinition>(false);
            aiController.Blackboard.Set<IsByFoodTargetStateDefinition>(false);
            aiController.Blackboard.Set<HasOwnFoodStateDefinition>(false);
            aiController.Blackboard.Set<HasTableStateDefinition>(false);

            var food = assetUser.GetUsingAsset<FoodEnviroAsset>();
            assetUser.RemoveUsage(food);
            Destroy(food);
 
            assetUser.RemoveAllUsage();
            gameplayCharacter.Hungry.Value -= 20;
            return Action.Status.Success;
        }

        private void Update()
        {
            Hungry.Value += 10 * Time.deltaTime;
        }

        public MovementTask MoveTo(PathFindingTarget p_target)
        {
            _pathFindingManager.BakeToTaret(p_target);
            return _movementController.FollowPath(p_target.transform.position.XZ(), _pathFindingObjectController.FindPath());
        }
    }
}