using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceStation.AI.Goap
{
    [CustomEditor(typeof(AiController))]
    public class AiControllerEditor : Editor
    {
        private Label _blackboardInfo;
        private Label _planInfo;
        
        private AiController ControllerTarget => (AiController)target;
        
        public override VisualElement CreateInspectorGUI()
        {
            if (!Application.isPlaying)
                return base.CreateInspectorGUI();

            _blackboardInfo = new Label();
            _blackboardInfo.schedule.Execute(UpdateBlackboardInfo).Every(100);

            _planInfo = new Label();
            _planInfo.schedule.Execute(UpdatePlanInfo).Every(100);
            
            var root = new VisualElement();
            root.Add(_blackboardInfo);
            root.Add(_planInfo);
            
            return root;
        }
        
        private void UpdateBlackboardInfo()
        {
            _blackboardInfo.text = "<color=yellow>Blackboard</color>\n";

            foreach (var (state, value) in ControllerTarget.Blackboard.GetAll())
            {
                _blackboardInfo.text += $"{state.Name, -40}: {value}\n";
            }
        }
        
        private void UpdatePlanInfo()
        {
            if (ControllerTarget.Plan == null)
            {
                _planInfo.text = "<color=red>No Plan</color>\n";
                return;
            }
            
            _planInfo.text = $"<color=yellow>Plan</color>\n";
            _planInfo.text += $"<color=magenta>Goal \"{ControllerTarget.Plan.Goal.Name}\"</color>\n";

            foreach (var action in ControllerTarget.Plan.Actions)
            {
                _planInfo.text += action.Name + '\n';
            }
        }
    }
}