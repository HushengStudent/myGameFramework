---
--- 禁止写入_G
---

function RegisterGlobal( name, target )
    rawset( _G, name, target or false )
end

function FindGlobal( name )
    return rawget( _G, name )
end

setmetatable( _G, {
    __newindex = function( table, key, value )
        local targetFunc = debug.getinfo(2)
        if _G.module == targetFunc.func then
            RegisterGlobal(key, value)
        else
            logError(string.format("[GlobalRegister]error:%s write to _G.", tostring(key)))
        end
    end,
} )
