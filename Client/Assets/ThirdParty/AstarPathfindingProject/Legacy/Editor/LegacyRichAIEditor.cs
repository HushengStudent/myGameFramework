using UnityEditor;

namespace Pathfinding.Legacy {
	[CustomEditor(typeof(LegacyRichAI))]
	[CanEditMultipleObjects]
	public class LegacyRichAIEditor : Editor {
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			LegacyEditorHelper.UpgradeDialog(targets, typeof(RichAI));
		}
	}
}
