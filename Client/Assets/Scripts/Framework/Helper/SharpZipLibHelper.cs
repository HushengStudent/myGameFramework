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
            /// ѹ�������ļ����ļ���ǰִ�еĻص�;
            /// </summary>
            /// <param name="entry"></param>
            /// <returns>�������true,��ѹ���ļ����ļ���,��֮��ѹ���ļ����ļ���</returns>
            public virtual bool OnPreZip(ZipEntry entry)
            {
                return true;
            }

            /// <summary>
            /// ѹ�������ļ����ļ��к�ִ�еĻص�;
            /// </summary>
            /// <param name="entry"></param>
            public virtual void OnPostZip(ZipEntry entry) { }

            /// <summary>
            /// ѹ��ִ����Ϻ�Ļص�;
            /// </summary>
            /// <param name="result">true��ʾѹ���ɹ�,false��ʾѹ��ʧ��</param>
            public virtual void OnFinished(bool result) { }
        }
        #endregion

        #region UnzipCallback
        public abstract class UnzipCallback
        {
            /// <summary>
            /// ��ѹ�����ļ����ļ���ǰִ�еĻص�;
            /// </summary>
            /// <param name="entry"></param>
            /// <returns>�������true,��ѹ���ļ����ļ���,��֮��ѹ���ļ����ļ���</returns>
            public virtual bool OnPreUnzip(ZipEntry entry)
            {
                return true;
            }

            /// <summary>
            /// ��ѹ�����ļ����ļ��к�ִ�еĻص�;
            /// </summary>
            /// <param name="entry"></param>
            public virtual void OnPostUnzip(ZipEntry entry) { }

            /// <summary>
            /// ��ѹִ����Ϻ�Ļص�;
            /// </summary>
            /// <param name="result">true��ʾ��ѹ�ɹ�,false��ʾ��ѹʧ��</param>
            public virtual void OnFinished(bool result) { }
        }
        #endregion

        /// <summary>
        /// ѹ���ļ����ļ���;
        /// </summary>
        /// <param name="fileOrDirectoryArray">�ļ���·�����ļ���</param>
        /// <param name="outputPathName">ѹ��������·���ļ���</param>
        /// <param name="password">ѹ������</param>
        /// <param name="zipCallback">ZipCallback����,����ص�</param>
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
            zipOutputStream.SetLevel(6);    // ѹ��������ѹ���ٶȵ�ƽ���;
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
        /// ��ѹZip��;
        /// </summary>
        /// <param name="filePathName">Zip�����ļ�·����</param>
        /// <param name="outputPath">��ѹ���·��</param>
        /// <param name="password">��ѹ����</param>
        /// <param name="unzipCallback">UnzipCallback����,����ص�</param>
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
        /// ��ѹZip��;
        /// </summary>
        /// <param name="fileBytes">Zip���ֽ�����</param>
        /// <param name="outputPath">��ѹ���·��</param>
        /// <param name="password">��ѹ����</param>
        /// <param name="unzipCallback">UnzipCallback����,����ص�</param>
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
        /// ��ѹZip��;
        /// </summary>
        /// <param name="inputStream">Zip��������</param>
        /// <param name="outputPath">��ѹ���·��</param>
        /// <param name="password">��ѹ����</param>
        /// <param name="unzipCallback">UnzipCallback����,����ص�</param>
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

            // �����ļ�Ŀ¼;
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // ��ѹZip��;
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
                        continue;   // ����;
                    }

                    var filePathName = Path.Combine(outputPath, entry.Name);

                    // �����ļ�Ŀ¼;
                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(filePathName);
                        continue;
                    }

                    // д���ļ�;
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
        /// ѹ���ļ�;
        /// </summary>
        /// <param name="filePathName">�ļ�·����</param>
        /// <param name="parentRelPath">Ҫѹ�����ļ��ĸ�����ļ���</param>
        /// <param name="zipOutputStream">ѹ�������</param>
        /// <param name="zipCallback">ZipCallback����,����ص�</param>
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
                    return true;    // ����;
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
        /// ѹ���ļ���;
        /// </summary>
        /// <param name="path">Ҫѹ�����ļ���</param>
        /// <param name="parentRelPath">Ҫѹ�����ļ��еĸ�����ļ���</param>
        /// <param name="zipOutputStream">ѹ�������</param>
        /// <param name="zipCallback">ZipCallback����,����ص�</param>
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
                    return true;    // ����;
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