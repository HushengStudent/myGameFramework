using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using NodeCanvas.Framework;
using Logger = ParadoxNotion.Services.Logger;

namespace FlowCanvas.Nodes
{

    [Name("New Custom Function", 10)]
    [Description("A custom function, defined by any number of parameters and an optional return value. It can be called using the 'Call Custom Function' node. To end the function and optionally return a value, the 'Return' node should be used.")]
    [Category("Functions/Custom")]
    [ParadoxNotion.Serialization.DeserializeFrom("FlowCanvas.Nodes.RelayFlowOutput")]
    public class CustomFunctionEvent : EventNode, IInvokable, IEditorMenuCallbackReceiver
    {

        [Tooltip("The identifier name of the function")]
        [DelayedField]
        public string identifier = "MyFunction";
        [SerializeField]
        private List<DynamicPortDefinition> _parameters = new List<DynamicPortDefinition>();
        [SerializeField]
        private DynamicPortDefinition _returns = new DynamicPortDefinition("Value", null);

        private object[] args;
        private object returnValue;
        private FlowOutput onInvoke;
        private bool isInvoking;

        ///the parameters port definition
        public List<DynamicPortDefinition> parameters {
            get { return _parameters; }
            private set { _parameters = value; }
        }

        ///the return port definition
        public DynamicPortDefinition returns {
            get { return _returns; }
            private set { _returns = value; }
        }

        //shortcut
        private System.Type returnType {
            get { return returns.type; }
        }

        //shortcut
        private System.Type[] parameterTypes {
            get { return parameters.Select(p => p.type).ToArray(); }
        }

        public override string name {
            get { return "➥ " + identifier; }
        }

        protected override void RegisterPorts() {
            onInvoke = AddFlowOutput(" ");
            for ( var _i = 0; _i < parameters.Count; _i++ ) {
                var i = _i;
                var parameter = parameters[i];
                AddValueOutput(parameter.name, parameter.ID, parameter.type, () => { return args[i]; });
            }
        }

        string IInvokable.GetInvocationID() { return identifier; }
        object IInvokable.Invoke(params object[] args) { return Invoke(new Flow(), args); }
        void IInvokable.InvokeAsync(System.Action<object> callback, params object[] args) {
            InvokeAsync(new Flow(), (f) => { callback(returnValue); }, args);
        }


        ///Invokes the function and return it's return value
        public object Invoke(Flow f, params object[] args) {
            if ( isInvoking ) {
                Logger.LogWarning("Invoking a Custom Function which is already running.", "Execution", this);
            }
            this.args = args;
            isInvoking = true;
            FlowReturn returnCallback = (o) => { this.returnValue = o; isInvoking = false; };
            var invocationFlow = new Flow();
            invocationFlow.SetReturnData(returnCallback, returns.type);
            onInvoke.Call(invocationFlow);
            return returnValue;
        }

        ///Invokes the function and callbacks when a Return node is hit.
        public void InvokeAsync(Flow f, FlowHandler flowCallback, params object[] args) {
            if ( isInvoking ) {
                Logger.LogWarning("Invoking a Custom Function which is already running.", "Execution", this);
            }
            this.args = args;
            isInvoking = true;
            FlowReturn returnCallback = (o) => { this.returnValue = o; isInvoking = false; flowCallback(f); };
            var invocationFlow = new Flow();
            invocationFlow.SetReturnData(returnCallback, returns.type);
            onInvoke.Call(invocationFlow);
        }

        ///Returns the function's last return value
        public object GetReturnValue() {
            return returnValue;
        }

        //Add a parameter to the function
        void AddParameter(System.Type type) {
            parameters.Add(new DynamicPortDefinition(type.FriendlyName(), type));
            GatherPortsUpdateRefs();
        }

        //Helper
        void GatherPortsUpdateRefs() {
            this.GatherPorts();
            foreach ( var call in flowGraph.GetAllNodesOfType<CustomFunctionCall>().Where(n => n.sourceFunction == this) ) {
                call.GatherPorts();
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        void IEditorMenuCallbackReceiver.OnMenu(UnityEditor.GenericMenu menu, Vector2 pos, Port contextPort, object dropInstance) {
            if ( contextPort != null ) {
                if ( contextPort is ValueInput && !contextPort.type.IsAssignableFrom(returnType) ) { return; }
                if ( contextPort is ValueOutput && !parameterTypes.Any(t => t.IsAssignableFrom(contextPort.type)) ) { return; }
            }
            menu.AddItem(new GUIContent(string.Format("Functions/Custom/Call '{0}()'", identifier)), false, () =>
           {
               flowGraph.AddFlowNode<CustomFunctionCall>(pos, contextPort, dropInstance).SetFunction(this);
           });
        }

        protected override void OnNodeInspectorGUI() {
            base.OnNodeInspectorGUI();

            if ( GUILayout.Button("Add Parameter") ) {
                EditorUtils.GetPreferedTypesSelectionMenu(typeof(object), (t) => { AddParameter(t); }).ShowAsContext();
            }

            UnityEditor.EditorGUI.BeginChangeCheck();

            var options = new EditorUtils.ReorderableListOptions();
            options.allowRemove = true;
            EditorUtils.ReorderableList(parameters, options, (i, r) =>
            {
                var parameter = parameters[i];
                GUILayout.BeginHorizontal();
                parameter.name = UnityEditor.EditorGUILayout.DelayedTextField(parameter.name, GUILayout.Width(150), GUILayout.ExpandWidth(true));
                EditorUtils.ButtonTypePopup("", parameter.type, (t) => { parameter.type = t; GatherPortsUpdateRefs(); });
                GUILayout.EndHorizontal();
            });

            EditorUtils.Separator();

            EditorUtils.ButtonTypePopup("Return Type", returns.type, (t) => { returns.type = t; GatherPortsUpdateRefs(); });

            if ( UnityEditor.EditorGUI.EndChangeCheck() ) {
                GatherPortsUpdateRefs();
            }
        }

#endif
        ///----------------------------------------------------------------------------------------------
    }
}