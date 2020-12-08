--[[本文件为工具自动生成]]
--[[本文件用于网络消息分发之前,根据网络消息进行全局预处理,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]
--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]
--character.xml

--region ID:12001 ResAllCharacterDataMessage 返回所有角色数据
---@param msgID LuaEnumNetDef 消息ID
---@param tblData character.ResAllCharacterData lua table类型消息数据
---@param csData character.ResAllCharacterData C# class类型消息数据(nil, 被优化掉了)
netMsgPreprocessing[12001] = function(msgID, tblData, csData)
    --在此处填入预处理代码
end
--endregion

--region ID:12002 ResCharacterDataMessage 返回单个角色数据
---@param msgID LuaEnumNetDef 消息ID
---@param tblData character.ResCharacterData lua table类型消息数据
---@param csData character.ResCharacterData C# class类型消息数据(nil, 被优化掉了)
netMsgPreprocessing[12002] = function(msgID, tblData, csData)
    --在此处填入预处理代码
end
--endregion
