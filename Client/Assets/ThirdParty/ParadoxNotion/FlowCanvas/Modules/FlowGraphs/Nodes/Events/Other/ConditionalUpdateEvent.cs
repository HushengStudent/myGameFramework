using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes
{

    [Name("Conditional Event")]
    [Category("Events/Other")]
    [Description("Checks the condition boolean input per frame and calls outputs when the value has changed")]
    public class ConditionalUpdateEvent : EventNode, IUpdatable
    {

        private FlowOutput becameTrue;
        private FlowOutput becameFalse;
        //private FlowOutput isTrue;
        //private FlowOutput isFalse;
        private ValueInput<bool> condition;
        private bool lastState;

        protected override void RegisterPorts() {
            becameTrue = AddFlowOutput("Became True");
            becameFalse = AddFlowOutput("Became False");
            //isTrue      = AddFlowOutput("Is True");
            //isFalse     = AddFlowOutput("Is False");
            condition = AddValueInput<bool>("Condition");
        }

        public void Update() {

            if ( condition.value == false ) {

                if ( lastState == true ) {
                    becameFalse.Call(new Flow());
                    lastState = false;
                }

                //isFalse.Call(new Flow());

            } else {

                if ( lastState == false ) {
                    becameTrue.Call(new Flow());
                    lastState = true;
                }

                //isTrue.Call(new Flow());
            }
        }
    }
}