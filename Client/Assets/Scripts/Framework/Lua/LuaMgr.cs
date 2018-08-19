/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:30:25
** desc:  Lua管理;
*********************************************************************************/

using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogUtil;

namespace Framework
{
    //参考(https://github.com/jarjin/LuaFramework_UGUI)集成tolua;
    public class LuaMgr : MonoSingleton<LuaMgr>, IManagr
    {
        private LuaState lua;
        private LuaLoaderUtility loader;
        private LuaLooper loop = null;

        /// <summary>
        /// 初始化;
        /// </summary>
        public void InitEx()
        {
            InitLuaPath();
            InitLuaBundle();
            this.lua.Start();    //启动LUAVM;
            this.StartMain();
            this.StartLooper();
        }

        protected override void AwakeEx()
        {
            base.AwakeEx();
            //初始化LuaMgr;
            loader = new LuaLoaderUtility();//TODO:Lua AssetBundle的使用;
            lua = new LuaState();
            this.OpenLibs();
            lua.LuaSetTop(0);
            LuaBinder.Bind(lua);
            DelegateFactory.Init();
            LuaCoroutine.Register(lua, this);
        }

        /// <summary>
        /// 初始化加载第三方库;
        /// </summary>
        void OpenLibs()
        {
            lua.OpenLibs(LuaDLL.luaopen_pb);
            //lua.OpenLibs(LuaDLL.luaopen_sproto_core);
            //lua.OpenLibs(LuaDLL.luaopen_protobuf_c);
            lua.OpenLibs(LuaDLL.luaopen_lpeg);
            lua.OpenLibs(LuaDLL.luaopen_bit);
            lua.OpenLibs(LuaDLL.luaopen_socket_core);
            this.OpenCJson();
        }

        //cjson比较特殊,只new了一个table,没有注册库,这里注册一下;
        protected void OpenCJson()
        {
            lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            lua.OpenLibs(LuaDLL.luaopen_cjson);
            lua.LuaSetField(-2, "cjson");
            lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
            lua.LuaSetField(-2, "cjson.safe");
        }

        /// <summary>
        /// 初始化Lua代码加载路径;
        /// </summary>
        void InitLuaPath()
        {
            lua.AddSearchPath(LuaConst.luaDir);
            lua.AddSearchPath(LuaConst.luaResDir);
            lua.AddSearchPath(Application.dataPath + "/LuaFramework/Lua/NetWork");
        }

        /// <summary>
        /// 初始化LuaBundle;
        /// </summary>
        void InitLuaBundle()
        {
            if (loader.beZip)
            {
                //loader.AddBundle("lua/lua.unity3d");
            }
        }

        void StartLooper()
        {
            loop = gameObject.AddComponent<LuaLooper>();
            loop.luaState = lua;
        }

        void StartMain()
        {
            lua.DoFile("Main.lua");
            LuaFunction main = lua.GetFunction("Main");
            main.Call();
            main.Dispose();
            main = null;
        }

        public void DoFile(string filename)
        {
            lua.DoFile(filename);
        }

        // Update is called once per frame;
        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = lua.GetFunction(funcName);
            if (func != null)
            {
                return func.LazyCall(args);
            }
            return null;
        }

        public void CallLuaModuleMethod(string funcName, params object[] args)
        {
            LuaFunction func = lua.GetFunction(funcName);
            if (func != null)
            {
                func.Call(args);
            }
        }

        public void CallLuaTableMethod(string module, string funcName, params object[] args)
        {
            LuaFunction func = lua.GetFunction(module + "." + funcName);
            LuaTable table = lua.GetTable(module);
            if (func != null && table != null)
            {
                func.Call(table, args);
            }
        }

        public void LuaGC()
        {
            lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
        }

        public void Close()
        {
            loop.Destroy();
            loop = null;

            lua.Dispose();
            lua = null;
            loader = null;
        }
    }
}
