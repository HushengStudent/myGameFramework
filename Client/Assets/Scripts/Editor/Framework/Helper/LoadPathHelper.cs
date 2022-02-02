/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/01 17:45:46
** desc:  加载路径拷贝;
*********************************************************************************/

using UnityEditor;

namespace Framework.EditorModule.Helper
{
    public class LoadPathHelper
    {
        private static readonly string _bundlesPath = "Assets/Bundles/";

        [MenuItem("Assets/myGameFramework/Helper/Copy Load Path", false, 0)]
        public static void CopyLoadPath()
        {
            var targets = Selection.objects;
            if (targets.Length > 1 || targets.Length < 1)
            {
                return;
            }
            var path = AssetDatabase.GetAssetPath(targets[0].GetInstanceID());
            path = path.Replace(_bundlesPath, "");
            TextEditorHelper.Copy(path);
            EditorUtility.DisplayDialog("tips", "copy success.", "ok");
        }
    }
}