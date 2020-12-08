--[[本文件为工具自动生成]]
--[[本文件用于网络消息分发之前,根据网络消息进行全局预处理,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]
--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]
--city.xml

--region ID:10001 ResCityDataMessage 返回城市的所有数据
---@param msgID LuaEnumNetDef 消息ID
---@param tblData city.ResCityData lua table类型消息数据
---@param csData city.ResCityData C# class类型消息数据(nil, 被优化掉了)
netMsgPreprocessing[10001] = function(msgID, tblData, csData)
    --在此处填入预处理代码
    gameMgr:GetDataMgr():GetLuaCityMgr():ResCityDataMessage(tblData)
end
--endregion

--region ID:10003 ResTerrainAreaInfoMessage 返回开启地面区域信息
---@param msgID LuaEnumNetDef 消息ID
---@param tblData city.ResTerrainAreaInfo lua table类型消息数据
---@param csData city.ResTerrainAreaInfo C# class类型消息数据(nil, 被优化掉了)
netMsgPreprocessing[10003] = function(msgID, tblData, csData)
    --在此处填入预处理代码
    gameMgr:GetDataMgr():GetLuaCityMgr():ResTerrainAreaInfoMessage(tblData)
end
--endregion

--region ID:10005 ResBuildInfoMessage 返回创建建筑信息
---@param msgID LuaEnumNetDef 消息ID
---@param tblData city.ResBuildInfo lua table类型消息数据
---@param csData city.ResBuildInfo C# class类型消息数据(nil, 被优化掉了)
netMsgPreprocessing[10005] = function(msgID, tblData, csData)
    --在此处填入预处理代码
    gameMgr:GetDataMgr():GetLuaCityMgr():ResBuildInfoMessage(tblData)
end
--endregion
