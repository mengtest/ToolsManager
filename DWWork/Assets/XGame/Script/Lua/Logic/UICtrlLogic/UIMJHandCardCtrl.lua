--------------------------------------------------------------------------------
-- 	 File      : UIMJHandCardCtrl.lua
--   author    :
--   function   : UI 麻将手牌显示控制
--   date      : 2017-11-8
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UIMJHandCardCtrl = class("UIMJHandCardCtrl",nil)

function UIMJHandCardCtrl:Init(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans

	--打出的牌的父节点
	self.parentNodeTrans = {}
	self.parentNodeTrans[SeatPosEnum.South] = luaWindowRoot:GetTrans(rootTrans,"south_node")
	self.parentNodeTrans[SeatPosEnum.East] = luaWindowRoot:GetTrans(rootTrans,"east_node")
	self.parentNodeTrans[SeatPosEnum.North] = luaWindowRoot:GetTrans(rootTrans,"north_node")
	self.parentNodeTrans[SeatPosEnum.West] = luaWindowRoot:GetTrans(rootTrans,"west_node")

	-- 资源名
	self.MJCardItemName =
	{
		[SeatPosEnum.South] = "mjcard_south_item",
		[SeatPosEnum.East] = "mjcard_east_item",
		[SeatPosEnum.North] = "mjcard_north_item",
		[SeatPosEnum.West] = "mjcard_west_item",
	}

	-- 资源名
	self.MJTanPaiCardItemName =
	{
		[SeatPosEnum.South] = "mjcard_south_record_item",
		[SeatPosEnum.East] = "mjcard_east_record_item",
		[SeatPosEnum.North] = "mjcard_north_record_item",
		[SeatPosEnum.West] = "mjcard_west_record_item",
	}

	-- 节点偏移
	self.MJSeatNodePos =
	{
		[SeatPosEnum.South] = UnityEngine.Vector3.New(580, -288, 0),
		[SeatPosEnum.East] = UnityEngine.Vector3.New(480, 262, 0),
		[SeatPosEnum.North] = UnityEngine.Vector3.New(-342, 312, 0),
		[SeatPosEnum.West] = UnityEngine.Vector3.New(-480, -150, 0),
	}

	-- 节点偏移
	self.MJTanPaiSeatNodePos =
	{
		[SeatPosEnum.South] = UnityEngine.Vector3.New(470, -300, 0),
		[SeatPosEnum.East] = UnityEngine.Vector3.New(470, 240, 0),
		[SeatPosEnum.North] = UnityEngine.Vector3.New(-340, 312, 0),
		[SeatPosEnum.West] = UnityEngine.Vector3.New(-468, -180, 0),
	}

	self.luaWindowRoot = luaWindowRoot
	self.isTanPai = nil
	self.upCardTrans = nil
	self.outCardTrans = nil
	self.cardItems = {}

	self:RegisterEvent()
end

---------------------------event zone ------------------------------------
function UIMJHandCardCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.MJPlayFaPaiAction, self.PlayFaPaiAction, self)
	LuaEvent.AddHandle(EEventType.MJInitHandCards, self.InitHandCards, self)
	LuaEvent.AddHandle(EEventType.MJInitTanPaiHandCards, self.InitTanPaiHandCards, self)
	LuaEvent.AddHandle(EEventType.MJInCard, self.InCard, self)
	LuaEvent.AddHandle(EEventType.MJRemoveOtherCard, self.RemoveOtherCard, self)
	LuaEvent.AddHandle(EEventType.MJRemoveSelfCard, self.RemoveSelfCard, self)
	LuaEvent.AddHandle(EEventType.MJOutSelfCard, self.OutSelfCard, self)
	LuaEvent.AddHandle(EEventType.MJClearHandCards, self.ClearHandCards, self)
	LuaEvent.AddHandle(EEventType.ChangePlayCanvas, self.ChangePlayCanvas, self)
	LuaEvent.AddHandle(EEventType.MJPlayCard, self.PlayCard, self)
	LuaEvent.AddHandle(EEventType.MJClickCard, self.ClickCard, self)
	LuaEvent.AddHandle(EEventType.MJRefreshCard, self.MJRefreshCard, self)
