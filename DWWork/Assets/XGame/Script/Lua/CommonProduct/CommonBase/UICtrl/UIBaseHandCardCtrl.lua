--------------------------------------------------------------------------------
-- 	 File       : UIBaseHandCardCtrl.lua
--   author     : zhanghaochun
--   function   : UI手牌显示控制
--   date       : 2018-03-05
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "LuaWindow.LuaUIElement.FrameSelect"

UIBaseHandCardCtrl = class("UIBaseHandCardCtrl", nil)

function UIBaseHandCardCtrl:ctor()

end

function UIBaseHandCardCtrl:BaseInit(rootTrans,luaWindowRoot,handCards)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	self.isPlayDealCardAnim = false
	self.cardTranItems = {}
	self.upCardIndexs = {}
	self.handCards = {}
	self.totalLen = 1140
	--self:RegisterEvent()
	--self:InitValues()
	self.m_eventTimeValue = WrapSys.GetTimeValue()
end

function UIBaseHandCardCtrl:BaseInitValues()
	self.CardResourceName = "card_item"    -- 使用牌的模型
	self.CardUpOffset = 35                 -- 选择的牌抬起的高度
end

function UIBaseHandCardCtrl:BaseRegisterEvent()
	LuaEvent.AddHandle(EEventType.SelfHandCardInit,self.InitSelfHandCards,self)
	LuaEvent.AddHandle(EEventType.SelfHandCardClear,self.SelfHandCardClear,self)
	LuaEvent.AddHandle(EEventType.ShadowCards,self.OnShadowCards,self)
	LuaEvent.AddHandle(EEventType.SelectCards,self.OnSelectCards,self)
	LuaEvent.AddHandle(EEventType.PKClickCard,self.ClickCard,self)
end

function UIBaseHandCardCtrl:BaseUnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.SelfHandCardInit,self.InitSelfHandCards,self)
	LuaEvent.RemoveHandle(EEventType.SelfHandCardClear,self.SelfHandCardClear,self)
	LuaEvent.RemoveHandle(EEventType.ShadowCards,self.OnShadowCards,self)
	LuaEvent.RemoveHandle(EEventType.SelectCards,self.OnSelectCards,self)
	LuaEvent.RemoveHandle(EEventType.PKClickCard,self.ClickCard,self)
end
------------------------------------事件-----------------------------------------
function UIBaseHandCardCtrl:InitSelfHandCards(eventId,p1,p2)
	local isNeedPlayAnim = p1
	self:ClearCards()
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	local selfPlayer = cardPlayLogic.roomObj.playerMgr:GetPlayerByPlayerID(cardPlayLogic.roomObj:GetSouthUID())
	if selfPlayer then
		self.cardTranItems = {}
		self.upCardIndexs = {}

		self.handCards = ShallowCopy(selfPlayer.cardMgr:GetHandCards())
		self.handCards = self:GetSortCard(self.handCards, cardPlayLogic:GetSortType())

		if isNeedPlayAnim then
			self:StartDealCards()
		else
			self:EndDealCards()
			self:InitAllCardsShow()
			LuaEvent.AddEvent(EEventType.EndDistributCard) -- 发牌完毕
		end
	end
end

function UIBaseHandCardCtrl:SelfHandCardClear(eventId,p1,p2)
	self:ClearCards()
end

function UIBaseHandCardCtrl:OnShadowCards(eventId, start_index, end_index)
	local shadow_flag = false
	for i,v in ipairs(self.cardTranItems) do
		if start_index > end_index then
			shadow_flag = i >= end_index and i <= start_index
		elseif start_index == end_index then
			shadow_flag = i == start_index
		else
			shadow_flag = i >= start_index and i <= end_index
		end
		self.luaWindowRoot:SetVisible(self.luaWindowRoot:GetTrans(v.trans,"shadow"), shadow_flag)
	end
end

function UIBaseHandCardCtrl:OnSelectCards(eventId, start_index, end_index)
	LuaEvent.AddEventNow(EEventType.RefreshRoomSetRoot, false)
	local aboutTo_upCard, downcards_indexes = self:CheckHasDownCardInRange(start_index, end_index)

	if aboutTo_upCard then

		for k,v in ipairs(downcards_indexes) do
			if self.cardTranItems[v] then
				self:UpDownCard(self.cardTranItems[v].trans, aboutTo_upCard)
				table.insert(self.upCardIndexs, v)
			end
		end
	else

		for i = start_index,end_index,start_index >= end_index and -1 or 1 do
			if self.cardTranItems[i] then
				self:UpDownCard(self.cardTranItems[i].trans, false)

				for k,v in pairs(self.upCardIndexs) do
					if v == i then
						table.remove(self.upCardIndexs,k)
						break
					end
				end
			end
		end
	end

	for i=start_index,end_index,start_index>end_index and -1 or 1 do
		if self.cardTranItems[i] then
			self.luaWindowRoot:SetVisible(self.luaWindowRoot:GetTrans(self.cardTranItems[i].trans,"shadow"), false)
		end
	end

	if aboutTo_upCard then
		self:PlayCardSelectAudio()
	end

	-- 如果是在一张牌内滑动，可能会触发onclick，没错，只是可能触发，也可能不触发（瞬间的小划一下）
	-- if start_index ~= end_index then
	-- end

	LuaEvent.AddEventNow(EEventType.CardDownUpChange)
