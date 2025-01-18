using System.Collections.Generic;
using SpaceStation.Core;

namespace SpaceStation.AI.Goap
{
    public class AiManager : GameSystemManager
    {
        private ActionsController _actionsController;
        private GoalsController _goalsController;
        private AiPlanner _planner;
        
        internal AiPlanner Planner => _planner;
        
        public override void Initialize()
        {
            _actionsController = new ActionsController();
            _goalsController = new GoalsController();
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
    }
}