--[[本文件为工具自动生成,禁止手动修改]]
--babel.xml
local protobufMgr = protobufMgr
local protoAdjust = protobufMgr.AdjustTable

---返回通天塔的基础数据
---msgID: 11001
---@param msgID LuaEnumNetDef 消息ID
---@param buffer string 消息内容
---@return table
function networkDeserialize.OnResBabelDataMessageReceived(msgID, buffer)
    if buffer == nil then
        CS.OnlineDebug.LogError("Lua解析消息: 待解析内容为空\r\nID: 11001 babel.ResBabelData 返回通天塔的基础数据")
        return nil
    end
    local res = protobufMgr.Deserialize("babel.ResBabelData", buffer)
    if protoAdjust.babel_adj ~= nil and protoAdjust.babel_adj.AdjustResBabelData ~= nil then
        protoAdjust.babel_adj.AdjustResBabelData(res)
    end
    return res
end

---返回指定层的通天塔数据
---msgID: 11003
---@param msgID LuaEnumNetDef 消息ID
---@param buffer string 消息内容
---@return table
function networkDeserialize.OnResBabelLevelDataMessageReceived(msgID, buffer)
    if buffer == nil then
        CS.OnlineDebug.LogError("Lua解析消息: 待解析内容为空\r\nID: 11003 babel.ResBabelLevelData 返回指定层的通天塔数据")
        return nil
    end
    local res = protobufMgr.Deserialize("babel.ResBabelLevelData", buffer)
    if protoAdjust.babel_adj ~= nil and protoAdjust.babel_adj.AdjustResBabelLevelData ~= nil then
        protoAdjust.babel_adj.AdjustResBabelLevelData(res)
    end
    return res
end

