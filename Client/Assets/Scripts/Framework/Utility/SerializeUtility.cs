/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/18 22:35:14
** desc:  序列化工具;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class SerializeUtility
    {
        public static void SerializeXml<T>(string path, T data)
        {

        }

        public static T DeserializeXml<T>(string path)
        {
            return default(T);
        }

        public static void SerializeBin<T>(string path, T data)
        {

        }

        public static T DeserializeBin<T>(string path)
        {
            return default(T);
        }
    }
}
