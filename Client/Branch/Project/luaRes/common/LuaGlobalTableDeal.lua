local LuaGlobalTableDeal = {}

---@return TABLE.CFG_GLOBAL
function LuaGlobalTableDeal.GetGlobalTabl(id)
    if id == nil then
        return nil
    end
    local isfind, data = CS.Cfg_GlobalTableManager.Instance:TryGetValue(id)
    return data
end

--region召唤令快捷设置信息
---召唤令限制等级
function LuaGlobalTableDeal.ZhaoHuanLingLimitLevel()
    if LuaGlobalTableDeal.mZhaoHuanLingLimitLevel == nil then
        LuaGlobalTableDeal.SetZhaoHuanLing()
    end
    return LuaGlobalTableDeal.mZhaoHuanLingLimitLevel
end
---召唤令ID
function LuaGlobalTableDeal.ZhaoHuanLingItemID()
    if LuaGlobalTableDeal.mZhaoHuanLingItemID == nil then
        LuaGlobalTableDeal.SetZhaoHuanLing()
    end
    return LuaGlobalTableDeal.mZhaoHuanLingItemID
end

---召唤令提示气泡限制次数
function LuaGlobalTableDeal.ZhaoHuanLingTipLimitNumber()
    if LuaGlobalTableDeal.mZhaoHuanLingTipLimitNumber == nil then
        LuaGlobalTableDeal.SetZhaoHuanLing()
    end
    return LuaGlobalTableDeal.mZhaoHuanLingTipLimitNumber
end

---设置召唤令快捷设置信息
function LuaGlobalTableDeal.SetZhaoHuanLing()
    local data = LuaGlobalTableDeal.GetGlobalTabl(22464)
    if data == nil then
        return
    end
    local strList = _fSplit(data.value, "#")
    if strList ~= nil and #strList == 3 then
        LuaGlobalTableDeal.mZhaoHuanLingLimitLevel = tonumber(strList[1])
        LuaGlobalTableDeal.mZhaoHuanLingItemID = tonumber(strList[2])
        LuaGlobalTableDeal.mZhaoHuanLingTipLimitNumber = tonumber(strList[3])
    end
end

function LuaGlobalTableDeal.ZhaoHuanLingShowNameBossTypeTable()
    if LuaGlobalTableDeal.mZhaoHuanLingShowNameBossTypeTable == nil then
        LuaGlobalTableDeal.SetZhaoHuanLingShowNameBossType()
    end
    return LuaGlobalTableDeal.mZhaoHuanLingShowNameBossTypeTable
end

function LuaGlobalTableDeal.SetZhaoHuanLingShowNameBossType()
    LuaGlobalTableDeal.mZhaoHuanLingShowNameBossTypeTable = {}

    local data = LuaGlobalTableDeal.GetGlobalTabl(22491)
    if data == nil then
        return
    end
    local strList = _fSplit(data.value, "#")
    if strList ~= nil then
        for i = 1, Utility.GetLuaTableCount(strList) do
            table.insert(LuaGlobalTableDeal.mZhaoHuanLingShowNameBossTypeTable, tonumber(strList[i]))
        end
    end
end

function LuaGlobalTableDeal.ZhaoHuanLingSecondConfirmLevel()
    if LuaGlobalTableDeal.mZhaoHuanLingSecondConfirmLevel == nil then
        LuaGlobalTableDeal.SetZhaoHuanLingSecondConfirmLevel()
    end
    return LuaGlobalTableDeal.mZhaoHuanLingSecondConfirmLevel
end

function LuaGlobalTableDeal.SetZhaoHuanLingSecondConfirmLevel()
    local data = LuaGlobalTableDeal.GetGlobalTabl(22492)
    if data == nil then
        return
    end
    LuaGlobalTableDeal.mZhaoHuanLingSecondConfirmLevel = tonumber(data.value)
end

function LuaGlobalTableDeal.ZhaoHuanLingDontNeedSecondConfirmMapTable()
    if LuaGlobalTableDeal.mZhaoHuanLingDontNeedSecondConfirmMapTable == nil then
        LuaGlobalTableDeal.SetZhaoHuanLingDontNeedSecondConfirmMap()
    end
    return LuaGlobalTableDeal.mZhaoHuanLingDontNeedSecondConfirmMapTable
end

function LuaGlobalTableDeal.SetZhaoHuanLingDontNeedSecondConfirmMap()
    LuaGlobalTableDeal.mZhaoHuanLingDontNeedSecondConfirmMapTable = {}

    local data = LuaGlobalTableDeal.GetGlobalTabl(22493)
    if data == nil then
        return
    end
    local strList = _fSplit(data.value, "#")
    if strList ~= nil then
        for i = 1, Utility.GetLuaTableCount(strList) do
            table.insert(LuaGlobalTableDeal.mZhaoHuanLingDontNeedSecondConfirmMapTable, tonumber(strList[i]))
        end
    end
end

--endregion

--region 交易行
---@return table<number,number> 元宝交易限制条件，condition集合
function LuaGlobalTableDeal.GetAuctionBuyYuanBaoItemLimit()
    if LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLimit == nil then
        LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLimit = {}
        local info = LuaGlobalTableDeal.GetGlobalTabl(22470)
        if info then
            local strs = string.Split(info.value, '#')
            for i = 1, #strs do
                table.insert(LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLimit, tonumber(strs[i]))
            end
        end
    end
    return LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLimit
end

