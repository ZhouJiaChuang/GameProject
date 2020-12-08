---@class LuaCityBuildItem:luaobject 城市建筑
local LuaCityBuildItem = {}

LuaCityBuildItem.BuildInfo = nil

---@param info city.ResBuildInfo
function LuaCityBuildItem:UpdateInfo(info)
    self.BuildInfo = info
end

---得到摆放信息
---@return number,number,number x坐标,y坐标,rotate旋转方向
function LuaCityBuildItem:GetPutInfo()
    return self.BuildInfo.x, self.BuildInfo.y, self.BuildInfo.rotate
end

return LuaCityBuildItem