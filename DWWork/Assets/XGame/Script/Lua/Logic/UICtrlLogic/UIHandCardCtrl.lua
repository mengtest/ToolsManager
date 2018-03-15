--------------------------------------------------------------------------------
-- 	 File      : UIHandCardCtrl.lua
--   author    : guoliang
--   function   : UI手牌显示控制
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "CommonProduct.CommonBase.UICtrl.UIBaseHandCardCtrl"

UIHandCardCtrl = class("UIHandCardCtrl", UIBaseHandCardCtrl)

function UIHandCardCtrl:Init(rootTrans,luaWindowRoot,handCards)
	self:BaseInit(rootTrans, luaWindowRoot, handCards)
	
	self:InitValues()
	self:RegisterEvent()
end

function UIHandCardCtrl:RegisterEvent()
	self:BaseRegisterEvent()    -- 一定要写
	LuaEvent.AddHandle(EEventType.SelfFindFriend,self.SelfFindFriend,self)
end

function UIHandCardCtrl:UnRegisterEvent()
	self:BaseUnRegisterEvent()    -- 一定要写
	LuaEvent.RemoveHandle(EEventType.SelfFindFriend,self.SelfFindFriend,self)
end

function UIHandCardCtrl:InitValues()
	self:BaseInitValues()
end

function UIHandCardCtrl:SelfFindFriend(eventId,p1,p2)
	local isOn = p1
	if isOn then
		self:RefreshHandCardShowStatus(1)
	else
		self:RefreshHandCardShowStatus(0)
	end
end

--切换到下一个状态
function UIHandCardCtrl:ChangeCardSort()
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	local curSortType = cardPlayLogic:GetSortType()
	curSortType = curSortType + 1
	if curSortType > 3 then
		curSortType = 1
	end

	self:ChangeCardSortType(curSortType)
end

--检查牌组是否有变化
local function CheckCardHasChange(cards1,cards2)
	if #cards1 == #cards2 then
		for i=1,#cards1 do
			if cards1[i].ID ~= cards2[i].ID then
				return true
			end
		end
	else
		return true
	end
	return false
end

--设置牌的排序
function UIHandCardCtrl:ChangeCardSortType(sType)
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
    local curSortType = cardPlayLogic:GetSortType()

	if sType == nil or type(sType) ~= "number" then
		sType = curSortType
	end

	--理牌会重新排序 显示不同 但是同类型 这里屏蔽掉
	--if sType == curSortType then return end

	cardPlayLogic:SetSortType(sType)
	--排完序后 和之前 一样 类型又不是1 则重新排序
	local nextCards = self:GetSortCard(self.handCards, sType)
	if not CheckCardHasChange(nextCards,self.handCards) and sType ~= 1 then
		self:ChangeCardSort()
		return
	end
	self.handCards = nextCards
	self:RefreshAllCardShow()
end

--抬起510K的牌
function UIHandCardCtrl:Up510KCard()
	-- 0 是负510K 1是正的
	if self.Up510KType == nil or self.Up510KType == 1 then
		self.Up510KType = 0
	else
		self.Up510KType = 1
	end

	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	local result = cardPlayLogic:GetHandCardStruct(self.handCards)

	local needUpCards = {}

	needUpCards = self:GetSmall510KCard(result.ftkCards)

	if self.Up510KType == 0 and needUpCards then
		self:TipCardsUp(needUpCards)
	elseif result.ftkBigCount > 0 then
		needUpCards = result.ftkBigCards[1]
		self:TipCardsUp(needUpCards)
	elseif needUpCards then
		self:TipCardsUp(needUpCards)
	else
		WindowUtil.LuaShowTips("您没有510K牌型！")
	end
end

--从510K牌组里面找到副510K
function UIHandCardCtrl:GetSmall510KCard(ftkCards)
	for k,cards in pairs(ftkCards) do
		if cards[1] ~= nil and cards[2] ~= nil and cards[3] ~= nil then
			if cards[1].color == cards[2].color and cards[1].color == cards[3].color then
			else
				return cards
			end
		end
	end
	return nil
end

--给牌打上标记 重新排序
function UIHandCardCtrl:CardTag()
	if self.nowTagIndex == nil then
		self.nowTagIndex = 0
	end

	--有抬起的牌 打上标记
	if self.upCardIndexs and #self.upCardIndexs > 0 then
		self.nowTagIndex = self.nowTagIndex + 1
		for k,v in pairs(self.upCardIndexs) do
			self.handCards[v].typeTag = self.nowTagIndex
		end
	else --没有选中抬起 则是取消标记
		self.nowTagIndex = 0
		local hasTagCard = false
		for k,v in pairs(self.handCards) do
			if v.typeTag  ~= 0 then
				hasTagCard = true
				v.typeTag = self.nowTagIndex
			end
		end
		--没有标记牌 点击无效化
		if not hasTagCard then
			return
		end
	end
	self:ChangeCardSortType()
end

--检查是否是理牌状态
function UIHandCardCtrl:IsInLiPaiState()
	local showLiPai = #self.upCardIndexs > 0

	local hasTagCard = false
	--没有抬起牌 又没有理过牌 还是显示理牌按钮
	if not showLiPai and self.handCards and #self.handCards > 0 then
		for k,v in pairs(self.handCards) do
			if v.typeTag > 0 then
				hasTagCard = true
				break
			end
		end
	end

	if not hasTagCard then
		showLiPai = true
	end

	return showLiPai
