using System;
using System.Reflection;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Script Control/Common")]
    [Description("Will subscribe to a public event of Action type and return true when the event is raised.\n(eg public static event System.Action [name])")]
    public class CheckStaticCSharpEvent : ConditionTask
    {

        [SerializeField]
        private System.Type targetType = null;
        [SerializeField]
        private string eventName = null;

        protected override string info {
            get
            {
                if ( string.IsNullOrEmpty(eventName) )
                    return "No Event Selected";
                return string.Format("'{0}' Raised", eventName);
            }
        }


        protected override string OnInit() {

            if ( eventName == null )
                return "No Event Selected";

            var eventInfo = targetType.RTGetEvent(eventName);
            if ( eventInfo == null )
                return "Event was not found";

            System.Action pointer = () => { Raised(); };
            eventInfo.AddEventHandler(null, pointer);
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
                Action<EventInfo> Selected = (e) =>
                {
                    targetType = e.DeclaringType;
                    eventName = e.Name;
                };

                var menu = new UnityEditor.GenericMenu();
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                    menu = EditorUtils.GetStaticEventSelectionMenu(t, null, Selected, menu);
                }
                menu.ShowAsBrowser("Select System.Action Event", this.GetType());
                Event.current.Use();
            }

            if ( targetType != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Selected Type", targetType.FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
                GUILayout.EndVertical();
            }
        }

#endif
    }

    [Category("✫ Script Control/Common")]
    [Description("Will subscribe to a public event of Action type and return true when the event is raised.\n(eg public static event System.Action<T> [name])")]
    public class CheckStaticCSharpEvent<T> : ConditionTask
    {

        [SerializeField]
        private System.Type targetType = null;
        [SerializeField]
        private string eventName = null;
        [SerializeField]
        [BlackboardOnly]
        private BBParameter<T> saveAs = null;

        protected override string info {
            get
            {
                if ( string.IsNullOrEmpty(eventName) )
                    return "No Event Selected";
                return string.Format("'{0}' Raised", eventName);
            }
        }


        protected override string OnInit() {

            if ( eventName == null )
                return "No Event Selected";

            var eventInfo = targetType.RTGetEvent(eventName);
            if ( eventInfo == null )
                return "Event was not found";

            System.Action<T> pointer = (v) => { Raised(v); };
            eventInfo.AddEventHandler(null, pointer);
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
                Action<EventInfo> Selected = (e) =>
                {
                    targetType = e.DeclaringType;
                    eventName = e.Name;
                };

                var menu = new UnityEditor.GenericMenu();
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                    menu = EditorUtils.GetStaticEventSelectionMenu(t, typeof(T), Selected, menu);
                }
                menu.ShowAsBrowser("Select System.Action<T> Event", this.GetType());
                Event.current.Use();
            }

            if ( targetType != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Selected Type", targetType.FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
                GUILayout.EndVertical();

                NodeCanvas.Editor.BBParameterEditor.ParameterField("Save Value As", saveAs, true);
            }
        }

#endif
    }
}