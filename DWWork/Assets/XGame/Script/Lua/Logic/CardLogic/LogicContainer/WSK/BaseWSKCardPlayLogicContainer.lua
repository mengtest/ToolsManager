--------------------------------------------------------------------------------
-- 	 File      : BaseWSKCardPlayLogicContainer.lua
--   author    : guoliang
--   function   : 510K打牌逻辑容器基类
--   date      : 2017-12-12
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.CardLogic.CardTypeEnum"
require "Logic.CardLogic.CCard"
require "Logic.CardLogic.CardAnalysisStruct.WSK.ChongRenWSKCardAnalysisStruct"
require "Logic.CardLogic.CardAIStruct.WSK.ChongRenWSKCardAIResultStruct"
require "Logic.CardLogic.Module.WSK.CardAnalysisHelperModule"
require "Logic.CardLogic.Module.WSK.CardPlayAIModule"
require "Logic.CardLogic.Module.WSK.CardPlayConfigModule"
require "Logic.CardLogic.Module.WSK.CardPlayOneTurnModule"
require "Logic.CardLogic.Module.WSK.CardPlayRuleHelperModule"


BaseWSKCardPlayLogicContainer = class("BaseWSKCardPlayLogicContainer", nil)


function BaseWSKCardPlayLogicContainer:ctor()
	self:BaseCtor()
end

function BaseWSKCardPlayLogicContainer:BaseCtor()
	self.analysisHelper = CardAnalysisHelperModule:New()-- 牌型分析助手
	self.playAI = CardPlayAIModule:New()-- 出牌提示
	self.playConfig = CardPlayConfigModule:New()-- 玩法配置
	self.oneTrunLogic = CardPlayOneTurnModule:New()-- 一轮出牌逻辑
	self.ruleHelper = CardPlayRuleHelperModule:New()-- 判牌规则助手
	self:RegisterEventHandle()
	self.isNeedNewAIResult = true
end

function BaseWSKCardPlayLogicContainer:BaseInit()
	-- body
end
function BaseWSKCardPlayLogicContainer:Init()
	self:BaseInit()
end

function BaseWSKCardPlayLogicContainer:RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.Alone_Play,self.SetAlone,self)
	LuaEvent.AddHandle(EEventType.RoundEndClear,self.NewRoundStart,self)

end

function BaseWSKCardPlayLogicContainer:UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.Alone_Play,self.SetAlone,self)
	LuaEvent.RemoveHandle(EEventType.RoundEndClear,self.NewRoundStart,self)

end

function BaseWSKCardPlayLogicContainer:Update()
	
end

function BaseWSKCardPlayLogicContainer:Destroy()
	self.analysisHelper = nil
	self.playAI = nil
	self.playConfig = nil
	self.oneTrunLogic = nil
	self.ruleHelper = nil
	self:UnRegisterEventHandle()
end

-- 玩法参数配置

-- 设置打独
function BaseWSKCardPlayLogicContainer:SetAlone(eventId, p1, p2)
	local aloneInfo = p1
	if aloneInfo then
		self.playConfig:SetPlayModel(isAlone)
	end
end

-- 清牌开始新的一轮
function BaseWSKCardPlayLogicContainer:NewRoundStart(eventId, p1, p2)
	self.oneTrunLogic:Clear()

end
-- 添加一轮出牌的一次有效出牌
function BaseWSKCardPlayLogicContainer:AddOneOutCards(outCardInfo)
	if outCardInfo and not outCardInfo.isSkip then
		self.oneTrunLogic:AddOneOutCardInfo(outCardInfo)
	end
end

-- 获取上一次上手的牌
function BaseWSKCardPlayLogicContainer:GetLastOutCardInfo()
	return self.oneTrunLogic:GetLastOutCardInfo()
end

-- 获取当前出牌轮数
function BaseWSKCardPlayLogicContainer:GetCurRoundIndex()
	return self.oneTrunLogic:GetCurRoundIndex()
end

--业务区

-- 能否出牌
function BaseWSKCardPlayLogicContainer:CheckCanOutCard(srcCards,handCardCount)
	local cardType = self.analysisHelper:GetCardType(srcCards,handCardCount)
	
	local canOut = false
	if cardType == CardTypeEnum.CT_ERROR then
		canOut = false
	else
		if cardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE then
			if handCardCount and handCardCount == #srcCards then
				canOut = true
			else
				canOut = false
			end
		elseif cardType == CardTypeEnum.CT_THREE then
			if handCardCount and handCardCount == #srcCards then
				canOut = true
			else
				canOut = false
			end
		else
			canOut = true
		end
	end

	return canOut, cardType
end

-- 出牌判定
function BaseWSKCardPlayLogicContainer:CompareCard(srcCards,nextCards,handCardCount)
	return self.ruleHelper:CompareCard(self.analysisHelper,srcCards,nextCards,handCardCount)
end
-- 生成AI结果
function BaseWSKCardPlayLogicContainer:CreateAIResult(handCards,srcCards)
	return self.playAI:CreateAIResult(self.analysisHelper,handCards,srcCards)
end

-- 重置AI结果
function BaseWSKCardPlayLogicContainer:ResetAIResult()
	return self.playAI:ResetAIResult()
end


-- 出牌提示
function BaseWSKCardPlayLogicContainer:FindIntroduceCards(handCards,srcCards)
--	print(self.isNeedNewAIResult)
	return self.playAI:FindIntroduceCards(self.analysisHelper,handCards,srcCards)
end



