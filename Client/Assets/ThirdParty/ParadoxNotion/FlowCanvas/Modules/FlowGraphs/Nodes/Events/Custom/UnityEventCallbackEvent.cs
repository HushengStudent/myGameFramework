using System;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;

namespace FlowCanvas.Nodes
{

    [Name("Unity Event Callback", 3)]
    [Category("Events/Custom")]
    [Description("Register a callback on a UnityEvent.\nWhen that event is raised, this node will get called.")]
    [ContextDefinedInputs(typeof(UnityEventBase))]
    public class UnityEventCallbackEvent : EventNode
    {

        [SerializeField]
        [ExposeField]
        [Tooltip("If enabled, registration will be handled on graph Enable/Disable automatically")]
        [GatherPortsCallback]
        protected bool _autoHandleRegistration;

        [SerializeField]
        private SerializedTypeInfo _type;

        private Type eventType {
            get { return _type != null ? _type.Get() : null; }
            set
            {
                if ( _type == null || _type.Get() != value ) {
                    _type = new SerializedTypeInfo(value);
                }
            }
        }

        private object[] argValues;
        private ValueInput eventInput;
        private FlowOutput flowCallback;
        private ReflectedUnityEvent reflectedEvent;

        public bool autoHandleRegistration {
            get { return _autoHandleRegistration; }
        }

        public override void OnGraphStarted() {
            if ( autoHandleRegistration ) {
                Register();
            }
        }

        public override void OnGraphStoped() {
            if ( autoHandleRegistration ) {
                Unregister();
            }
        }

        protected override void RegisterPorts() {
            eventType = eventType != null ? eventType : typeof(UnityEventBase);
            eventInput = AddValueInput("Event", eventType);
            if ( eventType == typeof(UnityEventBase) ) {
                return;
            }

            if ( reflectedEvent == null ) {
                reflectedEvent = new ReflectedUnityEvent(eventType);
            }
            if ( reflectedEvent.eventType != eventType ) {
                reflectedEvent.InitForEventType(eventType);
            }

            argValues = new object[reflectedEvent.parameters.Length];
            for ( var _i = 0; _i < reflectedEvent.parameters.Length; _i++ ) {
                var i = _i;
                var parameter = reflectedEvent.parameters[i];
                AddValueOutput(parameter.Name, "arg" + i, parameter.ParameterType, () => { return argValues[i]; });
            }

            flowCallback = AddFlowOutput("Callback");
            if ( !autoHandleRegistration ) {
                AddFlowInput("Register", Register, "Add");
                AddFlowInput("Unregister", Unregister, "Remove");
            }
        }

        void Register(Flow f = default(Flow)) {
            var unityEvent = eventInput.value as UnityEventBase;
            if ( unityEvent != null ) {
                reflectedEvent.StopListening(unityEvent, OnEventRaised);
                reflectedEvent.StartListening(unityEvent, OnEventRaised);
            }
        }

        void Unregister(Flow f = default(Flow)) {
            var unityEvent = eventInput.value as UnityEventBase;
            if ( unityEvent != null ) {
                reflectedEvent.StopListening(unityEvent, OnEventRaised);
            }
        }

        void OnEventRaised(params object[] args) {
            this.argValues = args;
            flowCallback.Call(new Flow());
        }

        public override System.Type GetNodeWildDefinitionType() {
            return typeof(UnityEventBase);
        }

        public override void OnPortConnected(Port port, Port otherPort) {
            if ( port == eventInput && otherPort.type.RTIsSubclassOf(typeof(UnityEventBase)) ) {
                eventType = otherPort.type;
                GatherPorts();
            }
        }
    }
}