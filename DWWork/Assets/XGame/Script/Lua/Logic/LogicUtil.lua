--------------------------------------------------------------------------------
--   File      : LogicUtil.lua
--   author    : guoliang
--   function   : 逻辑工具类
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
LogicUtil = {}
local _s = LogicUtil
require "Logic.MJCardLogic.CMJCard"
local CardAnalysisHelper = require "Area.DouDiZhu.NormalDDZ.Container.Module.DDZCardAnalysisHelperModule"
_s.DDZCardAnalysisHelper = CardAnalysisHelper.New()

_s.MJCardSize =
    {
        [SeatPosEnum.South] = UnityEngine.Vector3.New(-87.5, 0, 0),
        [SeatPosEnum.East] = UnityEngine.Vector3.New(0, -28, 0),
        [SeatPosEnum.North] = UnityEngine.Vector3.New(50, 0, 0),
        [SeatPosEnum.West] = UnityEngine.Vector3.New(0, 28, 0),
    }

_s.MJRecordSize =
    {
        [SeatPosEnum.South] = UnityEngine.Vector3.New(-72, 0, 0),
        [SeatPosEnum.East] = UnityEngine.Vector3.New(0, -34, 0),
        [SeatPosEnum.North] = UnityEngine.Vector3.New(49, 0, 0),
        [SeatPosEnum.West] = UnityEngine.Vector3.New(0, 34, 0),
    }

_s.MJCardDepth =
    {
        [SeatPosEnum.South] = 0,
        [SeatPosEnum.East] = 5,
        [SeatPosEnum.North] = 0,
        [SeatPosEnum.West] = -5,
    }

_s.MJCardBg_record =
    {
        [SeatPosEnum.South] = {"gameuilayer_pp_yellow", "gameuilayer_pp_blue", "gameuilayer_pp_green"},
        [SeatPosEnum.East] = {"gameuilayer_tpcm_yellow", "gameuilayer_tpcm_blue", "gameuilayer_tpcm_green"},
        [SeatPosEnum.North] = {"gameuilayer_pp_yellow", "gameuilayer_pp_blue", "gameuilayer_pp_green"},
        [SeatPosEnum.West] = {"gameuilayer_tpcm_yellow", "gameuilayer_tpcm_blue", "gameuilayer_tpcm_green"},
    }

_s.MJCardBg_normal =
    {
        [SeatPosEnum.South] = {"gameuilayer_zm_yellow", "gameuilayer_zm_blue", "gameuilayer_zm_green"},
        [SeatPosEnum.East] = {"gameuilayer_cm_yellow", "gameuilayer_cm_blue", "gameuilayer_cm_green"},
        [SeatPosEnum.North] = {"gameuilayer_bm_yellow", "gameuilayer_bm_blue", "gameuilayer_bm_green"},
        [SeatPosEnum.West] = {"gameuilayer_cm_yellow", "gameuilayer_cm_blue", "gameuilayer_cm_green"},
    }

_s.MJCardBg_outShow =
    {
        [SeatPosEnum.South] = {"gameuilayer_pp_yellow", "gameuilayer_pp_blue", "gameuilayer_pp_green"},
        [SeatPosEnum.East] = {"gameuilayer_pp_yellow", "gameuilayer_pp_blue", "gameuilayer_pp_green"},
        [SeatPosEnum.North] = {"gameuilayer_pp_yellow", "gameuilayer_pp_blue", "gameuilayer_pp_green"},
        [SeatPosEnum.West] = {"gameuilayer_pp_yellow", "gameuilayer_pp_blue", "gameuilayer_pp_green"},
    }

_s.MJGangCardBg = {"gameuilayer_bm_yellow", "gameuilayer_bm_blue", "gameuilayer_bm_green"}

------------------------------------------扑克牌工具函数区------------------------------------------
-- 手牌排序 升序或者降序
function LogicUtil.SortCards(cards, isUp)
    local compareFunc = nil
    if isUp then -- 升序
        compareFunc = function(card1, card2)
            if card1.logicValue < card2.logicValue then
                return true
            elseif card1.logicValue == card2.logicValue then
                return card1.ID > card2.ID
            else
                return false
            end
        end
    else -- 降序
        compareFunc = function(card1, card2)
            if card1.logicValue > card2.logicValue then
                return true
            elseif card1.logicValue == card2.logicValue then
                return card1.ID > card2.ID
            else
                return false
            end
        end
    end
    table.sort(cards, compareFunc)
    return cards
end

--检查炸弹
function LogicUtil.CheckHasBomb(cards)
    local analysisStruct = BaseWSKCardAnalysisStruct:New()
    analysisStruct:Analysis(cards)

    return analysisStruct.fourCount > 0 
    or analysisStruct.fiveCount > 0 
    or analysisStruct.sixCount > 0  
    or analysisStruct.sevenCount > 0  
    or analysisStruct.eightCount > 0 
end

-- 手牌排序，根据炸弹
function LogicUtil.SortCardsByBomb(cards)
    local cardList = LogicUtil.SortCards(cards, false)
    local bombTable = {}-- 放置炸弹牌
    for i = 1, 16 do
        table.insert(bombTable, {})
    end
    
    -- 讲相同的牌放到相同的分组里
    for i = #cards, 1, -1 do
        local insertIndex = math.min(cards[i].logicValue, 16)
        table.insert(bombTable[insertIndex], 1, cards[i])
    end
    
    -- 去除空的分组
    for i = #bombTable, 1, -1 do
        if #bombTable[i] == 0 then
            table.remove(bombTable, i)
        end
    end
    
    -- 分组排序
    table.sort(bombTable, function(list1, list2)
        local len_1
        local len_2
        if (list1[1].logicValue == 16 or list1[1].logicValue == 17) and #list1 > 2 then
            len_1 = #list1 * 2 + 0.5
        else
            len_1 = #list1
        end
        
        if (list2[1].logicValue == 16 or list2[1].logicValue == 17) and #list2 > 2 then
            len_2 = #list2 * 2 + 0.5
        else
            len_2 = #list2
        end
        
        if len_1 >= 4 and len_2 >= 4 then
            -- 两个炸弹
            if len_1 ~= len_2 then
                return len_1 > len_2
            else
                return list1[1].logicValue > list2[1].logicValue
            end
        elseif len_1 >= 4 or len_2 >= 4 then
            -- 一个炸弹
            return len_1 > len_2
        else
            -- 没有炸弹
            return list1[1].logicValue > list2[1].logicValue
        end
    end)
    
    local result = {}
    for k, v in ipairs(bombTable) do
        for key, value in ipairs(v) do
            table.insert(result, value)
        end
    end
    
    return result
end

--检查有没有510K
function LogicUtil.CheckHasWSK(cards)
    local analysisStruct = BaseWSKCardAnalysisStruct:New()
    analysisStruct:Analysis(cards)
    return analysisStruct.ftkCount > 0 or analysisStruct.ftkBigCount > 0
