namespace SpaceStation.Core
{
    public abstract class SystemSubcontroller<TSystem> : GameplayController
        where TSystem : GameplaySystemManager
    {
        protected TSystem SystemManager { get; private set; }

        public override void InitializeGame()
        {
            base.InitializeGame();

            SystemManager = GameplayManager.GetSystem<TSystem>();
        }
    }
}