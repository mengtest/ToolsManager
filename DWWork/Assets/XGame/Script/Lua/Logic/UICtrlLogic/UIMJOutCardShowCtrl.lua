--------------------------------------------------------------------------------
-- 	 File      : UIMJOutCardShowCtrl.lua
--   author    : guoliang
--   function   : UI 麻将出牌显示控制
--   date      : 2017-11-8
--   copyright : Copyright 2017 DW Inc.P
--------------------------------------------------------------------------------
require "Logic.MJCardLogic.CMJCard"

UIMJOutCardShowCtrl = class("UIMJOutCardShowCtrl",nil)

function UIMJOutCardShowCtrl:Init(rootTrans,luaWindowRoot)
	self.rootTrans = rootTrans
	self.outPosRootTrans = luaWindowRoot:GetTrans(rootTrans,"out_card_pos")
	self.ownOutPosRootTrans = luaWindowRoot:GetTrans(rootTrans,"own_out_card_pos")
	--打出的牌的父节点
	self.southRootTrans = luaWindowRoot:GetTrans(self.outPosRootTrans,"south_node")
	self.eastRootTrans = luaWindowRoot:GetTrans(self.outPosRootTrans,"east_node")
	self.northRootTrans = luaWindowRoot:GetTrans(self.outPosRootTrans,"north_node")
	self.westRootTrans = luaWindowRoot:GetTrans(self.outPosRootTrans,"west_node")
	--碰、杠、吃的节点
	self.own_southRootTrans = luaWindowRoot:GetTrans(self.ownOutPosRootTrans,"south_node")
	self.own_eastRootTrans = luaWindowRoot:GetTrans(self.ownOutPosRootTrans,"east_node")
	self.own_northRootTrans = luaWindowRoot:GetTrans(self.ownOutPosRootTrans,"north_node")
	self.own_westRootTrans = luaWindowRoot:GetTrans(self.ownOutPosRootTrans,"west_node")


	self.luaWindowRoot = luaWindowRoot
	self.outCardTransMap = {}
	self.own_outCardTransMap = {}
	self:RegisterEvent()
	self.MJSelCardHighlightTransMap = nil

	self.gangCardBg = {"gameuilayer_bm_yellow", "gameuilayer_bm_blue", "gameuilayer_bm_blue"}

	local up,down,left,right = -90,90,0,180
	self.cfg_arrow_rotation = {
		[SeatPosEnum.South] = {
			[SeatPosEnum.South] = down,
			[SeatPosEnum.East] = right,
			[SeatPosEnum.West] = left,
			[SeatPosEnum.North] = up,
		},
		[SeatPosEnum.East] = {
			[SeatPosEnum.South] = down,
			[SeatPosEnum.East] = right,
			[SeatPosEnum.West] = left,
			[SeatPosEnum.North] = up,
		},
		[SeatPosEnum.West] = {
			[SeatPosEnum.South] = down,
			[SeatPosEnum.East] = right,
			[SeatPosEnum.West] = left,
			[SeatPosEnum.North] = up,
		},
		[SeatPosEnum.North] = {
			[SeatPosEnum.South] = down,
			[SeatPosEnum.East] = right,
			[SeatPosEnum.West] = left,
			[SeatPosEnum.North] = up,
		},
	}
end

---------------------------event zone ------------------------------------
function UIMJOutCardShowCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.MJ_NormalOutMJCardCatch,self.NormalOutMJCardCatch,self)--打出的牌被吃、碰、杠走UI通知
	LuaEvent.AddHandle(EEventType.MJ_PlayQiangGangShow, self.MJ_PlayQiangGangShow, self)--抢杠胡显示
	LuaEvent.AddHandle(EEventType.MJ_PlayNormalOutMJCardShow,self.MJ_PlayNormalOutMJCardShow,self)-- 正常出一张牌UI通知
	LuaEvent.AddHandle(EEventType.MJ_PlayOwnOutMJCardsShow,self.MJ_PlayOwnOutMJCardsShow,self) -- 吃碰杠UI通知
	LuaEvent.AddHandle(EEventType.MJ_PlayOwnSupplyGangShow,self.MJ_PlayOwnSupplyGangShow,self) -- 自己抓牌杠UI通知
	LuaEvent.AddHandle(EEventType.MJ_ClearAllOutCards,self.MJ_ClearAllOutCards,self) -- 清楚所有的牌
	LuaEvent.AddHandle(EEventType.ChangePlayCanvas, self.ChangePlayCanvas, self) -- 背景切换
	LuaEvent.AddHandle(EEventType.MJSelCardHighlight, self.MJSelCardHighlight, self) -- 高亮选中牌
	LuaEvent.AddHandle(EEventType.MJAnGangTanPai, self.MJAnGangTanPai, self) -- 摊开暗杠的牌
