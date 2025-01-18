using System;

namespace SpaceStation.AI.Goap
{
    public class CustomStateNameAttribute : Attribute
    {
        public string Name { get; }
        
        public CustomStateNameAttribute(string p_name)
        {
            Name = p_name;
        }
    }
    
    public abstract class BlackboardStateDefinition
    {
        protected BlackboardStateDefinition()
        { }
    }
}