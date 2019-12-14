---
---

module("UI", package.seeall)

require "Panel.CtrlEnum"

BaseCtrl = class("BaseCtrl")

function BaseCtrl:ctor()

end

function BaseCtrl:GetCtrl(ctrlName)

end

function BaseCtrl:OnInitialize(args)

end

function BaseCtrl:OnUpdate(interval)

end

function BaseCtrl:OnHide()

end

function BaseCtrl:OnResume()

end

function BaseCtrl:UnInitialize()

end

return BaseCtrl
