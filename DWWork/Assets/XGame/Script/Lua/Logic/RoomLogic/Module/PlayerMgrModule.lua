--------------------------------------------------------------------------------
-- 	 File      : PlayerMgrModule.lua
--   author    : guoliang
--   function   : 玩家管理组件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerMgrModule = class("PlayerMgrModule", DWBaseModule)

function PlayerMgrModule:ctor()
	self.playerList = {} -- 玩家索引列表
	self.seatList = {}-- 座位索引列表
end

function PlayerMgrModule:Init(parent, maxSize)
	self.parent = parent
	self.maxSize = maxSize or 4
	self.count = 0
end

-- 不同的房间可以手动调用此函数设置满员大小
function PlayerMgrModule:SetMaxSize(maxSize)
	self.maxSize = maxSize
end

function PlayerMgrModule:GetMaxSize()
	return self.maxSize
end

function PlayerMgrModule:SetCount(count)
	if count >= self.maxSize then
		-- 房间满员事件
		LuaEvent.AddEventNow(EEventType.RefreshRoomFull, true)
	elseif self.count >= self.maxSize and count <= self.maxSize then
		-- 房间未满员事件
		LuaEvent.AddEventNow(EEventType.RefreshRoomFull, false)
	end
	self.count = count
end

function PlayerMgrModule:GetCount()
	return self.count
end

function PlayerMgrModule:GetPlayerlist()
	return self.playerList
end

--添加玩家
function PlayerMgrModule:AddPlayer(seatInfo)
	seatInfo.fullNickName = seatInfo.nickName
	seatInfo.nickName = utf8sub(seatInfo.nickName,1,10)

	local player = self.playerList[seatInfo.userId]
	if player == nil then
		player = CPlayer.New()
		player:Init(seatInfo)
		self.playerList[seatInfo.userId] = player
		--服务端座位id 与玩家映射
		self.seatList[seatInfo.seatId] = seatInfo.userId
		
		-- 新加成功
		self:SetCount(self.count + 1)
	else
		player.seatInfo = seatInfo
	end

	return player
end

--删除玩家
function PlayerMgrModule:DelPlayer(playerID)
	local player = self.playerList[playerID]
	if player and player.seatInfo then
		player:ChangeState(PlayerStateEnum.Destroy)
		self.playerList[playerID] = nil
		self.seatList[player.seatInfo.seatId] = nil
		-- 减员成功
		self:SetCount(self.count - 1)
	end
end
-- 房间初始化时设置所有玩家显示
function PlayerMgrModule:RoomInit()
	for k,v in pairs(self.playerList) do
		v:ChangeState(PlayerStateEnum.Init)
	end
end

--玩家空闲
function PlayerMgrModule:PlayerIdle(playerID)
	local player = self.playerList[playerID]
	if player then
		player:ChangeState(PlayerStateEnum.Idle)
	end
end

--玩家等候其他人出牌
function PlayerMgrModule:PlayerWaiting(playerID)
	local player = self.playerList[playerID]
	if player then
		player:ChangeState(PlayerStateEnum.Waiting)
	end
end

--轮到玩家独占操作，PlayerExpectPushCardState状态需要在任意玩家进入playing状态的时候退出
function PlayerMgrModule:PlayerMonopolizePlaying(playerID)
	for k,v in pairs(self.playerList) do
		if k == playerID then
			v:ChangeState(PlayerStateEnum.Playing)
		else
			v:ChangeState(PlayerStateEnum.Waiting)
		end
	end
end

--玩家准备通知
function PlayerMgrModule:PlayerPrepared(playerID)
	local player = self.playerList[playerID]
	if player then
		player:ChangeState(PlayerStateEnum.Prepared)
	end
end

-- 玩家上下线通知
function PlayerMgrModule:PlayerOnlineOrOffline(playerID,onlineStatus)
	local player = self.playerList[playerID]
	if player then
		player.seatInfo.onlineStatus = onlineStatus
		LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,onlineStatus)
	end
end

-- 玩家询问叫牌提示(服务端不会推送玩家拒绝叫牌的信息，只能依靠提示其他人叫牌或者找朋友消息取消上一个玩家的叫牌状态)
function PlayerMgrModule:PlayerCallAloneTips(playerID)
	for k,v in pairs(self.playerList) do
		v:ChangeState(PlayerStateEnum.Waiting)
	end
	if self.playerList[playerID] then
		self.playerList[playerID]:ChangeState(PlayerStateEnum.CallAlone)
	end
end

-- 玩家询问出牌提示
function PlayerMgrModule:PlayerOutCardTips(playerID,token,isForce)
	local player = self.playerList[playerID]
	if player then
		player:SetCurOutCardInfo(token,isForce)
		player:ChangeState(PlayerStateEnum.Playing)
	end
end

-- 玩家询问解散提示
function PlayerMgrModule:PlayerDismissTips(playerID)
	local player = self.playerList[playerID]
	if player then
		player:ChangeState(PlayerStateEnum.Dismiss)
	end
end

