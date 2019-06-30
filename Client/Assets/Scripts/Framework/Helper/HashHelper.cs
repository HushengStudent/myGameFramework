/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 22:44:23
** desc:  Hash工具;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Framework
{
    public static class HashHelper
    {
        public static string GetMD5(string str)
        {

            return GetMD5(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 计算二进制流的MD5;
        /// </summary>
        /// <param name="bytes">指定的二进制流;</param>
        /// <returns>计算后的MD5;</returns>
        public static string GetMD5(byte[] bytes)
        {
            using (MD5 alg = MD5.Create())
            {
                byte[] data = alg.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// 计算字符串的Hash;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetHash(string str)
        {

            return GetHash(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 计算类型名的Hash;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetHash(Type type)
        {
            return GetHash(type.Name);
        }

        /// <summary>
        /// 计算二进制流的Hash;
        /// </summary>
        /// <param name="bytes">指定的二进制流;</param>
        /// <returns>计算后的Hash;</returns>
        public static string GetHash(byte[] textBytes)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                StringBuilder stringBuilder = new StringBuilder();
                byte[] hash = algorithm.ComputeHash(textBytes);
                for (int i = 0; i < hash.Length; i++)
                {
                    stringBuilder.Append(hash[i].ToString("X2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}