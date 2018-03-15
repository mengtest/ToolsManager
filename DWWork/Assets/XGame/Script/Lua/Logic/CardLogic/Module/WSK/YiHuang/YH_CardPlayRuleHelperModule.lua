--------------------------------------------------------------------------------
-- 	 File      : CardPlayRuleHelperModule.lua
--   author    : guoliang
--   function  : 牌型判断规则组件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.CardLogic.Module.WSK.CardPlayRuleHelperModule"
require "Logic.CardLogic.CardAnalysisStruct.WSK.YH_WSKCardAnalysisStruct"

YH_CardPlayRuleHelperModule = class("YH_CardPlayRuleHelperModule", CardPlayRuleHelperModule)

function YH_CardPlayRuleHelperModule:ctor()

end

function YH_CardPlayRuleHelperModule:Init()

end

function YH_CardPlayRuleHelperModule:CheckIsThree(cardType)
	if cardType == CardTypeEnum.CT_THREE or cardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO 
				or cardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE then
		return true
	else
		return false
	end
end

-- 出牌比较
-- srcCards 上家牌组  nextCards 准备上手牌组
-- 返回值 上手大于上家牌 返回true
function YH_CardPlayRuleHelperModule:CompareCard(analysisHelper,srcCards,nextCards,handCardCount)
	local srcCardType = analysisHelper:GetCardType(srcCards,#srcCards)
	local nextCardType = analysisHelper:GetCardType(nextCards,handCardCount)

	-- 上家牌最大牌 要不起
	if srcCardType == CardTypeEnum.CT_DOUBLE_SAME_HongXin_5 then
		return false, nextCardType
	end

	-- 上手牌不合法
	if nextCardType == CardTypeEnum.CT_ERROR then
		return false, nextCardType
	end

	-- 上手牌最大牌
	if nextCardType == CardTypeEnum.CT_DOUBLE_SAME_HongXin_5 then
		return true, nextCardType
	end

	-- 上家牌第二大
	if srcCardType == CardTypeEnum.CT_FOUR_KING then
		return false, nextCardType
	end

	-- 上手牌第二大
	if nextCardType == CardTypeEnum.CT_FOUR_KING then
		return true, nextCardType
	end

	-- 纯色双王是对子的特殊类型 先过滤，
	if srcCardType == CardTypeEnum.CT_DOUBLE_SAME_KING and nextCardType == CardTypeEnum.CT_DOUBLE then
		return false, nextCardType
	elseif srcCardType == CardTypeEnum.CT_DOUBLE and nextCardType == CardTypeEnum.CT_DOUBLE_SAME_KING then
		return true, nextCardType
	end

	-- 首先上家牌是否炸弹
	if srcCardType >= CardTypeEnum.CT_FIVE_TEN_K then
		-- 炸弹类型
		if nextCardType > srcCardType then 
			return true, nextCardType
		elseif nextCardType == srcCardType then-- 类型相同，比大小
			if nextCards[1].logicValue > srcCards[1].logicValue then
				return true, nextCardType
			else
				return false, nextCardType
			end 
		else
			return false, nextCardType
		end

	elseif  nextCardType >= CardTypeEnum.CT_FIVE_TEN_K then--上家牌不是炸弹，上手牌是炸弹
		return true, nextCardType
	else-- 都是正常牌
		if nextCardType ~= srcCardType then -- 上家牌和上手牌牌型不一致
			--如果是对方牌型是三条类，但是手中牌也有三条，带牌数不同
			if self:CheckIsThree(srcCardType) then
				if (nextCardType == CardTypeEnum.CT_THREE or nextCardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE)
					and #nextCards == handCardCount then -- 自己手牌不足5张
					return self:_NormalTypeCompare(srcCardType,srcCards,nextCards)
				elseif (srcCardType == CardTypeEnum.CT_THREE or srcCardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE)
				 and nextCardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO  then -- 自己手牌足够5张
				 	return self:_NormalTypeCompare(srcCardType,srcCards,nextCards)
				end
			end
			return false, nextCardType
			
		else--牌型一致，进入正常的大小判断
			return self:_NormalTypeCompare(srcCardType,srcCards,nextCards), nextCardType
		end
	end

end

--比较牌
function  YH_CardPlayRuleHelperModule:_NormalTypeCompare(cardType,srcCards,nextCards)
	-- 简单牌型
	if(cardType == CardTypeEnum.CT_SINGLE
		or cardType == CardTypeEnum.CT_DOUBLE
		or cardType == CardTypeEnum.CT_THREE
		or cardType == CardTypeEnum.CT_DOUBLE_SAME_KING) then
		return nextCards[1].logicValue > srcCards[1].logicValue
	end

	-- 复杂牌型
	local analysisResult_src = YH_WSKCardAnalysisStruct:New()
	analysisResult_src:Analysis(srcCards)
	local analysisResult_next = YH_WSKCardAnalysisStruct:New()
	analysisResult_next:Analysis(nextCards)

	local srcCardCount = #srcCards
	local nextCardCount = #nextCards

	-- 顺子
	if cardType == CardTypeEnum.CT_SINGLE_LINE then
		if srcCardCount == nextCardCount then
			--比较最小牌即可
			return analysisResult_next.singleCards[1][1].logicValue > analysisResult_src.singleCards[1][1].logicValue
		else
			return false
		end
	end
	-- 连对
	if cardType == CardTypeEnum.CT_DOUBLE_LINE then
		if srcCardCount == nextCardCount then
			return analysisResult_next.doubleCards[1][1].logicValue > analysisResult_src.doubleCards[1][1].logicValue
		else
			return false
		end
	end
	-- 三条类(3+2,3*n)
	if cardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO then -- 4+1可以当做3+2
		local nextLogicValue = 0
		local srcLogicValue = 0
		if analysisResult_next.fourCount > 0 then
			nextLogicValue = analysisResult_next.fourCards[1][1].logicValue
		else
			nextLogicValue = analysisResult_next.threeCards[1][1].logicValue
		end

		if analysisResult_src.fourCount > 0 then
			srcLogicValue = analysisResult_src.fourCards[1][1].logicValue
		else
			srcLogicValue = analysisResult_src.threeCards[1][1].logicValue
		end
		return nextLogicValue > srcLogicValue
	end

	if cardType == CardTypeEnum.CT_THREE_LINE then
		local nextLogicValue = self:GetThreeLineMinLogicValue(analysisResult_next)
		local srcLogicValue = self:GetThreeLineMinLogicValue(analysisResult_src)

		return nextLogicValue > srcLogicValue
	end

	return false
end

function YH_CardPlayRuleHelperModule:GetThreeLineMinLogicValue(analysisResult)
	local nextLogicValue = 1000
	if analysisResult.threeCount > 0 then
		if nextLogicValue > analysisResult.threeCards[1][1].logicValue then
			nextLogicValue = analysisResult.threeCards[1][1].logicValue
		end
	end
	if analysisResult.fourCount > 0 then
		if nextLogicValue > analysisResult.fourCards[1][1].logicValue then
			nextLogicValue = analysisResult.fourCards[1][1].logicValue
		end
	end

	if analysisResult.fiveCount > 0 then
		if nextLogicValue > analysisResult.fiveCards[1][1].logicValue then
			nextLogicValue = analysisResult.fiveCards[1][1].logicValue
		end
	end

	if analysisResult.sixCount > 0 then
		if nextLogicValue > analysisResult.sixCards[1][1].logicValue then
			nextLogicValue = analysisResult.sixCards[1][1].logicValue
		end
	end

	if analysisResult.sevenCount > 0 then
		if nextLogicValue > analysisResult.sevenCards[1][1].logicValue then
			nextLogicValue = analysisResult.sevenCards[1][1].logicValue
		end
	end

	if analysisResult.eightCount > 0 then
		if nextLogicValue > analysisResult.eightCards[1][1].logicValue then
			nextLogicValue = analysisResult.eightCards[1][1].logicValue
		end
	end
	return nextLogicValue
end
