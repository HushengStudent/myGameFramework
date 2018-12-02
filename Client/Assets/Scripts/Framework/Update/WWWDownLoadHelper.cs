/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/07 22:50:56
** desc:  WWWœ¬‘ÿπ§æﬂ;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class WWWDownLoadHelper
    {
        private WWW _www = null;

        private float _progress = 0f;
        private bool _isDone = false;

        public float Progress { get { return _progress; } }
        public bool IsDone { get { return _isDone; } }

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

            //Dictionary<string, string> header = new Dictionary<string, string>();
            //header.Add("Range", string.Format("bytes={0}-", from.ToString()));

            //Dictionary<string, string> header = new Dictionary<string, string>();
            //header.Add("Range", string.Format("bytes={0}-{1}", from.ToString(), to.ToString()));

            //_www = new WWW(url, null, header);

            using (_www = new WWW(url))
            {
                if (!_www.isDone)
                {
                    _progress = _www.progress;
                    if (ProgressHandler != null)
                    {
                        ProgressHandler(_progress);
                    }
                    yield return null;
                }
                if (!string.IsNullOrEmpty(_www.error))
                {
                    if (ErrorHandler != null)
                    {
                        ErrorHandler(_www.error);
                    }
                }
                else
                {
                    FileHelper.Write2Bytes(filePath, _www.bytes);
                    _isDone = true;
                    if (SuccessHandler != null)
                    {
                        SuccessHandler();
                    }
                }
                _www.Dispose();
            }
        }
    }
}