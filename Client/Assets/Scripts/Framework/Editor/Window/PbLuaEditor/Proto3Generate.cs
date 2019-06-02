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

namespace Framework
{
    public class Proto3Generate
    {
        private static readonly string protocPath = Application.dataPath+ "/../../Tools/proto3/protoc.exe";
        private static readonly string protoFilePath = Application.dataPath + "/../../Tools/proto3/Test/proto/";
        private static readonly string outPath = Application.dataPath + "/../../Tools/proto3/Test/cs/";

        [MenuItem("myGameFramework/ProtoGen/Proto3/Generate proto3 to csharp", false, 1)]
        public static void GenerateProtoToCsharp()
        {
            string[] allPath = Directory.GetFiles(protoFilePath, "*.*", SearchOption.AllDirectories);
            List<string> allAssetPath = new List<string>();
            foreach (string tempPath in allPath)
            {
                string path = tempPath.Replace("\\", "/");
                if (Path.GetExtension(path) == ".proto")
                {
                    allAssetPath.Add(path);
                };
            }
            int index = 0;
            foreach (string f in allAssetPath)
            {
                index++;
                string fileName = Path.GetFileName(f);
                string fileExtension = Path.GetExtension(f);
                string workPath = Path.GetDirectoryName(f);
                string targetFileName = Path.GetFileNameWithoutExtension(f);
                if (!fileExtension.Equals(".proto")) continue;
                if (null == outPath) continue;
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                EditorUtility.DisplayProgressBar("Generate", "generate proto to c#:" + fileName, (float)index / (float)allAssetPath.Count);
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = protocPath;
                processStartInfo.Arguments = "--proto_path=" + workPath + " --csharp_out=" + outPath + " " + targetFileName + ".proto";
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.UseShellExecute = true;
                processStartInfo.WorkingDirectory = workPath;
                processStartInfo.ErrorDialog = true;
                LogHelper.Print("generate proto to c#:" + fileName);
                Process process = Process.Start(processStartInfo);
                process.WaitForExit();
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}