end

-- 510K优先的炸弹排序
function LogicUtil.SortCardsByWSK(cards)
    local analysisStruct = BaseWSKCardAnalysisStruct:New()
    analysisStruct:Analysis(cards)
    local wskCardList = {}
    local newCards = {}
    if analysisStruct.ftkCount > 0 or analysisStruct.ftkBigCount > 0 then
        for k, v in ipairs(cards) do
            if v.logicValue == 5 or v.logicValue == 10 or v.logicValue == 13 then
                table.insert(wskCardList, v)
            else
                table.insert(newCards, v)
            end
        end
        table.sort(wskCardList,function (x,y)
            if x.logicValue == y.logicValue then
                return false
            else
                return x.logicValue < y.logicValue
            end
        end)
    end
    
    if #wskCardList >= 3 then
        local bombList = LogicUtil.SortCardsByBomb(newCards)
        for i = 1, #wskCardList do
            table.insert(bombList, 1, wskCardList[i])
        end

        return bombList
    else
        return LogicUtil.SortCardsByBomb(cards)
    end
end

--是否有红心五
function LogicUtil.CheckHasBombYH(cards)
    local heartsFivePos = {}
    for k, v in ipairs(cards) do
		-- 是红心5
		if v.logicValue == 5 and v.color == 3 then
			table.insert(heartsFivePos, k)
		end
    end
   return #heartsFivePos == 2 or LogicUtil.CheckCardsByBomb(cards)
end

-- 宜黄红心五根据炸弹排序
function LogicUtil.SortCardsByBombYH(cards)
	local heartsFivePos = {}
	for k, v in ipairs(cards) do
		-- 是红心5
		if v.logicValue == 5 and v.color == 3 then
			table.insert(heartsFivePos, k)
		end
	end
	
	if #heartsFivePos == 2 then
		local heartsFive = {}
		for i = #heartsFivePos, 1, -1 do
			table.insert(heartsFive, cards[heartsFivePos[i]])
			table.remove(cards, heartsFivePos[i])
		end
	
		local bombList = LogicUtil.SortCardsByBomb(cards)
		for i = 1, #heartsFive do
			table.insert(bombList, 1, heartsFive[i])
		end

		return bombList
	else
		return LogicUtil.SortCardsByBomb(cards)
	end
end

-- 斗地主根据炸弹排序
function LogicUtil.SortCardsByBombDDZ(cards)
	local rocketPos = {}
	for k, v in ipairs(cards) do
		if v.logicValue == 16 or v.logicValue == 17 then
			table.insertZ(rocket, k)
		end
	end

	if #rocket ==2 then
		local rocket = {}
		for k, v in ipairs(rocketPos) do
			table.insert(rocket, cards[v])
			table.remove(cards, v)
		end
		rocket = LogicUtil.SortCards(rocket, true)

		local bombList = LogicUtil.SortCardsByBomb(cards)

		for i = 1, #rocket do
			table.insert(bombList, 1, rocket[i])
		end
		return bombList
	else
		return LogicUtil.SortCardsByBomb(cards)
	end
end

-- 出牌排序
function LogicUtil.SortOutCards(cards)
    local sortCards = {}
    local analysisHelper = CardAnalysisHelperModule:New()
    local cardType, analysisResult = analysisHelper:GetCardType(cards)
    
    if cardType == CardTypeEnum.THREE
        or cardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE
        or cardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO
        or cardType == CardTypeEnum.CT_THREE_LINE then

        cards = LogicUtil.SortCards(cards, false)
        local srcCardMap = {}
        for k, v in pairs(cards) do
            srcCardMap[v.seq] = v
        end
        for k, v in pairs(analysisResult.threeCards) do
            for k1, v1 in pairs(v) do
                table.insert(sortCards, 1, v1)
                srcCardMap[v1.seq] = nil
            end
        end
        
        for k, v in pairs(srcCardMap) do
            table.insert(sortCards, v)
        end
    else
        sortCards = LogicUtil.SortCards(cards, false)
    end
    return sortCards
end

-- 出牌顺序斗地主
function LogicUtil.SortOutCardsDDZ(cards)
	local sortCards = {}
	local cardType = _s.DDZCardAnalysisHelper:GetCardType(cards)

	if cardType == NewCardTypeEnum.NCCT_THREE_ADD_ONE or
	   cardType == NewCardTypeEnum.NCCT_THREE_ADD_DOUBLE or
	   cardType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE or
	   cardType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_DOUBLE or
	   cardType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO or
	   cardType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO_DOUBLE then
		local analysisResult = _s.DDZCardAnalysisHelper:GetCardAnalysisStruct()
		for k, v in ipairs(analysisResult:GetFourCards()) do
			for key, value in ipairs(v) do
				table.insert(sortCards, value)
			end
		end

		for k, v in ipairs(analysisResult:GetThreeCards()) do
			for key, value in ipairs(v) do
				table.insert(sortCards, value)
			end
		end

		for k, v in ipairs(analysisResult:GetDoubleCards()) do
			for key, value in ipairs(v) do
				table.insert(sortCards, value)
			end
		end

		for k, v in ipairs(analysisResult:GetSingleCards()) do
			for key, value in ipairs(v) do
				table.insert(sortCards, value)
			end
		end
	else
		sortCards = LogicUtil.SortCards(cards, false)
	end

	return sortCards
end

-- 初始化单张扑克牌显示(可以显示牌背)(针对card_item)
function LogicUtil.InitPKCard(luaWindowRoot, trans, card, index)
	if card then
		-- 显示牌面
		luaWindowRoot:SetActive(luaWindowRoot:GetTrans(trans, "CardBack"), false)
		LogicUtil.InitCardItem(luaWindowRoot, trans, card, index)
	else
		-- 显示牌背
		LogicUtil.InitCardBack(luaWindowRoot, trans, index)
	end
end

-- 显示牌背
function LogicUtil.InitCardBack(luaWindowRoot, trans, index)
	local CardBackTrans = luaWindowRoot:GetTrans(trans, "CardBack")
	luaWindowRoot:SetActive(CardBackTrans, true)
	
	index = index or 0
	local depth = index * 5
	luaWindowRoot:SetDepth(trans, depth)
	luaWindowRoot:SetDepth(CardBackTrans, depth +1)

	local lordTag = luaWindowRoot:GetTrans(trans, "LordTag")
	if lordTag then
		luaWindowRoot:SetActive(lordTag, false)
	end
end

