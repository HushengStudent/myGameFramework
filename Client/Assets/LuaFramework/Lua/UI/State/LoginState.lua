---
---

local super = import("UI.BaseState")

local LoginState = class("LoginState", super)

function LoginState:ctor()

end

function LoginState:CreateState()
    g_UIMgr:OnCreateCtrl(CtrlEnum.LoginCtrl)
end

function LoginState:DestroyState()
    g_UIMgr:OnDeActiveCtrl(CtrlEnum.LoginCtrl)
end

return LoginState
