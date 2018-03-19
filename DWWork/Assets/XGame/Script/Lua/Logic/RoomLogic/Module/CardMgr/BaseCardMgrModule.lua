--------------------------------------------------------------------------------
-- 	 File      : BaseCardMgrModule.lua
--   author    : guoliang
--   function   : 玩家手牌管理组件基础类
--   date      : 2017-11-5
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------


BaseCardMgrModule = class("BaseCardMgrModule", DWBaseModule)


function BaseCardMgrModule:ctor()
	self:BaseCtor()
end

function BaseCardMgrModule:BaseCtor()
	self.handCards = {}
	self.curOutCardIds = {}
end

function BaseCardMgrModule:Init(parent)
	self.parent = parent
end

--从服务端初始化手牌
function BaseCardMgrModule:InitCardsFromSvr(cardIds)
	
end

--保存服务端下发的玩家的当前出牌
function BaseCardMgrModule:SavePlayerCurOutCardIds(outCardIds)
	self.curOutCardIds = outCardIds
end

function BaseCardMgrModule:GetPlayerCurOutCardIds()
	return self.curOutCardIds
end

function BaseCardMgrModule:DeleteCards(outCards) -- 避免使用table.remove ,每次都会调整table结构，性能差,用临时table做，可以提升100部效率
	local newHandCards = {}
	local isContain = false
	for k,v in pairs(self.handCards) do
		isContain = false
		for k1,v1 in pairs(outCards) do
			if v.seq == v1.seq then
				isContain = true
				break
			end
		end
		if not isContain then
			table.insert(newHandCards,v)
		end
	end
	self.handCards = newHandCards
end

function BaseCardMgrModule:AddCards(inCards)
	local handCards = self.handCards or {}
	for k, v in pairs(inCards) do
		table.insert(handCards, v)
	end
	self.handCards = handCards
end

function BaseCardMgrModule:GetHandCards()
	return self.handCards
end

function BaseCardMgrModule:Clear()
	self.handCards = {}
	self.curOutCardIds = {}
end


function BaseCardMgrModule:Destroy()
	self.handCards = nil
	self.curOutCardIds = nil
end