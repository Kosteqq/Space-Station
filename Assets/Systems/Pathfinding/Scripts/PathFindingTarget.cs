using SpaceStation.Core;

namespace SpaceStation.PathFinding
{
    public class PathFindingTarget : SystemSubcontroller<PathFindingManager>
    {
        public override void InitializeGame()
        {
            base.InitializeGame();
            
            SystemManager.RegisterTarget(this);
        }
    }
}