using SpaceStation.Core;

namespace SpaceStation.AI.Goap
{
    public class Action
    {
        public class ActionRunContext
        {
            public GameController Object { get; set; }
        }
        
        public delegate void RunDelegate(ActionRunContext p_context);
        private readonly RunDelegate _runDelegate;
        
        public string Name { get; }
        public BlackboardStateValue[] Preconditions { get; }
        public BlackboardStateValue[] Effects { get; }

        internal Action(string p_name, BlackboardStateValue[] p_preconditions,
            BlackboardStateValue[] p_effects, RunDelegate p_runDelegate)
        {
            Name = p_name;
            Preconditions = p_preconditions;
            Effects = p_effects;
            _runDelegate = p_runDelegate;
        }

        public void Run(ActionRunContext p_context) => _runDelegate.Invoke(p_context);
    }
}