---@class UIManager
local UIManager = {}

---@type table 已打开的UI界面
UIManager.UIGameObjects = {}

---@type table 正在加载中的UI界面
UIManager.UILoadingGameObject = {}
---@type table 正在加载中的UI界面回调
UIManager.UILoadingCallbackDic = {}
---@type table 正在加载中的UI界面显示参数
UIManager.UIShowingParamsDic = {}

---等待打开的面板字典
---@type table<string, {paramList:table<number,any>}>
UIManager.WaitForOpenDic = {};

---初始化
function UIManager:Initialize()
    self:CreatePanel("UILoginPanel")
end

---创建面板
---@param panelName 面板名称
---@param action 回调方法
function UIManager:CreatePanel(panelName, action, ...)
    --region 检查界面是否打开
    local uiPanel = UIManager.UIGameObjects[panelName]
    --如果当前面板已经打开,那么直接执行面板的Show方面,并指向action回调
    if (uiPanel) then
        uiPanel:Show(...)
        if (action) then
            local res, error = pcall(action, uiPanel)
            if res == false then
                CS.UnityEngine.Debug.LogError(tostring(error))
            end
        end

        return
    end
    --endregion

    ---定义打开面板的结构
    local panelParam = {};
    panelParam.panelName = panelName;
    panelParam.action = action;
    panelParam.params = { ... };

    if (UIManager.WaitForOpenDic[panelName] ~= nil) then
        table.insert(UIManager.WaitForOpenDic[panelName].paramList, panelParam);
    else
        UIManager.WaitForOpenDic[panelName] = {};
        UIManager.WaitForOpenDic[panelName].paramList = {};
        table.insert(UIManager.WaitForOpenDic[panelName].paramList, panelParam);
    end

    if (UIManager.mCoroutineOpenPanel == nil) then
        UIManager.mCoroutineOpenPanel = StartCoroutine(UIManager.COpenPanel, UIManager)
    end
end

function UIManager:COpenPanel()
    ---只要等待加载的面板字典不为空,那么始终去尝试创建
    while (UIManager.WaitForOpenDic ~= nil) do
        coroutine.yield(0);

        local panelName = nil

        --拿到的等待队列里面table最前面的面板名字
        for i, v in pairs(UIManager.WaitForOpenDic) do
            panelName = i
            break
        end

        if panelName ~= nil then
            local panelParamsTemp = UIManager.WaitForOpenDic[panelName]

            while UIManager.WaitForOpenDic[panelName] ~= nil
                    and UIManager.WaitForOpenDic[panelName] == panelParamsTemp
                    and panelParamsTemp.paramList ~= nil
                    and #panelParamsTemp.paramList > 0 do

                local paramTemp = panelParamsTemp.paramList[1]
                table.remove(panelParamsTemp.paramList, 1)
                try {
                    main = function()
                        --print("Create panel", panelName, paramTemp.panelName, CS.UnityEngine.Time.frameCount)
                        UIManager:CreatePanelLogic(paramTemp.panelName, paramTemp.action, table.unpack(paramTemp.params));
                    end,
                    catch = function(error)
                        CS.UnityEngine.Debug.LogError(error);
                    end
                }
                coroutine.yield(0);
            end
            --print("Finish Create", panelName, CS.UnityEngine.Time.frameCount)
            if UIManager.WaitForOpenDic[panelName] == panelParamsTemp then
                ---如果队列中的界面打开数据与之前缓冲的一致,则认为该数据已经处理完毕,可以从队列中移除,否则认为期间进行了复杂的处理,如Create-Close-Create这样的操作,导致队列中当前的界面打开数据与刚才处理的不一致,不应当移除
                UIManager.WaitForOpenDic[panelName] = nil
            end
        end
    end
    UIManager.mCoroutineOpenPanel = nil;
end

---在lua中创建UI界面
---@param panelName string UI界面名
---@param action function 创建完毕回调
function UIManager:CreatePanelLogic(panelName, action, ...)
    --若界面正在被调用加载
    if UIManager.UILoadingGameObject[panelName] == true then
        --若界面已被加入加载队列,则替换界面加载回调和显示参数
        UIManager.UILoadingCallbackDic[panelName] = action
        UIManager.UIShowingParamsDic[panelName] = { ... }
        return
    end

    --检查界面是否已经打开
    local uiPanel = UIManager.UIGameObjects[panelName]

    if (uiPanel) then
        uiPanel:Show(...)
        if (action) then
            local res, error = pcall(action, uiPanel)
            if res == false then
                CS.UnityEngine.Debug.LogError(tostring(error))
            end
        end

        return
    end

    --若界面未被调用加载,则将界面名称加入正在加载队列中
    UIManager.UILoadingGameObject[panelName] = true
    UIManager.UILoadingCallbackDic[panelName] = action
    UIManager.UIShowingParamsDic[panelName] = { ... }

    UIManager.StartCreatePanel(panelName)
    --local co = coroutine.create(UIManager.StartCreatePanel)
    --assert(coroutine.resume(co, panelName))
