--------------------------------------------------------------------------------
-- 	 File      : YH_CardPlayAIModule.lua
--   author    : jianing
--   function   : 手牌提示出牌组件
--   date      : 2018-01-04
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Logic.CardLogic.CardAIStruct.WSK.YH_WSKCardAIResultStruct "

YH_CardPlayAIModule = class("YH_CardPlayAIModule",nil)

function YH_CardPlayAIModule:ctor()
	self.cardAIResultStruct = nil
end

function YH_CardPlayAIModule:Init()
	
end

function YH_CardPlayAIModule:ResetAIResult()
	if self.cardAIResultStruct then
		self.cardAIResultStruct:ResetAIResult()
	end
end

--创建提示牌组列表
function YH_CardPlayAIModule:CreateAIResult(analysisHelper,handCards,srcCards)
	local srcCardType = analysisHelper:GetCardType(srcCards,#srcCards)
	self.cardAIResultStruct = YH_WSKCardAIResultStruct:New()
	return self.cardAIResultStruct:CreateAIResult(handCards,srcCardType,srcCards)

end

--查找
function YH_CardPlayAIModule:FindIntroduceCards(analysisHelper,handCards,srcCards)
	--提示出牌列表
	local introduceResult = {}
	local srcCardType = analysisHelper:GetCardType(srcCards,#srcCards)
	if self.cardAIResultStruct == nil  then
		self.cardAIResultStruct = YH_WSKCardAIResultStruct:New()
	end
	--print("srcCardType " .. srcCardType .. "..handCards " ..#handCards)
	return self.cardAIResultStruct:Analysis(handCards,srcCardType,srcCards)

end
