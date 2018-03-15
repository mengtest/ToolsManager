--------------------------------------------------------------------------------
-- 	 File      : YH_WSKCardPlayLogicContainer.lua
--   author    : jianing
--   function   : 宜黄红心5打牌逻辑容器
--   date      : 2017-12-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.CardLogic.LogicContainer.WSK.BaseWSKCardPlayLogicContainer"
require "Logic.CardLogic.Module.WSK.YiHuang.YH_CardAnalysisHelperModule"
require "Logic.CardLogic.Module.WSK.YiHuang.YH_CardPlayRuleHelperModule"
require "Logic.CardLogic.Module.WSK.YiHuang.YH_CardPlayAIModule"

YH_WSKCardPlayLogicContainer = class("YH_WSKCardPlayLogicContainer", BaseWSKCardPlayLogicContainer)


function YH_WSKCardPlayLogicContainer:ctor()
	self:BaseCtor()
end

function YH_WSKCardPlayLogicContainer:BaseCtor()
	self.analysisHelper = YH_CardAnalysisHelperModule:New()-- 牌型分析助手
	self.playAI = YH_CardPlayAIModule:New()-- 出牌提示
	self.playConfig = CardPlayConfigModule:New()-- 玩法配置
	self.oneTrunLogic = CardPlayOneTurnModule:New()-- 一轮出牌逻辑
	self.ruleHelper = YH_CardPlayRuleHelperModule:New()-- 判牌规则助手
	self:RegisterEventHandle()
	self.isNeedNewAIResult = true
end