-- 初始化单张扑克卡牌显示(针对card_item)
function LogicUtil.InitCardItem(luaWindowRoot, trans, card, index,saveShadow)
    local depth = index * 5
    luaWindowRoot:SetDepth(trans, depth)
    local normalTrans = luaWindowRoot:GetTrans(trans, "normal_root")
    local specialTrans = luaWindowRoot:GetTrans(trans, "special_root")
    local numTrans
    local smallIcoTrans
    local bigIcoTrans
    local maskTrans = luaWindowRoot:GetTrans(trans, "ico_mask")
    local shadowTrans = luaWindowRoot:GetTrans(trans, "shadow")
    local tagShadow = luaWindowRoot:GetTrans(trans, "tagShadow")    --理牌颜色遮罩
    local ico_jiaoPai = luaWindowRoot:GetTrans(trans, "ico_jiaoPai")

    if card.ID >= 83 then
        luaWindowRoot:SetActive(normalTrans, false)
        luaWindowRoot:SetActive(specialTrans, true)
        bigIcoTrans = luaWindowRoot:GetTrans(specialTrans, "ico_big_title")
        luaWindowRoot:SetSprite(bigIcoTrans, card.ico_big)
        luaWindowRoot:SetDepth(bigIcoTrans, depth + 1)
    else
        luaWindowRoot:SetActive(normalTrans, true)
        luaWindowRoot:SetActive(specialTrans, false)
        numTrans = luaWindowRoot:GetTrans(normalTrans, "ico_num_one")
        smallIcoTrans = luaWindowRoot:GetTrans(normalTrans, "ico_small_title")
        bigIcoTrans = luaWindowRoot:GetTrans(normalTrans, "ico_big_title")
        luaWindowRoot:SetSprite(numTrans, card.ico_num)
        luaWindowRoot:SetSprite(smallIcoTrans, card.ico_big)
        luaWindowRoot:SetSprite(bigIcoTrans, card.ico_big)
        
        luaWindowRoot:SetDepth(numTrans, depth + 1)
        luaWindowRoot:SetDepth(smallIcoTrans, depth + 1)
        luaWindowRoot:SetDepth(bigIcoTrans, depth + 1)
    end
    luaWindowRoot:SetDepth(ico_jiaoPai, depth + 1)
    luaWindowRoot:SetDepth(tagShadow, depth)

    luaWindowRoot:SetDepth(maskTrans, depth + 3)
    luaWindowRoot:SetActive(maskTrans, false)
    luaWindowRoot:SetDepth(shadowTrans, depth + 3)
    if not saveShadow then
        luaWindowRoot:SetActive(shadowTrans, false)
    end


    local friendCardID = LogicUtil.GetFriendCardID()
    if friendCardID and friendCardID == card.ID then
        luaWindowRoot:SetActive(ico_jiaoPai, true)
    else
        luaWindowRoot:SetActive(ico_jiaoPai, false)
    end
    if card.typeTag > 0 then
        DwDebug.LogError("card.ID = "..card.ID .. " typeTag > 0")
        luaWindowRoot:SetActive(tagShadow, true)
        luaWindowRoot:SetColor(tagShadow,LogicUtil.GetTagTypeColor(card.typeTag))
    else
        luaWindowRoot:SetActive(tagShadow, false)
    end
end

