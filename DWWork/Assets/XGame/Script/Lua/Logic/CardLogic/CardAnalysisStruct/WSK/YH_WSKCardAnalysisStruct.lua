--------------------------------------------------------------------------------
-- 	 File      : YH_WSKCardAnalysisStruct.lua
--   author    : jianing
--   function   : 宜黄红心5 手牌分析结构
---   date      : 2017-12-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.CardLogic.CardAnalysisStruct.WSK.BaseWSKCardAnalysisStruct"

YH_WSKCardAnalysisStruct = class("YH_WSKCardAnalysisStruct", BaseWSKCardAnalysisStruct)

function YH_WSKCardAnalysisStruct:ctor()

end

function YH_WSKCardAnalysisStruct:Reset()
	self.hongXin5 = {}
	self.super.Reset(self)
end

--生成牌组结构
function YH_WSKCardAnalysisStruct:Analysis(cards)--参数 CCard 列表
	self:Reset()
	local cardCount = #cards
	local sorCards = ShallowCopy(cards)--因为会排序，会影响到传入的数组顺序
	LogicUtil.SortCards(sorCards,true)--升序排列
	
	local sameNum = 0
	local sameColor = true
	local sameCards = {}
	local lastCardColor = 0
	
	--一对红心5
	for i=1,cardCount do
		local card = cards[i]
		if card.ID == 45 then -- 红心5
			table.insert(self.hongXin5,card)
		end
	end

	local i = 1
	while i <= cardCount do
		sameNum = 0
		sameCards = {}
		local v = sorCards[i]
		if sameColor and lastCardColor > 0 and not v:IsSameColor(lastCardColor) then
			sameColor = false
		end
		lastCardColor = v.color

		sameNum = sameNum + 1
		table.insert(sameCards,v)

		if v.logicValue == 16 or v.logicValue == 17 then -- king 判断
			table.insert(self.kingCards,v)
			self.kingCount = self.kingCount + 1
		end

		-- 正常判断
		for j = i+1,cardCount do
			if v.logicValue == sorCards[j].logicValue then
				
				if v.logicValue == 16 or v.logicValue == 17 then -- king 判断
					table.insert(self.kingCards,sorCards[j])
					self.kingCount = self.kingCount + 1
				end
				
				sameNum = sameNum + 1
				table.insert(sameCards,sorCards[j])
			else
				break
			end
		end
		self:_Add(sameCards,sameNum)
		i = i + sameNum
	end
	self.isSameColor = sameColor
end


