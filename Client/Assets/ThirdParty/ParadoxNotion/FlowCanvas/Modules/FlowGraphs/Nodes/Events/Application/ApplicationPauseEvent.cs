using System.Collections;
using ParadoxNotion.Design;
using ParadoxNotion.Services;

namespace FlowCanvas.Nodes
{

    [Name("On Application Pause")]
    [Category("Events/Application")]
    [Description("Called when the Application is paused or resumed")]
    public class ApplicationPauseEvent : EventNode
    {

        private FlowOutput pause;
        private bool isPause;

        public override void OnGraphStarted() {
            MonoManager.current.onApplicationPause += ApplicationPause;
        }

        public override void OnGraphStoped() {
            MonoManager.current.onApplicationPause -= ApplicationPause;
        }

        void ApplicationPause(bool isPause) {
            this.isPause = isPause;
            pause.Call(new Flow());
        }

        protected override void RegisterPorts() {
            pause = AddFlowOutput("Out");
            AddValueOutput<bool>("Is Pause", () => { return isPause; });
        }
    }
}