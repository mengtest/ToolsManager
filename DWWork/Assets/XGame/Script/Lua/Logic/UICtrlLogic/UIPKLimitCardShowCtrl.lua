--------------------------------------------------------------------------------
-- 	 File       : UIPKLimitCardShowCtrl.lua
--   author     : zhanghaochun
--   function   : 指定数量的扑克牌显示控件
--   date       : 2018-01-15
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

require "Logic.UICtrlLogic.LimitCardShow.CardsHangItem"

UIPKLimitCardShowCtrl = class("UIPKLimitCardShowCtrl")

-- 定义扑克不同玩法，牌的默认数量
local PKDefaultNum = 
{
	[Common_PlayID.ThirtyTwo] = 2,
}

function UIPKLimitCardShowCtrl:ctor(rootTrans, luaWindowRoot, playID)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	self.playID = playID

	self:InitComponents()
	self:InitValues()
	self:InitItems()

	self:RegisterEvent()
end

function UIPKLimitCardShowCtrl:InitValues()
	self.playLogic = PlayGameSys.GetPlayLogic()
	-- 默认数量
	self.HandCardsDefaultNum = PKDefaultNum[self.playID]
	if self.HandCardsDefaultNum == nil then
		logError("Not have this play config. Please config PKDefault or check init playID, playID is : " .. tostring(self.playID) .. ". Now Set Default Num 2")
		self.HandCardsDefaultNum = 2
	end

	self.HangRootList = {}
	self.isRegisterTimer = false
	self.totalCardCount = 0
	self.curCardIndex = 1
	self.curSeatIndex = 1
	self.timer = 0
end

function UIPKLimitCardShowCtrl:InitComponents()

end

function UIPKLimitCardShowCtrl:InitItems()
	local north = CardsHangItem.New(self.luaWindowRoot:GetTrans(self.rootTrans, "north_node"), SeatPosEnum.North, self.luaWindowRoot)
	local south = CardsHangItem.New(self.luaWindowRoot:GetTrans(self.rootTrans, "south_node"), SeatPosEnum.South, self.luaWindowRoot)
	local west = CardsHangItem.New(self.luaWindowRoot:GetTrans(self.rootTrans, "west_node"), SeatPosEnum.West, self.luaWindowRoot)
	local east = CardsHangItem.New(self.luaWindowRoot:GetTrans(self.rootTrans, "east_node"), SeatPosEnum.East, self.luaWindowRoot)

	self.HangRootList[SeatPosEnum.North] = north
	self.HangRootList[SeatPosEnum.South] = south
	self.HangRootList[SeatPosEnum.West] = west
	self.HangRootList[SeatPosEnum.East] = east
end

-------------------------------------------------------------------------------------------------------
function UIPKLimitCardShowCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.ThirtyTwo_InitHandCards, self.InitHandCards, self)
	LuaEvent.AddHandle(EEventType.ThirtyTwo_OpenCards, self.OpenCards, self)
	LuaEvent.AddHandle(EEventType.ThirtyTwo_RefreshHandCards, self.RefreshHandCards, self)
	LuaEvent.AddHandle(EEventType.HideHandCard, self.DealHideCardsEvent, self)
end

function UIPKLimitCardShowCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.ThirtyTwo_InitHandCards, self.InitHandCards, self)
	LuaEvent.RemoveHandle(EEventType.ThirtyTwo_OpenCards, self.OpenCards, self)
	LuaEvent.RemoveHandle(EEventType.ThirtyTwo_RefreshHandCards, self.RefreshHandCards, self)
	LuaEvent.RemoveHandle(EEventType.HideHandCard, self.DealHideCardsEvent, self)
end

--初始化手上的牌
function UIPKLimitCardShowCtrl:InitHandCards(eventId,p1,p2)
	local seatPos = p1
	self:GiveCards(nil, p1)
end

