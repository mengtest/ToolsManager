--------------------------------------------------------------------------------
-- 	 File      : CardPlayAIModule.lua
--   author    : guoliang
--   function   : 手牌提示出牌组件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

CardPlayAIModule = class("CardPlayAIModule",nil)


function CardPlayAIModule:ctor()
	self.cardAIResultStruct = nil
end

function CardPlayAIModule:Init()
	
end

function CardPlayAIModule:ResetAIResult()
	if self.cardAIResultStruct then
		self.cardAIResultStruct:ResetAIResult()
	end
end
--创建提示牌组列表
function CardPlayAIModule:CreateAIResult(analysisHelper,handCards,srcCards)
	local srcCardType = analysisHelper:GetCardType(srcCards,#srcCards)
	self.cardAIResultStruct = ChongRenWSKCardAIResultStruct:New()
	return self.cardAIResultStruct:CreateAIResult(handCards,srcCardType,srcCards)
end

--查找
function CardPlayAIModule:FindIntroduceCards(analysisHelper,handCards,srcCards)
	local srcCardType = analysisHelper:GetCardType(srcCards,#srcCards)
	if self.cardAIResultStruct == nil then
		self.cardAIResultStruct = ChongRenWSKCardAIResultStruct:New()
	end
	return self.cardAIResultStruct:Analysis(handCards,srcCardType,srcCards)

end
