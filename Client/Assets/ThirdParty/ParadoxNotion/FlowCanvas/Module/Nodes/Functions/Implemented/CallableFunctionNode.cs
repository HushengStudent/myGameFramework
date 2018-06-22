using System;
using System.Collections;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	///Like PureFunctionNode but require Flow execution.
	abstract public class CallableFunctionNodeBase : SimplexNode {}


	abstract public class CallableFunctionNode<TResult> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke();
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2, T3> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b, T3 c);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value, p3.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2, T3, T4> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value, p3.value, p4.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2, T3, T4, T5> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value, p3.value, p4.value, p5.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2, T3, T4, T5, T6> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2, T3, T4, T5, T6, T7> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2, T3, T4, T5, T6, T7, T8> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			var p8 = node.AddValueInput<T8>(parameters[7].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
			var p1 = node.AddValueInput<T1>(parameters[0].Name);
			var p2 = node.AddValueInput<T2>(parameters[1].Name);
			var p3 = node.AddValueInput<T3>(parameters[2].Name);
			var p4 = node.AddValueInput<T4>(parameters[3].Name);
			var p5 = node.AddValueInput<T5>(parameters[4].Name);
			var p6 = node.AddValueInput<T6>(parameters[5].Name);
			var p7 = node.AddValueInput<T7>(parameters[6].Name);
			var p8 = node.AddValueInput<T8>(parameters[7].Name);
			var p9 = node.AddValueInput<T9>(parameters[8].Name);
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value); o.Call(f); });
		}
	}

	abstract public class CallableFunctionNode<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : CallableFunctionNodeBase {
		private TResult result;
		abstract public TResult Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i, T10 j);
		sealed protected override void OnRegisterPorts(FlowNode node){
			var o = node.AddFlowOutput(" ");
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
			node.AddValueOutput<TResult>("Value", ()=> {return result;});
			node.AddFlowInput(" ", (f)=> { result = Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value, p10.value); o.Call(f); });
		}
	}
}