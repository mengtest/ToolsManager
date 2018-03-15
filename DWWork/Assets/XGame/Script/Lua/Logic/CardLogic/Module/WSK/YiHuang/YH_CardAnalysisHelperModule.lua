--------------------------------------------------------------------------------
-- 	 File      : YH_CardAnalysisHelperModule.lua
--   author    : jianing
--   function  : 宜黄红心5 手牌分析组件
--   date      : 2017-12-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
--require "Logic.CardLogic.Module.YH_WSKCardAnalysisStruct"

YH_CardAnalysisHelperModule = class("YH_CardAnalysisHelperModule", CardAnalysisHelperModule)

function YH_CardAnalysisHelperModule:ctor()

end

--根据牌组获取牌型(同时返回分析结果)  (顺序 ：简单牌型 -> 特殊牌型 -> 常规牌型)
function YH_CardAnalysisHelperModule:GetCardType(cards,handCardCount)
	if cards == nil then
		return CardTypeEnum.CT_ERROR
	end

	local cardCount = #cards
	local wangBomb = true

	--简单牌型过滤
	if cardCount == 0 then
		return CardTypeEnum.CT_ERROR
	elseif cardCount == 1 then
		return CardTypeEnum.CT_SINGLE
	elseif cardCount == 2 then
		if cards[1].logicValue == cards[2].logicValue then
			if cards[1]:IsKing() then
				return CardTypeEnum.CT_DOUBLE_SAME_KING
			elseif cards[1].ID == 45 and cards[2].ID == 45 then --对红心5
				return CardTypeEnum.CT_DOUBLE_SAME_HongXin_5
			else
				return CardTypeEnum.CT_DOUBLE
			end
		else 
			return CardTypeEnum.CT_ERROR			
		end
	elseif cardCount == 4 or cardCount == 3 then
		for k,v in pairs(cards) do
			if wangBomb and not v:IsKing() then
				wangBomb = false
			end
		end
		if wangBomb then
			if cardCount == 3 then
				return CardTypeEnum.CT_THREE_KING
			else
				return CardTypeEnum.CT_FOUR_KING
			end
		end
	end

	-- 复杂牌型
	local analysisResult = BaseWSKCardAnalysisStruct:New()
	analysisResult:Analysis(cards)

	return self:_CheckSpecialType(analysisResult,cardCount,handCardCount),analysisResult
end

-- 特殊牌型判断
function  YH_CardAnalysisHelperModule:_CheckSpecialType(analysisResult,cardCount,handCardCount)
	-- 4带1 可以认为是3带2
	if cardCount == 5 and analysisResult.fourCount == 1 then
		return CardTypeEnum.CT_THREE_LINE_TAKE_TWO
	end

	--红心5
	if cardCount == 2 and #analysisResult.hongXin5 == 2 then
		return CardTypeEnum.CT_DOUBLE_SAME_HongXin_5
	end

	--特殊牌型判断完，进入正常牌型判定
	return	self:_NormalTypeCheck(analysisResult,cardCount,handCardCount)
end

