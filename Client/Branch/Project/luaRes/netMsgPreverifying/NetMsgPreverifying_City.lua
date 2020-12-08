--[[本文件为工具自动生成]]
--[[本文件用于向服务器发送消息前,对发送的消息进行预校验,返回的bool值决定该消息是否应当发送,可编辑区域为所生成的每个方法内部,对可编辑区域外的修改将在工具下次修改时作废]]
--[[不建议在方法内使用--region和--endregion,以免干扰工具读取]]
--city.xml

--region ID:10002 ReqOpenTerrinAreaInfoMessage 请求开启地面区域
---@param msgID LuaEnumNetDef 消息ID
---@param csData city.ReqOpenTerrinAreaInfo lua类型消息数据
---@return boolean 是否允许发送消息
netMsgPreverifying[10002] = function(msgID, csData)
    --在此处填入预校验代码
    return true
end
--endregion

--region ID:10004 ReqCreateBuildMessage 请求创建建筑
---@param msgID LuaEnumNetDef 消息ID
---@param csData city.ReqCreateBuild lua类型消息数据
---@return boolean 是否允许发送消息
netMsgPreverifying[10004] = function(msgID, csData)
    --在此处填入预校验代码
    return true
end
--endregion

--region ID:10006 ReqPutBuildMessage 请求摆放建筑
---@param msgID LuaEnumNetDef 消息ID
---@param csData city.ReqPutBuild lua类型消息数据
---@return boolean 是否允许发送消息
netMsgPreverifying[10006] = function(msgID, csData)
    --在此处填入预校验代码
    return true
end
--endregion

--region ID:10007 ReqRecycleBuildMessage 请求回收建筑
---@param msgID LuaEnumNetDef 消息ID
---@param csData city.ReqRecycleBuild lua类型消息数据
---@return boolean 是否允许发送消息
netMsgPreverifying[10007] = function(msgID, csData)
    --在此处填入预校验代码
    return true
end
--endregion

--region ID:10008 ReqBuildLevelUpMessage 请求建造等级提升
---@param msgID LuaEnumNetDef 消息ID
---@param csData city.ReqBuildLevelUp lua类型消息数据
---@return boolean 是否允许发送消息
netMsgPreverifying[10008] = function(msgID, csData)
    --在此处填入预校验代码
    return true
end
--endregion

--region ID:10009 ReqAccelerateBuildLevelMessage 请求加速建造等级
---@param msgID LuaEnumNetDef 消息ID
---@param csData city.ReqAccelerateBuildLevel lua类型消息数据
---@return boolean 是否允许发送消息
netMsgPreverifying[10009] = function(msgID, csData)
    --在此处填入预校验代码
    return true
end
--endregion

--region ID:10010 ReqFinishBuildLevelMessage 请求立即完成建造等级
---@param msgID LuaEnumNetDef 消息ID
---@param csData city.ReqFinishBuildLevel lua类型消息数据
---@return boolean 是否允许发送消息
netMsgPreverifying[10010] = function(msgID, csData)
    --在此处填入预校验代码
    return true
end
--endregion
