---
--- Created by husheng.
--- DateTime: 2018/1/17 3:11
--- 管理场景scene ui
---

module("Manager",package.seeall)

require "Panel.BaseCtrl"
require "UI.Canvas.BaseCanvas"

UIMgr = class("UIMgr")

local l_baseCanvas
local l_baseCtrl

function UIMgr:ctor()
    l_baseCanvas = UI.BaseCanvas.new()
    l_baseCtrl = UI.BaseCtrl.new()
end

function UIMgr:GetMgrName()
    log("UIMgr")
end

function UIMgr:OnEnterCanvas(canvasEnum)
    l_baseCanvas:EnterCanvas(canvasEnum)
end

function UIMgr:OnLeaveCanvas(canvasEnum)
    l_baseCanvas:LeaveCanvas(canvasEnum)
end

function UIMgr:OnCreateCtrl(ctrlEnum)

end

function UIMgr:OnDestroyCtrl(ctrlEnum)

end

function UIMgr:OnActiveCtrl(ctrlEnum)

end

function UIMgr:OnDeActiveCtrl(ctrlEnum)

end

return UIMgr
