using SpaceStation.AI;
using SpaceStation.Core;
using SpaceStation.Movement;
using SpaceStation.PathFinding;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.Gameplay.Character
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(PathFindingObjectController))]
    [RequireComponent(typeof(TaskController))]
    public class GameplayCharacterController : SystemController<GameplayCharactersManager>
    {
        private MovementController _movementController;
        private PathFindingObjectController _pathFindingObjectController;
        private TaskController _taskController;

        private PathFindingManager _pathFindingManager; // TEMP

        public override void InitializeGame()
        {
             base.InitializeGame();
            
            _movementController = GetComponent<MovementController>();
            _pathFindingObjectController = GetComponent<PathFindingObjectController>();
            _taskController = GetComponent<TaskController>();
            
            _pathFindingManager = GameManager.GetSystem<PathFindingManager>();
            
            _taskController.SetDefaultDispatcher(SystemManager.DefaultAiTaskDispatcher);
        }

        public override void StartGame()
        {
            base.StartGame();
        }

        public MovementTask MoveTo(PathFindingTarget p_target)
        {
            _pathFindingManager.BakeToTaret(p_target);
            return _movementController.FollowPath(p_target.transform.position.XZ(), _pathFindingObjectController.FindPath());
        }
    }
}