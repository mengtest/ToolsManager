--------------------------------------------------------------------------------
-- 	 File      : CardPlayOneTurnModule.lua
--   author    : guoliang
--   function   : 一轮打牌逻辑组件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

CardPlayOneTurnModule = class("CardPlayOneTurnModule",nil)


function CardPlayOneTurnModule:ctor()
	self.outCardsList = {}
	self.curRoundIndex = 0
end

function CardPlayOneTurnModule:Init()
	
end


function CardPlayOneTurnModule:AddOneOutCardInfo(outCardInfo)
--	print("AddOneOutCardInfo " .. ToString(outCardInfo))
	if outCardInfo then
		if outCardInfo.roundIndex > self.curRoundIndex then
			self.curRoundIndex = outCardInfo.roundIndex
		end
		table.insert(self.outCardsList,outCardInfo)
	end
end

function CardPlayOneTurnModule:Clear()
	self.outCardsList = {}
	self.curRoundIndex = 0
end

function CardPlayOneTurnModule:GetLastOutCardInfo()
	local len = #self.outCardsList
--	print("CardPlayOneTurnModule:GetLastOutCardInfo len = "..len)
	if len > 0 then
		return self.outCardsList[len]
	end
	return nil
end

function CardPlayOneTurnModule:GetCurRoundIndex()
	return self.curRoundIndex
end


