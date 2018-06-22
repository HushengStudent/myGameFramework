using System.Collections.Generic;

namespace FlowCanvas
{

    ///Data struct that is propagated within the graph through the FlowPorts
    [ParadoxNotion.Design.SpoofAOT]
	public struct Flow {
		///Number of ticks this Flow has made
		public int ticks;
		///Temporary set of Flow parameters
		public Dictionary<string, object> parameters;
		///Break Action
		public FlowBreak Break;
		///Return Action
		public FlowReturn Return;
		///The return type expected if any
		public System.Type ReturnType;

		///Short for 'new Flow()'
		public static Flow New{ get {return new Flow();} }

		///Alternative flow call method exactly the same as 'port.Call(f)'
		public void Call(FlowOutput port){
			port.Call(this);
		}

		///Read a temporary flow parameter
		public T ReadParameter<T>(string name){
			object parameter = default(T);
			if (parameters != null){
				parameters.TryGetValue(name, out parameter);
			}
			return parameter is T? (T)parameter : default(T);
		}

		///Write a temporary flow parameter
		public void WriteParameter<T>(string name, T value){
			if (parameters == null){
				parameters = new Dictionary<string, object>();
			}
			parameters[name] = value;
		}
	}
}