end

--点击卡牌
function UIBaseHandCardCtrl:ClickCard(eventid, cardTrans,index)
	LuaEvent.AddEventNow(EEventType.RefreshRoomSetRoot, false)
	local isUp = false

	for k,v in pairs(self.upCardIndexs) do
		if index == v then
			isUp = true
			table.remove(self.upCardIndexs,k)
			break
		end
	end
	if isUp then
		self:UpDownCard(cardTrans,false)
	else
		table.insert(self.upCardIndexs,index)
		self:UpDownCard(cardTrans,true)
	end

	if not isUp then
		self:PlayCardSelectAudio()
	end

	LuaEvent.AddEventNow(EEventType.CardDownUpChange)
end
--------------------------------------------------------------------------------

function UIBaseHandCardCtrl:StartDealCards()
	if self.isPlayDealCardAnim then
		UpdateBeat:Remove(self.UpdateBeat,self)
	end
	self.isPlayDealCardAnim = true
	self.timeDelta = 0
	self.playInterval = 0.1
	self.dealCardStartIndex = 1
	self.dealCardCount = #self.handCards
	UpdateBeat:Add(self.UpdateBeat,self)
end

function UIBaseHandCardCtrl:EndDealCards()
	if self.isPlayDealCardAnim then
		LuaEvent.AddEventNow(EEventType.ShowCardDealCardAction,false)
		UpdateBeat:Remove(self.UpdateBeat,self)
		self.isPlayDealCardAnim = false
		self.timeDelta = 0
		self.playInterval = 0.1
		self.dealCardStartIndex = 1
		self.dealCardCount = 0
	end
end

function UIBaseHandCardCtrl:UpdateBeat()
	self.timeDelta = self.timeDelta + UnityEngine.Time.deltaTime
	if self.timeDelta >= self.playInterval then
		self.timeDelta = 0
		if self.dealCardStartIndex <= self.dealCardCount then
			local pos = LogicUtil.GetHandCardPosWhenDealCard(0,self.dealCardCount,self.totalLen,self.dealCardStartIndex)
			self:InitSingleCard(self.handCards[self.dealCardStartIndex],self.dealCardStartIndex,pos)
			self.dealCardStartIndex = self.dealCardStartIndex + 1
			AudioManager.PlayCommonSound(UIAudioEnum.faPai)
		else
			self:EndDealCards()
			--LogicUtil.AdjustCards(#self.handCards,self.totalLen,self.cardTranItems)
			LuaEvent.AddEventNow(EEventType.ExcecuteDelayDealCardAction)
			LuaEvent.AddEvent(EEventType.EndDistributCard) -- 发牌完毕
		end
	end
end

function UIBaseHandCardCtrl:InitSingleCard(card,sort,pos)
	if card then
		local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, self.CardResourceName, RessStorgeType.RST_Never, false)
		if not resObj then
			return
		end

		-- 添加框选功能
		GetOrAddLuaComponent(resObj, "LuaWindow.LuaUIElement.FrameSelect", true)
		LogicUtil.EnableTouch(resObj.transform, true)

		local trans = resObj.transform
		trans.parent = self.rootTrans
		trans.name = sort.."_cardItem"
		trans.localScale = UnityEngine.Vector3.New(1,1,1)
		if pos then
			trans.localPosition = pos
		else
			trans.localPosition = UnityEngine.Vector3.New(0,0,0)
		end
		self.luaWindowRoot:SetActive(trans,true)
		self:InitCardShow(self.luaWindowRoot, trans, card, sort)
		local cardTranItem = {}
		cardTranItem.trans = trans
		cardTranItem.cardInfo = card
		table.insert(self.cardTranItems,cardTranItem)
	end
end

function UIBaseHandCardCtrl:InitAllCardsShow()
	local cardCount = #self.handCards
	for k,v in pairs(self.handCards) do
		self:InitSingleCard(v,k)
	end
	LogicUtil.AdjustCards(cardCount,self.totalLen,self.cardTranItems)
	self:AfterInitCards()
end

function UIBaseHandCardCtrl:RefreshAllCardShow()
	self:DownUpCards()
	for k, v in ipairs(self.cardTranItems) do
		self:InitCardShow(self.luaWindowRoot, v.trans, self.handCards[k], k)
		v.cardInfo = self.handCards[k]
	end
	self:AfterRefreshCards()
end

--抬起牌
function UIBaseHandCardCtrl:TipCardsUp(tipCards)
	if tipCards ~= nil then
		-- 有抬起的牌先还原
		self:DownUpCards()
		for k,v in pairs(self.handCards) do
			for k1,v1 in pairs(tipCards) do
				if v.seq == v1.seq then
					table.insert(self.upCardIndexs,k)
					self:UpDownCard(self.cardTranItems[k].trans,true)
				end
			end
		end
	end

	LuaEvent.AddEventNow(EEventType.CardDownUpChange)
