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
require "Manager.NetManager"

require "UI.Canvas.BaseCanvas"
require "Panel.BaseCtrl"

require "Protol.login_pb"
require "Protol.ProtoDefine"

---[luaMgr]
g_GameMgr = GameManager.new()
g_UIMgr = UIManager.new()
g_SceneMgr = SceneManager.new()
g_NetMgr = NetManager.new()

--主入口函数。从这里开始lua逻辑
function Main()					
	log("main logic start")
	--g_GameMgr:StartGame()
	--test_pblua_func()
	--TestSendPblua()
	luaNetUtil.Test()
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end


---=====================================================================================================================
--测试pblua--
function test_pblua_func()
	local login = Protol.login_pb.LoginRequest()
	login.id = 2000
	login.name = 'game'
	login.email = 'jarjin@163.com'

	local msg = login:SerializeToString()
	luaUtility.OnCallLuaFunc(msg, OnPbluaCall)
end

--pblua callback--
function OnPbluaCall(data)
	local msg = Protol.login_pb.LoginRequest()
	msg:ParseFromString(data)
	print(msg)
	print(msg.id..' '..msg.name)
end

---=====================================================================================================================
--测试发送PBLUA--
function TestSendPblua()
	local login = Protol.login_pb.LoginRequest()
	login.id = 2000
	login.name = 'game'
	login.email = 'jarjin@163.com'

	local msg = login:SerializeToString()
	----------------------------------------------------------------
	local buffer = luaBuffer.New()
	buffer:WriteBuffer(msg)
	luaNetUtil.SendLuaReq(10011,buffer)
end

function TestProcessPblua(buffer)
	local login = Protol.login_pb.LoginResponse()
	local data = buffer:ReadBuffer()
	login:ParseFromString(data)
	----------------------------------------------------------------
	logUtility.PrintError("lua:"..tostring(msg.id))
	logUtility.PrintError("lua:"..tostring(msg.name))
	logUtility.PrintError("lua:"..tostring(msg.email))
end

function Receive(id,buffer)
	Protol.ProtoProcess.Process(id,buffer)
end