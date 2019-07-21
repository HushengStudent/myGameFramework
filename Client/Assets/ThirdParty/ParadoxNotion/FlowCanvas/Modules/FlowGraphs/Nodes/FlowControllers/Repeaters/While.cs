using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("While True")]
    [Category("Flow Controllers/Iterators")]
    [Description("Once called, will continuously call 'Do' while the input boolean condition is true. Once condition becomes or is false, 'Done' is called")]
    [ContextDefinedInputs(typeof(bool))]
    public class While : FlowControlNode
    {

        private UnityEngine.Coroutine coroutine;

        public override void OnGraphStarted() {
            coroutine = null;
        }

        public override void OnGraphStoped() {
            if ( coroutine != null ) {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        protected override void RegisterPorts() {
            var c = AddValueInput<bool>("Condition");
            var fUpdate = AddFlowOutput("Do");
            var fFinish = AddFlowOutput("Done");
            AddFlowInput("In", (f) =>
            {
                if ( coroutine == null ) {
                    coroutine = StartCoroutine(DoWhile(fUpdate, fFinish, f, c));
                }
            });
        }

        IEnumerator DoWhile(FlowOutput fUpdate, FlowOutput fFinish, Flow f, ValueInput<bool> condition) {
            var active = true;
            f.BeginBreakBlock(() => { active = false; });
            while ( active && condition.value ) {
                while ( graph.isPaused ) {
                    yield return null;
                }
                fUpdate.Call(f);
                yield return null;
            }

            coroutine = null;
            f.EndBreakBlock();
            fFinish.Call(f);
        }
    }
}