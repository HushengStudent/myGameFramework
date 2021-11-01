/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2021/11/01 20:45:22
** desc:  SharpZipLib Test;
*********************************************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class SharpZipLibHelper
    {
        [MenuItem("myGameFramework/Helper/SharpZipLibHelper/Zip", false, 0)]
        public static void Zip()
        {
            var dir = EditorUtility.OpenFolderPanel("select", Application.dataPath, "");
            LogHelper.PrintError($"Zip:{dir}");
            LogHelper.PrintError($"Output:{dir}.zip");
            Framework.SharpZipLibHelper.Zip(new string[] { dir }, $"{dir}.zip");
        }

        [MenuItem("myGameFramework/Helper/SharpZipLibHelper/UnzipFile", false, 0)]
        public static void UnzipFile()
        {
            var zip = EditorUtility.OpenFilePanel("select", Application.dataPath, "zip");
            var path = Path.GetDirectoryName(zip);
            LogHelper.PrintError($"UnzipFile:{zip}");
            LogHelper.PrintError($"Output:{path}");
            Framework.SharpZipLibHelper.UnzipFile(zip, path);
        }
    }
}