﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_RenderSettingsWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("RenderSettings");
		L.RegFunction("__eq", op_Equality);
		L.RegVar("fog", get_fog, set_fog);
		L.RegVar("fogStartDistance", get_fogStartDistance, set_fogStartDistance);
		L.RegVar("fogEndDistance", get_fogEndDistance, set_fogEndDistance);
		L.RegVar("fogMode", get_fogMode, set_fogMode);
		L.RegVar("fogColor", get_fogColor, set_fogColor);
		L.RegVar("fogDensity", get_fogDensity, set_fogDensity);
		L.RegVar("ambientMode", get_ambientMode, set_ambientMode);
		L.RegVar("ambientSkyColor", get_ambientSkyColor, set_ambientSkyColor);
		L.RegVar("ambientEquatorColor", get_ambientEquatorColor, set_ambientEquatorColor);
		L.RegVar("ambientGroundColor", get_ambientGroundColor, set_ambientGroundColor);
		L.RegVar("ambientIntensity", get_ambientIntensity, set_ambientIntensity);
		L.RegVar("ambientLight", get_ambientLight, set_ambientLight);
		L.RegVar("subtractiveShadowColor", get_subtractiveShadowColor, set_subtractiveShadowColor);
		L.RegVar("skybox", get_skybox, set_skybox);
		L.RegVar("sun", get_sun, set_sun);
		L.RegVar("ambientProbe", get_ambientProbe, set_ambientProbe);
		L.RegVar("customReflection", get_customReflection, set_customReflection);
		L.RegVar("reflectionIntensity", get_reflectionIntensity, set_reflectionIntensity);
		L.RegVar("reflectionBounces", get_reflectionBounces, set_reflectionBounces);
		L.RegVar("defaultReflectionMode", get_defaultReflectionMode, set_defaultReflectionMode);
		L.RegVar("defaultReflectionResolution", get_defaultReflectionResolution, set_defaultReflectionResolution);
		L.RegVar("haloStrength", get_haloStrength, set_haloStrength);
		L.RegVar("flareStrength", get_flareStrength, set_flareStrength);
		L.RegVar("flareFadeSpeed", get_flareFadeSpeed, set_flareFadeSpeed);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.op_Equality");
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
	static int get_fog(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fog");
#endif
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.RenderSettings.fog);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fogStartDistance(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogStartDistance");
#endif
		try
		{
			LuaDLL.lua_pushnumber(L, UnityEngine.RenderSettings.fogStartDistance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fogEndDistance(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogEndDistance");
#endif
		try
		{
			LuaDLL.lua_pushnumber(L, UnityEngine.RenderSettings.fogEndDistance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fogMode(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogMode");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.fogMode);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fogColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogColor");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.fogColor);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fogDensity(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogDensity");
#endif
		try
		{
			LuaDLL.lua_pushnumber(L, UnityEngine.RenderSettings.fogDensity);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ambientMode(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientMode");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.ambientMode);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ambientSkyColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientSkyColor");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.ambientSkyColor);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ambientEquatorColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientEquatorColor");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.ambientEquatorColor);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ambientGroundColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientGroundColor");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.ambientGroundColor);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ambientIntensity(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientIntensity");
#endif
		try
		{
			LuaDLL.lua_pushnumber(L, UnityEngine.RenderSettings.ambientIntensity);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ambientLight(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientLight");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.ambientLight);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_subtractiveShadowColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.subtractiveShadowColor");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.subtractiveShadowColor);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_skybox(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.skybox");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.skybox);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_sun(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.sun");
#endif
		try
		{
			ToLua.PushSealed(L, UnityEngine.RenderSettings.sun);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ambientProbe(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientProbe");
#endif
		try
		{
			ToLua.PushValue(L, UnityEngine.RenderSettings.ambientProbe);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_customReflection(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.customReflection");
#endif
		try
		{
			ToLua.PushSealed(L, UnityEngine.RenderSettings.customReflection);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_reflectionIntensity(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.reflectionIntensity");
#endif
		try
		{
			LuaDLL.lua_pushnumber(L, UnityEngine.RenderSettings.reflectionIntensity);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_reflectionBounces(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.reflectionBounces");
#endif
		try
		{
			LuaDLL.lua_pushinteger(L, UnityEngine.RenderSettings.reflectionBounces);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_defaultReflectionMode(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.defaultReflectionMode");
#endif
		try
		{
			ToLua.Push(L, UnityEngine.RenderSettings.defaultReflectionMode);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_defaultReflectionResolution(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.defaultReflectionResolution");
#endif
		try
		{
			LuaDLL.lua_pushinteger(L, UnityEngine.RenderSettings.defaultReflectionResolution);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_haloStrength(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.haloStrength");
#endif
		try
		{
			LuaDLL.lua_pushnumber(L, UnityEngine.RenderSettings.haloStrength);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_flareStrength(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.flareStrength");
#endif
		try
		{
			LuaDLL.lua_pushnumber(L, UnityEngine.RenderSettings.flareStrength);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_flareFadeSpeed(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.flareFadeSpeed");
#endif
		try
		{
			LuaDLL.lua_pushnumber(L, UnityEngine.RenderSettings.flareFadeSpeed);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fog(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fog");
#endif
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			UnityEngine.RenderSettings.fog = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fogStartDistance(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogStartDistance");
#endif
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.fogStartDistance = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fogEndDistance(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogEndDistance");
#endif
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.fogEndDistance = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fogMode(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogMode");
#endif
		try
		{
			UnityEngine.FogMode arg0 = (UnityEngine.FogMode)ToLua.CheckObject(L, 2, typeof(UnityEngine.FogMode));
			UnityEngine.RenderSettings.fogMode = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fogColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogColor");
#endif
		try
		{
			UnityEngine.Color arg0 = ToLua.ToColor(L, 2);
			UnityEngine.RenderSettings.fogColor = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fogDensity(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.fogDensity");
#endif
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.fogDensity = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ambientMode(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientMode");
#endif
		try
		{
			UnityEngine.Rendering.AmbientMode arg0 = (UnityEngine.Rendering.AmbientMode)ToLua.CheckObject(L, 2, typeof(UnityEngine.Rendering.AmbientMode));
			UnityEngine.RenderSettings.ambientMode = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ambientSkyColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientSkyColor");
#endif
		try
		{
			UnityEngine.Color arg0 = ToLua.ToColor(L, 2);
			UnityEngine.RenderSettings.ambientSkyColor = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ambientEquatorColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientEquatorColor");
#endif
		try
		{
			UnityEngine.Color arg0 = ToLua.ToColor(L, 2);
			UnityEngine.RenderSettings.ambientEquatorColor = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ambientGroundColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientGroundColor");
#endif
		try
		{
			UnityEngine.Color arg0 = ToLua.ToColor(L, 2);
			UnityEngine.RenderSettings.ambientGroundColor = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ambientIntensity(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientIntensity");
#endif
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.ambientIntensity = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ambientLight(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientLight");
#endif
		try
		{
			UnityEngine.Color arg0 = ToLua.ToColor(L, 2);
			UnityEngine.RenderSettings.ambientLight = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_subtractiveShadowColor(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.subtractiveShadowColor");
#endif
		try
		{
			UnityEngine.Color arg0 = ToLua.ToColor(L, 2);
			UnityEngine.RenderSettings.subtractiveShadowColor = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_skybox(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.skybox");
#endif
		try
		{
			UnityEngine.Material arg0 = (UnityEngine.Material)ToLua.CheckObject<UnityEngine.Material>(L, 2);
			UnityEngine.RenderSettings.skybox = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_sun(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.sun");
#endif
		try
		{
			UnityEngine.Light arg0 = (UnityEngine.Light)ToLua.CheckObject(L, 2, typeof(UnityEngine.Light));
			UnityEngine.RenderSettings.sun = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ambientProbe(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.ambientProbe");
#endif
		try
		{
			UnityEngine.Rendering.SphericalHarmonicsL2 arg0 = StackTraits<UnityEngine.Rendering.SphericalHarmonicsL2>.Check(L, 2);
			UnityEngine.RenderSettings.ambientProbe = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_customReflection(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.customReflection");
#endif
		try
		{
			UnityEngine.Cubemap arg0 = (UnityEngine.Cubemap)ToLua.CheckObject(L, 2, typeof(UnityEngine.Cubemap));
			UnityEngine.RenderSettings.customReflection = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_reflectionIntensity(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.reflectionIntensity");
#endif
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.reflectionIntensity = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_reflectionBounces(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.reflectionBounces");
#endif
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.reflectionBounces = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_defaultReflectionMode(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.defaultReflectionMode");
#endif
		try
		{
			UnityEngine.Rendering.DefaultReflectionMode arg0 = (UnityEngine.Rendering.DefaultReflectionMode)ToLua.CheckObject(L, 2, typeof(UnityEngine.Rendering.DefaultReflectionMode));
			UnityEngine.RenderSettings.defaultReflectionMode = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_defaultReflectionResolution(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.defaultReflectionResolution");
#endif
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.defaultReflectionResolution = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_haloStrength(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.haloStrength");
#endif
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.haloStrength = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_flareStrength(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.flareStrength");
#endif
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.flareStrength = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_flareFadeSpeed(IntPtr L)
	{
#if UNITY_EDITOR
        ToluaProfiler.AddCallRecord("UnityEngine.RenderSettings.flareFadeSpeed");
#endif
		try
		{
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.RenderSettings.flareFadeSpeed = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

