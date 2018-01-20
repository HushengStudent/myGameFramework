﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Framework_Singleton_Framework_SceneMgrWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Framework.Singleton<Framework.SceneMgr>), typeof(System.Object), "Singleton_Framework_SceneMgr");
		L.RegFunction("Init", Init);
		L.RegFunction("OnDestroy", OnDestroy);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Framework.Singleton<Framework.SceneMgr> obj = (Framework.Singleton<Framework.SceneMgr>)ToLua.CheckObject<Framework.Singleton<Framework.SceneMgr>>(L, 1);
			obj.Init();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnDestroy(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Framework.Singleton<Framework.SceneMgr> obj = (Framework.Singleton<Framework.SceneMgr>)ToLua.CheckObject<Framework.Singleton<Framework.SceneMgr>>(L, 1);
			obj.OnDestroy();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.PushObject(L, Framework.Singleton<Framework.SceneMgr>.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

