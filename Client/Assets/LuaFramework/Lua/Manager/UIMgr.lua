---
--- ui mgr
---

local UIMgr = class("UIMgr")

function UIMgr:ctor()
    self:_initUIState()
end

function UIMgr:_onInitialize()
    self.stateEnum = import("UI.StateEnum")
    self.ctrlEnum = import("UI.CtrlEnum")
    self._allUIState = {}
    self._allUIState[self.stateEnum.LoginState] = import("UI.State.LoginState")
end

function UIMgr:StartUI()
    self:OnEnterState(self.stateEnum.LoginState)
end

function UIMgr:OnEnterState(stateEnum)
    self._allUIState[stateEnum]:EnterCanvas()
end

function UIMgr:OnLeaveState(stateEnum)
    self._allUIState[stateEnum]:LeaveCanvas()
end

function UIMgr:OnCreateCtrl(ctrlEnum)

end

function UIMgr:OnDestroyCtrl(ctrlEnum)

end

function UIMgr:OnActiveCtrl(ctrlEnum)

end

function UIMgr:OnDeActiveCtrl(ctrlEnum)

end

return UIMgr
