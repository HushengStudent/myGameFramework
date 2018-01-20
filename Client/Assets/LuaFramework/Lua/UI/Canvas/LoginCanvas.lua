---
--- Created by husheng.
--- DateTime: 2018/1/21 0:42
---

LoginCanvas = class("LoginCanvas",BaseCanvas)

function LoginCanvas:ctor()

end

function LoginCanvas:CreateCanvas()
    g_UIMgr:OnCreateCtrl(CtrlEnum.LoginCtrl)
end

function LoginCanvas:DestroyCanvas()
    g_UIMgr:OnDeActiveCtrl(CtrlEnum.LoginCtrl)
end