---@param type number 等级1/转生等级2
function LuaGlobalTableDeal.GetAuctionBuyYuanBaoItemLevelLimit(type)
    if LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLevelLimit == nil then
        LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLevelLimit = {}
        local info = LuaGlobalTableDeal.GetGlobalTabl(22471)
        if info then
            local strs = string.Split(info.value, '#')
            if #strs >= 2 then
                LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLevelLimit[1] = tonumber(strs[1])
                LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLevelLimit[2] = tonumber(strs[2])
            end
        end
    end
    if LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLevelLimit then
        return LuaGlobalTableDeal.mAuctionBuyYuanBaoItemLevelLimit[type]
    end
end

--endregion

--region 怪物
---查找怪物名字颜色
---@param type number 怪物类型
---@return string 怪物颜色
function LuaGlobalTableDeal.GetMonsterNameColorByType(type)

    if LuaGlobalTableDeal.mMonsterNameColorTable == nil then
        LuaGlobalTableDeal.mMonsterNameColorTable = {}
        local info = LuaGlobalTableDeal.GetGlobalTabl(20336)
        if info then
            local strs = string.Split(info.value, '&')
            for i = 1, #strs do
                local color = string.Split(strs[i], '#')
                if (#color == 2) then
                    LuaGlobalTableDeal.mMonsterNameColorTable[color[1]] = color[2]
                end
            end
        end
    end
    if (Utility.IsContainsKey(LuaGlobalTableDeal.mMonsterNameColorTable, type)) then
        return "[" .. LuaGlobalTableDeal.mMonsterNameColorTable[type] .. "]"
    else
        return "[FFFFFF]"
    end
end
--endregion

--region 怪物悬赏
---@alias MonsterArrestStepReward {itemId:number,count:number}
---@return MonsterArrestStepReward
function LuaGlobalTableDeal.GetMonsterArrestStepReward(group)
    if LuaGlobalTableDeal.mMonsterArrestStepReward == nil then
        LuaGlobalTableDeal.mMonsterArrestStepReward = {}
        local info = LuaGlobalTableDeal.GetGlobalTabl(22449)
        if info then
            local strs = string.Split(info.value, '&')
            for i = 1, #strs do
                local rewardList = string.Split(strs[i], '#')
                if #rewardList >= 3 then
                    local group = tonumber(rewardList[1])
                    local reward = {}
                    reward.itemId = tonumber(rewardList[2])
                    reward.count = tonumber(rewardList[3])
                    LuaGlobalTableDeal.mMonsterArrestStepReward[group] = reward
                end
            end
        end
    end
    return LuaGlobalTableDeal.mMonsterArrestStepReward[group]
end

function LuaGlobalTableDeal.GetMonsterArrestStepRewardBubbleTipsId(group)
    if LuaGlobalTableDeal.mMonsterArrestStepRewardBubbleTipsId == nil then
        LuaGlobalTableDeal.mMonsterArrestStepRewardBubbleTipsId = {}
    end
    local info = LuaGlobalTableDeal.GetGlobalTabl(22475)
    if info then
        local strs = string.Split(info.value, '#')
        local group = #strs / 2
        for i = 0, group - 1 do
            if (2 * i + 2) <= #strs then
                local group = tonumber(strs[2 * i + 1])
                local bubbleId = tonumber(strs[2 * i + 2])
                LuaGlobalTableDeal.mMonsterArrestStepRewardBubbleTipsId[group] = bubbleId
            end
        end
    end
    return LuaGlobalTableDeal.mMonsterArrestStepRewardBubbleTipsId[group]
end

--endregion

--region 恶魔广场
---@return boolean  判断当前时间是否需要显示加号闪烁标记
function LuaGlobalTableDeal.IsTimeNeedFlickerAddSign(time)
    if LuaGlobalTableDeal.mDevilSquareAddFlickerShowTime == nil then
        local info = LuaGlobalTableDeal.GetGlobalTabl(22484)
        if info then
            LuaGlobalTableDeal.mDevilSquareAddFlickerShowTime = tonumber(info.value)
        end
    end
    if LuaGlobalTableDeal.mDevilSquareAddFlickerShowTime then
        return time < LuaGlobalTableDeal.mDevilSquareAddFlickerShowTime
    end
    return false
end

---恶魔广场的地图组信息
function LuaGlobalTableDeal.EMOGUANGCHANGMapGroup()
    if LuaGlobalTableDeal.mEMOGUANGCHANGMapGroup == nil then
        LuaGlobalTableDeal.mEMOGUANGCHANGMapGroup = {}
        local info = LuaGlobalTableDeal.GetGlobalTabl(22459)
        if info then
            local allEMGCMApInfo = string.Split(info.value, '&')
            for i = 0, #allEMGCMApInfo do
                local mapInfo = string.Split(allEMGCMApInfo[i], '#')
                if mapInfo ~= nil and #mapInfo > 0 then
                    table.insert(LuaGlobalTableDeal.mEMOGUANGCHANGMapGroup, mapInfo)
                end
            end
        end
    end
    return LuaGlobalTableDeal.mEMOGUANGCHANGMapGroup
end

---是否是恶魔广场地图
---@return boolean
function LuaGlobalTableDeal.IsEMGCMap(mapId)
    local list = LuaGlobalTableDeal.EMOGUANGCHANGMapGroup()
    for i = 1, #list do
        if #list[i] > 2 then
            if tonumber(list[i][3]) == mapId then
                return true
            end
        end
    end
    return false
end

--endregion

--region 挑战boss
function LuaGlobalTableDeal.GetBossGoMapUnitColorTbl()
    if LuaGlobalTableDeal.mBossGoMapUnitColorTb == nil then
        local info = LuaGlobalTableDeal.GetGlobalTabl(22499)
        if info then
            LuaGlobalTableDeal.mBossGoMapUnitColorTb = string.Split(info.value, '#')
        end
    end
    return LuaGlobalTableDeal.mBossGoMapUnitColorTb
end
--endregion

--region 行会
---查看行会仇人花费
function LuaGlobalTableDeal.GetUnionEnemyCostTbl()
    if LuaGlobalTableDeal.mUnionEnemyCost == nil then
        local info = LuaGlobalTableDeal.GetGlobalTabl(22507)
        if info then
            LuaGlobalTableDeal.mUnionEnemyCost = string.Split(info.value, '#')
        end
    end
    return LuaGlobalTableDeal.mUnionEnemyCost
end
--endregion

--region 熔炼

---熔炼默认显示的奖励预览
function LuaGlobalTableDeal.GetSmeltDefaultRewardPreview()
    if LuaGlobalTableDeal.mSmeltDefaultRewardPreview == nil then
        local isFind, info = CS.Cfg_GlobalTableManager.Instance.dic:TryGetValue(22517)
        if isFind then
            LuaGlobalTableDeal.mSmeltDefaultRewardPreview = string.Split(info.value, '#')
        end
    end
    return LuaGlobalTableDeal.mSmeltDefaultRewardPreview
end

---熔炼行开启限制
function LuaGlobalTableDeal.OpenSmeltAuctionConditionList()
    if LuaGlobalTableDeal.mSmeltAuctionConditionList == nil then
        local isFind, info = CS.Cfg_GlobalTableManager.Instance.dic:TryGetValue(22524)
        if isFind then
            LuaGlobalTableDeal.mSmeltAuctionConditionList = string.Split(info.value, '#')
        end
    end
    return LuaGlobalTableDeal.mSmeltAuctionConditionList
end

---熔炼行是否开启
function LuaGlobalTableDeal.GetIsOpenSmeltAuction()
    if LuaGlobalTableDeal.OpenSmeltAuctionConditionList() ~= nil then
        for i = 1, #LuaGlobalTableDeal.OpenSmeltAuctionConditionList() do
            local conditionID = tonumber(LuaGlobalTableDeal.OpenSmeltAuctionConditionList()[i])
            if not CS.Cfg_ConditionManager.Instance:IsMainPlayerMatchCondition(conditionID) then
                return false
            end
        end
    end
    return true
end

---熔炼背包页签开启限制
function LuaGlobalTableDeal.OpenBagSmeltMarkCondition()
    if LuaGlobalTableDeal.mBagSmeltMarkCondition == nil then
        local isFind, info = CS.Cfg_GlobalTableManager.Instance.dic:TryGetValue(22525)
        if isFind then
            LuaGlobalTableDeal.mBagSmeltMarkCondition = tonumber(info.value)
        end
    end
    return LuaGlobalTableDeal.mBagSmeltMarkCondition
end

---熔炼收益tips显示信息（bg#标题）
function LuaGlobalTableDeal.GetSmeltRewardTipsViewInfo()
    if LuaGlobalTableDeal.SmeltRewardTipsViewInfo == nil then
        local info = LuaGlobalTableDeal.GetGlobalTabl(22592)
        if info then
            LuaGlobalTableDeal.SmeltRewardTipsViewInfo = string.Split(info.value, '#')
        end
    end
    return LuaGlobalTableDeal.SmeltRewardTipsViewInfo
end

--endregion

--region 天宫神剪
---获取天宫神剪deliver表信息
function LuaGlobalTableDeal.GetTianGongShenJianNpcDeliverInfoTable()
    if LuaGlobalTableDeal.TianGongShenJianNpcDeliverInfoTable == nil then
        LuaGlobalTableDeal.TianGongShenJianNpcDeliverInfoTable = {}
        local globalInfoIsFind, globalInfo = CS.Cfg_GlobalTableManager.Instance:TryGetValue(22518)
        if globalInfoIsFind then
            local deliverIdTable = string.Split(globalInfo.value, '#')
            if deliverIdTable ~= nil and type(deliverIdTable) == 'table' and #deliverIdTable > 0 then
                for k, v in pairs(deliverIdTable) do
                    local deliverTableInfoIsFind, deliverTableInfo = CS.Cfg_DeliverTableManager.Instance:TryGetValue(v)
                    if deliverTableInfoIsFind then
                        table.insert(LuaGlobalTableDeal.TianGongShenJianNpcDeliverInfoTable, deliverTableInfo)
                    end
                end
            end
        end
    end
    return LuaGlobalTableDeal.TianGongShenJianNpcDeliverInfoTable
end

---获取进入天宫神剪的花费
function LuaGlobalTableDeal.GetTianGongShenJianCost()
    local itemId, price
    local globalInfoIsFind, globalInfo = CS.Cfg_GlobalTableManager.Instance:TryGetValue(22522)
    if globalInfoIsFind then
        local costTable = string.Split(globalInfo.value, '#')
        if costTable ~= nil and type(costTable) == 'table' and #costTable > 1 then
            itemId = costTable[2]
            price = costTable[1]
        end
    end
    return itemId, price
end
--endregion

--region 奖励结算

---奖励结算显示的奖励bg
---@type table<number,string> number为奖励类型 string为bg图片名称
---
function LuaGlobalTableDeal.GetRewardBgTextureDic()
    if LuaGlobalTableDeal.mRewardBgTextureDic == nil then
        LuaGlobalTableDeal.mRewardBgTextureDic = {}
        local isFind, info = CS.Cfg_GlobalTableManager.Instance.dic:TryGetValue(22521)
        if isFind then
            local infos = string.Split(info.value, '&')
            for i = 1, #infos do
                local textureInfo = string.Split(infos[i], '#')
                if #textureInfo > 1 then
                    LuaGlobalTableDeal.mRewardBgTextureDic[tonumber(textureInfo[1])] = textureInfo[2]
                end
            end
        end
    end
    return LuaGlobalTableDeal.mRewardBgTextureDic
end
--endregion

--region 灵兽
---灵兽修炼
function LuaGlobalTableDeal.GetRemoveServantFlashTime()
    if LuaGlobalTableDeal.RemoveServantFlashTime == nil then
        local isFind, info = CS.Cfg_GlobalTableManager.Instance.dic:TryGetValue(22527)
        if isFind then
            LuaGlobalTableDeal.RemoveServantFlashTime = tonumber(info.value)
        end
    end
    return LuaGlobalTableDeal.RemoveServantFlashTime
end

function LuaGlobalTableDeal.IsOpenServantRein()
    local isFind, globalTable = CS.Cfg_GlobalTableManager.Instance.dic:TryGetValue(20233)
    if (isFind) then
        local ShowReinLevel = tonumber(string.Split(globalTable.value, "#")[1])
        if ShowReinLevel then
            for i = 0, CS.CSScene.MainPlayerInfo.ServantInfoV2.ServantInfoList.Count - 1 do
                if CS.CSScene.MainPlayerInfo.ServantInfoV2.ServantInfoList[i].level >= ShowReinLevel then
                    return true
                end
            end
        end
    end
end

---灵兽聚灵满级速率
function LuaGlobalTableDeal.GetServantGatherSoulMaxLevelSpeed()
    local isFind, info = CS.Cfg_GlobalTableManager.Instance.dic:TryGetValue(22584)
    if isFind then
        LuaGlobalTableDeal.ServantGatherSoulMaxLevelSpeed = tonumber(info.value)
    end
    return LuaGlobalTableDeal.ServantGatherSoulMaxLevelSpeed
end
--endregion

--region 商城
LuaGlobalTableDeal.StoreReplaceCoinGroup = {}

---可替换货币
function LuaGlobalTableDeal.GetStoreReplaceCoinTab()
    if LuaGlobalTableDeal.mStoreReplaceCoin == nil then
        LuaGlobalTableDeal.mStoreReplaceCoin = {}
        local info = LuaGlobalTableDeal.GetGlobalTabl(22532)
        if info then
            LuaGlobalTableDeal.StoreReplaceCoinGroup = string.Split(info.value, '&')
            for i = 1, #LuaGlobalTableDeal.StoreReplaceCoinGroup do
                local tab = string.Split(LuaGlobalTableDeal.StoreReplaceCoinGroup, '#')
                for j = 2, #tab do
                    table.insert(LuaGlobalTableDeal.mStoreReplaceCoin[tab[1]], tab[j])
                end
            end
        end
    end
    return LuaGlobalTableDeal.mStoreReplaceCoin
end
--endregion

--region 我要变强
---获取复活显示变强提示最大等级
function LuaGlobalTableDeal.GetReliveShowBianQiangHintMaxLevel()
    if LuaGlobalTableDeal.mReliveShowBianQiangHintMaxLevel == nil then
        local info = LuaGlobalTableDeal.GetGlobalTabl(22535)
        if info then
            LuaGlobalTableDeal.mReliveShowBianQiangHintMaxLevel = tonumber(info.value)
        end
    end
    return LuaGlobalTableDeal.mReliveShowBianQiangHintMaxLevel
end
--endregion

--region 技能
function LuaGlobalTableDeal.GetSkillElementShowInfo(id)
    if LuaGlobalTableDeal.mSkillElementShowInfo == nil then
        LuaGlobalTableDeal.mSkillElementShowInfo = {}
        local info = LuaGlobalTableDeal.GetGlobalTabl(22537)
        if info then
            local strs = string.Split(info.value, '&')
            for i = 1, #strs do
                local str = string.Split(strs[i], '#')
                if #str >= 2 then
                    local id = tonumber(str[1])
                    if id then
                        LuaGlobalTableDeal.mSkillElementShowInfo[id] = str[2]
                    end
                end
            end
        end
    end
    return LuaGlobalTableDeal.mSkillElementShowInfo[id]
end

---是否有任何禁止施法的buff存在在主角身上
---@return boolean
function LuaGlobalTableDeal.IsAnyAntiSkillReleaseBuffExistInMainPlayer()
    if CS.CSScene.MainPlayerInfo == nil then
        return false
    end
    ---@type CSBuffInfoV2
    local buffInfo = CS.CSScene.MainPlayerInfo.BuffInfo
    if buffInfo == nil then
        return false
    end
    return buffInfo:IsHasForbidenUseSkillBuff()
end
--endregion

--region 任务
---特殊处理原地自动提交任务
function LuaGlobalTableDeal.Task_SpecialOpenPanelSubmitTask()
    if LuaGlobalTableDeal.mTask_SpecialOpenPanelSubmitTask == nil then
        LuaGlobalTableDeal.SetSpecialOpenPanelSubmitTask()
    end
    return LuaGlobalTableDeal.mTask_SpecialOpenPanelSubmitTask
end

---设置特殊处理原地自动提交任务
function LuaGlobalTableDeal.SetSpecialOpenPanelSubmitTask()
    local data = LuaGlobalTableDeal.GetGlobalTabl(22534)
    if data == nil then
        return
    end
    local strList = _fSplit(data.value, "#")
    LuaGlobalTableDeal.mTask_SpecialOpenPanelSubmitTask = strList
end

---指定任务点击之后打开变强面板
function LuaGlobalTableDeal.Task_SpecialOpenPowPanel()
    if LuaGlobalTableDeal.mTask_SpecialOpenPowPanel == nil then
        LuaGlobalTableDeal.SetTask_SpecialOpenPowPanel()
    end
    return LuaGlobalTableDeal.mTask_SpecialOpenPowPanel
end

---设置指定任务点击之后打开变强面板
function LuaGlobalTableDeal.SetTask_SpecialOpenPowPanel()
    local data = LuaGlobalTableDeal.GetGlobalTabl(22536)
    if data == nil then
        return
    end
    local strList = _fSplit(data.value, "#")
    LuaGlobalTableDeal.mTask_SpecialOpenPowPanel = strList
end

--endregion

--region 组队
---是否需要组队提示
function LuaGlobalTableDeal.NeedTeamPrompt(isSameUnion)
    local ConditionTable
    if isSameUnion then
        if LuaGlobalTableDeal.mSameUnionCondition == nil then
            LuaGlobalTableDeal.mSameUnionCondition = {}
            local data = LuaGlobalTableDeal.GetGlobalTabl(22150)
            if data then
                local conditions = string.Split(data.value, '#')
                if #conditions >= 2 then
                    LuaGlobalTableDeal.mSameUnionCondition.MinCondition = tonumber(conditions[1])
                    LuaGlobalTableDeal.mSameUnionCondition.MaxCondition = tonumber(conditions[2])
                end
            end
        end
        ConditionTable = LuaGlobalTableDeal.mSameUnionCondition
    else
        if LuaGlobalTableDeal.mDifferentUnionCondition == nil then
            LuaGlobalTableDeal.mDifferentUnionCondition = {}
            local data = LuaGlobalTableDeal.GetGlobalTabl(22547)
            if data then
                local conditions = string.Split(data.value, '#')
                if #conditions >= 2 then
                    LuaGlobalTableDeal.mDifferentUnionCondition.MinCondition = tonumber(conditions[1])
                    LuaGlobalTableDeal.mDifferentUnionCondition.MaxCondition = tonumber(conditions[2])
                end
            end
        end
        ConditionTable = LuaGlobalTableDeal.mDifferentUnionCondition
    end
    ---@type CSMainPlayerInfo
    local mainPlayerInfo = CS.CSScene.MainPlayerInfo
    if mainPlayerInfo and ConditionTable and ConditionTable.MinCondition and ConditionTable.MaxCondition then
        return mainPlayerInfo.Level >= ConditionTable.MinCondition and mainPlayerInfo.Level <= ConditionTable.MaxCondition
    end
    return false
end
--endregion

--region 公告
---获取怪物类型与等级的限制
---@alias MonsterTypeAndLevelLimitList table<number,{playerMinLevel:number,playerMaxLevel:number,monsterLimits:table<number,{monsterType:number,monsterMinLevel:number,monsterMaxLevel:number}>}>
---@private
---@return MonsterTypeAndLevelLimitList
function LuaGlobalTableDeal:GetMonsterTypeAndLevelLimitForAnnounce()
    if self.mMonsterTypeAndLevelLimitList == nil then
        self.mMonsterTypeAndLevelLimitList = {}
        local isFind, tbl = CS.Cfg_GlobalTableManager.Instance:TryGetValue(22548)
        if isFind and tbl then
            local content = tbl.value
            local strs1 = string.Split(content, '&')
            for i = 1, #strs1 do
                local strTemp = strs1[i]
                local strs2 = string.Split(strTemp, '#')
                local tblTemp = {}
                if #strs2 > 1 then
                    tblTemp.playerMinLevel = tonumber(strs2[1])
                    tblTemp.playerMaxLevel = tonumber(strs2[2])
                    tblTemp.monsterLimits = {}
                    local index = 0
                    local monsterMinLevelTemp, monsterMaxLevelTemp, monsterType
                    for j = 3, #strs2 do
                        index = index + 1
                        if index == 1 then
                            monsterType = tonumber(strs2[j])
                        elseif index == 2 then
                            monsterMinLevelTemp = tonumber(strs2[j])
                        elseif index == 3 then
                            monsterMaxLevelTemp = tonumber(strs2[j])
                            index = 0
                            table.insert(tblTemp.monsterLimits, { monsterType = monsterType, monsterMinLevel = monsterMinLevelTemp, monsterMaxLevel = monsterMaxLevelTemp })
                        end
                    end
                end
                if tblTemp.playerMinLevel and tblTemp.playerMaxLevel then
                    table.insert(self.mMonsterTypeAndLevelLimitList, tblTemp)
                end
            end
        end
    end
    return self.mMonsterTypeAndLevelLimitList
end

---玩家等级和怪物类型、等级是否符合需要被公告屏蔽
---@param mainPlayerLevel number
---@param monsterType number
---@param monsterLevel number
---@return boolean 是否需要被公告屏蔽
function LuaGlobalTableDeal:IsPlayerLevelAndMonsterTypeLevelConfirmToAnnounce(mainPlayerLevel, monsterType, monsterLevel)
    if mainPlayerLevel == nil or monsterLevel == nil or monsterType == nil then
        return false
    end
    local tbl = self:GetMonsterTypeAndLevelLimitForAnnounce()
    for i = 1, #tbl do
        local tblTemp = tbl[i]
        if mainPlayerLevel >= tblTemp.playerMinLevel and mainPlayerLevel <= tblTemp.playerMaxLevel then
            for j = 1, #tblTemp.monsterLimits do
                local tblTemp2 = tblTemp.monsterLimits[j]
                if tblTemp2.monsterType == monsterType and monsterLevel <= tblTemp2.monsterMaxLevel and monsterLevel >= tblTemp2.monsterMinLevel then
                    return true
                end
            end
        end
    end
    return false
end
--endregion

--region 充值
function LuaGlobalTableDeal:GetSpecialFirstRebateTable()
    local isFind, tbl = CS.Cfg_GlobalTableManager.Instance:TryGetValue(22586)
    if isFind then
        local cont = string.Split(tbl.value, "#")
        return cont
    else
        return {}
    end
end
--endregion

--region 社交
---是否可以编辑自我介绍
function LuaGlobalTableDeal:CanEditorMyIntroduction()
    local globalTable = LuaGlobalTableDeal.GetGlobalTabl(22552);
    return CS.Cfg_ConditionManager.Instance:IsMainPlayerMatchCondition(tonumber(globalTable.value));
end
--endregion

--region 腕力精力相关

--region 腕力
---腕力最大值
---@return number
function LuaGlobalTableDeal:GetWristStrengthMaxValue()
    if LuaGlobalTableDeal.mWristStrengthMaxValue == nil then
        LuaGlobalTableDeal.mWristStrengthMaxValue = 0
        local info = LuaGlobalTableDeal.GetGlobalTabl(22313)
        if info then
            local strInfo = string.Split(info.value, '#')
            LuaGlobalTableDeal.mWristStrengthMaxValue = tonumber(strInfo[1])
        end
    end
    return LuaGlobalTableDeal.mWristStrengthMaxValue
end

---腕力比例尺
---@return number
function LuaGlobalTableDeal:GetWristStrengthScale()
    if LuaGlobalTableDeal.mWristStrengthScale == nil then
        LuaGlobalTableDeal.mWristStrengthScale = 1
        self:SetWristStrengthAndEnergyScale()
    end
    return LuaGlobalTableDeal.mWristStrengthScale
end

---腕力药水商品id
---@return number
function LuaGlobalTableDeal:GetWristStrengthStoreId()
    if LuaGlobalTableDeal.mWristStrengthStoreId == nil then
        LuaGlobalTableDeal.mWristStrengthStoreId = 0
        self:SetWristStrengthAndEnergyStoreId()
    end
    return LuaGlobalTableDeal.mWristStrengthStoreId
end

--endregion

--region 精力

---精力最大值
---@return number
function LuaGlobalTableDeal:GetEnergyMaxValue()
    if LuaGlobalTableDeal.mEnergyMaxValue == nil then
        LuaGlobalTableDeal.mEnergyMaxValue = 0
        local info = LuaGlobalTableDeal.GetGlobalTabl(22314)
        if info then
            local strInfo = string.Split(info.value, '#')
            LuaGlobalTableDeal.mEnergyMaxValue = tonumber(strInfo[1])
        end
    end
    return LuaGlobalTableDeal.mEnergyMaxValue
end

---精力比例尺
---@return number
function LuaGlobalTableDeal:GetEnergyScale()
    if LuaGlobalTableDeal.mEnergyScale == nil then
        LuaGlobalTableDeal.mEnergyScale = 1
        self:SetWristStrengthAndEnergyScale()
    end
    return LuaGlobalTableDeal.mEnergyScale
end

---精力药水商品id
---@return number
function LuaGlobalTableDeal:GetEnergyStoreId()
    if LuaGlobalTableDeal.mEnergyStoreId == nil then
        LuaGlobalTableDeal.mEnergyStoreId = 0
        self:SetWristStrengthAndEnergyStoreId()
    end
    return LuaGlobalTableDeal.mEnergyStoreId
end

--endregion

---设置腕力精力比例尺
function LuaGlobalTableDeal:SetWristStrengthAndEnergyScale()
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22557)
    if tableInfo then
        local info = string.Split(tableInfo.value, '#')
        if #info > 0 then
            LuaGlobalTableDeal.mWristStrengthScale = tonumber(info[1])
        end
        if #info > 1 then
            LuaGlobalTableDeal.mEnergyScale = tonumber(info[2])
        end
    end
