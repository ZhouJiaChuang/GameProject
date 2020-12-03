---@class LuaDebug
local LuaDebug = {}

local type = type

function LuaDebug.GetTableInfo(name, info, indent)
    if (isUnityEditor == false) then
        return 'isUnityEditor == false'
    end

    if (info == nil) then
        return 'nil'
    end

    if (indent == nil) then
        indent = 0
    end

    local str = ''

    if (type(info) == 'table') then
        indent = indent + 1
        str = str .. 'table:  ' .. name .. '\n' .. string.rep('    ', indent - 1)
        for k, v in pairs(info) do
            str = str .. '\n' .. string.rep('    ', indent) .. LuaDebug.GetTableInfo(k, v, indent)
        end
        str = str .. '\n' .. string.rep('   ', indent - 1)
    else
        str = str .. name .. ':  ' .. tostring(info)
    end

    return str
end

function LuaDebug.WriteToDebugLogTxt(msg)
    if (isUnityEditor == false or CS.CSDebug.developerConsoleVisible == false) then
        return
    end

    msg = '\n\n' .. tostring(CS.CSServerTime.Now) .. '\n' .. msg
    file_util.Append(CS.System.IO.Directory.GetCurrentDirectory() .. '/DebugLog.txt', msg)
end

function LuaDebug.PrintRecivedMsgProperties(className, resData, serverProtocol)
    if (isUnityEditor == false or CS.CSDebug.developerConsoleVisible == false) then
        return
    end

    if (serverProtocol == nil) then
        serverProtocol = 'nil'
    end

    local protocolId = 0

    CS.UIDebugInfo.BeginGroup(CS.ELogToggleType.NormalMSG, "Receive XLua " .. "ID==" .. protocolId .. " Msg: " .. serverProtocol .. " 时间=" .. tostring(CS.CSServerTime.Instance.ServerNows), CS.ELogColorType.Green)
    LuaDebug.PrintMsg(className, resData, 1, CS.ELogColorType.Green)
    CS.UIDebugInfo.EndGroup()
end

function LuaDebug.PrintMsg(name, info, indent, color)
    if (info == nil) then
        return
    end

    if (indent == nil) then
        indent = 1
    end

    local indentStr = string.rep('   ', indent)

    if (type(info) == 'table') then
        CS.UIDebugInfo.AddLog(CS.ELogToggleType.NormalMSG, indentStr .. 'table : ' .. name, color)
        CS.UIDebugInfo.AddLog(CS.ELogToggleType.NormalMSG, indentStr .. '{', color)
        for k, v in pairs(info) do
            LuaDebug.PrintMsg(k, v, indent + 1, color)
        end
        CS.UIDebugInfo.AddLog(CS.ELogToggleType.NormalMSG, indentStr .. '}', color)
    else
        CS.UIDebugInfo.AddLog(CS.ELogToggleType.NormalMSG, indentStr .. name .. ' : ' .. tostring(info), color)
    end
end

function LuaDebug.PrintSendMsgProperties(protocolId, className, msg)
    if (isUnityEditor == false or CS.CSDebug.developerConsoleVisible == false) then
        return
    end

    CS.UIDebugInfo.BeginGroup(CS.ELogToggleType.NormalMSG, "Send XLua " .. "ID==" .. protocolId .. " 时间=" .. tostring(CS.CSServerTime.Instance.ServerNows), CS.ELogColorType.White)
    LuaDebug.PrintMsg(className, msg, 1, CS.ELogColorType.White)
    CS.UIDebugInfo.EndGroup()
end

function LuaDebug.Log(param, isPrintLine)
    --if not isOpenLog then
    --    return
    --end
    if param == nil then
        local info = debug.getinfo(2, "Sl");
        local s = "[LuaDebug]nil\n" .. info.source .. " " .. info.currentline .. "行\n"
        CS.UnityEngine.Debug.Log(s)
        return
    end
    if isPrintLine == nil or isPrintLine == true then
        local info = debug.getinfo(2, "Sl");
        local s = "[LuaDebug]" .. param .. "\n" .. info.source .. " " .. info.currentline .. "行\n"
        CS.UnityEngine.Debug.Log(s)
    else
        CS.UnityEngine.Debug.Log(param)
    end

end

function LuaDebug.LogError(param, isPrintLine)
    --if CS.CSDebug.developerConsoleVisible == false then
    --	return
    --end
    if isPrintLine == nil or isPrintLine == true then
        local info = debug.getinfo(2, "Sl");
        local s = info.source .. " " .. info.currentline .. "行\n" .. param
        CS.UnityEngine.Debug.LogError(s)
    else
        CS.UnityEngine.Debug.LogError(param)
    end

