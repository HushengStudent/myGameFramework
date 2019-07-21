using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Name("Set Other Of Type")]
    [Category("Variables/Blackboard")]
    [Description("Use this to set a variable value of blackboards other than the one this flowscript is using")]
    [ContextDefinedInputs(typeof(Blackboard), typeof(Wild))]
    public class SetOtherVariable<T> : FlowNode
    {

        public OperationMethod operation = OperationMethod.Set;

        private ValueInput<string> varName;

        public override string name {
            get { return string.Format("Variable {0} Value", OperationTools.GetOperationString(operation)); }
        }

        protected override void RegisterPorts() {
            var bb = AddValueInput<Blackboard>("Blackboard");
            varName = AddValueInput<string>("Variable");
            var v = AddValueInput<T>("Value");

            var o = AddFlowOutput("Out");
            AddValueOutput<T>("Value", () => { return bb.value.GetValue<T>(varName.value); });
            AddFlowInput("In", (f) => { DoSet(bb.value, varName.value, v.value); o.Call(f); });
        }

        void DoSet(Blackboard bb, string name, T value) {
            var targetVariable = bb.GetVariable<T>(name);
            if ( targetVariable == null ) {
                targetVariable = (Variable<T>)bb.AddVariable(varName.value, typeof(T));
            }
            if ( operation != OperationMethod.Set ) {
                if ( typeof(T) == typeof(float) )
                    targetVariable.value = (T)(object)OperationTools.Operate((float)(object)targetVariable.value, (float)(object)value, operation);
                else if ( typeof(T) == typeof(int) )
                    targetVariable.value = (T)(object)OperationTools.Operate((int)(object)targetVariable.value, (int)(object)value, operation);
                else if ( typeof(T) == typeof(Vector3) )
                    targetVariable.value = (T)(object)OperationTools.Operate((Vector3)(object)targetVariable.value, (Vector3)(object)value, operation);
                else targetVariable.value = value;
            } else {
                targetVariable.value = value;
            }
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeInspectorGUI() {
            if ( typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(Vector3) ) {
                operation = (OperationMethod)UnityEditor.EditorGUILayout.EnumPopup("Operation", operation);
            }
            EditorUtils.BoldSeparator();
            base.DrawValueInputsGUI();
        }

#endif

    }
}