--[[本文件为工具自动生成,禁止手动修改]]
--role.xml
local protobufMgr = protobufMgr
local protoAdjust = protobufMgr.AdjustTable

---玩家基本数据
---msgID: 2001
---@param msgID LuaEnumNetDef 消息ID
---@param buffer string 消息内容
---@return table
function networkDeserialize.OnResPlayerBasicInfoMessageReceived(msgID, buffer)
    if buffer == nil then
        CS.OnlineDebug.LogError("Lua解析消息: 待解析内容为空\r\nID: 2001 role.ResPlayerBasicInfo 玩家基本数据")
        return nil
    end
    local res = protobufMgr.Deserialize("role.ResPlayerBasicInfo", buffer)
    if protoAdjust.role_adj ~= nil and protoAdjust.role_adj.AdjustResPlayerBasicInfo ~= nil then
        protoAdjust.role_adj.AdjustResPlayerBasicInfo(res)
    end
    return res
end

