---@class luaobject lua对象,不支持隐式实例化(即实例化不能进行在加载lua过程中),有继承关系的table必须遵循父类table定义在子类table之后
local luaobject = {}

local Utility = Utility
local IsNullFunction = CS.StaticUtility.IsNull

---类名
---@public
---@type string
luaobject.__className = nil
---lua对象对应的Unity GameObject对象
---@public
---@type UnityEngine.GameObject
luaobject.go = nil

---索引
---@private
---@type table
luaobject.__index = luaobject
---GC时执行
---@private
---@type function
luaobject.__gc = luaclass.OnTableDestroyed
---运行基类时对每层基类的调用进行计数,以确定元表的调用次数
---@private
---@type table
luaobject.__RunBaseCountDic = {}
---C#的Utility_Lua.Get方法
---@type function
luaobject.__CSGetFunction = CS.Utility_Lua.Get

---根据路径获取self.go物体下组件或Transform,GameObject
---@protected
---@param path string 相对路径
---@param typeStr string 组件类型字符串
function luaobject:Get(path, typeStr)
    if self.go == nil or path == nil or typeStr == nil then
        return nil
    end
    return luaobject.__CSGetFunction(self.go.transform, path, typeStr)
end

---根据路径获取模板的物体下组件或Transform,GameObject
---@protected
---@param go UnityEngine.GameObject 父节点
---@param path string 相对路径
---@param typeStr string 组件类型字符串
function luaobject:GetCurComp(go, path, typeStr)
    if IsNullFunction(go) or path == nil or typeStr == nil then
        return nil
    end
    return luaobject.__CSGetFunction(go.transform, path, typeStr)
end

---创建一个自身的新table实例
---动态参数将传入Init方法中作为形参
---@public
---@vararg any
---@return luaobject
function luaobject:New(...)
    return self:NewWithGO(nil, ...)
end

---创建一个自身的新table实例,并将go参数赋值给self.go字段
---动态参数将传入Init方法中作为形参
---@public
---@param go UnityEngine.GameObject 该lua对象对应的UnityEngine对象
---@vararg any
---@return luaobject
function luaobject:NewWithGO(go, ...)
    ---@type luaobject
    local tbl = {}
    setmetatable(tbl, self)
    ---@private
    ---@type boolean 是否为动态创建的对象(区别于静态对象)
    tbl.__DynamicObj = true
    tbl.go = go
    ---构造函数
    tbl:Init(...)
    --if luaDebugTool ~= nil then
    --    luaDebugTool.RecordTable(luaDebugTool.TableType.TemplateInstance, tbl)
    --end
    return tbl
end

---构造函数
---@protected
---@vararg any
function luaobject:Init(...)
end

---析构函数
---@protected
function luaobject:OnDestruct()
end

---执行元表中的函数
---@protected
---@param functionName string 元表中的函数名
---@vararg any
function luaobject:RunBaseFunction(functionName, ...)
    if self ~= nil and functionName ~= nil then
        --调用函数时,在__RunBaseCountDic表中对传入的self对象的调用次数加一或初始化为一
        if Utility.IsContainsKey(luaobject.__RunBaseCountDic, self) == false then
            luaobject.__RunBaseCountDic[self] = {}
        end
        if Utility.IsContainsKey(luaobject.__RunBaseCountDic[self], functionName) == false then
            luaobject.__RunBaseCountDic[self][functionName] = 1
        else
            luaobject.__RunBaseCountDic[self][functionName] = luaobject.__RunBaseCountDic[self][functionName] + 1
        end
        --该函数中传入的self一般为以class table为元表新建的表,所以需要先取一次元表以获取class table
        local baseMetaTable = self.__DynamicObj and getmetatable(self) or self
        --根据调用本函数的次数决定向上取多少次才能获取到上一级的元表,次数为1表示只需要取一次元表
        for i = 1, luaobject.__RunBaseCountDic[self][functionName] do
            if baseMetaTable ~= nil then
                baseMetaTable = getmetatable(baseMetaTable)
            else
                break
            end
        end
        local result
        --若元表中找到了该方法
        if baseMetaTable ~= nil and baseMetaTable[functionName] ~= nil then
            if Utility.IsTruelyContainsKey(baseMetaTable, functionName) then
                --若元表本表中有该方法,则调用元表中的方法,并将返回值放入返回值表中
                result = { baseMetaTable[functionName](self, ...) }
            else
                --若元表本表中没有该方法,则再次递归本方法
                result = { self:RunBaseFunction(functionName, ...) }
            end
        end
        --将__RunBaseCountDic表中self对应的调用次数减一
        luaobject.__RunBaseCountDic[self][functionName] = luaobject.__RunBaseCountDic[self][functionName] - 1
        ---释放内存
        if luaobject.__RunBaseCountDic[self][functionName] == 0 then
            luaobject.__RunBaseCountDic[self][functionName] = nil
            if Utility.IsNullTable(luaobject.__RunBaseCountDic[self]) then
                luaobject.__RunBaseCountDic[self] = nil
            end
        end
        --将返回值表中的元素依次放入返回队列中
        if result ~= nil then
            return table.unpack(result)
        end
    end
end

return luaobject