--玩家开牌
function UIPKLimitCardShowCtrl:OpenCards(eventId,p1,p2)
	local rsp = p1
	--self:PlayerOpenCard(rsp.seatId, rsp.pai)
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	local player = cardPlayLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if not player then
		return
	end

	local cards = {}
	for k, v in ipairs(rsp.pai) do
		local card = CCard.New()
		card:Init(v)
		table.insert(cards, card)
	end

	self:PlayerOpenCard(player.seatPos, cards)
	-- 增加牌型显示事件调度
	LuaEvent.AddEvent(EEventType.ShowPaiXing, rsp,true)
end

--整体刷新牌
function UIPKLimitCardShowCtrl:RefreshHandCards(eventId,p1,p2)
	local player
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	for i=0,3 do
		player = cardPlayLogic.roomObj.playerMgr:GetPlayerBySeatID(i)
		if player then
			local seatPos = player.seatPos
			local cards = ShallowCopy(player.cardMgr:GetHandCards())
			self:RecoveryGiveCardsToPlayer(seatPos, cards)
		end
	end
end

-- 隐藏牌
function UIPKLimitCardShowCtrl:DealHideCardsEvent(eventId,seatPos,p2)
	if seatPos then
		self:HidePlayerCards(seatPos)
	else
		self:HideAllPlayerCards()
	end
end
---------------------------------对外接口----------------------------------------
-- 移除指定位置的玩家
-- seatPos  : 玩家的座位信息，参见CommonEnum的SeatPosEnum
function UIPKLimitCardShowCtrl:RemovePlayer(seatPos)
	if seatPos == nil or self.HangRootList[seatPos] == nil then return end

	self.HangRootList[seatPos]:Destroy()
	self.HangRootList[seatPos] = nil
end

