package.path = package.path .. ";" .. CS.UnityEngine.Application.persistentDataPath .. "/?.lua"

---加载xlua
util = require 'xlua.util'

---@type LuaDebug
luaDebug = require 'luaRes.common.LuaDebug'

---通用
Utility = require "luaRes.common.CommonUtils"

--region 协程相关

---加载协程相关
local cs_coroutine = require 'luaRes.common.cs_coroutine'
---lua协程中调用C#的yield return方式
yield_return = cs_coroutine.yield_return
---开启C#协程方法(Lua中定义,C#中运行)
StartCoroutine = cs_coroutine.StartCoroutine
---关闭Lua中定义,在C#中运行的协程的方法
StopCoroutine = cs_coroutine.StopCoroutine

--endregion

gameMgr = require 'luaRes.scripts.LuaGameMgr'

--region UI相关

---lua的脚本注册
luaClassRegister = require 'luaRes.scripts.LuaClassRegister'

---UI给管理类,以用来进行UIL的创建/销毁/显示
uimanager = require 'luaRes.ui.UIManager'
---UI界面的基类
uibase = require 'luaRes.ui.UIBase'

templatebase = require 'luaRes.ui.temp.base.UITemplateBase'
--endregion

---初始化lua
---在C#中调用,初始化lua
function InitRequire()
    gameMgr:Initialize()
    luaClassRegister:Init()
    --游戏进入时初始化UI界面,创建第一个UI界面登录面板
    uimanager:Initialize()
end