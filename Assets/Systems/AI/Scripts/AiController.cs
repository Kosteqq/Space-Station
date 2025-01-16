using System;
using System.Collections.Generic;
using SpaceStation.Core;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.AI.Goap
{
    public class AiController : SystemController<AiManager>
    {
        private Blackboard _blackboard;
        private Plan _plan;

        public Blackboard Blackboard => _blackboard;

        public override void InitializeGame()
        {
            base.InitializeGame();
            _blackboard = new Blackboard(SystemManager.StateFactory);
        }

        private Action _prevAction;

        private void Update()
        {
            if (_plan == null || _plan.Actions.IsEmpty())
            {
                _plan = CreatePlan();
            }

            if (_plan == null || _plan.Actions.IsEmpty())
            {
                return;
            }

            var action = _plan.Actions.Peek();
            
            if (action != _prevAction)
            {
                action.Start(new Action.Context() { Object = this });
                _prevAction = action;
                Debug.Log($"Start action {action.Name}", this);
            }
            
            var status = action.Run(new Action.Context { Object = this });

            switch (status)
            {
                case Action.Status.Success:
                    Debug.Log($"Finished action {action.Name}", this);
                    action.Stop(new Action.Context() { Object = this });
                    _plan.Actions.Dequeue();
                    break;
                case Action.Status.Failure:
                    //_plan.Release();
                    _plan = null;
                    break;
            }
        }

        public Plan CreatePlan()
        {
            return SystemManager.Planner.CreatePlan(this, SystemManager.GetAllGoals()[0]);
        }
    }

    public class Plan
    {
        public Goal Goal { get; set; }
        public Queue<Action> Actions { get; set; }
    }
}