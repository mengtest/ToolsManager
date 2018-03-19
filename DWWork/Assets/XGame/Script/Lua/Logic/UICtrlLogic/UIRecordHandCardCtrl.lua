--------------------------------------------------------------------------------
-- 	 File      : UIRecordHandCardCtrl.lua
--   author    : guoliang
--   function   : 回放UI手牌显示控制 
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UIRecordHandCardCtrl = class("UIRecordHandCardCtrl",nil)

function UIRecordHandCardCtrl:Init(rootTrans,luaWindowRoot,handCards)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	self.parentNodeTrans = {}
	self.parentNodeTrans[SeatPosEnum.South] = luaWindowRoot:GetTrans(rootTrans,"south_node")
	self.parentNodeTrans[SeatPosEnum.East] = luaWindowRoot:GetTrans(rootTrans,"east_node")
	self.parentNodeTrans[SeatPosEnum.North] = luaWindowRoot:GetTrans(rootTrans,"north_node")
	self.parentNodeTrans[SeatPosEnum.West] = luaWindowRoot:GetTrans(rootTrans,"west_node")
	self.cardTranItems = {}
	self.handCards = {}
	self.totalLen_vec= 1140 -- 水平手牌长度
	self.totalLen_hor= 1080 -- 垂直方向手牌长度
	self.CardResourceName = "card_item"
	self:ChangeValues()
	self:RegisterEvent()
end

function UIRecordHandCardCtrl:ChangeValues()
	if DataManager.GetCurPlayID() == Common_PlayID.DW_DouDiZhu then
		self.totalLen_vec = 1110
		self.CardResourceName = "card_item_2"
	end
end

function UIRecordHandCardCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.RecordInitHandCards,self.RecordInitHandCards,self)
	LuaEvent.AddHandle(EEventType.RecordHandCardClear,self.RecordHandCardClear,self)
end

function UIRecordHandCardCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.RecordInitHandCards,self.RecordInitHandCards,self)
	LuaEvent.RemoveHandle(EEventType.RecordHandCardClear,self.RecordHandCardClear,self)
end


function UIRecordHandCardCtrl:RecordInitHandCards(eventId,p1,p2)
	self:ClearCards()

	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	local player
	for i=0,3 do
		player = cardPlayLogic.roomObj.playerMgr:GetPlayerBySeatID(i)
		if player then
			self.cardTranItems[player.seatPos] = {}
			self.handCards[player.seatPos] = ShallowCopy(player.cardMgr:GetHandCards())
			LogicUtil.SortCards(self.handCards[player.seatPos],false)
		end
	end
	self:InitCardsShow()
end


function UIRecordHandCardCtrl:RecordHandCardClear(eventId,p1,p2)
	self:ClearCards()
end


function UIRecordHandCardCtrl:InitCardsShow()
	local resObj
	local cardCount = 0
	for seatPos,v_playCards in pairs(self.handCards) do
		cardCount = #v_playCards
		for k,v in pairs(v_playCards) do
			local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, self.CardResourceName, RessStorgeType.RST_Never, false)
			if not resObj then
				return
			end
			LogicUtil.EnableTouch(resObj.transform, false)
			local trans = resObj.transform
			trans.parent = self.parentNodeTrans[seatPos]
			trans.name = k.."_cardItem"
			trans.localScale = UnityEngine.Vector3.New(1,1,1)
			trans.localPosition = UnityEngine.Vector3.New(0,0,0)
			self.luaWindowRoot:SetActive(trans,true)
			if self.CardResourceName == "card_item_2" then
				LogicUtil.InitCardItem_2(self.luaWindowRoot, trans, v, k, false, false)
			else
				LogicUtil.InitCardItem(self.luaWindowRoot,trans,v,k)
			end
			local cardTranItems = {}
			cardTranItems.trans = trans
			cardTranItems.cardInfo = v
			table.insert(self.cardTranItems[seatPos],cardTranItems)
		end
		if seatPos == SeatPosEnum.South or seatPos == SeatPosEnum.North then
			LogicUtil.AdjustCardsForOutCards(cardCount,self.totalLen_vec,self.cardTranItems[seatPos],seatPos)
		else
			LogicUtil.AdjustCardsForOutCards(cardCount,self.totalLen_hor,self.cardTranItems[seatPos],seatPos)
		end
	end


end


--出牌
function UIRecordHandCardCtrl:OutCards(player,outCardIds)
	if player then
		local seatPos = player.seatPos
		local newHandCards = {}
		local newCardTrans = {}
		local outCards = {}
		local cardCount = #outCardIds
		local containFlags = {}
		local chooseHandCards = {}
		for k1,v1 in pairs(self.cardTranItems[seatPos]) do
			for k,v in pairs(outCardIds) do
				if containFlags[k] == nil then
					if v1.cardInfo.ID == v and chooseHandCards[v1.cardInfo.seq] == nil then
						self.luaWindowRoot:SetActive(v1.trans,false)
						containFlags[k] = true
						chooseHandCards[v1.cardInfo.seq] = true
						isContain = true
						break
					end
				end
			end

			if chooseHandCards[v1.cardInfo.seq] == nil then
--				print("v1.cardInfo.ID "..v1.cardInfo.ID .."v1.cardInfo.seq ="..v1.cardInfo.seq.." not out")
				table.insert(newCardTrans,v1)
				table.insert(newHandCards,self.handCards[seatPos][k1])
			else
--				print("v1.cardInfo.ID "..v1.cardInfo.ID .."v1.cardInfo.seq ="..v1.cardInfo.seq.." out")
				table.insert(outCards,self.handCards[seatPos][k1])
			end
		end
--		print("OutCards before handCards count ="..#self.cardTranItems[seatPos])
		self.cardTranItems[seatPos] = newCardTrans
		self.handCards[seatPos] = newHandCards
--		print("OutCards count = " ..cardCount .. "remainCardCount" ..#newHandCards)

		if seatPos == SeatPosEnum.South or seatPos == SeatPosEnum.North then
			LogicUtil.AdjustCardsForOutCards(#newHandCards,self.totalLen_vec,self.cardTranItems[seatPos],seatPos)
		else
			LogicUtil.AdjustCardsForOutCards(#newHandCards,self.totalLen_hor,self.cardTranItems[seatPos],seatPos)
		end

		-- 更新手牌
		player.cardMgr:DeleteCards(outCards)
	end
end



function UIRecordHandCardCtrl:ClearCards()
	for seatPos,v_cardTranItems in pairs(self.cardTranItems) do
		for k,v_cardTransItem in pairs(v_cardTranItems) do
			self.luaWindowRoot:SetActive(v_cardTransItem.trans,false)
		end
	end
	self.cardTranItems = {}
	self.handCards = {}
end

-- 销毁
function UIRecordHandCardCtrl:Destroy()
	self:UnRegisterEvent()

	for seatPos,v_cardTranItems in pairs(self.cardTranItems) do
		for k,v_cardTransItem in pairs(v_cardTranItems) do
			self.luaWindowRoot:SetActive(v_cardTransItem.trans,false)
		end
	end
	self.cardTranItems = nil
	self.handCards = nil
end