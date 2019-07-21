using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes
{

    [Name("Trigger2D")]
    [Category("Events/Object")]
    [Description("Called when 2D Trigger based event happen on target")]
    public class Trigger2DEvents : MessageEventNode<Collider2D>
    {

        private FlowOutput onEnter;
        private FlowOutput onStay;
        private FlowOutput onExit;
        private Collider2D receiver;
        private GameObject other;

        protected override string[] GetTargetMessageEvents() {
            return new string[] { "OnTriggerEnter2D", "OnTriggerStay2D", "OnTriggerExit2D" };
        }

        protected override void RegisterPorts() {
            onEnter = AddFlowOutput("Enter");
            onStay = AddFlowOutput("Stay");
            onExit = AddFlowOutput("Exit");
            AddValueOutput<Collider2D>("Receiver", () => { return receiver; });
            AddValueOutput<GameObject>("Other", () => { return other; });
        }

        void OnTriggerEnter2D(ParadoxNotion.Services.MessageRouter.MessageData<Collider2D> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.other = msg.value.gameObject;
            onEnter.Call(new Flow());
        }

        void OnTriggerStay2D(ParadoxNotion.Services.MessageRouter.MessageData<Collider2D> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.other = msg.value.gameObject;
            onStay.Call(new Flow());
        }

        void OnTriggerExit2D(ParadoxNotion.Services.MessageRouter.MessageData<Collider2D> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.other = msg.value.gameObject;
            onExit.Call(new Flow());
        }
    }
}