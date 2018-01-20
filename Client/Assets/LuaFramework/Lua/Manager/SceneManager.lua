---
--- Created by husheng.
--- DateTime: 2018/1/17 3:14
--- 管理模块ui
---

SceneManager = class("SceneManager")

local m_curCanvas

function SceneManager:ctor() end

function SceneManager:GetManagerName()
    log("SceneManager")
end

function SceneManager:OnEnterScene(id)
    --TODO:根据场景id获取canvas id，并赋值m_curCanvas
    g_UIMgr:OnEnterCanvas(m_curCanvas)
end

function SceneManager:OnLeaveScene()
    g_UIMgr:OnLeaveCanvas(m_curCanvas)
    m_curCanvas = CanvasEnum.Non
end