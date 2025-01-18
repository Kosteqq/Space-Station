using System;
using System.Linq;

namespace SpaceStation.AI.Goap
{
    public struct BlackboardState : IEquatable<BlackboardState>
    {
        public Type StateType { get; }
        public string Name { get; }

        private BlackboardState(Type p_stateType)
        {
            StateType = p_stateType;

            var nameAttribute = p_stateType
                    .GetCustomAttributes(false)
                    .FirstOrDefault(attribtue => attribtue is CustomStateNameAttribute)
                as CustomStateNameAttribute;

            Name = nameAttribute?.Name ?? p_stateType.Name;
        }
        
        public static BlackboardState Create<T>()
            where T : BlackboardStateDefinition
        {
            return new BlackboardState(typeof(T));
        }
        
        public static bool operator ==(BlackboardState p_state, BlackboardState p_other) 
        {
            return p_state.Equals(p_other);
        }

        public static bool operator !=(BlackboardState p_state, BlackboardState p_other) 
        {
            return !p_state.Equals(p_other);
        }

        public override bool Equals(object p_other)
        {
            return p_other is BlackboardState state
                   && state.StateType == this.StateType;
        }

        public bool Equals(BlackboardState p_other)
        {
            return p_other.StateType == this.StateType;
        }

        public override int GetHashCode()
        {
            return StateType.GetHashCode();
        }
    }
}