end

function UIManager.StartCreatePanel(panelName)
    local go = CS.UIManager.Instance:CreatePanelByName(panelName);

    --if (CS.StaticUtility.IsNull(go) == false) then
    if (true) then
        --若该界面已从加载队列中清除,则不实例化该界面,并删掉之前拷贝的GameObject
        if UIManager.UILoadingGameObject[panelName] == nil then
            CS.UnityEngine.GameObject.Destroy(go)
            return
        end
        go.name = panelName
        local luaBehav = Utility.InitLuaBehaviour(go, panelName)

        ---此处现将luabehaviour组件置为不使能,因为后面还有1~2帧的延迟后才进入初始化流程
        luaBehav.enabled = false
        --yield_return(0)
        --若该界面已从加载队列中清除,则不实例化该界面,并删掉之前拷贝的GameObject
        if UIManager.UILoadingGameObject[panelName] == nil then
            CS.UnityEngine.GameObject.Destroy(go)
            return
        end
        UIManager.UILoadingGameObject[panelName] = false
        if (luaBehav and CS.StaticUtility.IsNull(luaBehav) == false) then
            local uiPanel = luaBehav:GetLuaTable()
            UIManager.UIGameObjects[panelName] = uiPanel
            if (uiPanel) then
                --没有模板则继承uibase,有模板则在UIPanelConfig查看
                if getmetatable(uiPanel) == nil then
                    setmetatable(uiPanel, uibase)
                end

                --设置界面的GameObject
                uiPanel.go = go
                --设置层级
                CS.UILayerManager.Instance:SetLayer(go, uiPanel:Get_PanelLayerType());

                local goIsActive = go.activeSelf
                if goIsActive == true and (CS.StaticUtility.IsNull(go) == false) then
                    go:SetActive(false)
                end
                go:SetActive(true)
                --若该界面已从加载队列中清除,则不实例化该界面,并删掉之前拷贝的GameObject
                if true then
                    --yield_return(0)
                    local isGODestroyed = CS.StaticUtility.IsNull(go)
                    if UIManager.UILoadingGameObject[panelName] == nil or isGODestroyed then
                        if isGODestroyed == false then
                            CS.UnityEngine.GameObject.Destroy(go)
                        end
                        return
                    end
                end
                luaBehav.enabled = true
                --判断uiPanel，防止打开此界面后马上关闭，导致uiPanel为nil
                if (uiPanel and CS.StaticUtility.IsNull(go) == false) then
                    --设置界面的GameObject
                    uiPanel.go = go
                    --设置界面名
                    uiPanel._PanelName = panelName
                    try {
                        main = function()
                            uiPanel:Init()
                            if uiPanel.IsNeedActiveDuringInitialize == false then
                                --yield_return(0)
                            end
                            if (UIManager.UIShowingParamsDic[panelName] == nil) then
                                uiPanel:Show();
                            else
                                uiPanel:Show(table.unpack(UIManager.UIShowingParamsDic[panelName]))
                            end
                            --uiPanel._PanelState = LuaEnumUIState.Normal;
                        end,
                        catch = function(errors)
                            CS.UnityEngine.Debug.LogError("catch : [" .. panelName .. "] " .. errors)
                        end
                    }
                    uiPanel.IsInitializing = false
                    local action = UIManager.UILoadingCallbackDic[panelName]
                    if (action) then
                        local res, error = pcall(action, uiPanel)
                        if res == false then
                            CS.UnityEngine.Debug.LogError(tostring(error))
                        end
                    end
                else
                    uimanager.UIGameObjects[panelName] = nil
                end
            end
        end
    end

    --从调用中队列和加载回调字典中移除panelName
    --UIManager:ClearUIPanelOpenRequest(panelName)
end

---关闭面板
---@param panelName 面板名称
function UIManager:ClosePanel(panelName, isForceClose)
    if (UIManager.WaitForOpenDic ~= nil) then
        if isForceClose then
            UIManager.WaitForOpenDic[panelName] = nil
        else
            local paramsDataTemp = UIManager.WaitForOpenDic[panelName]
            if paramsDataTemp ~= nil and paramsDataTemp.paramList ~= nil then
                local count = #paramsDataTemp.paramList
                if count > 0 then
                    table.remove(paramsDataTemp.paramList, count)
                end
                if (#paramsDataTemp.paramList == 0) then
                    UIManager.WaitForOpenDic[panelName] = nil;
                end
            end
        end
    end

    local uiPanel = UIManager.UIGameObjects[panelName];
    if (uiPanel and uiPanel.go) then
        CS.UnityEngine.GameObject.Destroy(uiPanel.go)
    end
    UIManager.UIGameObjects[panelName] = nil;
end

return UIManager