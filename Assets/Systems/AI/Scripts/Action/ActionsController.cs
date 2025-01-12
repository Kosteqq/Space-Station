using System.Collections.Generic;

namespace SpaceStation.AI.Goap
{
    public partial class ActionsController
    {
        private readonly BlackboardStateFactory _stateFactory;
        private readonly List<Action> _actions = new(64);
        
        internal ActionsController(BlackboardStateFactory p_stateFactory)
        {
            _stateFactory = p_stateFactory;
        }
        
        public IInitialBuilder CreateActionBuilder()
        {
            var builder = new Builder(_stateFactory);
            builder.OnBuild += _actions.Add;

            return builder;
        }
    }
}