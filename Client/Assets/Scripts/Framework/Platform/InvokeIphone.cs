/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/04 23:42:07
** desc:  iOS
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Framework
{
    public class InvokeIphone
    {
        public static string IOS_SDK_MANAGER = "";

        [DllImport("__Internal")]
        private static extern void Unity2Iphone(string methodName, string[] str, int length);

        [DllImport("__Internal")]
        private static extern string Unity2IphoneStr(string methodName, string[] str, int length);

        public static void CallIphoneStaticMethod(string className, string methodName, params string[] methodParams)
        {
            int length = methodParams.Length;
            Unity2Iphone(methodName, methodParams, length);
        }

        public static string CallIphoneStrStaticMethod(string className, string methodName, params string[] methodParams)
        {
            int length = methodParams.Length;
            string str = Unity2IphoneStr(methodName, methodParams, length);
            return str;
        }

        /*

        iOS调用Unity函数
        
        UnitySendMessage("GameObjectName1", "MethodName1", "Message to send");

        iOS实现Unity中声明的函数格式
        
        extern "C" 
        {
          float Unity2Iphone(string methodName, string[] str, int length);
        }
        extern "C" 
        {
          float Unity2IphoneStr(string methodName, string[] str, int length);
        }
        
        */
    }
}
