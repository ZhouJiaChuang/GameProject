--通用表格加载处理
local CommonTableDealMgr = {}

---回调池
---以 消息ID -> 消息ID对应的回调数组 的形式存储
CommonTableDealMgr.CallbackPool = {}

---绑定回调
---回调callback形式为callback(fileName,data)形式
function CommonTableDealMgr.BindCallback(fileName, callback)
    if callback == nil then
        return
    end
    if Utility.IsContainsKey(CommonTableDealMgr.CallbackPool, fileName) == false then
        local tbl = {}
        CommonTableDealMgr.CallbackPool[fileName] = tbl
    end
    if Utility.IsContainsValue(CommonTableDealMgr.CallbackPool[fileName], callback) == false then
        local length = Utility.GetLuaTableCount(CommonTableDealMgr.CallbackPool[fileName])
        table.insert(CommonTableDealMgr.CallbackPool[fileName], length + 1, callback)
    end
end

---表格加载完毕回调
function CommonTableDealMgr.DoCallback(fileName, data)
    for k, v in pairs(CommonTableDealMgr.CallbackPool) do
        if k == fileName then
            if CommonTableDealMgr.CallbackPool[fileName] then
                for k, v in pairs(CommonTableDealMgr.CallbackPool[fileName]) do
                    if v then
                        v(fileName, data)
                    end
                end
            end
            return
        end
    end
end

---移除fileName表上绑定的callback回调
function CommonTableDealMgr.RemoveCallback(fileName, callback)
    for k, v in pairs(CommonTableDealMgr.CallbackPool) do
        if k == fileName then
            if CommonTableDealMgr.CallbackPool[fileName] then
                local length = Utility.GetLuaTableCount(CommonTableDealMgr.CallbackPool[fileName])
                for i = 1, length do
                    if CommonTableDealMgr.CallbackPool[fileName][i] == callback then
                        table.remove(CommonTableDealMgr.CallbackPool[fileName], i, callback)
                        return
                    end
                end
            end
            return
        end
    end
end

---移除fileName表上绑定的所有的回调函数
function CommonTableDealMgr.ClearCallback(fileName)
    for k, v in pairs(CommonTableDealMgr.CallbackPool) do
        if k == fileName then
            CommonTableDealMgr.CallbackPool[fileName] = nil
            return
        end
    end
end

---移除所有的回调函数
function CommonTableDealMgr.ClearAllCallback()
    CommonTableDealMgr.CallbackPool = {}
end

return CommonTableDealMgr