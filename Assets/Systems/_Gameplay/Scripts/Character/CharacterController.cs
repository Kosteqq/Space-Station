using SpaceStation.Core;
using SpaceStation.Movement;
using SpaceStation.PathFinding;
using UnityEngine;

namespace SpaceStation.Gameplay.Character
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(PathFindingObjectController))]
    public class CharacterController : GameController
    {
        private MovementController _movementController;
        private PathFindingObjectController _pathFindingObjectController;

        private PathFindingManager _pathFindingManager; // TEMP

        public override void InitializeGame()
        {
             base.InitializeGame();
            
            _movementController = GetComponent<MovementController>();
            _pathFindingObjectController = GetComponent<PathFindingObjectController>();

            _pathFindingManager = GameManager.GetSystem<PathFindingManager>();
        }

        public override void StartGame()
        {
            base.StartGame();
        }

        public void MoveTo(PathFindingTarget p_target)
        {
            _pathFindingManager.BakeToTaret(p_target);
            _pathFindingObjectController.FindPath();
            _movementController.FollowPath(_pathFindingObjectController.Path);
        }
    }
}