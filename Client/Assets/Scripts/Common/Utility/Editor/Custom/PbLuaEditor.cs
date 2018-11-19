/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:22:03
** desc:  proto生成;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Framework;
using LogUtil;

namespace Common
{
    public static class PbLuaEditor
    {
        private static List<string> _paths = new List<string>();
        private static List<string> _files = new List<string>();
        //protoc.exe安装在c盘;
        private static string _protoc = "c:/protobuf-3.0.0/src/protoc.exe";

        //pblua目录;
        private static string _pbluaDir = Application.dataPath.ToLower() + "/LuaFramework/Lua/Network/Protol";
        //tools目录;
        private static string _protocGenDir = "\"" + Application.dataPath.ToLower() + "/../../Tools/protoc-gen-lua-master/plugin/protoc-gen-lua.bat\"";

        //protogen目录;
        private static string _protogen = Application.dataPath.ToLower() + "/../../Tools/protoc-gen-csharp/ProtoGen/bin/Release/protogen.exe";
        //csharp客户端输出目录;
        private static string _csharpClientPath = Application.dataPath.ToLower() + "/Scripts/Common/Protocol/Proto";
        //TODO:生成协议给服务器;
        //private static string csharpServerPath = Application.dataPath.ToLower() + "/Scripts/Common/Protocol";

        [MenuItem("myGameFramework/PbTool/Build Pb File => Lua", false, 0)]
        public static void BuildPb2Lua()
        {
            _paths.Clear();
            _files.Clear();
            Recursive(_pbluaDir);
            int index = 0;
            foreach (string f in _files)
            {
                index++;
                string name = Path.GetFileName(f);
                string ext = Path.GetExtension(f);
                string workPath = Path.GetDirectoryName(f);
                if (!ext.Equals(".proto")) continue;
                EditorUtility.DisplayProgressBar("Build PbLua File", "gen proto to lua:" + name, (float)index / (float)_files.Count);
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = _protoc;
                info.Arguments = " --lua_out=./ --plugin=protoc-gen-lua=" + _protocGenDir + " " + name;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.UseShellExecute = true;
                info.WorkingDirectory = workPath;
                info.ErrorDialog = true;
                LogUtility.Print("gen proto to lua:" + name);
                Process pro = Process.Start(info);
                pro.WaitForExit();
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem("myGameFramework/PbTool/Build Pb File => C#", false, 1)]
        public static void BuildPb2Csharp()
        {
            _paths.Clear();
            _files.Clear();
            Recursive(_pbluaDir);
            int index = 0;
            foreach (string f in _files)
            {
                index++;
                string name = Path.GetFileName(f);
                string ext = Path.GetExtension(f);
                string workPath = Path.GetDirectoryName(f);
                if (!ext.Equals(".proto")) continue;
                //输出目录;
                string outPath = GetCsharpPath(f);
                if (null == outPath) continue;
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                string fileName = Path.GetFileNameWithoutExtension(f);
                EditorUtility.DisplayProgressBar("Build PbLua File", "gen proto to c#:" + name, (float)index / (float)_files.Count);
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = _protogen;
                info.Arguments = "-i:" + name + " -o:" + outPath + "/" + fileName + ".cs -p:detectMissing";
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.UseShellExecute = true;
                info.WorkingDirectory = workPath;
                info.ErrorDialog = true;
                LogUtility.Print("gen proto to c#:" + name);
                Process pro = Process.Start(info);
                pro.WaitForExit();
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem("myGameFramework/PbTool/Build All", false, 2)]
        public static void BuildAll()
        {
            BuildPb2Lua();
            BuildPb2Csharp();
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
            if (!path.Contains(_pbluaDir))
            {
                return null;
            }
            return Path.GetDirectoryName(path.Replace(_pbluaDir, _csharpClientPath)) + "/";
        }
    }
}
