using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Blackboard/Lists")]
    public class ListIsEmpty : ConditionTask
    {

        [RequiredField]
        [BlackboardOnly]
        public BBParameter<IList> targetList;

        protected override string info {
            get { return string.Format("{0} Is Empty", targetList); }
        }

        protected override bool OnCheck() {
            return targetList.value.Count == 0;
        }
    }
}