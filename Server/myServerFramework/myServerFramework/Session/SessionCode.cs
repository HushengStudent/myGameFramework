/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/26 23:44:48
** desc:  网络协议错误码;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;

namespace myServerFramework
{
    public enum SessionType
    {
        Unknown = 0,
        IPv4,
        IPv6,
    }

    public enum SessionErrorCode : int
    {
        ConnectError = 0,
        SendError,
        ReceiveError,
        StateError,
        SerializeError,
        DeserializeError,
        HeaderError,
        OutOfRangeError,
        StreamError,
    }

    public enum SessionCode : int
    {

    }
}
