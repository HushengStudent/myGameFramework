/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/09 16:52:58
** desc:  导出AssetBundle更新包;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class ExportABPackage
    {
        private static string _tempABPath = FilePathHelper.AssetBundlePath + "/../tempABPath";
        private static string _zipABPath = FilePathHelper.AssetBundlePath + "/../AssetBundleZip";

        [MenuItem("myGameFramework/AssetBundleTools/Export AssetBundle Package", false, 41)]
        public static void ExportAssetBundlePackage()
        {
            if (Directory.Exists(_tempABPath))
            {
                Directory.Delete(_tempABPath, true);
            }
            string[] allFile = Directory.GetFiles(FilePathHelper.AssetBundlePath, "*.*", SearchOption.AllDirectories);
            //剔除.manifest文件;
            List<string> allAB = new List<string>();
            foreach (string tempPath in allFile)
            {
                string path = tempPath.Replace("\\", "/");
                if (Path.GetExtension(path) == ".manifest") continue;
                allAB.Add(path);
            }

            EditorUtility.ClearProgressBar();
            int index = 0;
            int count = allAB.Count;
            foreach (string ab in allAB)
            {
                index++;
                EditorUtility.DisplayProgressBar("复制AssetBundle", "复制AssetBundle文件", (float)index / (float)count);
                string target = ab.Replace(FilePathHelper.AssetBundlePath, _tempABPath + "/AssetBundle");
                string targetPath = Path.GetDirectoryName(target);
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                    File.Copy(ab, target);
                }
            }
            EditorUtility.ClearProgressBar();

            bool compressState = true;
            float compressProgress = 0f;
            ZipHelper.Compress(_tempABPath, _zipABPath, "AssetBundle", (progress) =>
            {
                compressProgress = progress;
                if (compressProgress >= 1)
                {
                    compressState = false;
                }
            });
            while (compressState)
            {
                EditorUtility.DisplayProgressBar("压缩AssetBundle", "压缩AssetBundle文件", compressProgress);
            }
            EditorUtility.ClearProgressBar();

            if (Directory.Exists(_tempABPath))
            {
                Directory.Delete(_tempABPath, true);
            }
        }

        [MenuItem("myGameFramework/AssetBundleTools/Decompress AssetBundle Zip", false, 42)]
        public static void DecompressAssetBundleZip()
        {
            if (!File.Exists(_zipABPath + "/AssetBundle.zip"))
            {
                return;
            }
            EditorUtility.ClearProgressBar();
            bool deCompressState = true;
            float deCompressProgress = 0f;
            ZipHelper.Decompress(_zipABPath + "/AssetBundle.zip", Application.dataPath + "/StreamingAssets/AssetBundle", (progress) =>
            {
                deCompressProgress = progress;
                if (deCompressProgress >= 1)
                {
                    deCompressState = false;
                }
            });
            while (deCompressState)
            {
                EditorUtility.DisplayProgressBar("解压AssetBundle", "解压AssetBundle文件", deCompressProgress);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}