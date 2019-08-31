---
--- Created by husheng.
--- DateTime: 2018/1/15 23:03
--- lua common fuction:参考cocos2d-x lua-bulding 实现
---

---table's number
function table.nums(t)
    local count = 0
    for k, v in pairs(t) do
        count = count + 1
    end
    return count
end

---table's keys
function table.keys(hashtable)
    local keys = {}
    for k, v in pairs(hashtable) do
        keys[#keys + 1] = k
    end
    return keys
end

---table's values
function table.values(hashtable)
    local values = {}
    for k, v in pairs(hashtable) do
        values[#values + 1] = v
    end
    return values
end

---
function table.merge(dest, src)
    for k, v in pairs(src) do
        dest[k] = v
    end
end

---
function table.walk(t, fn)
    for k,v in pairs(t) do
        fn(v, k)
    end
end

---string split
function string.split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter=='') then return false end
    local pos,arr = 0, {}
    -- for each divider found
    for st,sp in function() return string.find(input, delimiter, pos, true) end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end

---table dump
function table.dump(t)
    for k, v in pairs(t) do
        logGreen("--->>>key:"..tostring(k).." --->>>value:"..tostring(v))
    end
end

function IsNil(uObj)
    return uObj == nil or uObj:Equals(nil)
end

function ListToTable(list)
    local t = {}
    if list then
        for i = 0, list.Count - 1 do
            table.insert(t, list[i])
        end
    end
    return t
end

function ArrayToTable(array)
    local t = {}
    if array then
        for i = 0, array.Length - 1 do
            table.insert(t, array[i])
        end
    end
    return t
end

function DictToTable(dict)
    local t = {}
    if dict then
        local e = dict:GetEnumerator()
        while (e:MoveNext())
        do
            local current = e.Current
            local k = current.Key
            local v = current.Value
            table.insert(t, { k, v })
        end
    end
    return t
end