using System.Collections.Generic;
using ParadoxNotion;

namespace FlowCanvas
{

    ///Data struct that is propagated within the graph through the FlowPorts
    [ParadoxNotion.Design.SpoofAOT]
    public struct Flow
    {

        ///Contains data for Return calls
        public struct ReturnData
        {
            public FlowReturn returnCall { get; private set; }
            public System.Type returnType { get; private set; }
            public ReturnData(FlowReturn call, System.Type type) {
                returnCall = call;
                returnType = type;
            }
        }

        ///Number of ticks this Flow has made
        public int ticks;

        private Dictionary<string, object> parameters;
        private ReturnData returnData;
        private FlowBreak breakCall;

        ///Short for 'new Flow()'
        public static Flow New { get { return new Flow(); } }

        ///Alternative flow call method exactly the same as 'port.Call(f)'
        public void Call(FlowOutput port) {
            port.Call(this);
        }

        ///Read a temporary flow parameter
        public T ReadParameter<T>(string name) {
            object parameter = default(T);
            if ( parameters != null ) {
                parameters.TryGetValue(name, out parameter);
            }
            return parameter is T ? (T)parameter : default(T);
        }

        ///Write a temporary flow parameter
        public void WriteParameter<T>(string name, T value) {
            if ( parameters == null ) {
                parameters = new Dictionary<string, object>();
            }
            parameters[name] = value;
        }

        ///----------------------------------------------------------------------------------------------

        ///Set Return Data to be calledback when Return is called
        public void SetReturnData(FlowReturn call, System.Type expectedType) {
            returnData = new ReturnData(call, expectedType);
        }

        ///Invoke Return callback with provided return value 
        public void Return(object value, FlowNode context) {
            if ( returnData.returnCall == null ) {
                context.Fail("Called Return without anything to return out from.");
                return;
            }
            if ( returnData.returnType != null ) {
                var valueType = value != null ? value.GetType() : null;
                if ( valueType == null || !valueType.RTIsAssignableTo(returnData.returnType) ) {
                    context.Fail(string.Format("Return Value is not of expected type '{0}'", returnData.returnType.FriendlyName()));
                    return;
                }
            }
            if ( returnData.returnType == null && value != null ) {
                context.Warn("Returning a value when no value is required.");
            }
            returnData.returnCall(value);
        }

        ///----------------------------------------------------------------------------------------------

        ///Start a break callback
        public void BeginBreakBlock(FlowBreak callback) {
            breakCall = callback;
        }

        ///End a break callback
        public void EndBreakBlock() {
            if ( breakCall == null ) {
                ParadoxNotion.Services.Logger.LogWarning("Called EndBreakBlock wihout a previously BeginBreakBlock call.", "Execution");
                return;
            }
            breakCall = null;
        }

        ///Invoke Break callback.
        public void Break(FlowNode context) {
            if ( breakCall == null ) {
                context.Warn("Called Break without anything to break out from.");
                return;
            }
            breakCall();
        }

    }
}