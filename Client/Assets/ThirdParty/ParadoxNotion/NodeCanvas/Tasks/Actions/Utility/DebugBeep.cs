using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Utility")]
	[Description("Plays a 'Beep' in editor only")]
	public class DebugBeep : ActionTask, ISerializationCallbackReceiver {

		protected override void OnExecute(){
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.Beep();
			#endif
			EndAction();
		}

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            Debug.Log("DeSer");
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            Debug.Log("Ser");
        }
    }
}