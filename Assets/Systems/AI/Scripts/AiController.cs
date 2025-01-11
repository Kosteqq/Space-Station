using SpaceStation.Core;

namespace SpaceStation.AI.Goap
{
    public class AiController : SystemController<AiManager>
    {
        private Blackboard _blackboard;

        public Blackboard Blackboard => _blackboard;

        public override void InitializeGame()
        {
            base.InitializeGame();
            _blackboard = new Blackboard(SystemManager.StateFactory);
        }
    }
}