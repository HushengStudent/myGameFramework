using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("Coroutine")]
    [Category("Flow Controllers/Iterators")]
    [Description("Start a Coroutine that will repeat until Break is signaled")]
    public class CoroutineState : FlowControlNode
    {

        private bool active = false;
        UnityEngine.Coroutine coroutine = null;

        public override void OnGraphStoped() {
            if ( coroutine != null ) {
                StopCoroutine(coroutine);
                active = false;
            }
        }

        protected override void RegisterPorts() {
            var fStart = AddFlowOutput("Start");
            var fUpdate = AddFlowOutput("Update");
            var fFinish = AddFlowOutput("Finish");
            AddFlowInput("Start", (f) =>
            {
                if ( !active ) {
                    active = true;
                    coroutine = StartCoroutine(DoRepeat(fStart, fUpdate, fFinish, f));
                }
            });
            AddFlowInput("Break", (f) =>
            {
                active = false;
            });
        }


        IEnumerator DoRepeat(FlowOutput fStart, FlowOutput fUpdate, FlowOutput fFinish, Flow f) {
            fStart.Call(f);
            f.BeginBreakBlock(() => { active = false; });
            while ( active ) {
                while ( graph.isPaused ) {
                    yield return null;
                }
                fUpdate.Call(f);
                yield return null;
            }
            f.EndBreakBlock();
            fFinish.Call(f);
        }
    }
}