#define DO_EDITOR_BINDING //comment this out to test the real performance without editor binding specifics

using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;
using UnityEngine;
using NodeCanvas;


namespace FlowCanvas
{

    public class BinderConnection : Connection
    {

        [ParadoxNotion.Serialization.FullSerializer.fsSerializeAs("_sourcePortName")]
        private string _sourcePortID;
        [ParadoxNotion.Serialization.FullSerializer.fsSerializeAs("_targetPortName")]
        private string _targetPortID;

        [System.NonSerialized]
        private Port _sourcePort;
        [System.NonSerialized]
        private Port _targetPort;

        ///The source port ID name this binder is connected to
        public string sourcePortID {
            get { return sourcePort != null ? sourcePort.ID : _sourcePortID; }
            private set { _sourcePortID = value; }
        }

        ///The target port ID name this binder is connected to
        public string targetPortID {
            get { return targetPort != null ? targetPort.ID : _targetPortID; }
            private set { _targetPortID = value; }
        }

        ///The source Port
        public Port sourcePort {
            get
            {
                if ( _sourcePort == null ) {
                    if ( sourceNode is FlowNode ) { //In case it's 'MissingNode'
                        _sourcePort = ( sourceNode as FlowNode ).GetOutputPort(_sourcePortID);
                    }
                }
                return _sourcePort;
            }
        }

        ///The target Port
        public Port targetPort {
            get
            {
                if ( _targetPort == null ) {
                    if ( targetNode is FlowNode ) { //In case it's 'MissingNode'
                        _targetPort = ( targetNode as FlowNode ).GetInputPort(_targetPortID);
                    }
                }
                return _targetPort;
            }
        }

        ///The binder type. In case of Value connection, BinderConnection<T> is used, else it's basicaly a Flow binding
        public System.Type bindingType {
            get { return GetType().RTIsGenericType() ? GetType().RTGetGenericArguments()[0] : typeof(Flow); }
        }

        ///----------------------------------------------------------------------------------------------

        ///Create a NEW BinderConnection object between two ports
        public static BinderConnection Create(Port source, Port target) {

            var verbose = CanBeBoundVerbosed(source, target);
            if ( verbose != null ) {
                ParadoxNotion.Services.Logger.LogWarning(verbose, "Editor", source.parent);
                return null;
            }

            source.parent.graph.RecordUndo("Connect Ports");

            BinderConnection binder = null;
            if ( source is FlowOutput && target is FlowInput ) {
                binder = new BinderConnection();
            }

            if ( source is ValueOutput && target is ValueInput ) {
                binder = (BinderConnection)System.Activator.CreateInstance(typeof(BinderConnection<>).RTMakeGenericType(new System.Type[] { target.type }));
            }

            if ( binder != null ) {
                binder.SetSourcePort(source, true);
                binder.SetTargetPort(target, true);

                //we call SetSource and SetTarget with 'isNew' flag, because we need call 'OnPort...' callbacks after both are set
                binder.sourcePort.connections++;
                binder.targetPort.connections++;
                binder.sourcePort.parent.OnPortConnected(binder.sourcePort, binder.targetPort);
                binder.targetPort.parent.OnPortConnected(binder.targetPort, binder.sourcePort);

                //for live editing
                if ( Application.isPlaying ) {
                    binder.Bind();
                }
            }

            return binder;
        }

        ///Set binder source port
        public void SetSourcePort(Port newSourcePort) { SetSourcePort(newSourcePort, false); }
        void SetSourcePort(Port newSourcePort, bool isNew) {
            if ( newSourcePort == sourcePort ) {
                return;
            }

            if ( newSourcePort == null || !newSourcePort.IsOutputPort() ) {
                return;
            }

            if ( sourcePort != null ) {
                sourcePort.parent.OnPortDisconnected(sourcePort, targetPort);
                sourcePort.connections--;
            }

            sourcePortID = newSourcePort.ID;
            base.SetSourceNode(newSourcePort.parent);
            if ( !isNew ) { //FIXME: Bug -> use newSourcePort
                sourcePort.parent.OnPortConnected(newSourcePort, targetPort);
                GatherAndValidateSourcePort();
            }
        }

        ///Set binder target port
        public void SetTargetPort(Port newTargetPort) { SetTargetPort(newTargetPort, false); }
        void SetTargetPort(Port newTargetPort, bool isNew) {
            if ( newTargetPort == targetPort ) {
                return;
            }

            if ( newTargetPort == null || !newTargetPort.IsInputPort() ) {
                return;
            }

            if ( targetPort != null ) {
                targetPort.parent.OnPortDisconnected(targetPort, sourcePort);
                targetPort.connections--;
            }

            targetPortID = newTargetPort.ID;
            base.SetTargetNode(newTargetPort.parent);
            if ( !isNew ) { //FIXME: Bug -> use newTargetPort
                targetPort.parent.OnPortConnected(newTargetPort, sourcePort);
                GatherAndValidateTargetPort();
            }
        }


        ///Called after the node has GatherPorts to gather the references and validate the binding connection
        public void GatherAndValidateSourcePort() {
            _sourcePort = null;
            if ( sourcePort != null && TypeConverter.HasConvertion(sourcePort.type, bindingType) ) {
                sourcePortID = sourcePort.ID;
                sourcePort.connections++;
                return;
            }

            //auto remove connections only when no using reflected node wrapper or it's member exists
            var reflectedWrapper = sourceNode as IReflectedWrapper;
            if ( reflectedWrapper == null || reflectedWrapper.GetMemberInfo() != null ) {
                graph.RemoveConnection(this, false);
            }
        }