end

function UIMJHandCardCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.MJPlayFaPaiAction, self.PlayFaPaiAction, self)
	LuaEvent.RemoveHandle(EEventType.MJInitHandCards, self.InitHandCards, self)
	LuaEvent.RemoveHandle(EEventType.MJInitTanPaiHandCards, self.InitTanPaiHandCards, self)
	LuaEvent.RemoveHandle(EEventType.MJInCard, self.InCard, self)
	LuaEvent.RemoveHandle(EEventType.MJRemoveOtherCard, self.RemoveOtherCard, self)
	LuaEvent.RemoveHandle(EEventType.MJRemoveSelfCard, self.RemoveSelfCard, self)
	LuaEvent.RemoveHandle(EEventType.MJOutSelfCard, self.OutSelfCard, self)
	LuaEvent.RemoveHandle(EEventType.MJClearHandCards, self.ClearHandCards, self)
	LuaEvent.RemoveHandle(EEventType.ChangePlayCanvas, self.ChangePlayCanvas, self)
	LuaEvent.RemoveHandle(EEventType.MJPlayCard, self.PlayCard, self)
	LuaEvent.RemoveHandle(EEventType.MJClickCard, self.ClickCard, self)
	LuaEvent.RemoveHandle(EEventType.MJRefreshCard, self.MJRefreshCard, self)
end

-- 初始化手牌
function UIMJHandCardCtrl:InitHandCards(eventId, p1, p2)
	self:InitCardsShow(false)
end

-- 显示摊牌
function UIMJHandCardCtrl:InitTanPaiHandCards(eventId, p1, p2)
	self:InitCardsShow(true)
end

-- 清理手牌
function UIMJHandCardCtrl:ClearHandCards(eventId, p1, p2)
	self:ClearCards()
end

-- 设置单个牌背面
function UIMJHandCardCtrl:_changeMJPaiBg( index, seatPos, trans )
	if not index or not seatPos then
		return
	end

	local isNormal = 1
	if self.isTanPai then
		isNormal = 2
	end

	LogicUtil.ChangeSingleMJPaiBg(self.luaWindowRoot, index, seatPos, trans, isNormal)
end

-- 设置所有手牌背面
function UIMJHandCardCtrl:_changeMJPaiBgs( index )
	if not index then
		return
	end

	local luaWindowRoot = self.luaWindowRoot
	local isNormal = 1
	if self.isTanPai then
		isNormal = 2
	end
	if index then
		for seatPos,v_cardItems in pairs(self.cardItems) do
			LogicUtil.ChangeMJPaiBgs(luaWindowRoot, index, seatPos, v_cardItems, isNormal)--不摊牌是正常模式，摊牌了相当于回放模式了
		end
	end
end

-- 接手改变牌背事件
-- 第二个参数为牌背选中
function UIMJHandCardCtrl:ChangePlayCanvas(eventId, p1, p2)
	local index = p2

	self:_changeMJPaiBgs(index)
end

function UIMJHandCardCtrl:_CheckHandCard(seatPos, removeSize)
	local cardItems = self.cardItems[seatPos]
	if nil ~= cardItems then
		local count = #cardItems
		if count > 14 or 0 == count % 3 then
			DwDebug.LogError(string.format("UIMJHandCardCtrl cards count error, count:%s, seatPos:%s, removeSize:%s", count, seatPos, removeSize or 0))
		end
	end
end

function UIMJHandCardCtrl:MJRefreshCard(eventId, p1, p2)
	if p1 then
		self:RefreshCard(p1)
	end
end

