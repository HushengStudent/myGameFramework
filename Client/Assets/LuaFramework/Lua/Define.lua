---
--- 定义引用:c# to lua
---

---[Unity]
WWW = UnityEngine.WWW
GameObject = UnityEngine.GameObject

---[Mgr]
luaMgr = Framework.LuaMgr.singleton --luaMgr
sceneMgr = Framework.SceneMgr.singleton
resourceMgr = Framework.ResourceMgr.singleton

---[Utility]
luaUtility = Framework.LuaUtility.singleton --luaUtility
logHelper = LogHelper
luaBuffer = Framework.LuaBuffer
LuaNetHelper = Framework.LuaNetHelper
luaHelper = Framework.LuaHelper

---[Enum]
--logColor = LogColor

---[Function]

---log print
function log(msg, ...)
    logHelper.Print(string.format(tostring(msg), ...), debug.traceback())
end

function logWarn(msg, ...)
    logHelper.PrintWarning(string.format(tostring(msg), ...), debug.traceback())
end

function logError(msg, ...)
    logHelper.PrintError(string.format(tostring(msg), ...), debug.traceback())
end

function logGreen(msg, ...)
    logHelper.PrintGreen(string.format(tostring(msg), ...), debug.traceback())
end

function logYellow(msg, ...)
    logHelper.PrintYellow(string.format(tostring(msg), ...), debug.traceback())
end

--[[


1.关于module( ... , package.seeall)

一般在一个Lua文件内以module函数开始定义一个包。module同时定义了一个新的包的函数环境，
以使在此包中定义的全局变量都在这个环境中，而非使用包的函数的环境中。
理解这一点非常关键。 “module(..., package.seeall)”的意思是定义一个包，
包的名字与定义包的文件的名字相同，并且在包的函数环境里可以访问使用包的函数环境。

使用方式
一般用require函数来导入一个包，要导入的包必须被置于包路径（package path）上。
包路径可以通过package.path或者环境变量来设定。一般来说，当前工作路径总是在包路径中。

例如 文件 a.lua
module (..., package.seeall)
t = {}
function f()

end

文件 main.lua
require "a"

调用脚本a中的任何全局变量或者函数必须加上a.
实现在不同的lua文件中可以用相同的名称定义变量或者函数

不使用module，常见用法
local tab =...


return tab

2.lua编程中，经常遇到函数的定义和调用，有时候用点号调用，有时候用冒号调用，这里简单的说明一下原理。

girl = {money = 200}
function girl.goToMarket(girl ,someMoney)
	girl.money = girl.money - someMoney
end
girl.goToMarket(girl ,100)
print(girl.money)

可以看出，这里进行了方法的点号定义和点号调用。

boy = {money = 200}
function boy:goToMarket(someMoney)
	self.money = self.money - someMoney
end
boy:goToMarket(100)
print(boy.money)

这里进行了冒号定义和冒号调用。
以上的打印结果都是100。

可以看出，冒号定义和冒号调用其实跟上面的效果一样，只是把第一个隐藏参数省略了，而该参数self指向调用者自身
当然了，我们也可以点号定义冒号调用，或者冒号定义点号调用
如:

boy = {money = 200}
function boy.goToMarket(self ,someMoney)
	self.money = self.money - someMoney
end
boy:goToMarket(100)
print(boy.money)

总结:冒号只是起了省略第一个参数self的作用，该self指向调用者本身，并没有其他特殊的地方。


]]--