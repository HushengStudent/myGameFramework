/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/15 00:19:53
** desc:  zip工具;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 压缩解压缩辅助器接口;
    /// </summary>
    public interface IZipHelper
    {
        /// <summary>
        /// 压缩数据;
        /// </summary>
        /// <param name="bytes">要压缩的数据;</param>
        /// <returns>压缩后的数据;</returns>
        byte[] Compress(byte[] bytes);

        /// <summary>
        /// 解压缩数据;
        /// </summary>
        /// <param name="bytes">要解压缩的数据;</param>
        /// <returns>解压缩后的数据;</returns>
        byte[] Decompress(byte[] bytes);
    }

    public static class ZipUtility
    {

        private static IZipHelper _zipHelper = null;

        /// <summary>
        /// 设置压缩解压缩辅助器;
        /// </summary>
        /// <param name="zipHelper">要设置的压缩解压缩辅助器;</param>
        public static void SetZipHelper(IZipHelper zipHelper)
        {
            _zipHelper = zipHelper;
        }

        /// <summary>
        /// 压缩数据;
        /// </summary>
        /// <param name="bytes">要压缩的数据;</param>
        /// <returns>压缩后的数据;</returns>
        public static byte[] Compress(byte[] bytes)
        {
            if (_zipHelper == null)
            {
                throw new Exception("Zip helper is invalid.");
            }
            return _zipHelper.Compress(bytes);
        }

        /// <summary>
        /// 解压缩数据;
        /// </summary>
        /// <param name="bytes">要解压缩的数据;</param>
        /// <returns>解压缩后的数据;</returns>
        public static byte[] Decompress(byte[] bytes)
        {
            if (_zipHelper == null)
            {
                throw new Exception("Zip helper is invalid.");
            }
            return _zipHelper.Decompress(bytes);
        }
    }
}