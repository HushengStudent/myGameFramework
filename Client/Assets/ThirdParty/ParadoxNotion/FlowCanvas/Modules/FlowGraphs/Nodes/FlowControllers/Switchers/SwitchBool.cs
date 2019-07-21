using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("Switch Condition")]
    [Category("Flow Controllers/Switchers")]
    [Description("Branch the Flow based on a conditional boolean value")]
    [ContextDefinedInputs(typeof(bool))]
    public class SwitchBool : FlowControlNode
    {
        protected override void RegisterPorts() {
            var selector = AddValueInput<bool>("Condition");
            var caseTrue = AddFlowOutput("True");
            var caseFalse = AddFlowOutput("False");
            AddFlowInput("In", (f) => { f.Call(selector.value ? caseTrue : caseFalse); });
        }
    }
}