end

---设置腕力精力商品id
function LuaGlobalTableDeal:SetWristStrengthAndEnergyStoreId()
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22558)
    if tableInfo then
        local info = string.Split(tableInfo.value, '#')
        if #info > 0 then
            LuaGlobalTableDeal.mWristStrengthStoreId = tonumber(info[1])
        end
        if #info > 1 then
            LuaGlobalTableDeal.mEnergyStoreId = tonumber(info[2])
        end
    end
end

--endregion

--region 挑战Boss新版

---得到挑战BOSS页签描述总字典
---@return table<number,table<number,BossSubTab>>
function LuaGlobalTableDeal.BossSubTabDesDic()
    if LuaGlobalTableDeal.mBossSubTabDesDic == nil then
        LuaGlobalTableDeal.mBossSubTabDesDic = LuaGlobalTableDeal.GetBossSubTabDes()
    end
    return LuaGlobalTableDeal.mBossSubTabDesDic
end

---得到Boss子页签描述列表
---@return table<number,BossSubTab>
function LuaGlobalTableDeal.GetBossSubTabList(mainType)
    if mainType ~= nil and LuaGlobalTableDeal.BossSubTabDesDic() ~= nil then
        local dic = LuaGlobalTableDeal.BossSubTabDesDic()
        local list = dic[mainType]
        return list
    end
    return nil
