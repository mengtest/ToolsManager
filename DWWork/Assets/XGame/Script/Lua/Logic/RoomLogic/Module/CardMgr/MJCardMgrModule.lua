--------------------------------------------------------------------------------
-- 	 File      : MJCardMgrModule.lua
--   author    : guoliang
--   function   : 麻将手牌管理组件基础类
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.MJCardLogic.CMJCard"
require "Logic.RoomLogic.Module.CardMgr.BaseCardMgrModule"

MJCardMgrModule = class("MJCardMgrModule", BaseCardMgrModule)

--从服务端初始化手牌
function MJCardMgrModule:InitCardsFromSvr(mj_cards, is_tanpai)
	self.handCards = {}
	if mj_cards then
		-- 初始化手牌
		if mj_cards.normalPai then
			for k,v in ipairs(mj_cards.normalPai) do
				local cardItem = CMJCard.New()
				cardItem:Init(v,k)
				table.insert(self.handCards, cardItem)
			end
		end

		-- DwDebug.LogError("MJPlayLogic",self.handCards)
	end
end

function MJCardMgrModule:DeleteCards(cardIds,deleteNum) -- 避免使用table.remove ,每次都会调整table结构，性能差,用临时table做，可以提升100部效率
	local deleteFlags = {}
	local newHandCards = {}
	for k,v in pairs(self.handCards) do
		for k1,v1 in pairs(cardIds) do
			if v.id == v1 and deleteFlags[k1] == nil and deleteNum > 0 then
				deleteFlags[k1] = k1
				deleteNum = deleteNum - 1
			else
				table.insert(newHandCards,v)
			end
		end
	end
	self.handCards = newHandCards
end