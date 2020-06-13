/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/11/28 22:25:20
** desc:  É¾³ý¿ÕÎÄ¼þ¼Ð;
*********************************************************************************/

using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace FrameworkEditor
{
    public class DeleteEmptyFolderHelper : MonoBehaviour
    {
        [MenuItem("myGameFramework/Helper/Delete Empty Folder", false, 1)]
        public static void DeleteEmptyFolder()
        {
            var paths = Directory.GetDirectories(Application.dataPath, ".", SearchOption.AllDirectories);
            for (var i = paths.Length - 1; i > 0; i--)
            {
                var str = paths[i];
                var files = Directory.GetFiles(str);
                if (files.Length < 1)
                {
                    try
                    {
                        Directory.Delete(str);
                        LogHelper.Print($"[DeleteEmptyFolderHelper]Delete {str} success.");
                        File.Delete($"{str}.meta");
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            AssetDatabase.Refresh();
        }
    }
}