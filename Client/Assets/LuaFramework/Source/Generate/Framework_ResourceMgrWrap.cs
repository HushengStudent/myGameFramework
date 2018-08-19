﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Framework_ResourceMgrWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Framework.ResourceMgr), typeof(Framework.Singleton<Framework.ResourceMgr>));
		L.RegFunction("UnloadUnusedAssets", UnloadUnusedAssets);
		L.RegFunction("GameGC", GameGC);
		L.RegFunction("UnloadObject", UnloadObject);
		L.RegFunction("New", _CreateFramework_ResourceMgr);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateFramework_ResourceMgr(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				Framework.ResourceMgr obj = new Framework.ResourceMgr();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: Framework.ResourceMgr.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnloadUnusedAssets(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
			obj.UnloadUnusedAssets();
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
	static int UnloadObject(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Framework.ResourceMgr obj = (Framework.ResourceMgr)ToLua.CheckObject<Framework.ResourceMgr>(L, 1);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 2);
			obj.UnloadObject(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

