--------------------------------------------------------------------------------
-- 	 File      : PKCardMgrModule.lua
--   author    : guoliang
--   function   : 扑克手牌管理组件基础类
--   date      : 2017-11-5
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.RoomLogic.Module.CardMgr.BaseCardMgrModule"

PKCardMgrModule = class("PKCardMgrModule", BaseCardMgrModule)


function PKCardMgrModule:ctor()
	self:BaseCtor()
end

--从服务端初始化手牌
function PKCardMgrModule:InitCardsFromSvr(cardIds)
	if cardIds == nil then
		return
	end

	--保存一份有tag的手牌
	if self.savehandCards then
		self.tagHandCards = {}
		for k,v in pairs(self.savehandCards) do
			if v.typeTag > 0 then
				table.insert(self.tagHandCards,v)
			end
		end
	end

	self.handCards = {}
	local cardItem = nil
	for k,v in pairs(cardIds) do
		cardItem = CCard.New()
		cardItem:Init(v,k)
		--重新打上tag
		self:GetSaveCardTypeTag(cardItem)
		table.insert(self.handCards,cardItem)
	end
	LogicUtil.SortCards(self.handCards,false)
end

--断线重连 获取已经存在的tag
function PKCardMgrModule:GetSaveCardTypeTag(cardItem)
	if self.tagHandCards then
		for k,v in pairs(self.tagHandCards) do
			if v.ID == cardItem.ID then
				cardItem.typeTag = v.typeTag
				table.remove(self.tagHandCards,k)
				break
			end
		end
	end
end

function PKCardMgrModule:Clear()
	--存一份手牌
	self.savehandCards = {}
	for k, v in pairs(self.handCards) do
		table.insert(self.savehandCards, v)
	end

	self.handCards = {}
	self.curOutCardIds = {}
end

--不缓存 直接清理 为了清理tag
function PKCardMgrModule:ClearHandCard()
	self.handCards = {}
	self.savehandCards = {}
	self.tagHandCards = {}
	DwDebug.LogError("ClearHandCard ")
end
