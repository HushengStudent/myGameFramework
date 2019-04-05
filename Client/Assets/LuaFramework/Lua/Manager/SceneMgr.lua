---
--- Created by husheng.
--- DateTime: 2018/1/17 3:14
--- 管理模块ui
---

module("Manager",package.seeall)

SceneMgr = class("SceneMgr")

local m_curCanvas

function SceneMgr:ctor() end

function SceneMgr:GetMgrName()
    log("SceneMgr")
end

function SceneMgr:OnEnterScene(id)
    --TODO:根据场景id获取canvas id，并赋值m_curCanvas
    g_UIMgr:OnEnterCanvas(m_curCanvas)
end

function SceneMgr:OnLeaveScene()
    g_UIMgr:OnLeaveCanvas(m_curCanvas)
    m_curCanvas = CanvasEnum.Non
end