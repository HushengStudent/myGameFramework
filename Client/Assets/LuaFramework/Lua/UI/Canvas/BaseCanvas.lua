---
--- Created by husheng.
--- DateTime: 2018/1/21 0:33
---
require "UI.CanvasEnum"
require "UI.Canvas.LoginCanvas"

BaseCanvas = class("BaseCanvas") -- 场景ui 基类

local m_curCanvas --当前场景ui

local m_allCanvas ={} --全部场景ui

function BaseCanvas:ctor()
    m_curCanvas = CanvasEnum.Non
    m_allCanvas[CanvasEnum.LoginCanvas] = LoginCanvas.new()

    --注册Canvas...
end

function BaseCanvas:EnterCanvas(canvasEnum)
    log("EnterCanvas "..tostring(canvasEnum))
    m_curCanvas = canvasEnum
    m_allCanvas[m_curCanvas]:CreateCanvas()
end

function BaseCanvas:LeaveCanvas(canvasEnum)
    log("LeaveCanvas "..tostring(canvasEnum))
    m_allCanvas[m_curCanvas]:DestroyCanvas()
    m_curCanvas = CanvasEnum.Non
end