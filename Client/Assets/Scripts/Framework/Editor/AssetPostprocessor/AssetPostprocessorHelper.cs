/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/10/26 21:11:12
** desc:  资源导入处理;
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class AssetPostprocessorHelper : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
            }
            foreach (string str in deletedAssets)
            {
            }
            for (int i = 0; i < movedAssets.Length; i++)
            {
            }
        }

        void OnPostprocessTexture(Texture2D texture)
        {
            //EditorApplication.ExecuteMenuItem("Assets/Refresh");
        }
    }
}