end

---得到子页签描述显示
---@private
function LuaGlobalTableDeal.GetBossSubTabDes()
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22568)
    if tableInfo then
        ---@type table<number,table<BossSubTab>>
        local SubTabDesDic = {}
        local temp1 = string.Split(tableInfo.value, "&")
        for i, v in pairs(temp1) do
            local temp2 = string.Split(v, "#")
            if #temp2 > 4 then
                ---@class BossSubTab
                local BossSubTab = {
                    ---类型
                    type = tonumber(temp2[2]),
                    ---类型描述
                    typeDes = temp2[3],
                    ---详细信息描述
                    detailDes = temp2[4],
                    ---特殊掉落展示
                    speDrop = LuaGlobalTableDeal.GetBossSubTabDropShow(temp2[1], temp2[2]),
                    ---页签限制
                    conditionList = string.Split(temp2[5], '_'),
                    ---默认选择页签限制
                    SelectconditionList = string.Split(temp2[6], '_')
                }
                if SubTabDesDic[tonumber(temp2[1])] == nil then
                    SubTabDesDic[tonumber(temp2[1])] = {}
                end
                table.insert(SubTabDesDic[tonumber(temp2[1])], BossSubTab)
            end
        end
        return SubTabDesDic
    end
    return nil
end
---得到子页签描述显示特殊显示
---@private
function LuaGlobalTableDeal.GetBossSubTabDropShow(type, subtype)
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22587)
    if tableInfo then
        local temp1 = string.Split(tableInfo.value, "&")
        for i, v in pairs(temp1) do
            local temp2 = string.Split(v, "#")
            if #temp2 == 3 then
                if (type == temp2[1] and subtype == temp2[2]) then
                    local idList = CS.System.Collections.Generic["List`1[System.Int32]"]()
                    idList:Add(temp2[3])
                    return Utility.GetBossPanelDropListByList(idList)
                end
            end
        end
    end
    return nil
