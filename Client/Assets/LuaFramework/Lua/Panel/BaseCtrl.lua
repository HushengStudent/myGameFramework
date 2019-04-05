---
--- Created by husheng.
--- DateTime: 2018/1/21 1:10
---

module("UI",package.seeall)

require "Panel.CtrlEnum"

BaseCtrl = class("BaseCtrl")

local m_allCtrls = {} --全部ctrl
local m_curCtrls = {} --当前打开ctrl

local l_value = 2

function BaseCtrl:ctor()
    logGreen("--->>>BaseCtrl:ctor")
    self:PrintValue()
end

function BaseCtrl:GetCtrl(ctrlName)

end

function BaseCtrl:PrintValue()
    logGreen("--->>>value:"..tostring(l_value))
end