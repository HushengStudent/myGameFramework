---
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
            declareGlobal(key, value)
        else
            logError(string.format("错误：%s 写入lua _G 表!", tostring(key)))
        end
    end,
} )
