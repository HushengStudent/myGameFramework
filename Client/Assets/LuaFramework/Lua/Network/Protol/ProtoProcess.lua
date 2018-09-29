---
--- Created by husheng.
--- DateTime: 2018/9/28 1:55
---
module("Protol.ProtoProcess", package.seeall)

require "Protol.ProtoDefine"

ProtoProcessTable= {

    [Protol.ProtoDefine.ProtoID.LoginResponse] =
    function(buffer)
        TestProcessPblua(buffer)
    end,


}

function Process(msg)
    logGreen("----->>>id:"..tostring(msg[0]))
    logGreen("----->>>buffer:"..tostring(msg[1]))

    local func = ProtoProcessTable[msg[0]]
    if func then
        func(msg[1])
    end
end