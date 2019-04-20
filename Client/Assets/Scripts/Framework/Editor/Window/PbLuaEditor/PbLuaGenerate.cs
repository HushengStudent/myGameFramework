/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/04/14 18:22:48
** desc:  proto生成;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static class PbLuaGenerate
    {
        private static List<string> _paths = new List<string>();

        public static List<string> Paths
        {
            get
            {
                InitContext();
                return _paths;
            }
        }

        private static List<string> _files = new List<string>();

        public static List<string> Files
        {
            get
            {
                InitContext();
                return _files;
            }
        }

        private static string Protoc = FilePathHelper.ProtocPath + "protoc.exe";

        /// <summary>
        /// pblua目录;
        /// </summary>
        public static string PbluaPath
        {
            get
            {
                return Application.dataPath.ToLower() + "/LuaFramework/Lua/Network/Protol";
            }
        }

        /// <summary>
        /// tools目录;
        /// </summary>
        public static string ProtocGenPath
        {
            get
            {
                return "\"" + Application.dataPath.ToLower() + "/../../Tools/protoc-gen-lua-master/plugin/protoc-gen-lua.bat\"";
            }
        }

        /// <summary>
        /// protogen目录;
        /// </summary>
        public static string ProtogenPath
        {
            get
            {
                return Application.dataPath.ToLower() + "/../../Tools/protoc-gen-csharp/ProtoGen/bin/Release/protogen.exe";
            }
        }

        /// <summary>
        /// Csharp客户端输出目录;
        /// </summary>
        public static string CsharpClientPath
        {
            get
            {
                return Application.dataPath.ToLower() + "/Scripts/Common/Protocol/Proto";
            }
        }

        /// <summary>
        /// 生成协议给服务器;
        /// </summary>
        private static string CsharpServerPath
        {
            get
            {
                return Application.dataPath.ToLower() + "/Scripts/Common/Protocol";
            }
        }

        /// <summary>
        /// 初始化上下文;
        /// </summary>
        public static void InitContext()
        {
            _paths.Clear();
            _files.Clear();
            Recursive(PbluaPath);
        }

        [MenuItem("myGameFramework/PbTool/Generate Proto To Lua", false, 0)]
        public static void GenerateProtoToLua()
        {
            InitContext();
            int index = 0;
            foreach (string tempFile in _files)
            {
                index++;
                string fileName = Path.GetFileName(tempFile);
                string fileExtension = Path.GetExtension(tempFile);
                string workPath = Path.GetDirectoryName(tempFile);
                if (!fileExtension.Equals(".proto")) continue;
                EditorUtility.DisplayProgressBar("Generate", "generate proto to lua:" + fileName, (float)index / (float)_files.Count);
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = Protoc;
                processStartInfo.Arguments = " --lua_out=./ --plugin=protoc-gen-lua=" + ProtocGenPath + " " + fileName;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.UseShellExecute = true;
                processStartInfo.WorkingDirectory = workPath;
                processStartInfo.ErrorDialog = true;
                LogHelper.Print("generate proto to lua:" + fileName);
                Process process = Process.Start(processStartInfo);
                process.WaitForExit();
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem("myGameFramework/PbTool/Generate Proto To Csharp", false, 1)]
        public static void GenerateProtoToCsharp()
        {
            InitContext();
            int index = 0;
            foreach (string f in _files)
            {
                index++;
                string fileName = Path.GetFileName(f);
                string fileExtension = Path.GetExtension(f);
                string workPath = Path.GetDirectoryName(f);
                string targetFileName = Path.GetFileNameWithoutExtension(f);
                if (!fileExtension.Equals(".proto")) continue;
                //输出目录;
                string outPath = GetCsharpPath(f) + targetFileName;
                if (null == outPath) continue;
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                EditorUtility.DisplayProgressBar("Generate", "generate proto to c#:" + fileName, (float)index / (float)_files.Count);
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = ProtogenPath;
                processStartInfo.Arguments = "-i:" + fileName + " -o:" + outPath + "/" + targetFileName + ".cs -p:detectMissing";
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

        [MenuItem("myGameFramework/PbTool/Generate All", false, 2)]
        public static void GenerateAll()
        {
            GenerateProtoToLua();
            GenerateProtoToCsharp();
        }

        /// <summary>
        /// 遍历目录及其子目录;
        /// </summary>
        /// <param name="path"></param>
        private static void Recursive(string path)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta")) continue;
                _files.Add(filename.Replace('\\', '/'));
            }
            foreach (string dir in dirs)
            {
                _paths.Add(dir.Replace('\\', '/'));
                Recursive(dir);
            }
        }

        /// <summary>
        /// 得到协议C#输出目录;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetCsharpPath(string path)
        {
            if (!path.Contains(PbluaPath))
            {
                return null;
            }
            return Path.GetDirectoryName(path.Replace(PbluaPath, CsharpClientPath)) + "/";
        }
    }
}