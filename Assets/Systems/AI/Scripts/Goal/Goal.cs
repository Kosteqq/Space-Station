namespace SpaceStation.AI.Goap
{
    public class Goal
    {
        public string Name { get; }
        public float Priority { get; }
        public BlackboardStateValue[] ActivationConditions { get; }
        public BlackboardStateValue[] SatisfyConditions { get; }

        internal Goal(string p_name, float p_priority,
            BlackboardStateValue[] p_satisfyConditions, BlackboardStateValue[] p_activationConditions)
        {
            Name = p_name;
            Priority = p_priority;
            SatisfyConditions = p_satisfyConditions;
            ActivationConditions = p_activationConditions;
        }
    }
}