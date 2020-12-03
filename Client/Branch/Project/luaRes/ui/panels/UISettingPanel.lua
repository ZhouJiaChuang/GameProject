---设置面板
---@class UISettingPanel:UIBase
local UISettingPanel = {}

--region 组件获取
function UISettingPanel.GetBtn_Close()
    if UISettingPanel.mBtn_Close == nil then
        UISettingPanel.mBtn_Close = UISettingPanel:GetCurComp("Widget/Btn_Close", "Button")
    end
    return UISettingPanel.mBtn_Close
end
--endregion

function UISettingPanel:Init()
    self:InitUIEvent();
end

function UISettingPanel:Show()
end

--region UI事件
function UISettingPanel:InitUIEvent()
    if (self:GetBtn_Close()) then
        self:GetBtn_Close().onClick:AddListener(UISettingPanel.OnClickBtn_Close);
    end
end

function UISettingPanel.OnClickBtn_Close()
    uimanager:ClosePanel("UISettingPanel");
end
--endregion

function ondestroy()

end

return UISettingPanel