end
---获取boss页签
---@private
function LuaGlobalTableDeal.GetBossAllPageInfoList()
    if LuaGlobalTableDeal.mBossAllPageInfoList == nil then
        LuaGlobalTableDeal.mBossAllPageInfoList = {}
        local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22573)
        if tableInfo then
            local allPage = string.Split(tableInfo.value, '&')
            if #allPage > 0 then
                for i = 1, #allPage do
                    local info = string.Split(allPage[i], '#')
                    if info and #info > 1 then
                        table.insert(LuaGlobalTableDeal.mBossAllPageInfoList, info)
                    end
                end
            end
        end
    end
    return LuaGlobalTableDeal.mBossAllPageInfoList
end

--endregion

--region 魔之boss
---获取击杀魔之boss最大次数
function LuaGlobalTableDeal.GetAttackMagicBossMaxNum()
    if LuaGlobalTableDeal.mAttackMagicBossMaxNum == nil then
        LuaGlobalTableDeal.InitAttackMagicBossMaxNum()
    end
    return LuaGlobalTableDeal.mAttackMagicBossMaxNum
end

---初始化击杀魔之boss最大次数
function LuaGlobalTableDeal.InitAttackMagicBossMaxNum()
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22570)
    if tableInfo then
        local info = string.Split(tableInfo.value, '#')
        if #info > 0 then
            LuaGlobalTableDeal.mAttackMagicBossMaxNum = tonumber(info[1])
        end
        if #info > 1 then
            LuaGlobalTableDeal.mMagicBossReliveTime = tonumber(info[2])
        end
    end
