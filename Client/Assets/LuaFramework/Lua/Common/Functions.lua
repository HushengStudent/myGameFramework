function table.nums(t)
    local count = 0
    for k, v in pairs(t) do
        count = count + 1
    end
    return count
end

function table.keys(hashtable)
    local keys = {}
    for k, v in pairs(hashtable) do
        keys[#keys + 1] = k
    end
    return keys
end

function table.values(hashtable)
    local values = {}
    for k, v in pairs(hashtable) do
        values[#values + 1] = v
    end
    return values
end

function table.merge(dest, src)
    for k, v in pairs(src) do
        dest[k] = v
    end
end

function table.insertto(dest, src, begin)
    begin = checkint(begin)
    if begin <= 0 then
        begin = #dest + 1
    end

    local len = #src
    for i = 0, len - 1 do
        dest[i + begin] = src[i + 1]
    end
end

function table.indexof(array, value, begin)
    for i = begin or 1, #array do
        if array[i] == value then
            return i
        end
    end
    return false
end

function table.keyof(hashtable, value)
    for k, v in pairs(hashtable) do
        if v == value then
            return k
        end
    end
    return nil
end

function table.removebyvalue(array, value, removeall)
    local c, i, max = 0, 1, #array
    while i <= max do
        if array[i] == value then
            table.remove(array, i)
            c = c + 1
            i = i - 1
            max = max - 1
            if not removeall then
                break
            end
        end
        i = i + 1
    end
    return c
end

function table.map(t, fn)
    for k, v in pairs(t) do
        t[k] = fn(v, k)
    end
end

function table.walk(t, fn)
    for k, v in pairs(t) do
        fn(v, k)
    end
end

function table.filter(t, fn)
    for k, v in pairs(t) do
        if not fn(v, k) then
            t[k] = nil
        end
    end
end

function table.unique(t, bArray)
    local check = {}
    local n = {}
    local idx = 1
    for k, v in pairs(t) do
        if not check[v] then
            if bArray then
                n[idx] = v
                idx = idx + 1
            else
                n[k] = v
            end
            check[v] = true
        end
    end
    return n
end

string._htmlspecialchars_set = {}
string._htmlspecialchars_set["&"] = "&amp;"
string._htmlspecialchars_set["\""] = "&quot;"
string._htmlspecialchars_set["'"] = "&#039;"
string._htmlspecialchars_set["<"] = "&lt;"
string._htmlspecialchars_set[">"] = "&gt;"

function string.htmlspecialchars(input)
    for k, v in pairs(string._htmlspecialchars_set) do
        input = string.gsub(input, k, v)
    end
    return input
end

function string.restorehtmlspecialchars(input)
    for k, v in pairs(string._htmlspecialchars_set) do
        input = string.gsub(input, v, k)
    end
    return input
end

function string.nl2br(input)
    return string.gsub(input, "\n", "<br />")
end

function string.text2html(input)
    input = string.gsub(input, "\t", "    ")
    input = string.htmlspecialchars(input)
    input = string.gsub(input, " ", "&nbsp;")
    input = string.nl2br(input)
    return input
end

function string.split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter == '') then
        return false
    end
    local pos, arr = 0, {}
    -- for each divider found
    for st, sp in function()
        return string.find(input, delimiter, pos, true)
    end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end

function string.ltrim(input)
    return string.gsub(input, "^[ \t\n\r]+", "")
end

function string.rtrim(input)
    return string.gsub(input, "[ \t\n\r]+$", "")
end

function string.trim(input)
    input = string.gsub(input, "^[ \t\n\r]+", "")
    return string.gsub(input, "[ \t\n\r]+$", "")
end

function string.ucfirst(input)
    return string.upper(string.sub(input, 1, 1)) .. string.sub(input, 2)
end

local function urlencodechar(char)
    return "%" .. string.format("%02X", string.byte(char))
end

function string.urlencode(input)
    -- convert line endings
    input = string.gsub(tostring(input), "\n", "\r\n")
    -- escape all characters but alphanumeric, '.' and '-'
    input = string.gsub(input, "([^%w%.%- ])", urlencodechar)
    -- convert spaces to "+" symbols
    return string.gsub(input, " ", "+")
end

function string.urldecode(input)
    input = string.gsub(input, "+", " ")
    input = string.gsub(input, "%%(%x%x)", function(h)
        return string.char(checknumber(h, 16))
    end)
    input = string.gsub(input, "\r\n", "\n")
    return input
end

local function dump_value_(v)
    if type(v) == "string" then
        v = "\"" .. v .. "\""
    end
    return tostring(v)
end

function dump(value, description, nesting)
    if type(nesting) ~= "number" then
        nesting = 3
    end

    local lookupTable = {}
    local result = {}

    local traceback = string.split(debug.traceback("", 2), "\n")
    print("dump from: " .. string.trim(traceback[3]))

    local function dump_(value, description, indent, nest, keylen)
        description = description or "<var>"
        local spc = ""
        if type(keylen) == "number" then
            spc = string.rep(" ", keylen - string.len(dump_value_(description)))
        end
        if type(value) ~= "table" then
            result[#result + 1 ] = string.format("%s%s%s = %s", indent, dump_value_(description), spc, dump_value_(value))
        elseif lookupTable[tostring(value)] then
            result[#result + 1 ] = string.format("%s%s%s = *REF*", indent, dump_value_(description), spc)
        else
            lookupTable[tostring(value)] = true
            if nest > nesting then
                result[#result + 1 ] = string.format("%s%s = *MAX NESTING*", indent, dump_value_(description))
            else
                result[#result + 1 ] = string.format("%s%s = {", indent, dump_value_(description))
                local indent2 = indent .. "    "
                local keys = {}
                local keylen = 0
                local values = {}
                for k, v in pairs(value) do
                    keys[#keys + 1] = k
                    local vk = dump_value_(k)
                    local vkl = string.len(vk)
                    if vkl > keylen then
                        keylen = vkl
                    end
                    values[k] = v
                end
                table.sort(keys, function(a, b)
                    if type(a) == "number" and type(b) == "number" then
                        return a < b
                    else
                        return tostring(a) < tostring(b)
                    end
                end)
                for i, k in ipairs(keys) do
                    dump_(values[k], k, indent2, nest + 1, keylen)
                end
                result[#result + 1] = string.format("%s}", indent)
            end
        end
    end
    dump_(value, description, " ", 1)

    local info = ""
    for i, line in ipairs(result) do
        info = info .. line .. "\r\n"
        --print(line)
    end

    log(info)
end

function checknumber(value, base)
    return tonumber(value, base) or 0
end

function checkint(value)
    return math.round(checknumber(value))
end

function checkbool(value)
    return (value ~= nil and value ~= false)
end

function checktable(value)
    if type(value) ~= "table" then
        value = {}
    end
    return value
end

local iskindof_
iskindof_ = function(cls, name)
    local __index = rawget(cls, "__index")
    if type(__index) == "table" and rawget(__index, "__cname") == name then
        return true
    end

    if rawget(cls, "__cname") == name then
        return true
    end
    local __supers = rawget(__index, "__supers")
    if not __supers then
        return false
    end
    for _, super in ipairs(__supers) do
        if iskindof_(super, name) then
            return true
        end
    end
    return false
end

function iskindof(obj, classname)
    local t = type(obj)
    if t ~= "table" and t ~= "userdata" then
        return false
    end

    local mt
    if t == "userdata" then
        if tolua.iskindof(obj, classname) then
            return true
        end
        mt = getmetatable(tolua.getpeer(obj))
    else
        mt = getmetatable(obj)
    end
    if mt then
        return iskindof_(mt, classname)
    end
    return false
end

function clone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local newObject = {}
        lookup_table[object] = newObject
        for key, value in pairs(object) do
            newObject[_copy(key)] = _copy(value)
        end
        return setmetatable(newObject, getmetatable(object))
    end
    return _copy(object)
end

function handler(obj, method)
    return function(...)
        return method(obj, ...)
    end
end

function math.round(value)
    value = checknumber(value)
    return math.floor(value + 0.5)
end

local pi_div_180 = math.pi / 180
function math.angle2radian(angle)
    return angle * pi_div_180
end

function math.radian2angle(radian)
    return radian * 180 / math.pi
end

function io.exists(path)
    local file = io.open(path, "r")
    if file then
        io.close(file)
        return true
    end
    return false
end

function io.readfile(path)
    local file = io.open(path, "r")
    if file then
        local content = file:read("*a")
        io.close(file)
        return content
    end
    return nil
end

function io.writefile(path, content, mode)
    mode = mode or "w+b"
    local file = io.open(path, mode)
    if file then
        if file:write(content) == nil then
            return false
        end
        io.close(file)
        return true
    else
        return false
    end
end

function io.pathinfo(path)
    local pos = string.len(path)
    local extpos = pos + 1
    while pos > 0 do
        local b = string.byte(path, pos)
        if b == 46 then
            -- 46 = char "."
            extpos = pos
        elseif b == 47 then
            -- 47 = char "/"
            break
        end
        pos = pos - 1
    end

    local dirname = string.sub(path, 1, pos)
    local filename = string.sub(path, pos + 1)
    extpos = extpos - pos
    local basename = string.sub(filename, 1, extpos - 1)
    local extname = string.sub(filename, extpos)
    return {
        dirname = dirname,
        filename = filename,
        basename = basename,
        extname = extname
    }
end

function io.filesize(path)
    local size = false
    local file = io.open(path, "r")
    if file then
        local current = file:seek()
        size = file:seek("end")
        file:seek("set", current)
        io.close(file)
    end
    return size
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

local function charSize(ch)
    if not ch then
        return 0
    elseif ch >= 252 then
        return 6
    elseif ch >= 248 and ch < 252 then
        return 5
    elseif ch >= 240 and ch < 248 then
        return 4
    elseif ch >= 224 and ch < 240 then
        return 3
    elseif ch >= 192 and ch < 224 then
        return 2
    elseif ch < 192 then
        return 1
    end
end

function utf8len(str)
    local len = 0
    local aNum = 0 --字母个数
    local hNum = 0 --汉字个数
    local currentIndex = 1
    while currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        local cs = charSize(char)
        currentIndex = currentIndex + cs
        len = len + 1
        if cs == 1 then
            aNum = aNum + 1
        elseif cs >= 2 then
            hNum = hNum + 1
        end
    end
    return len, aNum, hNum
end

function utf8sub(str, startChar, numChars)
    local startIndex = 1
    while startChar > 1 do
        local char = string.byte(str, startIndex)
        startIndex = startIndex + charSize(char)
        startChar = startChar - 1
    end

    local currentIndex = startIndex

    while numChars > 0 and currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + charSize(char)
        numChars = numChars - 1
    end
    return str:sub(startIndex, currentIndex - 1)
end