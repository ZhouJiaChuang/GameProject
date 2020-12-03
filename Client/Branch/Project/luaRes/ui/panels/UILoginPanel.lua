---登入界面
---@class UILoginPanel:UIBase
local UILoginPanel = {}

--region 组件获取
function UILoginPanel:GetBtn_EditorLogin()
    if UILoginPanel.mGetBtn_EditorLogin == nil then
        UILoginPanel.mGetBtn_EditorLogin = UILoginPanel:GetCurComp("Widget/Editor/Btn_EditorLogin", "Button")
    end
    return UILoginPanel.mGetBtn_EditorLogin
end
--endregion

function UILoginPanel:Init()
    if (self:GetBtn_EditorLogin()) then
        self:GetBtn_EditorLogin().onClick:AddListener(UILoginPanel.OnClickBtn_EditorLogin);
    end
end

function UILoginPanel.OnClickBtn_EditorLogin()
    CS.CSSceneManager.Instance:Load(CS.ESceneType.GameScene, nil, nil, UILoginPanel.OnLoadedGameScene);
end

function UILoginPanel.OnLoadedGameScene()
    uimanager:ClosePanel("UILoginPanel")
    uimanager:CreatePanel("UIMainMenusPanel")
end

function UILoginPanel:Show()
end

function ondestroy()
end

return UILoginPanel