using System;
using SpaceStation.AI;
using SpaceStation.AI.Goap;
using SpaceStation.Core;
using SpaceStation.EnviroAsset;
using SpaceStation.Movement;
using SpaceStation.PathFinding;
using SpaceStation.Utils;
using UnityEngine;

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
                .WithRunCallback(ctx => Debug.Log("Eat"))
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Go to table")
                .WithPrecondition<HasTableStateDefinition>(true)
                .WithPrecondition<IsByTableTargetStateDefinition>(false)
                .WithEffect<IsByTableTargetStateDefinition>(true)
                .WithEffect<IsByFoodTargetStateDefinition>(false)
                .WithRunCallback(ctx => Debug.Log("Go to table"))
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Find Table")
                .WithEffect<HasTableStateDefinition>(true)
                .WithRunCallback(ctx => Debug.Log("Find table"))
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Find food")
                // .WithPrecondition<HasOwnFoodStateDefinition>(false)
                .WithEffect<HasOwnFoodStateDefinition>(true)
                .WithRunCallback(ctx => Debug.Log("Find food"))
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Go to food")
                .WithPrecondition<HasOwnFoodStateDefinition>(true)
                .WithPrecondition<IsByFoodTargetStateDefinition>(false)
                .WithEffect<IsByTableTargetStateDefinition>(false)
                .WithEffect<IsByFoodTargetStateDefinition>(true)
                .WithRunCallback(ctx => Debug.Log("Go to food"))
                .Build();

            _aiController.SystemManager
                .BuildAction()
                .WithName("Take Food")
                .WithPrecondition<HasOwnFoodStateDefinition>(true)
                .WithPrecondition<IsByFoodTargetStateDefinition>(true)
                .WithEffect<HasFoodInHandStateDefinition>(true)
                .WithRunCallback(ctx => Debug.Log("Take Food"))
                .Build();

            _aiController.SystemManager.BuildGoal()
                .WithName("Eat critical")
                .WithActivationCondition<IsCriticalHungryStateDefinition>(true)
                .WithSatisfyCondition<IsCriticalHungryStateDefinition>(false)
                .WithPriority(10)
                .Build();
            
            _aiController.Blackboard.Set<IsCriticalHungryStateDefinition>(true);
            _aiController.CreatePlan();
        }

        private void Update()
        {
            // Hungry.Value += 10 * Time.deltaTime;
        }

        public MovementTask MoveTo(PathFindingTarget p_target)
        {
            _pathFindingManager.BakeToTaret(p_target);
            return _movementController.FollowPath(p_target.transform.position.XZ(), _pathFindingObjectController.FindPath());
        }
    }
}