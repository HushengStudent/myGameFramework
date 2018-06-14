using UnityEditor;

namespace Pathfinding.Legacy {
	[CustomEditor(typeof(LegacyRVOController))]
	[CanEditMultipleObjects]
	public class LegacyRVOControllerEditor : Pathfinding.RVO.RVOControllerEditor {
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			LegacyEditorHelper.UpgradeDialog(targets, typeof(Pathfinding.RVO.RVOController));
		}
	}
}
