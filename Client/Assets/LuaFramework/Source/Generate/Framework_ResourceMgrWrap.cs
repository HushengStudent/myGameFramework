﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Framework_ResourceMgrWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Framework.ResourceMgr), typeof(Framework.MonoSingleton<Framework.ResourceMgr>));
		L.RegFunction("LoadAssetAsync", LoadAssetAsync);
		L.RegFunction("LoadResourceProxy", LoadResourceProxy);
		L.RegFunction("LoadResourceAsync", LoadResourceAsync);
		L.RegFunction("DestroyUnityAsset", DestroyUnityAsset);
		L.RegFunction("DestroyInstantiateObject", DestroyInstantiateObject);
		L.RegFunction("GameGC", GameGC);
		L.RegFunction("UnloadUnusedAssets", UnloadUnusedAssets);
		L.RegFunction("AddRemoveProxy", AddRemoveProxy);
		L.RegFunction("CancleAllProxy", CancleAllProxy);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("MAX_LOAD_TIME", get_MAX_LOAD_TIME, null);
		L.RegVar("onResourceInitAction", get_onResourceInitAction, set_onResourceInitAction);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadAssetAsync(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.Register");
#endif
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				Framework.AssetBundleAssetProxy o = obj.LoadAssetAsync(arg0);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3)
			{
				Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 3);
				Framework.AssetBundleAssetProxy o = obj.LoadAssetAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 4)
			{
				Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				System.Action<float> arg1 = (System.Action<float>)ToLua.CheckDelegate<System.Action<float>>(L, 3);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 4);
				Framework.AssetBundleAssetProxy o = obj.LoadAssetAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Framework.ResourceMgr.LoadAssetAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadResourceProxy(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.LoadResourceProxy");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			Framework.ResourceAssetProxy o = obj.LoadResourceProxy(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadResourceAsync(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.Register");
#endif
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				Framework.ResourceAssetProxy o = obj.LoadResourceAsync(arg0);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3)
			{
				Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				System.Action<float> arg1 = (System.Action<float>)ToLua.CheckDelegate<System.Action<float>>(L, 3);
				Framework.ResourceAssetProxy o = obj.LoadResourceAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Framework.ResourceMgr.LoadResourceAsync");
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
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.DestroyUnityAsset");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
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
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.DestroyInstantiateObject");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
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
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.GameGC");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
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
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.UnloadUnusedAssets");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
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
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.AddRemoveProxy");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
			Framework.AbsAssetProxy arg0 = (Framework.AbsAssetProxy)ToLua.CheckObject<Framework.AbsAssetProxy>(L, 2);
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
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.CancleAllProxy");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
			System.Collections.Generic.IEnumerator<float> o = obj.CancleAllProxy();
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
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.op_Equality");
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
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.MAX_LOAD_TIME");
#endif
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)o;
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
	static int get_onResourceInitAction(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.onResourceInitAction");
#endif
		try
		{
			ToLua.Push(L, Framework.ResourceMgr.onResourceInitAction);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_onResourceInitAction(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.ResourceMgr.onResourceInitAction");
#endif
		try
		{
			System.Action arg0 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 2);
			Framework.ResourceMgr.onResourceInitAction = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

