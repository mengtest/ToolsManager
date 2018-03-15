--------------------------------------------------------------------------------
-- 	 File      : UIMJRecordHandCardCtrl.lua
--   author    : 
--   function   : UI 麻将手牌显示控制 
--   date      : 2017-11-13
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UIMJRecordHandCardCtrl = class("UIMJRecordHandCardCtrl",nil)

local MJRecordCardItemName = 
{
	[SeatPosEnum.South] = "mjcard_south_record_item",
	[SeatPosEnum.East] = "mjcard_east_record_item",
	[SeatPosEnum.North] = "mjcard_north_record_item",
	[SeatPosEnum.West] = "mjcard_west_record_item",
}

function UIMJRecordHandCardCtrl:Init(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans
	--打出的牌的父节点

	self.parentNodeTrans = {}
	self.parentNodeTrans[SeatPosEnum.South] = luaWindowRoot:GetTrans(rootTrans,"south_node")
	self.parentNodeTrans[SeatPosEnum.East] = luaWindowRoot:GetTrans(rootTrans,"east_node")
	self.parentNodeTrans[SeatPosEnum.North] = luaWindowRoot:GetTrans(rootTrans,"north_node")
	self.parentNodeTrans[SeatPosEnum.West] = luaWindowRoot:GetTrans(rootTrans,"west_node")

	self.luaWindowRoot = luaWindowRoot
	self.cardItems = {}

	self:RegisterEvent()
end

---------------------------event zone ------------------------------------
function UIMJRecordHandCardCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.MJRecordInitHandCards, self.InitHandCards, self)
	LuaEvent.AddHandle(EEventType.MJRecordInCard, self.InCard, self)
	LuaEvent.AddHandle(EEventType.MJRecordRemoveCards, self.RemoveCards, self)
	LuaEvent.AddHandle(EEventType.MJRecordClearHandCards, self.ClearHandCards, self)
	LuaEvent.AddHandle(EEventType.ChangePlayCanvas, self.ChangePlayCanvas, self)
end

function UIMJRecordHandCardCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.MJRecordInitHandCards, self.InitHandCards, self)
	LuaEvent.RemoveHandle(EEventType.MJRecordInCard, self.InCard, self)
	LuaEvent.RemoveHandle(EEventType.MJRecordRemoveCards, self.RemoveCards, self)
	LuaEvent.RemoveHandle(EEventType.MJRecordClearHandCards, self.ClearHandCards, self)
	LuaEvent.RemoveHandle(EEventType.ChangePlayCanvas, self.ChangePlayCanvas, self)
end

-- 初始化手牌
function UIMJRecordHandCardCtrl:InitHandCards(eventId, p1, p2)
	self:InitCardsShow()
end

-- 清理手牌
function UIMJRecordHandCardCtrl:ClearHandCards(eventId, p1, p2)
	self:ClearCards()
end


function UIMJRecordHandCardCtrl:ChangePlayCanvas(eventId, p1, p2)
	local titleIndex = p2
	if titleIndex then
		for seatPos,v_cardItems in pairs(self.cardItems) do
			LogicUtil.ChangeMJPaiBgs(self.luaWindowRoot, titleIndex, seatPos, v_cardItems, true)
		end
	end
end

function UIMJRecordHandCardCtrl:RefreshCard(seatPos)
	local cardItems = self.cardItems[seatPos]
	local count = #cardItems
	for i=1,count do
		local cardItem = cardItems[i]
		local trans = cardItem.trans
		LogicUtil.AdjustMJCards(trans, i, count, seatPos, true)
		
		if SeatPosEnum.East == seatPos then
			LogicUtil.InitMJCardItem(self.luaWindowRoot, trans, cardItem.cardInfo, nil, i)
		else
			LogicUtil.InitMJCardItem(self.luaWindowRoot, trans, cardItem.cardInfo, nil, 14 - i)
		end
	end
end

