using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Category("Flow Controllers/Switchers")]
    [Description("Branch the Flow based on a string value. The Default output is called if there is no other matching output same as the input value")]
    [ContextDefinedInputs(typeof(string))]
    [HasRefreshButton]
    public class SwitchString : FlowControlNode
    {

        [Name("Cases")]
        public List<string> comparisonOutputs = new List<string>();

        protected override void RegisterPorts() {
            var selector = AddValueInput<string>("Value");
            var cases = new FlowOutput[comparisonOutputs.Count];
            for ( var i = 0; i < cases.Length; i++ ) {
                cases[i] = AddFlowOutput(string.Format("\"{0}\"", comparisonOutputs[i]), i.ToString());
            }
            var defaultCase = AddFlowOutput("Default");

            AddFlowInput("In", (f) =>
            {
                var selectorValue = selector.value;
                var caseCalled = false;
                if ( selectorValue != null ) {
                    for ( var i = 0; i < comparisonOutputs.Count; i++ ) {
                        if ( selectorValue.Equals(comparisonOutputs[i], System.StringComparison.OrdinalIgnoreCase) ) {
                            caseCalled = true;
                            cases[i].Call(f);
                        }
                    }
                }
                if ( !caseCalled ) {
                    defaultCase.Call(f);
                }
            });
        }
    }
}