end

function UIMJOutCardShowCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.MJ_NormalOutMJCardCatch,self.NormalOutMJCardCatch,self)
	LuaEvent.RemoveHandle(EEventType.MJ_PlayQiangGangShow, self.MJ_PlayQiangGangShow, self)
	LuaEvent.RemoveHandle(EEventType.MJ_PlayNormalOutMJCardShow,self.MJ_PlayNormalOutMJCardShow,self)
	LuaEvent.RemoveHandle(EEventType.MJ_PlayOwnOutMJCardsShow,self.MJ_PlayOwnOutMJCardsShow,self)
	LuaEvent.RemoveHandle(EEventType.MJ_PlayOwnSupplyGangShow,self.MJ_PlayOwnSupplyGangShow,self)
	LuaEvent.RemoveHandle(EEventType.MJ_ClearAllOutCards,self.MJ_ClearAllOutCards,self)
	LuaEvent.RemoveHandle(EEventType.ChangePlayCanvas, self.ChangePlayCanvas, self)
	LuaEvent.RemoveHandle(EEventType.MJSelCardHighlight, self.MJSelCardHighlight, self)
	LuaEvent.RemoveHandle(EEventType.MJAnGangTanPai, self.MJAnGangTanPai, self) -- 摊开暗杠的牌
end

-- 通知摊牌，把暗杠摊开
function UIMJOutCardShowCtrl:MJAnGangTanPai(event_id, p1, p2)
	for seatPos,v in pairs(self.own_outCardTransMap or {}) do
		for sort,cardTranItems in pairs(v or {}) do
			local _cardTranItems = cardTranItems.cards
			for k,item in pairs(_cardTranItems or {}) do
				if item and item.isTanPaiAnGang and item.trans then
					item.trans.localScale = UnityEngine.Vector3.one
				end
			end
		end
	end
end

-- 外面消息通知某个座位打出去的牌被吃或者碰或者杠走
function UIMJOutCardShowCtrl:NormalOutMJCardCatch(eventId,p1,p2)
	local seatPos,mjCardID = p1,p2
	if self.outCardTransMap[seatPos] then
		local cardCount = #self.outCardTransMap[seatPos]
		if cardCount  > 0 then
			--可以肯定的是对应玩家的打出最后一张牌是抓走的
			local cardTranItem = self.outCardTransMap[seatPos][cardCount]
			if cardTranItem and cardTranItem.cardInfo.ID == mjCardID then
				self.luaWindowRoot:SetActive(cardTranItem.trans,false)
				table.remove(self.outCardTransMap[seatPos],cardCount)
				self:PopSelectedHighlight(mjCardID)
			else
				print("NormalOutMJCardCatch mjCardID is not find   saveID = "..cardTranItem.cardInfo.ID .. " mjCardID = "..mjCardID)
			end
		end
	end
