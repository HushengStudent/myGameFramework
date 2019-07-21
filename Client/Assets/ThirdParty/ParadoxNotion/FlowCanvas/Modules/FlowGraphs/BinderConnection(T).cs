#define DO_EDITOR_BINDING //comment this out to test the real performance without editor binding specifics

using UnityEngine;
using ParadoxNotion;
using NodeCanvas;

namespace FlowCanvas
{

    ///Value bindings use the generic version of FlowBinderConnection.
    ///T is always the same at the 'target' ValueInput type.
    public class BinderConnection<T> : BinderConnection
    {

        ///Binds source and target value ports
        public override void Bind() {

            if ( !isActive ) {
                return;
            }

#if UNITY_EDITOR && DO_EDITOR_BINDING
            DoEditorBinding(sourcePort, targetPort);
#else
			DoNormalBinding(sourcePort, targetPort);
#endif
        }

        ///Unbinds source and target value ports
        public override void UnBind() {
            if ( targetPort is ValueInput ) {
                ( targetPort as ValueInput ).UnBind();
            }
        }

        //Normal binder from source Output, to target Input
        void DoNormalBinding(Port source, Port target) {
            ( target as ValueInput<T> ).BindTo((ValueOutput)source);
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        private T transferValue;
        private ValueHandler<T> getter;
        private bool hasExecute = false;

        protected override string GetTransferDataLabel() {
            if ( Application.isPlaying && hasExecute ) {
                return transferValue.ToStringAdvanced().CapLength(25);
            }
            return null;
        }

        protected override void OnConnectionInspectorGUI() {
            if ( sourcePort == null || targetPort == null ) { return; }
            if ( !targetPort.type.IsAssignableFrom(sourcePort.type) ) {
                GUILayout.Label(string.Format("AutoConvert: {0} ➲ {1}", sourcePort.type.FriendlyName(), targetPort.type.FriendlyName()));
            }
        }

        //Intermediate function used to store the value so we can show it
        T GetValue() {
            hasExecute = true;
            transferValue = this.getter();
            base.BlinkStatus();
            return transferValue;
        }

        //The editor binding is only for debuging values on top of connections + connection blinking.
        void DoEditorBinding(Port source, Port target) {
            if ( source is ValueOutput<T> ) {
                this.getter = ( source as ValueOutput<T> ).getter;
                ( target as ValueInput<T> ).BindTo(this.GetValue);
                return;
            }

            this.getter = TypeConverter.GetConverterFuncFromTo<T>(source.type, typeof(T), ( source as ValueOutput ).GetObjectValue);
            ( target as ValueInput<T> ).BindTo(this.GetValue);
        }

#endif
        ///----------------------------------------------------------------------------------------------

    }
}