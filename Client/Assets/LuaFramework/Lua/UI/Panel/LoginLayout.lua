---
---Auto generated@2020/3/21 22:59:52.
---Don't coding.
---
 
local LoginPanel = {}
 
function LoginPanel:BindLuaCom(go)
 
    local luaUIPanel = go:GetComponent("LuaUIPanel")
    self.layout = {}
    self.panel.GameObject = luaUIPanel.luaUIComArray[0]
    self.panel.GameObject1 = luaUIPanel.luaUIComArray[1]
    self.panel.GameObject2 = luaUIPanel.luaUIComArray[2]
    self.panel.GameObject3 = luaUIPanel.luaUIComArray[3]
    self.panel.GameObject4 = luaUIPanel.luaUIComArray[4]
    self.panel.GameObject5 = luaUIPanel.luaUIComArray[5]
    self.panel.GameObject6 = luaUIPanel.luaUIComArray[6]
    self.panel.GameObject7 = luaUIPanel.luaUIComArray[7]
    self.panel.GameObject8 = luaUIPanel.luaUIComArray[8]
    self.panel.GameObject9 = luaUIPanel.luaUIComArray[9]

    return self.layout
end
 
return LoginPanel
 
