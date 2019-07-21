using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using System.Linq;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace FlowCanvas.Nodes
{

    [Name("Custom Event", 100)]
    [Description("Called when a custom event is received on target(s).\n- Receiver, is the object which received the event.\n- Sender, is the object which invoked the event.\n\n- To send an event from a graph use the SendEvent node.\n- To send an event from code use: 'FlowScriptController.SendEvent(string)'")]
    [Category("Events/Custom")]
    public class CustomEvent : MessageEventNode<GraphOwner>
    {

        [RequiredField]
        [DelayedField]
        public BBParameter<string> eventName = "EventName";

        private FlowOutput onReceived;
        private GraphOwner sender;
        private GraphOwner receiver;

        public override string name {
            get { return base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", eventName); }
        }

        protected override string[] GetTargetMessageEvents() {
            return new string[] { "OnCustomEvent" };
        }

        protected override void RegisterPorts() {
            onReceived = AddFlowOutput("Received");
            AddValueOutput<GraphOwner>("Receiver", () => { return receiver; });
            AddValueOutput<GraphOwner>("Sender", () => { return sender; });
        }

        public void OnCustomEvent(ParadoxNotion.Services.MessageRouter.MessageData<EventData> msg) {
            if ( msg.value.name.ToUpper() == eventName.value.ToUpper() ) {
                var senderGraph = Graph.GetElementGraph(msg.sender);
                this.sender = senderGraph != null ? senderGraph.agent as GraphOwner : null;
                this.receiver = ResolveReceiver(msg.receiver);

#if UNITY_EDITOR
                if ( NodeCanvas.Editor.Prefs.logEvents ) {
                    Logger.Log(string.Format("Event Received from ({0}): '{1}'", receiver.name, msg.value.name), "Event", this);
                }
#endif

                onReceived.Call(new Flow());
            }
        }
    }


    [Name("Custom Event", 100)]
    [Description("Called when a custom event is received on target(s).\n- Receiver, is the object which received the event.\n- Sender, is the object which invoked the event.\n\n- To send an event from a graph use the SendEvent(T) node.\n- To send an event from code use: 'FlowScriptController.SendEvent(string, T)'")]
    [Category("Events/Custom")]
    [ContextDefinedOutputs(typeof(Wild))]
    public class CustomEvent<T> : MessageEventNode<GraphOwner>
    {

        [RequiredField]
        public BBParameter<string> eventName = "EventName";
        private FlowOutput onReceived;
        private GraphOwner sender;
        private GraphOwner receiver;
        private T receivedValue;

        public override string name {
            get { return base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", eventName); }
        }

        protected override string[] GetTargetMessageEvents() {
            return new string[] { "OnCustomEvent" };
        }

        protected override void RegisterPorts() {
            onReceived = AddFlowOutput("Received");
            AddValueOutput<GraphOwner>("Receiver", () => { return receiver; });
            AddValueOutput<GraphOwner>("Sender", () => { return sender; });
            AddValueOutput<T>("Event Value", () => { return receivedValue; });
        }

        public void OnCustomEvent(ParadoxNotion.Services.MessageRouter.MessageData<EventData> msg) {
            if ( msg.value.name.ToUpper() == eventName.value.ToUpper() ) {
                var senderGraph = Graph.GetElementGraph(msg.sender);
                this.sender = senderGraph != null ? senderGraph.agent as GraphOwner : null;
                this.receiver = ResolveReceiver(msg.receiver);

                var eventType = msg.value.GetType();
                if ( eventType.RTIsGenericType() ) {
                    var valueType = eventType.RTGetGenericArguments().FirstOrDefault();
                    if ( typeof(T).RTIsAssignableFrom(valueType) ) {
                        receivedValue = (T)msg.value.value;
                    }
                }

#if UNITY_EDITOR
                if ( NodeCanvas.Editor.Prefs.logEvents ) {
                    Logger.Log(string.Format("Event Received from ({0}): '{1}'", receiver.name, msg.value.name), "Event", this);
                }
#endif

                onReceived.Call(new Flow());
            }
        }
    }

}