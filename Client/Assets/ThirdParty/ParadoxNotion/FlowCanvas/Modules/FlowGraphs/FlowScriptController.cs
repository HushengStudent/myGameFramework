using NodeCanvas.Framework;

namespace FlowCanvas
{

    ///Add this component on a game object to be controlled by a FlowScript
    [UnityEngine.AddComponentMenu("FlowCanvas/FlowScript Controller")]
    public class FlowScriptController : GraphOwner<FlowScript>
    {

        ///Calls and returns a value of a custom function in the flowgraph
        public object CallFunction(string name, params object[] args) {
            return behaviour.CallFunction(name, args);
        }

        ///Calls a custom function in the flowgraph async. When the function is done, it will callback with return value
        public void CallFunctionAsync(string name, System.Action<object> callback, params object[] args) {
            behaviour.CallFunctionAsync(name, callback, args);
        }
    }
}