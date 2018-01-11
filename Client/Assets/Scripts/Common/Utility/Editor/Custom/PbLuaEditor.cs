/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:22:03
** desc:  proto生成
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Framework;

namespace Common
{
	public static class PbLuaEditor
    {
        static List<string> paths = new List<string>();
        static List<string> files = new List<string>();

        [MenuItem("MPbTool/Build PbLua File")]
        public static void BuildProtobufFile()
        {
            string pbluaDir = Application.dataPath.ToLower() + "/LuaFramework/Lua/Network/pblua";//pblua目录;
            paths.Clear();
            files.Clear();
            Recursive(pbluaDir);
            string protoc = "c:/protobuf-3.0.0/src/protoc.exe";//protoc.exe安装在c盘;
            //tools目录;
            string protoc_gen_dir = "\""+Application.dataPath.ToLower()+ "/../../Tools/protoc-gen-lua-master/plugin/protoc-gen-lua.bat\"";
            foreach (string f in files)
            {
                string name = Path.GetFileName(f);
                string ext = Path.GetExtension(f);
                if (!ext.Equals(".proto")) continue;
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = protoc;
                info.Arguments = " --lua_out=./ --plugin=protoc-gen-lua=" + protoc_gen_dir + " " + name;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.UseShellExecute = true;
                info.WorkingDirectory = pbluaDir;
                info.ErrorDialog = true;
                LogUtility.Print(info.FileName + " " + info.Arguments);
                Process pro = Process.Start(info);
                pro.WaitForExit();
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 遍历目录及其子目录;
        /// </summary>
        static void Recursive(string path)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta")) continue;
                files.Add(filename.Replace('\\', '/'));
            }
            foreach (string dir in dirs)
            {
                paths.Add(dir.Replace('\\', '/'));
                Recursive(dir);
            }
        }
    }
}
