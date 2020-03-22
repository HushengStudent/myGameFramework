---
---Auto generated from prefab:Login.prefab@2020/3/22 18:31:07.
---Don't coding.
---

local LoginLayout = {}

function LoginLayout:BindLuaCom(go)

    local luaUIPanel = go:GetComponent("LuaUIPanel")
    self.layout = {}

    self.layout._gameObjectBtn = luaUIPanel.LuaUIComArray[0]
    self.layout._gameObjectBtn = luaUIPanel.LuaUIComArray[1]

    self.layout._loginTextTemplate = luaUIPanel.LuaUITemplateArray[0]
    self.layout._loginTextTemplate = luaUIPanel.LuaUITemplateArray[1]

    return self.layout
end

return LoginLayout

