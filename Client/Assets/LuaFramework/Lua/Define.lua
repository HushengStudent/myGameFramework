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
logUtility = LogUtil.LogUtility
luaBuffer = Framework.LuaBuffer
luaNetUtil = Framework.LuaNetUtil

---[Enum]
logColor = LogUtil.LogColor

---[Function]

---log print
function log(msg)
    logUtility.Print(msg)
end

function logWarn(msg)
    logUtility.PrintWarning(msg)
end

function logError(msg)
    logUtility.PrintError(msg)
end

function logGreen(msg)
    logUtility.Print(msg,logColor.Green)
end

function logYellow(msg)
    logUtility.Print(msg,logColor.Yellow)
end