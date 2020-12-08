---@class LuaClassRegister lua的class脚本注册
local LuaClassRegister = {}

luaClass = {}

function LuaClassRegister:Init()

    LuaClassRegister.luaobject = require "luaRes.scripts.luaobject"

    self:LoadAllClassFiles()
    self:BindAllMetatables()
end

function LuaClassRegister:LoadAllClassFiles()
    ---lua的展示脚本
    luaClass.LuaObservationModel = require "luaRes.ui.extend.LuaObservationModel"

    luaClass.LuaDataMgr = require "scripts.data.LuaDataMgr"


    --region 城市
    ---城市管理类
    luaClass.LuaCityMgr = require "scripts.data.city.LuaCityMgr"

    luaClass.LuaCityBuildItem = require "scripts.data.city.build.LuaCityBuildItem"
    --endregion

    --region 通天塔
    ---通天塔管理类
    luaClass.LuaBabelMgr = require "scripts.data.babel.LuaBabelMgr"
    --endregion
end


---绑定所有元表
---@private
function LuaClassRegister:BindAllMetatables()
    for i, v in pairs(luaClass) do
        self:BindMetatable(v, i)
        --if luaDebugTool ~= nil then
        --    luaDebugTool.RecordTable(luaDebugTool.TableType.LuaClass, v)
        --end
    end
    luaClass.__index = luaClass
end

---为单个表绑定元表
---@private
---@param tbl luaobject 需要绑定元表的表
---@param classname string 表对应的类名
function LuaClassRegister:BindMetatable(tbl, classname)
    if tbl and type(tbl) == "table" then
        local metatable = getmetatable(tbl)
        ---当元表为空或元表等与自身且自身不为luaobject时,设置元表为luaobject(自身的元表为自身时,索引其对象将出现死循环)
        if (metatable == nil or tbl == metatable) and tbl ~= LuaClassRegister.luaobject then
            setmetatable(tbl, LuaClassRegister.luaobject)
        end
        tbl.__index = tbl
        tbl.__className = classname
    end
end

---表析构方法
---@public
---@param tbl luaobject
function LuaClassRegister.OnTableDestroyed(tbl)
    if tbl == nil then
        return
    end
    if tbl.OnDestruct then
        tbl:OnDestruct()
    end
    for i, v in pairs(tbl) do
        tbl[i] = nil
    end
end

return LuaClassRegister
