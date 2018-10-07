/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/07 16:08:43
** desc:  版本更新事件;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum UpdateCode : int
    {
        ConnectUrlError = 0,
        DownloadInterrupt = 1,
        DiskSpaceNotEnough = 2,
    }

    public delegate void UpdateStartEventHandler();
    public delegate void UpdateErrorEventHandler(UpdateCode code);
    public delegate void UpdateSuccessEventHandler();
}