-- 玩家找朋友亮牌提示(服务端不会推送玩家拒绝叫牌的信息，只能依靠提示其他人叫牌或者找朋友消息取消上一个玩家的叫牌状态)
function PlayerMgrModule:PlayerFindFriendTips(playerID)
	for k,v in pairs(self.playerList) do
		v:ChangeState(PlayerStateEnum.Waiting)
	end
	if self.playerList[playerID] then
		self.playerList[playerID]:ChangeState(PlayerStateEnum.FindFriend)
	end
end
--所有玩家进入未准备待机状态
function PlayerMgrModule:AllPlayerIdle(notSelf)
	for k,v in pairs(self.playerList) do
		if v then
			if not notSelf or k ~= self:GetSouthUID() then
				-- 已经准备状态的，不重置为待机
				if not v:IsState(PlayerStateEnum.Prepared) then
					v:ChangeState(PlayerStateEnum.Idle)
				end
			end
		end
	end
end

--所有玩家进入等待状态
function PlayerMgrModule:AllPlayerWaiting()
	for k,v in pairs(self.playerList) do
		if v then
			v:ChangeState(PlayerStateEnum.Waiting)
		end
	end
end

-- 玩家进入抢地主状态
function PlayerMgrModule:DDZWantLordState(playerID)
	self:AllPlayerWaiting()

	if self.playerList[playerID] then
		self.playerList[playerID]:ChangeState(PlayerStateEnum.WantLord)
	end
end
-- 斗地主，地主进入明牌状态
function PlayerMgrModule:DDZLordOpenCardState(playerID)
	self:AllPlayerWaiting()

	if self.playerList[playerID] then
		self.playerList[playerID]:ChangeState(PlayerStateEnum.LordIsOpenPlay)
	end
end
-- 斗地主，农民进入加倍状态
function PlayerMgrModule:DDZFarmerAddDoubleState(playerID)
	self:AllPlayerWaiting()

	if self.playerList[playerID] then
		self.playerList[playerID]:ChangeState(PlayerStateEnum.FamerAddPlus)
	end
end
-- 斗地主，地主进入加倍状态
function PlayerMgrModule:DDZLordAddDoubleState(playerID)
	self:AllPlayerWaiting()

	if self.playerList[playerID] then
		self.playerList[playerID]:ChangeState(PlayerStateEnum.LordSubPlus)
	end
end

-- 玩家切换状态
function PlayerMgrModule:ChangeStateByUID(userId, next_state)
	local player = self.playerList[playerID]
	if player then
		player:ChangeState(next_state)
	end
end

-- 初始化玩家手牌
function PlayerMgrModule:InitPlayerHandCards(playerID,handCardIds)
	local player = self.playerList[playerID]
	if player then
		player.cardMgr:InitCardsFromSvr(handCardIds)
	end
end

-- 初始化玩家摊牌
function PlayerMgrModule:InitTanpai(tanpai_cards)
	for k,v in pairs(self.playerList) do
		local found_cards = nil
		for index,card_ids in ipairs(tanpai_cards) do
			if next(card_ids) ~= nil then
				if k == card_ids.userId then
					found_cards = card_ids
				end
			end
		end
		if v and v.cardMgr and found_cards then
			v.cardMgr:InitCardsFromSvr(found_cards, true)
		end
	end
end

function PlayerMgrModule:GetPlayerByPlayerID(playerID)
	return self.playerList[playerID]
end

function PlayerMgrModule:GetPlayerBySeatID(seatID)
	local playerID = self.seatList[seatID]
	if playerID then
		return self.playerList[playerID]
	else
		return nil
	end
end

function PlayerMgrModule:GetSouthUID()
	if self.south_uid then
		return self.south_uid
	end

	local self_uid = DataManager.GetUserID()

	if self.playerList[self_uid] then
		self.south_uid = self_uid 
		return self_uid
	else 
		if self.seatList[0] then
			self.south_uid = self.seatList[0]
			return self.south_uid
		else 
			return nil
		end
	end
end

function PlayerMgrModule:CheckPlayerNumIsLegal(legalNum)
	if not legalNum or legalNum <= 0 then
		DwDebug.LogError("CheckPlayerNumIsLegal is true because legalNum is nil or smaller than 0")
		return true
	else
		local curPlayerNum = 0
		for k,v in pairs(self.playerList) do
			if v then
				curPlayerNum = curPlayerNum + 1
			end
		end
		if curPlayerNum == legalNum then
			return true
		end
		DwDebug.LogError("CheckPlayerNumIsLegal is false because curPlayerNum = "..curPlayerNum .. "legalNum = "..legalNum)
	end
	return false	
end

function PlayerMgrModule:GetPlayerRealNum()
	local curPlayerNum = 0
	for k,v in pairs(self.playerList) do
		if v then
			curPlayerNum = curPlayerNum + 1
		end
	end
	return curPlayerNum
end

function PlayerMgrModule:Destroy()
	self:SetCount(0)
	for k,v in pairs(self.playerList) do
		if v then
			v:Destroy()
		end
	end
	self.playerList = nil
	self.seatList = nil
end
