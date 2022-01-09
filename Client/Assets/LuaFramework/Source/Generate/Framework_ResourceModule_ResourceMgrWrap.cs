﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Framework_ResourceModule_ResourceMgrWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Framework.ResourceModule.ResourceMgr), typeof(Framework.MonoSingleton<Framework.ResourceModule.ResourceMgr>));
		L.RegFunction("LoadSceneAsync", LoadSceneAsync);
		L.RegFunction("UnloadSceneAsync", UnloadSceneAsync);
		L.RegFunction("DestroyUnityAsset", DestroyUnityAsset);
		L.RegFunction("DestroyInstantiateObject", DestroyInstantiateObject);
		L.RegFunction("GameGC", GameGC);
		L.RegFunction("UnloadUnusedAssets", UnloadUnusedAssets);
		L.RegFunction("AddRemoveProxy", AddRemoveProxy);
		L.RegFunction("CancleAllProxy", CancleAllProxy);
		L.RegFunction("LoadAssetSync", LoadAssetSync);
		L.RegFunction("LoadAssetAsync", LoadAssetAsync);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("MAX_LOAD_TIME", get_MAX_LOAD_TIME, null);
		L.RegVar("LOAD_BUNDLE_PRECENT", get_LOAD_BUNDLE_PRECENT, null);
		L.RegVar("LOAD_ASSET_PRECENT", get_LOAD_ASSET_PRECENT, null);
		L.RegVar("LuaAssetBundle", get_LuaAssetBundle, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadSceneAsync(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.Register");
#endif
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				obj.LoadSceneAsync(arg0);
				return 0;
			}
			else if (count == 3)
			{
				Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				System.Action<float> arg1 = (System.Action<float>)ToLua.CheckDelegate<System.Action<float>>(L, 3);
				obj.LoadSceneAsync(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				System.Action<UnityEngine.SceneManagement.Scene> arg1 = (System.Action<UnityEngine.SceneManagement.Scene>)ToLua.CheckDelegate<System.Action<UnityEngine.SceneManagement.Scene>>(L, 3);
				System.Action<float> arg2 = (System.Action<float>)ToLua.CheckDelegate<System.Action<float>>(L, 4);
				obj.LoadSceneAsync(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Framework.ResourceModule.ResourceMgr.LoadSceneAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnloadSceneAsync(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.Register");
#endif
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				System.Action<UnityEngine.SceneManagement.Scene> arg1 = (System.Action<UnityEngine.SceneManagement.Scene>)ToLua.CheckDelegate<System.Action<UnityEngine.SceneManagement.Scene>>(L, 3);
				obj.UnloadSceneAsync(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				System.Action<UnityEngine.SceneManagement.Scene> arg1 = (System.Action<UnityEngine.SceneManagement.Scene>)ToLua.CheckDelegate<System.Action<UnityEngine.SceneManagement.Scene>>(L, 3);
				System.Action<float> arg2 = (System.Action<float>)ToLua.CheckDelegate<System.Action<float>>(L, 4);
				obj.UnloadSceneAsync(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Framework.ResourceModule.ResourceMgr.UnloadSceneAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DestroyUnityAsset(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.DestroyUnityAsset");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 2);
			obj.DestroyUnityAsset(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DestroyInstantiateObject(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.DestroyInstantiateObject");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 2);
			obj.DestroyInstantiateObject(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GameGC(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.GameGC");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
			obj.GameGC();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnloadUnusedAssets(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.UnloadUnusedAssets");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
			System.Action arg0 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 2);
			obj.UnloadUnusedAssets(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddRemoveProxy(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.AddRemoveProxy");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
			Framework.ResourceModule.AbsAssetProxy arg0 = (Framework.ResourceModule.AbsAssetProxy)ToLua.CheckObject<Framework.ResourceModule.AbsAssetProxy>(L, 2);
			obj.AddRemoveProxy(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CancleAllProxy(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.CancleAllProxy");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
			System.Collections.IEnumerator o = obj.CancleAllProxy();
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadAssetSync(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.LoadAssetSync");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			Framework.ResourceModule.AbsAssetProxy o = obj.LoadAssetSync(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadAssetAsync(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.LoadAssetAsync");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)ToLua.CheckObject<Framework.ResourceModule.ResourceMgr>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			Framework.ResourceModule.AbsAssetProxy o = obj.LoadAssetAsync(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.op_Equality");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_MAX_LOAD_TIME(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.MAX_LOAD_TIME");
#endif
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)o;
			float ret = obj.MAX_LOAD_TIME;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index MAX_LOAD_TIME on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LOAD_BUNDLE_PRECENT(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.LOAD_BUNDLE_PRECENT");
#endif
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)o;
			float ret = obj.LOAD_BUNDLE_PRECENT;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index LOAD_BUNDLE_PRECENT on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LOAD_ASSET_PRECENT(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.LOAD_ASSET_PRECENT");
#endif
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)o;
			float ret = obj.LOAD_ASSET_PRECENT;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index LOAD_ASSET_PRECENT on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LuaAssetBundle(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceModule.ResourceMgr.LuaAssetBundle");
#endif
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Framework.ResourceModule.ResourceMgr obj = (Framework.ResourceModule.ResourceMgr)o;
			UnityEngine.AssetBundle ret = obj.LuaAssetBundle;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index LuaAssetBundle on a nil value");
		}
	}
}
