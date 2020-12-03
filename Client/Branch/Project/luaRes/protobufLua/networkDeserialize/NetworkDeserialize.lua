--[[本文件为工具自动生成,禁止手动修改]]
---网络消息解析
networkDeserialize = {}

---客户端链接服务器成功
---msgID: 101
---@param msgID LuaEnumNetDef 消息ID
---@param buffer string 消息内容
---@return nil
function networkDeserialize.OnResConnectSuccessMessageReceived(msgID, buffer)
    return nil
end

---客户端链接服务器失败
---msgID: 102
---@param msgID LuaEnumNetDef 消息ID
---@param buffer string 消息内容
---@return nil
function networkDeserialize.OnResConnectFailedMessageReceived(msgID, buffer)
    return nil
end

--account.xml
require 'luaRes.protobufLua.networkDeserialize.NetworkDeserialize_Account'
--role.xml
require 'luaRes.protobufLua.networkDeserialize.NetworkDeserialize_Role'
