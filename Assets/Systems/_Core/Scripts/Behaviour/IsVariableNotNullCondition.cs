using System;
using Unity.Behavior;
using UnityEngine;

namespace SpaceStation.Core
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Is variable not null",
        story: "Is [variable] not null",
        category: "Conditions",
        id: "59fb43bdcd74ce68810c52e500b31154")]
    public partial class IsVariableNotNullCondition : Condition
    {
        [SerializeReference] public BlackboardVariable Variable;

        public override bool IsTrue()
        {
            return Variable.ObjectValue != null;
        }
    }
}
