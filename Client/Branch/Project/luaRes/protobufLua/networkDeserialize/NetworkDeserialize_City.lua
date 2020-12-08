--[[本文件为工具自动生成,禁止手动修改]]
--city.xml
local protobufMgr = protobufMgr
local protoAdjust = protobufMgr.AdjustTable

---返回城市的所有数据
---msgID: 10001
---@param msgID LuaEnumNetDef 消息ID
---@param buffer string 消息内容
---@return table
function networkDeserialize.OnResCityDataMessageReceived(msgID, buffer)
    if buffer == nil then
        CS.OnlineDebug.LogError("Lua解析消息: 待解析内容为空\r\nID: 10001 city.ResCityData 返回城市的所有数据")
        return nil
    end
    local res = protobufMgr.Deserialize("city.ResCityData", buffer)
    if protoAdjust.city_adj ~= nil and protoAdjust.city_adj.AdjustResCityData ~= nil then
        protoAdjust.city_adj.AdjustResCityData(res)
    end
    return res
end

---返回开启地面区域信息
---msgID: 10003
---@param msgID LuaEnumNetDef 消息ID
---@param buffer string 消息内容
---@return table
function networkDeserialize.OnResTerrainAreaInfoMessageReceived(msgID, buffer)
    if buffer == nil then
        CS.OnlineDebug.LogError("Lua解析消息: 待解析内容为空\r\nID: 10003 city.ResTerrainAreaInfo 返回开启地面区域信息")
        return nil
    end
    local res = protobufMgr.Deserialize("city.ResTerrainAreaInfo", buffer)
    if protoAdjust.city_adj ~= nil and protoAdjust.city_adj.AdjustResTerrainAreaInfo ~= nil then
        protoAdjust.city_adj.AdjustResTerrainAreaInfo(res)
    end
    return res
end

---返回创建建筑信息
---msgID: 10005
---@param msgID LuaEnumNetDef 消息ID
---@param buffer string 消息内容
---@return table
function networkDeserialize.OnResBuildInfoMessageReceived(msgID, buffer)
    if buffer == nil then
        CS.OnlineDebug.LogError("Lua解析消息: 待解析内容为空\r\nID: 10005 city.ResBuildInfo 返回创建建筑信息")
        return nil
    end
    local res = protobufMgr.Deserialize("city.ResBuildInfo", buffer)
    if protoAdjust.city_adj ~= nil and protoAdjust.city_adj.AdjustResBuildInfo ~= nil then
        protoAdjust.city_adj.AdjustResBuildInfo(res)
    end
    return res
end

