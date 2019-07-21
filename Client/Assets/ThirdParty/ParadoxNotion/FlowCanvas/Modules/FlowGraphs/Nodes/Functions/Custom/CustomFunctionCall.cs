using System.Linq;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
    [DoNotList]
    [Name("Function Call")]
    [Description("Calls an existing Custom Function")]
    [Category("Functions/Custom")]
    [ParadoxNotion.Serialization.DeserializeFrom("FlowCanvas.Nodes.RelayFlowInput")]
    public class CustomFunctionCall : FlowControlNode
    {

        [SerializeField]
        private string _sourceOutputUID;
        private string sourceFunctionUID {
            get { return _sourceOutputUID; }
            set { _sourceOutputUID = value; }
        }

        private ValueInput[] portArgs;
        private object[] objectArgs;
        private FlowOutput fOut;

        private object _sourceFunction;
        public CustomFunctionEvent sourceFunction {
            get
            {
                if ( _sourceFunction == null ) {
                    _sourceFunction = graph.GetAllNodesOfType<CustomFunctionEvent>().FirstOrDefault(i => i.UID == sourceFunctionUID);
                    if ( _sourceFunction == null ) {
                        _sourceFunction = new object();
                    }
                }
                return _sourceFunction as CustomFunctionEvent;
            }
            set { _sourceFunction = value; }
        }

        public override string name {
            get { return string.Format("Call {0} ()", sourceFunction != null ? sourceFunction.identifier : "NONE"); }
        }

        public override string description {
            get { return sourceFunction != null && !string.IsNullOrEmpty(sourceFunction.comments) ? sourceFunction.comments : base.description; }
        }

        //...
        public void SetFunction(CustomFunctionEvent func) {
            sourceFunctionUID = func != null ? func.UID : null;
            sourceFunction = func != null ? func : null;
            GatherPorts();
        }

        //...
        protected override void RegisterPorts() {
            AddFlowInput(" ", Invoke);
            if ( sourceFunction != null ) {
                var parameters = sourceFunction.parameters;
                portArgs = new ValueInput[parameters.Count];
                for ( var _i = 0; _i < parameters.Count; _i++ ) {
                    var i = _i;
                    var parameter = parameters[i];
                    portArgs[i] = AddValueInput(parameter.name, parameter.type, parameter.ID);
                }

                if ( sourceFunction.returns.type != null ) {
                    AddValueOutput(sourceFunction.returns.name, sourceFunction.returns.ID, sourceFunction.returns.type, sourceFunction.GetReturnValue);
                }

                fOut = AddFlowOutput(" ");
            }
        }

        //...
        void Invoke(Flow f) {
            if ( sourceFunction != null ) {
                if ( objectArgs == null ) {
                    objectArgs = new object[portArgs.Length];
                }
                for ( var i = 0; i < portArgs.Length; i++ ) {
                    objectArgs[i] = portArgs[i].value;
                }
                sourceFunction.InvokeAsync(f, fOut.Call, objectArgs);
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeInspectorGUI() {
            if ( GUILayout.Button("Refresh") ) { GatherPorts(); }
            if ( sourceFunction == null ) {
                var functions = graph.GetAllNodesOfType<CustomFunctionEvent>();
                var currentFunc = functions.FirstOrDefault(i => i.UID == sourceFunctionUID);
                var newFunc = EditorUtils.Popup<CustomFunctionEvent>("Target Function", currentFunc, functions);
                if ( newFunc != currentFunc ) {
                    SetFunction(newFunc);
                }
            }
            base.OnNodeInspectorGUI();
        }

#endif
        ///----------------------------------------------------------------------------------------------
    }
}