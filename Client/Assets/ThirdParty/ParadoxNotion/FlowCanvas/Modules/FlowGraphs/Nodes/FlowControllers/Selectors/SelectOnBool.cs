using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [ExposeAsDefinition]
    [ContextDefinedInputs(typeof(Wild), typeof(bool))]
    [ContextDefinedOutputs(typeof(Wild))]
    [Category("Flow Controllers/Selectors")]
    [Description("Select a Result value out of the two input cases provided, based on a boolean Condition")]
    public class SelectOnBool<T> : FlowControlNode
    {

        protected override void RegisterPorts() {
            var condition = AddValueInput<bool>("Condition");
            var isTrue = AddValueInput<T>("Is True", "True");
            var isFalse = AddValueInput<T>("Is False", "False");
            AddValueOutput<T>("Result", "Value", () => { return condition.value ? isTrue.value : isFalse.value; });
        }
    }
}