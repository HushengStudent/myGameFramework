-- -
---Auto generated.
---Coding to do what u want to do.
---

local super = import("UI.BaseCtrl")

local LoginCtrl = class("LoginCtrl", super)

--这里这样写主要是实现函数重载,比如BaseCtrl与LoginCtrl都有函数Display(),
--BaseCtrl.ctor()函数中调用函数Display(),则使用super.ctor(self)调用的话,
--BaseCtrl.ctor()调用的就是LoginCtrl中的Display()而不是BaseCtrl中的Display(),
--使用super:ctor()调用的话,BaseCtrl.ctor()调用的就是BaseCtrl中的Display(),重载错误!
function LoginCtrl:ctor()
    super.ctor(self)
    self:ctorEx()
end

function LoginCtrl:onInit(...)
    super.onInit(self, ...)
    local layout = import(".LoginLayout").new()
    self.layout = layout:BindLuaCom(self.go)
    self:onInitEx(...)
end

function LoginCtrl:onRefresh(...)
    super.onRefresh(self, ...)
    self:onRefreshEx(...)
end

function LoginCtrl:onUpdate(interval)
    super.onUpdate(self, interval)
    self:onUpdateEx(interval)
end

function LoginCtrl:onHide(...)
    super.onHide(self, ...)
    self:onHideEx(...)
end

function LoginCtrl:onResume(...)
    super.onResume(self, ...)
    self:onResumeEx(...)
end

function LoginCtrl:onUnInit(...)
    super.onUnInit(self, ...)
    self:onUnInitEx(...)
end

-----------------------------///beautiful line///-----------------------------



function LoginCtrl:ctorEx()

end

function LoginCtrl:onInitEx(...)
    --Test Net
    local login = login_pb.LoginRequest()
    login.id = 2000
    login.name = 'husheng'
    local msg = login:SerializeToString()
    ----------------------------------------------------------------
    local buffer = luaBuff.New()
    --buffer:WriteShort(10011)
    --buffer:WriteBuffer(msg)
    --networkMgr:SendMessage(buffer)
end

function LoginCtrl:onRefreshEx(...)

end

function LoginCtrl:onUpdateEx(interval)

end

function LoginCtrl:onHideEx()

end

function LoginCtrl:onResumeEx()

end

function LoginCtrl:onUnInitEx()

end

return LoginCtrl