end
--外部消息通知某个座位打出一张牌
function UIMJOutCardShowCtrl:MJ_PlayNormalOutMJCardShow(eventId, p1, p2)
	local seatPos, mjCardID, showArrow,notReconnect = p1, p2[1], p2[2],p2[3]
	local mjCard = CMJCard.New()
	if self.outCardTransMap[seatPos] == nil then
		self.outCardTransMap[seatPos] = {}
	end
	mjCard:Init(mjCardID,#self.outCardTransMap[seatPos] + 1)
	local trans = self:PlayNormalOutMJCardShow(seatPos,mjCard)
	if nil ~= trans and showArrow then
		AnimManager.PlayArrowAnim(true, trans.position)
	end

	--重连不播放声音
	if notReconnect then
		AudioManager.PlayCommonSound(UIAudioEnum.mj_paiLuoZhuo)
	end
end

function UIMJOutCardShowCtrl:genArrow(luaWindowRoot,index,parentTrans,owner,provider,rotation )
	-- body
	local resName = "mj_group_arrow_item"

	local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, resName, RessStorgeType.RST_Never, false)
	if not resObj then
		return
	end
	local trans = resObj.transform
	trans.parent = parentTrans
	luaWindowRoot:SetActive(trans,true)
	trans.name = "mj_group_arrow_item_"..index

	trans.localEulerAngles = UnityEngine.Vector3.New(0,0,rotation)
	trans.localScale = UnityEngine.Vector3.one

	if owner == SeatPosEnum.South then
		trans.localPosition = UnityEngine.Vector3.New((index-1)*242+147,28,0)
	elseif owner == SeatPosEnum.North then
		trans.localPosition = UnityEngine.Vector3.New((index-1)*-242+10,26,0)
	elseif owner == SeatPosEnum.East then
		trans.localPosition = UnityEngine.Vector3.New(15,28+(index-1)*-110,0)
	else
		trans.localPosition = UnityEngine.Vector3.New(140,28+(index-1)*-110,0)
	end


	luaWindowRoot:SetDepth(trans,104)

	return trans
end
-- 外部消息通知某个座位碰、吃、杠牌
function UIMJOutCardShowCtrl:MJ_PlayOwnOutMJCardsShow(eventId,p1,p2)
	local seatPos,providerSeatPos,mjCardIDs = p1[1],p1[2],p2

	if self.own_outCardTransMap[seatPos] == nil then
		self.own_outCardTransMap[seatPos] = {}
	end
	local luaWindowRoot = self.luaWindowRoot
	local parentTrans = self:GetOwnOutCardParentTrans(seatPos)
	local cardTranItem = {}
	local mjCard
	local sort = #self.own_outCardTransMap[seatPos]+1 -- 当前是第几组了
	if self.own_outCardTransMap[seatPos][sort] == nil then
		self.own_outCardTransMap[seatPos][sort] = {}
	end
	self.own_outCardTransMap[seatPos][sort].cards = {}
	local rotation = self.cfg_arrow_rotation[seatPos][providerSeatPos]
	if not rotation then print(rotation) return end
	self.own_outCardTransMap[seatPos][sort].pointer = self:genArrow(luaWindowRoot,sort,parentTrans,seatPos,providerSeatPos,rotation)
	for k,v in pairs(mjCardIDs) do
		local trans = self:CreateMJCardItem(seatPos,parentTrans,k)
		if trans then
			mjCard = CMJCard.New()
			mjCard:Init(v,k)
			--初始化牌的显示及位置
			LogicUtil.SetOwnOutMJCardShow(seatPos,luaWindowRoot,trans,sort,mjCard,k)

			cardTranItem = {}
			cardTranItem.trans = trans
			cardTranItem.cardInfo = mjCard
			table.insert(self.own_outCardTransMap[seatPos][sort].cards,cardTranItem)
		end
	end

	AnimManager.PlayArrowAnim(false)
end

-- 外部消息通知某个座位自摸杠牌
function UIMJOutCardShowCtrl:MJ_PlayOwnSupplyGangShow(eventId,p1,p2)
	local seatPos,mjCardIDs = p1,p2
	if self.own_outCardTransMap[seatPos] == nil then
		self.own_outCardTransMap[seatPos] = {}
	end

	local parentTrans = self:GetOwnOutCardParentTrans(seatPos)
	local ownCardMap = self.own_outCardTransMap[seatPos]
	if ownCardMap then
		--是否补杠
		for k,v in pairs(ownCardMap) do
			local v1 = v.cards

			if #v1 and v1[1].cardInfo.ID == mjCardIDs[1] then
				print("bu gang")

				if #v1 >=  4 then
				 	-- 容错（服务端发送了错误的牌）
					print("MJ_PlayOwnSupplyGangShow gang num more than 4 id = "..mjCardIDs[1])
					return
				end

				local mjCard = CMJCard.New()
				mjCard:Init(mjCardIDs[1],4)
				local trans = self:CreateMJCardItem(seatPos,parentTrans,4)
				--初始化牌的显示及位置
				LogicUtil.SetOwnOutMJCardShow(seatPos,self.luaWindowRoot,trans,k,mjCard,4)

				local cardTranItem = {}
				cardTranItem.trans = trans
				cardTranItem.cardInfo = mjCard
				table.insert(self.own_outCardTransMap[seatPos][k].cards,cardTranItem)
				return
			end
		end

		--暗杠
		self:PlayerAnGangShow(seatPos,mjCardIDs)
	end

