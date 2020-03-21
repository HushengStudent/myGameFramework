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

    RegisterGlobal("g_helper", import(".Common.Helper"))

    RegisterGlobal("g_gameMgr", import(".Manager.GameMgr").new())
    RegisterGlobal("g_uiMgr", import(".Manager.UIMgr").new())
    RegisterGlobal("g_sceneMgr", import(".Manager.SceneMgr").new())
    RegisterGlobal("g_netMgr", import(".Manager.NetMgr").new())

end

--主入口函数。从这里开始lua逻辑
function Main()
    _onInitialize()
    log("main logic start")
    --test_pblua_func()
    --TestSendPblua()
    g_gameMgr:StartGame()
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
    LuaNetHelper.SendLuaReq(10011, buffer)
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

function import(moduleName, currentModuleName)
    local currentModuleNameParts
    local moduleFullName = moduleName
    local offset = 1

    while true do
        if string.byte(moduleName, offset) ~= 46 then
            -- .
            moduleFullName = string.sub(moduleName, offset)
            if currentModuleNameParts and #currentModuleNameParts > 0 then
                moduleFullName = table.concat(currentModuleNameParts, ".") .. "." .. moduleFullName
            end
            break
        end
        offset = offset + 1

        if not currentModuleNameParts then
            if not currentModuleName then
                local n, v = debug.getlocal(3, 1)
                currentModuleName = v
            end

            currentModuleNameParts = string.split(currentModuleName, ".")
        end
        table.remove(currentModuleNameParts, #currentModuleNameParts)
    end

    return require(moduleFullName)
end