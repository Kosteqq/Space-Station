namespace SpaceStation.AI.Goap
{
    public class Goal
    {
        public string Name { get; }
        public float Priority { get; }
        public BlackboardStateValue[] Preconditions { get; }
        public BlackboardStateValue[] Effects { get; }

        internal Goal(string p_name, float p_priority,
            BlackboardStateValue[] p_preconditions, BlackboardStateValue[] p_effects)
        {
            Name = p_name;
            Priority = p_priority;
            Preconditions = p_preconditions;
            Effects = p_effects;
        }
    }
}