-- 发牌接口(给所有玩家发牌，cards给我发牌)
-- cards        : 要显示的牌
-- startSeatPos : 发牌的起始位置
-- describe     : cards的牌给我，其他玩家牌的数量为HandCardsDefaultNum
function UIPKLimitCardShowCtrl:GiveCards(cards, startSeatPos)
	if self.isRegisterTimer then
		UpdateBeat:Remove(self.DealCardsTimer, self)
		self.isRegisterTimer = false
	end

	if startSeatPos == nil then
		logError("Function GiveCards, startSeatPos is nil")
	end

	self:RemoveNoPlayerHangRoot()

	-- 设置每一边需要展示的牌
	for k, v in pairs(self.HangRootList) do
		if v ~= nil then
			if k == SeatPosEnum.South then
				--v:SetCardsNum(#cards)
				v:SetCardsNum(self.HandCardsDefaultNum)
				--v:Reset(cards)
				v:Reset()
			else
				v:SetCardsNum(self.HandCardsDefaultNum)
				v:Reset()
			end
		end
	end
	self:HideAllPlayerCards()

	--self.totalCardCount = self.HandCardsDefaultNum * 3 + #cards
	self.totalCardCount = self:GetTotalPlayerNum() * 2
	self.curSeatIndex = SeatPosSort[startSeatPos]
	self.curCardIndex = 1
	self.timer = 0

	if not self.isRegisterTimer then
		UpdateBeat:Add(self.DealCardsTimer, self)
		self.isRegisterTimer = true
	end
	
end

-- 发牌接口(给指定位置的玩家发牌)
-- seatPos  : 玩家的座位信息，参见CommonEnum的SeatPosEnum
-- cards    : 要显示的牌
-- describe : 当cards为空，牌的显示数量以cardNum为准，cardNum为空，自动赋值HandCardsDefaultNum的数量
--            当cards不为空，以cards牌的数量为准
-- 由于GiveCardsToPlayer，此窗口用于快速恢复指定玩家的牌
function UIPKLimitCardShowCtrl:RecoveryGiveCardsToPlayer(seatPos, cards, cardNum)
	if self.isRegisterTimer then
		UpdateBeat:Remove(self.DealCardsTimer, self)
		self.isRegisterTimer = false
	end
	
	local item = self.HangRootList[seatPos]
	if item == nil then return end
	local cardsNum = #cards
	if cardsNum ~= 0 then
		--排序下 防止变动
		 table.sort(cards, function(card1, card2)
			 	if card1 and card2 and card1.logicValue and card2.logicValue then
		 			return card1.logicValue > card2.logicValue
			 	else
		 			return 0
			 	end
		 	end)

		-- 牌面
		item:RecoveryCardsFront(cards)
	else
		-- 牌背
		cardNum = cardNum or self.HandCardsDefaultNum
		item:RecoveryCardsBack(cardNum)
	end
end

--------[[GiveCardsToPlayer与RecoveryGiveCardsToPlayer要同事修改]]--------

-- 亮牌接口
-- seatPos  : 玩家的座位信息，参见CommonEnum的SeatPosEnum
-- cards    : 要显示的牌
function UIPKLimitCardShowCtrl:PlayerOpenCard(seatPos, cards)
	if seatPos == nil then
		logError("seatPos is nil")
		return		
	end

	local item = self.HangRootList[seatPos]
	if item == nil then
		logError("Item is nil. Please check has deleted this pos or seatPos is exist. seatPos is : " .. tostring(seatPos))
		return
	end

	self:RecoveryGiveCardsToPlayer(seatPos, cards)
end

-- 隐藏所有玩家的牌
function UIPKLimitCardShowCtrl:HideAllPlayerCards()
	for k, v in pairs(self.HangRootList) do
		if v ~= nil then
			v:HideCards()
		end
	end
end

-- 隐藏玩家牌
function UIPKLimitCardShowCtrl:HidePlayerCards(seatPos)
	local item = self.HangRootList[seatPos]
	if item == nil then return end

	item:HideCards()
end

-- 销毁接口
function UIPKLimitCardShowCtrl:Destroy()
	if self.isRegisterTimer then
		UpdateBeat:Remove(self.DealCardsTimer, self)
		self.isRegisterTimer = false
	end

	for k, v in pairs(self.HangRootList) do
		if v ~= nil then
			v:Destroy()
			v = nil
		end
	end
	self.HangRootList = {}

	self.rootTrans = nil
	self.luaWindowRoot = nil
	self.playID = nil

	self:UnRegisterEvent()
end
--------------------------------------------------------------------------------

---------------------------------私有接口----------------------------------------
function UIPKLimitCardShowCtrl:RemoveNoPlayerHangRoot()
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	for k, v in pairs(SeatPosEnum) do
		local player = cardPlayLogic.roomObj:GetPlayerBySeatPos(v)
		if player == nil then
			self:RemovePlayer(v)
		end
	end
end

function UIPKLimitCardShowCtrl:GetTotalPlayerNum()
	local result = 0
	for k, v in pairs(self.HangRootList) do
		if v ~= nil then
			result = result + 1
		end
	end

	return result
end

function UIPKLimitCardShowCtrl:DealCardsTimer()
	self.timer = self.timer + UnityEngine.Time.deltaTime
	if self.timer > 0.08 then
		if self.curCardIndex <= self.totalCardCount then
			local item = self.HangRootList[SeatPosIndex[self.curSeatIndex]]
			if self.playLogic ~= nil then
				LuaEvent.AddEventNow(EEventType.ShowRemainCardNum, true, self.playLogic:GetRemain(true) - self.curCardIndex)
			end

			if item ~= nil then
				item:GiveACard()
				self.curCardIndex = self.curCardIndex + 1
			end
			
			self.curSeatIndex = self.curSeatIndex + 1
			if self.curSeatIndex > 4 then
				self.curSeatIndex = 1
			end
		elseif self.curCardIndex >= self.totalCardCount + 3 then
			LuaEvent.AddEventNow(EEventType.DealCardEnd)
			if self.playLogic:GetRemain(true) - self.totalCardCount ~= self.playLogic:GetRemain(false) then
				LuaEvent.AddEventNow(EEventType.ShowRemainCardNum, true, self.playLogic:GetRemain(false))
			end
			UpdateBeat:Remove(self.DealCardsTimer, self)
			self.isRegisterTimer = false
		else
			self.curCardIndex = self.curCardIndex + 1
		end
		self.timer = 0
	end
end
--------------------------------------------------------------------------------