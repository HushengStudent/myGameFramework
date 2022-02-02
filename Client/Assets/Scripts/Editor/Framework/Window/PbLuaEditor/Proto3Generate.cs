/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/02 22:42:59
** desc:  proto3 tool;
*********************************************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.EditorModule.Window
{
    public class Proto3Generate
    {
        private static readonly string protocPath = Application.dataPath + "/../../Tools/proto3/protoc.exe";
        private static readonly string protoFilePath = Application.dataPath + "/../../Tools/proto3/Test/proto/";
        private static readonly string outPath = Application.dataPath + "/../../Tools/proto3/Test/cs/";

        [MenuItem("myGameFramework/ProtoGen/Proto3/Generate proto3 to csharp", false, 1)]
        public static void GenerateProtoToCsharp()
        {
            var allPath = Directory.GetFiles(protoFilePath, "*.*", SearchOption.AllDirectories);
            var allAssetPath = new List<string>();
            foreach (var tempPath in allPath)
            {
                var path = tempPath.Replace("\\", "/");
                if (Path.GetExtension(path) == ".proto")
                {
                    allAssetPath.Add(path);
                };
            }
            var index = 0;
            foreach (var f in allAssetPath)
            {
                index++;
                var fileName = Path.GetFileName(f);
                var fileExtension = Path.GetExtension(f);
                var workPath = Path.GetDirectoryName(f);
                var targetFileName = Path.GetFileNameWithoutExtension(f);
                if (!fileExtension.Equals(".proto"))
                {
                    continue;
                }
                if (null == outPath)
                {
                    continue;
                }
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                EditorUtility.DisplayProgressBar("Generate", "generate proto to c#:" + fileName, (float)index / (float)allAssetPath.Count);
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = protocPath,
                    Arguments = "--proto_path=" + workPath + " --csharp_out=" + outPath + " " + targetFileName + ".proto",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    WorkingDirectory = workPath,
                    ErrorDialog = true
                };
                LogHelper.Print("generate proto to c#:" + fileName);
                Process process = Process.Start(processStartInfo);
                process.WaitForExit();
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}