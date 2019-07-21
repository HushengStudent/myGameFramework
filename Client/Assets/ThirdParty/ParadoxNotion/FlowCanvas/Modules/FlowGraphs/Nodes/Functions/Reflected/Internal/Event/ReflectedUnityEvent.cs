using System;
using System.Reflection;
using System.Linq;
using ParadoxNotion;
using UnityEngine.Events;

namespace FlowCanvas.Nodes
{

    [ParadoxNotion.Design.SpoofAOT]
    public class ReflectedUnityEvent
    {

        public delegate void UnityEventCallback(params object[] args);

        private event UnityEventCallback _callback;

        private Type _eventType;
        private MethodInfo _addListenerMethod;
        private MethodInfo _removeListenerMethod;
        private ParameterInfo[] _parameters;
        private MethodInfo _callMethod;

        public ParameterInfo[] parameters {
            get { return _parameters; }
        }

        public Type eventType {
            get { return _eventType; }
        }

        public ReflectedUnityEvent() { }

        ///Create a reflected unity event for target unity event type
        public ReflectedUnityEvent(Type eventType) {
            InitForEventType(eventType);
        }

        ///Initialize for target unity event type
        public void InitForEventType(Type eventType) {
            this._eventType = eventType;
            if ( _eventType == null || !_eventType.RTIsSubclassOf(typeof(UnityEventBase)) ) {
                return;
            }
            var invokeMethod = _eventType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
            _parameters = invokeMethod.GetParameters();
            var thisType = this.GetType();
            if ( _parameters.Length == 0 ) { _callMethod = thisType.RTGetMethod("CallbackMethod0"); }
            if ( _parameters.Length == 1 ) { _callMethod = thisType.RTGetMethod("CallbackMethod1"); }
            if ( _parameters.Length == 2 ) { _callMethod = thisType.RTGetMethod("CallbackMethod2"); }
            if ( _parameters.Length == 3 ) { _callMethod = thisType.RTGetMethod("CallbackMethod3"); }
            if ( _parameters.Length == 4 ) { _callMethod = thisType.RTGetMethod("CallbackMethod4"); }
            if ( _callMethod.IsGenericMethodDefinition ) {
                _callMethod = _callMethod.MakeGenericMethod(_parameters.Select(p => p.ParameterType).ToArray());
            }

            var typeArray = new Type[] { typeof(object), typeof(MethodInfo) };
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            _addListenerMethod = typeof(UnityEventBase).GetMethod("AddListener", flags, null, typeArray, null);
            _removeListenerMethod = typeof(UnityEventBase).GetMethod("RemoveListener", flags, null, typeArray, null);
        }

        ///Subscribe to target unity event
        public void StartListening(UnityEventBase targetEvent, UnityEventCallback callback) {
            this._callback += callback;
            _addListenerMethod.Invoke(targetEvent, new object[] { this, _callMethod });
        }

        ///Unsubscribe from target unity event
        public void StopListening(UnityEventBase targetEvent, UnityEventCallback callback) {
            this._callback -= callback;
            _removeListenerMethod.Invoke(targetEvent, new object[] { this, _callMethod });
        }

        public void CallbackMethod0() { _callback(); }
        public void CallbackMethod1<T0>(T0 arg0) { _callback(arg0); }
        public void CallbackMethod2<T0, T1>(T0 arg0, T1 arg1) { _callback(arg0, arg1); }
        public void CallbackMethod3<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2) { _callback(arg0, arg1, arg2); }
        public void CallbackMethod4<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3) { _callback(arg0, arg1, arg2, arg3); }
    }
}