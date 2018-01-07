---
--- Created by husheng.
--- DateTime: 2018/1/7 18:49
---

--[Unity]
WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;

--[Mgr]
luaMgr = Framework.LuaMgr.Instance --luaMgr

--[Utility]
luaUtility = Framework.LuaUtility.Instance --luaUtility
logUtility = Framework.LogUtility

--[Enum]
logColor = Framework.LogColor

--[Function]

--log print
function log(msg)
    logUtility.Print(msg,logColor.Green)
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