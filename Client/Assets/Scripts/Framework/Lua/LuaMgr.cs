/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:30:25
** desc:  Lua管理;
*********************************************************************************/

using Framework.ResourceModule;
using LuaInterface;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    //参考(https://github.com/jarjin/LuaFramework_UGUI)集成tolua;
    public class LuaMgr : MonoSingleton<LuaMgr>
    {
        private LuaState lua;
        private LuaLoaderHelper loader;
        private LuaLooper loop = null;

        public List<string> RequirePathList { get; private set; }

        protected override void OnInitialize()
        {
            InitLuaPath();
            InitLuaBundle();
            lua.Start();    //启动LUAVM;
            StartMain();
            StartLooper();
        }

        protected override void AwakeEx()
        {
            base.AwakeEx();
            loader = new LuaLoaderHelper();
            lua = new LuaState();
            OpenLibs();
            lua.LuaSetTop(0);
            LuaBinder.Bind(lua);
            DelegateFactory.Init();
            LuaCoroutine.Register(lua, this);
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            ToluaProfiler.Update();
        }

        /// 初始化加载第三方库;
        void OpenLibs()
        {
            lua.OpenLibs(LuaDLL.luaopen_pb);
            //lua.OpenLibs(LuaDLL.luaopen_sproto_core);
            //lua.OpenLibs(LuaDLL.luaopen_protobuf_c);
            lua.OpenLibs(LuaDLL.luaopen_lpeg);
            lua.OpenLibs(LuaDLL.luaopen_bit);
            lua.OpenLibs(LuaDLL.luaopen_socket_core);
            OpenCJson();
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

        /// 初始化Lua代码加载路径;
        void InitLuaPath()
        {
#if UNITY_EDITOR
            RequirePathList = new List<string>();
            RequirePathList.Add(LuaConst.luaDir);
            RequirePathList.Add(LuaConst.luaResDir);
            RequirePathList.Add(Application.dataPath + "/LuaFramework/Lua/NetWork");
            if (!loader.beZip)
            {
                foreach (var path in RequirePathList)
                {
                    lua.AddSearchPath(path);
                }
            }
#endif
        }

        /// 初始化LuaBundle;
        void InitLuaBundle()
        {
            if (loader.beZip)
            {
                var name = FilePathHelper.luaAssetBundleName;
                var ab = ResourceMgr.singleton.LuaAssetBundle;
                loader.AddSearchBundle(name, ab);
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
            var func = lua.GetFunction(funcName);
            if (func != null)
            {
                return func.Invoke<object[], object[]>(args);
            }
            return null;
        }

        public void CallLuaModuleMethod(string funcName, params object[] args)
        {
            var func = lua.GetFunction(funcName);
            if (func != null)
            {
                func.Call(args);
            }
        }

        public void CallLuaTableMethod(string module, string funcName, params object[] args)
        {
            var func = lua.GetFunction(module + "." + funcName);
            var table = lua.GetTable(module);
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

        public void Dostring(string str)
        {
            if (lua != null)
            {
                lua.DoString(str);
            }
        }
    }
}

