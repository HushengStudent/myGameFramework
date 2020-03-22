---
---Auto generated from prefab:Login.prefab@2020/3/22 20:03:31.
---Don't coding.
---

local LoginPanelLayout = {}

function LoginPanelLayout:BindLuaCom(go)

    local component = go:GetComponent("LuaUIPanel")
    self.layout = {}

    self.layout._gameObjectBtn = component.LuaUIComArray[0]
    self.layout._gameObject1Btn = component.LuaUIComArray[1]

    self.layout._loginTextTemplate = component.LuaUITemplateArray[0]

    return self.layout
end

return LoginPanelLayout

