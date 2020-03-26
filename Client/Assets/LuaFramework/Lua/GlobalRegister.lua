---
--- 禁止写入_G
---

function RegisterGlobal(name, target)
    rawset(_G, name, target or false)
end

function FindGlobal( name )
    return rawget(_G, name)
end

setmetatable(_G, {
    __index = function(table, key)
        if not FindGlobal(key) then
            logError(string.format("[GlobalRegister]error:_G.__index not find: %s.", tostring(key)))
        end
        return nil
    end,

    __newindex = function(table, key, value)
        if not FindGlobal(key) then
            logError(string.format("[GlobalRegister]error:_G.__newindex not find: %s.", tostring(key)))
        else
            RegisterGlobal(key, value)
        end
    end,
} )
