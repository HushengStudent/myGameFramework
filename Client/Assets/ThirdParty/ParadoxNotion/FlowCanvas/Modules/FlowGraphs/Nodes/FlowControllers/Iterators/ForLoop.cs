using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Description("Perform a for loop")]
    [Category("Flow Controllers/Iterators")]
    [ContextDefinedInputs(typeof(int))]
    [ContextDefinedOutputs(typeof(int))]
    public class ForLoop : FlowControlNode
    {
        [Tooltip("If true, loop will start from last and decrease by increment towards start")]
        public bool reverse;

        private int current;
        private bool broken;

        ValueInput<int> first;
        ValueInput<int> last;
        ValueInput<int> step;

        protected override void RegisterPorts() {
            first = AddValueInput<int>("First");
            last = AddValueInput<int>("Last", "Loops");
            step = AddValueInput<int>("Step").SetDefaultAndSerializedValue(1);

            AddValueOutput<int>("Index", () => { return current; });
            var fCurrent = AddFlowOutput("Do");
            var fFinish = AddFlowOutput("Done");
            AddFlowInput("In", (f) =>
            {
                current = 0;
                broken = false;
                f.BeginBreakBlock(() => { broken = true; });
                var increment = Mathf.Max(Mathf.Abs(step.value), 1);
                if ( !reverse ) {
                    for ( var i = first.value; i < last.value; i += increment ) {
                        if ( broken ) { break; }
                        current = i;
                        fCurrent.Call(f);
                    }
                } else {
                    for ( var i = last.value - 1; i >= first.value; i -= increment ) {
                        if ( broken ) { break; }
                        current = i;
                        fCurrent.Call(f);
                    }
                }
                f.EndBreakBlock();
                fFinish.Call(f);
            });

            AddFlowInput("Break", (f) => { broken = true; });
        }
    }
}