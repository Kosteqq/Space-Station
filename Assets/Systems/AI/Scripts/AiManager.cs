using SpaceStation.Core;

namespace SpaceStation.AI.Goap
{
    public class AiManager : GameSystemManager
    {
        private BlackboardStateFactory _stateFactory;
        private ActionsController _actionsController;
        
        public BlackboardStateFactory StateFactory => _stateFactory;
        
        public override void Initialize()
        {
            _stateFactory = new BlackboardStateFactory();
            _actionsController = new ActionsController();
        }

        public ActionsController.IInitialBuilder BuildAction()
        {
            return _actionsController.CreateActionBuilder();
        }

        private void OnDestroy()
        {
            _stateFactory?.Dispose();
        }
    }
}