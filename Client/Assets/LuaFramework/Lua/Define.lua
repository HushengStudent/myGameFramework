---
--- Created by husheng.
--- DateTime: 2018/1/7 18:49
--- 定义引用:c# to lua
---

---[Unity]
WWW = UnityEngine.WWW
GameObject = UnityEngine.GameObject

---[Mgr]
luaMgr = Framework.LuaMgr.Instance --luaMgr
sceneMgr = Framework.SceneMgr.Instance
resourceMgr = Framework.ResourceMgr.Instance

---[Utility]
luaUtility = Framework.LuaUtility.Instance --luaUtility
logHelper = LogHelper
luaBuffer = Framework.LuaBuffer
LuaNetUtility = Framework.LuaNetUtility

---[Enum]
logColor = LogColor

---[Function]

---log print
function log(msg)
    logHelper.Print(msg)
end

function logWarn(msg)
    logHelper.PrintWarning(msg)
end

function logError(msg)
    logHelper.PrintError(msg)
end

function logGreen(msg)
    logHelper.Print(msg,logColor.Green)
end

function logYellow(msg)
    logHelper.Print(msg,logColor.Yellow)
end