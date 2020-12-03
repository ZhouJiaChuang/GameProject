local cs_coroutine = {}

local xluamgr = CS.XLuaManager.Instance

local function async_yield_return(to_yield, cb)
    xluamgr:YieldAndCallback(to_yield, cb)
end

---启动C#中的协程,返回开启的协程
function cs_coroutine.StartCoroutine(coroutinefunction, ...)
    if coroutinefunction == nil or xluamgr == nil then
        return nil
    end
    return xluamgr:StartCoroutine(util.cs_generator(coroutinefunction, ...))
end

---关闭从本脚本启动的C#中的协程
function cs_coroutine.StopCoroutine(coroutine)
    if coroutine ~= nil and xluamgr ~= nil then
        xluamgr:StopCoroutine(coroutine)
    end
end

cs_coroutine.yield_return = util.async_to_sync(async_yield_return)
return cs_coroutine