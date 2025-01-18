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
            IEffectsBuilder WithEffect(BlackboardState p_state, bool p_value);
            IEffectsBuilder WithEffect<T>(bool p_value)
                where T : BlackboardStateDefinition;
        }

        public interface IPreconditionsBuilder : IEffectsBuilder
        {
            IPreconditionsBuilder WithPrecondition(BlackboardState p_state, bool p_value);
            IPreconditionsBuilder WithPrecondition<T>(bool p_value)
                where T : BlackboardStateDefinition;
        }
        
        public interface IInitialBuilder
        {
            IPreconditionsBuilder WithName(string p_name);
        }

        internal class Builder : IInitialBuilder, IPreconditionsBuilder, IEffectsBuilder, IRunBuilder, IBuildBuilder
        {
            private readonly List<BlackboardStateValue> _preconditions = new(32);
            private readonly List<BlackboardStateValue> _effects = new(32);
            private string _name;
            private Action.ContextStatusDelegate _startDelegate;
            private Action.ContextDelegate _stopDelegate;
            private Action.ContextStatusDelegate _runDelegate;

            public event Action<Action> OnBuild;

            public Builder()
            {
            }

            public IPreconditionsBuilder WithName(string p_name)
            {
                _name = p_name;
                return this;
            }
            
            public IPreconditionsBuilder WithPrecondition(BlackboardState p_state, bool p_value)
            {
                _preconditions.Add(BlackboardStateValue.Create(p_state, p_value));
                return this;
            }

            public IPreconditionsBuilder WithPrecondition<T>(bool p_value) where T : BlackboardStateDefinition
            {
                _preconditions.Add(BlackboardStateValue.Create<T>(p_value));
                return this;
            }

            public IEffectsBuilder WithEffect(BlackboardState p_state, bool p_value)
            {
                _effects.Add(BlackboardStateValue.Create(p_state, p_value));
                return this;
            }

            public IEffectsBuilder WithEffect<T>(bool p_value) where T : BlackboardStateDefinition
            {
                _effects.Add(BlackboardStateValue.Create<T>(p_value));
                
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