using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using UnityEngine;

namespace FlowCanvas.Nodes{


	[Name("Identity", 10)]
	[Category("Utility")]
	[Description("Use this for organization. It returns exactly what is provided in the input.")]
	[ExposeAsDefinition]
	public class Identity<T> : PureFunctionNode<T, T>{
		public override string name{get{return null;}}
		public override T Invoke(T value){
			return value;
		}
	}

	[Name("Cache", 9)]
	[Category("Utility")]
	[Description("Caches the value only when the node is called.")]
	[ExposeAsDefinition]
	public class Cache<T> : CallableFunctionNode<T, T>{
		public override T Invoke(T value){
			return value;
		}
	}

	[Name("Switch", 8)]
	[Category("Utility")]
	[Description("Returns either one of the two inputs, based on the boolean condition")]
	[ExposeAsDefinition]
	public class SwitchValue<T> : PureFunctionNode<T, bool, T, T>{
		public override T Invoke(bool condition, T isTrue, T isFalse){
			return condition? isTrue : isFalse;
		}
	}

	[Obsolete]
	[Category("Utility")]
	[Description("Return a value from the list by index")]
	[ExposeAsDefinition]
	public class PickValue<T> : PureFunctionNode<T, int, IList<T>>{
		public override T Invoke(int index, IList<T> values){
			try {return values[index];}
			catch {return default(T);}
		}
	}

	[Category("Utility")]
	[Description("Remaps from input min/max to output min/max, by current value provided between input min/max")]
	[Name("Remap To Float")]
	public class RemapFloat : PureFunctionNode<float, float, float, float, float, float>{
		public override float Invoke(float current, float iMin, float iMax = 1f, float oMin = 0, float oMax = 100){
			return Mathf.Lerp(oMin, oMax, Mathf.InverseLerp(iMin, iMax, current));
		}
	}

	[Category("Utility")]
	[Description("Remaps from input min/max to output min/max, by current value provided between input min/max")]
	[Name("Remap To Vector3")]
	public class RemapVector3 : PureFunctionNode<Vector3, float, float, float, Vector3, Vector3>{
		public override Vector3 Invoke(float current, float iMin, float iMax, Vector3 oMin, Vector3 oMax){
			return Vector3.Lerp(oMin, oMax, Mathf.InverseLerp(iMin, iMax, current));
		}
	}

	[Category("Utility")]
	[Description("Log input value on the console")]
	public class LogValue : CallableActionNode<object>{
		public override void Invoke(object obj){
			Debug.Log(obj);
		}
	}

	[Category("Utility")]
	[Description("Log text in the console")]
	public class LogText : CallableActionNode<string>{
		public override void Invoke(string text){
			Debug.Log(text);
		}
	}

	[Category("Utility")]
	[Description("Send a Local Event to specified graph")]
	public class SendEvent : CallableActionNode<GraphOwner, string>{
		public override void Invoke(GraphOwner target, string eventName){
			target.SendEvent(new EventData(eventName));
		}
	}

	[Category("Utility")]
	[Description("Send a Local Event with 1 argument to specified graph")]
	[ExposeAsDefinition]
	public class SendEvent<T> : CallableActionNode<GraphOwner, string, T>{
		public override void Invoke(GraphOwner target, string eventName, T eventValue){
			target.SendEvent(new EventData<T>(eventName, eventValue));
		}
	}

	[Category("Utility")]
	[Description("Send a Global Event to all graphs")]
	public class SendGlobalEvent : CallableActionNode<string>{
		public override void Invoke(string eventName){
			Graph.SendGlobalEvent(new EventData(eventName));
		}
	}

	[Category("Utility")]
	[Description("Send a Global Event with 1 argument to all graphs")]
	[ExposeAsDefinition]
	public class SendGlobalEvent<T> : CallableActionNode<string, T>{
		public override void Invoke(string eventName, T eventValue){
			Graph.SendGlobalEvent(new EventData<T>(eventName, eventValue));
		}
	}
}