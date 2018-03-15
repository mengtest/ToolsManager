--------------------------------------------------------------------------------
-- 	 File       : DDZCardAnalysisHelperModule.lua
--   author     : zhanghaochun
--   function   : 手牌分析组件
--   date       : 2018-01-04
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

local AnalysisStruct = require "CommonProduct.DouDiZhu.NormalDDZ.Container.Module.AnalysisStruct.DDZCardAnalysisStruct"

local DDZCardAnalysisHelperModule = class("DDZCardAnalysisHelperModule")

function DDZCardAnalysisHelperModule:ctor()
	self:Init()
end

function DDZCardAnalysisHelperModule:Init()
	self.moduleAnalysis = AnalysisStruct.New()
	self.CompareFunc = function(cards1, cards2)
		if cards1[1].logicValue < cards2[1].logicValue then
			return true
		elseif cards1[1].logicValue == cards2[1].logicValue then
			return cards1[1].ID > cards2[1].ID
		else
			return false
		end
	end
end

-- 根据牌组获取类型
function DDZCardAnalysisHelperModule:GetCardType(cards, handCardsCount)
	if cards == nil then
		return NewCardTypeEnum.NCCT_ERROR
	end
	self.moduleAnalysis:Analysis(cards)
	
	local count = self.moduleAnalysis:GetTotalCount()
	
	if count == 0 then
		return NewCardTypeEnum.NCCT_ERROR
	elseif count == 1 then
		return self:AnalysisCards_1(self.moduleAnalysis)
	elseif count == 2 then
		return self:AnalysisCards_2(self.moduleAnalysis)
	elseif count == 3 then	
		return self:AnalysisCards_3(self.moduleAnalysis)
	elseif count == 4 then
		return self:AnalysisCards_4(self.moduleAnalysis)
	elseif count == 5 then
		return self:AnalysisCards_5(self.moduleAnalysis)
	elseif count == 6 then
		return self:AnalysisCards_6(self.moduleAnalysis)
	elseif count == 7 then	
		return self:AnalysisCards_7(self.moduleAnalysis)
	elseif count == 8 then
		return self:AnalysisCards_8(self.moduleAnalysis)
	elseif count == 9 then
		return self:AnalysisCards_9(self.moduleAnalysis)
	elseif count == 10 then
		return self:AnalysisCards_10(self.moduleAnalysis)
	elseif count == 11 then
		return self:AnalysisCards_11(self.moduleAnalysis)
	elseif count == 12 then
		return self:AnalysisCards_12(self.moduleAnalysis)
	elseif count == 13 then
		return self:AnalysisCards_13(self.moduleAnalysis)
	elseif count == 14 then
		return self:AnalysisCards_14(self.moduleAnalysis)
	elseif count == 15 then
		return self:AnalysisCards_15(self.moduleAnalysis)
	elseif count == 16 then
		return self:AnalysisCards_16(self.moduleAnalysis)
	elseif count == 17 then
		return self:AnalysisCards_17(self.moduleAnalysis)
	elseif count == 18 then
		return self:AnalysisCards_18(self.moduleAnalysis)
	elseif count == 19 then
		return self:AnalysisCards_19(self.moduleAnalysis)
	elseif count == 20 then
		return self:AnalysisCards_20(self.moduleAnalysis)
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:GetCardAnalysisStruct()
	return self.moduleAnalysis
end

-------------------------------根据牌的数量判断牌型--------------------------------
--[[相当私有方法,默认已经判断好牌的数量]]
function DDZCardAnalysisHelperModule:AnalysisCards_1(analyser)	
	-- 可能的牌型 : 单张
	local cards, count = analyser:GetSingleCards()
	if count == 1 then
		return NewCardTypeEnum.NCCT_SINGLE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_2(analyser)	
	-- 可能的牌型 : 火箭，对子
	
	local cards, count = analyser:GetSingleCards()
	-- 分析是否是火箭
	if count == 2  then
		if cards[1][1]:IsKing() and cards[2][1]:IsKing() then
			return NewCardTypeEnum.NCCT_ROCKET         -- 火箭
		end
	end

	-- 分析是否是对子
	cards, count = analyser:GetDoubleCards()
	if count == 2 then
		return NewCardTypeEnum.NCCT_DOUBLE         -- 对子
	end

	return NewCardTypeEnum.NCCT_ERROR
