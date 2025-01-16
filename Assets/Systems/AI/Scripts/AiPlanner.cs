using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.AI.Goap
{
    internal class AiPlanner
    {
        private readonly ActionsController _actionsController;
        
        public AiPlanner(ActionsController p_actionsController)
        {
            _actionsController = p_actionsController;
        }
        
        public Plan CreatePlan(AiController p_controller, Goal p_goal)
        {
            var goalConditions = new List<BlackboardStateValue>();

            foreach (var condition in p_goal.SatisfyConditions)
            {
                if (p_controller.Blackboard.Get(condition.Definition) != condition.Value)
                    goalConditions.Add(new BlackboardStateValue(condition));
            }

            var roots = FindChildSubActions(p_controller.Blackboard, goalConditions);

            if (roots == null)
            {
                Debug.LogError(
                    $"Failed to satisfy goal \"{p_goal.Name}\" " +
                    $"conditions: {string.Join(", ", goalConditions.Select(condition => condition.Definition.Name))}!");
                return null;
            }

            // Debug.Log(GetDebugHierarchy(roots, ""));

            var flattenHierarchy = new Queue<Action>(64);
            FlatNodesHierarchy(roots, flattenHierarchy);

            return new Plan()
            {
                Goal = p_goal,
                Actions = flattenHierarchy,
            };
        }

        private List<Node> FindChildSubActions(IBlackboardRO p_blackboard, IReadOnlyList<BlackboardStateValue> p_conditions)
        {
            var blackboard = p_blackboard.Clone();
            var childs = new List<Node>();

            var conditions = new List<BlackboardStateValue>(p_conditions);
            var solvedConditions = new List<BlackboardStateValue>();
            
            while (true)
            {
                if (conditions.IsEmpty())
                    return childs;
                
                var child = GetNodeForConditions(blackboard, conditions);

                if (child == null)
                    return null;

                if (solvedConditions.Any(solvedCondition => !child.Blackboard.CheckValue(solvedCondition)))
                {
                    childs.Insert(0, child);
                }
                else
                {
                    childs.Add(child);
                }

                for (var childIndex = 0; childIndex < childs.Count; childIndex++)
                {
                    blackboard.Overwrite(childs[childIndex].Blackboard);
                    
                    for (var conditionIndex = conditions.Count - 1; conditionIndex >= 0; conditionIndex--)
                    {
                        var condition = conditions[conditionIndex];
                        
                        if (blackboard.CheckValue(condition))
                        {
                            solvedConditions.Add(condition);
                            conditions.RemoveAt(conditionIndex);
                        }
                    }
                }
            }
        }

        private Node GetNodeForConditions(IBlackboardRO p_blackboard, IReadOnlyList<BlackboardStateValue> p_conditions)
        {
            var allActions = _actionsController.GetAll();

            Action action = null;

            for (var index = p_conditions.Count - 1; index >= 0; index--)
            {
                action = allActions.FirstOrDefault(action => action.Effects.Any(p_conditions[index].Equals));
                
                if (action != null)
                    break;
            }

            if (action == null)
                return null;

            var childs = new List<Node>();
            {
                var conditions =
                    action.Preconditions
                        .Where(condition => !p_blackboard.CheckValue(condition))
                        .ToList();
                
                if (!conditions.IsEmpty())
                    childs = FindChildSubActions(p_blackboard, conditions);
            }

            if (childs == null)
                return null;

            {
                var blackboard = p_blackboard.Clone();

                // Overwrite blackboard in execute order (1. childs, 2. action)
                foreach (var childNode in childs)
                {
                    blackboard.Overwrite(childNode.Blackboard);
                }

                foreach (var effect in action.Effects)
                {
                    blackboard.Set(effect.Definition, effect.Value);
                }

                return new Node
                {
                    Action = action,
                    Blackboard = blackboard,
                    Childs = childs,
                };
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

        private string GetDebugHierarchy(List<Node> p_nodes, string p_prefix)
        {
            var output = "";
            
            foreach (var node in p_nodes)
            {
                if (!node.Childs.IsEmpty())
                    output += GetDebugHierarchy(node.Childs, p_prefix + '\t');
                
                output += p_prefix + (node.Action?.Name ?? "NULL") + '\n';
            }

            return output;
        }

        private class Node
        {
            [CanBeNull] public Action Action;
            public List<Node> Childs = new();
            public Blackboard Blackboard;
        }
    }
}