using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas.DialogueTrees;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Dialogue")]
    [Description("Starts the Dialogue Tree assigned on a Dialogue Tree Controller object with specified agent used for 'Instigator'.")]
    [Icon("Dialogue")]
    public class StartDialogueTree : ActionTask<IDialogueActor>
    {

        [RequiredField]
        public BBParameter<DialogueTreeController> dialogueTreeController;
        public bool waitActionFinish = true;

        public bool isPrefab;

        private DialogueTreeController instance;

        protected override string info {
            get { return string.Format("Start Dialogue {0}", dialogueTreeController); }
        }

        protected override void OnExecute() {

            instance = isPrefab ? GameObject.Instantiate(dialogueTreeController.value) : dialogueTreeController.value;

            if ( waitActionFinish ) {
                instance.StartDialogue(agent, (success) => { if ( isPrefab ) { Object.Destroy(instance.gameObject); } EndAction(success); });
            } else {
                instance.StartDialogue(agent, (success) => { if ( isPrefab ) Object.Destroy(instance.gameObject); });
                EndAction();
            }
        }
    }
}