-- 刷新手牌
function UIMJHandCardCtrl:RefreshCard(seatPos)
	-- 先清除掉某个方位的动画
	MJGameActionManager.KillBySeat(seatPos)

	local cardItems = self.cardItems[seatPos]
	if nil == cardItems then
		DwDebug.LogError("nil == cardTrans UIMJHandCardCtrl:RefreshCard")
		return
	end

	local count = #cardItems

	DwDebug.Log("UIMJHandCardCtrl:RefreshCard")
	DwDebug.Log("count:", count)
	local depth = LogicUtil.MJCardDepth[seatPos]
	for i=1,count do
		local cardItem = cardItems[i]
		local trans = cardItem.trans
		LogicUtil.AdjustMJCards(trans, i, count, seatPos, self.isTanPai)

		LogicUtil.InitMJCardItem(self.luaWindowRoot, trans, cardItem.cardInfo, nil, depth * i)
	end

	self:_CheckHandCard(seatPos)

	if nil == self.upCardTrans then
		self:SelCardHighlight(false)
	else
		self:UpDownCard(self.upCardTrans, true)
	end
end

-- 抓牌
-- cardinfo --> CMJCard 抓牌信息
function UIMJHandCardCtrl:_inCard(player, cardInfo)
	if not player or not player.seatPos then
		DwDebug.Log("----------------UIMJHandCardCtrl:_inCard:nil == player")
		return
	end

	local seatPos = player.seatPos
	DwDebug.Log("----------------UIMJHandCardCtrl:_inCard:", seatPos)
	local trans = self:_initMJCardItem(seatPos, 14, 14, cardInfo)
	self:_CheckHandCard(seatPos)
	if nil == trans then
		return
	end

	self:_changeMJPaiBg(LogicUtil.GetMJPaiType(), seatPos, trans)

	-- 补杠刷新动画
	self:RefreshCard(seatPos)
end

function UIMJHandCardCtrl:InCard(eventId, p1, p2)
	if nil == p1 or nil == p2 then
		return
	end

	self:_inCard(p1, p2)
end

function UIMJHandCardCtrl:_playFaPaiAction(handler)
	local time = 4 --10
	-- handler 如果需要可以注册一个OnActionFinish函数，会回调这个接口
	-- if nil == handler then
	-- 	-- print("----------------nil == handler")
	-- end

	for k,v in pairs(SeatPosEnum) do
		-- 只设置一个回调
		if SeatPosEnum.South == v then
			MJGameActionManager.Play(v, MJFaPaiAction, handler, self.cardItems[v], self.cardItems[v], time)
		else
			MJGameActionManager.Play(v, MJFaPaiAction, nil, self.cardItems[v], self.cardItems[v], time)
		end
	end
end

function UIMJHandCardCtrl:PlayFaPaiAction(eventId, p1, p2)
	self:_playFaPaiAction(p1)
end

function UIMJHandCardCtrl:_playChuPaiAction(seatPos, startIndex, endIndex, nodes)
	if nil == startIndex or nil == endIndex then
		return
	end

	local count = #nodes
	-- print("----------------UIMJHandCardCtrl:_playChuPaiAction")
	-- print("----------------startIndex:",startIndex)
	-- print("----------------endIndex:",endIndex)
	-- print("----------------count:",count)

	local startNode = nodes[count]
	local startTransform = startNode.trans
	startTransform.name = endIndex .. "_cardItem"

	local nodeList = {}
	local moveList = {}
	-- 更新手牌排序
	if startIndex < endIndex then
		for i=startIndex + 1,endIndex do
			table.insert(moveList, nodes[i])
		end
		nodeList[-1] = moveList
	elseif startIndex > endIndex then
		for i=endIndex,startIndex - 1 do
			table.insert(moveList, nodes[i])
		end
		nodeList[1] = moveList
	end

	if count <= endIndex + 1 then
		local moveTime = 1
		MJGameActionManager.Play(seatPos, MJChuPaiMoveAction, nil, startTransform, moveTime, nodeList)
	else
		local moveTime = math.floor((count - endIndex)/5 + 1)
		local endPos = LogicUtil.GetMJCardsPos(endIndex, count, seatPos)
		MJGameActionManager.Play(seatPos, MJChuPaiAction, nil, nodeList, startTransform, endPos, endIndex, moveTime)
	end
end

