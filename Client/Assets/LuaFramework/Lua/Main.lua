require "Define"
require "Game"

--主入口函数。从这里开始lua逻辑
function Main()					
	log("logic start")
	Game.StartGame()
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end