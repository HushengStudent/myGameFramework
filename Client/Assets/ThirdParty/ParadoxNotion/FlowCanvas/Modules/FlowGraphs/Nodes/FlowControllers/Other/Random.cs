using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Description("Calls one random output each time In is called")]
    [Category("Flow Controllers/Togglers")]
    [ContextDefinedOutputs(typeof(Flow), typeof(int))]
    public class Random : FlowControlNode
    {

        [SerializeField]
        [ExposeField]
        [GatherPortsCallback]
        [MinValue(2)]
        [DelayedField]
        private int _portCount = 4;

        private int current;

        protected override void RegisterPorts() {
            var outs = new List<FlowOutput>();
            for ( var i = 0; i < _portCount; i++ ) {
                outs.Add(AddFlowOutput(i.ToString()));
            }
            AddFlowInput("In", (f) =>
            {
                current = UnityEngine.Random.Range(0, _portCount);
                outs[current].Call(f);
            });
            AddValueOutput<int>("Current", () => { return current; });
        }
    }
}