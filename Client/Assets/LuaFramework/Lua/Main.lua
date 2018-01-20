---
--- 游戏初始化入口
---

--全局变量:g_Xxx;成员变量:m_Xxx;局部变量:l_Xxx;

--Common
require "Define"
require "Common.Class"
require "Common.Functions"
--Manager
require "Manager.GameManager"
require "Manager.UIManager"
require "Manager.SceneManager"

require "UI.Canvas.BaseCanvas"
require "Panel.BaseCtrl"


---[luaMgr]
g_GameMgr = GameManager.new()
g_UIMgr = UIManager.new()
g_SceneMgr = SceneManager.new()

--主入口函数。从这里开始lua逻辑
function Main()					
	log("main logic start")
	g_GameMgr:StartGame()
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end