--颜色值
local tageTypeColor = {"38CECDC3","CBCC18C3"}
--根据typeTag 获取对应颜色
function LogicUtil.GetTagTypeColor(typeTag)
    local count = #tageTypeColor
    typeTag = math.ceil(typeTag % #tageTypeColor) + 1
    if tageTypeColor[typeTag] ~= nil then
        return tageTypeColor[typeTag]
    end
    return "38CECDC3"
end

--获得找朋友的牌
function LogicUtil.GetFriendCardID()
    local logicCtrl = PlayGameSys.GetPlayLogic()
    if logicCtrl and logicCtrl.roomObj then
        return logicCtrl.roomObj.friendCardID
    end
    return nil
end

-- 初始化单张扑克牌显示(可以显示牌背)(针对card_item_2)
function LogicUtil.InitPKCard_2(luaWindowRoot, trans, card, index, isShowLordTag, isShowOpenTag)
	if card then
		-- 显示牌面
		luaWindowRoot:SetActive(luaWindowRoot:GetTrans(trans, "CardBack"), false)
		LogicUtil.InitCardItem_2(luaWindowRoot, trans, card, index, isShowLordTag, isShowOpenTag)
	else
		-- 显示牌背
		LogicUtil.InitCardBack(luaWindowRoot, trans, index)
	end
end

-- 初始化单张扑克卡牌显示(针对card_item_2)
function LogicUtil.InitCardItem_2(luaWindowRoot, trans, card, index, isShowLordTag, isShowOpenTag)
	local depth = index * 5
	isShowLordTag = isShowLordTag or false
	isShowOpenTag = isShowOpenTag or false
    luaWindowRoot:SetDepth(trans, depth)
    local normalTrans = luaWindowRoot:GetTrans(trans, "normal_root")
    local specialTrans = luaWindowRoot:GetTrans(trans, "special_root")
    local maskTrans = luaWindowRoot:GetTrans(trans, "ico_mask")
    local shadowTrans = luaWindowRoot:GetTrans(trans, "shadow")
	local lordTagTrans = luaWindowRoot:GetTrans(trans, "LordTag")
	local openTagTrans = luaWindowRoot:GetTrans(trans, "OpenTag")
    if card.ID >= 83 then
        luaWindowRoot:SetActive(normalTrans, false)
        luaWindowRoot:SetActive(specialTrans, true)
        local bigIcoTrans = luaWindowRoot:GetTrans(specialTrans, "ico_big_title")
        luaWindowRoot:SetSprite(bigIcoTrans, card.ico_big)
        luaWindowRoot:SetDepth(bigIcoTrans, depth + 1)
    else
        luaWindowRoot:SetActive(normalTrans, true)
        luaWindowRoot:SetActive(specialTrans, false)
		local numTrans_1 = luaWindowRoot:GetTrans(normalTrans, "ico_num_1")
		local numTrans_2 = luaWindowRoot:GetTrans(normalTrans, "ico_num_2")
		local titleTrans_1 = luaWindowRoot:GetTrans(normalTrans, "ico_title_1")
		local titleTrans_2 = luaWindowRoot:GetTrans(normalTrans, "ico_title_2")
        luaWindowRoot:SetSprite(numTrans_1, card.ico_num)
		luaWindowRoot:SetSprite(numTrans_2, card.ico_num)
        luaWindowRoot:SetSprite(titleTrans_1, card.ico_big)
		luaWindowRoot:SetSprite(titleTrans_2, card.ico_big)
        
        luaWindowRoot:SetDepth(numTrans_1, depth + 1)
		luaWindowRoot:SetDepth(numTrans_2, depth + 1)
        luaWindowRoot:SetDepth(titleTrans_1, depth + 1)
		luaWindowRoot:SetDepth(titleTrans_2, depth + 1)
    end
	
	if lordTagTrans then
		luaWindowRoot:SetActive(lordTagTrans, isShowLordTag)
		luaWindowRoot:SetDepth(lordTagTrans, depth +2)
	end
	if openTagTrans then
		luaWindowRoot:SetActive(openTagTrans, isShowOpenTag)
		luaWindowRoot:SetDepth(openTagTrans, depth +2)
	end
    luaWindowRoot:SetDepth(maskTrans, depth + 3)
    luaWindowRoot:SetActive(maskTrans, false)
    luaWindowRoot:SetDepth(shadowTrans, depth + 3)
    --luaWindowRoot:SetActive(shadowTrans, false)
end

-- 初始化单张扑克牌显示(可以显示牌背)(针对card_item_small)
function LogicUtil.InitPKCard_3(luaWindowRoot, trans, card, index, isShowLordTag)
	if card then
		-- 显示牌面
		luaWindowRoot:SetActive(luaWindowRoot:GetTrans(trans, "CardBack"), false)
		LogicUtil.InitSmallCardItem(luaWindowRoot, trans, card, index, isShowLordTag)
	else
		-- 显示牌背
		LogicUtil.InitCardBack(luaWindowRoot, trans, index)
	end
end

-- 初始化单张扑克卡牌显示(针对card_item_small)
function LogicUtil.InitSmallCardItem(luaWindowRoot, trans, card, index, isShowLordTag)
	local depth = index * 5
	isShowLordTag = isShowLordTag or false
    luaWindowRoot:SetDepth(trans, depth)
    local normalTrans = luaWindowRoot:GetTrans(trans, "normal_root")
    local specialTrans = luaWindowRoot:GetTrans(trans, "special_root")
    local maskTrans = luaWindowRoot:GetTrans(trans, "ico_mask")
    local shadowTrans = luaWindowRoot:GetTrans(trans, "shadow")
	local lordTagTrans = luaWindowRoot:GetTrans(trans, "LordTag")
    if card.ID >= 83 then
        luaWindowRoot:SetActive(normalTrans, false)
        luaWindowRoot:SetActive(specialTrans, true)
        local bigIcoTrans = luaWindowRoot:GetTrans(specialTrans, "ico_big_title")
        luaWindowRoot:SetSprite(bigIcoTrans, card.ico_big)
        luaWindowRoot:SetDepth(bigIcoTrans, depth + 1)
    else
        luaWindowRoot:SetActive(normalTrans, true)
        luaWindowRoot:SetActive(specialTrans, false)
		local numTrans_1 = luaWindowRoot:GetTrans(normalTrans, "ico_num_1")
		local titleTrans_1 = luaWindowRoot:GetTrans(normalTrans, "ico_title_1")
        luaWindowRoot:SetSprite(numTrans_1, card.ico_num)
        luaWindowRoot:SetSprite(titleTrans_1, card.ico_big)
        
        luaWindowRoot:SetDepth(numTrans_1, depth + 1)
        luaWindowRoot:SetDepth(titleTrans_1, depth + 1)
    end
	if lordTagTrans then
		luaWindowRoot:SetDepth(lordTagTrans, depth +2)
		luaWindowRoot:SetActive(lordTagTrans, isShowLordTag)
	end
    luaWindowRoot:SetDepth(maskTrans, depth + 3)
    luaWindowRoot:SetActive(maskTrans, false)
    luaWindowRoot:SetDepth(shadowTrans, depth + 3)
    luaWindowRoot:SetActive(shadowTrans, false)
end

function LogicUtil.SetCardLordTag(luaWindowRoot, cardTrans, state)
	local lordTagTrans = luaWindowRoot:GetTrans(cardTrans, "LordTag")
	if lordTagTrans then
		if state ~= true then
			state = false
		end

		luaWindowRoot:SetActive(lordTagTrans, state)
	end
end

function LogicUtil.SetCardOpenTag(luaWindowRoot, cardTrans, state)
	local openTagTrans = luaWindowRoot:GetTrans(cardTrans, "OpenTag")
	if openTagTrans then
		if state ~= true then
			state = false
		end
	
		luaWindowRoot:SetActive(openTagTrans, state)
	end
end

-- 出牌后调整卡牌间距和transform改名（点击牌需要索引）
function LogicUtil.AdjustCards(cardCount, totalLen, cardItemTrans)
    if cardCount <= 0 then
        return
    end
    local eachCardShowLen = totalLen / (cardCount - 1)--第一张牌不需要间隔
    --  print("AdjustCards eachCardShowLen "..eachCardShowLen)
    local startPos = -totalLen / 2
    if cardCount > 15 then --需要挤压放牌
        for k, v in pairs(cardItemTrans) do
            --          print("newPos "..(startPos+eachCardShowLen*(k-1)))
            v.trans.localPosition = UnityEngine.Vector3.New(startPos + eachCardShowLen * (k - 1), 0, 0)
            v.trans.name = k .. "_cardItem"
        end
    else -- 往中间靠拢收牌
        eachCardShowLen = totalLen / 14
        startPos = -(totalLen - (15 - cardCount) * eachCardShowLen) / 2
        for k, v in pairs(cardItemTrans) do
            --print("15 newPos "..(startPos+eachCardShowLen*(k-1)))
            v.trans.localPosition = UnityEngine.Vector3.New(startPos + eachCardShowLen * (k - 1), 0, 0)
            v.trans.name = k .. "_cardItem"
        end
    end
end


-- 出牌的卡牌间距调整（主要是东西座位会有两行）
function LogicUtil.AdjustCardsForOutCards(cardCount, totalLen, cardItemTrans, seatPos)
    if seatPos == SeatPosEnum.South or seatPos == SeatPosEnum.North then
        return LogicUtil.AdjustCards(cardCount, totalLen, cardItemTrans)
    end
    
    if cardCount <= 0 then
        return
    end
    local offset_y = 100 -- 两行的y偏移
    local offset_y_new = 100
    local lineNum = 1
    if cardCount > 14 then
        lineNum = 2
    end
    local numLimit = 14
    local trueTotalLen = totalLen / 2
    local eachCardShowLen = trueTotalLen / (numLimit - 1)--第一张牌不需要间隔，count -1
    --  print("AdjustCardsForOutCards eachCardShowLen "..eachCardShowLen .. "trueTotalLen "..trueTotalLen)
    local startPos = -trueTotalLen / 2
    if lineNum == 1 then
        -- 往中间靠拢收牌
        if cardCount < 8 then
            eachCardShowLen = trueTotalLen / 7 -- 固定最大的间距为7张牌的时候
            startPos = -(trueTotalLen - (7 - cardCount) * eachCardShowLen) / 2
        else
            eachCardShowLen = trueTotalLen / (cardCount - 1)
        end
        
        offset_y_new = offset_y / 2
        for k, v in pairs(cardItemTrans) do
            --          print("13 newPos "..(startPos+eachCardShowLen*(k-1)))
            v.trans.localPosition = UnityEngine.Vector3.New(startPos + eachCardShowLen * (k - 1), offset_y_new, 0)
            v.trans.name = k .. "_cardItem"
        end
    else
        --不需要往中间收缩
        local startIndex = 1
        for k, v in pairs(cardItemTrans) do
            if k <= numLimit then
                offset_y_new = offset_y
                startIndex = k
            else
                startIndex = k - numLimit -- 第二行起點刷新
                offset_y_new = 0
            end
            v.trans.localPosition = UnityEngine.Vector3.New(startPos + eachCardShowLen * (startIndex - 1), offset_y_new, 0)
            v.trans.name = k .. "_cardItem"
        end
    end
end


-- 查找牌组中同ID的牌
function LogicUtil.FindSameColorCards(handCards)
    local sameCards = {}
    local lastCard = nil
    for k, v in pairs(handCards) do
        
        if lastCard and lastCard.ID == v.ID then
            sameCards[lastCard.seq] = lastCard
            sameCards[v.seq] = v
            lastCard = nil
        else
            lastCard = v
        end
    end
    
    return sameCards
end

-- 获取发牌动画时牌的位置
function LogicUtil.GetHandCardPosWhenDealCard(posType, cardCount, totalLen, index)-- posType 0 南北方向  1 东西方向
    local eachCardShowLen = totalLen / (cardCount - 1)--第一张牌不需要间隔
    local startPos = -totalLen / 2
    if posType == 0 then
        return UnityEngine.Vector3.New(startPos + eachCardShowLen * (index - 1), 0, 0)
    else
        
        local numLimit = 14
        local offset_y = 100 -- 两行的y偏移
        local offset_y_new = 100
        local startIndex = 1
        local trueTotalLen = totalLen / 2
        eachCardShowLen = trueTotalLen / (numLimit - 1)--第一张牌不需要间隔，count -1
        startPos = -trueTotalLen / 2
        if index <= numLimit then
            offset_y_new = offset_y
            startIndex = index
        else
            startIndex = index - numLimit -- 第二行起點刷新
            offset_y_new = 0
        end
        return UnityEngine.Vector3.New(startPos + eachCardShowLen * (startIndex - 1), offset_y_new, 0)
    end
end

-- 设置扑克场景背景
function LogicUtil.GetPKBgType(playID)
    if playID == nil then
        playID = PlayGameSys.GetNowPlayId()
    end
    return PlayerDataRefUtil.GetInt("FightBg".."_"..playID, 1)
end

-- 获取扑克场景背景
function LogicUtil.SetPKBgType(bgIndex,playID)
    if playID == nil then
        playID = PlayGameSys.GetNowPlayId()
    end
    PlayerDataRefUtil.SetInt("FightBg".."_"..playID, bgIndex)
end

---------------------------------------------------麻将公共函数区----------------------------------------------
--通用麻将设置函数
function LogicUtil.InitMJCardItem(luaWindowRoot, trans, mjCard, floor, depthCeff)-- floor 第几层 depthCeff 深度修正
    local icoTrans = luaWindowRoot:GetTrans(trans, "ico")
    local maskTrans = luaWindowRoot:GetTrans(trans, "ico_mask")
    local baseDepth = 100
    if floor and floor > 1 then
        baseDepth = baseDepth + 50 * (floor - 1)
    end
    if depthCeff then
        baseDepth = baseDepth - depthCeff
    end
    luaWindowRoot:SetDepth(trans, baseDepth)
    
    if icoTrans then
        luaWindowRoot:SetDepth(icoTrans, baseDepth + 1)
        luaWindowRoot:SetSprite(icoTrans, mjCard.ico)
    end
    
    if maskTrans then
        luaWindowRoot:SetDepth(maskTrans, baseDepth + 2)
        luaWindowRoot:SetActive(maskTrans, false)
    end
    
    local back = luaWindowRoot:GetTrans(trans, "Back")
    if back then
        luaWindowRoot:SetDepth(back, baseDepth + 3)
        luaWindowRoot:SetActive(back, false)
    end
end

function LogicUtil.InitMJBgCardItem(luaWindowRoot, trans, floor, depthCeff)-- floor 第几层 depthCeff 深度修正
    local baseDepth = 100
    if floor and floor > 1 then
        baseDepth = baseDepth + 50 * (floor - 1)
    end
    if depthCeff then
        baseDepth = baseDepth - depthCeff
    end
    luaWindowRoot:SetDepth(trans, baseDepth)
end

function LogicUtil.GetMJCardsPos(index, totalCount, seatPos, isRecordorTanPai)
    local jiangPai = 14 - totalCount - (2 - totalCount % 3)
    -- 派牌位和其他牌之间间隔
    local offset = 0.35
    local startIndex = 14 - index - jiangPai
    startIndex = (startIndex == 0 and startIndex or startIndex + offset)
    local size = (isRecordorTanPai and _s.MJRecordSize[seatPos] or _s.MJCardSize[seatPos])
    return size * startIndex
end

-- 麻将设置位置
function LogicUtil.AdjustMJCards(trans, index, totalCount, seatPos, isRecordorTanPai)
    trans.localPosition = _s.GetMJCardsPos(index, totalCount, seatPos, isRecordorTanPai)
    if SeatPosEnum.East == seatPos and not isRecordorTanPai then
        trans.localEulerAngles = UnityEngine.Vector3.New(0, 180, 0)
    else
        trans.localEulerAngles = UnityEngine.Vector3.New(0, 0, 0)
    end
    
    -- 需要更新名字
    trans.name = index .. "_cardItem"
end

-- 手牌排序 升序或者降序
function LogicUtil.SortMJCards(cards, isUp)
    local compareFunc = nil
    if isUp then -- 升序
        compareFunc = function(card1, card2)
            return card1.ID < card2.ID
        end
    else -- 降序
        compareFunc = function(card1, card2)
            return card1.ID > card2.ID
        end
    end
    table.sort(cards, compareFunc)
    
    return cards
end

-- 手牌排序 升序或者降序
function LogicUtil.SortMJCardsByItems(cards, isUp)
    local compareFunc = nil
    if isUp then -- 升序
        compareFunc = function(card1, card2)
            if nil == card1.cardInfo or nil == card1.cardInfo.ID then
                DwDebug.LogError("sort mj card error")
                return false
            end
            if nil == card2.cardInfo or nil == card2.cardInfo.ID then
                DwDebug.LogError("sort mj card error")
                return true
            end
            return card1.cardInfo.ID < card2.cardInfo.ID
        end
    else -- 降序
        compareFunc = function(card1, card2)
            if nil == card1.cardInfo or nil == card1.cardInfo.ID then
                DwDebug.LogError("sort mj card error")
                return true
            end
            if nil == card2.cardInfo or nil == card2.cardInfo.ID then
                DwDebug.LogError("sort mj card error")
                return false
            end
            return card1.cardInfo.ID > card2.cardInfo.ID
        end
    end
    table.sort(cards, compareFunc)
    for i, v in ipairs(cards) do
        v.trans.name = i .. "_cardItem"
    end
    return cards
end

-- 获取打出的牌的层数和行数、列数
function LogicUtil.GetMJOutCardShowPos(seatPos, curCardCount)
    --垂直方向（南北方位）
    local vecRow = 3 -- 3行
    local vecCol = 8 -- 10列
    local floorNum_vec = 24 --一层麻将总数
    -- 水平方向（东西方位）
    local horRow = 3 -- 4行
    local horCol = 6 -- 7列
    local floorNum_hor = 18 -- 一层麻将总数
    
    local result = {}
    local remainNum = 0
    local newCardCount = curCardCount + 1
    
    if seatPos == SeatPosEnum.South or seatPos == SeatPosEnum.North then
        result.floor = math.ceil(newCardCount / floorNum_vec)
        result.row = math.floor(((newCardCount-1) / vecCol) % vecRow)+1
        result.col = newCardCount - (result.floor - 1) * floorNum_vec - (result.row - 1) * vecCol
    else
        result.floor = math.ceil(newCardCount / floorNum_hor)
        result.row = math.floor(((newCardCount-1) / horCol) % horRow)+1
        result.col = newCardCount - (result.floor - 1) * floorNum_hor - (result.row - 1) * horCol
    end
    return result
end

-- 初始化一张正常打出的牌，包括显示和位置
function LogicUtil.SetNoramlOutMJCardShow(seatPos, luaWindowRoot, trans, mjCard, posResult)
    if trans == nil then
        
        end
    
    local posX = 0
    local posY = 0
    if seatPos == SeatPosEnum.South then
        posX = (posResult.col - 1) * 73 -- 73 UI上x轴实际间距
        posY = (posResult.row - 1) * 89 --89 UI上y轴实际间距
        _s.InitMJCardItem(luaWindowRoot, trans, mjCard, posResult.floor, posResult.row)
    elseif seatPos == SeatPosEnum.North then
        posX = (posResult.col - 1) * -73 -- 73 UI上x轴实际间距
        posY = (posResult.row - 1) * -89 --89 UI上y轴实际间距
        _s.InitMJCardItem(luaWindowRoot, trans, mjCard, posResult.floor, -posResult.row)
    elseif seatPos == SeatPosEnum.East then
        posX = (posResult.row - 1) * -73 -- 73 UI上x轴实际间距
        posY = (posResult.col - 1) * 89 --89 UI上y轴实际间距
        _s.InitMJCardItem(luaWindowRoot, trans, mjCard, posResult.floor, posResult.col)
    else
        posX = (posResult.row - 1) * 73 -- 73 UI上x轴实际间距
        posY = (posResult.col - 1) * -89 --89 UI上y轴实际间距
        _s.InitMJCardItem(luaWindowRoot, trans, mjCard, posResult.floor, -posResult.col)
    end
    
    if posResult.floor > 1 then
        posY = posY + 13 * (posResult.floor - 1)
    end
    
    trans.localPosition = UnityEngine.Vector3.New(posX, posY, 0)
end

--初始化一张碰杠吃等牌，包括显示和位置
function LogicUtil.SetOwnOutMJCardShow(seatPos, luaWindowRoot, trans, sort, mjCard, cardSort, showBack)-- sort 代表当前第几组吃碰杠了
    local startPosX = 0
    local startPosY = 0
    local selfPosX = 0
    local tempSort = cardSort
    if tempSort == 4 then
        tempSort = 2
    end
    if seatPos == SeatPosEnum.South then
        startPosX = (sort - 1) * 242
        selfPosX = startPosX + (tempSort - 1) * 73
    elseif seatPos == SeatPosEnum.North then
        startPosX = (sort - 1) * -242
        selfPosX = startPosX + (tempSort - 1) * -73
    elseif seatPos == SeatPosEnum.East then
        startPosY = (sort - 1) * -110
        selfPosX = startPosX + (tempSort - 1) * -73
    else
        startPosY = (sort - 1) * -110
        selfPosX = startPosX + (tempSort - 1) * 73
    end
    
    if cardSort == 4 then
        trans.localPosition = UnityEngine.Vector3.New(selfPosX, startPosY + 16, 0)
        if showBack then
            _s.InitMJBgCardItem(luaWindowRoot, trans, 2)
        else
            _s.InitMJCardItem(luaWindowRoot, trans, mjCard, 2)
        end
    else
        trans.localPosition = UnityEngine.Vector3.New(selfPosX, startPosY, 0)
        if showBack then
            _s.InitMJBgCardItem(luaWindowRoot, trans, 1)
        else
            _s.InitMJCardItem(luaWindowRoot, trans, mjCard, 1)
        end
    end
end

--初始化一张碰杠吃等牌 在小结算面板和在操作提醒面板上
function LogicUtil.SetMJCardAtrr(luaWindowRoot, trans, paiId, seqId, posX, posY, cardSort, isAnGang, isSelfShow, isHandCard)
    
    local isBack
    
    if cardSort == 4 then
        isBack = false
    else
        isBack = isAnGang
    end
    
    local trans = _s.CreateMJCardItem(luaWindowRoot, trans, seqId, isBack, isHandCard)
    
    require "Logic.MJCardLogic.CMJCard"
    local mjCard = CMJCard.New()
    mjCard:Init(paiId, seqId)
    
    if cardSort == 4 then
        trans.localPosition = UnityEngine.Vector3.New(posX, posY + 15, 0)
        if isAnGang and not isSelfShow then
            _s.InitMJBgCardItem(luaWindowRoot, trans, 2, -300)
            _s.ChangeSingleMJPaiGangBg(luaWindowRoot, LogicUtil.GetMJPaiType(), trans)
        else
            _s.InitMJCardItem(luaWindowRoot, trans, mjCard, 2, -300)
            _s.ChangeSingleMJPaiBg(luaWindowRoot, LogicUtil.GetMJPaiType(), SeatPosEnum.South, trans, isHandCard and 1 or 3)
        end
    else
        trans.localPosition = UnityEngine.Vector3.New(posX, posY, 0)
        if isAnGang then
            _s.InitMJBgCardItem(luaWindowRoot, trans, 1, -300)
            _s.ChangeSingleMJPaiGangBg(luaWindowRoot, LogicUtil.GetMJPaiType(), trans)
        else
            _s.InitMJCardItem(luaWindowRoot, trans, mjCard, 1, -300)
            _s.ChangeSingleMJPaiBg(luaWindowRoot, LogicUtil.GetMJPaiType(), SeatPosEnum.South, trans, isHandCard and 1 or 3)
        end
    end
    return trans
end

function LogicUtil.CreateMJCardItem(luaWindowRoot, parentTrans, seqId, isBg, isHandCard)
    local resName = "mjcard_out_show_item"
    if isBg then
        resName = "mjcard_bg_out_show_item"
    elseif isHandCard then
        resName = "mjcard_south_item"
    end
    local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, resName, RessStorgeType.RST_Never, false)
    if not resObj then
        return
    end
    LogicUtil.EnableTouch(resObj.transform, false)
    local trans = resObj.transform
    trans.parent = parentTrans
    luaWindowRoot:SetActive(trans, true)
    trans.name = seqId .. "_cardItem"
    trans.localScale = UnityEngine.Vector3.New(1, 1, 1)
    return trans
