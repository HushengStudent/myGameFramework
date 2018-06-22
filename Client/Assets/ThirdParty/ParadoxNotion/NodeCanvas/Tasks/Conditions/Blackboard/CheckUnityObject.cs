using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Blackboard")]
	public class CheckUnityObject : ConditionTask{

		[BlackboardOnly]
		public BBParameter<UnityEngine.Object> valueA;
		public BBParameter<UnityEngine.Object> valueB;

		protected override string info{
			get {return valueA + " == " + valueB;}
		}

		protected override bool OnCheck(){
			return valueA.value == valueB.value;
		}
	}
}