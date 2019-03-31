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
LuaNetHelper = Framework.LuaNetHelper

---[Enum]
logColor = LogColor

---[Function]

---log print
function log(msg)
    logHelper.Print(msg,debug.traceback())
end

function logWarn(msg)
    logHelper.PrintWarning(msg,debug.traceback())
end

function logError(msg)
    logHelper.PrintError(msg,debug.traceback())
end

function logGreen(msg)
    logHelper.Print(msg,logColor.Green,debug.traceback())
end

function logYellow(msg)
    logHelper.Print(msg,logColor.Yellow,debug.traceback())
end