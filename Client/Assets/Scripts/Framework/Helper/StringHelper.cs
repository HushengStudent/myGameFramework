/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/22 00:00:58
** desc:  ×Ö·û´®¹¤¾ß¼¯;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework
{
    public static class StringHelper
    {
        public static string GetUTF8String(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}