--------------------------------------------------------------------------------
-- 	 File      : UIOutCardShowCtrl.lua
--   author    : guoliang
--   function   : UI出牌显示控制 
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UIOutCardShowCtrl = class("UIOutCardShowCtrl",nil)

local CardItemConfig = 
{
	card_item = "card_item",
	card_item_2 = "card_item_2",
}

local CardScaleConfig = 
{
	[Common_PlayID.chongRen_510K] = 
	{
		normal = Vector3.one,
		bigger = Vector3.one *1.5,
	},
	[Common_PlayID.DW_DouDiZhu] = 
	{
		normal = Vector3.one * 1.1,
		bigger = Vector3.one *1.4,
	},
}

function UIOutCardShowCtrl:Init(rootTrans,luaWindowRoot)
	self.rootTrans = rootTrans
	self.southRootTrans = luaWindowRoot:GetTrans(rootTrans,"south_node")
	self.eastRootTrans = luaWindowRoot:GetTrans(rootTrans,"east_node")
	self.northRootTrans = luaWindowRoot:GetTrans(rootTrans,"north_node")
	self.westRootTrans = luaWindowRoot:GetTrans(rootTrans,"west_node")
	self.cardTypeShowRoot = luaWindowRoot:GetTrans(rootTrans, "card_type_show_root")
	self.luaWindowRoot = luaWindowRoot
	self.totalLen_upDown = 1140
	self.totalLen_leftRight= 1080
	self:InitCardTypeShowRoot()
	self.seatCardTransMap = {}
	self:RegisterEvent()
	self:InitValues()
end

function UIOutCardShowCtrl:InitValues()
	self.playID = PlayGameSys.GetNowPlayId()

	self.CardResourceName = CardItemConfig.card_item
	if self.playID == Common_PlayID.DW_DouDiZhu then
		self.CardResourceName = CardItemConfig.card_item_2
	end

	self.CardScale = CardScaleConfig[Common_PlayID.chongRen_510K]
	if self.playID == Common_PlayID.DW_DouDiZhu then
		self.CardScale = CardScaleConfig[Common_PlayID.DW_DouDiZhu]
	end
end

function UIOutCardShowCtrl:InitCardTypeShowRoot()
	self.southShowRoot = self.luaWindowRoot:GetTrans(self.cardTypeShowRoot, "south_show")
	self.eastShowRoot = self.luaWindowRoot:GetTrans(self.cardTypeShowRoot, "east_show")
	self.northShowRoot = self.luaWindowRoot:GetTrans(self.cardTypeShowRoot, "north_show")
	self.westShowRoot = self.luaWindowRoot:GetTrans(self.cardTypeShowRoot, "west_show")

	-- 将下面的节点隐藏
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.southShowRoot, "zha_show"), false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.eastShowRoot, "zha_show"), false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.northShowRoot, "zha_show"), false)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.westShowRoot, "zha_show"), false)
end

function UIOutCardShowCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.TanPaiAction,self.TanPaiAction,self)
end

function UIOutCardShowCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.TanPaiAction,self.TanPaiAction,self)
	
end

function UIOutCardShowCtrl:PlayOutCardsShow(seatPos,showCards)
	if not seatPos then
		DwDebug.LogError("PlayOutCardsShow seatPos is nil")
	end
--	print("PlayOutCardsShow " .. seatPos)
	self:ClearCards(seatPos)

	local sortCards = nil
	if self.playID == Common_PlayID.DW_DouDiZhu then
		sortCards = LogicUtil.SortOutCardsDDZ(showCards)
	else
		sortCards = LogicUtil.SortOutCards(showCards)
	end
	
	local parentTrans = self:GetSeatRootTrans(seatPos)

	local resObj
	local cardCount = #sortCards
--	print("#sortCards"..#sortCards)
	local cardTranItems = {}
	local scale = self.CardScale.normal
	if #sortCards <= 6 then
		scale = self.CardScale.bigger
	end
	for k,v in pairs(sortCards) do
		local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, self.CardResourceName, RessStorgeType.RST_Never, false)
		if not resObj then
			return
		end	

		LogicUtil.EnableTouch(resObj.transform, false)

		local trans = resObj.transform
		trans.parent = parentTrans
		self.luaWindowRoot:SetActive(trans,true)
		trans.name = k.."_cardItem"
		trans.localScale = scale
		v.typeTag = 0
		self:InitCardShow(self.luaWindowRoot, trans, v, k)
		
		local cardTranItem = {}
		cardTranItem.trans = trans
		cardTranItem.cardInfo = v
		table.insert(cardTranItems,cardTranItem)
	end
	
	local totalLen = self.totalLen_upDown
	if seatPos == SeatPosEnum.East or seatPos == SeatPosEnum.West then
		totalLen = self.totalLen_leftRight
	end
	LogicUtil.AdjustCardsForOutCards(cardCount,totalLen,cardTranItems,seatPos)
	self.seatCardTransMap[seatPos] = cardTranItems
	self:SetLastCardTransLordTag(seatPos)
