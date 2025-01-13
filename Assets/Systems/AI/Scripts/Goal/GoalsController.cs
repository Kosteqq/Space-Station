using System.Collections.Generic;

namespace SpaceStation.AI.Goap
{
    public partial class GoalsController
    {
        private readonly BlackboardStateFactory _stateFactory;
        private readonly List<Goal> _goals = new(64);

        internal GoalsController(BlackboardStateFactory p_stateFactory)
        {
            _stateFactory = p_stateFactory;
        }
        
        public IInitialBuilder CreateActionBuilder()
        {
            var builder = new Builder(_stateFactory);
            builder.OnBuild += _goals.Add;

            return builder;
        }

        public IReadOnlyList<Goal> GetAll()
        {
            return _goals;
        }
    }
}