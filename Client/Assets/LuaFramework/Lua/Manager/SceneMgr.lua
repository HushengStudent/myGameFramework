---
--- 管理模块ui
---

local SceneMgr = class("SceneMgr")

local m_curCanvas

function SceneMgr:ctor()

end

function SceneMgr:update()

end

function SceneMgr:OnEnterScene(id)
    --TODO:根据场景id获取canvas id，并赋值m_curCanvas
    g_UIMgr:OnEnterCanvas(m_curCanvas)
end

function SceneMgr:OnLeaveScene()
    g_UIMgr:OnLeaveCanvas(m_curCanvas)
    m_curCanvas = CanvasEnum.Non
end

return SceneMgr
