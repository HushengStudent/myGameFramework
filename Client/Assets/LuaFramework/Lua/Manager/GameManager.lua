---
--- Created by husheng.
--- DateTime: 2018/1/8 23:49
--- 游戏管理器
---

GameManager = class("GameManager")

function GameManager:ctor() end

function GameManager:GetManagerName()
    log("GameManager")
end

function GameManager:StartGame()
    g_UIMgr:OnEnterCanvas(CanvasEnum.LoginCanvas)
end



