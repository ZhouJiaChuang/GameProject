---@class LuaCityMgr 城市管理
local LuaCityMgr = {}

--region 数据
---@type table<LuaCityBuildItem>
LuaCityMgr.AllCityBuildData = {}
--endregion

--region 服务器消息返回

---返回城市的所有数据
---@param tblData city.ResCityData lua table类型消息数据
function LuaCityMgr:ResCityDataMessage(tblData)

end

---返回开启地面区域信息
---@param tblData city.ResTerrainAreaInfo lua table类型消息数据
function LuaCityMgr:ResTerrainAreaInfoMessage(tblData)

end

---返回创建建筑信息
---@param tblData city.ResBuildInfo lua table类型消息数据
function LuaCityMgr:ResBuildInfoMessage(tblData)

end

--endregion

--region 关于建筑的处理

---添加或者更新建筑数据
---@param tblData city.ResBuildInfo
function LuaCityMgr:AddOrUpdateBuildInfo(tblData)
    if(self.AllCityBuildData[tblData.id] == nil) then
        self.AllCityBuildData[tblData.id] = luaClass.LuaCityBuildItem:New()
    end
    ---@type LuaCityBuildItem
    local item = self.AllCityBuildData[tblData.id]
    item:UpdateInfo(tblData)
end

---移除建筑
function LuaCityMgr:RemoveBuild(id)
    self.AllCityBuildData[id] = nil
end

---得到所有建筑信息
function LuaCityMgr:GetAllBuildInfo()
    return self.AllCityBuildData;
end
--endregion

return LuaCityMgr