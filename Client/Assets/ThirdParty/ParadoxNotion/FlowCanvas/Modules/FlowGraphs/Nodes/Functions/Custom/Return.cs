using ParadoxNotion;
using ParadoxNotion.Design;
using Logger = ParadoxNotion.Services.Logger;

namespace FlowCanvas.Nodes
{
    [Description("Should always be used to return out of a Custom Function. The return value is only required if the Custom Function returns a value as well.")]
    [Category("Functions/Custom")]
    [ContextDefinedInputs(typeof(object))]
    public class Return : FlowControlNode
    {
        [GatherPortsCallback]
        public bool useReturnValue = true;
        private ValueInput<object> returnPort;

        protected override void RegisterPorts() {
            if ( useReturnValue ) {
                returnPort = AddValueInput<object>("Value");
            }
            AddFlowInput(" ", (f) => { f.Return(useReturnValue ? returnPort.value : null, this); });
        }
    }

    /*
        ///----------------------------------------------------------------------------------------------

        [Description("Should always be used to return out of a Custom Function. The return value is only required if the Custom Function returns a value as well.")]
        [Category("Functions/Custom")]
        [ContextDefinedInputs(typeof(object))]
        [ExposeAsDefinition]
        public class Return<T> : FlowControlNode
        {
            protected override void RegisterPorts() {
                var returnPort = AddValueInput<T>("Value");
                AddFlowInput(" ", (f) => { f.Return(returnPort.value, this); });
            }
        }
    */
}