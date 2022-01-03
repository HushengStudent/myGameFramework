/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/09 16:52:58
** desc:  导出AssetBundle更新包;
*********************************************************************************/

using Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class ExportABPackage
    {
        public static readonly string TempABPath =
            $"{FilePathHelper.AssetBundlePath}/../tempABPath";

        public static readonly string ZipABPath =
            $"{FilePathHelper.AssetBundlePath}/../AssetBundleZip";

        public static readonly string AssetBundleZipFileName = "AssetBundle.zip";

        public static readonly string ZipStreamingAssetsPath =
            $"{FilePathHelper.StreamingAssetsPath}/{AssetBundleZipFileName}";

        [MenuItem("myGameFramework/AssetBundle/Export AssetBundle Package", false, 41)]
        public static void ExportAssetBundlePackage()
        {
            var allFiles = Directory.GetFiles(FilePathHelper.AssetBundlePath, "*.*", SearchOption.AllDirectories);
            //剔除.manifest文件;
            var allABs = new List<string>();
            foreach (var tempPath in allFiles)
            {
                var path = tempPath.Replace("\\", "/");
                if (Path.GetExtension(path) == ".manifest")
                {
                    continue;
                }
                allABs.Add(path);
            }

            EditorUtility.ClearProgressBar();


            if (Directory.Exists(TempABPath))
            {
                Directory.Delete(TempABPath, true);
            }
            var targetPath = $"{TempABPath}/AssetBundle";
            Directory.CreateDirectory(targetPath);

            var index = 0;
            var count = allABs.Count;

            foreach (var ab in allABs)
            {
                index++;
                EditorUtility.DisplayProgressBar("复制AssetBundle", "复制AssetBundle文件", index / (float)count);
                var target = ab.Replace(FilePathHelper.AssetBundlePath, targetPath);
                File.Copy(ab, target);
            }
            EditorUtility.ClearProgressBar();

            var zipFile = $"{ZipABPath}/{AssetBundleZipFileName}";
            Framework.SharpZipLibHelper.Zip(new string[] { targetPath }, zipFile);

            if (Directory.Exists(TempABPath))
            {
                Directory.Delete(TempABPath, true);
            }
            if (File.Exists(ZipStreamingAssetsPath))
            {
                File.Delete(ZipStreamingAssetsPath);
            }
            File.Copy(zipFile, ZipStreamingAssetsPath);

            AssetDatabase.Refresh();
        }

        [MenuItem("myGameFramework/AssetBundle/Decompress AssetBundle Zip", false, 42)]
        public static void DecompressAssetBundleZip()
        {
            var zipFile = $"{ZipABPath}/{AssetBundleZipFileName}";
            if (!File.Exists(zipFile))
            {
                return;
            }
            EditorUtility.ClearProgressBar();

            Framework.SharpZipLibHelper.UnzipFile(zipFile, $"{Application.dataPath}/StreamingAssets/AssetBundle");

            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();
        }

        public static void CopyAssetBundle(string destPath)
        {
            var allFiles = Directory.GetFiles(FilePathHelper.AssetBundlePath, "*.*", SearchOption.AllDirectories);
            //剔除.manifest文件;
            var allABs = new List<string>();
            foreach (var tempPath in allFiles)
            {
                var path = tempPath.Replace("\\", "/");
                if (Path.GetExtension(path) == ".manifest")
                {
                    continue;
                }
                allABs.Add(path);
            }

            var targetPath = $"{destPath}/AssetBundle";
            if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }
            Directory.CreateDirectory(targetPath);

            var index = 0;
            var count = allABs.Count;

            foreach (var ab in allABs)
            {
                index++;
                EditorUtility.DisplayProgressBar("复制AssetBundle", "复制AssetBundle文件", index / (float)count);
                var target = ab.Replace(FilePathHelper.AssetBundlePath, targetPath);
                File.Copy(ab, target);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}