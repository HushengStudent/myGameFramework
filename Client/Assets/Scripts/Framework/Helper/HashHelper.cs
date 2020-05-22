/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 22:44:23
** desc:  Hash����;
*********************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Framework
{
    public static class HashHelper
    {
        public static string GetMD5(string str)
        {

            return GetMD5(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// �������������MD5;
        /// </summary>
        /// <param name="bytes">ָ���Ķ�������;</param>
        /// <returns>������MD5;</returns>
        public static string GetMD5(byte[] bytes)
        {
            using (var alg = MD5.Create())
            {
                var data = alg.ComputeHash(bytes);
                var stringBuilder = new StringBuilder();
                for (var i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// �ļ�MD5;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileMD5(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            var bufferSize = 1024 * 16;//��������С16K,Ĭ��4K
            var buffer = new byte[bufferSize];
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var md5 = MD5.Create();
                var length = 0;
                var output = new byte[bufferSize];
                while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    md5.TransformBlock(buffer, 0, length, output, 0);
                }
                md5.TransformFinalBlock(buffer, 0, 0);
                var hash = BitConverter.ToString(md5.Hash).Replace("-", "");
                md5.Clear();
                stream.Close();
                return hash;
            }
        }

        /// <summary>
        /// �����ַ�����Hash;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetHash(string str)
        {
            return GetHash(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// ������������Hash;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetHash(Type type)
        {
            return GetHash(type.Name);
        }

        /// <summary>
        /// �������������Hash;
        /// </summary>
        /// <param name="bytes">ָ���Ķ�������;</param>
        /// <returns>������Hash;</returns>
        public static string GetHash(byte[] textBytes)
        {
            using (var algorithm = SHA256.Create())
            {
                var stringBuilder = new StringBuilder();
                var hash = algorithm.ComputeHash(textBytes);
                for (var i = 0; i < hash.Length; i++)
                {
                    stringBuilder.Append(hash[i].ToString("X2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}