---
--- 游戏初始化入口
---

function _onInitialize()

    import(".Define")
    import(".Enum")
    import(".Common.Functions")
    import(".Common.Class")
    import("Protol.ProtoDefine")
    import("Protol.ProtoProcess")
    import("Protol.login_pb")

    import(".GlobalRegister")

    UpdateBeat:Add(_update)
    LateUpdateBeat:Add(_lateUpdate)
    FixedUpdateBeat:Add(_fixedUpdate)

    RegisterGlobal("g_helper", import(".Common.Helper"))

    RegisterGlobal("g_gameMgr", import(".Manager.GameMgr").new())
    RegisterGlobal("g_uiMgr", import(".Manager.UIMgr").new())
    RegisterGlobal("g_sceneMgr", import(".Manager.SceneMgr").new())
    RegisterGlobal("g_netMgr", import(".Manager.NetMgr").new())

    --TODO
    RegisterGlobal("g_hotupdat", import(".hotupdatemain"))
end

--主入口函数。从这里开始lua逻辑
function Main()
    _onInitialize()
    log("main logic start")
    --test_pblua_func()
    --TestSendPblua()
    g_gameMgr:startGame()
end

--场景切换通知
function OnLevelWasLoaded(level)
    collectgarbage("collect")
    Time.timeSinceLevelLoad = 0
end

function _update()
    g_gameMgr:update()
    g_uiMgr:update()
    g_sceneMgr:update()
    g_netMgr:update()
end

function _lateUpdate()

end

function _fixedUpdate()

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
    print(msg.id .. ' ' .. msg.name)
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
    luaNetHelper.SendLuaReq(10011, buffer)
end

function TestProcessPblua(buffer)
    local msg = Protol.login_pb.LoginResponse()
    msg:ParseFromString(buffer)
    ----------------------------------------------------------------
    logGreen("+++++>>>>>Lua process protocol!" .. tostring(msg.id), LogUtil.LogColor.Green)
    logGreen("+++++>>>>>id:" .. tostring(msg.id), LogUtil.LogColor.Green)
end

function Receive(id, buffer)
    Protol.ProtoProcess.Process(id, buffer)
end
