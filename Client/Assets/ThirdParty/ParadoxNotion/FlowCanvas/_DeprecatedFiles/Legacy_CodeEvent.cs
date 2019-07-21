using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using System;
using System.Reflection;

namespace FlowCanvas.Nodes
{

    [Description("Subscribes to a C# System.Action Event and is called when the event is raised")]
    [Category("Events/Script")]
    [Obsolete]
    abstract public class CodeEventBase : EventNode<Transform>
    {

        [SerializeField]
        protected string eventName;
        [SerializeField]
        protected Type targetType;

        protected Component targetComponent;
        protected EventInfo eventInfo {
            get { return targetType != null ? targetType.RTGetEvent(eventName) : null; }
        }

        public void SetEvent(EventInfo e, object instace = null) {
            targetType = e.RTReflectedOrDeclaredType();
            eventName = e.Name;
            GatherPorts();
        }

        public override void OnGraphStarted() {

            ResolveSelf();

            if ( string.IsNullOrEmpty(eventName) ) {
                Debug.LogError("No Event Selected for CodeEvent, or target is NULL");
                return;
            }

            targetComponent = target.value.GetComponent(targetType);
            if ( targetComponent == null ) {
                Debug.LogError("Target is null");
                return;
            }

            if ( eventInfo == null ) {
                Debug.LogError(string.Format("Event {0} is not found", eventName));
                return;
            }
        }
    }


    ///----------------------------------------------------------------------------------------------

    [Obsolete]
    public class CodeEvent : CodeEventBase
    {

        private FlowOutput o;
        private Action pointer;

        public override void OnGraphStarted() {
            base.OnGraphStarted();
            pointer = Call;
            eventInfo.AddEventHandler(targetComponent, pointer);
        }

        public override void OnGraphStoped() {
            if ( !string.IsNullOrEmpty(eventName) && eventInfo != null ) {
                eventInfo.RemoveEventHandler(target.value.GetComponent(targetType), pointer);
            }
        }

        protected override void RegisterPorts() {
            if ( !string.IsNullOrEmpty(eventName) ) {
                o = AddFlowOutput(eventName);
            }
        }

        void Call() {
            o.Call(new Flow());
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeInspectorGUI() {

            base.OnNodeInspectorGUI();

            if ( eventName == null && !Application.isPlaying && GUILayout.Button("Select Event") ) {
                var o = target.value == null ? graphAgent.gameObject : target.value.gameObject;
                EditorUtils.ShowGameObjectEventSelectionMenu(o, null, (e) => { SetEvent(e); });
            }

            if ( eventName != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Selected Type", targetType.FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
                GUILayout.EndVertical();
            }
        }

#endif
    }


    ///----------------------------------------------------------------------------------------------


    [Obsolete]
    public class CodeEvent<T> : CodeEventBase
    {

        private FlowOutput o;
        private Action<T> pointer;
        private T eventValue;

        public override void OnGraphStarted() {
            base.OnGraphStarted();
            pointer = Call;
            eventInfo.AddEventHandler(targetComponent, pointer);
        }

        public override void OnGraphStoped() {
            if ( !string.IsNullOrEmpty(eventName) && eventInfo != null ) {
                eventInfo.RemoveEventHandler(target.value.GetComponent(targetType), pointer);
            }
        }

        void Call(T eventValue) {
            this.eventValue = eventValue;
            o.Call(new Flow());
        }

        protected override void RegisterPorts() {
            if ( !string.IsNullOrEmpty(eventName) ) {
                o = AddFlowOutput(eventName);
                AddValueOutput<T>("Value", () => { return eventValue; });
            }
        }


        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeInspectorGUI() {

            base.OnNodeInspectorGUI();

            if ( eventName == null && !Application.isPlaying && GUILayout.Button("Select Event") ) {
                var o = target.value == null ? graphAgent.gameObject : target.value.gameObject;
                EditorUtils.ShowGameObjectEventSelectionMenu(o, typeof(T), (e) => { SetEvent(e); });
            }

            if ( eventName != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Selected Type", targetType.FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
                GUILayout.EndVertical();
            }
        }

#endif
    }
}