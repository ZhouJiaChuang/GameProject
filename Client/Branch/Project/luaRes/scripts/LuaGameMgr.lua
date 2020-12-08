---@class LuaGameMgr
local LuaGameMgr = {}

---@return LuaDataMgr
function LuaGameMgr:GetDataMgr()
    if(self.mLuaDataMgr == nil) then
        self.mLuaDataMgr = luaClass.LuaDataMgr:New()
    end
end


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