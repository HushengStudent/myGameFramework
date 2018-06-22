using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{
 
    [Category("✫ Blackboard")]
    [Description("Use this to get a variable on any blackboard by overriding the agent")]
    public class GetOtherBlackboardVariable : ActionTask<Blackboard> {

        [RequiredField]
        public BBParameter<string> targetVariableName;
        [BlackboardOnly]
        public BBObjectParameter saveAs;
       
        protected override string info{
            get {return string.Format("{0} = {1}", saveAs, targetVariableName ); }
        }

        protected override void OnExecute(){
            var targetVar = agent.GetVariable(targetVariableName.value);
            if (targetVar == null){
                EndAction(false);
                return;
            }

            saveAs.value = targetVar.value;
            EndAction(true);
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR
        
        protected override void OnTaskInspectorGUI(){
            if (GUILayout.Button("Select Target Variable Type")){
                EditorUtils.ShowPreferedTypesSelectionMenu(typeof(object), (t)=> {saveAs.SetType(t);} );
            }
            if (saveAs.varType != typeof(object)){
                DrawDefaultInspector();
            }
        }
        
        #endif
    }
}
 