end

-- 暗杠
function UIMJOutCardShowCtrl:PlayerAnGangShow(seatPos,mjCardIDs)
	if self.own_outCardTransMap[seatPos] == nil then
		self.own_outCardTransMap[seatPos] = {}
	end
	local luaWindowRoot = self.luaWindowRoot
	local parentTrans = self:GetOwnOutCardParentTrans(seatPos)
	local cardTranItem = {}
	local mjCard
	local sort = #self.own_outCardTransMap[seatPos]+1 -- 当前是第几组了
	if self.own_outCardTransMap[seatPos][sort] == nil then
		self.own_outCardTransMap[seatPos][sort] = {}
	end
	self.own_outCardTransMap[seatPos][sort].cards = {}
	if not self.cfg_arrow_rotation then
		DwDebug.Log( "not self.cfg_arrow_rotation")
		return
	end
	if not self.cfg_arrow_rotation[seatPos] then
		DwDebug.Log(" not self.cfg_arrow_rotation[seatPos]")
		return
	end
	local rotation = self.cfg_arrow_rotation[seatPos][seatPos]
	if not rotation then
		 DwDebug.Log("no rotation") 
		 return 
	end

	self.own_outCardTransMap[seatPos][sort].pointer = self:genArrow(luaWindowRoot,sort,parentTrans,seatPos,seatPos,rotation)

	-- 取是否回放
	local playLogic = PlayGameSys.GetPlayLogic()
	local playLogicType = playLogic.GetType()
	local is_record = false
	if playLogicType == PlayLogicTypeEnum.WSK_Record or playLogicType == PlayLogicTypeEnum.MJ_Record then
		is_record = true
	end

	for k,v in pairs(mjCardIDs) do
		local trans
		local tanpai_trans
		-- 此函数只显示暗杠。如果是自己的第四张、或者回放下所有人的第四张，则显示牌面,而不是牌背
		local show_back = not ((is_record or seatPos == SeatPosEnum.South) and k == 4)
		if not show_back then
			trans = self:CreateMJCardItem(seatPos,parentTrans,k)
		else
			trans = self:CreateMJCardItem(seatPos,parentTrans,k,true)
			-- 先创建出来摊牌的时候用的牌
			if k == 4 then
				tanpai_trans = self:CreateMJCardItem(seatPos,parentTrans,k)
			end
		end
		if trans then
			mjCard = CMJCard.New()
			mjCard:Init(v,k)
			--初始化牌的显示及位置
			LogicUtil.SetOwnOutMJCardShow(seatPos,luaWindowRoot,trans,sort,mjCard,k,show_back)

			cardTranItem = {}
			cardTranItem.trans = trans
			cardTranItem.cardInfo = mjCard
			cardTranItem.isAnGang = show_back
			table.insert(self.own_outCardTransMap[seatPos][sort].cards,cardTranItem)

			if tanpai_trans then
				LogicUtil.SetOwnOutMJCardShow(seatPos,luaWindowRoot,tanpai_trans,sort,mjCard,k,false)
				luaWindowRoot:ChangeDepth(tanpai_trans, 10)
				tanpai_trans.localScale = UnityEngine.Vector3.zero

				cardTranItem = {}
				cardTranItem.trans = tanpai_trans
				cardTranItem.cardInfo = mjCard
				cardTranItem.isTanPaiAnGang = true
				table.insert(self.own_outCardTransMap[seatPos][sort].cards,cardTranItem)
			end
		end
	end


end

