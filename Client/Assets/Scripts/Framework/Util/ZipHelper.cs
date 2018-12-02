/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/15 00:19:53
** desc:  ZIP文件解压工具;
*********************************************************************************/

using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public static class ZipHelper
    {
        public static byte[] Decompress(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                return bytes;
            }
            MemoryStream decompressedStream = null;
            MemoryStream memoryStream = null;
            try
            {
                decompressedStream = new MemoryStream();
                memoryStream = new MemoryStream(bytes);
                using (GZipInputStream gZipInputStream = new GZipInputStream(memoryStream))
                {
                    memoryStream = null;
                    int bytesRead = 0;
                    byte[] clip = new byte[0x1000];
                    while ((bytesRead = gZipInputStream.Read(clip, 0, clip.Length)) != 0)
                    {
                        decompressedStream.Write(clip, 0, bytesRead);
                    }
                }
                return decompressedStream.ToArray();
            }
            finally
            {
                if (decompressedStream != null)
                {
                    decompressedStream.Dispose();
                    decompressedStream = null;
                }
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            }
        }
    }
}