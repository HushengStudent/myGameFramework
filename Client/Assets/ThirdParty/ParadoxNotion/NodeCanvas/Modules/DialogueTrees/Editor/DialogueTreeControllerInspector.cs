using NodeCanvas.Framework;
using NodeCanvas.DialogueTrees;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace NodeCanvas.Editor{

	[CustomEditor(typeof(DialogueTreeController))]
	public class DialogueTreeControllerInspector : GraphOwnerInspector {

		private DialogueTreeController controller{
			get {return target as DialogueTreeController; }
		}

		protected override void OnExtraOptions(){
			if (controller.graph != null){
				DialogueTreeInspector.ShowActorParameters( (DialogueTree)controller.graph );
			}
		}
	}
}