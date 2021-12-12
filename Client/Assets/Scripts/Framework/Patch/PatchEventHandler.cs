/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/07 16:08:43
** desc:  版本更新事件;
*********************************************************************************/

namespace Framework.PatchModule
{
    public enum PatchCode : int
    {
        ConnectUrlError = 0,
        DownloadInterrupt = 1,
        DiskSpaceNotEnough = 2,
    }

    public delegate void PatchStartEventHandler();
    public delegate void PatchErrorEventHandler(PatchCode patchCode);
    public delegate void PatchSuccessEventHandler();

    public delegate void DownLoadStartEventHandler();
    public delegate void DownLoadErrorEventHandler(string error);
    public delegate void DownLoadProgressEventHandler(float progress);
    public delegate void DownLoadSuccessEventHandler();

}