--[[本文件为工具自动生成,禁止手动修改]]
--babel.xml

--region ID:11002 请求指定层的通天塔数据
---请求指定层的通天塔数据
---msgID: 11002
---@param level number 必填参数
---@return boolean 网络请求是否成功发送
function networkRequest.ReqBabelLevelData(level)
    local reqTable = {}
    reqTable.level = level
    local reqMsgData = protobufMgr.Serialize("babel.ReqBabelLevelData" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqBabelLevelDataMessage] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqBabelLevelDataMessage](LuaEnumNetDef.ReqBabelLevelDataMessage, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqBabelLevelDataMessage, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqBabelLevelDataMessage", 11002, "ReqBabelLevelData", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