end

function UIOutCardShowCtrl:ShowBombTip(seatPos, bombNum)
	local parentTrans = self:GetCardShowRootTrans(seatPos)
	local zhaTrans = self.luaWindowRoot:GetTrans(parentTrans, "zha_show")
	if bombNum >= 4 and bombNum <= 8 then
		self.luaWindowRoot:SetActive(zhaTrans, true)
		self.luaWindowRoot:SetSprite(self.luaWindowRoot:GetTrans(zhaTrans, "num"), "zha_" .. bombNum) 
	else
		self.luaWindowRoot:SetActive(zhaTrans, false)
	end
	
end

function UIOutCardShowCtrl:HideBombTip(seatPos)
	local parentTrans = self:GetCardShowRootTrans(seatPos)
	self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(parentTrans, "zha_show"), false)
end

function UIOutCardShowCtrl:HideAllBombTip()
	self:HideBombTip(SeatPosEnum.South)
	self:HideBombTip(SeatPosEnum.East)
	self:HideBombTip(SeatPosEnum.North)
	self:HideBombTip(SeatPosEnum.West)
end


function UIOutCardShowCtrl:ClearCards(seatPos)
--	print("ClearCards "..seatPos)
	self:HideCards(seatPos)
	self.seatCardTransMap[seatPos] = nil
end

function UIOutCardShowCtrl:HideCards(seatPos)
	local cardTranItems = self.seatCardTransMap[seatPos]
	if cardTranItems then
		for k,v in pairs(cardTranItems) do
--			print("HideCards SetActive  "..v.name)
			self.luaWindowRoot:SetActive(v.trans,false)
		end
	end
end

function UIOutCardShowCtrl:TanPaiAction(eventId,p1,p2)
	local seatPos = p1
	local pai = p2
	local srcCards = {}
	local cardItem
	for k,v in pairs(pai) do
		cardItem = CCard.New()
		cardItem:Init(v,k)
		table.insert(srcCards,cardItem)
	end
	self:PlayOutCardsShow(seatPos,srcCards)
end


function UIOutCardShowCtrl:ClearAllCards()
--	print("ClearAllCards")
	for k,v in pairs(self.seatCardTransMap) do
		self:HideCards(k)
	end
	self.seatCardTransMap = {}
end

function UIOutCardShowCtrl:GetSeatRootTrans(seatPos)
	if seatPos == SeatPosEnum.South then
		return self.southRootTrans
	elseif seatPos == SeatPosEnum.East then
		return self.eastRootTrans
	elseif seatPos == SeatPosEnum.North then
		return self.northRootTrans
	else
		return self.westRootTrans
	end 
end

function UIOutCardShowCtrl:GetCardShowRootTrans(seatPos)
	if seatPos == SeatPosEnum.South then
		return self.southShowRoot
	elseif seatPos == SeatPosEnum.East then
		return self.eastShowRoot
	elseif seatPos == SeatPosEnum.North then
		return self.northShowRoot
	else
		return self.westShowRoot
	end
end

function UIOutCardShowCtrl:InitCardShow(luaWindowRoot,trans, card, index)
	if self.CardResourceName == CardItemConfig.card_item then
		LogicUtil.InitCardItem(luaWindowRoot, trans, card, index)
	elseif self.CardResourceName == CardItemConfig.card_item_2 then
		LogicUtil.InitCardItem_2(luaWindowRoot, trans, card, index, false, false)
	end
end

function UIOutCardShowCtrl:SetLastCardTransLordTag(seatPos)
	if self.playID == Common_PlayID.DW_DouDiZhu then
		local cardLogic = PlayGameSys.GetPlayLogic()
		local lordPlayer = cardLogic.roomObj.playerMgr:GetPlayerBySeatID(cardLogic.bankerSeatId)
		if lordPlayer then
			local isLord = lordPlayer.seatPos == seatPos
			if isLord then
				local transItems = self.seatCardTransMap[seatPos]
				if #transItems > 0 then
					local lastCardTrans = transItems[#transItems].trans
					if lastCardTrans then
						LogicUtil.SetCardLordTag(self.luaWindowRoot, lastCardTrans, isLord)
					end
				end
			end
		end
	end
end

-- 销毁
function UIOutCardShowCtrl:Destroy()
--	print("UIOutCardShowCtrl:Destroy")
	self:UnRegisterEvent()
	self:ClearAllCards()
	self.seatCardTransMap = {}
end