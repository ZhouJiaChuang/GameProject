--[[本文件为工具自动生成,禁止手动修改]]
---网络消息响应
networkRespond = {}

---客户端链接服务器成功
---msgID: 101
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return nil
function networkRespond.OnResConnectSuccessMessageReceived(msgID, tblData)
    commonNetMsgDeal.DoCallback(msgID, nil, nil)
    return nil
end

---客户端链接服务器失败
---msgID: 102
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return nil
function networkRespond.OnResConnectFailedMessageReceived(msgID, tblData)
    commonNetMsgDeal.DoCallback(msgID, nil, nil)
    return nil
end

--account.xml
require 'luaRes.protobufLua.networkRespond.NetworkRespond_Account'
--role.xml
require 'luaRes.protobufLua.networkRespond.NetworkRespond_Role'
