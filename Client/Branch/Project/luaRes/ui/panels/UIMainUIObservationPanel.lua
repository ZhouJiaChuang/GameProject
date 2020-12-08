---@class UIMainUIObservationPanel 主界面UI展示面板  这个面板会展示一个模型 以及模型的互动 外带一些天气之类的
local UIMainUIObservationPanel = {}

--region 组件获取
function UIMainUIObservationPanel:GetObservantRoleRoot()
    if self.mObservantRole == nil then
        self.mObservantRole = UIMainUIObservationPanel:GetCurComp("Widget/ObservantRole", "Transform")
    end
    return self.mObservantRole
end
--endregion

UIMainUIObservationPanel.ObservationModel = nil

function UIMainUIObservationPanel:Init()
    if(self.ObservationModel == nil) then
        self.ObservationModel = luaClass.LuaObservationModel:New()
    end
    self.ObservationModel:Init(self:GetObservantRoleRoot())
end

function UIMainUIObservationPanel:Show()
end


--region UI事件
--function UIMainUIObservationPanel:InitUIEvent()
--    if (self:GetBtn_Setting()) then
--        self:GetBtn_Setting().onClick:AddListener(UIGameNavPanel.OnClickBtn_Setting);
--    end
--end
--endregion

function ondestroy()

end

return UIMainUIObservationPanel