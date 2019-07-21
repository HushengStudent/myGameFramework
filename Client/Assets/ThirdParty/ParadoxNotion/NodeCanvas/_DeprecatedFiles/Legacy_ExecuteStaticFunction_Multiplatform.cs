using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Name("Execute Static Function (mp)")]
    [Category("✫ Script Control/Multiplatform")]
    [Description("Execute a static function and optionaly save the return value")]
    [System.Obsolete("Execute Function now supports static functions as well")]
    public class ExecuteStaticFunction_Multiplatform : ActionTask
    {

        [SerializeField]
        protected SerializedMethodInfo method;
        [SerializeField]
        protected List<BBObjectParameter> parameters = new List<BBObjectParameter>();
        [SerializeField]
        [BlackboardOnly]
        protected BBObjectParameter returnValue;

        private MethodInfo targetMethod {
            get { return method != null ? method.Get() : null; }
        }

        protected override string info {
            get
            {
                if ( method == null )
                    return "No Method Selected";
                if ( targetMethod == null )
                    return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString());

                var returnInfo = targetMethod.ReturnType == typeof(void) ? "" : returnValue.ToString() + " = ";
                var paramInfo = "";
                for ( var i = 0; i < parameters.Count; i++ )
                    paramInfo += ( i != 0 ? ", " : "" ) + parameters[i].ToString();
                return string.Format("{0}{1}.{2} ({3})", returnInfo, targetMethod.DeclaringType.FriendlyName(), targetMethod.Name, paramInfo);
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

        //store the method info on init
        protected override string OnInit() {
            if ( method == null ) {
                return "No methMethodd selected";
            }
            if ( targetMethod == null ) {
                return string.Format("Missing Method '{0}'", method.GetMethodString());
            }
            return null;
        }

        //do it by calling delegate or invoking method
        protected override void OnExecute() {
            var args = parameters.Select(p => p.value).ToArray();
            returnValue.value = targetMethod.Invoke(agent, args);
            EndAction();
        }


        void SetMethod(MethodInfo method) {

            if ( method == null ) {
                return;
            }

            this.method = new SerializedMethodInfo(method);
            this.parameters.Clear();
            foreach ( var p in method.GetParameters() ) {
                var newParam = new BBObjectParameter(p.ParameterType) { bb = blackboard };
                if ( p.IsOptional ) {
                    newParam.value = p.DefaultValue;
                }
                parameters.Add(newParam);
            }

            if ( method.ReturnType != typeof(void) ) {
                this.returnValue = new BBObjectParameter(method.ReturnType) { bb = blackboard };
            } else {
                this.returnValue = null;
            }
        }


        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Static Method") ) {

                UnityEditor.GenericMenu.MenuFunction2 MethodSelected = (m) =>
                {
                    SetMethod((MethodInfo)m);
                };

                var menu = new UnityEditor.GenericMenu();
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                    foreach ( var m in t.GetMethods(BindingFlags.Static | BindingFlags.Public).OrderBy(m => !m.IsSpecialName) ) {

                        if ( m.IsGenericMethod )
                            continue;

                        menu.AddItem(new GUIContent(t.FriendlyName() + "/" + m.SignatureName()), false, MethodSelected, m);

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
                GUILayout.EndVertical();

                var paramNames = targetMethod.GetParameters().Select(p => p.Name.SplitCamelCase()).ToArray();
                for ( var i = 0; i < paramNames.Length; i++ ) {
                    NodeCanvas.Editor.BBParameterEditor.ParameterField(paramNames[i], parameters[i]);
                }

                if ( targetMethod.ReturnType != typeof(void) ) {
                    NodeCanvas.Editor.BBParameterEditor.ParameterField("Save Return Value", returnValue, true);
                }
            }
        }

#endif
    }
}