using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("Switch Integer Custom")]
    [Category("Flow Controllers/Switchers")]
    [Description("Branch the Flow based on an integer value. The Default output is called when the Index value is out of range.")]
    [ContextDefinedInputs(typeof(int))]
    [HasRefreshButton]
    public class SwitchIntCustom : FlowControlNode
    {

        public List<int> intCases = new List<int>();

        protected override void RegisterPorts() {
            var selector = AddValueInput<int>("Value", "Index");
            var cases = new FlowOutput[intCases.Count];
            for ( var i = 0; i < cases.Length; i++ ) {
                cases[i] = AddFlowOutput(intCases[i].ToString(), i.ToString());
            }
            var defaultCase = AddFlowOutput("Default");
            AddFlowInput("In", (f) =>
           {
               var selectorValue = selector.value;
               var caseCalled = false;
               for ( var i = 0; i < intCases.Count; i++ ) {
                   if ( selectorValue == intCases[i] ) {
                       caseCalled = true;
                       cases[i].Call(f);
                   }
               }
               if ( !caseCalled ) {
                   defaultCase.Call(f);
               }
           });
        }
    }
}