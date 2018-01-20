---
--- Created by husheng.
--- DateTime: 2018/1/17 3:11
--- 管理场景scene ui
---

UIManager = class("UIManager")

local l_baseCanvas
local l_baseCtrl

function UIManager:ctor()
    l_baseCanvas = BaseCanvas.new()
    l_baseCtrl = BaseCtrl.new()
end

function UIManager:GetManagerName()
    log("UIManager")
end

function UIManager:OnEnterCanvas(canvasEnum)
    l_baseCanvas:EnterCanvas(canvasEnum)
end

function UIManager:OnLeaveCanvas(canvasEnum)
    l_baseCanvas:LeaveCanvas(canvasEnum)
end

function UIManager:OnCreateCtrl(ctrlEnum)

end

function UIManager:OnDestroyCtrl(ctrlEnum)

end

function UIManager:OnActiveCtrl(ctrlEnum)

end

function UIManager:OnDeActiveCtrl(ctrlEnum)

end