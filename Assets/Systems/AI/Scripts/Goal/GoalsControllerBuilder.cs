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

        public interface ISatisfyConditionsBuilder : IPriorityBuilder
        {
            ISatisfyConditionsBuilder WithSatisfyCondition(BlackboardStateDefinition p_stateDefinition, bool p_value);
            ISatisfyConditionsBuilder WithSatisfyCondition<T>(bool p_value)
                where T : BlackboardStateDefinition;
        }
        
        public interface IActivationBuilder : ISatisfyConditionsBuilder
        {
            IActivationBuilder WithActivationCondition(BlackboardStateDefinition p_stateDefinition, bool p_value);
            IActivationBuilder WithActivationCondition<T>(bool p_value)
                where T : BlackboardStateDefinition;
        }
        
        public interface IInitialBuilder
        {
            IActivationBuilder WithName(string p_name);
        }

        internal class Builder : IInitialBuilder, ISatisfyConditionsBuilder, IActivationBuilder, IPriorityBuilder, IBuildBuilder
        {
            private readonly BlackboardStateFactory _stateFactory;
            private readonly List<BlackboardStateValue> _satisfyConditions = new(32);
            private readonly List<BlackboardStateValue> _activationConditions = new(32);
            private string _name;
            private float _priority;

            public event Action<Goal> OnBuild;

            public Builder(BlackboardStateFactory p_stateFactory)
            {
                _stateFactory = p_stateFactory;
            }

            public IActivationBuilder WithName(string p_name)
            {
                _name = p_name;
                return this;
            }

            public IActivationBuilder WithActivationCondition(BlackboardStateDefinition p_stateDefinition, bool p_value)
            {
                var definition = _stateFactory.Get(p_stateDefinition.GetType());
                _activationConditions.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IActivationBuilder WithActivationCondition<T>(bool p_value) where T : BlackboardStateDefinition
            {
                var definition = _stateFactory.Get<T>();
                _activationConditions.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }
            
            public ISatisfyConditionsBuilder WithSatisfyCondition(BlackboardStateDefinition p_stateDefinition, bool p_value)
            {
                var definition = _stateFactory.Get(p_stateDefinition.GetType());
                _satisfyConditions.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public ISatisfyConditionsBuilder WithSatisfyCondition<T>(bool p_value) where T : BlackboardStateDefinition
            {
                var definition = _stateFactory.Get<T>();
                _satisfyConditions.Add(new BlackboardStateValue(definition, p_value));
                
                return this;
            }

            public IBuildBuilder WithPriority(float p_priority)
            {
                _priority = p_priority;
                return this;
            }

            public Goal Build()
            {
                var actionInstance = new Goal(_name, _priority, _satisfyConditions.ToArray(), _activationConditions.ToArray());
                
                OnBuild?.Invoke(actionInstance);
                
                return actionInstance;
            }
        }
    }
}