end

function UIHandCardCtrl:OutCardUpdateShow()
	if PlayGameSys.GetPlayLogicType() == PlayLogicTypeEnum.WSK_Normal or
	   PlayGameSys.GetPlayLogicType() == PlayLogicTypeEnum.DDZ_Normal then
		local cardPlayLogic = PlayGameSys.GetPlayLogic()
		local sortType = cardPlayLogic:GetSortType()
		if sortType == 1 then
			LogicUtil.AdjustCards(#self.handCards,self.totalLen,self.cardTranItems)
		elseif sortType == 2 or sortType == 3 then
			-- 针对炸弹拆牌时，要从新排序
			self.handCards = self:GetSortCard(self.handCards, sortType)
			self:RefreshAllCardShow()
		else
			LogicUtil.AdjustCards(#self.handCards,self.totalLen,self.cardTranItems)
		end
	else
		LogicUtil.AdjustCards(#self.handCards,self.totalLen,self.cardTranItems)
	end
end

-- 自己设置手牌状态  status :0 正常  1 找朋友时
function UIHandCardCtrl:RefreshHandCardShowStatus(status)
--	print("RefreshHandCardShowStatus "..status)
	local isFindFriend = (status == 1)
	local sameCards = LogicUtil.FindSameColorCards(self.handCards)
	for k,v in pairs(self.handCards) do
		if sameCards[v.seq] or v:IsKing() then
			self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(self.cardTranItems[k].trans,"ico_mask"), isFindFriend)
		end
	end
	self:DownUpCards()
end

--获取牌的排序
function UIHandCardCtrl:GetSortCard(cards, sType)
	local tagCards,normalCards = self:HandleTagCards(cards)

	--只对未标签的部分排序
	cards = normalCards

	if PlayGameSys.GetPlayLogicType() == PlayLogicTypeEnum.WSK_Normal then
		if sType == 1 then
			cards = LogicUtil.SortCards(cards, false)
		elseif sType == 2 then
			if PlayGameSys.GetNowPlayId() == Common_PlayID.yiHuang_510K then
				cards = LogicUtil.SortCardsByBombYH(cards)
			else
				cards = LogicUtil.SortCardsByBomb(cards)
			end
		elseif sType == 3 then
			if PlayGameSys.GetNowPlayId() == Common_PlayID.leAn_510K or
				PlayGameSys.GetNowPlayId() == Common_PlayID.chongRen_510K then
				cards = LogicUtil.SortCardsByWSK(cards)
			end
		else
			cards = LogicUtil.SortCards(cards, false)
		end
	elseif PlayGameSys.GetPlayLogicType == PlayLogicTypeEnum.DDZ_Normal then
		if sType == 1 then
			cards = LogicUtil.SortCards(cards, false)
		elseif sType == 2 then
			cards = LogicUtil.SortCardsByBombDDZ(cards)
		else
			cards = LogicUtil.SortCards(cards, false)
		end
	else
		cards = LogicUtil.SortCards(cards, false)
	end

	--合并最终结果
	for k,v in ipairs(cards) do
		table.insert(tagCards,v)
	end

	return tagCards
end

--处理打标签的牌 为了不影响之前的 单独分出来处理
function UIHandCardCtrl:HandleTagCards(cards)
	local tagCards = {}
	local normalCards = {}
	for k,v in ipairs(cards) do
		if v.typeTag ~= nil and v.typeTag ~= 0 then
			table.insert(tagCards, v)
		else
			table.insert(normalCards, v)
		end
	end

	--按照typeTag排序
	table.sort(tagCards, function(cards1, cards2)
		if cards1.typeTag > cards2.typeTag then
			return true
		elseif cards1.typeTag == cards2.typeTag then
			if cards1.logicValue > cards2.logicValue then
				return true
			elseif cards1.logicValue == cards2.logicValue then
				return cards1.ID > cards2.ID
			end
		end
		return false
	end
	)

	return tagCards,normalCards
end

--点击卡牌
function UIHandCardCtrl:ClickCard(eventid, cardTrans,index)
	LuaEvent.AddEventNow(EEventType.RefreshRoomSetRoot, false)
	local isUp = false
	--要改要改 写的太挫了
	for k,v in pairs(self.upCardIndexs) do
		if index == v then
			isUp = true
			break
		end
	end

	local tempCard = self.handCards[index]
	--有标记的牌 要一起弹起 放下
	if tempCard and tempCard.typeTag ~= 0 then
		local sameTagCards = self:GetMyTagCard(tempCard.typeTag)
		if isUp then
			self:DownUpCards(sameTagCards)
		else
			self:TipCardsUp(sameTagCards)
		end
	else
		--删除就要遍历 写的好丑陋啊
		for k,v in pairs(self.upCardIndexs) do
			if index == v then
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
	end

	if not isUp then
		self:PlayCardSelectAudio()
	end

	LuaEvent.AddEventNow(EEventType.CardDownUpChange)
end

--获取手牌中 同typeTag的牌
function UIHandCardCtrl:GetMyTagCard(typeTag)
	local tempCards = {}
	for k,card in pairs(self.handCards) do
		if card.typeTag == typeTag then
			table.insert(tempCards,card)
		end
	end
	return tempCards
end

-- 销毁
function UIHandCardCtrl:Destroy()
	self:UnRegisterEvent()
	self:BaseDestroy()
end
