﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_InputWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("Input");
		L.RegFunction("GetAxis", GetAxis);
		L.RegFunction("GetAxisRaw", GetAxisRaw);
		L.RegFunction("GetButton", GetButton);
		L.RegFunction("GetButtonDown", GetButtonDown);
		L.RegFunction("GetButtonUp", GetButtonUp);
		L.RegFunction("GetMouseButton", GetMouseButton);
		L.RegFunction("GetMouseButtonDown", GetMouseButtonDown);
		L.RegFunction("GetMouseButtonUp", GetMouseButtonUp);
		L.RegFunction("ResetInputAxes", ResetInputAxes);
		L.RegFunction("GetJoystickNames", GetJoystickNames);
		L.RegFunction("GetAccelerationEvent", GetAccelerationEvent);
		L.RegFunction("GetKey", GetKey);
		L.RegFunction("GetKeyUp", GetKeyUp);
		L.RegFunction("GetKeyDown", GetKeyDown);
		L.RegFunction("GetTouch", GetTouch);
		L.RegVar("simulateMouseWithTouches", get_simulateMouseWithTouches, set_simulateMouseWithTouches);
		L.RegVar("anyKey", get_anyKey, null);
		L.RegVar("anyKeyDown", get_anyKeyDown, null);
		L.RegVar("inputString", get_inputString, null);
		L.RegVar("mousePosition", get_mousePosition, null);
		L.RegVar("mouseScrollDelta", get_mouseScrollDelta, null);
		L.RegVar("imeCompositionMode", get_imeCompositionMode, set_imeCompositionMode);
		L.RegVar("compositionString", get_compositionString, null);
		L.RegVar("imeIsSelected", get_imeIsSelected, null);
		L.RegVar("compositionCursorPos", get_compositionCursorPos, set_compositionCursorPos);
		L.RegVar("mousePresent", get_mousePresent, null);
		L.RegVar("touchCount", get_touchCount, null);
		L.RegVar("touchPressureSupported", get_touchPressureSupported, null);
		L.RegVar("stylusTouchSupported", get_stylusTouchSupported, null);
		L.RegVar("touchSupported", get_touchSupported, null);
		L.RegVar("multiTouchEnabled", get_multiTouchEnabled, set_multiTouchEnabled);
		L.RegVar("deviceOrientation", get_deviceOrientation, null);
		L.RegVar("acceleration", get_acceleration, null);
		L.RegVar("compensateSensors", get_compensateSensors, set_compensateSensors);
		L.RegVar("accelerationEventCount", get_accelerationEventCount, null);
		L.RegVar("backButtonLeavesApp", get_backButtonLeavesApp, set_backButtonLeavesApp);
		L.RegVar("location", get_location, null);
		L.RegVar("compass", get_compass, null);
		L.RegVar("gyro", get_gyro, null);
		L.RegVar("touches", get_touches, null);
		L.RegVar("accelerationEvents", get_accelerationEvents, null);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAxis(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetAxis");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			float o = UnityEngine.Input.GetAxis(arg0);
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAxisRaw(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetAxisRaw");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			float o = UnityEngine.Input.GetAxisRaw(arg0);
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetButton(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetButton");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			bool o = UnityEngine.Input.GetButton(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetButtonDown(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetButtonDown");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			bool o = UnityEngine.Input.GetButtonDown(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetButtonUp(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetButtonUp");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			bool o = UnityEngine.Input.GetButtonUp(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMouseButton(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetMouseButton");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			bool o = UnityEngine.Input.GetMouseButton(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMouseButtonDown(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetMouseButtonDown");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			bool o = UnityEngine.Input.GetMouseButtonDown(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMouseButtonUp(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetMouseButtonUp");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			bool o = UnityEngine.Input.GetMouseButtonUp(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResetInputAxes(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.ResetInputAxes");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.Input.ResetInputAxes();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetJoystickNames(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetJoystickNames");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string[] o = UnityEngine.Input.GetJoystickNames();
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAccelerationEvent(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetAccelerationEvent");
#endif
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			UnityEngine.AccelerationEvent o = UnityEngine.Input.GetAccelerationEvent(arg0);
			ToLua.PushValue(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetKey(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.Register");
#endif
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1 && TypeChecker.CheckTypes<UnityEngine.KeyCode>(L, 1))
			{
				UnityEngine.KeyCode arg0 = (UnityEngine.KeyCode)ToLua.ToObject(L, 1);
				bool o = UnityEngine.Input.GetKey(arg0);
				LuaDLL.lua_pushboolean(L, o);
				return 1;
			}
			else if (count == 1 && TypeChecker.CheckTypes<string>(L, 1))
			{
				string arg0 = ToLua.ToString(L, 1);
				bool o = UnityEngine.Input.GetKey(arg0);
				LuaDLL.lua_pushboolean(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: UnityEngine.Input.GetKey");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetKeyUp(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.Register");
#endif
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1 && TypeChecker.CheckTypes<UnityEngine.KeyCode>(L, 1))
			{
				UnityEngine.KeyCode arg0 = (UnityEngine.KeyCode)ToLua.ToObject(L, 1);
				bool o = UnityEngine.Input.GetKeyUp(arg0);
				LuaDLL.lua_pushboolean(L, o);
				return 1;
			}
			else if (count == 1 && TypeChecker.CheckTypes<string>(L, 1))
			{
				string arg0 = ToLua.ToString(L, 1);
				bool o = UnityEngine.Input.GetKeyUp(arg0);
				LuaDLL.lua_pushboolean(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: UnityEngine.Input.GetKeyUp");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetKeyDown(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.Register");
#endif
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1 && TypeChecker.CheckTypes<UnityEngine.KeyCode>(L, 1))
			{
				UnityEngine.KeyCode arg0 = (UnityEngine.KeyCode)ToLua.ToObject(L, 1);
				bool o = UnityEngine.Input.GetKeyDown(arg0);
				LuaDLL.lua_pushboolean(L, o);
				return 1;
			}
			else if (count == 1 && TypeChecker.CheckTypes<string>(L, 1))
			{
				string arg0 = ToLua.ToString(L, 1);
				bool o = UnityEngine.Input.GetKeyDown(arg0);
				LuaDLL.lua_pushboolean(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: UnityEngine.Input.GetKeyDown");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTouch(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.GetTouch");
#endif
        try
        {
		    int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
            int arg1 = LuaDLL.luaL_optinteger(L, 2, TouchBits.ALL);        
		    UnityEngine.Touch o = UnityEngine.Input.GetTouch(arg0);
            ToLua.Push(L, o, arg1);
            return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);			
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_simulateMouseWithTouches(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.simulateMouseWithTouches");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.simulateMouseWithTouches);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_anyKey(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.anyKey");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.anyKey);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_anyKeyDown(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.anyKeyDown");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.anyKeyDown);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_inputString(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.inputString");
#endif
		try
		{
			LuaDLL.lua_pushstring(L, UnityEngine.Input.inputString);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mousePosition(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.mousePosition");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.Input.mousePosition);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mouseScrollDelta(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.mouseScrollDelta");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.Input.mouseScrollDelta);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_imeCompositionMode(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.imeCompositionMode");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.Input.imeCompositionMode);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_compositionString(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.compositionString");
#endif
		try
		{
			LuaDLL.lua_pushstring(L, UnityEngine.Input.compositionString);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_imeIsSelected(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.imeIsSelected");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.imeIsSelected);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_compositionCursorPos(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.compositionCursorPos");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.Input.compositionCursorPos);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mousePresent(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.mousePresent");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.mousePresent);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_touchCount(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.touchCount");
#endif
		try
		{
			LuaDLL.lua_pushinteger(L, UnityEngine.Input.touchCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_touchPressureSupported(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.touchPressureSupported");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.touchPressureSupported);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_stylusTouchSupported(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.stylusTouchSupported");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.stylusTouchSupported);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_touchSupported(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.touchSupported");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.touchSupported);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_multiTouchEnabled(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.multiTouchEnabled");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.multiTouchEnabled);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_deviceOrientation(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.deviceOrientation");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.Input.deviceOrientation);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_acceleration(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.acceleration");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.Input.acceleration);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_compensateSensors(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.compensateSensors");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.compensateSensors);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_accelerationEventCount(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.accelerationEventCount");
#endif
		try
		{
			LuaDLL.lua_pushinteger(L, UnityEngine.Input.accelerationEventCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_backButtonLeavesApp(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.backButtonLeavesApp");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Input.backButtonLeavesApp);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_location(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.location");
#endif
		try
		{
			ToLua.PushObject(L, UnityEngine.Input.location);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_compass(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.compass");
#endif
		try
		{
			ToLua.PushObject(L, UnityEngine.Input.compass);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_gyro(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.gyro");
#endif
		try
		{
			ToLua.PushObject(L, UnityEngine.Input.gyro);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_touches(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.touches");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.Input.touches);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_accelerationEvents(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.accelerationEvents");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.Input.accelerationEvents);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_simulateMouseWithTouches(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.simulateMouseWithTouches");
#endif
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			UnityEngine.Input.simulateMouseWithTouches = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_imeCompositionMode(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.imeCompositionMode");
#endif
		try
		{
			UnityEngine.IMECompositionMode arg0 = (UnityEngine.IMECompositionMode)ToLua.CheckObject(L, 2, typeof(UnityEngine.IMECompositionMode));
			UnityEngine.Input.imeCompositionMode = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_compositionCursorPos(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.compositionCursorPos");
#endif
		try
		{
			UnityEngine.Vector2 arg0 = ToLua.ToVector2(L, 2);
			UnityEngine.Input.compositionCursorPos = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_multiTouchEnabled(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.multiTouchEnabled");
#endif
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			UnityEngine.Input.multiTouchEnabled = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_compensateSensors(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.compensateSensors");
#endif
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			UnityEngine.Input.compensateSensors = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_backButtonLeavesApp(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.Input.backButtonLeavesApp");
#endif
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			UnityEngine.Input.backButtonLeavesApp = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

