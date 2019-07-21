#define DO_EDITOR_BINDING //comment this out to test the real performance without editor binding specifics

using System;
using System.Linq;
using ParadoxNotion;
using UnityEngine;

namespace FlowCanvas
{
    ///Base Port class for all port types
    [ParadoxNotion.Design.SpoofAOT]
    abstract public class Port
    {

#if UNITY_EDITOR

        ///Editor display name
        internal string displayName { get; private set; }
        ///Editor display content
        internal GUIContent displayContent { get; private set; }
        ///Editor is port drawn in gui
        internal bool willDraw { get; set; }
        ///Editor port highlight flag
        internal bool highlightFlag { get; set; }
        ///Editor relative y offset of the port in relation to it's parent
        internal float posOffsetY { get; set; }
        ///Editor position of port
        internal Vector2 pos { get { return rect.center; } }
        ///Editor rect of port
        internal Rect rect {
            get
            {
                var result = new Rect(0, 0, 12, 12);
                var finalOffset = willDraw ? posOffsetY : Mathf.Min(parent.rect.height / 2, 18);
                if ( IsInputPort() ) {
                    result.center = new Vector2(parent.rect.xMin - 5, parent.rect.y + finalOffset);
                }
                if ( IsOutputPort() ) {
                    result.center = new Vector2(parent.rect.xMax + 5, parent.rect.y + finalOffset);
                }
                return result;
            }
        }

        ///Editor GUI Caching of port
        internal void EnsureCachedGUIContent() {
            if ( displayContent != null ) { return; }
            var content = new GUIContent();
            displayName = name.SplitCamelCase();
            var displayNameCaped = displayName.CapLength(25);
            if ( IsFlowPort() ) {
                if ( this is FlowInput ) {
                    content.text = string.Format("<b>► {0}</b>", displayNameCaped);
                } else {
                    content.text = string.Format("<b>{0} ►</b>", displayNameCaped);
                }
            } else {
                var hexColor = ColorUtils.ColorToHex(ParadoxNotion.Design.TypePrefs.GetTypeColor(type));
                var text = string.Format("<color={0}>{1}{2}</color>", hexColor, IsEnumerableCollection() ? "#" : string.Empty, displayNameCaped);
                Texture icon = null;
                if ( NodeCanvas.Editor.Prefs.showIcons ) {
                    var wildDefType = parent.GetNodeWildDefinitionType();
                    var isWildDef = wildDefType != null && ( wildDefType == type || wildDefType == type.GetEnumerableElementType() );
                    icon = ParadoxNotion.Design.TypePrefs.GetTypeIcon(isWildDef ? typeof(Wild) : type);
                }

                if ( IsDelegate() ) {
                    text = string.Format("<b><color={0}>[{1}]</color></b>", hexColor, text);
                }
                content.text = text;
                content.image = icon;
            }

            content.tooltip = displayName.Length > displayNameCaped.Length ? displayName : string.Empty;
            displayContent = content;
        }
#endif

        ///----------------------------------------------------------------------------------------------

        public Port() { }
        public Port(FlowNode parent, string name, string ID) {
            this.parent = parent;
            this.name = name;
            this.ID = ID;
        }

        ///The FlowNode parent reference of the port
        public FlowNode parent { get; private set; }
        ///The ID name of the port. Usualy is the same as the name
        public string ID { get; private set; }
        ///The name of the port
        public string name { get; private set; }
        ///The number of connections the port currently has
        public int connections { get; set; }
        ///Is the port connected?
        public bool isConnected { get { return connections > 0; } }

        ///The type of the port
        abstract public Type type { get; }

        ///----------------------------------------------------------------------------------------------

        ///Helper method to determine if a port can accept further connections
        public bool CanAcceptConnections() {
            if ( this is ValueOutput || ( this is FlowOutput && !this.isConnected ) ) {
                return true;
            }
            if ( this is FlowInput || ( this is ValueInput && !this.isConnected ) ) {
                return true;
            }
            return false;
        }

        ///Get all BinderConnections the port is using
        public BinderConnection[] GetPortConnections() {
            if ( IsInputPort() ) {
                return parent.inConnections.OfType<BinderConnection>().Where(c => c.targetPort == this).ToArray();
            }
            return parent.outConnections.OfType<BinderConnection>().Where(c => c.sourcePort == this).ToArray();
        }

        ///Gets the first input BinderConnection of the port
        public BinderConnection GetFirstInputConnection() {
            return parent.inConnections.OfType<BinderConnection>().FirstOrDefault(c => c.targetPort == this);
        }

        ///Gets the first output BinderConnection of the port
        public BinderConnection GetFirstOutputConnection() {
            return parent.outConnections.OfType<BinderConnection>().FirstOrDefault(c => c.sourcePort == this);
        }

