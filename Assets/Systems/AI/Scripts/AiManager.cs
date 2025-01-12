using SpaceStation.Core;

namespace SpaceStation.AI.Goap
{
    public class AiManager : GameSystemManager
    {
        private BlackboardStateFactory _stateFactory;
        private ActionsController _actionsController;
        private GoalsController _goalsController;
        
        public BlackboardStateFactory StateFactory => _stateFactory;
        
        public override void Initialize()
        {
            _stateFactory = new BlackboardStateFactory();
            _actionsController = new ActionsController(_stateFactory);
            _goalsController = new GoalsController(_stateFactory);
        }

        public ActionsController.IInitialBuilder BuildAction()
        {
            return _actionsController.CreateActionBuilder();
        }

        public GoalsController.IInitialBuilder BuildGoal()
        {
            return _goalsController.CreateActionBuilder();
        }

        private void OnDestroy()
        {
            _stateFactory?.Dispose();
        }
    }
}