--------------------------------------------------------------------------------
-- 	 File       : DDZCardPlayAIModule.lua
--   author     : zhanghaochun
--   function   : 斗地主手牌提示出牌组件
--   date       : 2018-01-05
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

local CardAIResultStruct = require "Area.DouDiZhu.NormalDDZ.Container.Module.AI.DDZCardAIResultStruct"

local DDZCardPlayAIModule = class("DDZCardPlayAIModule")

function DDZCardPlayAIModule:ctor()
	self:InitModule()
end

function DDZCardPlayAIModule:InitModule()
	self.AIStruct = CardAIResultStruct.New()
end

function DDZCardPlayAIModule:FindIntroduceCards(analysisHelper,handCards,srcCards,bNewTip)
	local srcCardType = analysisHelper:GetCardType(srcCards)
	return self.AIStruct:Analysis(handCards, srcCardType, srcCards, bNewTip)
end

function DDZCardPlayAIModule:GetResultCount(analysisHelper,handCards,srcCards,bNewTip)
	local srcCardType = analysisHelper:GetCardType(srcCards)
	return self.AIStruct:GetCount(handCards, srcCardType, srcCards, bNewTip)
end

function DDZCardPlayAIModule:ResetAIResult()
	self.AIStruct:ResetAIResult()
end

return DDZCardPlayAIModule