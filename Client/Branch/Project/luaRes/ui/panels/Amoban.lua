---界面的模板
---@class Amoban:UIBase
local Amoban = {}

--region 组件获取
--function Amoban.GetBtn_EditorLogin()
--    if Amoban.mGetBtn_EditorLogin == nil then
--        Amoban.mGetBtn_EditorLogin = UILoginPanel:GetCurComp("Widget/Editor/Btn_EditorLogin", "Button")
--    end
--    return Amoban.mGetBtn_EditorLogin
--end
--endregion

function Amoban:Init()
end

function Amoban:Show()
end


--region UI事件
--function UIGameNavPanel:InitUIEvent()
--    if (self:GetBtn_Setting()) then
--        self:GetBtn_Setting().onClick:AddListener(UIGameNavPanel.OnClickBtn_Setting);
--    end
--end
--endregion

function ondestroy()

end

return Amoban