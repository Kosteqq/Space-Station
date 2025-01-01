using SpaceStation.Core;

namespace SpaceStation.PathFinding
{
    public class PathFindingTarget : SystemController<PathFindingManager>
    {
        public override void InitializeGame()
        {
            base.InitializeGame();
            
            SystemManager.RegisterTarget(this);
        }
    }
}