end

---获取协助次数最大值
function LuaGlobalTableDeal.GetXieZhuMaxNum()
    if LuaGlobalTableDeal.mXieZhuMaxNum == nil then
        local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22571)
        if tableInfo then
            LuaGlobalTableDeal.mXieZhuMaxNum = tonumber(tableInfo.value)
        end
    end
    return LuaGlobalTableDeal.mXieZhuMaxNum
end

---获取添加击杀次数材料itemid
function LuaGlobalTableDeal.GetAddKillNumMaterialItemId()
    if LuaGlobalTableDeal.mAddKillNumMaterialItemId == nil then
        local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22577)
        if tableInfo then
            LuaGlobalTableDeal.mAddKillNumMaterialItemId = tonumber(tableInfo.value)
        end
    end
    return LuaGlobalTableDeal.mAddKillNumMaterialItemId
end

---主角是否满足显示魔之boss红点
---@return boolean 是否显示魔之boss红点
function LuaGlobalTableDeal.ShowMagicBossRedPointByConditionTable()
    if LuaGlobalTableDeal.mMagicBossRedPointConditionTable == nil then
        LuaGlobalTableDeal.InitMagicBossRedPointConditionTable()
    end
    if LuaGlobalTableDeal.mMagicBossRedPointConditionTable ~= nil then
        return CS.Cfg_ConditionManager.Instance:IsMainPlayerMatchConditionList(LuaGlobalTableDeal.mMagicBossRedPointConditionTable)
    end
    return false