end

-- 设置单个牌背面
function LogicUtil.ChangeSingleMJPaiBg(luaWindowRoot, titleIndex, seatPos, trans, isNormal)--isNormal 1 -正常模式 2-回放模式 3-出牌状态
    if not titleIndex or not seatPos then
        return
    end
    
    local spriteName
    if isNormal == 1 then
        spriteName = _s.MJCardBg_normal[seatPos][titleIndex]
        -- 刷新背景盖牌图片
        if SeatPosEnum.South == seatPos then
            local back = luaWindowRoot:GetTrans(trans, "Back")
            if back then
                -- 设置成杠牌的背景
                luaWindowRoot:SetSprite(back, _s.MJGangCardBg[titleIndex])
            end
        end
    elseif isNormal == 2 then
        spriteName = _s.MJCardBg_record[seatPos][titleIndex]
    else
        spriteName = _s.MJCardBg_outShow[seatPos][titleIndex]
    end
    luaWindowRoot:SetSprite(trans, spriteName)
end

-- 设置所有手牌背面
function LogicUtil.ChangeMJPaiBgs(luaWindowRoot, titleIndex, seatPos, cardTranItems, isNormal)--isNormal 1 -正常模式 2-回放模式 3-出牌状态
    if titleIndex then
        local spriteName
        if isNormal == 1 then
            spriteName = _s.MJCardBg_normal[seatPos][titleIndex]
            if SeatPosEnum.South == seatPos then
                local name = _s.MJGangCardBg[titleIndex]
                for k, cardTransItem in pairs(cardTranItems) do
                    local back = luaWindowRoot:GetTrans(cardTransItem.trans, "Back")
                    luaWindowRoot:SetSprite(back, name)
                end
            end
        elseif isNormal == 2 then
            spriteName = _s.MJCardBg_record[seatPos][titleIndex]
        else
            spriteName = _s.MJCardBg_outShow[seatPos][titleIndex]
        end
        for k, cardTransItem in pairs(cardTranItems) do
            luaWindowRoot:SetSprite(cardTransItem.trans, spriteName)
        end
    end
