using UnityEngine;

namespace SpaceStation.AI.Goap
{
    public abstract class BlackboardStateDefinition : ScriptableObject
    {
        public abstract string Name { get; }
    }
}