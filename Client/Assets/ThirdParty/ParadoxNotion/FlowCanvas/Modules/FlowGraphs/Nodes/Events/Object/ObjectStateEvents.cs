using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes
{

    [Name("Object State")]
    [Category("Events/Object")]
    [Description("OnEnable, OnDisable and OnDestroy callback events for target object")]
    public class ObjectStateEvents : MessageEventNode<Transform>
    {

        private FlowOutput onEnable;
        private FlowOutput onDisable;
        private FlowOutput onDestroy;
        private GameObject receiver;

        protected override string[] GetTargetMessageEvents() {
            return new string[] { "OnEnable", "OnDisable", "OnDestroy" };
        }

        protected override void RegisterPorts() {
            onEnable = AddFlowOutput("On Enable");
            onDisable = AddFlowOutput("On Disable");
            onDestroy = AddFlowOutput("On Destroy");
            AddValueOutput<GameObject>("Receiver", () => { return receiver; });
        }

        void OnEnable(ParadoxNotion.Services.MessageRouter.MessageData msg) {
            this.receiver = ResolveReceiver(msg.receiver).gameObject;
            onEnable.Call(new Flow());
        }

        void OnDisable(ParadoxNotion.Services.MessageRouter.MessageData msg) {
            this.receiver = ResolveReceiver(msg.receiver).gameObject;
            onDisable.Call(new Flow());
        }

        void OnDestroy(ParadoxNotion.Services.MessageRouter.MessageData msg) {
            this.receiver = ResolveReceiver(msg.receiver).gameObject;
            onDestroy.Call(new Flow());
        }
    }
}