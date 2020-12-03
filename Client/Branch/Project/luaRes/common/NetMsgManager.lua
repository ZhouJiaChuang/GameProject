---通用网络消息包处理
---@class NetMsgManager
local NetMsgManager = {}

local Utility = Utility

---回调池
---以 消息ID -> 消息ID对应的回调数组 的形式存储
NetMsgManager.CallbackPool = {}
---执行回调的try table
NetMsgManager.DoCallbackTryTable = {}
---绑定警告的数量限制
---@type number
NetMsgManager.BindWarningCountLimit = nil
---绑定警告的回调
---@type fun<number,number>
NetMsgManager.BindWarningCallBack = nil

---正在分发的消息的ID
NetMsgManager.msgID = nil
---正在分发的消息的table类型数据
NetMsgManager.tblData = nil
---正在分发的消息的C#类型数据
NetMsgManager.csData = nil

---正在执行回调标志位
NetMsgManager.IsDoingCallback = false
---需要移除所有的回调
NetMsgManager.NeedRemoveAllCallback = false
---需要移除消息ID上的所有回调
NetMsgManager.NeedRemoveAllCallbackInMsgID = false
---待移除全部回调的消息ID列表
NetMsgManager.CallbackToBeAllCleared = {}
---需要移除消息ID上的目标回调
NetMsgManager.NeedRemoveMsgInMsgID = false
---待移除回调列表(ID)
NetMsgManager.CallbackToBeRemoved_ID = {}
---待移除回调列表(回调)
NetMsgManager.CallbackToBeRemoved_Callback = {}

---在消息上绑定回调
---msgID 为消息的 ID ,在 C# 中为 long 类型, lua 中为 number 类型,枚举类型需先使用 Utility.EnumToInt 转换为number类型
---回调callback形式为callback(id,data)形式
---@private
---@param msgID number 消息ID
---@param callback function 回调方法 function(msgID, tblData, csData)
function NetMsgManager.BindCallback(msgID, callback)
    if callback == nil then
        return
    end
    if Utility.IsContainsKey(NetMsgManager.CallbackPool, msgID) == false then
        NetMsgManager.CallbackPool[msgID] = {}
    end
    local pool = NetMsgManager.CallbackPool[msgID]
    if Utility.IsContainsValue(NetMsgManager.CallbackPool[msgID], callback) == false then
        table.insert(NetMsgManager.CallbackPool[msgID], callback)
    end
    ---回调数量监视,超过预定数量则发出警告
    if NetMsgManager.BindWarningCountLimit ~= nil and NetMsgManager.BindWarningCountLimit > 0 then
        local count = Utility.GetLuaTableCount(pool)
        if count > NetMsgManager.BindWarningCountLimit then
            NetMsgManager.BindWarningCountLimit = nil
            if NetMsgManager.BindWarningCallBack ~= nil then
                NetMsgManager.BindWarningCallBack(msgID, count)
            end
        end
    end
end

--region 分发消息
---消息分发
---@param msgID number 消息ID
---@param tblData table lua table类型消息数据
---@param csData userdata C# class类型消息数据
function NetMsgManager.DoCallback(msgID, tblData, csData)
    NetMsgManager.IsDoingCallback = true
    NetMsgManager.msgID = msgID
    NetMsgManager.tblData = tblData
    NetMsgManager.csData = csData
    try(NetMsgManager.DoCallbackTryTable)
    NetMsgManager.msgID = nil
    NetMsgManager.tblData = nil
    NetMsgManager.csData = nil
    NetMsgManager.IsDoingCallback = false
    NetMsgManager.ClearCallbacksNeededToBeRemoved()
end

---回调主方法
NetMsgManager.DoCallbackTryTable.main = function()
    --先进行网络消息预处理
    if netMsgPreprocessing and netMsgPreprocessing[NetMsgManager.msgID] then
        netMsgPreprocessing[NetMsgManager.msgID](NetMsgManager.msgID, NetMsgManager.tblData, NetMsgManager.csData)
    end
    --分发消息
    for k, v in pairs(NetMsgManager.CallbackPool) do
        if k == NetMsgManager.msgID then
            if NetMsgManager.CallbackPool[NetMsgManager.msgID] then
                for i = 1, #NetMsgManager.CallbackPool[NetMsgManager.msgID] do
                    if NetMsgManager.CallbackPool[NetMsgManager.msgID][i] then
                        NetMsgManager.CallbackPool[NetMsgManager.msgID][i](NetMsgManager.msgID, NetMsgManager.tblData, NetMsgManager.csData)
                    end
                end
            end
            return
        end
    end
end

