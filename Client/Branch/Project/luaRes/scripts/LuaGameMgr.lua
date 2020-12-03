---@class LuaGameMgr
local LuaGameMgr = {}

function LuaGameMgr:Initialize()
    self:BindEvent()
end

function LuaGameMgr:BindEvent()
    CS.XLuaManager.Instance.luaGameMgrUpdate = function()
        self:Update()
    end
end

function LuaGameMgr:Update()

end

return LuaGameMgr;