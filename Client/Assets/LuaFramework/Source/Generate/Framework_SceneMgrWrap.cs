﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Framework_SceneMgrWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Framework.SceneMgr), typeof(Framework.Singleton<Framework.SceneMgr>));
		L.RegFunction("New", _CreateFramework_SceneMgr);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateFramework_SceneMgr(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				Framework.SceneMgr obj = new Framework.SceneMgr();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: Framework.SceneMgr.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

