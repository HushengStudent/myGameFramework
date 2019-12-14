---
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
luaHelper = Framework.LuaHelper

---[Enum]
logColor = LogColor

---[Function]

---log print
function log(msg)
    logHelper.Print(msg, debug.traceback())
end

function logWarn(msg)
    logHelper.PrintWarning(msg, debug.traceback())
end

function logError(msg)
    logHelper.PrintError(msg, debug.traceback())
end

function logGreen(msg)
    logHelper.Print(msg, logColor.Green, debug.traceback())
end

function logYellow(msg)
    logHelper.Print(msg, logColor.Yellow, debug.traceback())
end

--[[
3.关于module( ... , package.seeall)

        一般在一个Lua文件内以module函数开始定义一个包。module同时定义了一
个新的包的函数环境，以使在此包中定义的全局变量都在这个环境中，而非
使用包的函数的环境中。理解这一点非常关键。 “module(..., package.seeall)”
的意思是定义一个包，包的名字与定义包的文件的名字相同，并且在包的函数
环境里可以访问使用包的函数环境。
使用方式
        一般用require函数来导入一个包，要导入的包必须被置于包路径（package
path）上。包路径可以通过package.path或者环境变量来设定。一般来说，
当前工作路径总是在包路径中。

       例如 文件 a.lua

[plain] view plain copy
module (..., package.seeall)
t = {}
function f()
 --todo
end
         文件 main.lua
[plain] view plain copy
require "a"
调用脚本a中的任何全局变量或者函数必须加上a.
实现在不同的lua文件中可以用相同的名称定义变量或者函数
]]--