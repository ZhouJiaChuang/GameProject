--[[本文件为工具自动生成,禁止手动修改]]
--city.xml
local protobufMgr = protobufMgr
local commonNetMsgDeal = commonNetMsgDeal

---返回城市的所有数据
---msgID: 10001
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return city.ResCityData C#数据结构
function networkRespond.OnResCityDataMessageReceived(msgID, tblData)
    if tblData == nil then
        CS.OnlineDebug.LogError("Lua消息内容为空\r\nID: 10001 city.ResCityData 返回城市的所有数据")
        commonNetMsgDeal.DoCallback(msgID, nil)
        return nil
    end
    ---消息不回写到C#中
    if isOpenLog then
        luaDebug.WriteNetMsgToLog("ResCityDataMessage", 10001, "ResCityData", tblData)
    end
    commonNetMsgDeal.DoCallback(msgID, tblData, nil)
    return nil
end

---返回开启地面区域信息
---msgID: 10003
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return city.ResTerrainAreaInfo C#数据结构
function networkRespond.OnResTerrainAreaInfoMessageReceived(msgID, tblData)
    if tblData == nil then
        CS.OnlineDebug.LogError("Lua消息内容为空\r\nID: 10003 city.ResTerrainAreaInfo 返回开启地面区域信息")
        commonNetMsgDeal.DoCallback(msgID, nil)
        return nil
    end
    ---消息不回写到C#中
    if isOpenLog then
        luaDebug.WriteNetMsgToLog("ResTerrainAreaInfoMessage", 10003, "ResTerrainAreaInfo", tblData)
    end
    commonNetMsgDeal.DoCallback(msgID, tblData, nil)
    return nil
end

---返回创建建筑信息
---msgID: 10005
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return city.ResBuildInfo C#数据结构
function networkRespond.OnResBuildInfoMessageReceived(msgID, tblData)
    if tblData == nil then
        CS.OnlineDebug.LogError("Lua消息内容为空\r\nID: 10005 city.ResBuildInfo 返回创建建筑信息")
        commonNetMsgDeal.DoCallback(msgID, nil)
        return nil
    end
    ---消息不回写到C#中
    if isOpenLog then
        luaDebug.WriteNetMsgToLog("ResBuildInfoMessage", 10005, "ResBuildInfo", tblData)
    end
    commonNetMsgDeal.DoCallback(msgID, tblData, nil)
    return nil
end

