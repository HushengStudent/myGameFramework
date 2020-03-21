---
---

local super = import("UI.BaseState")

local LoginState = class("LoginState", super)

function LoginState:ctor()

end

function LoginState:onEnterState()
    g_uiMgr:showPanel(g_uiMgr.ctrlEnum.LoginCtrl)
end

function LoginState:onExitState()
    g_uiMgr:closePanel(g_uiMgr.ctrlEnum.CtrlEnum.LoginCtrl)
end

return LoginState
