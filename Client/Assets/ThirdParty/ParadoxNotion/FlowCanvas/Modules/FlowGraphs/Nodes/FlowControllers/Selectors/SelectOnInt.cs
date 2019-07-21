using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [ExposeAsDefinition]
    [ContextDefinedInputs(typeof(Wild), typeof(int))]
    [ContextDefinedOutputs(typeof(Wild))]
    [Category("Flow Controllers/Selectors")]
    [Description("Select a Result value out of the input cases provided, based on an Integer")]
    public class SelectOnInt<T> : FlowControlNode
    {

        [SerializeField]
        [ExposeField]
        [GatherPortsCallback]
        [MinValue(2)]
        [DelayedField]
        private int _portCount = 4;

        protected override void RegisterPorts() {
            var selector = AddValueInput<int>("Value");
            var cases = new ValueInput<T>[_portCount];
            for ( var i = 0; i < _portCount; i++ ) {
                cases[i] = AddValueInput<T>("Is " + i.ToString(), i.ToString());
            }
            var defaultCase = AddValueInput<T>("Default");
            AddValueOutput<T>("Result", "Value", () =>
            {
                var i = selector.value;
                if ( i >= 0 && i < cases.Length ) {
                    return cases[i].value;
                }
                return defaultCase.value;
            });
        }
    }
}