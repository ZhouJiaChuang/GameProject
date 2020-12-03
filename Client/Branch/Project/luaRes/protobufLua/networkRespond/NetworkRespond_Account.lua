--[[本文件为工具自动生成,禁止手动修改]]
--account.xml
local protobufMgr = protobufMgr
local commonNetMsgDeal = commonNetMsgDeal

---返回登入账号信息
---msgID: 1002
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return nil
function networkRespond.OnResLoginAccountReceived(msgID, tblData)
    commonNetMsgDeal.DoCallback(msgID, nil, nil)
    return nil
end

---返回服务器信息
---msgID: 1004
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return nil
function networkRespond.OnResLoginServerReceived(msgID, tblData)
    commonNetMsgDeal.DoCallback(msgID, nil, nil)
    return nil
end

