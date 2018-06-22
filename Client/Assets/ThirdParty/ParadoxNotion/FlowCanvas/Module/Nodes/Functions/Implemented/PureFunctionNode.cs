using System;
using System.Collections;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	///Function Nodes return a value and can have up to 10 parameters. They don't need Flow execution.
	abstract public class PureFunctionNodeBase : SimplexNode {}



	abstract public class PureFunctionNode<TResult> : PureFunctionNodeBase {
		abstract public TResult Invoke();
		sealed protected override void OnRegisterPorts(FlowNode node){
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2, T3> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b, T3 c);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value, p3.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2, T3, T4> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value, p3.value, p4.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2, T3, T4, T5> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value, p3.value, p4.value, p5.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2, T3, T4, T5, T6> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2, T3, T4, T5, T6, T7> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2, T3, T4, T5, T6, T7, T8> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			var p8 = node.AddValueInput<T8>(parameters[7].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			var p8 = node.AddValueInput<T8>(parameters[7].Name);
			var p9 = node.AddValueInput<T9>(parameters[8].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value); });
		}
	}

	abstract public class PureFunctionNode<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : PureFunctionNodeBase {
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i, T10 j);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			var p8 = node.AddValueInput<T8>(parameters[7].Name);
			var p9 = node.AddValueInput<T9>(parameters[8].Name);
			var p10 = node.AddValueInput<T10>(parameters[9].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value, p10.value); });
		}
	}
}