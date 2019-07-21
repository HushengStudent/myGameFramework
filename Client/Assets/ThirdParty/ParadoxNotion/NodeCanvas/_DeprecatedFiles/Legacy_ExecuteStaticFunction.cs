using System.Collections;
using System.Linq;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Script Control/Standalone Only")]
    [Description("Execute a static function of up to 6 parameters and optionaly save the return value")]
    [System.Obsolete("Execute Function now supports static functions as well")]
    public class ExecuteStaticFunction : ActionTask, ISubParametersContainer
    {

        [SerializeField] /* [IncludeParseVariables] */
        protected ReflectedWrapper functionWrapper;

        BBParameter[] ISubParametersContainer.GetSubParameters() {
            return functionWrapper != null ? functionWrapper.GetVariables() : null;
        }

        private MethodInfo targetMethod {
            get { return functionWrapper != null ? functionWrapper.GetMethod() : null; }
        }

        protected override string info {
            get
            {
                if ( functionWrapper == null )
                    return "No Method Selected";
                if ( targetMethod == null )
                    return string.Format("<color=#ff6457>* {0} *</color>", functionWrapper.GetMethodString());

                var variables = functionWrapper.GetVariables();
                var returnInfo = "";
                var paramInfo = "";
                if ( targetMethod.ReturnType == typeof(void) ) {
                    for ( var i = 0; i < variables.Length; i++ )
                        paramInfo += ( i != 0 ? ", " : "" ) + variables[i].ToString();
                } else {
                    returnInfo = variables[0].isNone ? "" : variables[0] + " = ";
                    for ( var i = 1; i < variables.Length; i++ )
                        paramInfo += ( i != 1 ? ", " : "" ) + variables[i].ToString();
                }

                return string.Format("{0}{1}.{2} ({3})", returnInfo, targetMethod.DeclaringType.FriendlyName(), targetMethod.Name, paramInfo);
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

        //store the method info on init
        protected override string OnInit() {

            if ( functionWrapper == null ) {
                return "No Method selected";
            }
            if ( targetMethod == null ) {
                return string.Format("Missing Method '{0}'", functionWrapper.GetMethodString());
            }

            try {
                functionWrapper.Init(null);
                return null;
            }
            catch { return "ExecuteFunction Error"; }
        }

        //do it by calling delegate or invoking method
        protected override void OnExecute() {

            if ( targetMethod == null ) {
                EndAction(false);
                return;
            }

            if ( functionWrapper is ReflectedActionWrapper ) {
                ( functionWrapper as ReflectedActionWrapper ).Call();
            } else {
                ( functionWrapper as ReflectedFunctionWrapper ).Call();
            }

            EndAction();
        }

        void SetMethod(MethodInfo method) {
            if ( method != null ) {
                functionWrapper = ReflectedWrapper.Create(method, blackboard);
            }
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Static Method") ) {
                var menu = new UnityEditor.GenericMenu();
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                    foreach ( var _m in t.GetMethods(BindingFlags.Static | BindingFlags.Public).OrderBy(m => !m.IsSpecialName) ) {
                        var m = _m;
                        if ( m.IsGenericMethod )
                            continue;

                        var parameters = m.GetParameters();
                        if ( parameters.Length > 6 )
                            continue;

                        menu.AddItem(new GUIContent(t.FriendlyName() + "/" + m.SignatureName()), false, () => { SetMethod(m); });

                    }
                }
                menu.ShowAsBrowser("Select Static Method", this.GetType());
                Event.current.Use();
            }


            if ( targetMethod != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.DeclaringType.FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Method", targetMethod.Name);
                UnityEditor.EditorGUILayout.LabelField("Returns", targetMethod.ReturnType.FriendlyName());

                if ( targetMethod.ReturnType == typeof(IEnumerator) )
                    GUILayout.Label("<b>This will execute as a Coroutine</b>");

                GUILayout.EndVertical();

                var paramNames = targetMethod.GetParameters().Select(p => p.Name.SplitCamelCase()).ToArray();
                var variables = functionWrapper.GetVariables();
                if ( targetMethod.ReturnType == typeof(void) ) {
                    for ( var i = 0; i < paramNames.Length; i++ ) {
                        NodeCanvas.Editor.BBParameterEditor.ParameterField(paramNames[i], variables[i]);
                    }
                } else {
                    for ( var i = 0; i < paramNames.Length; i++ ) {
                        NodeCanvas.Editor.BBParameterEditor.ParameterField(paramNames[i], variables[i + 1]);
                    }
                    NodeCanvas.Editor.BBParameterEditor.ParameterField("Save Return Value", variables[0], true);
                }
            }
        }

#endif
    }
}