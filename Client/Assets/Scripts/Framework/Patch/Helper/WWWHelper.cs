/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/07 22:50:56
** desc:  WWW下载工具;
*********************************************************************************/

using System.Collections;
using UnityEngine;

namespace Framework.PatchModule
{
    public class WWWHelper
    {
        private WWW _www = null;

        public float Progress { get; private set; } = 0f;

        public bool IsDone { get; private set; } = false;

        public DownLoadStartEventHandler StartHandler = null;
        public DownLoadErrorEventHandler ErrorHandler = null;
        public DownLoadProgressEventHandler ProgressHandler = null;
        public DownLoadSuccessEventHandler SuccessHandler = null;

        public IEnumerator StartDownLoad(string url, string filePath)
        {
            StartHandler?.Invoke();

            //Dictionary<string, string> header = new Dictionary<string, string>();
            //header.Add("Range", string.Format("bytes={0}-", from.ToString()));

            //Dictionary<string, string> header = new Dictionary<string, string>();
            //header.Add("Range", string.Format("bytes={0}-{1}", from.ToString(), to.ToString()));

            //_www = new WWW(url, null, header);

            using (_www = new WWW(url))
            {
                if (!_www.isDone)
                {
                    Progress = _www.progress;
                    ProgressHandler?.Invoke(Progress);
                    yield return null;
                }
                if (!string.IsNullOrEmpty(_www.error))
                {
                    ErrorHandler?.Invoke(_www.error);
                }
                else
                {
                    FileHelper.Write2Bytes(filePath, _www.bytes);
                    IsDone = true;
                    SuccessHandler?.Invoke();
                }
                _www.Dispose();
            }
        }
    }
}