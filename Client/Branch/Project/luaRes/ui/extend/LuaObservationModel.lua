---@class LuaObservationModel 模型展示
local LuaObservationModel = {}

---模型的挂载点
---当初始化调用这个展示组件的时候,会将这个脚本挂载上去,在销毁的时候可以调用ondestroy
---但是由于可能会不断的进行模型替换,不能直接把脚本挂载模型上,所以在模型的上层再放一个节点
LuaObservationModel.RootPoint = nil

---@param parent CS.UnityEngine.GameObject 模型的挂载点
function LuaObservationModel:Init(trans)
    if (trans == nil or CS.StaticUtility.IsNull(trans)) then
        return nil
    end

    if(LuaObservationModel.RootPoint == nil) then
        LuaObservationModel.RootPoint = CS.UnityEngine.GameObject("LuaObservationModel")
    end

    CS.CSAssist.SetParent(trans, self.RootPoint, true);

    local luaBehavi = CS.Utility_Lua.GetComponent(go, "LuaBehaviour")

    if (luaBehavi == nil or CS.StaticUtility.IsNull(luaBehavi)) then
        luaBehavi = trans.gameObject:AddComponent(typeof(CS.LuaBehaviour))
    end
    luaBehavi:InitWithFilePath('ui/extend/LuaObservationModel.lua', "LuaObservationModel")

    --luaBehavi.luaOnDestroy = function()
    --    self:Destroy()
    --end
end

---创建模型
---@param name string 模型的名字
function LuaObservationModel:CreateModel(name, parent)

end

---当模型被销毁的时候
function LuaObservationModel:Destroy()
    print("LuaObservationModel销毁")
end

return LuaObservationModel