-- 抢杠
function UIMJOutCardShowCtrl:MJ_PlayQiangGangShow(eventId, seatPos, cardId)
	if nil == seatPos or nil == cardId then
		DwDebug.LogError("play qiang gang show seatPos is nil or cardId is nil")
		return
	end

	local transMap = self.own_outCardTransMap[seatPos]
	if nil == transMap or 0 == #transMap then
		DwDebug.LogError("play qiang gang show transMap is nil" .. seatPos)
		return
	end

	local cards = transMap[#transMap].cards
	local cardTranItem = cards[4]
	if nil == cardTranItem then
		DwDebug.LogError("play qiang gang show transMap cardTranItem is nil")
		return
	end

	if nil ~= cardTranItem.cardInfo and cardId == cardTranItem.cardInfo.ID then
		self.luaWindowRoot:SetActive(cardTranItem.trans, false)
		table.remove(cards, 4) --移除杠牌
	else
		DwDebug.LogError("play qiang gang show no found cardId:" .. cardId)
	end
end

--通知清理牌桌出的牌，用于清桌或者断网重连刷新
function UIMJOutCardShowCtrl:MJ_ClearAllOutCards(eventId,p1,p2)
	self:ClearAllCards()
end

--背景切换
function UIMJOutCardShowCtrl:ChangePlayCanvas(eventId,p1,p2)
	local titleIndex = p2
	for seatPos,v in pairs(self.own_outCardTransMap) do
		for sort,cardTranItems in pairs(v) do
			local _cardTranItems = cardTranItems.cards
			for k,item in pairs(_cardTranItems) do
				if item.isAnGang then
					LogicUtil.ChangeSingleMJPaiGangBg(self.luaWindowRoot, titleIndex, item.trans)
				else
					LogicUtil.ChangeSingleMJPaiBg(self.luaWindowRoot, titleIndex, seatPos, item.trans, 3)
				end
			end

			-- LogicUtil.ChangeMJPaiBgs(self.luaWindowRoot,titleIndex,seatPos,_cardTranItems,3)
		end
	end
	for seatPos,v in pairs(self.outCardTransMap) do
		LogicUtil.ChangeMJPaiBgs(self.luaWindowRoot,titleIndex,seatPos,v,3)
	end
end

function UIMJOutCardShowCtrl:CreateMJCardItem(seatPos,parentTrans,index,isBg)
	local resName = "mjcard_out_show_item"
	if isBg then
		resName = "mjcard_bg_out_show_item"
	end
	local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, resName, RessStorgeType.RST_Never, false)
	if not resObj then
		return
	end
	local trans = resObj.transform
	trans.parent = parentTrans
	self.luaWindowRoot:SetActive(trans,true)
	trans.name = index.."_cardItem"
	trans.localScale = UnityEngine.Vector3.New(1,1,1)
	if isBg then
		LogicUtil.ChangeSingleMJPaiGangBg(self.luaWindowRoot,LogicUtil.GetMJPaiType(), trans)
	else
		LogicUtil.ChangeSingleMJPaiBg(self.luaWindowRoot,LogicUtil.GetMJPaiType(), seatPos, trans,3)
	end

	return trans
end

function UIMJOutCardShowCtrl:GetOutCardParentTrans(seatPos)
	local parentTrans
	if seatPos == SeatPosEnum.South then
		parentTrans = self.southRootTrans
	elseif seatPos == SeatPosEnum.East then
		parentTrans = self.eastRootTrans
	elseif seatPos == SeatPosEnum.North then
		parentTrans = self.northRootTrans
	else
		parentTrans = self.westRootTrans
	end
	return parentTrans
end

function UIMJOutCardShowCtrl:GetOwnOutCardParentTrans(seatPos)
	local parentTrans
	if seatPos == SeatPosEnum.South then
		parentTrans = self.own_southRootTrans
	elseif seatPos == SeatPosEnum.East then
		parentTrans = self.own_eastRootTrans
	elseif seatPos == SeatPosEnum.North then
		parentTrans = self.own_northRootTrans
	else
		parentTrans = self.own_westRootTrans
	end
	return parentTrans
end


