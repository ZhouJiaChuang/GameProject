--[[本文件为工具自动生成,禁止手动修改]]
--city.xml

--region ID:10002 请求开启地面区域
---请求开启地面区域
---msgID: 10002
---@param id number 必填参数 区域ID
---@return boolean 网络请求是否成功发送
function networkRequest.ReqOpenTerrinAreaInfo(id)
    local reqTable = {}
    reqTable.id = id
    local reqMsgData = protobufMgr.Serialize("city.ReqOpenTerrinAreaInfo" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqOpenTerrinAreaInfoMessage] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqOpenTerrinAreaInfoMessage](LuaEnumNetDef.ReqOpenTerrinAreaInfoMessage, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqOpenTerrinAreaInfoMessage, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqOpenTerrinAreaInfoMessage", 10002, "ReqOpenTerrinAreaInfo", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

--region ID:10004 请求创建建筑
---请求创建建筑
---msgID: 10004
---@param cid number 必填参数 建筑配置ID
---@param x number 必填参数
---@param y number 必填参数
---@param rotate number 必填参数 摆放旋转方向
---@return boolean 网络请求是否成功发送
function networkRequest.ReqCreateBuild(cid, x, y, rotate)
    local reqTable = {}
    reqTable.cid = cid
    reqTable.x = x
    reqTable.y = y
    reqTable.rotate = rotate
    local reqMsgData = protobufMgr.Serialize("city.ReqCreateBuild" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqCreateBuildMessage] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqCreateBuildMessage](LuaEnumNetDef.ReqCreateBuildMessage, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqCreateBuildMessage, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqCreateBuildMessage", 10004, "ReqCreateBuild", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

--region ID:10006 请求摆放建筑
---请求摆放建筑
---msgID: 10006
---@param id number 必填参数 建筑唯一ID
---@param x number 必填参数
---@param y number 必填参数
---@param rotate number 必填参数 摆放旋转方向
---@return boolean 网络请求是否成功发送
function networkRequest.ReqPutBuild(id, x, y, rotate)
    local reqTable = {}
    reqTable.id = id
    reqTable.x = x
    reqTable.y = y
    reqTable.rotate = rotate
    local reqMsgData = protobufMgr.Serialize("city.ReqPutBuild" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqPutBuildMessage] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqPutBuildMessage](LuaEnumNetDef.ReqPutBuildMessage, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqPutBuildMessage, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqPutBuildMessage", 10006, "ReqPutBuild", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

--region ID:10007 请求回收建筑
---请求回收建筑
---msgID: 10007
---@param id number 必填参数 建筑唯一ID
---@return boolean 网络请求是否成功发送
function networkRequest.ReqRecycleBuild(id)
    local reqTable = {}
    reqTable.id = id
    local reqMsgData = protobufMgr.Serialize("city.ReqRecycleBuild" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqRecycleBuildMessage] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqRecycleBuildMessage](LuaEnumNetDef.ReqRecycleBuildMessage, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqRecycleBuildMessage, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqRecycleBuildMessage", 10007, "ReqRecycleBuild", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

--region ID:10008 请求建造等级提升
---请求建造等级提升
---msgID: 10008
---@param id number 必填参数 请求升级
---@return boolean 网络请求是否成功发送
function networkRequest.ReqBuildLevelUp(id)
    local reqTable = {}
    reqTable.id = id
    local reqMsgData = protobufMgr.Serialize("city.ReqBuildLevelUp" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqBuildLevelUpMessage] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqBuildLevelUpMessage](LuaEnumNetDef.ReqBuildLevelUpMessage, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqBuildLevelUpMessage, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqBuildLevelUpMessage", 10008, "ReqBuildLevelUp", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

--region ID:10009 请求加速建造等级
---请求加速建造等级
---msgID: 10009
---@param id number 必填参数 建造ID
---@param costItemId number 必填参数 加速所消耗的道具
---@return boolean 网络请求是否成功发送
function networkRequest.ReqAccelerateBuildLevel(id, costItemId)
    local reqTable = {}
    reqTable.id = id
    reqTable.costItemId = costItemId
    local reqMsgData = protobufMgr.Serialize("city.ReqAccelerateBuildLevel" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqAccelerateBuildLevelMessage] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqAccelerateBuildLevelMessage](LuaEnumNetDef.ReqAccelerateBuildLevelMessage, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqAccelerateBuildLevelMessage, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqAccelerateBuildLevelMessage", 10009, "ReqAccelerateBuildLevel", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

--region ID:10010 请求立即完成建造等级
---请求立即完成建造等级
---msgID: 10010
---@param id number 必填参数 建造ID
---@return boolean 网络请求是否成功发送
function networkRequest.ReqFinishBuildLevel(id)
    local reqTable = {}
    reqTable.id = id
    local reqMsgData = protobufMgr.Serialize("city.ReqFinishBuildLevel" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqFinishBuildLevelMessage] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqFinishBuildLevelMessage](LuaEnumNetDef.ReqFinishBuildLevelMessage, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqFinishBuildLevelMessage, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqFinishBuildLevelMessage", 10010, "ReqFinishBuildLevel", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