end

function UIBaseHandCardCtrl:DownUpCards(tipCards)
	if tipCards then
		for k,v in pairs(self.handCards) do
			for k1,v1 in pairs(tipCards) do
				if v.seq == v1.seq then
					self:UpDownCard(self.cardTranItems[k].trans,false)
					
					----删除就要遍历 写的好丑陋啊
					--从手牌表删除
					for k2,v2 in pairs(self.upCardIndexs) do
						if k == v2 then
							table.remove(self.upCardIndexs,k2)
							break
						end
					end
					break
				end
			end
		end
	else
		for k,v in pairs(self.upCardIndexs) do
			self:UpDownCard(self.cardTranItems[v].trans,false)
		end
		self.upCardIndexs = {}
	end
	LuaEvent.AddEventNow(EEventType.CardDownUpChange)
end


function UIBaseHandCardCtrl:UpDownCard(cardTrans,isUp)
	if cardTrans then
		if isUp then
			cardTrans.localPosition = UnityEngine.Vector3.New(cardTrans.localPosition.x, self.CardUpOffset,0)
		else
			cardTrans.localPosition = UnityEngine.Vector3.New(cardTrans.localPosition.x, 0,0)
		end
	end
end

function UIBaseHandCardCtrl:CheckHasDownCardInRange(start_index, end_index)
	local found = false
	local down_cards = {}
	for i=start_index,end_index, start_index > end_index and -1 or 1 do
		-- 必须是手上有的卡牌
		if self.cardTranItems[i] then
			found = false
			for k,v in pairs(self.upCardIndexs) do
				if v == i then
					found = true
					break
				end
			end

			if not found then
				table.insert(down_cards, i)
			end
		end
	end

	return #down_cards > 0, down_cards
end

--出牌
function UIBaseHandCardCtrl:OutCards()
	--测试代码 ，后面要走cardmgrmodule
	local newHandCards = {}
	local newCardTrans = {}
	local outCards = {}
	local isContain = false
	for k1,v1 in pairs(self.cardTranItems) do
		isContain = false
		for k,v in pairs(self.upCardIndexs) do
			if k1 == v then
				self.luaWindowRoot:SetActive(v1.trans,false)
				isContain = true
				break
			end
		end
		if not isContain then
			table.insert(newCardTrans,v1)
			table.insert(newHandCards,self.handCards[k1])
		else
			table.insert(outCards,self.handCards[k1])
		end
	end
	self.upCardIndexs = {}
	self.cardTranItems = newCardTrans
	self.handCards = newHandCards

	LogicUtil.AdjustCards(#self.handCards,self.totalLen,self.cardTranItems)
	self:OutCardUpdateShow()
	self:AfterOutCards()

	-- 更新手牌
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	local selfPlayer = cardPlayLogic.roomObj.selfPlayer
	if selfPlayer then
		selfPlayer.cardMgr:DeleteCards(outCards)
	end
end

function UIBaseHandCardCtrl:GetUpCards()
	local upcards = {}
	for k,v in pairs(self.upCardIndexs) do
		for k1,v1 in pairs(self.handCards) do
			if v == k1 then
--				print("ID".. v1.ID .. "logicValue " ..v1.logicValue)
				table.insert(upcards,v1)
			end
		end
	end
	return upcards
end

function UIBaseHandCardCtrl:ClearCards()
	for k,v in pairs(self.cardTranItems) do
		self.luaWindowRoot:SetActive(v.trans,false)
	end
	self.upCardIndexs = {}
	self.cardTranItems = {}
	self.handCards = {}
	
	self:AfterClearCards()
end

-- 销毁
function UIBaseHandCardCtrl:BaseDestroy()
	--self:UnRegisterEvent()

	if self.isPlayDealCardAnim then
		UpdateBeat:Remove(self.UpdateBeat,self)
	end

	for k,v in pairs(self.cardTranItems) do
		self.luaWindowRoot:SetActive(v.trans,false)
	end
	self.isPlayDealCardAnim = false
	self.upCardIndexs = nil
	self.cardTranItems = nil
	self.handCards = nil
end
------------------------------可以重写的函数---------------------------------------
function UIBaseHandCardCtrl:GetSortCard(cards, sType)
	return LogicUtil.SortCards(cards, false)
end

function UIBaseHandCardCtrl:InitCardShow(luaWindowRoot, trans, card, index)
	LogicUtil.InitCardItem(luaWindowRoot, trans, card, index,true)
end

function UIBaseHandCardCtrl:OutCardUpdateShow()
	LogicUtil.AdjustCards(#self.handCards,self.totalLen,self.cardTranItems)
end

function UIBaseHandCardCtrl:PlayCardSelectAudio()
	
end

-- 在初始化手牌后运行
function UIBaseHandCardCtrl:AfterInitCards()
end

-- 在刷新手牌后运行
function UIBaseHandCardCtrl:AfterRefreshCards()
end

-- 在运行手牌后运行
function UIBaseHandCardCtrl:AfterOutCards()
end

-- 在清理手牌后运行
function UIBaseHandCardCtrl:AfterClearCards()
end
--------------------------------------------------------------------------------