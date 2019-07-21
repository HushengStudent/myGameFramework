using System;
using System.Reflection;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Script Control/Common")]
    [Description("Will subscribe to a public event of Action type and return true when the event is raised.\n(eg public event System.Action [name])")]
    public class CheckCSharpEvent : ConditionTask
    {

        [SerializeField]
        private System.Type targetType = null;
        [SerializeField]
        private string eventName = null;

        public override Type agentType {
            get { return targetType != null ? targetType : typeof(Transform); }
        }

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

            var eventInfo = agentType.RTGetEvent(eventName);
            if ( eventInfo == null ) {
                return "Event was not found";
            }

            var methodInfo = this.GetType().RTGetMethod("Raised");
            var handler = methodInfo.RTCreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(agent, handler);
            // System.Action action = ()=>{ Raised(); };
            // eventInfo.AddEventHandler(agent, action );
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
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ) {
                        menu = EditorUtils.GetInstanceEventSelectionMenu(comp.GetType(), null, Selected, menu);
                    }
                    menu.AddSeparator("/");
                }
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(Component)) ) {
                    menu = EditorUtils.GetInstanceEventSelectionMenu(t, null, Selected, menu);
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
    [Description("Will subscribe to a public event of Action<T> type and return true when the event is raised.\n(eg public event System.Action<T> [name])")]
    public class CheckCSharpEvent<T> : ConditionTask
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
                if ( string.IsNullOrEmpty(eventName) )
                    return "No Event Selected";
                return string.Format("'{0}' Raised", eventName);
            }
        }


        protected override string OnInit() {

            if ( eventName == null )
                return "No Event Selected";

            var eventInfo = agentType.RTGetEvent(eventName);
            if ( eventInfo == null ) {
                return "Event was not found";
            }

            var methodInfo = this.GetType().RTGetMethod("Raised");
            var handler = methodInfo.RTCreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(agent, handler);
            // System.Action<T> action = (v)=>{ Raised(v); };
            // eventInfo.AddEventHandler(agent, action );
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
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ) {
                        menu = EditorUtils.GetInstanceEventSelectionMenu(comp.GetType(), typeof(T), Selected, menu);
                    }
                    menu.AddSeparator("/");
                }
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(Component)) ) {
                    menu = EditorUtils.GetInstanceEventSelectionMenu(t, typeof(T), Selected, menu);
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
    [Description("Will subscribe to a public event of Action<T> type and return true when the event is raised and it's value is equal to provided value as well.\n(eg public event System.Action<T> [name])")]
    public class CheckCSharpEventValue<T> : ConditionTask
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

            var eventInfo = agentType.RTGetEvent(eventName);
            if ( eventInfo == null ) {
                return "Event was not found";
            }

            var methodInfo = this.GetType().RTGetMethod("Raised");
            var handler = methodInfo.RTCreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(agent, handler);
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
                Action<EventInfo> Selected = (e) =>
                {
                    targetType = e.DeclaringType;
                    eventName = e.Name;
                };


                var menu = new UnityEditor.GenericMenu();
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ) {
                        menu = EditorUtils.GetInstanceEventSelectionMenu(comp.GetType(), typeof(T), Selected, menu);
                    }
                    menu.AddSeparator("/");
                }
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(Component)) ) {
                    menu = EditorUtils.GetInstanceEventSelectionMenu(t, typeof(T), Selected, menu);
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