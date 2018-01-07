require "Define"

--主入口函数。从这里开始lua逻辑
function Main()					
	log("logic start")
	logWarn("logic start")
	logError("logic start")
	logGreen("====>")
	logYellow("logYellow")
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end