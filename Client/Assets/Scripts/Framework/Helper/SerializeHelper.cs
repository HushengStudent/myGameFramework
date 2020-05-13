/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/18 22:35:14
** desc:  序列化工具;
*********************************************************************************/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Framework
{
    public static class SerializeHelper
    {
        public static void SerializeXml<T>(string path, T data)
        {
            using (var writer = new FileStream(path, FileMode.Create))
            {
                try
                {
                    var xml = new XmlSerializer(typeof(T));
                    xml.Serialize(writer, data);
                }
                catch (Exception e)
                {
                    LogHelper.PrintError(e.ToString());
                }
            }
        }

        public static T DeserializeXml<T>(string path)
        {
            try
            {
                using (var reader = new FileStream(path, FileMode.Open))
                {
                    var xml = new XmlSerializer(typeof(T));
                    return (T)xml.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                LogHelper.PrintError(e.ToString());
                return default(T);
            }
        }

        public static void SerializeBinary<T>(string path, T data)
        {
            using (var writer = new FileStream(path, FileMode.Create))
            {
                try
                {
                    var binary = new BinaryFormatter();
                    binary.Serialize(writer, data);
                }
                catch (Exception e)
                {
                    LogHelper.PrintError(e.ToString());
                }
            }
        }

        public static T DeserializeBinary<T>(string path)
        {
            try
            {
                using (var reader = new FileStream(path, FileMode.Open))
                {
                    var binary = new BinaryFormatter();
                    return (T)binary.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                LogHelper.PrintError(e.ToString());
                return default(T);
            }
        }
    }
}
