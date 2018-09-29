---
--- Created by husheng.
--- DateTime: 2018/1/31 23:54
--- lua网络模块
---

require "Protol.ProtoProcess"

NetManager = class("NetManager")

function NetManager:ctor() end

function NetManager:GetManagerName()
    log("NetManager")
end

function NetManager:SendReq(msg)

end

function NetManager:SendNtf(msg)

end
