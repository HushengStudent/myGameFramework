---
---This code was generated by a tool.
---To coding to do what u want to do.
---
 
require "Panel.View.LoginPanel"
require "Panel.Data.LoginData"
 
LoginCtrl = class("LoginCtrl",BaseCtrl)
 
function LoginCtrl:ctor()
 
end
 
function LoginCtrl:Awake(msg)
       log("--->>>LoginCtrl Awake be called.")
       local l_panel = LoginPanel.new()
       self.panel = l_panel:BindLuaComponent(msg[0])
end
 
function LoginCtrl:Start()
       log("--->>>LoginCtrl Start be called.")
 
end
 
function LoginCtrl:OnEnable()
       log("--->>>LoginCtrl OnEnable be called.")
 
end
 
function LoginCtrl:OnDisable()
       log("--->>>LoginCtrl OnDisable be called.")
 
end
 
function LoginCtrl:OnDestroy()
       log("--->>>LoginCtrl OnDestroy be called.")
 
end
-----------------------------超华丽的分割线-----------------------------