end

-- 设置暗杠时牌背时
function LogicUtil.ChangeSingleMJPaiGangBg(luaWindowRoot, titleIndex, trans)
    if not titleIndex then
        return
    end
    
    local spriteName = _s.MJGangCardBg[titleIndex]
    luaWindowRoot:SetSprite(trans, spriteName)
end

function LogicUtil.EnableTouch(trans, flag)
    -- local boxes = trans:GetComponentsInChildren(UnityEngine.BoxCollider.GetClassType())
    -- logError("enable touch " .. boxes.Length)
    -- for i=0,boxes.Length-1 do
    --  boxes[i].enabled = flag
    -- end
    local box = trans:GetComponent("BoxCollider")
    
    if box then
        box.enabled = flag
    end
    
    -- lua这个组件也disable掉
    local com_base = trans:GetComponent("ComponentBaseLua")
    
    if com_base then
        com_base.enabled = flag
    end
end

-- 获取排序key
function LogicUtil.GetSortIndex(playerList)
    local sortKey = SeatPosSort
    local sortIndexs = {}

    local playerMgr = PlayGameSys.GetPlayLogic().roomObj.playerMgr
    for k, v in pairs(playerList) do
        local userID = type(v) == "table" and v.userId or v
        local player = playerMgr:GetPlayerByPlayerID(userID)
        if player then
            local seatPos = player.seatPos
            if seatPos then
                sortIndexs[sortKey[seatPos]] = k
            end
        end
    end
    return sortIndexs
