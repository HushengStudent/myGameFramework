using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [ExposeAsDefinition]
    [ContextDefinedInputs(typeof(Wild), typeof(string))]
    [ContextDefinedOutputs(typeof(Wild))]
    [Category("Flow Controllers/Selectors")]
    [Description("Select a Result value out of the input cases provided, based on a String")]
    [HasRefreshButton]
    public class SelectOnString<T> : FlowControlNode
    {

        [Name("Cases")]
        public List<string> stringCases = new List<string>();

        protected override void RegisterPorts() {
            var selector = AddValueInput<string>("Value");
            var cases = new ValueInput<T>[stringCases.Count];
            for ( var i = 0; i < stringCases.Count; i++ ) {
                cases[i] = AddValueInput<T>(string.Format("Is \"{0}\"", stringCases[i].ToString()), i.ToString());
            }
            var defaultCase = AddValueInput<T>("Default");
            AddValueOutput<T>("Result", "Value", () =>
            {
                var selectorValue = selector.value;
                if ( selectorValue != null ) {
                    for ( var i = 0; i < cases.Length; i++ ) {
                        if ( selectorValue.Equals(stringCases[i], System.StringComparison.OrdinalIgnoreCase) ) {
                            return cases[i].value;
                        }
                    }
                }
                return defaultCase.value;
            });
        }
    }
}