---游戏导航按钮
---@class UIGameNavPanel:UIBase
local UIGameNavPanel = {}

--region 组件获取
---设置按钮  点击打开设置面板
function UIGameNavPanel.GetBtn_Setting()
    if UIGameNavPanel.mBtn_Setting == nil then
        UIGameNavPanel.mBtn_Setting = UIGameNavPanel:GetCurComp("Widget/BtnSetting", "Button")
    end
    return UIGameNavPanel.mBtn_Setting
end

---设置按钮  点击打开设置面板
function UIGameNavPanel.GetBtn_Fight()
    if UIGameNavPanel.mBtnFight == nil then
        UIGameNavPanel.mBtnFight = UIGameNavPanel:GetCurComp("Widget/BtnFight", "Button")
    end
    return UIGameNavPanel.mBtnFight
end


--endregion

function UIGameNavPanel:Init()
    self:InitUIEvent()
end

function UIGameNavPanel:Show()
end

--region UI事件
function UIGameNavPanel:InitUIEvent()
    if (self:GetBtn_Setting()) then
        self:GetBtn_Setting().onClick:AddListener(UIGameNavPanel.OnClickBtn_Setting);
    end

    if (self:GetBtn_Fight()) then
        self:GetBtn_Fight().onClick:AddListener(UIGameNavPanel.OnClickBtn_Fight);
    end
end

function UIGameNavPanel.OnClickBtn_Fight()
    uimanager:ClosePanel("UIMainMenusPanel")
    uimanager:CreatePanel("UIMainChapterPanel")
end

function UIGameNavPanel.OnClickBtn_Setting()
    uimanager:CreatePanel("UISettingPanel")
end

--endregion

function ondestroy()

end

return UIGameNavPanel