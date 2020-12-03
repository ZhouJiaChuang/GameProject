---通用网络消息包处理
---  绑定回调: function ClientMsgManager.BindCallback(msgID, callback)
---  存在回调: function ClientMsgManager.HasCallback(msgID)
---  执行回调: function ClientMsgManager.DoCallback(msgID, data)
---  移除回调: function ClientMsgManager.RemoveCallback(msgID, callback)
---@class ClientMsgManager
local ClientMsgManager = {}

---回调池
---以 消息ID -> 消息ID对应的回调数组 的形式存储
ClientMsgManager.CallbackPool = {}
---执行回调的try table
ClientMsgManager.DoCallbackTryTable = {}
---绑定警告的数量限制
---@type number
ClientMsgManager.BindWarningCountLimit = nil
---绑定警告的回调
---@type fun<number,number>
ClientMsgManager.BindWarningCallBack = nil

---正在分发的消息的ID
ClientMsgManager.msgID = nil
---正在分发的消息的table类型数据
ClientMsgManager.data = nil

---正在执行回调标志位
ClientMsgManager.IsDoingCallback = false
---需要移除所有的回调
ClientMsgManager.NeedRemoveAllCallback = false
---需要移除消息ID上的所有回调
ClientMsgManager.NeedRemoveAllCallbackInMsgID = false
---待移除全部回调的消息ID列表
ClientMsgManager.CallbackToBeAllCleared = {}
---需要移除消息ID上的目标回调
ClientMsgManager.NeedRemoveMsgInMsgID = false
---待移除回调列表(ID)
ClientMsgManager.CallbackToBeRemoved_ID = {}
---待移除回调列表(回调)
ClientMsgManager.CallbackToBeRemoved_Callback = {}

---在消息上绑定回调
---msgID为消息枚举
---回调callback形式为callback(msgID,data)形式
---@private
---@param msgID LuaCEvent 消息枚举
---@param callback function 回调方法 function(msgID, data)
function ClientMsgManager.BindCallback(msgID, callback)
    if msgID == nil or callback == nil then
        return
    end
    if Utility.IsContainsKey(ClientMsgManager.CallbackPool, msgID) == false then
        ClientMsgManager.CallbackPool[msgID] = {}
    end
    local pool = ClientMsgManager.CallbackPool[msgID]
    if Utility.IsContainsValue(pool, callback) == false then
        table.insert(pool, callback)
    end
    ---回调数量监视,超过预定数量则发出警告
    if ClientMsgManager.BindWarningCountLimit ~= nil and ClientMsgManager.BindWarningCountLimit > 0 then
        local count = Utility.GetLuaTableCount(pool)
        if count > ClientMsgManager.BindWarningCountLimit then
            ClientMsgManager.BindWarningCountLimit = nil
            if ClientMsgManager.BindWarningCallBack ~= nil then
                ClientMsgManager.BindWarningCallBack(msgID, count)
            end
        end
    end
end

---查询某消息上是否有绑定了回调
---@param msgID LuaCEvent 消息
---@return boolean 该消息上是否有绑定了回调
function ClientMsgManager.HasCallback(msgID)
    return msgID and (Utility.IsContainsKey(ClientMsgManager.CallbackPool, msgID)) and (Utility.GetLuaTableCount(ClientMsgManager.CallbackPool[msgID]) > 0)
end

---消息分发
---@param msgID LuaCEvent 消息ID
---@param data table
function ClientMsgManager.DoCallback(msgID, data)
    if msgID == nil then
        return
    end
    ClientMsgManager.IsDoingCallback = true
    ClientMsgManager.msgID = msgID
    ClientMsgManager.data = data
    try(ClientMsgManager.DoCallbackTryTable)
    ClientMsgManager.msgID = nil
    ClientMsgManager.data = nil
    ClientMsgManager.IsDoingCallback = false
    ClientMsgManager.ClearCallbacksNeededToBeRemoved()
end

---回调主方法
ClientMsgManager.DoCallbackTryTable.main = function()
    --分发消息
    local msgId = ClientMsgManager.msgID
    local data = ClientMsgManager.data
    local callbacks = ClientMsgManager.CallbackPool[msgId]
    if callbacks then
        for i = #callbacks, 1, -1 do
            if callbacks[i] then
                callbacks[i](msgId, data)
            end
        end
    end
end

---回调catch方法
ClientMsgManager.DoCallbackTryTable.catch = function(errors)
    CS.UnityEngine.Debug.LogError("catch : 分发Lua内部消息: ID:" .. tostring(ClientMsgManager.msgID) .. "\r\n" .. errors)
end

