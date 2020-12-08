---@class LuaBabelMgr:通天塔管理
local LuaBabelMgr = {}

--region 数据
LuaBabelMgr.OpenLimit = 0
--endregion

--region 服务器消息接收

---返回通天塔的基础数据
---@param tblData babel.ResBabelData lua table类型消息数据
function LuaBabelMgr:ResBabelDataMessage(tblData)
    self.OpenLimit = tblData.OpenLimit;
end

---返回指定层的通天塔数据
---@param tblData babel.ResBabelLevelData lua table类型消息数据
function LuaBabelMgr:ResBabelLevelDataMessage(tblData)

end

--endregion

return LuaBabelMgr