        ///Called after the node has GatherPorts to gather the references and validate the binding connection
        public void GatherAndValidateTargetPort() {
            _targetPort = null;
            if ( targetPort != null ) {

                //all good
                if ( targetPort.type == bindingType ) {
                    targetPortID = targetPort.ID;
                    targetPort.connections++;
                    return;
                }

                //replace binder connection type if possible
                if ( targetPort is ValueInput && sourcePort is ValueOutput ) {
                    if ( TypeConverter.HasConvertion(sourcePort.type, targetPort.type) ) {
                        graph.RemoveConnection(this);
                        Create(sourcePort, targetPort);
                        targetPortID = targetPort.ID;
                        targetPort.connections++;
                        return;
                    }
                }
            }

            //auto remove connections only when no using reflected node wrapper or it's member exists
            var reflectedWrapper = targetNode as IReflectedWrapper;
            if ( reflectedWrapper == null || reflectedWrapper.GetMemberInfo() != null ) {
                graph.RemoveConnection(this, false);
            }
        }

        ///Return whether or not source can connect to target.
        public static bool CanBeBound(Port source, Port target, BinderConnection refConnection = null) { return CanBeBoundVerbosed(source, target, refConnection) == null; }
        ///Return whether or not source can connect to target. The return is a string with the reason why NOT, null if possible.
        ///Providing an existing ref connection, will bypass source/target validation respectively if that connection is already connected to that source/target port.
        public static string CanBeBoundVerbosed(Port source, Port target, BinderConnection refConnection = null) {
            if ( source == null || target == null ) {
                return "A port is null.";
            }

            if ( source == target ) {
                // return "Can't connect port to itself.";
                return string.Empty;
            }

            if ( source.parent == target.parent ) {
                return "Can't connect ports on the same node.";
            }

            if ( source.parent == target.parent ) {
                return "Can't connect ports on the same parent node.";
            }

            if ( source.IsInputPort() && target.IsInputPort() ) {
                return "Can't connect input to input.";
            }

            if ( source.IsOutputPort() && target.IsOutputPort() ) {
                return "Can't connect output to output.";
            }

            if ( source.IsFlowPort() != target.IsFlowPort() ) {
                return "Flow ports can only be connected to other Flow ports.";
            }

            if ( refConnection == null || refConnection.sourcePort != source ) {
                if ( !source.CanAcceptConnections() ) {
                    return "Source port can accept no more out connections.";
                }
            }

            if ( refConnection == null || refConnection.targetPort != target ) {
                if ( !target.CanAcceptConnections() ) {
                    return "Target port can accept no more in connections.";
                }
            }

            if ( !TypeConverter.HasConvertion(source.type, target.type) ) {
                return string.Format("Can't connect ports. Type '{0}' is not assignable from Type '{1}' and there exists no automatic conversion for those types.", target.type.FriendlyName(), source.type.FriendlyName());
            }

            return null;
        }

        ///Callback from base class. The connection reference is already removed from target and source Nodes
        sealed public override void OnDestroy() {
            if ( sourcePort != null ) {
                sourcePort.parent.OnPortDisconnected(sourcePort, targetPort);
                sourcePort.connections--;
            }
            if ( targetPort != null ) {
                targetPort.parent.OnPortDisconnected(targetPort, sourcePort);
                targetPort.connections--;
            }

            //for live editing
            if ( Application.isPlaying ) {
                UnBind();
            }
        }

        ///Called in runtime intialize to actualy bind the delegates
        virtual public void Bind() {

            if ( !isActive ) {
                return;
            }

            if ( sourcePort is FlowOutput && targetPort is FlowInput ) {
                ( sourcePort as FlowOutput ).BindTo((FlowInput)targetPort);

#if UNITY_EDITOR && DO_EDITOR_BINDING
                ( sourcePort as FlowOutput ).Append(BlinkStatus);
#endif
            }
        }

        ///UnBinds the delegates
        virtual public void UnBind() {
            if ( sourcePort is FlowOutput ) {
                ( sourcePort as FlowOutput ).UnBind();
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        private int lastBlinkFrame;

        public override TipConnectionStyle tipConnectionStyle { get { return TipConnectionStyle.None; } }
        public override ParadoxNotion.PlanarDirection direction { get { return ParadoxNotion.PlanarDirection.Horizontal; } }

        public override Color defaultColor {
            get { return bindingType == typeof(Flow) ? base.defaultColor : Color.grey; }
        }

        public override float defaultSize {
            get { return bindingType == typeof(Flow) ? 4 : base.defaultSize; }
        }

        protected override string GetConnectionInfo() {
            if ( sourcePort == null || targetPort == null ) { return null; }

            if ( targetPort.willDraw ) {

                if ( Application.isPlaying ) {
                    return GetTransferDataLabel();
                }

                if ( !targetPort.type.IsAssignableFrom(sourcePort.type) ) {
                    return "<size=14>➲</size>";
                }
            }

            return null;
        }

        //Data label to show on binder info
        virtual protected string GetTransferDataLabel() {
            return null;
        }

        ///Blinks connection status
        protected void BlinkStatus(Flow f = default(Flow)) {
            if ( Application.isPlaying ) {
                lastBlinkFrame = Time.frameCount;
                if ( status == Status.Resting ) {
                    if ( MonoManager.current != null ) {
                        MonoManager.current.StartCoroutine(MonitorBlink(f));
                    }
                }
                status = Status.Running;
            }
        }

        ///Monitors status to reset
        IEnumerator MonitorBlink(Flow f) {
            while ( Time.frameCount <= lastBlinkFrame ) {
                yield return null;
            }
            status = Status.Resting;
        }

#endif
        ///----------------------------------------------------------------------------------------------

    }
}