---
---
---

local BaseState = class("BaseState") -- 场景ui 基类

local _curState      --当前场景ui
local _allState = {} --全部场景ui

function BaseState:ctor()
    _curState = StateEnum.Non

    --注册State...

end

function BaseState:EnterState(stateEnum)
    log("[BaseState]EnterState:" .. tostring(stateEnum))
    _curState = stateEnum
    _allState[_curState]:CreateCanvas()
end

function BaseState:LeaveState(stateEnum)
    log("[BaseState]LeaveState:" .. tostring(stateEnum))
    _curState[_curState]:DestroyCanvas()
    _allState = StateEnum.Non
end

return BaseState
