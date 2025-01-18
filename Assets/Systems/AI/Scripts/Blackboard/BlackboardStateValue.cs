using System;

namespace SpaceStation.AI.Goap
{
    public struct BlackboardStateValue : IEquatable<BlackboardStateValue>
    {
        public BlackboardState State { get; }
        public bool Value { get; set; }

        private BlackboardStateValue(BlackboardState p_state, bool p_value)
        {
            State = p_state;
            Value = p_value;
        }

        internal static BlackboardStateValue Create(BlackboardState p_state, bool p_value)
        {
            return new BlackboardStateValue(p_state, p_value);
        }

        internal static BlackboardStateValue Create<TDefiniton>(bool p_value)
            where TDefiniton : BlackboardStateDefinition
        {
            return new BlackboardStateValue(BlackboardState.Create<TDefiniton>(), p_value);
        }

        internal BlackboardStateValue Clone()
        {
            return new BlackboardStateValue(State, Value);
        }

        public override bool Equals(object p_other)
        {
            return p_other is BlackboardStateValue stateValue 
                   && stateValue.State == State 
                   && stateValue.Value == Value;
        }

        public bool Equals(BlackboardStateValue p_other)
        {
            return p_other.State == State 
                && p_other.Value == Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(State, Value);
        }
    }
}