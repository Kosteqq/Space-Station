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

        public interface IRunBuilder : IBuildBuilder
        {
            IRunBuilder WithStartCallback(Action.ContextStatusDelegate p_startDelegate);
            IRunBuilder WithStopCallback(Action.ContextDelegate p_stopDelegate);
            IRunBuilder WithRunCallback(Action.ContextStatusDelegate p_runDelegate);
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
            private readonly BlackboardStateFactory _stateFactory;
            private readonly List<BlackboardStateValue> _preconditions = new(32);
            private readonly List<BlackboardStateValue> _effects = new(32);
            private string _name;
            private Action.ContextStatusDelegate _startDelegate;
            private Action.ContextDelegate _stopDelegate;
            private Action.ContextStatusDelegate _runDelegate;

            public event Action<Action> OnBuild;

            public Builder(BlackboardStateFactory p_stateFactory)
            {
                _stateFactory = p_stateFactory;
            }

            public IPreconditionsBuilder WithName(string p_name)
            {
                _name = p_name;
                return this;
            }
            
            public IPreconditionsBuilder WithPrecondition(BlackboardStateDefinition p_stateDefinition, bool p_value)
            {
                var definition = _stateFactory.Get(p_stateDefinition.GetType());
                _preconditions.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IPreconditionsBuilder WithPrecondition<T>(bool p_value) where T : BlackboardStateDefinition
            {
                var definition = _stateFactory.Get<T>();
                _preconditions.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IEffectsBuilder WithEffect(BlackboardStateDefinition p_stateDefinition, bool p_value)
            {
                var definition = _stateFactory.Get(p_stateDefinition.GetType());
                _effects.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IEffectsBuilder WithEffect<T>(bool p_value) where T : BlackboardStateDefinition
            {
                var definition = _stateFactory.Get<T>();
                _effects.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }
            public IRunBuilder WithStartCallback(Action.ContextStatusDelegate p_startDelegate)
            {
                _startDelegate += p_startDelegate;
                return this;
            }
                
            public IRunBuilder WithStopCallback(Action.ContextDelegate p_stopDelegate)
            {
                _stopDelegate += p_stopDelegate;
                return this;
            }
            
            public IRunBuilder WithRunCallback(Action.ContextStatusDelegate p_runDelegate)
            {
                _runDelegate += p_runDelegate;
                return this;
            }

            public Action Build()
            {
                var actionInstance = new Action(_name, _preconditions.ToArray(), _effects.ToArray(),
                    _startDelegate, _stopDelegate, _runDelegate);
                
                OnBuild?.Invoke(actionInstance);
                
                return actionInstance;
            }
        }
    }
}