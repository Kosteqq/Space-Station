using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceStation.AI.Goap
{
    [CustomEditor(typeof(AiController))]
    public class AiControllerEditor : Editor
    {
        private Label _blackboardInfo;
        
        private AiController ControllerTarget => (AiController)target;
        
        public override VisualElement CreateInspectorGUI()
        {
            if (!Application.isPlaying)
                return base.CreateInspectorGUI();

            _blackboardInfo = new Label();
            _blackboardInfo.schedule.Execute(UpdateBlackboardInfo).Every(100);
            
            var root = new VisualElement();
            root.Add(_blackboardInfo);
            
            return root;
        }
        
        private void UpdateBlackboardInfo()
        {
            _blackboardInfo.text = "<color=yellow>Blackboard</color>\n";

            var stateTypes = TypeCache.GetTypesDerivedFrom<BlackboardStateDefinition>();

            foreach (var type in stateTypes)
            {
                var definition = ControllerTarget.SystemManager.StateFactory.Get(type);
                _blackboardInfo.text += $"{definition.Name, -40}: {ControllerTarget.Blackboard.Get(definition)}\n";
            }
        }
    }
}