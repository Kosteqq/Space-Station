using System.Collections.Generic;
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

        public void CreatePlan()
        {
            SystemManager.Planner.CreatePlan(this, SystemManager.GetAllGoals()[0]);
        }
    }

    public class Plan
    {
        public Goal Goal { get; set; }
        public Queue<Action> Actions { get; set; }
    }
}