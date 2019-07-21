using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Blackboard/Lists")]
    [Description("Remove an element from the target list")]
    public class RemoveElementFromList<T> : ActionTask
    {

        [RequiredField]
        [BlackboardOnly]
        public BBParameter<List<T>> targetList;
        public BBParameter<T> targetElement;

        protected override string info {
            get { return string.Format("Remove {0} From {1}", targetElement, targetList); }
        }

        protected override void OnExecute() {
            targetList.value.Remove(targetElement.value);
            EndAction(true);
        }
    }
}