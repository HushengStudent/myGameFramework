using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Blackboard")]
	[Description("It's best to use the respective Condition for a type if existant since they support operations as well")]
	public class CheckVariable<T> : ConditionTask {

		[BlackboardOnly]
		public BBParameter<T> valueA;
		public BBParameter<T> valueB;

		protected override string info{
			get {return valueA + " == " + valueB;}
		}

		protected override bool OnCheck(){
			return Equals(valueA.value, valueB.value);
		}
	}
}