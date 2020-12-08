--[[本文件为工具自动生成,禁止手动修改]]
--babel.xml
local protobufMgr = protobufMgr
local commonNetMsgDeal = commonNetMsgDeal

---返回通天塔的基础数据
---msgID: 11001
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return babel.ResBabelData C#数据结构
function networkRespond.OnResBabelDataMessageReceived(msgID, tblData)
    if tblData == nil then
        CS.OnlineDebug.LogError("Lua消息内容为空\r\nID: 11001 babel.ResBabelData 返回通天塔的基础数据")
        commonNetMsgDeal.DoCallback(msgID, nil)
        return nil
    end
    ---消息不回写到C#中
    if isOpenLog then
        luaDebug.WriteNetMsgToLog("ResBabelDataMessage", 11001, "ResBabelData", tblData)
    end
    commonNetMsgDeal.DoCallback(msgID, tblData, nil)
    return nil
end

---返回指定层的通天塔数据
---msgID: 11003
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return babel.ResBabelLevelData C#数据结构
function networkRespond.OnResBabelLevelDataMessageReceived(msgID, tblData)
    if tblData == nil then
        CS.OnlineDebug.LogError("Lua消息内容为空\r\nID: 11003 babel.ResBabelLevelData 返回指定层的通天塔数据")
        commonNetMsgDeal.DoCallback(msgID, nil)
        return nil
    end
    ---消息不回写到C#中
    if isOpenLog then
        luaDebug.WriteNetMsgToLog("ResBabelLevelDataMessage", 11003, "ResBabelLevelData", tblData)
    end
    commonNetMsgDeal.DoCallback(msgID, tblData, nil)
    return nil
end

