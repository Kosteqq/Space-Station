using System;
using System.Collections.Generic;

namespace SpaceStation.AI.Goap
{
    public interface IBlackboardRO
    {
        Blackboard Clone();
        bool Get<T>();
        bool Get(Type p_stateType);
        bool Get(BlackboardStateDefinition p_definition);
        IReadOnlyDictionary<BlackboardStateDefinition, bool> GetAll();
        bool CheckValue(BlackboardStateValue p_stateValue);
    }
    
    public class Blackboard : IBlackboardRO
    {
        private readonly BlackboardStateFactory _stateFactory;
        private readonly Dictionary<BlackboardStateDefinition, bool> _states = new(64);

        internal Blackboard(BlackboardStateFactory p_stateFactory)
        {
            _stateFactory = p_stateFactory;
        }

        public Blackboard Clone()
        {
            var clone = new Blackboard(_stateFactory);

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

        public void Set<T>(bool p_value)
            where T : BlackboardStateDefinition
        {
            InternalSet(typeof(T), p_value);
        }

        public void Set(Type p_stateType, bool p_value)
        {
            InternalSet(p_stateType, p_value);
        }

        public void Set(BlackboardStateDefinition p_definition, bool p_value)
        {
            InternalSet(p_definition.GetType(), p_value);
        }

        public bool Get<T>()
        {
            return InternalGet(typeof(T));
        }

        public bool Get(Type p_stateType)
        {
            return InternalGet(p_stateType);
        }

        public bool Get(BlackboardStateDefinition p_definition)
        {
            return InternalGet(p_definition.GetType());
        }

        public IReadOnlyDictionary<BlackboardStateDefinition, bool> GetAll()
        {
            return _states;
        }

        public bool CheckValue(BlackboardStateValue p_stateValue)
        {
            return Get(p_stateValue.Definition) == p_stateValue.Value;
        }

        private void InternalSet(Type p_stateType, bool p_value)
        {
            var definition = _stateFactory.Get(p_stateType);

            if (definition == null)
                return;

            if (!_states.ContainsKey(definition))
                _states.Add(definition, false);

            _states[definition] = p_value;
        }

        private bool InternalGet(Type p_stateType)
        {
            var definition = _stateFactory.Get(p_stateType);

            if (definition == null)
                return false;

            if (!_states.TryGetValue(definition, out var value))
            {
                _states.Add(definition, false);
                value = false;
            }

            return value;
        }

        // Set On/Off
        // Add Monitor
        //   Monitor - OnEventCheck
        //   Monitor - Set On/Off
        //   Several monitors/single monitor??
        //   Override Monitor
    }
}