        public bool IsFlowPort() {
            return ( this is FlowInput ) || ( this is FlowOutput );
        }

        public bool IsValuePort() {
            return ( this is ValueInput ) || ( this is ValueOutput );
        }

        public bool IsInputPort() {
            return ( this is FlowInput ) || ( this is ValueInput );
        }

        public bool IsOutputPort() {
            return ( this is FlowOutput ) || ( this is ValueOutput );
        }

        public bool IsUnityObject() {
            return typeof(UnityEngine.Object).RTIsAssignableFrom(type);
        }

        public bool IsUnitySceneObject() {
            return typeof(Component).RTIsAssignableFrom(type) || type == typeof(GameObject);
        }

        public bool IsDelegate() {
            return typeof(Delegate).RTIsAssignableFrom(type);
        }

        public bool IsEnumerableCollection() {
            return type.IsEnumerableCollection();
        }

        public bool IsWild() {
            return type == typeof(Wild);
        }

        public override string ToString() {
            return name.SplitCamelCase();
        }
    }


    ///----------------------------------------------------------------------------------------------
    ///FLOW PORTS
    ///----------------------------------------------------------------------------------------------

    ///Input port for Flow type
    public class FlowInput : Port
    {

        ///pointer refers to callback when the port is called
        public FlowInput(FlowNode parent, string name, string ID, FlowHandler pointer) : base(parent, name, ID) {
            this.pointer = pointer;
        }

        ///Used for port binding. Points to the call for when port is called
        public FlowHandler pointer { get; private set; }
        ///The type of the port which is always type of Flow
        public override Type type { get { return typeof(Flow); } }
    }

    ///Output port for Flow type
    public class FlowOutput : Port
    {

        public FlowOutput(FlowNode parent, string name, string ID) : base(parent, name, ID) { }

        ///Used for port binding. Points to a FlowInput pointer if ports are connected
        public FlowHandler pointer { get; private set; }
        ///The type of the port which is always type of Flow
        public override Type type { get { return typeof(Flow); } }

        ///Calls the target bound pointer
        public void Call(Flow f) {
            if ( pointer != null && !parent.graph.isPaused ) {
                f.ticks++;

#if UNITY_EDITOR && DO_EDITOR_BINDING

                if ( parent.isBreakpoint ) {
                    ParadoxNotion.Services.MonoManager.current.StartCoroutine(DebugBreakWait(f));
                    return;
                }

                Continue(f);

#else

				pointer(f);

#endif

            }
        }

#if UNITY_EDITOR && DO_EDITOR_BINDING //Breakpoint helpers
        System.Collections.IEnumerator DebugBreakWait(Flow f) {
            ParadoxNotion.Services.Logger.LogWarning(string.Format("On Node '{0}'", parent.name), "Breakpoint", parent);
            UnityEngine.Debug.Break();
            parent.SetStatus(NodeCanvas.Framework.Status.Running);
            yield return null;
            parent.SetStatus(NodeCanvas.Framework.Status.Resting);
            Continue(f);
        }

        void Continue(Flow f) {
            try { pointer(f); }
            catch ( Exception e ) {
                var connection = GetFirstOutputConnection();
                var targetNode = (FlowNode)connection.targetNode;
                targetNode.Error(e);
            }
        }
#endif


        ///Bind the port to the target FlowInput
        public void BindTo(FlowInput target) {
            this.pointer = target.pointer;
        }

        ///Appends a delegate when port is called
        public void Append(FlowHandler action) {
            this.pointer += action;
        }

        ///Unbinds the port
        public void UnBind() {
            this.pointer = null;
        }
    }


    ///----------------------------------------------------------------------------------------------
    ///VALUE INPUT PORT
    ///----------------------------------------------------------------------------------------------

    ///Base input port for values
    abstract public class ValueInput : Port
    {

        public ValueInput() { }
        public ValueInput(FlowNode parent, string name, string ID) : base(parent, name, ID) { }

        ///Creates a generic instance of ValueInput
        public static ValueInput<T> CreateInstance<T>(FlowNode parent, string name, string ID) {
            return new ValueInput<T>(parent, name, ID);
        }

        ///Creates a generic instance of ValueInput
        public static ValueInput CreateInstance(Type t, FlowNode parent, string name, string ID) {
            return (ValueInput)Activator.CreateInstance(typeof(ValueInput<>).RTMakeGenericType(t), new object[] { parent, name, ID });
        }

        ///The value as object type when accessed as ValueInput
        public object value {
            get { return GetObjectValue(); }
        }

        public ValueInput SetDefaultAndSerializedValue(object v) {
            this.defaultValue = v;
            this.serializedValue = v;
            return this;
        }

