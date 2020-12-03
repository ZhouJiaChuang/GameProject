---@class LuaClassRegister lua的class脚本注册
local LuaClassRegister = {}

luaclass = {}

function LuaClassRegister:Init()

    LuaClassRegister.luaobject = require "luaRes.scripts.luaobject"

    self:LoadAllClassFiles()
    self:BindAllMetatables()
end

function LuaClassRegister:LoadAllClassFiles()
    ---lua的展示脚本
    luaclass.LuaObservationModel = require "luaRes.ui.extend.LuaObservationModel"
end


---绑定所有元表
---@private
function LuaClassRegister:BindAllMetatables()
    for i, v in pairs(luaclass) do
        self:BindMetatable(v, i)
        --if luaDebugTool ~= nil then
        --    luaDebugTool.RecordTable(luaDebugTool.TableType.LuaClass, v)
        --end
    end
    luaclass.__index = luaclass
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
