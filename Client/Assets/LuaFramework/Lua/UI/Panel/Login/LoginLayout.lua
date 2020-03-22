---
---Auto generated from prefab:Login.prefab@2020/3/22 17:06:03.
---Don't coding.
---

local LoginLayout = {}

function LoginLayout:BindLuaCom(go)

    local luaUIPanel = go:GetComponent("LuaUIPanel")
    self.layout = {}
    self.layout._GameObjectBtn = luaUIPanel.luaUIComArray[0]
    self.layout._GameObject2Com = luaUIPanel.luaUIComArray[1]

    return self.layout
end

return LoginLayout

