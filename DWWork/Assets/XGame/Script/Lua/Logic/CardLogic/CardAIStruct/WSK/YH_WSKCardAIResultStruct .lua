--------------------------------------------------------------------------------
-- 	 File      : BaseWSKCardAIResultStruct.lua
--   author    : guoliang
--   function   : 崇仁510K手牌提示出牌结果结构
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.CardLogic.CardAIStruct.WSK.BaseWSKCardAIResultStruct"

YH_WSKCardAIResultStruct  = class("YH_WSKCardAIResultStruct ", BaseWSKCardAIResultStruct)


function YH_WSKCardAIResultStruct :ctor()
	self:BaseCtor()
end

function YH_WSKCardAIResultStruct :Reset()
	
end

--添加对应牌数
function  YH_WSKCardAIResultStruct:CreateAIResult(cards,srcCardType,srcCards)
	self.introduceCards = {}
	self.introduceCardKeys = {}

	self.analysisStruct = YH_WSKCardAnalysisStruct:New()
	self.analysisStruct:Analysis(cards)
	
	self.srcCardAnalysisStruct = BaseWSKCardAnalysisStruct:New()
	self.srcCardAnalysisStruct:Analysis(srcCards)

	self.srcCardType = srcCardType
	self.srcCards = srcCards
	self.handCards = cards
	self.tipIndex = 0

	if srcCardType == CardTypeEnum.CT_SINGLE then
		self:SingleAI()
	elseif srcCardType == CardTypeEnum.CT_DOUBLE or srcCardType == CardTypeEnum.CT_DOUBLE_SAME_KING then
		self:DoubleAI()
	elseif srcCardType == CardTypeEnum.CT_THREE or srcCardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE 
		or srcCardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO then
		self:ThreeAI()
	elseif srcCardType == CardTypeEnum.CT_SINGLE_LINE then
		self:SingleLineAI()
	elseif srcCardType == CardTypeEnum.CT_DOUBLE_LINE then
		self:DoubleLineAI()
	elseif srcCardType == CardTypeEnum.CT_THREE_LINE then
		self:ThreeLineAI()
	elseif srcCardType >= CardTypeEnum.CT_BOMB_FOUR then
		self:BombAI()
	end
	
	return #self.introduceCardKeys
end

-- 用于基本类型时的炸弹查询
function YH_WSKCardAIResultStruct:BaseBombCheck()
--	print("BaseBombCheck")
	local result = self.analysisStruct
	-- bomb
	if result.fourCount > 0 then
		for k,v in pairs(result.fourCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_FOUR then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_FOUR then
				self:AddIntroduceTips(v)
			end
		end
	end

	if result.fiveCount > 0 then
		for k,v in pairs(result.fiveCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_FIVE then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_FIVE then
				self:AddIntroduceTips(v)
			end
		end
	end

	if result.sixCount > 0 then
		for k,v in pairs(result.sixCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_SIX then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_SIX then
				self:AddIntroduceTips(v)
			end
		end
	end

	-- 三王
	if result.kingCount >= 3 then
		if self.srcCardType < CardTypeEnum.CT_THREE_KING then
			local tempList = {}
			for k,v in pairs(result.kingCards) do
				if k <= 3 then
					table.insert(tempList,v)
				else
					break
				end
			end

			self:AddIntroduceTips(tempList)
		end
	end

	if result.sevenCount > 0 then
		for k,v in pairs(result.sevenCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_SEVEN then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_SEVEN then
				self:AddIntroduceTips(v)
			end
		end
	end

	if result.eightCount > 0 then
		for k,v in pairs(result.eightCards) do
			if self.srcCardType == CardTypeEnum.CT_BOMB_EIGTH then
				if v[1].logicValue > self.srcCards[1].logicValue then
					self:AddIntroduceTips(v)
				end
			elseif self.srcCardType < CardTypeEnum.CT_BOMB_EIGTH then
				self:AddIntroduceTips(v)
			end
		end
	end

	-- 四王
	if result.kingCount == 4 then
		if self.srcCardType < CardTypeEnum.CT_FOUR_KING then
			self:AddIntroduceTips(result.kingCards)
		end
	end

	--一对红心5
	if self.srcCardType < CardTypeEnum.CT_DOUBLE_SAME_HongXin_5 and #result.hongXin5 > 1 then
		self:AddIntroduceTips(result.hongXin5)
	end

end

function YH_WSKCardAIResultStruct:ThreeAI()
	local result = self.analysisStruct
	--飞机的起始牌值
	local srcThreeCardLogicValue = self.srcCards[1].logicValue
	-- 收集上家打出牌中的3张以上的牌
	local threeValueArray = LogicUtil.CollectThreeCardValueArray(self.srcCardAnalysisStruct)
	if threeValueArray and #threeValueArray > 0 then
		srcThreeCardLogicValue = threeValueArray[1].logicValue
	end

	-- 首先对应牌型

	if result.threeCount > 0 then
		local temp
		for k,v in pairs(result.threeCards) do
			if v[1].logicValue > srcThreeCardLogicValue then
				temp = ShallowCopy(v)
				--从剩余牌组里补充带的牌
				local reminTable = self:GetLimitCountCardsByExclude(result,{v[1].logicValue},2)
				for k,v in pairs(reminTable) do
					table.insert(temp,v)
				end
				if #temp == 5 then
					self:AddIntroduceTips(temp)
				end
			end
		end
	end

	self:BaseBombCheck()
	-- 走拆分


	local tempThreeTable = {}
	-- 从4张中找
	if result.fourCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.fourCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	-- 从5张中找
	if result.fiveCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.fiveCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	-- 从6张中找
	if result.sixCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.sixCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	-- 从7张中找
	if result.sevenCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.sevenCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	-- 从8炸中找
	if result.eightCount > 0 then
		self:InsertIntroduceFromTable_three(result,result.eightCards,tempThreeTable,srcThreeCardLogicValue,3)
	end

	local compareFunc = function(cardTable1, cardTable2)
        if cardTable1[1].logicValue < cardTable2[1].logicValue then
            return true
        else
            return false
        end
    end

    table.sort(tempThreeTable,compareFunc)

	for k,v in pairs(tempThreeTable) do
		if #v == 5 then
			self:AddIntroduceTips(v)
		end
	end

end