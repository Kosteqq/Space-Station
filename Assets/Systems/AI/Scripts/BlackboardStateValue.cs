namespace SpaceStation.AI.Goap
{
    public class BlackboardStateValue
    {
        public BlackboardStateDefinition Definition;
        public bool Value;
        
        internal BlackboardStateValue()
        { }

        internal BlackboardStateValue(BlackboardStateDefinition p_definition, bool p_value)
        {
            Definition = p_definition;
            Value = p_value;
        }
    }
}