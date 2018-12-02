/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/18 22:35:14
** desc:  序列化工具;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace Framework
{
    public static class SerializeHelper
    {
        public static void SerializeXml<T>(string path, T data)
        {
            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                try
                {
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    xml.Serialize(writer, data);
                }
                catch (Exception e)
                {
                    LogUtil.LogUtility.PrintError(e.ToString());
                }
            }
        }

        public static T DeserializeXml<T>(string path)
        {
            try
            {
                using (FileStream reader = new FileStream(path, FileMode.Open))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    return (T)xml.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                LogUtil.LogUtility.PrintError(e.ToString());
                return default(T);
            }
        }

        public static void SerializeBinary<T>(string path, T data)
        {
            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                try
                {
                    BinaryFormatter binary = new BinaryFormatter();
                    binary.Serialize(writer, data);
                }
                catch (Exception e)
                {
                    LogUtil.LogUtility.PrintError(e.ToString());
                }
            }
        }

        public static T DeserializeBinary<T>(string path)
        {
            try
            {
                using (FileStream reader = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter binary = new BinaryFormatter();
                    return (T)binary.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                LogUtil.LogUtility.PrintError(e.ToString());
                return default(T);
            }
        }
    }
}
