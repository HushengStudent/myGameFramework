using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("Flip Flop (Multi)")]
    [Category("Flow Controllers/Togglers")]
    [Description("Each time input is signaled, the next output in order is called. After the last output, the order loops from the start.\nReset, resets the current index to zero.")]
    [ContextDefinedOutputs(typeof(int))]
    public class Sequence : FlowControlNode
    {

        [SerializeField]
        [ExposeField]
        [GatherPortsCallback]
        [MinValue(2)]
        [DelayedField]
        private int _portCount = 4;

        [MinValue(0)]
        public int current;

        private int original;

        public override void OnGraphStarted() { current = Mathf.Clamp(current, 0, _portCount - 1); original = current; }
        public override void OnGraphStoped() { current = original; }

        public override string name {
            get { return base.name + " " + string.Format("[{0}]", current.ToString()); }
        }

        protected override void RegisterPorts() {
            var outs = new List<FlowOutput>();

            for ( var i = 0; i < _portCount; i++ ) {
                outs.Add(AddFlowOutput(i.ToString()));
            }

            AddFlowInput("In", (f) =>
            {
                outs[current].Call(f);
                current = (int)Mathf.Repeat(current + 1, _portCount);
            });

            AddFlowInput("Reset", (f) => { current = 0; });
            AddValueOutput<int>("Current", () => { return current; });
        }
    }
}