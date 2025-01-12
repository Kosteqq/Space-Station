using System;
using System.Collections.Generic;

namespace SpaceStation.AI.Goap
{
    public partial class ActionsController
    {
        public interface IBuildBuilder
        {
            Action Build();
        }

        public interface IRunBuilder
        {
            IBuildBuilder WithRunCallback(Action.RunDelegate p_runDelegate);
        }

        public interface IEffectsBuilder : IRunBuilder
        {
            IEffectsBuilder WithEffect(BlackboardStateDefinition p_stateDefinition, bool p_value);
            IEffectsBuilder WithEffect<T>(bool p_value)
                where T : BlackboardStateDefinition;
        }

        public interface IPreconditionsBuilder : IEffectsBuilder
        {
            IPreconditionsBuilder WithPrecondition(BlackboardStateDefinition p_stateDefinition, bool p_value);
            IPreconditionsBuilder WithPrecondition<T>(bool p_value)
                where T : BlackboardStateDefinition;
        }
        
        public interface IInitialBuilder
        {
            IPreconditionsBuilder WithName(string p_name);
        }

        internal class Builder : IInitialBuilder, IPreconditionsBuilder, IEffectsBuilder, IRunBuilder, IBuildBuilder
        {
            private readonly AiManager _aiManager;
            private readonly List<BlackboardStateValue> _preconditions = new(32);
            private readonly List<BlackboardStateValue> _effects = new(32);
            private string _name;
            private Action.RunDelegate _runDelegate;

            public event Action<Action> OnBuild;

            public IPreconditionsBuilder WithName(string p_name)
            {
                _name = p_name;
                return this;
            }
            
            public IPreconditionsBuilder WithPrecondition(BlackboardStateDefinition p_stateDefinition, bool p_value)
            {
                var definition = _aiManager.StateFactory.Get(p_stateDefinition.GetType());
                _preconditions.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IPreconditionsBuilder WithPrecondition<T>(bool p_value) where T : BlackboardStateDefinition
            {
                var definition = _aiManager.StateFactory.Get<T>();
                _preconditions.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IEffectsBuilder WithEffect(BlackboardStateDefinition p_stateDefinition, bool p_value)
            {
                var definition = _aiManager.StateFactory.Get(p_stateDefinition.GetType());
                _effects.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IEffectsBuilder WithEffect<T>(bool p_value) where T : BlackboardStateDefinition
            {
                var definition = _aiManager.StateFactory.Get<T>();
                _effects.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IBuildBuilder WithRunCallback(Action.RunDelegate p_runDelegate)
            {
                _runDelegate = p_runDelegate;
                return this;
            }

            public Action Build()
            {
                var actionInstance = new Action(_name, _preconditions.ToArray(), _effects.ToArray(), _runDelegate);
                
                OnBuild?.Invoke(actionInstance);
                
                return actionInstance;
            }
        }
    }
}