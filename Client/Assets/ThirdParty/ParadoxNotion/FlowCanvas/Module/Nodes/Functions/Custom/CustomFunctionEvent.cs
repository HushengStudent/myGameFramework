using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using NodeCanvas.Framework;

namespace FlowCanvas.Nodes{

    [Name("New Custom Function", 10)]
    [Description("A custom function, defined by any number of parameters and an optional return value. It can be called using the 'Call Custom Function' node. To return a value, the 'Return' node should be used.")]
    [Category("Functions/Custom")]
    [ParadoxNotion.Serialization.DeserializeFrom("FlowCanvas.Nodes.RelayFlowOutput")]
    public class CustomFunctionEvent : EventNode, IInvokable, IEditorMenuCallbackReceiver {

        [Tooltip("The identifier name of the function")] [DelayedField]
        public string identifier = "MyFunction";
        [SerializeField]
        private List<DynamicPortDefinition> _parameters = new List<DynamicPortDefinition>();
        [SerializeField]
        private DynamicPortDefinition _returns = new DynamicPortDefinition("Value", null);

        private object[] args;
        private object returnValue;
        private FlowOutput onInvoke;

        ///the parameters port definition
        public List<DynamicPortDefinition> parameters{
            get {return _parameters;}
            private set {_parameters = value;}
        }

        ///the return port definition
        public DynamicPortDefinition returns{
            get {return _returns;}
            private set {_returns = value;}
        }

        //shortcut
        private System.Type returnType{
            get {return returns.type; }
        }

        //shortcut
        private System.Type[] parameterTypes{
            get {return parameters.Select(p => p.type).ToArray();}
        }

        public override string name{
            get { return "➥ " + identifier; }
        }

        protected override void RegisterPorts(){
            onInvoke = AddFlowOutput(" ");
            for (var _i = 0; _i < parameters.Count; _i++){
                var i = _i;
                var parameter = parameters[i];
                AddValueOutput(parameter.name, parameter.ID, parameter.type, ()=>{ return args[i]; });
            }
        }

        string IInvokable.GetInvocationID(){ return identifier; }
        object IInvokable.Invoke(params object[] args){ return Invoke(new Flow(), args); }

        ///Invokes the function and return it's return value
        public object Invoke(Flow f, params object[] args){
            this.args = args;
            f.ReturnType = returns.type;
            f.Return = (o)=>{ this.returnValue = o; };
            onInvoke.Call(f);
            return returnValue;
        }

        ///Invokes the function and callbacks when a Return node is hit.
        public void InvokeAsync(Flow f, FlowHandler Callback, params object[] args){
            this.args = args;
            f.ReturnType = returns.type;
            f.Return = (o)=>{ this.returnValue = o; Callback(f); };
            onInvoke.Call(f);            
        }

        ///Returns the function's last return value
        public object GetReturnValue(){
            return returnValue;
        }

        //Add a parameter to the function
        void AddParameter(System.Type type){
            parameters.Add( new DynamicPortDefinition(type.FriendlyName(), type) );
            GatherPortsUpdateRefs();
        }

        //Helper
        void GatherPortsUpdateRefs(){
            this.GatherPorts();
            foreach(var call in flowGraph.GetAllNodesOfType<CustomFunctionCall>().Where(n => n.sourceFunction == this)){
                call.GatherPorts();
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
        #if UNITY_EDITOR
        
        void IEditorMenuCallbackReceiver.OnMenu(UnityEditor.GenericMenu menu, Vector2 pos, Port sourcePort, object dropInstance){
            if (sourcePort != null){
                if (sourcePort is ValueInput && !sourcePort.type.IsAssignableFrom(returnType)){ return; }
                if (sourcePort is ValueOutput && !parameterTypes.Any(t => t.IsAssignableFrom(sourcePort.type))){ return; }
            }
			menu.AddItem(new GUIContent(string.Format("Functions/Custom/Call '{0}()'", identifier) ), false, ()=>
            {
                var node = flowGraph.AddNode<CustomFunctionCall>(pos);
                node.SetFunction(this);
                FlowGraphExtensions.Finalize(node, sourcePort, null);
            });
        }

        protected override void OnNodeInspectorGUI(){
            base.OnNodeInspectorGUI();

            if (GUILayout.Button("Add Parameter")){
                EditorUtils.GetPreferedTypesSelectionMenu(typeof(object), (t)=>{ AddParameter(t); } ).ShowAsContext();
            }

            UnityEditor.EditorGUI.BeginChangeCheck();

			var options = new EditorUtils.ReorderableListOptions();
			options.allowRemove = true;
			EditorUtils.ReorderableList(parameters, options, (i, r)=>
            {
                var parameter = parameters[i];
                GUILayout.BeginHorizontal();
                parameter.name = UnityEditor.EditorGUILayout.DelayedTextField(parameter.name, GUILayout.Width(150), GUILayout.ExpandWidth(true));
                EditorUtils.ButtonTypePopup("", parameter.type, (t)=>{ parameter.type = t; GatherPortsUpdateRefs(); });
                GUILayout.EndHorizontal();
			});

            EditorUtils.Separator();

            EditorUtils.ButtonTypePopup("Return Type", returns.type, (t)=>{ returns.type = t; GatherPortsUpdateRefs(); } );

            if (UnityEditor.EditorGUI.EndChangeCheck()){
                GatherPortsUpdateRefs();
            }
        }
        
        #endif
        ///----------------------------------------------------------------------------------------------
    }
}