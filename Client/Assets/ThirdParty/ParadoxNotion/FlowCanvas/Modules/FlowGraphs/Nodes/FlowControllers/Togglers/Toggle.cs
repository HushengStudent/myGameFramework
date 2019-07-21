using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Category("Flow Controllers/Togglers")]
    [Description("When In is called, calls On or Off depending on the current toggle state. Whenever Toggle input is called the state changes.")]
    public class Toggle : FlowControlNode
    {

        [Name("Start Open")]
        public bool open = true;
        private bool original;

        public override string name {
            get { return base.name + " " + ( open ? "[ON]" : "[OFF]" ); }
        }

        public override void OnGraphStarted() { original = open; }
        public override void OnGraphStoped() { open = original; }

        protected override void RegisterPorts() {
            var fOn = AddFlowOutput("On");
            var fOff = AddFlowOutput("Off");
            AddFlowInput("In", (f) => { f.Call(open ? fOn : fOff); });
            AddFlowInput("Open", (f) => { open = true; });
            AddFlowInput("Close", (f) => { open = false; });
            AddFlowInput("Toggle", (f) => { open = !open; });
        }
    }
}