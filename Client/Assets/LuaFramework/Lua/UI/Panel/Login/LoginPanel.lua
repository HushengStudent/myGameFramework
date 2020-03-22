---
---Auto generated@2020/3/22 19:56:33.
---Coding to do what u want to do.
---

local super = import("UI.BasePanel")

local LoginPanel = class("LoginPanel", super)

--这里这样写主要是实现函数重载,比如BaseCtrl与LoginCtrl都有函数Display(),
--BaseCtrl.ctor()函数中调用函数Display(),则使用super.ctor(self)调用的话,
--BaseCtrl.ctor()调用的就是LoginCtrl中的Display()而不是BaseCtrl中的Display(),
--使用super:ctor()调用的话,BaseCtrl.ctor()调用的就是BaseCtrl中的Display(),重载错误!
function LoginPanel:ctor()
    super.ctor(self)
    self:ctorEx()
end

function LoginPanel:onInit(...)
    super.onInit(self, ...)
    local layout = import(".LoginLayout").new()
    self.layout = layout:BindLuaCom(self.go)
    self:onInitEx(...)
end

function LoginPanel:onRefresh(...)
    super.onRefresh(self, ...)
    self:onRefreshEx(...)
end

function LoginPanel:onUpdate(interval)
    super.onUpdate(self, interval)
    self:onUpdateEx(interval)
end

function LoginPanel:onHide(...)
    super.onHide(self, ...)
    self:onHideEx(...)
end

function LoginPanel:onResume(...)
    super.onResume(self, ...)
    self:onResumeEx(...)
end

function LoginPanel:onUnInit(...)
    super.onUnInit(self, ...)
    self:onUnInitEx(...)
end

-----------------------------///beautiful line///-----------------------------



function LoginPanel:ctorEx()

end

function LoginPanel:onInitEx(...)
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

function LoginPanel:onRefreshEx(...)

end

function LoginPanel:onUpdateEx(interval)

end

function LoginPanel:onHideEx()

end

function LoginPanel:onResumeEx()

end

function LoginPanel:onUnInitEx()

end

return LoginPanel
