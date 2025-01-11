using System;
using System.Collections.Generic;

namespace SpaceStation.AI.Goap
{
    public class Blackboard
    {
        private readonly BlackboardStateFactory _stateFactory;
        private readonly Dictionary<BlackboardStateDefinition, bool> _states = new(64);

        internal Blackboard(BlackboardStateFactory p_stateFactory)
        {
            _stateFactory = p_stateFactory;
        }

        public void Set<T>()
            where T : BlackboardStateDefinition
        {
            InternalSet(typeof(T), true);
        }

        public void Set(Type p_stateType)
        {
            InternalSet(p_stateType, true);
        }

        public void Set(BlackboardStateDefinition p_definition)
        {
            InternalSet(p_definition.GetType(), true);
        }

        public void Reset<T>()
            where T : BlackboardStateDefinition
        {
            InternalSet(typeof(T), false);
        }

        public void Reset(Type p_stateType)
        {
            InternalSet(p_stateType, false);
        }

        public void Reset(BlackboardStateDefinition p_definition)
        {
            InternalSet(p_definition.GetType(), false);
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
    }
}