---
--- 游戏初始化入口
---

require "Define"
require "Common.Class"
require "Common.Functions"
require "Manager.GameManager"
require "Manager.UIManager"
require "Manager.SceneManager"

---[luaMgr]
GameMgr = GameManager.new()
UIMgr = UIManager.new()
SceneMgr = SceneManager.new()

--主入口函数。从这里开始lua逻辑
function Main()					
	log("logic start")
	GameMgr:GetClassName()
	UIMgr:GetClassName()
	SceneMgr:GetClassName()
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end









