/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/04 23:41:40
** desc:  Android;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class InvokeAndroid
    {
        public static string ANDROID_SDK_MANAGER = "";

        public static void CallAndroidStaticMethod(string className, string methodName, params object[] methodParams)
        {
#if UNITY_ANDROID&&!UNITY_EDITOR
		AndroidJavaClass clazz = null;
		using (clazz = new AndroidJavaClass(className))
        {
			clazz.CallStatic(methodName, methodParams);
        }
		clazz.Dispose();
		clazz = null;
#endif
        }

        public static T CallAndroidStaticMethod<T>(string className, string methodName, params object[] methodParams)
        {
            T res = default(T);
#if UNITY_ANDROID&&!UNITY_EDITOR
		AndroidJavaClass clazz = null;
		using (clazz = new AndroidJavaClass(className))
		{
			res = clazz.CallStatic<T>(methodName, methodParams);
		}
		clazz.Dispose();
		clazz = null;
#endif
        return res;
        }

        public static void CallMethod(string className, string methodName, params object[] methodParams)
        {
#if UNITY_ANDROID&&!UNITY_EDITOR
		AndroidJavaObject clazzObj = null;
		using (clazzObj = new AndroidJavaObject(className))
		{
			clazzObj.Call(methodName, methodParams);
		}
		clazzObj.Dispose();
		clazzObj = null;
#endif
        }

        public static T CallMethod<T>(string className, string methodName, params object[] methodParams)
        {
            T res = default(T);
#if UNITY_ANDROID&&!UNITY_EDITOR
		AndroidJavaObject clazzObj = null;
		using (clazzObj = new AndroidJavaObject(className))
		{
			res = clazzObj.Call<T>(methodName, methodParams);
		}
		clazzObj.Dispose();
		clazzObj = null;
#endif
        return res;
        }
    }
}