end

-- 设置扑克场景背景
function LogicUtil.GetMJBgType()
    return PlayerDataRefUtil.GetInt("MJFightBg", 2)
end

-- 获取扑克场景背景
function LogicUtil.SetMJBgType(bgIndex)
    PlayerDataRefUtil.SetInt("MJFightBg", bgIndex)
end

-- 设置麻将牌背景
function LogicUtil.GetMJPaiType()
    -- 添加本地缓存，不免多次读取
    if nil == _s.mjPaiIndex then
        _s.mjPaiIndex = PlayerDataRefUtil.GetInt("MJPaiType", 3)
    end
    return _s.mjPaiIndex
end

-- 获取麻将牌背景
function LogicUtil.SetMJPaiType(bgIndex)
    _s.mjPaiIndex = bgIndex
    PlayerDataRefUtil.SetInt("MJPaiType", bgIndex)
end

-- 获取麻将单双击选项
function LogicUtil.GetMJSingleClickType()
    if nil == _s.m_singleClick then
        _s.m_singleClick = PlayerDataRefUtil.GetInt("MJSingleClickType", 2)
    end
    
    return _s.m_singleClick
end

function LogicUtil.IsMJSingleClick()
    return _s.GetMJSingleClickType() == 1
end

function LogicUtil.SetMJSingleClickType(index)
    _s.m_singleClick = index
    PlayerDataRefUtil.SetInt("MJSingleClickType", index)
end

