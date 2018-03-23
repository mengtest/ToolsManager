--------------------------------------------------------------------------------
-- 	 File       : UIDDZLordOpenCardsCtrl.lua
--   author     : zhanghaochun
--   function   : 斗地主地主名牌控件
--   date       : 2018-02-08
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

local UIDDZLordOpenCardsCtrl = class("UIDDZLordOpenCardsCtrl")

function UIDDZLordOpenCardsCtrl:ctor(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot

	self:InitValues()
	self:InitComponents()
	self:RegisterEvent()
	self:SetDefaultShow()
end

function UIDDZLordOpenCardsCtrl:InitValues()
	self.SeatPosToTransName = {
		[SeatPosEnum.West] = "Show_1",
		[SeatPosEnum.East] = "Show_2",
	}
	self.list = {}
	self.totalLen = 1800
end

function UIDDZLordOpenCardsCtrl:InitComponents()
end

function UIDDZLordOpenCardsCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.DDZLordOpenPlay, self.DealDDZLordOpenPlayEvent, self)
	LuaEvent.AddHandle(EEventType.PlayOutCardAction, self.DealPlayerOutCardEvent, self)
	LuaEvent.AddHandle(EEventType.DDZCloseLordOpenPlayCard, self.DealCloseLordCardsEvent, self)
end

function UIDDZLordOpenCardsCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.DDZLordOpenPlay, self.DealDDZLordOpenPlayEvent, self)
	LuaEvent.RemoveHandle(EEventType.PlayOutCardAction, self.DealPlayerOutCardEvent, self)
	LuaEvent.RemoveHandle(EEventType.DDZCloseLordOpenPlayCard, self.DealCloseLordCardsEvent, self)
end

function UIDDZLordOpenCardsCtrl:SetDefaultShow()
end

-------------------------------------事件----------------------------------------
function UIDDZLordOpenCardsCtrl:DealDDZLordOpenPlayEvent(eventID, p1, p2)
	local rsp = p1
	if rsp ~= nil then
		if rsp.isOpen then
			self:SetDDZLordOpenPlayEventRsp(rsp)
		end
	end
end

function UIDDZLordOpenCardsCtrl:DealPlayerOutCardEvent(eventID, p1, p2)
	local rsp = p1
	if rsp ~= nil then	
		self:LordDeleateCards(rsp)
	end
end

function UIDDZLordOpenCardsCtrl:DealCloseLordCardsEvent(eventID, p1, p2)
	self:CloseAllCardShow()
end
--------------------------------------------------------------------------------

-------------------------------------对外接口------------------------------------
function UIDDZLordOpenCardsCtrl:Destroy()
	self:UnRegisterEvent()
	self.list = {}
end
--------------------------------------------------------------------------------

----------------------------------私有接口----------------------------------------
function UIDDZLordOpenCardsCtrl:SetDDZLordOpenPlayEventRsp(rsp)
	local playLogic = PlayGameSys.GetPlayLogic()
	local player = playLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if player ~= nil then
		local transName = self.SeatPosToTransName[player.seatPos]
		if transName == nil then
			return
		end
		if self.list[transName] ~= nil then
			self:SetCardsToInvisible(self.list[transName])
		end
		self.list[transName] = {}
	
		local cardList = self.list[transName]
		
		local cards = {}
		for k, v in ipairs(rsp.pai) do
			local card = CCard.New()
			card:Init(v, k)
			table.insert(cards, card)
		end
		LogicUtil.SortCards(cards, false)

		local root = self.luaWindowRoot:GetTrans(self.rootTrans, transName)
		for k, v in ipairs(cards) do
			local trans = self:InitCardShow(v, root, k)
			local cardInfo = {}
			cardInfo.trans = trans
			cardInfo.cardInfo = v
			table.insert(cardList, cardInfo)
		end

		LogicUtil.AdjustCards(#cardList, self.totalLen, cardList)
		self:SetLastCardTransLordTag(cardList)
	end
end

function UIDDZLordOpenCardsCtrl:InitCardShow(card, root, index)
	local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "card_item_small", RessStorgeType.RST_Never, false)
	local trans = resObj.transform
	LogicUtil.EnableTouch(trans, false)
	trans.parent = root
	trans.localScale = Vector3.one
	self.luaWindowRoot:SetActive(trans, true)
	trans.name = index .. "_CardItem"
	LogicUtil.InitPKCard_3(self.luaWindowRoot, trans, card, index, false)

	return trans
end

function UIDDZLordOpenCardsCtrl:SetCardsToInvisible(cards)
	for k, v in ipairs(cards) do
		self.luaWindowRoot:SetActive(v.trans, false)
	end
end

function UIDDZLordOpenCardsCtrl:LordDeleateCards(rsp)
	local playLogic = PlayGameSys.GetPlayLogic()
	
	if rsp.seatId ~= playLogic.bankerSeatId then return end	

	local player = playLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if player ~= nil then
		local transName = self.SeatPosToTransName[player.seatPos]
		local cardList = self.list[transName]
		if cardList == nil then
			return
		end

		for k, v in ipairs(rsp.pai) do
			local index = 0
			for key, value in ipairs(cardList) do
				if v == value.cardInfo.ID then
					index = key
					break
				end
			end
			
			if index ~= 0 then
				self.luaWindowRoot:SetActive(cardList[index].trans, false)
				table.remove(cardList, index)
			end
		end

		LogicUtil.AdjustCards(#cardList, self.totalLen, cardList)
		self:SetLastCardTransLordTag(cardList)
	end
end

function UIDDZLordOpenCardsCtrl:SetLastCardTransLordTag(cardList)
	if cardList == nil or #cardList == 0 then return end

	local lastTrans = cardList[#cardList].trans
	LogicUtil.SetCardLordTag(self.luaWindowRoot, lastTrans, true)
end

function UIDDZLordOpenCardsCtrl:CloseAllCardShow()
	for k, v in pairs(self.list) do
		if v then
			self:SetCardsToInvisible(v)
		end
		v = nil
	end
end
--------------------------------------------------------------------------------

return UIDDZLordOpenCardsCtrl
