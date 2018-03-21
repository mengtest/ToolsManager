--------------------------------------------------------------------------------
-- 	 File       : NormalDiZhuContainer.lua
--   author     : zhanghaochun
--   function   : 斗地主打牌逻辑容器基类
--   date       : 2018-01-03
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "CommonProduct.CommonBase.NewCardTypeEnum"
require "Logic.CardLogic.CCard"

require "Logic.CardLogic.Module.WSK.CardPlayOneTurnModule"
local CardAnalysisHelper = require "Area.DouDiZhu.NormalDDZ.Container.Module.DDZCardAnalysisHelperModule"
local CardRuleHelper = require "Area.DouDiZhu.NormalDDZ.Container.Module.DDZCardPlayRuleHelperModule"
local CardPlayAIModule = require "Area.DouDiZhu.NormalDDZ.Container.Module.DDZCardPlayAIModule"

local NormalDiZhuContainer = class("NormalDiZhuContainer")

function NormalDiZhuContainer:ctor()
	self:InitModules()
	self:InitValues()
end

function NormalDiZhuContainer:InitModules()
	self.analysisHelper = CardAnalysisHelper.New()
	self.ruleHelper = CardRuleHelper.New()
	self.playAI = CardPlayAIModule.New()
	self.oneTrunLogic = CardPlayOneTurnModule:New()    -- 一轮出牌逻辑
end

function NormalDiZhuContainer:InitValues()
	self.isNeedNewAIResult = true
end

-- 添加一轮出牌的一次有效出牌
function NormalDiZhuContainer:AddOneOutCards(outCardInfo)
	if outCardInfo and not outCardInfo.isSkip then
		self.oneTrunLogic:AddOneOutCardInfo(outCardInfo)
	end
end

-- 获取上一次上手的牌
function NormalDiZhuContainer:GetLastOutCardInfo()
	return self.oneTrunLogic:GetLastOutCardInfo()
end

-- 获取当前出牌轮数
function NormalDiZhuContainer:GetCurRoundIndex()
	return self.oneTrunLogic:GetCurRoundIndex()
end

-------------------------------关于出牌的接口-------------------------------------
-- 判断将要出的牌是否合法
-- cards : 将要打出去的牌    handCardsCount : 手牌数量
function NormalDiZhuContainer:DDZCardsIsRule(cards, handCardsCount)
	local cardType = self.analysisHelper:GetCardType(cards, handCardsCount)
	local canOut = false

	if cardType == NewCardTypeEnum.NCCT_ERROR then
		canOut = false
	else
		canOut = true
	end

	return canOut, cardType
end

-- 判断出牌的大小
-- cards_1 : 上一个玩家出的牌   cards_2 : 我将要出的手牌    handCardsCount : 手牌数量
function NormalDiZhuContainer:DDZCompareCards(cards_1, cards_2, handCardsCount)
	return self.ruleHelper:CompareCard(self.analysisHelper, cards_1, cards_2, handCardsCount)
end

-- 获取推荐出牌
-- handCards : 我的手牌    lastCards : 上一个玩家出的牌
function NormalDiZhuContainer:DDZFindIntroduceCards(handCards, lastCards)
	return self.playAI:FindIntroduceCards(self.analysisHelper, handCards, lastCards, self.isNeedNewAIResult)
end

-- 获取推荐出牌的数量
function NormalDiZhuContainer:DDZGetIntroduceCardsCount(handCards, lastCards)
	return self.playAI:GetResultCount(self.analysisHelper, handCards, lastCards, self.isNeedNewAIResult)
end

function NormalDiZhuContainer:DDZSetGetIntroduceCardsState(state)
	if state == nil or state == false then
		state = false
	else
		state = true
	end

	self.isNeedNewAIResult = state
end

function NormalDiZhuContainer:ResetAIResult()
	self.playAI:ResetAIResult()
end
--------------------------------------------------------------------------------

function NormalDiZhuContainer:Destroy()
	self.analysisHelper = nil
	self.ruleHelper = nil
end

return NormalDiZhuContainer