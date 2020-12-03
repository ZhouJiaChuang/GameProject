---登入界面
---@class UIMainMenusPanel:UIBase
local UIMainMenusPanel = {}

--region 9方向节点

function UIMainMenusPanel:GeCenterRoot()
    if (self.mCenterRoot == nil) then
        self.mCenterRoot = self:GetCurComp("Widget/CenterRoot", "Transform")
    end
    return self.mCenterRoot;
end

--region 左侧

function UIMainMenusPanel:GetLeftUpRoot()
    if (self.mLeftUpRoot == nil) then
        self.mLeftUpRoot = self:GetCurComp("Widget/LeftUpRoot", "Transform")
    end
    return self.mLeftUpRoot;
end

function UIMainMenusPanel:GetLeftCenterRoot()
    if (self.mLeftCenterRoot == nil) then
        self.mLeftCenterRoot = self:GetCurComp("Widget/LeftCenterRoot", "Transform")
    end
    return self.mLeftCenterRoot;
end

function UIMainMenusPanel:GetLeftDownRoot()
    if (self.mLeftDownRoot == nil) then
        self.mLeftDownRoot = self:GetCurComp("Widget/LeftDownRoot", "Transform")
    end
    return self.mLeftDownRoot;
end

--endregion

--region 中间

function UIMainMenusPanel:GetUpRoot()
    if (self.mUpRoot == nil) then
        self.mUpRoot = self:GetCurComp("Widget/UpRoot", "Transform")
    end
    return self.mUpRoot;
end

function UIMainMenusPanel:GetDownRoot()
    if (self.mDownRoot == nil) then
        self.mDownRoot = self:GetCurComp("Widget/DownRoot", "Transform")
    end
    return self.mDownRoot;
end

--endregion

--region 右侧

function UIMainMenusPanel:GetRightUpRoot()
    if (self.mRightUpRoot == nil) then
        self.mRightUpRoot = self:GetCurComp("Widget/RightUpRoot", "Transform")
    end
    return self.mRightUpRoot;
end

function UIMainMenusPanel:GetRightCenterRoot()
    if (self.mRightCenterRoot == nil) then
        self.mRightCenterRoot = self:GetCurComp("Widget/RightCenterRoot", "Transform")
    end
    return self.mRightCenterRoot;
end

function UIMainMenusPanel:GetRightDownRoot()
    if (self.mRightDownRoot == nil) then
        self.mRightDownRoot = self:GetCurComp("Widget/RightDownRoot", "Transform")
    end
    return self.mRightDownRoot;
end

--endregion


--endregion

UIMainMenusPanel.WaitForPanelNum = 0

function UIMainMenusPanel:Init()
    self:InitAllMenusModule();
end

function UIMainMenusPanel:Show()
end

function ondestroy()
    uimanager:ClosePanel("UIMainUIObservationPanel")
    uimanager:ClosePanel("UIRoleHeadPanel")
    uimanager:ClosePanel("UIMiniMapPanel")
    uimanager:ClosePanel("UIGameNavPanel")
end

function UIMainMenusPanel:InitAllMenusModule()
    self:AddMainMenuModule("UIMainUIObservationPanel",self:GeCenterRoot());
    self:AddMainMenuModule("UIRoleHeadPanel",self:GetLeftUpRoot());
    self:AddMainMenuModule("UIMiniMapPanel",self:GetRightUpRoot());
    self:AddMainMenuModule("UIGameNavPanel",self:GetRightUpRoot());
end

--- 增加主界面的组件插件
--- @param moduleName 组件名称
--- @param moduleName 组件节点
function UIMainMenusPanel:AddMainMenuModule(moduleName, root, callBack)
    if (root == nil) then
        root = UIMainMenusPanel.go.transform
    end
    UIMainMenusPanel.WaitForPanelNum = UIMainMenusPanel.WaitForPanelNum + 1;
    uimanager:CreatePanel(moduleName)
end

return UIMainMenusPanel