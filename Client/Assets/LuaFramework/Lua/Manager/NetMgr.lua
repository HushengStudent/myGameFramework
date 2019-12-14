---
--- lua网络模块
---

module("Manager", package.seeall)

require "Protol.ProtoProcess"

NetMgr = class("NetMgr")

function NetMgr:ctor()
end

function NetMgr:GetMgrName()
    log("NetMgr")
end

function NetMgr:SendReq(msg)

end

function NetMgr:SendNtf(msg)

end

return NetMgr