-- 抓牌
-- cardinfo --> CMJCard 抓牌信息
function UIMJRecordHandCardCtrl:_inCard(player, cardInfo)
	if not player then
		return
	end

	local seatPos = player.seatPos
	local trans = self:_initMJCardItem(seatPos, 14, 14, cardInfo)

	self:RefreshCard(seatPos)
end

function UIMJRecordHandCardCtrl:InCard(eventId, p1, p2)
	if nil == p1 or nil == p2 then
		return
	end

	self:_inCard(p1, p2)
end

-- 碰、杠、吃、出牌移除手牌
-- outCardIds --> {id1, id2} 移除牌id
function UIMJRecordHandCardCtrl:_removeCards(player, outCardIds)
	if not player then
		return
	end

	local tempOutCardIds = ShallowCopy(outCardIds)
	-- 添加限制 能杠的情况下碰
	if 3 == #tempOutCardIds then
		tempOutCardIds[3] = nil
	end

	local seatPos = player.seatPos
	local cardItems = self.cardItems[seatPos]

	local newCardItems = {}
	local isContain = false

	if cardItems then
		for k1,v1 in pairs(cardItems) do
			isContain = false
			for k,v in pairs(tempOutCardIds) do
				if v1.cardInfo.ID == v then
					self.luaWindowRoot:SetActive(v1.trans, false)
					isContain = true
					tempOutCardIds[k] = nil
					break
				end
			end

			if not isContain then
				table.insert(newCardItems, v1)
			end
		end
	else 
		DwDebug.LogError("nil cardItems")
	end

	LogicUtil.SortMJCardsByItems(newCardItems, true)

	self.cardItems[seatPos] = newCardItems

	self:RefreshCard(seatPos)
end

function UIMJRecordHandCardCtrl:RemoveCards(eventId, p1, p2)
	if nil == p1 or nil == p2 then
		return
	end

	self:_removeCards(p1, p2)
end

function UIMJRecordHandCardCtrl:_initMJCardItem(seatPos, index, count, cardInfo)
	local cardItemName = MJRecordCardItemName[seatPos]
	local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, cardItemName, RessStorgeType.RST_Never, false)
	if not resObj then
		return
	end
	local trans = resObj.transform
	trans.parent = self.parentNodeTrans[seatPos]
	trans.name = index.."_cardItem"
	trans.localScale = Vector3.one
	LogicUtil.AdjustMJCards(trans, index, count, seatPos, true)
	self.luaWindowRoot:SetActive(trans, true)

	if SeatPosEnum.East == seatPos then
		LogicUtil.InitMJCardItem(self.luaWindowRoot, trans, cardInfo, nil, index)
	else
		LogicUtil.InitMJCardItem(self.luaWindowRoot, trans, cardInfo, nil, 14 - index)
	end
	LogicUtil.ChangeSingleMJPaiBg(self.luaWindowRoot, LogicUtil.GetMJPaiType(), seatPos, trans, 2)
	local cardItems = {}
	cardItems.trans = trans
	cardItems.cardInfo = cardInfo
	table.insert(self.cardItems[seatPos], cardItems)

	return trans
end

function UIMJRecordHandCardCtrl:InitCardsShow()
	local player
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	for i=0,3 do
		player = cardPlayLogic.roomObj.playerMgr:GetPlayerBySeatID(i)
		if player and player.seatPos then
			local seatPos = player.seatPos
			self.cardItems[seatPos] = {}

			local cards = ShallowCopy(player.cardMgr:GetHandCards())
			LogicUtil.SortMJCards(cards, true)
			local count = #cards
			for i=1, count do
				self:_initMJCardItem(seatPos, i, count, cards[i])
			end
		end
	end
end

function UIMJRecordHandCardCtrl:ClearCards()
	for seatPos,v_cardItems in pairs(self.cardItems) do
		for k,v_cardTransItem in pairs(v_cardItems) do
			self.luaWindowRoot:SetActive(v_cardTransItem.trans,false)
		end
	end

	self.cardItems = {}
end

function UIMJRecordHandCardCtrl:Destroy()
	self:UnRegisterEvent()
	self:ClearCards()
end

  