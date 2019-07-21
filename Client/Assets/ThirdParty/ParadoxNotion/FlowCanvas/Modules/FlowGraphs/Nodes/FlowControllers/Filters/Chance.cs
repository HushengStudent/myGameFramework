using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Category("Flow Controllers/Filters")]
    [Description("Filter the Flow based on a percentage between min and max.\nFor example: With Min = 0, Max = 1 and Percentage = 0.5, the chance is 50%.")]
    [ContextDefinedInputs(typeof(float))]
    public class Chance : FlowControlNode
    {

        protected override void RegisterPorts() {
            var success = AddFlowOutput("Success", "Out");
            var failure = AddFlowOutput("Failure");
            var chance = AddValueInput<float>("Percentage");
            var min = AddValueInput<float>("Min");
            var max = AddValueInput<float>("Max").SetDefaultAndSerializedValue(1);
            AddFlowInput("In", (f) => { f.Call(( UnityEngine.Random.Range(min.value, max.value) <= chance.value ) ? success : failure); });
        }
    }
}