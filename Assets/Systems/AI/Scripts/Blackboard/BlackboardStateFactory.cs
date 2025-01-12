using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpaceStation.AI.Goap
{
    public class BlackboardStateFactory
    {
        private readonly List<BlackboardStateDefinition> _instances = new(64);
        
        internal BlackboardStateFactory()
        { }
        
        internal void Dispose()
        {
            foreach (var instance in _instances)
            {
                Object.Destroy(instance);
            }
            
            _instances.Clear();
        }

        public T Get<T>()
            where T : BlackboardStateDefinition
        {
            return Get(typeof(T)) as T;
        }

        public BlackboardStateDefinition Get(Type p_type)
        {
            if (p_type.IsAssignableFrom(typeof(BlackboardStateDefinition)))
            {
                Debug.LogError($"Type {p_type.Name} does not inherit from {nameof(BlackboardStateDefinition)}!");
                return null;
            }

            var instance = _instances.FirstOrDefault(instance => instance.GetType() == p_type);
            
            if (instance == null)
            {
                instance = (BlackboardStateDefinition)ScriptableObject.CreateInstance(p_type);
                _instances.Add(instance);
            }

            return instance;
        }
    }
}