using SpaceStation.Core;

namespace SpaceStation.AI.Goap
{
    public class Action
    {
        public struct Context
        {
            public GameController Object { get; set; }
        }
        
        public enum Status
        {
            Success,
            Running,
            Failure
        }
        
        public delegate Status ContextStatusDelegate(in Context p_context);
        public delegate void ContextDelegate(in Context p_context);
        public delegate Status Delegate(in Context p_context);
        private readonly ContextStatusDelegate _startDelegate;
        private readonly ContextDelegate _stopDelegate;
        private readonly ContextStatusDelegate _runDelegate;
        
        public string Name { get; }
        public BlackboardStateValue[] Preconditions { get; }
        public BlackboardStateValue[] Effects { get; }

        internal Action(string p_name, BlackboardStateValue[] p_preconditions,
            BlackboardStateValue[] p_effects, ContextStatusDelegate p_startDelegate, 
            ContextDelegate p_stopDelegate, ContextStatusDelegate p_runDelegate)
        {
            Name = p_name;
            Preconditions = p_preconditions;
            Effects = p_effects;
            _startDelegate = p_startDelegate;
            _stopDelegate = p_stopDelegate;
            _runDelegate = p_runDelegate;
        }

        public Status Start(in Context p_context) => _startDelegate?.Invoke(in p_context) ?? Status.Success;
        public void Stop(in Context p_context) => _stopDelegate?.Invoke(in p_context);
        public Status Run(in Context p_context) => _runDelegate?.Invoke(in p_context) ?? Status.Success;
    }
}