-- 正常复杂牌型判断
function YH_CardAnalysisHelperModule:_NormalTypeCheck(analysisResult,cardCount,handCardCount)
	-- 注意，牌型判断顺序是有依赖的，不能修改本函数的判断顺序
	-- 1 数量相同炸弹判断
	if cardCount >= 4 and cardCount <= 8 then
		if analysisResult.fourCount == 1 and cardCount == 4 then
			return CardTypeEnum.CT_BOMB_FOUR
		elseif analysisResult.fiveCount == 1 and cardCount == 5 then
			return CardTypeEnum.CT_BOMB_FIVE
		elseif analysisResult.sixCount == 1 and cardCount == 6 then
			return CardTypeEnum.CT_BOMB_SIX
		elseif analysisResult.sevenCount == 1 and cardCount == 7 then
			return CardTypeEnum.CT_BOMB_SEVEN
		elseif analysisResult.eightCount == 1 and cardCount == 8 then
			return CardTypeEnum.CT_BOMB_EIGTH
		end
	end
		
	local firstLogicValue
	-- 2 三张
	if analysisResult.threeCount > 0 or analysisResult.fourCount > 0 
		or analysisResult.fiveCount > 0 or analysisResult.sixCount > 0
		or analysisResult.sevenCount > 0 or analysisResult.eightCount > 0 then
		--3带2
		if analysisResult.threeCount == 1 and cardCount == 5 then
			return CardTypeEnum.CT_THREE_LINE_TAKE_TWO
		end

		-- 收集 3张以上的牌
		local threeValueArray = LogicUtil.CollectThreeCardValueArray(analysisResult)
		--print("threeValueArray count ="..#threeValueArray)
		--三张连牌判断
		local maxLineNum = 1
		local curLineNum = 1
		local card
		firstLogicValue = threeValueArray[1].logicValue
		--连牌判断
		for k,v in pairs(threeValueArray) do --因为从索引从1开始
			card = threeValueArray[k+1]
			if card ~=nil then
				--print("firstLogicValue = "..firstLogicValue .. " card.logicValue "..card.logicValue .. " curLineNum "..curLineNum)
				if not card:IsCanLine() or (firstLogicValue ~= card.logicValue - curLineNum) then -- 不能三连的牌 或者 两个三条，连不起来，肯定是错牌（默认升序）
					curLineNum = 1
					firstLogicValue = card.logicValue
				else
					curLineNum = curLineNum + 1
					if maxLineNum < curLineNum then
						maxLineNum = curLineNum
					end
				end
			end
		end

		if maxLineNum >= 6 and PlayGameSys.GetPlayLogic():CheckMyIsAlone() then--关刀 不用判断带牌了
			return CardTypeEnum.CT_THREE_LINE
		end
	
		local canDuoFei = PlayGameSys.GetPlayLogic():GetDuoFei()
		--飞机
		if maxLineNum*5 == cardCount then -- (3+2) * threeCount == cardCount
			if canDuoFei then--多飞
				return CardTypeEnum.CT_THREE_LINE
			elseif maxLineNum <= 2 then
				return CardTypeEnum.CT_THREE_LINE
			end
		else
			local realLineNum = math.floor(cardCount/5)
			if realLineNum < maxLineNum then -- 多个飞机下，其他的三张可以当做散牌
				if realLineNum*5 == cardCount then
					if canDuoFei then--多飞
						return CardTypeEnum.CT_THREE_LINE
					elseif realLineNum <= 2 then
						return CardTypeEnum.CT_THREE_LINE
					end
				elseif handCardCount and realLineNum * 5 >= handCardCount then
					if canDuoFei then--多飞
						return CardTypeEnum.CT_THREE_LINE
					elseif realLineNum <= 2 then
						return CardTypeEnum.CT_THREE_LINE
					end
				end
			end
		end
	end

	local cards
	--3 连对
	if analysisResult.doubleCount > 1 then
		cards = analysisResult.doubleCards[1]
		-- 不能二连的牌
		if not cards[1]:IsCanLine() then
			return CardTypeEnum.CT_ERROR
		end
		firstLogicValue = cards[1].logicValue
		--连牌判断
		for k,v in pairs(analysisResult.doubleCards) do --因为从索引从1开始
			cards = analysisResult.doubleCards[k+1]
			if cards ~=nil then
				-- 不能二连的牌
				if not cards[1]:IsCanLine() then
					return CardTypeEnum.CT_ERROR
				end
				if firstLogicValue ~= cards[1].logicValue - k then -- 连不起来，肯定是错牌（默认升序）
					return CardTypeEnum.CT_ERROR
				end
			else
				break
			end
		end
		--没有错误出现，可以放心判定了
		if (analysisResult.doubleCount*2 == cardCount) then  -- 2 * doubleCount = cardCount
			return CardTypeEnum.CT_DOUBLE_LINE
		end
	end

	-- 4 顺子
	if analysisResult.singleCount > 1 then
		if analysisResult.singleCount  >= 5 then
			cards = analysisResult.singleCards[1]
			-- 不能二连的牌
			if not cards[1]:IsCanLine() then
				return CardTypeEnum.CT_ERROR
			end

			firstLogicValue = cards[1].logicValue
			--连牌判断
			for k,v in pairs(analysisResult.singleCards) do --因为从索引从1开始
				cards = analysisResult.singleCards[k+1];
				if cards ~=nil then
					-- 不能二连的牌
					if not cards[1]:IsCanLine() then
						return CardTypeEnum.CT_ERROR
					end
				 	if firstLogicValue ~= cards[1].logicValue - k then -- 连不起来，肯定是错牌（默认升序）
						return CardTypeEnum.CT_ERROR
					end
				else
					break
				end
			end
			--没有错误出现，可以放心判定了
			if (analysisResult.singleCount == cardCount) then  -- singleCount = cardCount
				return CardTypeEnum.CT_SINGLE_LINE
			end
		else -- 单牌数小于5 肯定是错的
			return CardTypeEnum.CT_ERROR
		end
	end

	return CardTypeEnum.CT_ERROR
end
