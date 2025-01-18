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
            ISatisfyConditionsBuilder WithSatisfyCondition(BlackboardState p_state, bool p_value);
            ISatisfyConditionsBuilder WithSatisfyCondition<TDefinition>(bool p_value)
                where TDefinition : BlackboardStateDefinition;
        }
        
        public interface IActivationBuilder : ISatisfyConditionsBuilder
        {
            IActivationBuilder WithActivationCondition(BlackboardState p_state, bool p_value);
            IActivationBuilder WithActivationCondition<TDefinition>(bool p_value)
                where TDefinition : BlackboardStateDefinition;
        }
        
        public interface IInitialBuilder
        {
            IActivationBuilder WithName(string p_name);
        }

        internal class Builder : IInitialBuilder, ISatisfyConditionsBuilder, IActivationBuilder, IPriorityBuilder, IBuildBuilder
        {
            private readonly List<BlackboardStateValue> _satisfyConditions = new(32);
            private readonly List<BlackboardStateValue> _activationConditions = new(32);
            private string _name;
            private float _priority;

            public event Action<Goal> OnBuild;

            public IActivationBuilder WithName(string p_name)
            {
                _name = p_name;
                return this;
            }

            public IActivationBuilder WithActivationCondition(BlackboardState p_state, bool p_value)
            {
                _activationConditions.Add(BlackboardStateValue.Create(p_state, p_value));
                
                return this;
            }

            public IActivationBuilder WithActivationCondition<TDefinition>(bool p_value)
                where TDefinition : BlackboardStateDefinition
            {
                _activationConditions.Add(BlackboardStateValue.Create<TDefinition>(p_value));
                
                return this;
            }
            
            public ISatisfyConditionsBuilder WithSatisfyCondition(BlackboardState p_state, bool p_value)
            {
                _satisfyConditions.Add(BlackboardStateValue.Create(p_state, p_value));
                
                return this;
            }

            public ISatisfyConditionsBuilder WithSatisfyCondition<TDefinition>(bool p_value) 
                where TDefinition : BlackboardStateDefinition
            {
                _satisfyConditions.Add(BlackboardStateValue.Create<TDefinition>(p_value));
                
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