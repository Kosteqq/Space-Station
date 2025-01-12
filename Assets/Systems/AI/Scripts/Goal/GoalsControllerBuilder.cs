using System;
using System.Collections.Generic;

namespace SpaceStation.AI.Goap
{
    public partial class GoalsController
    {
        public interface IBuildBuilder
        {
            Goal Build();
        }
        
        public interface IPriorityBuilder
        {
            IBuildBuilder WithPriority(float p_priority);
        }
        
        public interface IEffectsBuilder : IPriorityBuilder
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

        internal class Builder : IInitialBuilder, IPreconditionsBuilder, IEffectsBuilder, IPriorityBuilder, IBuildBuilder
        {
            private readonly BlackboardStateFactory _stateFactory;
            private readonly List<BlackboardStateValue> _preconditions = new(32);
            private readonly List<BlackboardStateValue> _effects = new(32);
            private string _name;
            private float _priority;

            public event Action<Goal> OnBuild;

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

            public IBuildBuilder WithPriority(float p_priority)
            {
                _priority = p_priority;
                return this;
            }

            public Goal Build()
            {
                var actionInstance = new Goal(_name, _priority, _preconditions.ToArray(), _effects.ToArray());
                
                OnBuild?.Invoke(actionInstance);
                
                return actionInstance;
            }
        }
    }
}