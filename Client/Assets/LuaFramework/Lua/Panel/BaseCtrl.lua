---
--- Created by husheng.
--- DateTime: 2018/1/21 1:10
---

require "Panel.CtrlEnum"
require "Panel.Controller.LoginCtrl"

BaseCtrl = class("BaseCtrl")

local m_allCtrls = {} --全部ctrl
local m_curCtrls = {} --当前打开ctrl

function BaseCtrl:ctor()
    m_allCtrls[CtrlEnum.LoginCtrl] = LoginCtrl.new()

    --注册Ctrl...
end

function BaseCtrl:GetCtrl(ctrlName)

end