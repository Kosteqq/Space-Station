namespace SpaceStation.Core
{
    public abstract class SystemController<TSystem> : GameController
        where TSystem : GameSystemManager
    {
        public TSystem SystemManager { get; private set; }

        public override void InitializeGame()
        {
            base.InitializeGame();

            SystemManager = GameManager.GetSystem<TSystem>();
        }
    }
}