-- 麻将的组合的布局类型
local mj_gen_layoutType = {
    Peng = 1,
    Chi = 2,
    MingGang = 3, -- 明杠
    SelfAnGang = 4, -- 自己视角的暗杠
    OtherAnGang = 5, -- 别人视角的暗杠
}
-- 水平间距
local mj_gen_spacing = 71
-- 资源名字
local mj_gen_res_common_str = "gameuilayer_" --gameuilayer_zm_%Color%
local mj_gen_res_color_str = {"yellow", "blue", "green"}
--[[
@desc
生成一个组合
@parm
(ctx 上下文) (layoutType 组合类型) (cardIds 组合内容) (depth 深度基准值(组合牌第4张再加10))
@return
返回对象根节点的transform
]]
function LogicUtil.GenCardGroup(ctx, layoutType, cardIds, depth)
    depth = depth or 1000 -- 1000 强行置顶哈哈
    local go = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "mj_group", RessStorgeType.RST_Never, false)
    if not go then
        DwDebug.Log("LogicUtil.GenCardGroup error :no go")
        return
    end
    local tran = go.transform
    ctx:SetActive(tran, true)
    -- [[处理前三张]]
    local is_front = layoutType < mj_gen_layoutType.SelfAnGang
    local res_str = mj_gen_res_common_str .. (is_front and "pp_" or "bm_") .. mj_gen_res_color_str[_s.GetMJPaiType()]
    for i = 1, 3 do
        -- 节点
        local card = ctx:GetTrans(tran, "card_" .. i)
        -- 牌id
        local card_id = ctx:GetTrans(card, "card_id")
        ctx:SetDepth(card_id, depth + 2)
        -- 数据对象
        local card_obj = CMJCard.New()
        card_obj:Init(cardIds[i], i)
        ctx:SetSprite(card_id, card_obj.ico)
        ctx:SetActive(card_id, is_front)
        -- 牌背
        local card_bg = ctx:GetTrans(card, "card_bg")
        ctx:SetDepth(card_bg, depth + 1)
        ctx:SetSprite(card_bg, res_str)
        card_bg.localScale =  is_front and UnityEngine.Vector3.New(1, -1, 1) or UnityEngine.Vector3.New(1, 1, 1)
        card_bg.localPosition =  is_front and UnityEngine.Vector3.New(0, 0, 0) or UnityEngine.Vector3.New(-5, -3, 1)
    end
    -- 处理第4张
    -- 节点
    local card = ctx:GetTrans(tran, "card_4")
    local isActive_4 = layoutType > mj_gen_layoutType.Peng
    ctx:SetActive(card, isActive_4)
    if isActive_4 then
        local is_front = not (layoutType == mj_gen_layoutType.OtherAnGang)
        local res_str = mj_gen_res_common_str .. (is_front and "pp_" or "bm_") .. mj_gen_res_color_str[_s.GetMJPaiType()]
        -- 牌id
        local card_id = ctx:GetTrans(card, "card_id")
        ctx:SetDepth(card_id, depth + 12)

        -- 数据对象
        local card_obj = CMJCard.New()
        card_obj:Init(cardIds[4], 4)
        ctx:SetSprite(card_id, card_obj.ico)
        ctx:SetActive(card_id, is_front)
        -- 牌背
        local card_bg = ctx:GetTrans(card, "card_bg")
        ctx:SetDepth(card_bg, depth + 11)
        ctx:SetSprite(card_bg, res_str)
        card_bg.localScale =  is_front and UnityEngine.Vector3.New(1, -1, 1) or UnityEngine.Vector3.New(1, 1, 1)
        card_bg.localPosition =  is_front and UnityEngine.Vector3.New(0, 0, 0) or UnityEngine.Vector3.New(-5, -3, 1)
    end
    return tran
end
--[[
@desc
生成一排麻将
@parma
(ctx 上下文)(father_node 父节点)(offsetX 横坐标基数)(offsetY 纵坐标基数)(cardIds 组合内容) (depth 深度基准值)
@return
返回对象根节点的transform
]]
function LogicUtil.GenCardArray(ctx, father_node, offsetX, cardIds, depth)
    depth = depth or 200
    local function genSingle(index, cardId)-- 序号
        local go = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "mj_card", RessStorgeType.RST_Never, false)
        if not go then
            DwDebug.Log("LogicUtil.GenCardGroup error :no go")
            return
        end
        local tran = go.transform
        ctx:SetActive(tran, true)
        tran.parent = father_node
        tran.name = "cardArray_" .. index
        tran.localPosition = UnityEngine.Vector3.New(offsetX + (index - 1) * mj_gen_spacing, 0, 0)
        tran.localScale = UnityEngine.Vector3.New(1, 1, 1)
        -- 数据节点
        local card_obj = CMJCard.New()
        card_obj:Init(cardId, index)
        -- 牌id
        local card_id = ctx:GetTrans(tran, "card_id")
        ctx:SetDepth(card_id, depth + 12)
        ctx:SetSprite(card_id, card_obj.ico)
        -- 牌背
        local card_bg = ctx:GetTrans(tran, "card_bg")
        ctx:SetDepth(card_bg, depth + 11)
        return tran
    end
    local _trans = {}
    if type(cardIds) == "number" then
        _trans[#_trans + 1] = genSingle(1, cardIds)
    else
        for i = 1, #cardIds do
            _trans[#_trans + 1] = genSingle(i, cardIds[i])
        end
    end
    return _trans
end

-- 32张获取派牌位玩家
function LogicUtil.PK32GetInitHandCardsSeatPos(bankerSeatId, pai)
    if nil ~= bankerSeatId and nil ~= pai then
        local cardItem = CCard.New()
        cardItem:Init(pai,0)

        local count_value = cardItem.logicValue or 1
        if count_value >= 14 and count_value <= 15 then
            count_value = count_value - 13
        elseif count_value == 16 then
            count_value = 6
        end

        local logic = PlayGameSys.GetPlayLogic()
        if logic and logic.roomObj and logic.roomObj.roomInfo and logic.roomObj.roomInfo.playerNum then
            local playerNum = logic.roomObj.roomInfo.playerNum
            local start_seatId = (((count_value-1)%playerNum)+bankerSeatId)%playerNum

            --两人场 对面玩家在2号位置上
            if playerNum == 2 and start_seatId == 1 then
                start_seatId = 2
            end
            
            local player = logic.roomObj.playerMgr:GetPlayerBySeatID(start_seatId)
            if player and player.seatPos then
                return player.seatPos
            end
        end
    end

    return SeatPosEnum.South
end

--从牌组分析结果里找出3张及其以上的牌
function LogicUtil.CollectThreeCardValueArray(analysisResult)
    local threeValueArray = {}
    if analysisResult.threeCount > 0 then
        for k,v in pairs(analysisResult.threeCards) do
            table.insert(threeValueArray,v[1])
        end
    end
    if analysisResult.fourCount > 0 then
        for k,v in pairs(analysisResult.fourCards) do
            table.insert(threeValueArray,v[1])
        end
    end
    if analysisResult.fiveCount > 0 then
        for k,v in pairs(analysisResult.fiveCards) do
            table.insert(threeValueArray,v[1])
        end
    end

    if analysisResult.sixCount > 0 then
        for k,v in pairs(analysisResult.sixCards) do
            table.insert(threeValueArray,v[1])
        end
    end
    if analysisResult.sevenCount > 0 then
        for k,v in pairs(analysisResult.sevenCards) do
            table.insert(threeValueArray,v[1])
        end
    end
    if analysisResult.eightCount > 0 then
        for k,v in pairs(analysisResult.eightCards) do
            table.insert(threeValueArray,v[1])
        end
    end

    table.sort(threeValueArray,function (x,y)
        return x.logicValue < y.logicValue
    end)
    return threeValueArray
end