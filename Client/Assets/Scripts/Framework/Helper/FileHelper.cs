/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/15 23:13:47
** desc:  文件读写;
*********************************************************************************/

using System;
using System.IO;

namespace Framework
{
    public static class FileHelper
    {
        #region Txt

        /// <summary>
        /// 保存Txt;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        public static void Write2Txt(string path, string[] contents)
        {
            File.WriteAllLines(path, contents);
        }
        /// <summary>
        /// 读取Txt;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] ReadFromTxt(string path)
        {
            return File.ReadAllLines(path);
        }

        /// <summary>
        /// 保存Txt;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        public static void Write2Txt(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
        /// <summary>
        /// 读取Txt;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        #endregion

        #region Bytes

        /// <summary>
        /// 保存Bytes;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        public static void Write2Bytes(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }
        /// <summary>
        /// 读取Bytes;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] ReadFromBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        #endregion

        #region File

        //参考:http://www.jb51.net/article/113407.htm;
        private static byte[] _bufferBytes = new byte[1024];

        public static byte[] ReadFromFile(string path)
        {
            MemoryStream ms = new MemoryStream();
            ms.Position = 0;
            ms.SetLength(0);
            try
            {
                using (FileStream fsRead = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    if (fsRead.CanRead)
                    {
                        long leftLength = fsRead.Length;     //还没有读取的文件内容长度;
                        int maxLength = _bufferBytes.Length; //每次读取的最大字节数;
                        int num = 0;                         //每次实际返回的字节数长度;
                        int fileStart = 0;                   //文件开始读取的位置;
                        while (leftLength > 0)
                        {
                            fsRead.Position = fileStart;     //设置文件流的读取位置;
                            if (leftLength < maxLength)
                            {
                                num = fsRead.Read(_bufferBytes, 0, Convert.ToInt32(leftLength));
                            }
                            else
                            {
                                num = fsRead.Read(_bufferBytes, 0, maxLength);
                            }
                            if (num == 0)
                            {
                                break;
                            }
                            fileStart += num;
                            leftLength -= num;
                            ms.Write(_bufferBytes, 0, num);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.PrintWarning(e.ToString());
            }
            return ms.GetBuffer();
        }

        #endregion
    }
}
