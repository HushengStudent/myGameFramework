---
--- 协议序列号
---

module("Protol", package.seeall)


ProtoID = {
    LoginRequest = 10011,
    LoginResponse = 10012,
}

function GetProtoID(name)
    return ProtoID[name]
end