---模板管理
local TemplateManager = {}

---lua组件模板列表,用于Lua中的继承
---@type table<string,TemplateBase>
luaComponentTemplates = {}

---初始化模板管理
function TemplateManager:Initialize()
    luaComponentTemplates = {}
    TemplateManager.RegisterLuaComponents()
    TemplateManager.SetAllLuaComponentMetaTable()
end

---所有需要作为组件模板的lua文件均需在此注册并加以注释,以便后续调用
function TemplateManager.RegisterLuaComponents()
    --luaComponentTemplates.copytemplatebase = require "luaRes.ui.copytemplatebase"
end


function TemplateManager.SetAllLuaComponentMetaTable()
    for k, v in pairs(luaComponentTemplates) do
        if getmetatable(v) == nil then
            --设置模板的元表为模板
            setmetatable(v, templatebase)
        end
        --设置模板的__index为自身
        v.__index = v
        v.chunkName = k
        v.__gc = TemplateManager.OnTemplateGetGCed
    end
end

return TemplateManager