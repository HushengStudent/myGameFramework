using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Description("Utility node to merge the flow. It's exactly the same as connecting multiple Flow outputs to the same Flow input.")]
    [Name("Merge", 89)]
    public class Merge : FlowControlNode
    {

        [SerializeField]
        [ExposeField]
        [GatherPortsCallback]
        [MinValue(2)]
        [DelayedField]
        private int _portCount = 4;

        protected override void RegisterPorts() {
            var fOut = AddFlowOutput("Out");
            for ( var i = 0; i < _portCount; i++ ) {
                AddFlowInput(i.ToString(), fOut.Call);
            }
        }
    }
}