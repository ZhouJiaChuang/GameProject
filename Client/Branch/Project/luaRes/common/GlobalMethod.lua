--[[******************************************本脚本用于记录一些轻量级的全局方法,目的是简化代码逻辑,令代码可读性更强*********************************************]]

--region 逻辑方法
---简单的封装了三元运算符
---但对于参数为函数返回值的情况
---应当直接使用ifelse结构而非本函数
---该方法不应再使用,使用 a and b or c 的结构表达三元表达式更好
---@param a boolean
---@param b any
---@param c any
function ternary(a, b, c)
    if a then
        return b
    else
        return c
    end
end
--endregion

--region 数学方法
---将数字限定在范围内
---@param v number
---@param min number
---@param max number
---@return number
function math.clamp(v, min, max)
    if v < min then
        v = min
    end
    if v > max then
        v = max
    end
    return v
end

---将数字限定在0~1范围内
---@param v number
function math.clamp01(v)
    return math.clamp(v, 0, 1)
end
--endregion