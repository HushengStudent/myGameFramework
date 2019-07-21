using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Events;
using ParadoxNotion.Serialization;

namespace FlowCanvas.Nodes
{

    [DoNotList]
    [Name("C# Event")]
    [Description("Automatically Subscribes to the target C# Event when the graph is enabled, and is called when the event is raised")]
    public class CSharpAutoCallbackEvent : EventNode
    {

        [SerializeField]
        private SerializedEventInfo _event;

        private ReflectedDelegateEvent reflectedEvent;
        private ValueInput instancePort;
        private FlowOutput callback;
        private object instance;
        private object[] args;

        private EventInfo eventInfo {
            get { return _event != null ? _event.Get() : null; }
        }

        private bool isStaticEvent {
            get { return eventInfo != null ? eventInfo.IsStatic() : false; }
        }

        public override string name {
            get
            {
                if ( eventInfo != null && isStaticEvent ) {
                    return string.Format("{0} ({1})", base.name, eventInfo.RTReflectedOrDeclaredType().FriendlyName());
                }
                return base.name;
            }
        }


        public void SetEvent(EventInfo info, object instance = null) {
            _event = new SerializedEventInfo(info);
            GatherPorts();
        }

        protected override void RegisterPorts() {
            if ( eventInfo == null ) {
                return;
            }

            var delegateType = eventInfo.EventHandlerType;
            if ( reflectedEvent == null ) {
                reflectedEvent = new ReflectedDelegateEvent(delegateType);
            }

            if ( !isStaticEvent ) {
                instancePort = AddValueInput(eventInfo.RTReflectedOrDeclaredType().FriendlyName(), eventInfo.RTReflectedOrDeclaredType(), "Instance");
            }

            var parameters = delegateType.RTGetDelegateTypeParameters();
            args = new object[parameters.Length];
            for ( var _i = 0; _i < parameters.Length; _i++ ) {
                var i = _i;
                var parameter = parameters[i];
                AddValueOutput(parameter.Name, "arg" + i, parameter.ParameterType, () => { return args[i]; });
            }

            callback = AddFlowOutput(eventInfo.Name, "Event");
        }

        public override void OnGraphStarted() {

            if ( eventInfo == null ) {
                return;
            }

            instance = null;
            if ( !isStaticEvent ) {
                instance = instancePort.value;
                if ( instance == null ) {
                    Fail("Target is null");
                    return;
                }
            }

            eventInfo.AddEventHandler(instance, reflectedEvent.AsDelegate());
            reflectedEvent.Add(OnEventRaised);
        }

        public override void OnGraphStoped() {
            if ( eventInfo != null ) {
                eventInfo.RemoveEventHandler(instance, reflectedEvent.AsDelegate());
                reflectedEvent.Remove(OnEventRaised);
            }
        }

        void OnEventRaised(params object[] args) {
            this.args = args;
            callback.Call(new Flow());
        }

    }
}