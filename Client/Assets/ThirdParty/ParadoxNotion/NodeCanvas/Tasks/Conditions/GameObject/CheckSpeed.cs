using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

	[Category("GameObject")]
	[Description("Checks the current speed of the agent against a value based on it's Rigidbody velocity")]
	public class CheckSpeed : ConditionTask<Rigidbody>{

		public CompareMethod checkType = CompareMethod.EqualTo;
		public BBParameter<float> value;

		[SliderField(0,0.1f)]
		public float differenceThreshold = 0.05f;

		protected override string info{
			get	{return "Speed" + OperationTools.GetCompareString(checkType) + value;}
		}

		protected override bool OnCheck(){
			var speed = agent.velocity.magnitude;
			return OperationTools.Compare((float)speed, (float)value.value, checkType, differenceThreshold);
		}
	}
}