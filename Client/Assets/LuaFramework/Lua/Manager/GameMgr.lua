---
--- 游戏管理器
---

local GameMgr = class("GameMgr")

function GameMgr:ctor()
end

function GameMgr:startGame()

    g_uiMgr:startUI()

end

return GameMgr
