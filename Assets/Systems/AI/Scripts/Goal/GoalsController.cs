using System.Collections.Generic;

namespace SpaceStation.AI.Goap
{
    public partial class GoalsController
    {
        private readonly List<Goal> _goals = new(64);

        internal GoalsController()
        {
        }
        
        public IInitialBuilder CreateActionBuilder()
        {
            var builder = new Builder();
            builder.OnBuild += _goals.Add;

            return builder;
        }

        public IReadOnlyList<Goal> GetAll()
        {
            return _goals;
        }
    }
}