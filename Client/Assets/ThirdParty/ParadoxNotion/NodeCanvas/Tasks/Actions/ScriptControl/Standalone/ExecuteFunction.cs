using System.Collections;
using System.Collections.Generic;
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
    [Description("Execute a function on a script, of up to 6 parameters and save the return if any. If function is an IEnumerator it will execute as a coroutine.")]
    public class ExecuteFunction : ActionTask, ISubParametersContainer, IReflectedWrapper
    {

        [SerializeField] /* [IncludeParseVariables] */
        protected ReflectedWrapper functionWrapper;
        private bool routineRunning;

        MemberInfo IReflectedWrapper.GetMemberInfo() {
            return targetMethod;
        }

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
                var returnInfo = "";
                var paramInfo = "";
                if ( targetMethod.ReturnType == typeof(void) ) {
                    for ( var i = 0; i < variables.Length; i++ ) {
                        paramInfo += ( i != 0 ? ", " : "" ) + variables[i].ToString();
                    }
                } else {
                    returnInfo = targetMethod.ReturnType == typeof(void) || targetMethod.ReturnType == typeof(IEnumerator) || variables[0].isNone ? "" : variables[0] + " = ";
                    for ( var i = 1; i < variables.Length; i++ ) {
                        paramInfo += ( i != 1 ? ", " : "" ) + variables[i].ToString();
                    }
                }

                var mInfo = targetMethod.IsStatic ? targetMethod.RTReflectedOrDeclaredType().FriendlyName() : agentInfo;
                return string.Format("{0}{1}.{2}({3})", returnInfo, mInfo, targetMethod.Name, paramInfo);
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
                functionWrapper.Init(targetMethod.IsStatic ? null : agent);
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

            try {
                if ( targetMethod.ReturnType == typeof(IEnumerator) ) {
                    StartCoroutine(InternalCoroutine((IEnumerator)( (ReflectedFunctionWrapper)functionWrapper ).Call()));
                    return;
                }

                if ( targetMethod.ReturnType == typeof(void) ) {
                    ( (ReflectedActionWrapper)functionWrapper ).Call();
                } else {
                    ( (ReflectedFunctionWrapper)functionWrapper ).Call();
                }

                EndAction(true);
            }

            catch ( System.Exception e ) {
                Debug.LogError(string.Format("{0}\n{1}", e.Message, e.StackTrace));
                EndAction(false);
            }
        }

        protected override void OnStop() {
            routineRunning = false;
        }

        IEnumerator InternalCoroutine(IEnumerator routine) {
            routineRunning = true;
            while ( routineRunning && routine.MoveNext() ) {
                if ( routineRunning == false ) {
                    yield break;
                }
                yield return routine.Current;
            }

            if ( routineRunning ) {
                EndAction();
            }
        }

        void SetMethod(MethodInfo method) {
            if ( method != null ) {
                functionWrapper = ReflectedWrapper.Create(method, blackboard);
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Method") ) {
                var menu = new UnityEditor.GenericMenu();
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags != HideFlags.HideInInspector) ) {
                        menu = EditorUtils.GetInstanceMethodSelectionMenu(comp.GetType(), typeof(object), typeof(object), SetMethod, 6, false, false, menu);
                    }
                    menu.AddSeparator("/");
                }

                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                    menu = EditorUtils.GetStaticMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 6, false, false, menu);
                    if ( typeof(UnityEngine.Component).IsAssignableFrom(t) ) {
                        menu = EditorUtils.GetInstanceMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 6, false, false, menu);
                    }
                }
                menu.ShowAsBrowser("Select Method", this.GetType());
                Event.current.Use();
            }

            var m = targetMethod;
            if ( m != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.RTReflectedOrDeclaredType().FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Method", m.Name);
                UnityEditor.EditorGUILayout.LabelField("Returns", m.ReturnType.FriendlyName());

                UnityEditor.EditorGUILayout.HelpBox(DocsByReflection.GetMemberSummary(targetMethod), UnityEditor.MessageType.None);

                if ( m.ReturnType == typeof(IEnumerator) ) {
                    GUILayout.Label("<b>This will execute as a Coroutine!</b>");
                }

                GUILayout.EndVertical();

                var paramNames = m.GetParameters().Select(p => p.Name.SplitCamelCase()).ToArray();
                var variables = functionWrapper.GetVariables();
                if ( m.ReturnType == typeof(void) ) {
                    for ( var i = 0; i < paramNames.Length; i++ ) {
                        NodeCanvas.Editor.BBParameterEditor.ParameterField(paramNames[i], variables[i]);
                    }
                } else {
                    for ( var i = 0; i < paramNames.Length; i++ ) {
                        NodeCanvas.Editor.BBParameterEditor.ParameterField(paramNames[i], variables[i + 1]);
                    }

                    if ( m.ReturnType != typeof(IEnumerator) ) {
                        NodeCanvas.Editor.BBParameterEditor.ParameterField("Save Return Value", variables[0], true);
                    }
                }
            }
        }

#endif
    }
}