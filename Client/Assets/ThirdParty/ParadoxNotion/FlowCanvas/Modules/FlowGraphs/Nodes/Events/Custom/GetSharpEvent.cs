using UnityEngine;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;

namespace FlowCanvas.Nodes
{
    [DoNotList]
    [Description("Returns a reference of a C# event, which can be used with the C# Event Callback node.")]
    public class GetSharpEvent : FlowNode
    {

        [SerializeField]
        private SerializedEventInfo _event;
        private ValueInput instancePort;
        private EventInfo eventInfo {
            get { return _event != null ? _event.Get() : null; }
        }

        public override string name {
            get
            {
                if ( eventInfo != null ) {
                    if ( eventInfo.IsStatic() ) {
                        return string.Format("{0}.{1}", eventInfo.DeclaringType.FriendlyName(), eventInfo.Name);
                    }
                    return eventInfo.Name;
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
            if ( !eventInfo.IsStatic() ) {
                instancePort = AddValueInput(eventInfo.RTReflectedOrDeclaredType().FriendlyName(), eventInfo.RTReflectedOrDeclaredType(), "Instance");
            }
            var wrapper = SharpEvent.Create(eventInfo);
            AddValueOutput("Value", wrapper.GetType(), () =>
            {
                wrapper.SetInstance(instancePort != null ? instancePort.value : null);
                return wrapper;
            });
        }
    }
}