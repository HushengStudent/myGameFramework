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

        [MenuItem("myGameFramework/AssetBundleTools/Export AssetBundle Package", false, 41)]
        public static void ExportAssetBundlePackage()
        {
            if (Directory.Exists(TempABPath))
            {
                Directory.Delete(TempABPath, true);
            }

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

            var index = 0;
            var count = allABs.Count;

            foreach (var ab in allABs)
            {
                index++;
                EditorUtility.DisplayProgressBar("复制AssetBundle", "复制AssetBundle文件", index / (float)count);
                var target = ab.Replace(FilePathHelper.AssetBundlePath, $"{TempABPath}/AssetBundle");
                var targetPath = Path.GetDirectoryName(target);
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                    File.Copy(ab, target);
                }
            }
            EditorUtility.ClearProgressBar();

            var compressState = true;
            var compressProgress = 0f;
            var zipFile = ZipHelper.Compress(TempABPath, ZipABPath, "AssetBundle", (progress) =>
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

            if (Directory.Exists(TempABPath))
            {
                Directory.Delete(TempABPath, true);
            }
            if (File.Exists(ZipStreamingAssetsPath))
            {
                File.Delete(ZipStreamingAssetsPath);
            }
            File.Copy(zipFile, ZipStreamingAssetsPath);
        }

        [MenuItem("myGameFramework/AssetBundleTools/Decompress AssetBundle Zip", false, 42)]
        public static void DecompressAssetBundleZip()
        {
            if (!File.Exists(ZipABPath + "/" + AssetBundleZipFileName))
            {
                return;
            }
            EditorUtility.ClearProgressBar();
            bool deCompressState = true;
            float deCompressProgress = 0f;
            ZipHelper.Decompress(ZipABPath + "/" + AssetBundleZipFileName, Application.dataPath + "/StreamingAssets/AssetBundle", (progress) =>
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