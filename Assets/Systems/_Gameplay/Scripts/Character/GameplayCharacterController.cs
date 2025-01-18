using System;
using System.Collections.Generic;
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
    [CustomStateName("Is_Critical_Hungry")]
    public sealed class IsCriticalHungryStateDefinition : BlackboardStateDefinition
    {
    }

    public class Sensor
    {
        private Action<bool> _setAction; 
        private Func<bool> _getAction;
        public Sensor(Action<bool> p_setAction, Func<bool> p_getAction)
        {
            _setAction = p_setAction;
            _getAction = p_getAction;
        }

        public void Update()
        {
            _setAction(_getAction());
        }
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

        private List<Sensor> _sensors = new();

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
            
            EvnrioActions<FoodEnviroAsset>.Register(_aiController.SystemManager, false);
            EvnrioActions<EatPlaceEnviroAsset>.Register(_aiController.SystemManager, true);

            _aiController.SystemManager.BuildGoal()
                .WithName("Eat critical")
                .WithActivationCondition<EvnrioActions<EatPlaceEnviroAsset>.IsAnyAvailableAssetState>(true)
                .WithActivationCondition<EvnrioActions<FoodEnviroAsset>.IsAnyAvailableAssetState>(true)
                .WithActivationCondition<IsCriticalHungryStateDefinition>(true)
                .WithSatisfyCondition<IsCriticalHungryStateDefinition>(false)
                .WithPriority(10)
                .Build();

            _aiController.SystemManager.BuildGoal()
                .WithName("Idle")
                .WithPriority(-10)
                .Build();

            _sensors.Add(
                new Sensor(
                    _aiController.Blackboard.Set<EvnrioActions<FoodEnviroAsset>.IsByOwnEnviroAssetState>,
                    () =>
                    {
                        if (!_enviroAssetUserController.IsOwningAsset<FoodEnviroAsset>())
                            return false;

                        return Vector3.Distance(_enviroAssetUserController.GetOwninggAsset<FoodEnviroAsset>().transform.position, transform.position) < 0.5f;
                    }));
            
            _sensors.Add(
                new Sensor(
                    _aiController.Blackboard.Set<EvnrioActions<FoodEnviroAsset>.IsAnyAvailableAssetState>,
                    _enviroAssetUserController.SystemManager.IsAnyAssetAvaiable<FoodEnviroAsset>));
            
            _sensors.Add(
                new Sensor(
                    _aiController.Blackboard.Set<EvnrioActions<FoodEnviroAsset>.IsOwningAssetState>,
                    _enviroAssetUserController.IsOwningAsset<FoodEnviroAsset>));
            
            _sensors.Add(
                new Sensor(
                    _aiController.Blackboard.Set<EvnrioActions<EatPlaceEnviroAsset>.IsByOwnEnviroAssetState>,
                    () =>
                    {
                        if (!_enviroAssetUserController.IsOwningAsset<EatPlaceEnviroAsset>())
                            return false;

                        return Vector3.Distance(_enviroAssetUserController.GetOwninggAsset<EatPlaceEnviroAsset>().transform.position, transform.position) < 0.5f;
                    }));
            
            _sensors.Add(
                new Sensor(
                    _aiController.Blackboard.Set<EvnrioActions<EatPlaceEnviroAsset>.IsAnyAvailableAssetState>,
                    _enviroAssetUserController.SystemManager.IsAnyAssetAvaiable<EatPlaceEnviroAsset>));
            
            _sensors.Add(
                new Sensor(
                    _aiController.Blackboard.Set<EvnrioActions<EatPlaceEnviroAsset>.IsOwningAssetState>,
                    _enviroAssetUserController.IsOwningAsset<EatPlaceEnviroAsset>));
            
            _aiController.Blackboard.Set<IsCriticalHungryStateDefinition>(true);
            _aiController.CreatePlan();
        }

        private Action.Status UseStation(in Action.Context p_context)
        {
            if (!p_context.Object.TryGetComponent<EnviroAssetUserController>(out var assetUser))
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<GameplayCharacterController>(out var gameplayCharacter))
                return Action.Status.Failure;
            
            if (!p_context.Object.TryGetComponent<AiController>(out var aiController))
                return Action.Status.Failure;
            
            // aiController.Blackboard.Set<HasFoodInHandStateDefinition>(false);
            // aiController.Blackboard.Set<IsByTableTargetStateDefinition>(false);
            // aiController.Blackboard.Set<IsByFoodTargetStateDefinition>(false);
            // aiController.Blackboard.Set<HasOwnFoodStateDefinition>(false);
            // aiController.Blackboard.Set<HasTableStateDefinition>(false);

            var food = assetUser.GetOwninggAsset<FoodEnviroAsset>();
            assetUser.RemoveOwn(food);
            Destroy(food);
 
            assetUser.RemoveAllOwnings();
            gameplayCharacter.Hungry.Value -= 20;
            return Action.Status.Success;
        }

        private void Update()
        {
            foreach (var sensor in _sensors)
            {
                sensor.Update();
            }
            Hungry.Value += 10 * Time.deltaTime;
        }

        public MovementTask MoveTo(PathFindingTarget p_target)
        {
            _pathFindingManager.BakeToTaret(p_target);
            return _movementController.FollowPath(p_target.transform.position.XZ(), _pathFindingObjectController.FindPath());
        }
    }
}