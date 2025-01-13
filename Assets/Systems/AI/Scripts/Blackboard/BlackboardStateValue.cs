namespace SpaceStation.AI.Goap
{
    public class BlackboardStateValue
    {
        public BlackboardStateDefinition Definition;
        public bool Value;
        
        internal BlackboardStateValue()
        { }

        internal BlackboardStateValue(BlackboardStateValue p_other)
        {
            Definition = p_other.Definition;
            Value = p_other.Value;
        }

        internal BlackboardStateValue(BlackboardStateDefinition p_definition, bool p_value)
        {
            Definition = p_definition;
            Value = p_value;
        }

        public override bool Equals(object p_other)
        {
            return p_other is BlackboardStateValue stateValue 
                   && stateValue.Definition == Definition 
                   && stateValue.Value == Value;
        }
    }
}