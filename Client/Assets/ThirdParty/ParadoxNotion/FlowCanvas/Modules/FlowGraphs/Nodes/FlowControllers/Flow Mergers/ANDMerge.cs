using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("AND")]
    [Category("Flow Controllers/Flow Merge")]
    [Description("Calls Out when all inputs are called together in the same frame, but the output is only called once per frame regardless of number of inputs called.")]
    public class ANDMerge : FlowControlNode
    {

        [SerializeField]
        [ExposeField]
        [GatherPortsCallback]
        [MinValue(2)]
        [DelayedField]
        private int _portCount = 2;

        private FlowOutput fOut;
        private int[] calls;
        private int lastFrameCall;

        protected override void RegisterPorts() {
            calls = new int[_portCount];
            fOut = AddFlowOutput("Out");
            for ( var _i = 0; _i < _portCount; _i++ ) {
                var i = _i;
                AddFlowInput(i.ToString(), (f) => { Check(i, f); });
            }
        }

        void Check(int index, Flow f) {
            calls[index] = Time.frameCount;
            for ( var i = 0; i < calls.Length; i++ ) {
                if ( calls[i] != calls[index] ) {
                    return;
                }
            }
            if ( Time.frameCount != lastFrameCall ) {
                lastFrameCall = Time.frameCount;
                fOut.Call(f);
            }
        }
    }
}