end

---初始化魔之boss的红点condition表
function LuaGlobalTableDeal.InitMagicBossRedPointConditionTable()
    if LuaGlobalTableDeal.mMagicBossRedPointConditionTable == nil then
        local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22581)
        if tableInfo then
            LuaGlobalTableDeal.mMagicBossRedPointConditionTable = string.Split(tableInfo.value, '#')
            LuaGlobalTableDeal.mMagicBossRedPointConditionTable = Utility.ChangeNumberTable(LuaGlobalTableDeal.mMagicBossRedPointConditionTable)
        end
    end
    return LuaGlobalTableDeal.mMagicBossRedPointConditionTable
end

---主角是否满足显示魔之boss红点
---@return boolean 是否显示魔之boss闪烁气泡
function LuaGlobalTableDeal.ShowMagicBossFlashByConditionTable()
    if LuaGlobalTableDeal.mMagicBossFlashConditionTable == nil then
        LuaGlobalTableDeal.InitMagicBossFlashConditionTable()
    end
    if LuaGlobalTableDeal.mMagicBossFlashConditionTable ~= nil then
        return CS.Cfg_ConditionManager.Instance:IsMainPlayerMatchConditionList(LuaGlobalTableDeal.mMagicBossFlashConditionTable)
    end
    return false
end

