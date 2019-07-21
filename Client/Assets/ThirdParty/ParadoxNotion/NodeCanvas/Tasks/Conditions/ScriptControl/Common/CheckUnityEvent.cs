using System;
using System.Reflection;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.Events;

namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Script Control/Common")]
    [Description("Will subscribe to a public UnityEvent and return true when that event is raised.")]
    public class CheckUnityEvent : ConditionTask
    {

        [SerializeField]
        private System.Type targetType = null;
        [SerializeField]
        private string eventName = null;

        public override Type agentType {
            get { return targetType ?? typeof(Transform); }
        }

        protected override string info {
            get
            {
                if ( string.IsNullOrEmpty(eventName) ) {
                    return "No Event Selected";
                }
                return string.Format("'{0}' Raised", eventName);
            }
        }


        protected override string OnInit() {

            if ( eventName == null ) {
                return "No Event Selected";
            }

            var eventField = agentType.RTGetField(eventName);
            if ( eventField == null ) {
                return "Event was not found";
            }
            var unityEvent = (UnityEvent)eventField.GetValue(agent);
            unityEvent.AddListener(Raised);
            return null;
        }

        public void Raised() {
            YieldReturn(true);
        }

        protected override bool OnCheck() {
            return false;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Event") ) {
                Action<FieldInfo> Selected = (f) =>
                {
                    targetType = f.DeclaringType;
                    eventName = f.Name;
                };

                var menu = new UnityEditor.GenericMenu();
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ) {
                        menu = EditorUtils.GetInstanceFieldSelectionMenu(comp.GetType(), typeof(UnityEvent), Selected, menu);
                    }
                    menu.AddSeparator("/");
                }
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(Component)) ) {
                    menu = EditorUtils.GetInstanceFieldSelectionMenu(t, typeof(UnityEvent), Selected, menu);
                }

                menu.ShowAsBrowser("Select Event", this.GetType());
                Event.current.Use();
            }

            if ( targetType != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Selected Type", agentType.FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
                GUILayout.EndVertical();
            }
        }

#endif
    }


    [Category("✫ Script Control/Common")]
    [Description("Will subscribe to a public UnityEvent<T> and return true when that event is raised.")]
    public class CheckUnityEvent<T> : ConditionTask
    {

        [SerializeField]
        private System.Type targetType = null;
        [SerializeField]
        private string eventName = null;
        [SerializeField]
        private BBParameter<T> saveAs = null;

        public override Type agentType {
            get { return targetType ?? typeof(Transform); }
        }

        protected override string info {
            get
            {
                if ( string.IsNullOrEmpty(eventName) ) {
                    return "No Event Selected";
                }
                return string.Format("'{0}' Raised", eventName);
            }
        }


        protected override string OnInit() {

            if ( eventName == null ) {
                return "No Event Selected";
            }

            var eventField = agentType.RTGetField(eventName);
            if ( eventField == null ) {
                return "Event was not found";
            }

            var unityEvent = (UnityEvent<T>)eventField.GetValue(agent);
            unityEvent.AddListener(Raised);
            return null;
        }

        public void Raised(T eventValue) {
            saveAs.value = eventValue;
            YieldReturn(true);
        }

        protected override bool OnCheck() {
            return false;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Event") ) {
                Action<FieldInfo> Selected = (f) =>
                {
                    targetType = f.DeclaringType;
                    eventName = f.Name;
                };

                var menu = new UnityEditor.GenericMenu();
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ) {
                        menu = EditorUtils.GetInstanceFieldSelectionMenu(comp.GetType(), typeof(UnityEvent<T>), Selected, menu);
                    }
                    menu.AddSeparator("/");
                }
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(Component)) ) {
                    menu = EditorUtils.GetInstanceFieldSelectionMenu(t, typeof(UnityEvent<T>), Selected, menu);
                }

                menu.ShowAsBrowser("Select Event", this.GetType());
                Event.current.Use();
            }

            if ( targetType != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Selected Type", agentType.FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
                GUILayout.EndVertical();

                NodeCanvas.Editor.BBParameterEditor.ParameterField("Save Value As", saveAs, true);
            }
        }

#endif
    }



    [Category("✫ Script Control/Common")]
    [Description("Will subscribe to a public UnityEvent<T> and return true when that event is raised and it's value is equal to provided value as well.")]
    public class CheckUnityEventValue<T> : ConditionTask
    {

        [SerializeField]
        private System.Type targetType = null;
        [SerializeField]
        private string eventName = null;
        [SerializeField]
        private BBParameter<T> checkValue = null;

        public override Type agentType {
            get { return targetType ?? typeof(Transform); }
        }

        protected override string info {
            get
            {
                if ( string.IsNullOrEmpty(eventName) ) {
                    return "No Event Selected";
                }
                return string.Format("'{0}' Raised && Value == {1}", eventName, checkValue);
            }
        }


        protected override string OnInit() {

            if ( eventName == null ) {
                return "No Event Selected";
            }

            var eventField = agentType.RTGetField(eventName);
            if ( eventField == null ) {
                return "Event was not found";
            }

            var unityEvent = (UnityEvent<T>)eventField.GetValue(agent);
            unityEvent.AddListener(Raised);
            return null;
        }

        public void Raised(T eventValue) {
            if ( ObjectUtils.TrueEquals(checkValue.value, eventValue) ) {
                YieldReturn(true);
            }
        }

        protected override bool OnCheck() {
            return false;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Event") ) {
                Action<FieldInfo> Selected = (f) =>
                {
                    targetType = f.DeclaringType;
                    eventName = f.Name;
                };

                var menu = new UnityEditor.GenericMenu();
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ) {
                        menu = EditorUtils.GetInstanceFieldSelectionMenu(comp.GetType(), typeof(UnityEvent<T>), Selected, menu);
                    }
                    menu.AddSeparator("/");
                }
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(Component)) ) {
                    menu = EditorUtils.GetInstanceFieldSelectionMenu(t, typeof(UnityEvent<T>), Selected, menu);
                }

                menu.ShowAsBrowser("Select Event", this.GetType());
                Event.current.Use();
            }

            if ( targetType != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Selected Type", agentType.FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
                GUILayout.EndVertical();

                NodeCanvas.Editor.BBParameterEditor.ParameterField("Check Value", checkValue);
            }
        }

#endif
    }

}