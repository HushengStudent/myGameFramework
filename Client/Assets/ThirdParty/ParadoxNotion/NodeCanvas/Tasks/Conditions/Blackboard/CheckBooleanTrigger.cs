using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Blackboard")]
    [Description("Check if a boolean variable is true and if so, it is immediately reset to false.")]
    public class CheckBooleanTrigger : ConditionTask
    {

        [BlackboardOnly]
        public BBParameter<bool> trigger;

        protected override string info {
            get { return string.Format("Trigger {0}", trigger); }
        }

        protected override bool OnCheck() {
            if ( trigger.value ) {
                trigger.value = false;
                return true;
            }
            return false;
        }
    }
}