---初始化魔之boss的闪烁气泡condition表
function LuaGlobalTableDeal.InitMagicBossFlashConditionTable()
    if LuaGlobalTableDeal.mMagicBossFlashConditionTable == nil then
        local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22588)
        if tableInfo then
            LuaGlobalTableDeal.mMagicBossFlashConditionTable = string.Split(tableInfo.value, '#')
            LuaGlobalTableDeal.mMagicBossFlashConditionTable = Utility.ChangeNumberTable(LuaGlobalTableDeal.mMagicBossFlashConditionTable)
        end
    end
    return LuaGlobalTableDeal.mMagicBossFlashConditionTable
end

---获取魔之boss奖励表（策划要求所有的魔之boss的奖励都一样，有问题找高翔）
function LuaGlobalTableDeal.GetMagicBossRewardTable()
    if LuaGlobalTableDeal.MagicBossRewardTable ~= nil then
        return LuaGlobalTableDeal.MagicBossRewardTable
    end
    LuaGlobalTableDeal.MagicBossRewardTable = {}
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22580)
    if tableInfo ~= nil then
        local rewardTable = string.Split(tableInfo.value,'&')
        if rewardTable ~= nil and type(rewardTable) == 'table' then
            for k,v in pairs(rewardTable) do
                local strTable = string.Split(v,'#')
                if strTable ~= nil and type(strTable) == 'table' and #strTable > 1 then
                    local rewardInfoTable = {}
                    rewardInfoTable.itemId = strTable[1]
                    rewardInfoTable.count = strTable[2]
                    table.insert(LuaGlobalTableDeal.MagicBossRewardTable,rewardInfoTable)
                end
            end
        end
    end
    return LuaGlobalTableDeal.MagicBossRewardTable
end

---获取魔之boss范围数据
function LuaGlobalTableDeal.GetMagicBossRangeNumber()
    if LuaGlobalTableDeal.MagicBossRange ~= nil then
        return LuaGlobalTableDeal.MagicBossRange
    end
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22575)
    if tableInfo ~= nil then
        LuaGlobalTableDeal.MagicBossRange = tonumber(tableInfo.value)
    end
    return LuaGlobalTableDeal.MagicBossRange
end

---获取魔之boss自动领取奖励时间(秒)
function LuaGlobalTableDeal.GetMagicBossAutoGetRewardTime()
    if LuaGlobalTableDeal.MagicBossAutoGetRewardTime ~= nil then
        return LuaGlobalTableDeal.MagicBossAutoGetRewardTime
    end
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22598)
    if tableInfo ~= nil then
        LuaGlobalTableDeal.MagicBossAutoGetRewardTime = tonumber(tableInfo.value)
    end
    return LuaGlobalTableDeal.MagicBossAutoGetRewardTime
end

---获取魔之boss开服天数conditionId
function LuaGlobalTableDeal.GetMagicBossOpenServerDayConditionId()
    if LuaGlobalTableDeal.MagicBossOpenServerDayConditionId ~= nil then
        return LuaGlobalTableDeal.MagicBossOpenServerDayConditionId
    end
    local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22600)
    if tableInfo ~= nil then
        LuaGlobalTableDeal.MagicBossOpenServerDayConditionId = tonumber(tableInfo.value)
    end
    return LuaGlobalTableDeal.MagicBossOpenServerDayConditionId
end
--endregion

--region 宝箱
---宝箱使用的vip提示是否显示
function LuaGlobalTableDeal:IsSpecialBoxUseVIPTipsShow(mainPlayerLevel, itemID)
    if mainPlayerLevel == nil or itemID == nil then
        return false
    end
    if self.mSpecialBoxUseVIPTipsShowConditions == nil then
        ---@type TABLE.CFG_GLOBAL
        local globalTbl
        ___, globalTbl = CS.Cfg_GlobalTableManager.Instance:TryGetValue(22579)
        if globalTbl and globalTbl.value ~= nil then
            self.mSpecialBoxUseVIPTipsShowConditions = {}
            local strs = string.Split(globalTbl.value, '&')
            for i = 1, #strs do
                local strs2 = string.Split(strs[i], '#')
                if #strs2 >= 2 then
                    local itemIDTemp = tonumber(strs2[1])
                    local vipLevelTemp = tonumber(strs2[2])
                    self.mSpecialBoxUseVIPTipsShowConditions[itemIDTemp] = vipLevelTemp
                end
            end
        end
    end
    if self.mSpecialBoxUseVIPTipsShowConditions then
        local maxVIPLevel = self.mSpecialBoxUseVIPTipsShowConditions[itemID]
        if maxVIPLevel then
            return mainPlayerLevel <= maxVIPLevel
        else
            return false
        end
    else
        return false
    end
end
--endregion

--region 狼烟梦境
---xp技能对应的狼烟梦境的时间系数
function LuaGlobalTableDeal:LYMJTimeValueOfXpSkillIdDic()
    if self.mLYMJTimeValueOfXpSkillIdDic == nil then
        self.mLYMJTimeValueOfXpSkillIdDic = {}
        local tableInfo = LuaGlobalTableDeal.GetGlobalTabl(22578)
        if tableInfo then
            local lymjAllXpInfo = string.Split(tableInfo.value, '&')
            for i = 1, #lymjAllXpInfo do
                local lymhXpInfo = string.Split(lymjAllXpInfo[i], '#')
                if #lymhXpInfo > 1 then
                    self.mLYMJTimeValueOfXpSkillIdDic[tonumber(lymhXpInfo[1])] = tonumber(lymhXpInfo[3]) / 10000
                end
            end
        end
    end
    return self.mLYMJTimeValueOfXpSkillIdDic
end
--endregion

return LuaGlobalTableDeal