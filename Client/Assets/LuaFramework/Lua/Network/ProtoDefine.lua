---
--- Created by husheng.
--- DateTime: 2018/1/8 23:37
--- 协议序列号
---

ProtoID =
{
    LoginRequest = 10011,
    LoginResponse = 10012,
}

function GetProtoID(name)
    return ProtoID[name]
end