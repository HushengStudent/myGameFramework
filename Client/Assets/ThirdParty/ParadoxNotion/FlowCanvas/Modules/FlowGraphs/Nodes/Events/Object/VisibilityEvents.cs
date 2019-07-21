using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes
{

    [Name("Visibility")]
    [Category("Events/Object")]
    [Description("Calls events based on object's render visibility")]
    public class VisibilityEvents : MessageEventNode<Transform>
    {

        private FlowOutput onVisible;
        private FlowOutput onInvisible;
        private GameObject receiver;


        protected override string[] GetTargetMessageEvents() {
            return new string[] { "OnBecameVisible", "OnBecameInvisible" };
        }

        protected override void RegisterPorts() {
            onVisible = AddFlowOutput("Became Visible");
            onInvisible = AddFlowOutput("Became Invisible");
            AddValueOutput<GameObject>("Receiver", () => { return receiver; });
        }

        void OnBecameVisible(ParadoxNotion.Services.MessageRouter.MessageData msg) {
            this.receiver = ResolveReceiver(msg.receiver).gameObject;
            onVisible.Call(new Flow());
        }

        void OnBecameInvisible(ParadoxNotion.Services.MessageRouter.MessageData msg) {
            this.receiver = ResolveReceiver(msg.receiver).gameObject;
            onInvisible.Call(new Flow());
        }
    }
}