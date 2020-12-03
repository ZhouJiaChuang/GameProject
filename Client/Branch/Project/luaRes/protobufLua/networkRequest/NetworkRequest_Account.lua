--[[本文件为工具自动生成,禁止手动修改]]
--account.xml

--region ID:1001 请求登入账号
---请求登入账号
---msgID: 1001
---@param type number 必填参数
---@param account string 必填参数 根据类型,存在不同的账号数据
---@return boolean 网络请求是否成功发送
function networkRequest.ReqLoginAccount(type, account)
    local reqTable = {}
    reqTable.type = type
    reqTable.account = account
    local reqMsgData = protobufMgr.Serialize("account.AccountLoginReqData" , reqTable)
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqLoginAccount] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqLoginAccount](LuaEnumNetDef.ReqLoginAccount, reqTable)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsgByteByLua(LuaEnumNetDef.ReqLoginAccount, reqMsgData)
        if isOpenLog then
            luaDebug.WriteNetMsgToLog("ReqLoginAccount", 1001, "AccountLoginReqData", reqTable, true)
        end
    end
    return canSendMsg
end
--endregion

--region ID:1003 请求登入服务器
---请求登入服务器
---msgID: 1003
---@return boolean 网络请求是否成功发送
function networkRequest.ReqLoginServer()
    local canSendMsg = true
    if netMsgPreverifying and netMsgPreverifying[LuaEnumNetDef.ReqLoginServer] then
        canSendMsg = netMsgPreverifying[LuaEnumNetDef.ReqLoginServer](LuaEnumNetDef.ReqLoginServer, nil)
    end
    if canSendMsg then
        CS.CSNetwork.Instance:SendMsg(LuaEnumNetDef.ReqLoginServer, nil, true)
    end
    return canSendMsg
end
--endregion

