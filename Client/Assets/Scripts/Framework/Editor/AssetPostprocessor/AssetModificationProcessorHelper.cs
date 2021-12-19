/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2020/05/23 20:00:08
** desc:  资源修改处理;
*********************************************************************************/

using UnityEditor;

namespace FrameworkEditor
{
    public class AssetModificationProcessorHelper : UnityEditor.AssetModificationProcessor
    {
        public static bool IsOpenForEdit(string assetPath, out string message)
        {
            message = null;
            return true;
        }

        public static void OnWillCreateAsset(string path)
        {
        }

        public static string[] OnWillSaveAssets(string[] paths)
        {
            if (paths != null)
            {
            }
            return paths;
        }

        public static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
        {
            return AssetMoveResult.DidNotMove;
        }

        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
        {
            return AssetDeleteResult.DidNotDelete;
        }
    }
}