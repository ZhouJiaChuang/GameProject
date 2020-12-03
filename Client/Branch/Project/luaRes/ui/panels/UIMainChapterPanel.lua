---主线章节面板
---@class UIMainChapterPanel:UIBase
local UIMainChapterPanel = {}

--region 组件获取
--function UIMainChapterPanel.GetBtn_EditorLogin()
--    if UIMainChapterPanel.mGetBtn_EditorLogin == nil then
--        UIMainChapterPanel.mGetBtn_EditorLogin = UILoginPanel:GetCurComp("Widget/Editor/Btn_EditorLogin", "Button")
--    end
--    return UIMainChapterPanel.mGetBtn_EditorLogin
--end
--endregion

function UIMainChapterPanel:Init()
    print("当前测试,直接跳过章节选择,进入1-1")
    uimanager:ClosePanel("UIMainChapterPanel")
    CS.CSSceneManager.Instance:LoadByName("1-1", nil, nil);
end

function UIMainChapterPanel:Show()
end


--region UI事件
--function UIMainChapterPanel:InitUIEvent()
--    if (self:GetBtn_Setting()) then
--        self:GetBtn_Setting().onClick:AddListener(UIGameNavPanel.OnClickBtn_Setting);
--    end
--end
--endregion

function ondestroy()

end

return UIMainChapterPanel