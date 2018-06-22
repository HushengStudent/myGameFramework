using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Blackboard")]
	public class CheckInt : ConditionTask{

		[BlackboardOnly]
		public BBParameter<int> valueA;
		public CompareMethod checkType = CompareMethod.EqualTo;
		public BBParameter<int> valueB;

		protected override string info{
			get {return valueA + OperationTools.GetCompareString(checkType) + valueB;}
		}

		protected override bool OnCheck(){
			return OperationTools.Compare((int)valueA.value, (int)valueB.value, checkType);
		}
	}
}