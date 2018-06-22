using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Name("Set Integer Random")]
	[Category("✫ Blackboard")]
	[Description("Set a blackboard integer variable at random between min and max value")]
	public class SetIntRandom : ActionTask {

		public BBParameter<int> minValue;
		public BBParameter<int> maxValue;

		[BlackboardOnly]
		public BBParameter<int> intVariable;

		protected override string info{
			get {return "Set " + intVariable + " Random(" + minValue + ", " + maxValue + ")";}
		}

		protected override void OnExecute(){
			intVariable.value = Random.Range(minValue.value, maxValue.value + 1);
			EndAction();
		}
	}
}