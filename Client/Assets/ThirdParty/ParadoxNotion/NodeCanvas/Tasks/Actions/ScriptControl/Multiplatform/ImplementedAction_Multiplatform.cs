using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Name("Implemented Action (mp)")]
    [Category("✫ Script Control/Multiplatform")]
    [Description("Calls a function that has signature of 'public Status NAME()' or 'public Status NAME(T)'. You should return Status.Success, Failure or Running within that function.")]
    public class ImplementedAction_Multiplatform : ActionTask
    {

        [SerializeField]
        private SerializedMethodInfo method;
        [SerializeField]
        private List<BBObjectParameter> parameters = new List<BBObjectParameter>();

        private Status actionStatus = Status.Resting;

        private MethodInfo targetMethod {
            get { return method != null ? method.Get() : null; }
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
                if ( method == null ) {
                    return "No Action Selected";
                }
                if ( targetMethod == null ) {
                    return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString());
                }
                var mInfo = targetMethod.IsStatic ? targetMethod.RTReflectedOrDeclaredType().FriendlyName() : agentInfo;
                return string.Format("[ {0}.{1}({2}) ]", mInfo, targetMethod.Name, parameters.Count == 1 ? parameters[0].ToString() : "");
            }
        }

        public override void OnValidate(ITaskSystem ownerSystem) {
            if ( method != null && method.HasChanged() ) {
                SetMethod(method.Get());
            }
            if ( method != null && method.Get() == null ) {
                Error(string.Format("Missing Method '{0}'", method.GetMethodString()));
            }
        }

        protected override string OnInit() {
            if ( method == null ) {
                return "No method selected";
            }
            if ( targetMethod == null ) {
                return string.Format("Missing method '{0}'", method.GetMethodString());
            }
            return null;
        }

        protected override void OnUpdate() {
            var args = parameters.Select(p => p.value).ToArray();
            actionStatus = (Status)targetMethod.Invoke(targetMethod.IsStatic ? null : agent, args);

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
                this.method = new SerializedMethodInfo(method);
                this.parameters.Clear();
                foreach ( var p in method.GetParameters() ) {
                    var newParam = new BBObjectParameter(p.ParameterType) { bb = blackboard };
                    parameters.Add(newParam);
                }
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
                    NodeCanvas.Editor.BBParameterEditor.ParameterField(paramName, parameters[0]);
                }
            }
        }

#endif
    }
}