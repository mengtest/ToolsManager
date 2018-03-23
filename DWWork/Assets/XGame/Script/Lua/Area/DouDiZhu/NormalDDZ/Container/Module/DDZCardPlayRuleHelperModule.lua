--------------------------------------------------------------------------------
-- 	 File       : DDZCardPlayRuleHelperModule.lua
--   author     : zhanghaochun
--   function   : 斗地主牌型判断规则组件
--   date       : 2017-01-05
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

local AnalysisStruct = require "Area.DouDiZhu.NormalDDZ.Container.Module.AnalysisStruct.DDZCardAnalysisStruct"

local DDZCardPlayRuleHelperModule = class("DDZCardPlayRuleHelperModule")


function DDZCardPlayRuleHelperModule:ctor()
	self.struct_1 = AnalysisStruct.New()
	self.struct_2 = AnalysisStruct.New()
end

-- 出牌比较
-- analysisHelper : 牌组分析模块    cards_1 : 上家牌组    cards_2 : 准备上手的牌组    handCardsCount : 准备上手的玩家的手牌数量
function DDZCardPlayRuleHelperModule:CompareCard(analysisHelper, cards_1, cards_2, handCardsCount)
	local srcType = analysisHelper:GetCardType(cards_1, #cards_1)
	local nextType = analysisHelper:GetCardType(cards_2, handCardsCount)

	-- 准备上手牌组不合法
	if nextType == NewCardTypeEnum.NCCT_ERROR then
		return false, nextType
	end

	if srcType == nextType then
		self.struct_1:Analysis(cards_1)
		self.struct_2:Analysis(cards_2)
		-- 牌型相同
		if srcType == NewCardTypeEnum.NCCT_SINGLE then
			-- 单牌
			local oneCards_1, count_1 = self.struct_1:GetSingleCards()
			local oneCards_2, count_2 = self.struct_2:GetSingleCards()
			return self:CompareCardsFirst(oneCards_1, oneCards_2), nextType
		elseif srcType == NewCardTypeEnum.NCCT_DOUBLE then
			-- 对子
			local doubleCards_1, count_1 = self.struct_1:GetDoubleCards()
			local doubleCards_2, count_2 = self.struct_2:GetDoubleCards()
			return self:CompareCardsFirst(doubleCards_1, doubleCards_2), nextType
		elseif srcType == NewCardTypeEnum.NCCT_THREE or
			   srcType == NewCardTypeEnum.NCCT_THREE_ADD_ONE or
			   srcType == NewCardTypeEnum.NCCT_THREE_ADD_DOUBLE then
			-- 三张, 三带一张，三带一对
			return self:CompareThreeSeries(self.struct_1, self.struct_2), nextType
		elseif srcType == NewCardTypeEnum.NCCT_SINGLE_LINE then
			-- 顺子
			return self:CompareSingleLine(self.struct_1, self.struct_2), nextType
		elseif srcType == NewCardTypeEnum.NCCT_DOUBLE_LINE then
			-- 连对
			return self:CompareDoubleLine(self.struct_1, self.struct_2), nextType
		elseif srcType == NewCardTypeEnum.NCCT_THREE_LINE or
			   srcType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE or
			   srcType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_DOUBLE then
			-- 飞机，飞机+相同单张，飞机+相同对
			return self:CompareAirplaneSeries(self.struct_1, self.struct_2), nextType
		elseif srcType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO or
			   srcType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO_DOUBLE then
			-- 四带二，四带两对
			return self:CompareFourSeries(self.struct_1, self.struct_2), nextType
		elseif srcType == NewCardTypeEnum.NCCT_BOMB then
			-- 炸弹
			return self:CompareFourSeries(self.struct_1, self.struct_2), nextType
		elseif srcType == NewCardTypeEnum.NCCT_ROCKET then
			-- 火箭
			-- 这种情况在斗地主不肯能存在，因为火箭只有一个
			return false, nextType
		else
			-- 此种情况按道理来说，根本不存在
			DwDebug.LogError("Logic is wrong")
			return false, nextType
		end
	else
		-- 牌型不同
		if srcType >= NewCardTypeEnum.NCCT_SINGLE and
		   srcType <= NewCardTypeEnum.NCCT_FOUR_ADD_TWO_DOUBLE then
			if nextType == NewCardTypeEnum.NCCT_BOMB or
			   nextType == NewCardTypeEnum.NCCT_ROCKET then
				return true, nextType
			else
				return false, nextType
			end
		elseif srcType == NewCardTypeEnum.NCCT_BOMB then
			if nextType == NewCardTypeEnum.NCCT_ROCKET then
				return true, nextType
			else
				return false, nextType
			end
		elseif srcType == NewCardTypeEnum.NCCT_ROCKET then
			return false, nextType
		else
			-- 此种情况按道理来说，根本不存在
			logError("Logic is wrong")
			return false, nextType
		end
	end
end

-------------------------------私有方法(比较)-------------------------------------
-- 比较牌组中第一个牌的大小 如果cards_2 > cards_1 返回true
function DDZCardPlayRuleHelperModule:CompareCardsFirst(structCards_1, structCards_2)
	return structCards_2[1][1].logicValue > structCards_1[1][1].logicValue
end

-- 比较顺子
function DDZCardPlayRuleHelperModule:CompareSingleLine(struct_1, struct_2)
	if struct_1:GetTotalCount() == struct_2:GetTotalCount() then
		local oneCards_1, count_1 = struct_1:GetSingleCards()
		local oneCards_2, count_2 = struct_2:GetSingleCards()
		return self:CompareCardsFirst(oneCards_1, oneCards_2)
	else
		return false
	end
end

-- 比较连对
function DDZCardPlayRuleHelperModule:CompareDoubleLine(struct_1, struct_2)
	if struct_1:GetTotalCount() == struct_2:GetTotalCount() then
		local doubleCards_1, count_1 = struct_1:GetDoubleCards()
		local doubleCards_2, count_2 = struct_2:GetDoubleCards()
		return self:CompareCardsFirst(doubleCards_1, doubleCards_2)
	else
		return false
	end
end

-- 比较飞机系列
function DDZCardPlayRuleHelperModule:CompareAirplaneSeries(struct_1, struct_2)
	if struct_1:GetTotalCount() == struct_2:GetTotalCount() then
		--return self:CompareThreeSeries(struct_1, struct_2)
		local cards_1
		local cards_2
		local threeCards_1, threeCount_1 = struct_1:GetThreeCards()
		if threeCount_1 == 0 then
			local fourCards_1, fourCount_1 = struct_1:GetFourCards()
			if fourCount_1 == 0 then
				return false
			else
				cards_1 = fourCards_1
			end
		else
			cards_1 = threeCards_1
		end

		local threeCards_2, threeCount_2 = struct_2:GetThreeCards()
		if threeCount_2 == 0 then
			local fourCards_2, fourCount_2 = struct_2:GetFourCards()
			if fourCount_2 == 0 then
				return false
			else
				cards_2 = fourCards_2
			end
		else
			cards_2 = threeCards_2
		end

		return self:CompareCardsFirst(cards_1, cards_2)
	else
		return false
	end
end

-- 比较三张系列
function DDZCardPlayRuleHelperModule:CompareThreeSeries(struct_1, struct_2)
	if struct_1:GetTotalCount() == struct_2:GetTotalCount() then
		local threeCards_1, count_1 = struct_1:GetThreeCards()
		local threeCards_2, count_2 = struct_2:GetThreeCards()
		if count_1 == count_2 then
			return self:CompareCardsFirst(threeCards_1, threeCards_2)
		else
			return false
		end
	else
		return false
	end	
	
end

--比较四张系列
function DDZCardPlayRuleHelperModule:CompareFourSeries(struct_1, struct_2)
	if struct_1:GetTotalCount() == struct_2:GetTotalCount() then
		local fourCards_1, count_1 = struct_1:GetFourCards()
		local fourCards_2, count_2 = struct_2:GetFourCards()
		if count_1 == count_2 then
			return self:CompareCardsFirst(fourCards_1, fourCards_2)
		else
			return false
		end
	else
		return false
	end
end
--------------------------------------------------------------------------------

return DDZCardPlayRuleHelperModule
