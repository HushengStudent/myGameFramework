using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Blackboard")]
    [Description("Saves the blackboard variables in the provided key and to be loaded later on")]
    public class SaveBlackboard : ActionTask<Blackboard>
    {

        [RequiredField]
        public BBParameter<string> saveKey;

        protected override string info {
            get { return string.Format("Save Blackboard [{0}]", saveKey.ToString()); }
        }

        protected override void OnExecute() {
            agent.Save(saveKey.value);
            EndAction();
        }
    }
}