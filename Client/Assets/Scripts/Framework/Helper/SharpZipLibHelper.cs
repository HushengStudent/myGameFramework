/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2021/11/01 20:09:39
** desc:  SharpZipLib;
*********************************************************************************/

using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Framework
{
    public static class SharpZipLibHelper
    {
        #region ZipCallback
        public abstract class ZipCallback
        {
            /// <summary>
            /// 压缩单个文件或文件夹前执行的回调;
            /// </summary>
            /// <param name="entry"></param>
            /// <returns>如果返回true,则压缩文件或文件夹,反之则不压缩文件或文件夹</returns>
            public virtual bool OnPreZip(ZipEntry entry)
            {
                return true;
            }

            /// <summary>
            /// 压缩单个文件或文件夹后执行的回调;
            /// </summary>
            /// <param name="entry"></param>
            public virtual void OnPostZip(ZipEntry entry) { }

            /// <summary>
            /// 压缩执行完毕后的回调;
            /// </summary>
            /// <param name="result">true表示压缩成功,false表示压缩失败</param>
            public virtual void OnFinished(bool result) { }
        }
        #endregion

        #region UnzipCallback
        public abstract class UnzipCallback
        {
            /// <summary>
            /// 解压单个文件或文件夹前执行的回调;
            /// </summary>
            /// <param name="entry"></param>
            /// <returns>如果返回true,则压缩文件或文件夹,反之则不压缩文件或文件夹</returns>
            public virtual bool OnPreUnzip(ZipEntry entry)
            {
                return true;
            }

            /// <summary>
            /// 解压单个文件或文件夹后执行的回调;
            /// </summary>
            /// <param name="entry"></param>
            public virtual void OnPostUnzip(ZipEntry entry) { }

            /// <summary>
            /// 解压执行完毕后的回调;
            /// </summary>
            /// <param name="result">true表示解压成功,false表示解压失败</param>
            public virtual void OnFinished(bool result) { }
        }
        #endregion

        /// <summary>
        /// 压缩文件和文件夹;
        /// </summary>
        /// <param name="fileOrDirectoryArray">文件夹路径和文件名</param>
        /// <param name="outputPathName">压缩后的输出路径文件名</param>
        /// <param name="password">压缩密码</param>
        /// <param name="zipCallback">ZipCallback对象,负责回调</param>
        /// <returns></returns>
        public static bool Zip(string[] fileOrDirectoryArray, string outputPathName,
            string password = null, ZipCallback zipCallback = null)
        {
            if ((null == fileOrDirectoryArray) || string.IsNullOrEmpty(outputPathName))
            {
                if (null != zipCallback)
                {
                    zipCallback.OnFinished(false);
                }

                return false;
            }

            var zipOutputStream = new ZipOutputStream(File.Create(outputPathName));
            zipOutputStream.SetLevel(6);    // 压缩质量和压缩速度的平衡点;
            if (!string.IsNullOrEmpty(password))
            {
                zipOutputStream.Password = password;
            }

            for (var index = 0; index < fileOrDirectoryArray.Length; ++index)
            {
                var result = false;
                var fileOrDirectory = fileOrDirectoryArray[index];
                if (Directory.Exists(fileOrDirectory))
                {
                    result = ZipDirectory(fileOrDirectory, string.Empty, zipOutputStream, zipCallback);
                }
                else if (File.Exists(fileOrDirectory))
                {
                    result = ZipFile(fileOrDirectory, string.Empty, zipOutputStream, zipCallback);
                }
                if (!result)
                {
                    if (null != zipCallback)
                    {
                        zipCallback.OnFinished(false);
                    }

                    return false;
                }
            }

            zipOutputStream.Finish();
            zipOutputStream.Close();

            if (null != zipCallback)
            {
                zipCallback.OnFinished(true);
            }

            return true;
        }

        /// <summary>
        /// 解压Zip包;
        /// </summary>
        /// <param name="filePathName">Zip包的文件路径名</param>
        /// <param name="outputPath">解压输出路径</param>
        /// <param name="password">解压密码</param>
        /// <param name="unzipCallback">UnzipCallback对象,负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(string filePathName, string outputPath,
            string password = null, UnzipCallback unzipCallback = null)
        {
            if (string.IsNullOrEmpty(filePathName) || string.IsNullOrEmpty(outputPath))
            {
                if (null != unzipCallback)
                {
                    unzipCallback.OnFinished(false);
                }

                return false;
            }

            try
            {
                return UnzipFile(File.OpenRead(filePathName), outputPath, password, unzipCallback);
            }
            catch (System.Exception e)
            {
                LogHelper.PrintError($"[SharpZipLibHelper.UnzipFile]: {e.ToString()}");

                if (null != unzipCallback)
                {
                    unzipCallback.OnFinished(false);
                }

                return false;
            }
        }

        /// <summary>
        /// 解压Zip包;
        /// </summary>
        /// <param name="fileBytes">Zip包字节数组</param>
        /// <param name="outputPath">解压输出路径</param>
        /// <param name="password">解压密码</param>
        /// <param name="unzipCallback">UnzipCallback对象,负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(byte[] fileBytes, string outputPath,
            string password = null, UnzipCallback unzipCallback = null)
        {
            if ((null == fileBytes) || string.IsNullOrEmpty(outputPath))
            {
                if (null != unzipCallback)
                {
                    unzipCallback.OnFinished(false);
                }

                return false;
            }

            var result = UnzipFile(new MemoryStream(fileBytes), outputPath, password, unzipCallback);
            if (!result)
            {
                if (null != unzipCallback)
                {
                    unzipCallback.OnFinished(false);
                }
            }

            return result;
        }

        /// <summary>
        /// 解压Zip包;
        /// </summary>
        /// <param name="inputStream">Zip包输入流</param>
        /// <param name="outputPath">解压输出路径</param>
        /// <param name="password">解压密码</param>
        /// <param name="unzipCallback">UnzipCallback对象,负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(Stream inputStream, string outputPath,
            string password = null, UnzipCallback unzipCallback = null)
        {
            if ((null == inputStream) || string.IsNullOrEmpty(outputPath))
            {
                if (null != unzipCallback)
                {
                    unzipCallback.OnFinished(false);
                }

                return false;
            }

            // 创建文件目录;
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // 解压Zip包;
            ZipEntry entry = null;
            using (var zipInputStream = new ZipInputStream(inputStream))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zipInputStream.Password = password;
                }

                while (null != (entry = zipInputStream.GetNextEntry()))
                {
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        continue;
                    }

                    if ((null != unzipCallback) && !unzipCallback.OnPreUnzip(entry))
                    {
                        continue;   // 过滤;
                    }

                    var filePathName = Path.Combine(outputPath, entry.Name);

                    // 创建文件目录;
                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(filePathName);
                        continue;
                    }

                    // 写入文件;
                    try
                    {
                        using (var fileStream = File.Create(filePathName))
                        {
                            var bytes = new byte[1024];
                            while (true)
                            {
                                var count = zipInputStream.Read(bytes, 0, bytes.Length);
                                if (count > 0)
                                {
                                    fileStream.Write(bytes, 0, count);
                                }
                                else
                                {
                                    if (null != unzipCallback)
                                    {
                                        unzipCallback.OnPostUnzip(entry);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        LogHelper.PrintError($"[SharpZipLibHelper.UnzipFile]: {e.ToString()}");

                        if (null != unzipCallback)
                        {
                            unzipCallback.OnFinished(false);
                        }

                        return false;
                    }
                }
            }

            if (null != unzipCallback)
            {
                unzipCallback.OnFinished(true);
            }

            return true;
        }

        /// <summary>
        /// 压缩文件;
        /// </summary>
        /// <param name="filePathName">文件路径名</param>
        /// <param name="parentRelPath">要压缩的文件的父相对文件夹</param>
        /// <param name="zipOutputStream">压缩输出流</param>
        /// <param name="zipCallback">ZipCallback对象,负责回调</param>
        /// <returns></returns>
        private static bool ZipFile(string filePathName, string parentRelPath,
            ZipOutputStream zipOutputStream, ZipCallback zipCallback = null)
        {
            //Crc32 crc32 = new Crc32();
            ZipEntry entry = null;
            FileStream fileStream = null;
            try
            {
                var entryName = parentRelPath + '/' + Path.GetFileName(filePathName);
                entry = new ZipEntry(entryName)
                {
                    DateTime = System.DateTime.Now
                };

                if ((null != zipCallback) && !zipCallback.OnPreZip(entry))
                {
                    return true;    // 过滤;
                }

                fileStream = File.OpenRead(filePathName);
                var buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();

                entry.Size = buffer.Length;

                //crc32.Reset();
                //crc32.Update(buffer);
                //entry.Crc = crc32.Value;

                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (System.Exception e)
            {
                LogHelper.PrintError($"[SharpZipLibHelper.ZipFile]: {e.ToString()}");
                return false;
            }
            finally
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

            if (null != zipCallback)
            {
                zipCallback.OnPostZip(entry);
            }

            return true;
        }

        /// <summary>
        /// 压缩文件夹;
        /// </summary>
        /// <param name="path">要压缩的文件夹</param>
        /// <param name="parentRelPath">要压缩的文件夹的父相对文件夹</param>
        /// <param name="zipOutputStream">压缩输出流</param>
        /// <param name="zipCallback">ZipCallback对象,负责回调</param>
        /// <returns></returns>
        private static bool ZipDirectory(string path, string parentRelPath,
            ZipOutputStream zipOutputStream, ZipCallback zipCallback = null)
        {
            ZipEntry entry = null;
            try
            {
                var entryName = Path.Combine(parentRelPath, Path.GetFileName(path) + '/');
                entry = new ZipEntry(entryName)
                {
                    DateTime = System.DateTime.Now,
                    Size = 0
                };

                if ((null != zipCallback) && !zipCallback.OnPreZip(entry))
                {
                    return true;    // 过滤;
                }

                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Flush();

                var files = Directory.GetFiles(path);
                for (var index = 0; index < files.Length; ++index)
                {
                    ZipFile(files[index], Path.Combine(parentRelPath, Path.GetFileName(path)), zipOutputStream, zipCallback);
                }
            }
            catch (System.Exception e)
            {
                LogHelper.PrintError($"[SharpZipLibHelper.ZipDirectory]: {e.ToString()}");
                return false;
            }

            var directories = Directory.GetDirectories(path);
            for (var index = 0; index < directories.Length; ++index)
            {
                if (!ZipDirectory(directories[index], Path.Combine(parentRelPath, Path.GetFileName(path)), zipOutputStream, zipCallback))
                {
                    return false;
                }
            }

            if (null != zipCallback)
            {
                zipCallback.OnPostZip(entry);
            }
            return true;
        }
    }
}