--打出去的一张牌的正常显示
function UIMJOutCardShowCtrl:PlayNormalOutMJCardShow(seatPos,mjCard)
	if self.outCardTransMap[seatPos] == nil then
		self.outCardTransMap[seatPos] = {}
	end
	local curLen = #self.outCardTransMap[seatPos]
	local curIndex = curLen + 1
	local curFloor = 1

	local parentTrans = self:GetOutCardParentTrans(seatPos)

	local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "mjcard_out_show_item", RessStorgeType.RST_Never, false)
	if not resObj then
		return
	end

	local luaWindowRoot = self.luaWindowRoot
	local trans = resObj.transform
	trans.parent = parentTrans
	luaWindowRoot:SetActive(trans,true)
	trans.name = curIndex.."_cardItem"
	trans.localScale = UnityEngine.Vector3.New(1,1,1)
	local posResult = LogicUtil.GetMJOutCardShowPos(seatPos,#self.outCardTransMap[seatPos])
	--初始化麻将显示及其位置
	LogicUtil.SetNoramlOutMJCardShow(seatPos,luaWindowRoot,trans,mjCard,posResult)
	-- 设置麻将背景
	LogicUtil.ChangeSingleMJPaiBg(luaWindowRoot, LogicUtil.GetMJPaiType(), seatPos, trans, 3)

	local cardTranItem = {}
	cardTranItem.trans = trans
	cardTranItem.cardInfo = mjCard
	table.insert(self.outCardTransMap[seatPos],cardTranItem)

	-- 选中牌映射表
	self:checkSelData(mjCard.ID)
	local t = self.MJSelCardHighlightTransMap[mjCard.ID]
	t = t or {}
	t[#t+1] = luaWindowRoot:GetTrans(trans,"ico_mask")

	return trans
end

function UIMJOutCardShowCtrl:checkSelData( card_id )
	self.MJSelCardHighlightTransMap = self.MJSelCardHighlightTransMap or {}
	self.MJSelCardHighlightTransMap[card_id] = self.MJSelCardHighlightTransMap[card_id] or {}
end

function UIMJOutCardShowCtrl:MJSelCardHighlight(event_id,p1)

	local luaWindowRoot = self.luaWindowRoot
	local last_card_id = self.last_card_id
	local card_id = p1

	local function setVisible( card_id,isVisible )
		self:checkSelData(card_id)
		local t = self.MJSelCardHighlightTransMap[card_id]
		for i=1,#t do
			luaWindowRoot:SetActive(t[i],isVisible)
		end
	end

	if last_card_id then
		setVisible(last_card_id,false)
	end

	if card_id then
		setVisible(card_id,true)
		self.last_card_id = card_id
	end

end

-- 碰杠牌逻辑 消掉一个选中牌的引用
function UIMJOutCardShowCtrl:PopSelectedHighlight( card_id )
	local t = self.MJSelCardHighlightTransMap[card_id]
	t[#t] = nil
end

function UIMJOutCardShowCtrl:ClearCards(seatPos)
	local cardTranItems = self.outCardTransMap[seatPos]
	if cardTranItems then
		for k,v in pairs(cardTranItems) do
			self.luaWindowRoot:SetActive(v.trans,false)
		end
	end
	self.outCardTransMap[seatPos] = {}

	local ownOutCardItems = self.own_outCardTransMap[seatPos]
	if ownOutCardItems then
		for k,v in pairs(ownOutCardItems) do
			for k1,v1 in pairs(v.cards) do
				self.luaWindowRoot:SetActive(v1.trans,false)
			end
			self.luaWindowRoot:SetActive(v.pointer,false)
		end
	end
	self.own_outCardTransMap[seatPos] = {}

	self.MJSelCardHighlightTransMap = nil
end


function UIMJOutCardShowCtrl:ClearAllCards()
	self:ClearCards(SeatPosEnum.South)
	self:ClearCards(SeatPosEnum.North)
	self:ClearCards(SeatPosEnum.East)
	self:ClearCards(SeatPosEnum.West)

	self.outCardTransMap = {}
	self.own_outCardTransMap = {}
	self.MJSelCardHighlightTransMap = nil
end

-- 销毁
function UIMJOutCardShowCtrl:Destroy()
--	print("UIMJOutCardShowCtrl:Destroy")
	self:UnRegisterEvent()
	self:ClearAllCards()
	self.MJSelCardHighlightTransMap = nil
end
