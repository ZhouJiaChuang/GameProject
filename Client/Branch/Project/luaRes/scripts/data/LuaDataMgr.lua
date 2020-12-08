---@class LuaDataMgr:数据管理类
local LuaDataMgr = {}

---得到城市数据管理类
---@return LuaCityMgr
function LuaDataMgr:GetLuaCityMgr()
    if(self.mLuaCityMgr == nil) then
        self.mLuaCityMgr = luaClass.LuaCityMgr:New()
    end
    return self.mLuaCityMgr
end

---得到通天塔数据管理类
---@return LuaBabelMgr
function LuaDataMgr:GetLuaBabelMgr()
    if(self.mBabelMgr == nil) then
        self.mBabelMgr = luaClass.LuaBabelMgr:New()
    end
    return self.mBabelMgr
end

return LuaDataMgr