-- 选中牌高亮
function UIMJHandCardCtrl:SelCardHighlight(isShow)
	-- 如果没选中牌 则派发事件隐藏选中牌
	local luaWindowRoot = self.luaWindowRoot
	if not isShow then
		LuaEvent.AddEventNow(EEventType.MJSelCardHighlight)
		if self.highlightObj then
			for i=1,#self.highlightObj do
				luaWindowRoot:SetActive(self.highlightObj[i],false)
			end
			self.highlightObj = nil
		end
		return
	end

	if self.upCardTrans == nil then
		return
	end
	local own_card = self.cardItems[SeatPosEnum.South]
	if nil == own_card then
		DwDebug.LogError("nil == own_card UIMJHandCardCtrl:SelCardHighlight")
		return
	end

	local split_index = string.find(self.upCardTrans.name, "_")
	local card_index = tonumber(string.sub(self.upCardTrans.name, 1, split_index-1))
	
	local play_logic = PlayGameSys.GetPlayLogic()
	-- 加个保护
	if card_index ~= 14 and card_index > #own_card then
		DwDebug.LogError("手牌管理数据乱了")
		return
	end
	-- 如果是14则是新抓手牌，传最后一张牌的id
	local card_id = card_index == 14 and own_card[#own_card].cardInfo.ID or own_card[card_index].cardInfo.ID

	local ico_masks = {}

	local sel_indexes = {}
	for i=1,#own_card do
		if own_card[i].cardInfo.ID == card_id then
			sel_indexes[#sel_indexes+1] = i
		end
	end

	for i=1,#sel_indexes do
		local index = sel_indexes[i]
		local tr = own_card[index].trans
		ico_masks[#ico_masks+1] = luaWindowRoot:GetTrans(tr, "ico_mask")
	end

	if self.highlightObj then
		for i=1,#self.highlightObj do
			luaWindowRoot:SetActive(self.highlightObj[i],false)
		end
		self.highlightObj = nil
	end

	for i=1,#ico_masks do
		luaWindowRoot:SetActive(ico_masks[i],true)
	end

	self.highlightObj = ico_masks

	LuaEvent.AddEventNow(EEventType.MJSelCardHighlight, card_id)
end

-- 出牌
function UIMJHandCardCtrl:PlayCard(eventid, cardTrans, p2)
	if cardTrans == nil then
		return
	end

	self.outCardTrans = cardTrans

	self:_CheckHandCard(SeatPosEnum.South)
	-- 遍历trans节点，使用名字判断是准确的
	local cardItems = self.cardItems[SeatPosEnum.South]
	if nil == cardItems then
		DwDebug.LogError("nil == cardItems UIMJHandCardCtrl:PlayCard")
		return
	end
	-- print("----------------#own_trans:", #cardItems)
	for i=1,#cardItems do
		local cardItem = cardItems[i]
		if cardTrans == cardItem.trans then
			local cardInfo = cardItem.cardInfo
			if nil == cardInfo then
				DwDebug.LogError("手牌管理数据乱了")
			else
				-- print("----------------play_logic:SendPlayCard:", cardInfo.ID)
				PlayGameSys.GetPlayLogic():SendPlayCard(cardInfo.ID)
			end
			return
		end
	end
end

-- 正常模式，减少其他玩家手牌
-- count --> int 移除牌张数量
function UIMJHandCardCtrl:_removeOtherCard(player, count)
	if not player or not player.seatPos then
		DwDebug.LogError("nil == player UIMJHandCardCtrl:_removeOtherCard")
		return
	end

	local seatPos = player.seatPos
	local cardItems = self.cardItems[seatPos]
	if nil == cardItems then
		DwDebug.LogError("nil == cardTrans UIMJHandCardCtrl:_removeOtherCard")
		return
	end

	-- local startIndex, endIndex = nil, nil

	-- 屏蔽钱其他玩家动画
	-- if 1 == count then
	-- 	-- print("----------------UIMJHandCardCtrl:_removeOtherCard seatPos:", seatPos)
	-- 	-- print("----------------size:", #cardItems)
	-- 	local tab = {}
	-- 	local size = #cardItems
	-- 	for i=1, size - 1 do
	-- 		table.insert(tab, i)
	-- 	end
	-- 	local random = math.random(#tab)
	-- 	startIndex = tab[random]
	-- 	table.remove(tab, startIndex)

	-- 	local count = #tab
	-- 	if 0 == count then
	-- 		endIndex = startIndex
	-- 	else
	-- 		random = math.random(#tab)
	-- 		endIndex = tab[random] or startIndex
	-- 	end

	-- 	-- print("----------------startIndex:", startIndex)
	-- 	-- print("----------------endIndex", endIndex)
	-- 	-- print("----------------#cardItems", #cardItems)
	-- 	self.luaWindowRoot:SetActive(cardItems[startIndex].trans, false)

	-- 	self:_playChuPaiAction(seatPos, startIndex, endIndex, cardItems)

	-- 	-- 位置调整
	-- 	local startNode = cardItems[size]
	-- 	table.remove(cardItems, startIndex)
	-- 	table.insert(cardItems, endIndex, startNode)
	-- 	cardItems[size] = nil -- 末尾置空

	-- 	-- 重新命名
	-- 	for i,v in ipairs(cardItems) do
	-- 		v.trans.name = i .. "_cardItem"
	-- 	end
	-- 	self.cardItems[seatPos] = cardItems
	-- else
		for i=1,count do
			local size = #cardItems
			local trans = cardItems[size].trans
			self.luaWindowRoot:SetActive(trans, false)
			cardItems[size] = nil
		end
		-- print("---------------------outCard count#", #cardItems)
		self.cardItems[seatPos] = cardItems
		self:RefreshCard(seatPos)
	-- end

	self:_CheckHandCard(seatPos, count)
end

function UIMJHandCardCtrl:RemoveOtherCard(eventId, p1, p2)
	if nil == p1 or nil == p2 then
		return
	end

	self:_removeOtherCard(p1, p2)
end

-- 碰、杠、吃移除手牌 出牌也是这里
-- outCardIds --> {id1, id2}
function UIMJHandCardCtrl:_removeSelfCard(player, outCardIds)
	if not player or not player.seatPos then
		DwDebug.LogError("nil == player UIMJHandCardCtrl:_removeSelfCard")
		return
	end

	local tempOutCardIds = ShallowCopy(outCardIds)

	local seatPos = player.seatPos
	local cardItems = self.cardItems[seatPos]
	if nil == cardItems then
		DwDebug.LogError("nil == cardTrans UIMJHandCardCtrl:_removeSelfCard:", seatPos)
		return
	end

	local newCardItems = {}
	local isContain = false

	for k1,v1 in ipairs(cardItems) do
		isContain = false
		for k,v in pairs(tempOutCardIds) do
			if v1.cardInfo.ID == v then
				tempOutCardIds[k] = nil
				self.luaWindowRoot:SetActive(v1.trans,false)
				isContain = true
				-- 这里必须要break，不然同一张牌就被全清掉了
				break
			end
		end

		if not isContain then
			table.insert(newCardItems, v1)
		end
	end

	if nil ~= self.upCardTrans then
		self:UpDownCard(self.upCardTrans, false)
		self.upCardTrans = nil
	end

	LogicUtil.SortMJCardsByItems(newCardItems, true)
	self.cardItems[seatPos] = newCardItems

	self:RefreshCard(seatPos)

	self:_CheckHandCard(seatPos, #outCardIds)
end

function UIMJHandCardCtrl:RemoveSelfCard(eventId, p1, p2)
	if nil == p1 or nil == p2 then
		return
	end

	self:_removeSelfCard(p1, p2)
end

-- 自己手牌出牌
function UIMJHandCardCtrl:_outSelfCard(player, outCardId)
	if not player or not player.seatPos then
		DwDebug.LogError("nil == player UIMJHandCardCtrl:_outSelfCard")
		return
	end

	local seatPos = player.seatPos
	local cardItems = self.cardItems[seatPos]
	if nil == cardItems then
		DwDebug.LogError("nil == cardTrans UIMJHandCardCtrl:_removeSelfCard:", seatPos)
		return
	end

	local newCardItems = {}
	local isContain = false

	local paiID = nil
	local startIndex = nil

	-- 优先隐藏 隐藏点击出牌的那个
	if nil ~= self.outCardTrans then
		for k1, v1 in ipairs(cardItems) do
			-- isContain = false
			if not isContain and (nil == self.outCardTrans or v1.trans == self.outCardTrans) then
				if v1.cardInfo.ID == outCardId then
					isContain = true
					paiID = outCardId
					startIndex = k1
					self.luaWindowRoot:SetActive(v1.trans, false)
				else
					-- 预防upCardTrans值和出牌的值不一致
					if nil ~= self.outCardTrans then
						DwDebug.LogError(string.format("cardinfo.ID ~= outCardId, v1.cardinfo.ID:%s, outCardId:%s", v1.cardinfo.ID, outCardId))
						-- self:UpDownCard(self.upCardTrans, false)
						-- self.upCardTrans = nil
					end
					table.insert(newCardItems, v1)
				end
			else
				table.insert(newCardItems, v1)
			end
		end
	end

	if nil ~= self.upCardTrans then
		self:UpDownCard(self.upCardTrans, false)
		self.upCardTrans = nil
	end

	-- 出一张牌
	if nil ~= startIndex then
		local count = #cardItems
		local paipaiID = cardItems[count].cardInfo.ID
		if paiID == paipaiID then
			endIndex = startIndex
		elseif paiID < paipaiID then
			endIndex = count
			for i = startIndex, count, 1 do
				if cardItems[i].cardInfo.ID >= paipaiID then
					endIndex = i - 1
					break
				end
			end
		else
			endIndex = 1
			for i = startIndex, 1, -1 do
				if cardItems[i].cardInfo.ID <= paipaiID then
					endIndex = i + 1
					break
				end
			end
		end

		self:_playChuPaiAction(seatPos, startIndex, endIndex, cardItems)

		LogicUtil.SortMJCardsByItems(newCardItems, true)
		self.cardItems[seatPos] = newCardItems
	end

	self:_CheckHandCard(seatPos, 1)
end

-- 自己手牌出牌事件
function UIMJHandCardCtrl:OutSelfCard(eventId, p1, p2)
	if nil == p1 or nil == p1 then
		return
	end

	self:_outSelfCard(p1, p2)
end

function UIMJHandCardCtrl:_initMJCardItem(seatPos, index, count, cardInfo)
	local cardItems = self.cardItems[seatPos]
	if nil == cardItems then
		DwDebug.LogError("nil == cardTrans UIMJHandCardCtrl:_initMJCardItem:", seatPos)
		return
	end

	local isTanPai = self.isTanPai
	local cardItemName = isTanPai and self.MJTanPaiCardItemName[seatPos] or self.MJCardItemName[seatPos]
	local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, cardItemName, RessStorgeType.RST_Never, false)
	if not resObj then
		return
	end

	-- 添加拖动出牌功能
	if seatPos == SeatPosEnum.South then
		GetOrAddLuaComponent(resObj, "LuaWindow.LuaUIElement.DragOutCard", true)
		LogicUtil.EnableTouch(resObj.transform, true)
	end

	local trans = resObj.transform
	trans.parent = self.parentNodeTrans[seatPos]
	trans.name = index.."_cardItem"
	trans.localScale = Vector3.one
	LogicUtil.AdjustMJCards(trans, index, count, seatPos, isTanPai)
	self.luaWindowRoot:SetActive(trans,true)

	local depth = LogicUtil.MJCardDepth[seatPos]
	LogicUtil.InitMJCardItem(self.luaWindowRoot, trans, cardInfo, nil, index * depth)

	local cardItem = {}
	cardItem.trans = trans
	cardItem.cardInfo = cardInfo
	table.insert(cardItems, cardItem)

	return trans
end

-- 选中手牌
function UIMJHandCardCtrl:ClickCard(event_id, cardTrans)
	LuaEvent.AddEventNow(EEventType.RefreshRoomSetRoot, false)
	AudioManager.PlayCommonSound(UIAudioEnum.mj_click)

	if LogicUtil.IsMJSingleClick() then
		-- 单击操作
		if nil ~= self.upCardTrans then
			self:UpDownCard(self.upCardTrans, false)
		end

		local play_logic = PlayGameSys.GetPlayLogic()
		if play_logic and play_logic:IsMyTurn() then
			self:PlayCard(EEventType.MJPlayCard, cardTrans)
			return
		else
			if cardTrans == self.upCardTrans then
				self.upCardTrans = nil
			else
				self.upCardTrans = cardTrans
				self:UpDownCard(cardTrans, true)
			end
		end
	else
		local isUp = true
		if self.upCardTrans then
			if cardTrans == self.upCardTrans then
				-- print("----------------UIMJHandCardCtrl:ClickCard")
				isUp = false
				local play_logic = PlayGameSys.GetPlayLogic()

				if play_logic and play_logic:IsMyTurn() then
					self:PlayCard(EEventType.MJPlayCard, self.upCardTrans)
					return
				end
			else
				self:UpDownCard(self.upCardTrans, false)
			end
		end
		if isUp then
			self.upCardTrans = cardTrans
			self:UpDownCard(cardTrans, true)
		else
			-- 打出去后在取消
			self.upCardTrans = nil
			self:UpDownCard(cardTrans, false)
		end
	end
end

-- 自己手牌调用
function UIMJHandCardCtrl:UpDownCard(cardTrans, isUp)
	if cardTrans then
		if isUp then
			self:SelCardHighlight(true)
			cardTrans.localPosition = Vector3.New(cardTrans.localPosition.x,26,0)
		else
			self:SelCardHighlight(false)
			cardTrans.localPosition = Vector3.New(cardTrans.localPosition.x,0,0)
		end
	end
end

function UIMJHandCardCtrl:InitCardsShow(isTanPai)
	-- print("----------------UIMJHandCardCtrl:InitCardsShow")
	self:ClearCards()

	self.isTanPai = isTanPai
	local posTabs = isTanPai and self.MJTanPaiSeatNodePos or self.MJSeatNodePos

	local player
	local cardPlayLogic = PlayGameSys.GetPlayLogic()
	for i=0,3 do
		player = cardPlayLogic.roomObj.playerMgr:GetPlayerBySeatID(i)
		if player then
			local seatPos = player.seatPos
			self.cardItems[seatPos] = {}
			-- 重置node节点位置
			self.parentNodeTrans[seatPos].localPosition = posTabs[seatPos]

			local cards = ShallowCopy(player.cardMgr:GetHandCards())
			LogicUtil.SortMJCards(cards, true)
			local count = #cards
			for k=1, count do
				self:_initMJCardItem(seatPos, k, count, cards[k])
			end
			self:_CheckHandCard(seatPos)
		end
	end

	self:_changeMJPaiBgs(LogicUtil.GetMJPaiType())
end

function UIMJHandCardCtrl:ClearCards()
	MJGameActionManager.Kill()

	-- print("----------------UIMJHandCardCtrl:ClearCards")
	local luaWindowRoot = self.luaWindowRoot
	for seatPos,v_cardItems in pairs(self.cardItems) do
		for k,v_cardTransItem in pairs(v_cardItems) do
			luaWindowRoot:SetActive(v_cardTransItem.trans,false)
		end
	end
	if self.highlightObj then
		for i=1,#self.highlightObj do
			self.luaWindowRoot:SetActive(self.highlightObj[i],false)
		end
		self.highlightObj = nil
	end

	self.isTanPai = nil
	self.upCardTrans = nil
	self.outCardTrans = nil
	self.cardItems = {}
end

function UIMJHandCardCtrl:Destroy()
	self:UnRegisterEvent()
	self:ClearCards()
end
