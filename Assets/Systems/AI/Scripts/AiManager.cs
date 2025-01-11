using SpaceStation.Core;

namespace SpaceStation.AI.Goap
{
    public class AiManager : GameSystemManager
    {
        private BlackboardStateFactory _stateFactory;
        
        public BlackboardStateFactory StateFactory => _stateFactory;
        
        public override void Initialize()
        {
            _stateFactory = new();
        }

        private void OnDestroy()
        {
            _stateFactory?.Dispose();
        }
    }
}