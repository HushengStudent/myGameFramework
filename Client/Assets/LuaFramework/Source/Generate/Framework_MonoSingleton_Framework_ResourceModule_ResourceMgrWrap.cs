﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Framework_MonoSingleton_Framework_ResourceModule_ResourceMgrWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Framework.MonoSingleton<Framework.ResourceModule.ResourceMgr>), typeof(Framework.MonoSingletonBase), "MonoSingleton_Framework_ResourceModule_ResourceMgr");
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("singleton", get_singleton, null);
		L.RegVar("ApplicationIsPlaying", get_ApplicationIsPlaying, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.MonoSingleton<Framework.ResourceModule.ResourceMgr>.op_Equality");
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
	static int get_singleton(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.MonoSingleton<Framework.ResourceModule.ResourceMgr>.singleton");
#endif
		try
		{
			ToLua.Push(L, Framework.MonoSingleton<Framework.ResourceModule.ResourceMgr>.singleton);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ApplicationIsPlaying(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("Framework.MonoSingleton<Framework.ResourceModule.ResourceMgr>.ApplicationIsPlaying");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, Framework.MonoSingleton<Framework.ResourceModule.ResourceMgr>.ApplicationIsPlaying);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
