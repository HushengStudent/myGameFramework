using System.Reflection;
using System.Linq;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Script Control/Standalone Only")]
    [Description("Calls a function that has signature of 'public Status NAME()' or 'public Status NAME(T)'. You should return Status.Success, Failure or Running within that function.")]
    public class ImplementedAction : ActionTask, ISubParametersContainer
    {

        [SerializeField] /*[IncludeParseVariables]*/
        protected ReflectedFunctionWrapper functionWrapper;
        private Status actionStatus = Status.Resting;

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
                    return "No Action Selected";
                }
                if ( targetMethod == null ) {
                    return string.Format("<color=#ff6457>* {0} *</color>", functionWrapper.GetMethodString());
                }
                var mInfo = targetMethod.IsStatic ? targetMethod.RTReflectedOrDeclaredType().FriendlyName() : agentInfo;
                return string.Format("[ {0}.{1}({2}) ]", mInfo, targetMethod.Name, functionWrapper.GetVariables().Length == 2 ? functionWrapper.GetVariables()[1].ToString() : "");
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

        protected override string OnInit() {

            if ( functionWrapper == null ) {
                return "No Method selected";
            }
            if ( targetMethod == null ) {
                return string.Format("Missing Method '{0}'", functionWrapper.GetMethodString());
            }

            try {
                functionWrapper.Init(targetMethod.IsStatic ? null : agent);
                return null;
            }
            catch { return "ImplementedAction Error"; }
        }

        protected override void OnUpdate() {
            if ( functionWrapper == null ) {
                EndAction(false);
                return;
            }

            actionStatus = (Status)functionWrapper.Call();

            if ( actionStatus == Status.Success ) {
                EndAction(true);
                return;
            }

            if ( actionStatus == Status.Failure ) {
                EndAction(false);
                return;
            }
        }

        protected override void OnStop() {
            actionStatus = Status.Resting;
        }

        void SetMethod(MethodInfo method) {
            if ( method != null ) {
                functionWrapper = ReflectedFunctionWrapper.Create(method, blackboard);
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Action Method") ) {
                var menu = new UnityEditor.GenericMenu();
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags != HideFlags.HideInInspector) ) {
                        menu = EditorUtils.GetInstanceMethodSelectionMenu(comp.GetType(), typeof(Status), typeof(object), SetMethod, 1, false, true, menu);
                    }
                    menu.AddSeparator("/");
                }

                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                    menu = EditorUtils.GetStaticMethodSelectionMenu(t, typeof(Status), typeof(object), SetMethod, 1, false, true, menu);
                    if ( typeof(UnityEngine.Component).IsAssignableFrom(t) ) {
                        menu = EditorUtils.GetInstanceMethodSelectionMenu(t, typeof(Status), typeof(object), SetMethod, 1, false, true, menu);
                    }
                }
                menu.ShowAsBrowser("Select Action Method", this.GetType());
                Event.current.Use();
            }

            if ( targetMethod != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.RTReflectedOrDeclaredType().FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Selected Action Method:", targetMethod.Name);
                GUILayout.EndVertical();

                if ( targetMethod.GetParameters().Length == 1 ) {
                    var paramName = targetMethod.GetParameters()[0].Name.SplitCamelCase();
                    NodeCanvas.Editor.BBParameterEditor.ParameterField(paramName, functionWrapper.GetVariables()[1]);
                }
            }
        }

#endif
    }
}