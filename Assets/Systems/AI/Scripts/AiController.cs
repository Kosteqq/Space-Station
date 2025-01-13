using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SpaceStation.Core;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.AI.Goap
{
    public class AiController : SystemController<AiManager>
    {
        private Blackboard _blackboard;

        public Blackboard Blackboard => _blackboard;

        public override void InitializeGame()
        {
            base.InitializeGame();
            _blackboard = new Blackboard(SystemManager.StateFactory);
        }

        public void CreatePlan()
        {
            var allGoals = SystemManager.GetAllGoals();
            
            var availableGoals = new List<Goal>();

            foreach (var goal in allGoals)
            {
                if (goal.ActivationConditions.All(condition => _blackboard.Get(condition.Definition) == condition.Value))
                {
                    availableGoals.Add(goal);
                }
            }

            if (availableGoals.IsEmpty())
            {
                Debug.LogError("Failed to find any goal for AI!", this);
                return;
            }
            

            var mostImportantGoal = availableGoals.OrderByDescending(goal => goal.Priority).First();
            Debug.Log($"Goal: {mostImportantGoal.Name}");
  
            var conditions = new List<BlackboardStateValue>();

            foreach (var condition in mostImportantGoal.SatisfyConditions)
            {
                if (_blackboard.Get(condition.Definition) != condition.Value)
                    conditions.Add(new BlackboardStateValue(condition));
            }

            var tempBlackboard = _blackboard.Clone();
            var roots = new List<Node>();
            
            while (true)
            {
                if (conditions.IsEmpty())
                    break;
                
                var child = CheckBranch(tempBlackboard, conditions);

                if (child == null)
                    break;

                tempBlackboard = child.Blackboard;
                roots.Add(child);

                for (var index = conditions.Count - 1; index >= 0; index--)
                {
                    if (tempBlackboard.Get(conditions[index].Definition) == conditions[index].Value)
                        conditions.RemoveAt(index);
                }
            }

            if (!conditions.IsEmpty())
            {
                Debug.LogError("Failed to resolve every conditions!");
                return;
            }

            var list = new Queue<Action>();
            Debug.Log(PrintNodesHierarchy(roots, ""));
            FlatNodesHierarchy(roots, list);

            Debug.Log("Path");
            foreach (var action in list)
            {
                Debug.Log(action.Name);
            }
        }

        private void FlatNodesHierarchy(List<Node> p_nodes, Queue<Action> p_actionList)
        {
            foreach (var node in p_nodes)
            {
                if (!node.Childs.IsEmpty())
                    FlatNodesHierarchy(node.Childs, p_actionList);
                
                p_actionList.Enqueue(node.Action);
            }
        }

        private string PrintNodesHierarchy(List<Node> p_nodes, string p_prefix)
        {
            var output = "";
            
            foreach (var node in p_nodes)
            {
                output += p_prefix + (node.Action?.Name ?? "NULL") + '\n';
                
                if (!node.Childs.IsEmpty())
                    output += PrintNodesHierarchy(node.Childs, p_prefix + '\t');
            }

            return output;
        }

        private Node CheckBranch(IBlackboardRO p_blackboard, IReadOnlyList<BlackboardStateValue> p_requirements)
        {
            var allActions = SystemManager.GetAllActions();

            Action action = null;

            for (var problemIndex = p_requirements.Count - 1; problemIndex >= 0; problemIndex--)
            {
                action = allActions
                    .FirstOrDefault(action => action.Effects
                        .Any(effect => 
                            effect.Definition == p_requirements[problemIndex].Definition
                            && effect.Value == p_requirements[problemIndex].Value));
                
                if (action != null)
                    break;
            }

            if (action == null)
                return null;

            var node = new Node();
            node.Action = action;
            node.Blackboard = p_blackboard.Clone();

            var currentRequirements = new List<BlackboardStateValue>();
            var solvedRequirements = new List<BlackboardStateValue>();

            foreach (var effect in action.Effects)
            {
                node.Blackboard.Set(effect.Definition, effect.Value);
            }
            
            foreach (var precondition in action.Preconditions)
            {
                if (p_blackboard.Get(precondition.Definition) != precondition.Value)
                    currentRequirements.Add(new BlackboardStateValue(precondition));
            }

            while (true)
            {
                if (currentRequirements.IsEmpty())
                    break;
                
                var child = CheckBranch(node.Blackboard, currentRequirements);

                if (child == null)
                    break;

                node.Blackboard = child.Blackboard;

                if (solvedRequirements
                    .Any(requirement => node.Blackboard.Get(requirement.Definition) != requirement.Value))
                {
                    node.Childs.Insert(0, child);
                }
                else
                {
                    node.Childs.Add(child);
                }

                for (var index = currentRequirements.Count - 1; index >= 0; index--)
                {
                    if (node.Blackboard.Get(currentRequirements[index].Definition) == currentRequirements[index].Value)
                    {
                        solvedRequirements.Add(currentRequirements[index]);
                        currentRequirements.RemoveAt(index);
                    }
                }
            }

            if (!currentRequirements.IsEmpty())
                return null;
            
            return node;
        }

        private class Node
        {
            [CanBeNull] public Action Action;
            public List<Node> Childs = new();
            public Blackboard Blackboard;
        }
    }

    public class AiPlan
    {
        public Goal Goal { get; set; }
        public Queue<Action> Actiosn { get; set; }
    }
}