---
--- ui mgr
---

local UIMgr = class("UIMgr")

function UIMgr:ctor()
    self.stateEnum = import("UI.StateEnum")
    self.ctrlEnum = import("UI.CtrlEnum")
    self._allUIState = {}
    self._allUIState[self.stateEnum.LoginState] = import("UI.State.LoginState")

    self._curState = self.stateEnum.Non
end

function UIMgr:startUI()
    self:OnEnterState(self.stateEnum.LoginState)
end

function UIMgr:OnEnterState(stateEnum)
    self._allUIState[stateEnum]:onEnterState()
end

function UIMgr:onLeaveState(stateEnum)
    self._allUIState[stateEnum]:onExitState()
end

function UIMgr:showPanel(name, ...)

end

function UIMgr:refreshPanel(name, ...)

end

function UIMgr:hidePanel(name, ...)

end

function UIMgr:resumePanel(name, ...)

end

function UIMgr:closePanel(name, ...)

end

return UIMgr