end
--try
--{
--	main = function()
--	end,
--	catch = function(errors)
--		--CS.UnityEngine.Debug.LogError("catch : ["..panelName.."] " .. errors)
--	end
--}
-- 异常捕获
function try(block)
    local main = block.main
    local catch = block.catch
    local finally = block.finally

    assert(main)

    -- try to call it
    local ok, errors = xpcall(main, debug.traceback)
    if not ok then
        -- run the catch function
        if catch then
            catch(errors)
        else
            luaDebug.LogError(errors, false)
        end
    end

    -- run the finally function
    if finally then
        finally(ok, errors)
    end

    -- ok?
    if ok then
        return errors
    end
end

---写网络消息到日志中
---@param msgName string 消息名
---@param msgID number 消息ID
---@param protoName string proto名
---@param luaTblData table 消息内容
---@param isToServer boolean|nil 是否为发送到服务端的消息
function LuaDebug.WriteNetMsgToLog(msgName, msgID, protoName, luaTblData, isToServer)
    if isOpenLog ~= true or CS.CSLogManager.Instance == nil then
        return
    end
    local str = "MsgName: " .. tostring(msgName) .. ",\r\nProtoName: " .. tostring(protoName) .. ";\r\nMsgContent:\r\n" .. LuaDebug.GetLuaTableContent(luaTblData)
    if isToServer then
        CS.CSLogManager.Instance.NetworkMsgLogger:Write(CS.CSLogFormatter_NetworkMsg.MsgDirection.ClientToServer, msgID, true, str, false)
    else
        CS.CSLogManager.Instance.NetworkMsgLogger:Write(CS.CSLogFormatter_NetworkMsg.MsgDirection.ServerToClient, msgID, true, str, false)
    end
end

local recursiveCallCount = 0
local maxRecursiveCallLevel = 100

---获取LuaTable内容
---@private
---@overload fun(luaTblData:table):string
---@param luaTblData table
---@param indent number|nil 缩进
---@param concatBuffer table|nil
---@return string
function LuaDebug.GetLuaTableContent(luaTblData, indent, concatBuffer, isOutputString)
    local isMainTbl = false
    if concatBuffer == nil then
        isOutputString = true
        concatBuffer = {}
    end
    if indent == nil then
        isMainTbl = true
        indent = 0
    end
    recursiveCallCount = recursiveCallCount + 1
    if recursiveCallCount > maxRecursiveCallLevel then
        if isOpenLog then
            luaDebug.LogError("Recursive Level Exceed Than " .. tostring(maxRecursiveCallLevel) .. ", Check Input LuaTable!!!")
        end
        recursiveCallCount = recursiveCallCount - 1
        return "nil,\r\n"
    end
    if luaTblData == nil then
        recursiveCallCount = recursiveCallCount - 1
        return "nil,\r\n"
    end
    local typeStr = type(luaTblData)
    --local strTemp
    if typeStr == "table" then
        if luaTblData[1] ~= nil then
            table.insert(concatBuffer, "[#")
            table.insert(concatBuffer, tostring(Utility.GetTableCount(luaTblData)))
            table.insert(concatBuffer, "]{\r\n")
        else
            table.insert(concatBuffer, "{\r\n")
        end
        for i, v in pairs(luaTblData) do
            for j = 1, indent do
                table.insert(concatBuffer, "  ")
            end
            table.insert(concatBuffer, "    ")
            table.insert(concatBuffer, tostring(i))
            table.insert(concatBuffer, ":    ")
            LuaDebug.GetLuaTableContent(v, indent + 4, concatBuffer)
        end
        for j = 1, indent do
            table.insert(concatBuffer, "  ")
        end
        table.insert(concatBuffer, "}")
    else
        if typeStr == "string" then
            table.insert(concatBuffer, "\"")
            table.insert(concatBuffer, tostring(luaTblData))
            table.insert(concatBuffer, "\"")
        else
            table.insert(concatBuffer, tostring(luaTblData))
        end
    end
    if isMainTbl then
        table.insert(concatBuffer, "\r\n")
    else
        table.insert(concatBuffer, ",\r\n")
    end
    recursiveCallCount = recursiveCallCount - 1
    if isOutputString then
        return table.concat(concatBuffer)
    end
end

return LuaDebug