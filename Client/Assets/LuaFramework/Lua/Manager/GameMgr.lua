---
--- 游戏管理器
---

local GameMgr = class("GameMgr")

function GameMgr:ctor()
end

function GameMgr:StartGame()

    g_uiMgr:StartUI()

end

return GameMgr
