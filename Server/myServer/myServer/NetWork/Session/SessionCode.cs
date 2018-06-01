using System.Collections;
using System.Collections.Generic;

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
