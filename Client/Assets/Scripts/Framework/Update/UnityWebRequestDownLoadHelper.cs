/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/05 02:40:11
** desc:  UnityWebRequestœ¬‘ÿπ§æﬂ;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    public class UnityWebRequestDownLoadHelper
    {
        private float _progress;
        private bool _isDone;
        private bool _isStop;

        public float Progress { get { return _progress; } }
        public bool IsDone { get { return _isDone; } }
        public bool IsStop { get { return _isStop; } }

        public DownLoadStartEventHandler StartHandler;
        public DownLoadErrorEventHandler ErrorHandler;
        public DownLoadProgressEventHandler ProgressHandler;
        public DownLoadSuccessEventHandler SuccessHandler;

        public IEnumerator StartDownLoad(string url, string filePath)
        {
            if (StartHandler != null)
            {
                StartHandler();
            }

            var req = UnityWebRequest.Head(url);
            yield return req.Send();
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
                    request.Send();

                    var index = 0;
                    while (!request.isDone)
                    {
                        if (_isStop)
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
                                _progress = 1f;
                            }
                            else
                            {
                                _progress = fileLength / (float)length;
                            }
                        }
                        yield return null;
                    }
                    bool isError = false;
                    isError = request.isError;
                    if (isError)
                    {
                        if (ErrorHandler != null)
                        {
                            ErrorHandler(request.error);
                        }
                    }
                    else
                    {
                        if (ProgressHandler != null)
                        {
                            ProgressHandler(_progress);
                        }
                    }
                    request.Dispose();
                }
                else
                {
                    _progress = 1f;
                }
                fileStream.Close();
                fileStream.Dispose();
            }
            if (_progress >= 1f)
            {
                _isDone = true;
                if (SuccessHandler != null)
                {
                    SuccessHandler();
                }
            }
        }

        public void Stop()
        {
            _isStop = true;
        }
    }
}