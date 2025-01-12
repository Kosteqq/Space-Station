using System.Collections.Generic;
using SpaceStation.Gameplay.Character;

namespace SpaceStation.AI.Goap
{
    public partial class ActionsController
    {
        private readonly List<Action> _actions = new(64);
        
        internal ActionsController()
        {
        }
        
        public IInitialBuilder CreateActionBuilder()
        {
            var builder = new Builder();
            builder.OnBuild += _actions.Add;

            return builder;
        }
    }
}