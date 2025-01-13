using System.Collections.Generic;
using SpaceStation.Core;

namespace SpaceStation.AI.Goap
{
    public class AiManager : GameSystemManager
    {
        private BlackboardStateFactory _stateFactory;
        private ActionsController _actionsController;
        private GoalsController _goalsController;
        private AiPlanner _planner;
        
        public BlackboardStateFactory StateFactory => _stateFactory;
        internal AiPlanner Planner => _planner;
        
        public override void Initialize()
        {
            _stateFactory = new BlackboardStateFactory();
            _actionsController = new ActionsController(_stateFactory);
            _goalsController = new GoalsController(_stateFactory);
            _planner = new AiPlanner(_actionsController);
        }

        public ActionsController.IInitialBuilder BuildAction()
        {
            return _actionsController.CreateActionBuilder();
        }

        public IReadOnlyList<Action> GetAllActions()
        {
            return _actionsController.GetAll();
        }

        public GoalsController.IInitialBuilder BuildGoal()
        {
            return _goalsController.CreateActionBuilder();
        }

        public IReadOnlyList<Goal> GetAllGoals()
        {
            return _goalsController.GetAll();
        }

        private void OnDestroy()
        {
            _stateFactory?.Dispose();
        }
    }
}