        abstract public object defaultValue { get; set; }
        abstract public object serializedValue { get; set; }
        abstract public bool isDefaultValue { get; }
        abstract public override Type type { get; }
        abstract public void BindTo(ValueOutput target);
        abstract public void UnBind();
        abstract public object GetObjectValue();
    }

    ///Value Input port for a known type. .value refers to either connected binded port if any, or the serialized exposed value
    public class ValueInput<T> : ValueInput
    {

        public ValueInput() { }
        public ValueInput(FlowNode parent, string name, string ID) : base(parent, name, ID) { }

        ///Used for port binding
        public ValueHandler<T> getter { get; private set; }

        private T _value;
        private T _defaultValue;

        ///The port value
        new public T value {
            get
            {
                if ( getter != null ) {

#if UNITY_EDITOR && DO_EDITOR_BINDING

                    try { return getter(); }
                    catch ( Exception e ) {
                        var connection = GetFirstInputConnection();
                        var targetNode = (FlowNode)connection.sourceNode;
                        targetNode.Error(e);
                    }

#else

					return getter();

#endif
                }

                return _value;
            }
        }

        ///The value which is considered default
        public override object defaultValue {
            get { return _defaultValue; }
            set { _defaultValue = (T)value; }
        }

        ///Used to get/set the serialized value directly
        public override object serializedValue {
            get { return _value; }
            set { _value = (T)value; }
        }

        ///Returns if the serializedValue is equal to the default value, usually simply default(T)
        public override bool isDefaultValue {
            get { return Equals(serializedValue, defaultValue); }
        }

        ///The port value type which is always of type T
        public override Type type { get { return typeof(T); } }

        ///Sets the default and serialized value of the port. Can be used when registering ports.
        public ValueInput<T> SetDefaultAndSerializedValue(T v) {
            this._defaultValue = v;
            this._value = v;
            return this;
        }

        ///Get the value same as .value (can be used for delegate since properties can't directly)
        public T GetValue() {
            return value;
        }

        ///Get the value as object
        public override object GetObjectValue() {
            return value;
        }

        ///Binds the port to the target source ValueOutput port
        public override void BindTo(ValueOutput source) {
            //if same T use directly
            if ( source is ValueOutput<T> ) {
                this.getter = ( source as ValueOutput<T> ).getter;
                return;
            }

            //otherwise use the converter
            this.getter = TypeConverter.GetConverterFuncFromTo<T>(source.type, typeof(T), source.GetObjectValue);
        }

        ///Binds the port to a delegate directly
        public void BindTo(ValueHandler<T> getter) {
            this.getter = getter;
        }

        ///Unbinds the port
        public override void UnBind() {
            this.getter = null;
        }

        //operator
        public static explicit operator T(ValueInput<T> port) {
            return port.value;
        }
    }


    ///----------------------------------------------------------------------------------------------
    ///VALUE OUTPUT PORT
    ///----------------------------------------------------------------------------------------------

    ///Base output port for values
    abstract public class ValueOutput : Port
    {

        public ValueOutput() { }
        public ValueOutput(FlowNode parent, string name, string ID) : base(parent, name, ID) { }

        ///Creates a generic instance of ValueOutput
        public static ValueOutput<T> CreateInstance<T>(FlowNode parent, string name, string ID, ValueHandler<T> getter) {
            return new ValueOutput<T>(parent, name, ID, getter);
        }

        ///Creates a generic instance of ValueOutput
        public static ValueOutput CreateInstance(Type t, FlowNode parent, string name, string ID, ValueHandlerObject getter) {
            return (ValueOutput)Activator.CreateInstance(typeof(ValueOutput<>).RTMakeGenericType(t), new object[] { parent, name, ID, getter });
        }

        ///Used only in case that a binder required casting cause of different port types (a conversion)
        abstract public object GetObjectValue();
    }

    ///An output value port
    public class ValueOutput<T> : ValueOutput
    {

        public ValueOutput() { }

        //normal
        public ValueOutput(FlowNode parent, string name, string ID, ValueHandler<T> getter) : base(parent, name, ID) {
            this.getter = getter;
        }

        //casted
        public ValueOutput(FlowNode parent, string name, string ID, ValueHandlerObject getter) : base(parent, name, ID) {
            this.getter = () => { return (T)getter(); };
        }

        ///Used for port binding
        public ValueHandler<T> getter { get; private set; }

        ///Used only in case that a binder required casting cause of different port types
        public override object GetObjectValue() { return (object)getter(); }
        ///The type of the port which is always of type T
        public override Type type { get { return typeof(T); } }

        //operator
        public static explicit operator T(ValueOutput<T> port) {
            return port.getter();
        }
    }
}