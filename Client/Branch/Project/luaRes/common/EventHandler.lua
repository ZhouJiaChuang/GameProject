---主要用于管理lua中NetMsg和LuaEvent的成对绑定和解绑
---@class LuaEventHandler
local EventHandler = {}

EventHandler.__index = EventHandler

local luaEventManager = luaEventManager
local commonNetMsgDeal = commonNetMsgDeal

---创建一个新的实例
---@return LuaEventHandler
function EventHandler.CreateNew()
    local tbl = {}
    setmetatable(tbl, EventHandler)
    tbl.__index = tbl
    return tbl
end

---@private
---获取网络消息的绑定列表
---@param createNewIfNull boolean|nil
function EventHandler:GetNetMsgBindList(createNewIfNull)
    if createNewIfNull == true then
        if self.mNetMsgBindList == nil then
            self.mNetMsgBindList = {}
        end
    end
    return self.mNetMsgBindList
end

---@private
---获取LuaEvent的绑定列表
---@param createNewIfNull boolean|nil
function EventHandler:GetLuaEventBindList(createNewIfNull)
    if createNewIfNull == true then
        if self.mLuaEventBindList == nil then
            self.mLuaEventBindList = {}
        end
    end
    return self.mLuaEventBindList
end

---绑定网络事件
---@param netMsgID LuaEnumNetDef
---@param callback function
function EventHandler:BindNetMsg(netMsgID, callback)
    if self.mIsReleased then
        return
    end
    if netMsgID == nil or callback == nil then
        return
    end
    local tbl = self:GetNetMsgBindList(true)
    if tbl[netMsgID] == nil then
        tbl[netMsgID] = {}
    end
    local list = tbl[netMsgID]
    for i = 1, #list do
        if list[i] == callback then
            ---已经绑定过了
            return
        end
    end
    table.insert(list, callback)
    commonNetMsgDeal.BindCallback(netMsgID, callback)
end

---解绑网络事件
---@param netMsgID LuaEnumNetDef
---@param callback function
function EventHandler:RemoveNetMsg(netMsgID, callback)
    if self.mIsReleased then
        return
    end
    if netMsgID == nil or callback == nil then
        return
    end
    local tbl = self:GetNetMsgBindList(false)
    if tbl == nil then
        return
    end
    local isExist = false
    local list = tbl[netMsgID]
    for i = 1, #list do
        if list[i] == callback then
            table.remove(list, i)
            isExist = true
            break
        end
    end
    if isExist then
        commonNetMsgDeal.RemoveCallback(netMsgID, callback)
    end
end

---解绑由本handler绑定的所有的网络事件
function EventHandler:ReleaseAllNetMsg()
    if self.mIsReleased then
        return
    end
    local tbl = self:GetNetMsgBindList(false)
    if tbl == nil then
        return
    end
    for netMsgID, list in pairs(tbl) do
        for i = 1, #list do
            commonNetMsgDeal.RemoveCallback(netMsgID, list[i])
        end
        ---从字典中移除消息ID
        tbl[netMsgID] = nil
    end
end

---绑定lua客户端事件
---@param luaEventID LuaCEvent
---@param callback function
function EventHandler:BindLuaEvent(luaEventID, callback)
    if self.mIsReleased then
        return
    end
    if luaEventID == nil or callback == nil then
        return
    end
    local tbl = self:GetLuaEventBindList(true)
    if tbl[luaEventID] == nil then
        tbl[luaEventID] = {}
    end
    local list = tbl[luaEventID]
    for i = 1, #list do
        if list[i] == callback then
            ---已经绑定过了
            return
        end
    end
    table.insert(list, callback)
    luaEventManager.BindCallback(luaEventID, callback)
end

---解绑lua客户端事件
---@param luaEventID LuaCEvent
---@param callback function
function EventHandler:RemoveLuaEvent(luaEventID, callback)
    if self.mIsReleased then
        return
    end
    if luaEventID == nil or callback == nil then
        return
    end
    local tbl = self:GetLuaEventBindList(false)
    if tbl == nil then
        return
    end
    local isExist = false
    local list = tbl[luaEventID]
    if list then
        for i = 1, #list do
            if list[i] == callback then
                table.remove(list, i)
                isExist = true
                break
            end
        end
    end
    if isExist then
        luaEventManager.RemoveCallback(luaEventID, callback)
    end
end

---解绑由本handler绑定的所有的lua客户端事件
function EventHandler:ReleaseAllLuaEvent()
    if self.mIsReleased then
        return
    end
    local tbl = self:GetLuaEventBindList(false)
    if tbl == nil then
        return
    end
    for netMsgID, list in pairs(tbl) do
        for i = 1, #list do
            luaEventManager.RemoveCallback(netMsgID, list[i])
        end
        ---从字典中移除消息ID
        tbl[netMsgID] = nil
    end
end

---解绑所有事件
function EventHandler:ReleaseAll()
    self:ReleaseAllLuaEvent()
    self:ReleaseAllNetMsg()
end

return EventHandler