---
--- Created by husheng.
--- DateTime: 2018/9/28 1:55
---
module("Protol.ProtoProcess", package.seeall)

require "Protol.ProtoDefine"

ProtoProcessTable= {

    [ProtoDefine.LoginRequest] = function(buffer)
        TestProcessPblua(buffer)
    end,


}

function Process(id,buffer)
    local func = ProtoProcessTable[id]
    if func then
        func(buffer)
    end
end