---移除消息ID为msgID上绑定的callback回调
---@private
---@param msgID LuaCEvent 需要移除的消息ID
---@param callback function 需要从消息上移除的回调
function ClientMsgManager.RemoveCallback(msgID, callback)
    if callback == nil then
        return
    end
    if ClientMsgManager.IsDoingCallback then
        ClientMsgManager.NeedRemoveMsgInMsgID = true
        table.insert(ClientMsgManager.CallbackToBeRemoved_ID, msgID)
        table.insert(ClientMsgManager.CallbackToBeRemoved_Callback, callback)
        return
    end
    for k, v in pairs(ClientMsgManager.CallbackPool) do
        if k == msgID then
            if ClientMsgManager.CallbackPool[msgID] then
                for i = 1, #ClientMsgManager.CallbackPool[msgID] do
                    if ClientMsgManager.CallbackPool[msgID][i] == callback then
                        table.remove(ClientMsgManager.CallbackPool[msgID], i)
                        return
                    end
                end
            end
            return
        end
    end
end

---移除消息ID为msgID上所有的回调函数
---@param msgID LuaCEvent 需要清除所有回调函数的消息ID
function ClientMsgManager.ClearCallback(msgID)
    if ClientMsgManager.IsDoingCallback then
        ClientMsgManager.NeedRemoveAllCallbackInMsgID = true
        table.insert(ClientMsgManager.CallbackToBeAllCleared, msgID)
        return
    end
    for k, v in pairs(ClientMsgManager.CallbackPool) do
        if k == msgID then
            ClientMsgManager.CallbackPool[msgID] = nil
            return
        end
    end
end

---移除所有的回调函数
function ClientMsgManager.ClearAllCallback()
    if ClientMsgManager.IsDoingCallback then
        ClientMsgManager.NeedRemoveAllCallback = true
        return
    end
    ClientMsgManager.CallbackPool = {}
end

---清除需要被移除的回调
function ClientMsgManager.ClearCallbacksNeededToBeRemoved()
    ---检查是否需要移除所有回调
    if ClientMsgManager.NeedRemoveAllCallback then
        ClientMsgManager.ClearAllCallback()
        ClientMsgManager.NeedRemoveAllCallback = false
        ClientMsgManager.NeedRemoveMsgInMsgID = false
        ClientMsgManager.NeedRemoveAllCallbackInMsgID = false
        ClientMsgManager.ClearTable(ClientMsgManager.CallbackToBeAllCleared)
        ClientMsgManager.ClearTable(ClientMsgManager.CallbackToBeRemoved_ID)
        ClientMsgManager.ClearTable(ClientMsgManager.CallbackToBeRemoved_Callback)
        return
    end
    ---检查是否需要移除某消息ID上的回调
    if ClientMsgManager.NeedRemoveAllCallbackInMsgID then
        for i = 1, #ClientMsgManager.CallbackToBeAllCleared do
            ClientMsgManager.ClearCallback(ClientMsgManager.CallbackToBeAllCleared[i])
        end
        ClientMsgManager.NeedRemoveAllCallbackInMsgID = false
        ClientMsgManager.ClearTable(ClientMsgManager.CallbackToBeAllCleared)
    end
    ---检查是否需要移除目标回调
    if ClientMsgManager.NeedRemoveMsgInMsgID then
        for i = 1, #ClientMsgManager.CallbackToBeRemoved_ID do
            ClientMsgManager.RemoveCallback(ClientMsgManager.CallbackToBeRemoved_ID[i], ClientMsgManager.CallbackToBeRemoved_Callback[i])
        end
        ClientMsgManager.ClearTable(ClientMsgManager.CallbackToBeRemoved_ID)
        ClientMsgManager.ClearTable(ClientMsgManager.CallbackToBeRemoved_Callback)
        ClientMsgManager.NeedRemoveMsgInMsgID = false
    end
end

---清理table内容
---@param tbl table 待清理的table
function ClientMsgManager.ClearTable(tbl)
    for i, v in pairs(tbl) do
        tbl[i] = nil
    end
end

---获取回调状态
---@return table
function ClientMsgManager.GetCallBackState()
    local tbl = {}
    for i, v in pairs(ClientMsgManager.CallbackPool) do
        table.insert(tbl, { id = i, count = #v })
    end
    table.sort(tbl, function(l, r)
        return l.count > r.count
    end)
    local strs = {}
    if #tbl > 0 then
        for i = 1, #tbl do
            table.insert(strs, "Lua Event  ID:" .. tostring(tbl[i].id) .. "  CallBackCount:" .. tostring(tbl[i].count))
        end
    end
    return strs
end

return ClientMsgManager