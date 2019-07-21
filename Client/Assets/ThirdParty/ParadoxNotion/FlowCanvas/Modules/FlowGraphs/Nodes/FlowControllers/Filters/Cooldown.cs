using System.Collections;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Category("Flow Controllers/Filters")]
    [Description("Filters the flow execution so that it can't be called very frequently")]
    [ContextDefinedInputs(typeof(float))]
    [ContextDefinedOutputs(typeof(float))]
    public class Cooldown : FlowControlNode
    {

        private Coroutine coroutine;
        private float remaining = 0;
        private float remainingNormalized = 0;

        FlowOutput start;
        FlowOutput finish;
        FlowOutput update;

        ValueInput<float> time;

        public override string name {
            get { return base.name + string.Format(" [{0}]", remaining.ToString("0.0")); }
        }

        public override void OnGraphStarted() {
            remaining = 0;
            remainingNormalized = 0;
            coroutine = null;
        }

        public override void OnGraphStoped() {
            if ( coroutine != null ) {
                StopCoroutine(coroutine);
                coroutine = null;
                remaining = 0;
                remainingNormalized = 0;
            }
        }

        protected override void RegisterPorts() {
            start = AddFlowOutput("Start", "Out");
            update = AddFlowOutput("Update");
            finish = AddFlowOutput("Finish", "Ready");
            time = AddValueInput<float>("Time").SetDefaultAndSerializedValue(1);

            AddValueOutput<float>("Time Left", "Current", () => { return Mathf.Max(remaining, 0); });
            AddValueOutput<float>("Normalized", () => { return Mathf.Clamp01(remainingNormalized); });

            AddFlowInput("In", Begin);
            AddFlowInput("Cancel", Cancel);
        }

        void Begin(Flow f) {
            if ( remaining <= 0 && coroutine == null ) {
                coroutine = StartCoroutine(CountDown(f));
            }
        }

        void Cancel(Flow f) {
            if ( coroutine != null ) {
                StopCoroutine(coroutine);
                coroutine = null;
                remaining = 0;
                remainingNormalized = 0;
            }
        }

        IEnumerator CountDown(Flow f) {
            var total = time.value;
            remaining = total;
            start.Call(f);
            while ( remaining > 0 ) {
                while ( graph.isPaused ) {
                    yield return null;
                }
                remaining -= Time.deltaTime;
                remainingNormalized = remaining / total;
                update.Call(f);
                yield return null;
            }
            coroutine = null;
            finish.Call(f);
        }
    }
}