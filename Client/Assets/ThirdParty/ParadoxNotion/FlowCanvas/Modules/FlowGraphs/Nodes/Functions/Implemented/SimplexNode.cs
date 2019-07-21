using System;
using System.Collections;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace FlowCanvas.Nodes
{

    ///SimplexNodes are used within a SimplexNodeWrapper node.
    ///Derive CallableActionNode, CallableFunctionNode, LatentActionNode, PureFunctionNode or ExtractorNode, for creating simple nodes easily and type cast safe.
    [ParadoxNotion.Design.SpoofAOT]
    abstract public class SimplexNode
    {

        [System.NonSerialized]
        private string _name;
        [System.NonSerialized]
        private string _description;

        ///A reference to the parent node this SimplexNode lives within
        protected FlowNode parentNode { get; private set; }

        virtual public string name {
            get
            {
                if ( string.IsNullOrEmpty(_name) ) {
                    var nameAtt = this.GetType().RTGetAttribute<NameAttribute>(false);
                    _name = nameAtt != null ? nameAtt.name : this.GetType().FriendlyName().SplitCamelCase();
                }
                return _name;
            }
        }

        virtual public string description {
            get
            {
                if ( string.IsNullOrEmpty(_description) ) {
                    var descAtt = this.GetType().RTGetAttribute<DescriptionAttribute>(false);
                    _description = descAtt != null ? descAtt.description : "No Description";
                }
                return _description;
            }
        }


        private ParameterInfo[] _parameters;
        public ParameterInfo[] parameters {
            get
            {
                if ( _parameters != null ) { return _parameters; }
                var invokeMethod = this.GetType().GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                return _parameters = invokeMethod != null ? invokeMethod.GetParameters() : new ParameterInfo[0];
            }
        }

        public void RegisterPorts(FlowNode node) {

            this.parentNode = node;

            OnRegisterPorts(node);

            //output ports are done through public properties for all simplex nodes
            var type = this.GetType();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            for ( var i = 0; i < props.Length; i++ ) {
                var prop = props[i];
                if ( prop.CanRead && !prop.GetGetMethod().IsVirtual ) {
                    node.TryAddPropertyValueOutput(prop, this);
                }
            }

            OnRegisterExtraPorts(node);
        }

        //set default parameter values if any
        public void SetDefaultParameters(FlowNode node) {
            if ( parameters == null ) {
                return;
            }
            for ( var i = 0; i < parameters.Length; i++ ) {
                var parameter = parameters[i];
                if ( parameter.IsOptional && parameter.DefaultValue != null ) {
                    var inPort = node.GetInputPort(parameter.Name) as ValueInput;
                    if ( inPort != null ) {
                        inPort.serializedValue = parameter.DefaultValue;
                    }
                }
            }
        }


        ///-------------------------------------------------------------------------

        abstract protected void OnRegisterPorts(FlowNode node);
        virtual protected void OnRegisterExtraPorts(FlowNode node) { }
        virtual public void OnGraphStarted() { }
        virtual public void OnGraphPaused() { }
        virtual public void OnGraphUnpaused() { }
        virtual public void OnGraphStoped() { }
    }
}