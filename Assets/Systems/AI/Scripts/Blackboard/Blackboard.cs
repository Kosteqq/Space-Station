using System;
using System.Collections.Generic;

namespace SpaceStation.AI.Goap
{
    public interface IBlackboardRO
    {
        Blackboard Clone();
        bool Get<T>() where T : BlackboardStateDefinition;
        bool Get(BlackboardState p_state);
        IReadOnlyDictionary<BlackboardState, bool> GetAll();
        bool CheckValue(BlackboardStateValue p_stateValue);
    }
    
    public class Blackboard : IBlackboardRO
    {
        private readonly Dictionary<BlackboardState, bool> _states = new(64);

        internal Blackboard()
        {
        }

        public Blackboard Clone()
        {
            var clone = new Blackboard();

            foreach (var (definition, value) in _states)
            {
                clone._states[definition] = value;
            }

            return clone;
        }

        public void Overwrite(Blackboard p_other)
        {
            foreach (var state in p_other._states)
            {
                _states[state.Key] = state.Value;
            }
        }

        public void Set<TDefinition>(bool p_value)
            where TDefinition : BlackboardStateDefinition
        {
            Set(BlackboardState.Create<TDefinition>(), p_value);
        }

        public void Set(BlackboardState p_state, bool p_value)
        {
            if (!_states.ContainsKey(p_state))
                _states.Add(p_state, false);

            _states[p_state] = p_value;
        }

        public bool Get<TDefinition>()
            where TDefinition : BlackboardStateDefinition
        {
            return Get(BlackboardState.Create<TDefinition>());
        }

        public bool Get(BlackboardState p_state)
        {
            if (!_states.TryGetValue(p_state, out var value))
            {
                _states.Add(p_state, false);
                value = false;
            }

            return value;
        }

        public IReadOnlyDictionary<BlackboardState, bool> GetAll()
        {
            return _states;
        }

        public bool CheckValue(BlackboardStateValue p_stateValue)
        {
            return Get(p_stateValue.State) == p_stateValue.Value;
        }

        // Set On/Off
        // Add Monitor
        //   Monitor - OnEventCheck
        //   Monitor - Set On/Off
        //   Several monitors/single monitor??
        //   Override Monitor
    }
}