---回调catch方法
NetMsgManager.DoCallbackTryTable.catch = function(errors)
    CS.UnityEngine.Debug.LogError("catch : 分发消息: ID:" .. NetMsgManager.msgID .. "\r\n" .. errors)
end
--endregion

--region 移除Callback
---移除消息ID为msgID上绑定的callback回调
---@private
---@param msgID number 需要移除的消息ID
---@param callback function 需要从消息上移除的回调
function NetMsgManager.RemoveCallback(msgID, callback)
    if callback == nil then
        return
    end
    if NetMsgManager.IsDoingCallback then
        NetMsgManager.NeedRemoveMsgInMsgID = true
        table.insert(NetMsgManager.CallbackToBeRemoved_ID, msgID)
        table.insert(NetMsgManager.CallbackToBeRemoved_Callback, callback)
        return
    end
    for k, v in pairs(NetMsgManager.CallbackPool) do
        if k == msgID then
            if NetMsgManager.CallbackPool[msgID] then
                for i = 1, #NetMsgManager.CallbackPool[msgID] do
                    if NetMsgManager.CallbackPool[msgID][i] == callback then
                        table.remove(NetMsgManager.CallbackPool[msgID], i)
                        return
                    end
                end
            end
            return
        end
    end
end

---移除消息ID为msgID上所有的回调函数
---@param msgID number 需要清除所有回调函数的消息ID
function NetMsgManager.ClearCallback(msgID)
    if NetMsgManager.IsDoingCallback then
        NetMsgManager.NeedRemoveAllCallbackInMsgID = true
        table.insert(NetMsgManager.CallbackToBeAllCleared, msgID)
        return
    end
    for k, v in pairs(NetMsgManager.CallbackPool) do
        if k == msgID then
            NetMsgManager.CallbackPool[msgID] = nil
            return
        end
    end
end

---移除所有的回调函数
function NetMsgManager.ClearAllCallback()
    if NetMsgManager.IsDoingCallback then
        NetMsgManager.NeedRemoveAllCallback = true
        return
    end
    NetMsgManager.CallbackPool = {}
end

---清除需要被移除的回调
function NetMsgManager.ClearCallbacksNeededToBeRemoved()
    ---检查是否需要移除所有回调
    if NetMsgManager.NeedRemoveAllCallback then
        NetMsgManager.ClearAllCallback()
        NetMsgManager.NeedRemoveAllCallback = false
        NetMsgManager.NeedRemoveMsgInMsgID = false
        NetMsgManager.NeedRemoveAllCallbackInMsgID = false
        NetMsgManager.ClearTable(NetMsgManager.CallbackToBeAllCleared)
        NetMsgManager.ClearTable(NetMsgManager.CallbackToBeRemoved_ID)
        NetMsgManager.ClearTable(NetMsgManager.CallbackToBeRemoved_Callback)
        return
    end
    ---检查是否需要移除某消息ID上的回调
    if NetMsgManager.NeedRemoveAllCallbackInMsgID then
        for i = 1, #NetMsgManager.CallbackToBeAllCleared do
            NetMsgManager.ClearCallback(NetMsgManager.CallbackToBeAllCleared[i])
        end
        NetMsgManager.NeedRemoveAllCallbackInMsgID = false
        NetMsgManager.ClearTable(NetMsgManager.CallbackToBeAllCleared)
    end
    ---检查是否需要移除目标回调
    if NetMsgManager.NeedRemoveMsgInMsgID then
        for i = 1, #NetMsgManager.CallbackToBeRemoved_ID do
            NetMsgManager.RemoveCallback(NetMsgManager.CallbackToBeRemoved_ID[i], NetMsgManager.CallbackToBeRemoved_Callback[i])
        end
        NetMsgManager.ClearTable(NetMsgManager.CallbackToBeRemoved_ID)
        NetMsgManager.ClearTable(NetMsgManager.CallbackToBeRemoved_Callback)
        NetMsgManager.NeedRemoveMsgInMsgID = false
    end
end

---清理table内容
function NetMsgManager.ClearTable(tbl)
    for i, v in pairs(tbl) do
        tbl[i] = nil
    end
end
--endregion


---获取回调状态
---@return table
function NetMsgManager.GetCallBackState()
    local tbl = {}
    for i, v in pairs(NetMsgManager.CallbackPool) do
        table.insert(tbl, { id = i, count = #v })
    end
    table.sort(tbl, function(l, r)
        return l.count > r.count
    end)
    local strs = {}
    if #tbl > 0 then
        for i = 1, #tbl do
            table.insert(strs, "NetMsg Event  ID:" .. tostring(tbl[i].id) .. "  CallBackCount:" .. tostring(tbl[i].count))
        end
    end
    return strs
end

return NetMsgManager