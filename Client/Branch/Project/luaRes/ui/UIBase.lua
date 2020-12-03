---@class UIBase
local UIBase = {}

UIBase.__index = UIBase

---@type UnityEngine.GameObject UI界面对应的根游戏物体
UIBase.go = nil

---界面层级类型
---@type UILayerType
UIBase.PanelLayerType = CS.EUILayerType.WindowsPlane

---获取界面层级类型
---@return UILayerType
function UIBase:Get_PanelLayerType()
    return self.PanelLayerType
end

---获取组件
---@param path string 组件的相对路径
---@param type string 组建的C#类型
---@return UnityEngine.Component
function UIBase:GetCurComp(path, type)
    print("aaaaaaaaaaaa")
    if self.go and CS.StaticUtility.IsNull(self.go) == false then
        return self:GetComp(self.go.transform, path, type)
    else
        return nil
    end
end

---获取组件
---@param trans UnityEngine.Transform 组件的Transform根节点
---@param path string 组件的相对路径
---@param type string 组建的C#类型
---@return UnityEngine.Component
function UIBase:GetComp(trans, path, type)
    if (trans == nil) then
        return nil
    end
    trans = trans:Find(path)
    if (trans == nil) then
        return nil;
    end
    if (type == 'GameObject') then
        return trans.gameObject
    elseif (type == 'Transform') then
        return trans
    end
    return CS.Utility_Lua.GetComponent(trans, type)
end

---获取组件
---@param path string 组件的相对路径
---@param type string 组建的C#类型
---@return UnityEngine.Component
function UIBase:GetCurComp(path, type)
    if self.go and CS.StaticUtility.IsNull(self.go) == false then
        return self:GetComp(self.go.transform, path, type)
    else
        return nil
    end
end

return UIBase