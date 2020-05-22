/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/07 17:33:17
** desc:  Lua工具集;
*********************************************************************************/

using LuaInterface;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Framework
{
    public class LuaUtility : Singleton<LuaUtility>
    {
        #region Math

        public static int Int(object o)
        {
            return Convert.ToInt32(o);
        }

        public static float Float(object o)
        {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static long Long(object o)
        {
            return Convert.ToInt64(o);
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        #endregion

        #region Unity

        /// <summary>
        /// 搜索子物体组件:GameObject版;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="subnode"></param>
        /// <returns></returns>
        public static T Get<T>(GameObject go, string subnode) where T : Component
        {
            if (go != null)
            {
                var sub = go.transform.Find(subnode);
                if (sub != null)
                {
                    return sub.GetComponent<T>();
                }
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件:Transform版;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="subnode"></param>
        /// <returns></returns>
        public static T Get<T>(Transform go, string subnode) where T : Component
        {
            if (go != null)
            {
                var sub = go.Find(subnode);
                if (sub != null)
                {
                    return sub.GetComponent<T>();
                }
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件:Component版;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="subnode"></param>
        /// <returns></returns>
        public static T Get<T>(Component go, string subnode) where T : Component
        {
            return go.transform.Find(subnode).GetComponent<T>();
        }

        /// <summary>
        /// 添加组件;
        /// </summary>
        public static T Add<T>(GameObject go) where T : Component
        {
            if (go != null)
            {
                var ts = go.GetComponents<T>();
                for (var i = 0; i < ts.Length; i++)
                {
                    if (ts[i] != null)
                    {
                        UnityObject.Destroy(ts[i]);
                    }
                }
                return go.gameObject.AddComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 添加组件;
        /// </summary>
        public static T Add<T>(Transform go) where T : Component
        {
            return Add<T>(go.gameObject);
        }

        /// <summary>
        /// 查找子对象;
        /// </summary>
        public static GameObject Child(GameObject go, string subnode)
        {
            return Child(go.transform, subnode);
        }

        /// <summary>
        /// 查找子对象;
        /// </summary>
        public static GameObject Child(Transform go, string subnode)
        {
            var tran = go.Find(subnode);
            if (tran == null)
            {
                return null;
            }
            return tran.gameObject;
        }

        /// <summary>
        /// 取平级对象;
        /// </summary>
        public static GameObject Peer(GameObject go, string subnode)
        {
            return Peer(go.transform, subnode);
        }

        /// <summary>
        /// 取平级对象;
        /// </summary>
        public static GameObject Peer(Transform go, string subnode)
        {
            var tran = go.parent.Find(subnode);
            if (tran == null)
            {
                return null;
            }
            return tran.gameObject;
        }

        /// <summary>
        /// 清除所有子节点;
        /// </summary>
        public static void ClearChild(Transform go)
        {
            if (go == null)
            {
                return;
            }
            for (var i = go.childCount - 1; i >= 0; i--)
            {
                UnityObject.Destroy(go.GetChild(i).gameObject);
            }
        }

        #endregion

        #region System

        public static string Uid(string uid)
        {
            var position = uid.LastIndexOf('_');
            return uid.Remove(0, position + 1);
        }

        public static long GetTime()
        {
            //TODO:使用服务器时间;
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 计算字符串的MD5值;
        /// </summary>
        public static string md5(string source)
        {
            var md5 = new MD5CryptoServiceProvider();
            var data = Encoding.UTF8.GetBytes(source);
            var md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            var destString = "";
            for (var i = 0; i < md5Data.Length; i++)
            {
                destString += Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值;
        /// </summary>
        public static string md5file(string file)
        {
            try
            {
                var fs = new FileStream(file, FileMode.Open);
                var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(fs);
                fs.Close();

                var sb = new StringBuilder();
                for (var i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 清理内存;
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
            if (LuaMgr.singleton != null)
            {
                LuaMgr.singleton.LuaGC();
            }
        }

        /// <summary>
        /// 取得行文本;
        /// </summary>
        public static string GetFileText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 网络可用;
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线;
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }

        /// <summary>
        /// 应用程序内容路径;
        /// </summary>
        public static string AppContentPath()
        {
            var path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                default:
                    //path = Application.dataPath + "/" + AppConst.AssetDir + "/";
                    break;
            }
            return path;
        }

        #endregion

        #region Call Lua

        /// <summary>
        /// 执行Lua方法;
        /// </summary>
        public static object[] CallMethod(string module, string func, params object[] args)
        {
            if (LuaMgr.singleton == null)
            {
                return null;
            }
            return LuaMgr.singleton.CallFunction(module + "." + func, args);
        }

        public static void CallLuaModuleMethod(string module, string func, params object[] args)
        {
            if (LuaMgr.singleton == null)
            {
                return;
            }
            LuaMgr.singleton.CallLuaModuleMethod(module + "." + func, args);
        }

        public static void CallLuaTableMethod(string module, string func, params object[] args)
        {
            if (LuaMgr.singleton == null)
            {
                return;
            }
            LuaMgr.singleton.CallLuaTableMethod(module, func, args);
        }

        /// <summary>
        /// pbc/pblua函数回调;
        /// </summary>
        /// <param name="data"></param>
        /// <param name="func"></param>
        public static void OnCallLuaFunc(LuaByteBuffer data, LuaFunction func)
        {
            if (func != null)
            {
                func.Call(data);
            }
            LogHelper.Print("OnCallLuaFunc length:>>" + data.buffer.Length);
        }

        #endregion

        #region Json

        /// <summary>
        /// cjson函数回调;
        /// </summary>
        /// <param name="data"></param>
        /// <param name="func"></param>
        public static void OnJsonCallFunc(string data, LuaFunction func)
        {
            Debug.LogWarning("OnJsonCallback data:>>" + data + " lenght:>>" + data.Length);
            if (func != null)
            {
                func.Call(data);
            }
        }

        #endregion

    }
}
