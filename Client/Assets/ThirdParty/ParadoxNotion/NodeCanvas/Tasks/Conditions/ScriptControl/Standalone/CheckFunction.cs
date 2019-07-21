using System.Linq;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Script Control/Standalone Only")]
    [Description("Call a function with none or up to 6 parameters on a component and return whether or not the return value is equal to the check value")]
    public class CheckFunction : ConditionTask, ISubParametersContainer
    {

        [SerializeField] /*[IncludeParseVariables]*/
        protected ReflectedFunctionWrapper functionWrapper;
        [SerializeField]
        protected BBParameter checkValue;
        [SerializeField]
        protected CompareMethod comparison;

        BBParameter[] ISubParametersContainer.GetSubParameters() {
            return functionWrapper != null ? functionWrapper.GetVariables() : null;
        }

        private MethodInfo targetMethod {
            get { return functionWrapper != null ? functionWrapper.GetMethod() : null; }
        }

        public override System.Type agentType {
            get
            {
                if ( targetMethod == null ) { return typeof(Transform); }
                return targetMethod.IsStatic ? null : targetMethod.RTReflectedOrDeclaredType();
            }
        }

        protected override string info {
            get
            {
                if ( functionWrapper == null ) {
                    return "No Method Selected";
                }
                if ( targetMethod == null ) {
                    return string.Format("<color=#ff6457>* {0} *</color>", functionWrapper.GetMethodString());
                }

                var variables = functionWrapper.GetVariables();
                var paramInfo = "";
                for ( var i = 1; i < variables.Length; i++ ) {
                    paramInfo += ( i != 1 ? ", " : "" ) + variables[i].ToString();
                }
                var mInfo = targetMethod.IsStatic ? targetMethod.RTReflectedOrDeclaredType().FriendlyName() : agentInfo;
                return string.Format("{0}.{1}({2}){3}", mInfo, targetMethod.Name, paramInfo, OperationTools.GetCompareString(comparison) + checkValue);
            }
        }

        public override void OnValidate(ITaskSystem ownerSystem) {
            if ( functionWrapper != null && functionWrapper.HasChanged() ) {
                SetMethod(functionWrapper.GetMethod());
            }
            if ( functionWrapper != null && targetMethod == null ) {
                Error(string.Format("Missing Method '{0}'", functionWrapper.GetMethodString()));
            }
        }

        //store the method info on agent set for performance
        protected override string OnInit() {

            if ( targetMethod == null )
                return "CheckFunction Error";

            try {
                functionWrapper.Init(targetMethod.IsStatic ? null : agent);
                return null;
            }
            catch { return "CheckFunction Error"; }
        }

        //do it by invoking method
        protected override bool OnCheck() {

            if ( functionWrapper == null ) {
                return true;
            }

            if ( checkValue.varType == typeof(float) ) {
                return OperationTools.Compare((float)functionWrapper.Call(), (float)checkValue.value, comparison, 0.05f);
            }

            if ( checkValue.varType == typeof(int) ) {
                return OperationTools.Compare((int)functionWrapper.Call(), (int)checkValue.value, comparison);
            }

            return ObjectUtils.TrueEquals(functionWrapper.Call(), checkValue.value);
        }

        void SetMethod(MethodInfo method) {
            if ( method != null ) {
                functionWrapper = ReflectedFunctionWrapper.Create(method, blackboard);
                checkValue = BBParameter.CreateInstance(method.ReturnType, blackboard);
                comparison = CompareMethod.EqualTo;
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Method") ) {

                var menu = new UnityEditor.GenericMenu();
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ) {
                        menu = EditorUtils.GetInstanceMethodSelectionMenu(comp.GetType(), typeof(object), typeof(object), SetMethod, 6, false, true, menu);
                    }
                    menu.AddSeparator("/");
                }

                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                    menu = EditorUtils.GetStaticMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 6, false, true, menu);
                    if ( typeof(UnityEngine.Component).IsAssignableFrom(t) ) {
                        menu = EditorUtils.GetInstanceMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 6, false, true, menu);
                    }
                }
                menu.ShowAsBrowser("Select Method", this.GetType());
                Event.current.Use();
            }

            if ( targetMethod != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.RTReflectedOrDeclaredType().FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Method", targetMethod.Name);
                GUILayout.EndVertical();

                var paramNames = targetMethod.GetParameters().Select(p => p.Name.SplitCamelCase()).ToArray();
                var variables = functionWrapper.GetVariables();
                for ( var i = 0; i < paramNames.Length; i++ ) {
                    NodeCanvas.Editor.BBParameterEditor.ParameterField(paramNames[i], variables[i + 1]);
                }

                GUI.enabled = checkValue.varType == typeof(float) || checkValue.varType == typeof(int);
                comparison = (CompareMethod)UnityEditor.EditorGUILayout.EnumPopup("Comparison", comparison);
                GUI.enabled = true;
                NodeCanvas.Editor.BBParameterEditor.ParameterField("Check Value", checkValue);
            }
        }

#endif
    }
}