/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 22:44:23
** desc:  Hash工具;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Framework
{
    public static class HashHelper
    {
        public static string GetHash(string str)
        {
            return GetHash(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 计算二进制流的Hash;
        /// </summary>
        /// <param name="bytes">指定的二进制流;</param>
        /// <returns>计算后的Hash;</returns>
        public static string GetHash(byte[] bytes)
        {
            MD5 alg = new MD5CryptoServiceProvider();
            byte[] data = alg.ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}