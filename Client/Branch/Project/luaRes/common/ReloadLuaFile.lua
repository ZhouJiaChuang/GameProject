---避免重新加载的文件名table
AvoidAutoReloadFileNameTable = {
    'luaRes.main_out',
    'luaRes.ui.UIManager'
}

---重新加载修改过的lua文件
---@param modifiedLuaFileArray CS.System.String[]
function ReloadModifiedLuaFiles(modifiedLuaFileArray)
    for i = 0, modifiedLuaFileArray.Length - 1 do
        SafelyReloadSingleLuaFile(modifiedLuaFileArray[i])
    end
    --重新加载后需要刷新模板管理
    templatemanager:Initialize()
    --重新加载后需要class控制管理
    classCtrl:Initialize()
    --重新加载后需要刷新ui面板配置
    uipanelconfig:Initialize()
end

---安全地重新加载单个lua文件
---@param luaFilePath string lua文件名
function SafelyReloadSingleLuaFile(luaFilePath)
    --若该文件不在避免自动重载列表中且已被加载过
    if Utility.IsContainsValue(AvoidAutoReloadFileNameTable, luaFilePath) == false and package.loaded[luaFilePath] ~= nil then
        --重新加载lua文件
        local reversedTable = package.loaded[luaFilePath]
        local reloadRes = pcall(ReloadLuaFile, luaFilePath)
        --若加载错误则使用之前的table
        if reloadRes == false then
            --print("Reload failed:  " .. luaFilePath)
            package.loaded[luaFilePath] = reversedTable
        else
            --print("Reload succeed:  " .. luaFilePath)
            OnLuaFileSafelyReloaded(luaFilePath, reversedTable, package.loaded[luaFilePath])
        end
    end
end

---重新加载lua文件
function ReloadLuaFile(luaFilePath)
    package.loaded[luaFilePath] = nil
    require(luaFilePath)
end

---lua文件安全加载完毕事件
---@param luaFilePath 被重新加载的lua文件地址
---@param previewTable 重新加载之前的表
---@param newTable 重新加载之后的表
function OnLuaFileSafelyReloaded(luaFilePath, previewTable, newTable)
    if package.loaded and luaFilePath and previewTable and newTable then
        --在package.loaded中遍历,若package.loaded中有元素将其元表设置为重新加载前的表,则将其更正为重新加载后的表
        for i, v in pairs(package.loaded) do
            if i ~= luaFilePath and v and getmetatable(v) == previewTable then
                setmetatable(v, newTable)
            end
        end
    end
end