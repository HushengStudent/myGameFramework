---
---

module("UI", package.seeall)

LoginCanvas = class("LoginCanvas", BaseCanvas)

function LoginCanvas:ctor()

end

function LoginCanvas:CreateCanvas()
    g_UIMgr:OnCreateCtrl(CtrlEnum.LoginCtrl)
end

function LoginCanvas:DestroyCanvas()
    g_UIMgr:OnDeActiveCtrl(CtrlEnum.LoginCtrl)
end

return LoginCanvas
