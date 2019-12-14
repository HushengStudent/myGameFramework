---
--- 游戏管理器
---

module("Manager", package.seeall)

GameMgr = class("GameMgr")

function GameMgr:ctor()
end

function GameMgr:GetMgrName()
    log("GameMgr")
end

function GameMgr:StartGame()
    g_UIMgr:OnEnterCanvas(CanvasEnum.LoginCanvas)
end

return GameMgr
