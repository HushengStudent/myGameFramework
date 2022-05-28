/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/05 02:40:11
** desc:  UnityWebRequest下载工具;
*********************************************************************************/

using System.Collections;
using System.IO;
using UnityEngine.Networking;

namespace Framework.PatchModule
{
    public class UnityWebRequestHelper
    {
        public float Progress { get; private set; } = 0f;

        public bool IsDone { get; private set; } = false;

        public bool IsStop { get; private set; } = false;

        public DownLoadStartEventHandler StartHandler = null;
        public DownLoadErrorEventHandler ErrorHandler = null;
        public DownLoadProgressEventHandler ProgressHandler = null;
        public DownLoadSuccessEventHandler SuccessHandler = null;

        public IEnumerator StartDownLoad(string url, string filePath)
        {
            StartHandler?.Invoke();

            var req = UnityWebRequest.Head(url);
            yield return req.SendWebRequest();
            var allLength = long.Parse(req.GetResponseHeader("Content-Length"));
            req.Dispose();

            var dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var fileLength = fileStream.Length;
                if (fileLength < allLength)
                {
                    fileStream.Seek(fileLength, SeekOrigin.Begin);
                    var request = UnityWebRequest.Get(url);
                    request.SetRequestHeader("Range", "bytes=" + fileLength + "-" + allLength);
                    request.SendWebRequest();

                    var index = 0;
                    while (!request.isDone)
                    {
                        if (IsStop)
                        {
                            break;
                        }
                        var buff = request.downloadHandler.data;
                        if (buff != null)
                        {
                            var length = buff.Length - index;
                            fileStream.Write(buff, index, length);
                            index += length;
                            fileLength += length;
                            if (fileLength == length)
                            {
                                Progress = 1f;
                            }
                            else
                            {
                                Progress = fileLength / (float)length;
                            }
                        }
                        yield return null;
                    }
                    var isError = request.isNetworkError;
                    if (isError)
                    {
                        ErrorHandler?.Invoke(request.error);
                    }
                    else
                    {
                        ProgressHandler?.Invoke(Progress);
                    }
                    request.Dispose();
                }
                else
                {
                    Progress = 1f;
                }
                fileStream.Close();
                fileStream.Dispose();
            }
            if (Progress >= 1f)
            {
                IsDone = true;
                SuccessHandler?.Invoke();
            }
        }

        public void Stop()
        {
            IsStop = true;
        }
    }
}