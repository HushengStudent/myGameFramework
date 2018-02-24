using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoContract]
public class TestProtoReq //: ProtoBody
{
    [ProtoMember(1)]
    public int number;
}

[ProtoContract]
public class TestProtoRsp //: ProtoBody
{
    [ProtoMember(1)]
    public ProtoErrorCode errorCode;

    [ProtoMember(2)]
    public int number;
}
