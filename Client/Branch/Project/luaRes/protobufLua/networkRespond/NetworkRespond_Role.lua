--[[本文件为工具自动生成,禁止手动修改]]
--role.xml
local protobufMgr = protobufMgr
local commonNetMsgDeal = commonNetMsgDeal

---玩家基本数据
---msgID: 2001
---@param msgID LuaEnumNetDef 消息ID
---@param tblData table 消息内容
---@return role.ResPlayerBasicInfo C#数据结构
function networkRespond.OnResPlayerBasicInfoMessageReceived(msgID, tblData)
    if tblData == nil then
        CS.OnlineDebug.LogError("Lua消息内容为空\r\nID: 2001 role.ResPlayerBasicInfo 玩家基本数据")
        commonNetMsgDeal.DoCallback(msgID, nil)
        return nil
    end
    ---消息不回写到C#中
    if isOpenLog then
        luaDebug.WriteNetMsgToLog("ResPlayerBasicInfoMessage", 2001, "ResPlayerBasicInfo", tblData)
    end
    commonNetMsgDeal.DoCallback(msgID, tblData, nil)
    return nil
end

