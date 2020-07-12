---
--- 协议处理
---

local ProtoProcessTable = {

    [ProtoID.LoginResponse] = function(buffer)
        TestProcessPblua(buffer)
    end,


}

function Process(msg)
    local func = ProtoProcessTable[msg[0]]
    if func then
        func(msg[1])
    end
end