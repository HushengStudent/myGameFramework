/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/22 00:00:58
** desc:  字符串工具集;
*********************************************************************************/

using System.Text;

namespace Framework
{
    public static class StringHelper
    {
        public static string GetUTF8String(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}