end

function DDZCardAnalysisHelperModule:AnalysisCards_3(analyser)
	-- 可能出现的牌型：三张

	local cards, count = analyser:GetThreeCards()
	if count == 3 then
		return NewCardTypeEnum.NCCT_THREE
	end
	
	return NewCardTypeEnum.NCCT_ERROR
end

function DDZCardAnalysisHelperModule:AnalysisCards_4(analyser)
	-- 可能出现的牌型：炸弹， 连对， 三带一
	local cards, count = analyser:GetFourCards()
	
	if count == 4 then
		-- 分析是否是炸弹
		return NewCardTypeEnum.NCCT_BOMB
	end
	
	if self:IsDoubleLine(analyser) then
		-- 分析是否是连对
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	elseif self:IsThreeAddOne(analyser) then
		-- 分析是否是三带一
		return NewCardTypeEnum.NCCT_THREE_ADD_ONE
	end

	return NewCardTypeEnum.NCCT_ERROR
end

function DDZCardAnalysisHelperModule:AnalysisCards_5(analyser)
	-- 可能出现的牌型：三带一对，四带一，顺子
	if self:IsThreeAddDouble(analyser) then
		return NewCardTypeEnum.NCCT_THREE_ADD_DOUBLE
	elseif self:IsSingleLine(analyser) then
		return NewCardTypeEnum.NCCT_SINGLE_LINE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_6(analyser)
	-- 可能出现的牌型：四带二，四带一对，顺子，连对，飞机
	if self:IsFourAddTwo(analyser) then
		return NewCardTypeEnum.NCCT_FOUR_ADD_TWO
	elseif self:IsSingleLine(analyser) then
		return NewCardTypeEnum.NCCT_SINGLE_LINE
	elseif self:IsDoubleLine(analyser) then
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	elseif self:IsAirplane(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_7(analyser)
	-- 可能出现的牌型：顺子
	if self:IsSingleLine(analyser) then
		return NewCardTypeEnum.NCCT_SINGLE_LINE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_8(analyser)
	-- 可能出现的牌型：顺子，连对， 飞机+相同数量的单牌， 四带两对
	if self:IsSingleLine(analyser) then
		return NewCardTypeEnum.NCCT_SINGLE_LINE
	elseif self:IsDoubleLine(analyser) then
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	elseif self:IsAirplaneAddSameOne(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE
	elseif self:IsFourAddTwoDouble(analyser) then
		return NewCardTypeEnum.NCCT_FOUR_ADD_TWO_DOUBLE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_9(analyser)
	-- 可能出现的牌型：顺子，飞机
	if self:IsSingleLine(analyser) then	
		return NewCardTypeEnum.NCCT_SINGLE_LINE
	elseif self:IsAirplane(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_10(analyser)
	-- 可能出现的牌型：顺子，连对，飞机+相等数量的对牌
	if self:IsSingleLine(analyser) then
		return NewCardTypeEnum.NCCT_SINGLE_LINE
	elseif self:IsDoubleLine(analyser) then
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	elseif self:IsAirplaneAddSameDouble(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_DOUBLE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_11(analyser)
	-- 可能出现的牌型：顺子
	if self:IsSingleLine(analyser) then	
		return NewCardTypeEnum.NCCT_SINGLE_LINE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_12(analyser)
	-- 可能出现的牌型：顺子，连对，飞机，飞机+数量相等的单牌
	if self:IsSingleLine(analyser) then
		return NewCardTypeEnum.NCCT_SINGLE_LINE
	elseif self:IsDoubleLine(analyser) then
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	elseif self:IsAirplane(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE
	elseif self:IsAirplaneAddSameOne(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_13(analyser)
	-- 可能出现的牌型：
	return NewCardTypeEnum.NCCT_ERROR
end

function DDZCardAnalysisHelperModule:AnalysisCards_14(analyser)
	-- 可能出现的牌型：连对
	if self:IsDoubleLine(analyser) then
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_15(analyser)
	-- 可能出现的牌型：飞机，飞机+相等数量的对牌
	if self:IsAirplane(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE
	elseif self:IsAirplaneAddSameDouble(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_DOUBLE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_16(analyser)
	-- 可能出现的牌型：连对，飞机+数量相等的单牌
	if self:IsDoubleLine(analyser) then
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	elseif self:IsAirplaneAddSameOne(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_17(analyser)
	-- 可能出现的牌型：
	return NewCardTypeEnum.NCCT_ERROR
end

function DDZCardAnalysisHelperModule:AnalysisCards_18(analyser)
	-- 可能出现的牌型：连对，飞机
	if self:IsDoubleLine(analyser) then
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	elseif self:IsAirplane(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end

function DDZCardAnalysisHelperModule:AnalysisCards_19(analyser)
	-- 可能出现的牌型：
	return NewCardTypeEnum.NCCT_ERROR
end

function DDZCardAnalysisHelperModule:AnalysisCards_20(analyser)
	-- 可能出现的牌型：连对，飞机+数量相等的单牌，飞机+相等数量的对牌
	if self:IsDoubleLine(analyser) then
		return NewCardTypeEnum.NCCT_DOUBLE_LINE
	elseif self:IsAirplaneAddSameOne(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE
	elseif self:IsAirplaneAddSameDouble(analyser) then
		return NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_DOUBLE
	else
		return NewCardTypeEnum.NCCT_ERROR
	end
end
--------------------------------------------------------------------------------

----------------------------------判断牌型函数------------------------------------
--[[相当于私有方法]]
-- 是否是连对
function DDZCardAnalysisHelperModule:IsDoubleLine(analyser)
	local result = true
	local totalCount = analyser:GetTotalCount()
	local cards, count = analyser:GetDoubleCards()
	
	if totalCount == count and IsEven(count) and count > 4 then
		result = self:IsSeries_2(cards)
	else
		result = false
	end

	return result
end

-- 是否是三带一
function DDZCardAnalysisHelperModule:IsThreeAddOne(analyser)
	local totalCount = analyser:GetTotalCount()
	local cards1, count1 = analyser:GetSingleCards()
	local cards3, count3 = analyser:GetThreeCards()
	
	if totalCount == (count1 + count3) then
		if count3 == 3 and count1 == 1 then
			return true
		else
			return false
		end
	else
		return false
	end
	
end

-- 是否是三带一对
function DDZCardAnalysisHelperModule:IsThreeAddDouble(analyser)
	local totalCount = analyser:GetTotalCount()
	local cards2, count2 = analyser:GetDoubleCards()
	local cards3, count3 = analyser:GetThreeCards()

	if totalCount == (count2 + count3) then
		if count3 == 3 and count2 == 2 then
			return true
		else
			return false
		end
	else
		return false
	end
	
end

-- 是否是四带二
function DDZCardAnalysisHelperModule:IsFourAddTwo(analyser)
	local totalCount = analyser:GetTotalCount()
	local cards4, count4 = analyser:GetFourCards()

	if count4 == 4 and totalCount == 6 then
		return true
	else
		return false
	end
end

-- 是否是四带二对
function DDZCardAnalysisHelperModule:IsFourAddTwoDouble(analyser)
	local totalCount = analyser:GetTotalCount()
	local cards4, count4 = analyser:GetFourCards()
	-- 连续的四张不合法
	if self:IsSeries_2(cards4) then
		if count4 == 8 then 
			return false 
		end
	end

	local cards2, count2 = analyser:GetDoubleCards()

	if totalCount == (count2 + count4) then
		if count4 == 4 and count2 == 4 then
			return true
		elseif count4 == 8 then
			return true
		else
			return false
		end
	else
		return false
	end
	
end

-- 是否是顺子
function DDZCardAnalysisHelperModule:IsSingleLine(analyser)
	local result = true
	local totalCount = analyser:GetTotalCount()
	local cards, count = analyser:GetSingleCards()
	
	if totalCount == count then
		if count >= 5 then
			result = self:IsSeries_2(cards)
		else
			result = false
		end
	else
		result = false
	end
	
	return result
end

-- 是否是飞机
function DDZCardAnalysisHelperModule:IsAirplane(analyser, isUseTotalNum)
	local result = true
	local totalCount = analyser:GetTotalCount()
	local cards, count = analyser:GetThreeCards()
	if isUseTotalNum == nil then isUseTotalNum = true end
	
	if totalCount == count or not isUseTotalNum then
		local integer, remainder = Division(count, 3)
		if count >= 6 and remainder == 0 then
			result = self:IsSeries_2(cards)
		else
			result = false
		end
	else
		result = false
	end

	return result
end

-- 是否是飞机带相同数量的单牌
function DDZCardAnalysisHelperModule:IsAirplaneAddSameOne(analyser)
	local totalCount = analyser:GetTotalCount()

	-- 两个连续的四张不合法
	local fourResult = {}
	analyser:FindAllOnlyCards(fourResult, 4, 0, true)
	if self:IsSeries_2(fourResult) then 
		if #fourResult == 8 then
			return false
		end 
	end
	
	local threeResult = {}
	analyser:FindAllOnlyCards(threeResult, 3, 0, true)
	
	local series = self:FindSeries_2(threeResult)
	local seriesLen = #series
	if seriesLen < 2 then return false end
	for i = seriesLen, 2, -1 do
		-- 注:看成连续的三带一，每个三带一有四张牌
		if i * 4 == totalCount then
			return true
		end
	end
	return false
end

-- 飞机带相同数量的对牌
function DDZCardAnalysisHelperModule:IsAirplaneAddSameDouble(analyser)
	local twoCards, twoCount = analyser:GetDoubleCards()
	local threeCards, threeCount = analyser:GetThreeCards()
	local fourResult, fourCount = analyser:GetFourCards()
	if self:IsSeries_2(threeCards) then
		local otherTotal = twoCount + fourCount
		if otherTotal * 3 == threeCount * 2 then
			return true
		else
			return false
		end
	else
		return false
	end
end
--------------------------------------------------------------------------------

-----------------------------------私有方法--------------------------------------
-- 用于判断{1,2,3,4}结构的
function DDZCardAnalysisHelperModule:IsSeries_1(cards)
	local result = true
	
	for i = #cards, 2, -1 do
		if not cards[i]:IsCanLine() then
			result = false
			break
		end
	
		if cards[i].logicValue - cards[i-1].logicValue ~= 1 then
			result = false
			break
		end
	end

	return result
end

-- 用于判断{{1,...},{1,...},{1,...},{1,...}}结构的
function DDZCardAnalysisHelperModule:IsSeries_2(cards)
	local result = true

	if #cards < 2 then result = false end

	for i = #cards, 2, -1 do
		if not cards[i][1]:IsCanLine() then
			result = false
			break
		end
			
		if cards[i][1].logicValue - cards[i-1][1].logicValue ~= 1 then
			result = false
			break
		end
	end
	
	return result
end

-- 用于查找{{1,...},{1,...},{1,...},{1,...}}第一个能连在一起的结构
--[[function DDZCardAnalysisHelperModule:FindSeries_2(cards)
	local result = {}
	
	if #cards < 2 then return result end

	for i = #cards, 2, 1 do
		if cards[i][1]:IsCanLine() then
			table.insert(result, cards[i])
			if cards[i][1].logicValue - cards[i-1][1].logicValue ~= 1 then
				table.remove(result, #result)
				if #result < 2 then
					result = {}
				else
					break
				end
			end
		else
			break
		end
	end
end]]

-- 获取cards中第一个line的
function DDZCardAnalysisHelperModule:FindSeries_2(cards)
	local sIndex, eIndex = 1, 1
	for i = 1, #cards - 1 do
		local iCard = cards[i]
		local i_1_Card = cards[i + 1]
		if iCard[1]:IsCanLine() and i_1_Card[1]:IsCanLine() and
		   i_1_Card[1].logicValue - iCard[1].logicValue == 1 then
			eIndex = eIndex + 1
		else
			if eIndex - sIndex > 0 then
				break
			else
				sIndex = i + 1
				eIndex = i + 1
			end
		end
	end

	local result = {}
	if eIndex ~= sIndex then
		for i = sIndex, eIndex do
			table.insert(result, cards[i])
		end
	end
	return result
end
--------------------------------------------------------------------------------

return DDZCardAnalysisHelperModule