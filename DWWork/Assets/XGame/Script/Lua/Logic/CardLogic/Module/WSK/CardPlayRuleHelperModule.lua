--------------------------------------------------------------------------------
-- 	 File      : CardPlayRuleHelperModule.lua
--   author    : guoliang
--   function   : 牌型判断规则组件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

CardPlayRuleHelperModule = class("CardPlayRuleHelperModule", nil)


function CardPlayRuleHelperModule:ctor()

end

function CardPlayRuleHelperModule:Init()

end

function CardPlayRuleHelperModule:CheckIsThree(cardType)
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
function CardPlayRuleHelperModule:CompareCard(analysisHelper,srcCards,nextCards,handCardCount)
	local srcCardType = analysisHelper:GetCardType(srcCards,#srcCards)

	local nextCardType = analysisHelper:GetCardType(nextCards,handCardCount)

	-- 上家牌最大牌 要不起
	if srcCardType == CardTypeEnum.CT_FOUR_KING then
		return false, nextCardType
	end

	

	-- 上手牌不合法
	if nextCardType == CardTypeEnum.CT_ERROR then
		return false, nextCardType
	end

	-- 上手牌最大牌
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
		print("srcCardType "..srcCardType)
		print("nextCardType "..nextCardType)
		print("handCardCount "..handCardCount)


		if nextCardType ~= srcCardType then -- 上家牌和上手牌牌型不一致
			--如果是对方牌型是三条类，但是手中牌也有三条，带牌数不同
			if self:CheckIsThree(srcCardType) then
				if (nextCardType == CardTypeEnum.CT_THREE or nextCardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE)
					and #nextCards == handCardCount then -- 自己手牌不足5张
					return self:_NormalTypeCompare(srcCardType,srcCards,nextCards,handCardCount), nextCardType
				elseif (srcCardType == CardTypeEnum.CT_THREE or srcCardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE)
				 and nextCardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO  then -- 自己手牌足够5张
				 	return self:_NormalTypeCompare(srcCardType,srcCards,nextCards,handCardCount), nextCardType
				end
			end
			
			return false, nextCardType
			
		else--牌型一致，进入正常的大小判断
			return self:_NormalTypeCompare(srcCardType,srcCards,nextCards,handCardCount), nextCardType
		end
	end

end


function  CardPlayRuleHelperModule:_NormalTypeCompare(cardType,srcCards,nextCards,handCardCount)
	-- 简单牌型
	if(cardType == CardTypeEnum.CT_SINGLE
		or cardType == CardTypeEnum.CT_DOUBLE
		or cardType == CardTypeEnum.CT_DOUBLE_SAME_KING) then
		return nextCards[1].logicValue > srcCards[1].logicValue
	end

	-- 复杂牌型,
	local analysisResult_src = BaseWSKCardAnalysisStruct:New()
	analysisResult_src:Analysis(srcCards)
	local analysisResult_next = BaseWSKCardAnalysisStruct:New()
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
	-- 三条类(3,3+1,3+2,3*n)
	if(cardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE
		or cardType == CardTypeEnum.CT_THREE) then
		return analysisResult_next.threeCards[1][1].logicValue > analysisResult_src.threeCards[1][1].logicValue
	elseif cardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO then -- 4+1可以当做3+2
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
		--print("next "..nextLogicValue .."src "..srcLogicValue)
		return nextLogicValue > srcLogicValue
	end

	if cardType == CardTypeEnum.CT_THREE_LINE then
		local nextLogicValueTable,nextLineNum = self:GetThreeLineLogicValueTable(analysisResult_next,nextCardCount,handCardCount)
		local srcLogicValueTable,srcLineNum = self:GetThreeLineLogicValueTable(analysisResult_src,srcCardCount,srcCardCount)--目前假定出牌数是对应手牌数（2飞机会有问题）
		if nextLogicValueTable and srcLogicValueTable and nextLineNum and srcLineNum then
--			print("nextLineNum "..nextLineNum .. " srcLineNum " ..srcLineNum)
--			if nextLineNum >= srcLineNum then -- 先放开这个判断让服务端去过滤
				local nextTableLen = #nextLogicValueTable
				local srcTableLen = #srcLogicValueTable
--				print("next "..nextLogicValueTable[nextTableLen] .."src "..srcLogicValueTable[srcTableLen])
				return nextLogicValueTable[nextTableLen] > srcLogicValueTable[srcTableLen]
--			end
		end
	end


	return false
end
--获取飞机的牌值数组
-- analysisResult 牌的分析结构， cardCount 牌的数量  handCardCount 手牌的数量
function CardPlayRuleHelperModule:GetThreeLineLogicValueTable(analysisResult,cardCount,handCardCount)
	-- 收集 3张以上的牌
	local threeValueArray = self:CollectThreeCardValueArray(analysisResult)
	if #threeValueArray <= 0 then
		return 
	end
	--三张连牌判断
	local maxLineNum = 1
	local curLineNum = 1
	local card
	local firstLogicValue = threeValueArray[1].logicValue
	local lineNumTable = {}
	local tempNumTable = {}

	--连牌判断
	for k,v in pairs(threeValueArray) do --因为从索引从1开始
		card = threeValueArray[k+1]
		if card ~=nil then
			if not card:IsCanLine() or (firstLogicValue ~= card.logicValue - curLineNum)then -- 不能三连的牌 或者 两个三条，连不起来，肯定是错牌（默认升序）
				curLineNum = 1
				firstLogicValue = card.logicValue
				tempNumTable = {}
				table.insert(tempNumTable,firstLogicValue)
			else
				curLineNum = curLineNum + 1
				table.insert(tempNumTable,card.logicValue)
				if maxLineNum < curLineNum then
					maxLineNum = curLineNum
					lineNumTable = tempNumTable
					tempNumTable = {}
				end
			end
		end
	end

	if maxLineNum*5 == cardCount then -- (3+2) * threeCount == cardCount
		return lineNumTable,maxLineNum
	elseif handCardCount and maxLineNum*5 >= handCardCount then
		return lineNumTable,maxLineNum
	else
		local realLineNum = math.floor(cardCount/5)
		if realLineNum < maxLineNum then -- 多个飞机下，其他的三张可以当做散牌
			if realLineNum*5 == cardCount then
				return lineNumTable,realLineNum
			elseif handCardCount and realLineNum * 5 >= handCardCount then
				return lineNumTable,realLineNum
			end
		end
	end

	return nil
end

function CardPlayRuleHelperModule:CollectThreeCardValueArray(analysisResult)
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
