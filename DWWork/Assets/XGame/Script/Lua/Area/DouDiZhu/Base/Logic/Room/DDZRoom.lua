--------------------------------------------------------------------------------
--   File      : DDZRoom.lua
--   author    : zhanghaochun
--   function  : 斗地主房间
--   date      : 2018-01-26 
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.Room.CBaseRoom"
require "Area.DouDiZhu.Base.Logic.Room.RoomStateMgr.DDZRoomStateMgrModule"

DDZRoom = class("DDZRoom", CBaseRoom)

function DDZRoom:ctor()
	self.roomStateMgr = DDZRoomStateMgrModule.New()    -- 房间状态管理
	self:BaseCtor()
end

function DDZRoom:SeatTranslate(seatinfo)
	local seatDistance = seatinfo.seatId - self.selfSeatInfo.seatId
	if seatDistance == 1 or seatDistance == -2 then
		self.seatConfig[seatinfo.userId] = SeatPosEnum.East
		self.seatConfig[SeatPosEnum.East] = seatinfo.userId
	elseif seatDistance == 2 or seatDistance == -1 then
		self.seatConfig[seatinfo.userId] = SeatPosEnum.West
		self.seatConfig[SeatPosEnum.West] = seatinfo.userId
	end
end

function DDZRoom:InitPlayerStatus(player)
	if player then
		local roomStatus = self.roomStateMgr:GetCurStateType()
		if roomStatus == 7 then
			-- 房间抢地主
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			if player.seatInfo.userId == self.curWorkingID then
				player:ChangeState(PlayerStateEnum.WantLord)
			else
				player:ChangeState(PlayerStateEnum.Waiting)
			end
		elseif roomStatus == 8 then
			-- 房间加倍(农民)
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			if player.seatInfo.userId == self.curWorkingID then
				player:ChangeState(PlayerStateEnum.FamerAddPlus)
			else
				player:ChangeState(PlayerStateEnum.Waiting)
			end
		elseif roomStatus == 9 then
			-- 房间地主明牌
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			if player.seatInfo.userId == self.curWorkingID then
				player:ChangeState(PlayerStateEnum.LordIsOpenPlay)
			else
				player:ChangeState(PlayerStateEnum.Waiting)
			end
		elseif roomStatus == 10 then
			-- 房间加倍(地主)
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			if player.seatInfo.userId == self.curWorkingID then
				player:ChangeState(PlayerStateEnum.LordSubPlus)
			else
				player:ChangeState(PlayerStateEnum.Waiting)
			end
		elseif roomStatus == 1 then
			-- playing
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			if player.seatInfo.userId == self.curWorkingID then
			else
				player:ChangeState(PlayerStateEnum.Waiting)
			end
		else
			player:ChangeState(PlayerStateEnum.Init)
		end
	end
end

function DDZRoom:InitSelfHandCards()
	local selfPlayer = self.playerMgr:GetPlayerByPlayerID(self:GetSouthUID())
	local saveHandCards = selfPlayer.cardMgr:GetHandCards()
	local saveLen = 0
	if saveHandCards then
		saveLen = #saveHandCards
	end

--	if #self.reconnectGameInfo.handPai ~= saveLen then --缓存与服务端下发不一致更新
		--先清理缓存手牌
		selfPlayer.cardMgr:Clear()
		-- 玩家自己手牌
		self.playerMgr:InitPlayerHandCards(self:GetSouthUID(),self.reconnectGameInfo.handPai)
		-- 展示手牌(会先清理已经显示的手牌，然后初始化新的)
		LuaEvent.AddEventNow(EEventType.SelfHandCardInit,false,nil)
--	end
end

function DDZRoom:ResetPlayerStatus()
	for i=1,4 do
		local player = self.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
		if player then
			player:ChangeState(PlayerStateEnum.Waiting)
			LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,player)
		end
	end
end

--断线重连回来刷新所有的游戏信息
function DDZRoom:RefreshGameInfo(gameInfo)
	self:BaseRefreshGameInfo(gameInfo)

	local roomStatus = self.roomInfo.status

	self:CleanRoom()

	-- 取消显示地主明的牌
	LuaEvent.AddEvent(EEventType.DDZCloseLordOpenPlayCard)
	if roomStatus == 7 then
		-- 房间抢地主
		self:InitSelfHandCards()
		self:ResetPlayerStatus()
		local rsp = self:ToPBHintRob(gameInfo)

		LuaEvent.AddEvent(EEventType.NotifyWantLordEvent, rsp)
		
		for k, v in ipairs(gameInfo.historyRob) do
			if v ~= 0 then
				local RobRsp = {}
				RobRsp.seatId = k-1
				RobRsp.isRob = v ~= 4
				RobRsp.score = v
				if v == 4 then RobRsp.score = 0 end
				LuaEvent.AddEvent(EEventType.DDZWantLordScoreEvent, RobRsp)
			end
		end
		
		local player = self.playerMgr:GetPlayerBySeatID(gameInfo.choiceSeatId)
		if player then
			self.playerMgr:DDZWantLordState(player.seatInfo.userId)
			self:SaveWorkingPlayerID(player.seatInfo.userId)
		end
	elseif roomStatus == 8 then
		-- 房间加倍(农民)
		self.parent.bankerSeatId = gameInfo.bankerSeatId

		self:InitSelfHandCards()
		self:ResetPlayerStatus()		

		local rsp = self:ToPBAskPlus(gameInfo)

		LuaEvent.AddEvent(EEventType.DDZNotifyFarmerAddDouble, rsp)
		local player = self.playerMgr:GetPlayerBySeatID(gameInfo.choiceSeatId)
		if player then
			self.playerMgr:DDZFarmerAddDoubleState(player.seatInfo.userId)
			self:SaveWorkingPlayerID(player.seatInfo.userId)
		end
		self:DealPlusInfo(gameInfo.plusSeatId)
	elseif roomStatus == 9 then
		-- 房间地主明牌
		self.parent.bankerSeatId = gameInfo.bankerSeatId
		self:InitSelfHandCards()
		self:ResetPlayerStatus()
		local rsp = self:ToPBHintOpen(gameInfo)

		LuaEvent.AddEvent(EEventType.DDZNotifyLordOpenPlay, rsp)
		local player = self.playerMgr:GetPlayerBySeatID(gameInfo.bankerSeatId)
		if player then
			self.playerMgr:DDZLordOpenCardState(player.seatInfo.userId)
			self:SaveWorkingPlayerID(player.seatInfo.userId)
		end
	elseif roomStatus == 10 then
		-- 房间地主反加倍
		self.parent.bankerSeatId = gameInfo.bankerSeatId
		self:InitSelfHandCards()
		self:ResetPlayerStatus()

		local rsp = self:ToPBHintBack(gameInfo)

		LuaEvent.AddEvent(EEventType.DDZNotifyLordAddDouble, rsp)
		local player = self.playerMgr:GetPlayerBySeatID(gameInfo.bankerSeatId)
		if player then
			self.playerMgr:DDZLordAddDoubleState(player.seatInfo.userId)
			self:SaveWorkingPlayerID(player.seatInfo.userId)
		end
		self:DealPlusInfo(gameInfo.plusSeatId)
	elseif roomStatus == 1 then
		-- 房间游戏中
		self.parent.bankerSeatId = gameInfo.bankerSeatId
		self:InitSelfHandCards()
		self:ResetPlayerStatus()

		--展示新的桌牌
		for k,v in pairs(gameInfo.desktopPai) do -- 这个是按照出牌顺序的
			-- 添加到一轮出牌队列
			self.parent.cardLogicContainer:AddOneOutCards(v)

			LuaEvent.AddEventNow(EEventType.PlayOutCardAction,v,true)
		end
		
		if gameInfo.curOutPaiSeat then
			local player = self.playerMgr:GetPlayerBySeatID(gameInfo.curOutPaiSeat)
			if player then
				--如果是自己出牌
				if player.seatInfo.userId == self:GetSouthUID() then
					-- 出牌后重置AI提示结果
					self.parent.cardLogicContainer:ResetAIResult()
					local lastOutCardInfo = self.parent.cardLogicContainer:GetLastOutCardInfo()
					if lastOutCardInfo then
						local srcCardIds = lastOutCardInfo.pai
						local srcCards = {}
						for k,v in pairs(srcCardIds) do
							cardItem = CCard.New()
							cardItem:Init(v,k)
							table.insert(srcCards,cardItem)
						end
						local handCards = player.cardMgr:GetHandCards()
					end
				end

				self.playerMgr:PlayerOutCardTips(player.seatInfo.userId,gameInfo.outPaiToken,gameInfo.mustOutPai)
				self:SaveWorkingPlayerID(player.seatInfo.userId)
			end
		end
	end

	-- 重新显示底牌
	if roomStatus == 0 then
		-- 隐藏底牌显示
		LuaEvent.AddEvent(EEventType.DDZShowBaseCards, nil, 1)
	elseif roomStatus == 7 then
		-- 显示底牌牌背
		LuaEvent.AddEvent(EEventType.DDZShowBaseCards, nil, 2)
	else
		-- 显示底牌牌面
		local rsp = self:ToPBWildCards(gameInfo)
		LuaEvent.AddEvent(EEventType.DDZShowBaseCards, rsp)
	end

	-- 显示地主明牌的牌
	if not (roomStatus == 0 or roomStatus == 7 or roomStatus == 9) then
		local player = self.playerMgr:GetPlayerBySeatID(gameInfo.bankerSeatId)
		if player then
			local rsp = self:ToPBUseOpen(gameInfo)
			LuaEvent.AddEvent(EEventType.DDZLordOpenPlay, rsp)
		end
	end

	-- 显示地主图标
	if not (roomStatus == 0 or roomStatus == 7) then
		local player = self.playerMgr:GetPlayerBySeatID(gameInfo.bankerSeatId)
		if player then
			LuaEvent.AddEvent(EEventType.DDZLordFind, player.seatPos, true)
		end
	end

	--剩余牌数
	if roomStatus ~= 0 and roomStatus ~= 2 and roomStatus ~= 4 then
		for i=1,4 do
			local player = self.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
			if player then
				-- 剩余牌数
				if gameInfo.restNum[i] then
					LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards,player.seatPos,gameInfo.restNum[i])
				end
			end
		end
	end
	

	-- 获取倍数信息(后台给)
	self.parent.MyPBMultiple = gameInfo.multiple
	LuaEvent.AddEvent(EEventType.DDZMultiplePBInfo, self.parent.MyPBMultiple)
end

---------------------------根据gameInfo拼装信息-----------------------------------
-- 提示抢地主的消息
function DDZRoom:ToPBHintRob(gameInfo)
	local rsp = {}
	rsp.seatId = gameInfo.choiceSeatId
	rsp.score = gameInfo.robScore
	rsp.isForce = gameInfo.isForceRob

	return rsp
end

-- 提示农民加倍
function DDZRoom:ToPBAskPlus(gameInfo)
	local rsp = {}
	rsp.seatId = gameInfo.choiceSeatId
		
	return rsp
end

-- 提示地主是否明牌
function DDZRoom:ToPBHintOpen(gameInfo)
	local rsp = {}
	rsp.seatId = gameInfo.bankerSeatId

	return rsp
end

-- 提示地主加倍
function DDZRoom:ToPBHintBack(gameInfo)
	local rsp = {}
	rsp.seatId = gameInfo.bankerSeatId
	rsp.plusSeatId = gameInfo.plusSeatId
	
	return rsp
end

-- 广播的底牌信息
function DDZRoom:ToPBWildCards(gameInfo)
	local rsp = {}
	rsp.bankerUserId = gameInfo.bankerUserId
	rsp.bankerSeatId = gameInfo.bankerSeatId
	rsp.baseScore = gameInfo.baseScore
	rsp.pai = gameInfo.wildCards

	return rsp
end

-- 广播地主明牌的信息
function DDZRoom:ToPBUseOpen(gameInfo)
	local rsp = {}
	rsp.seatId = gameInfo.bankerSeatId
	rsp.isOpen = gameInfo.diZhuIsOpen
	rsp.pai = gameInfo.diZhuPai

	return rsp
end
--------------------------------------------------------------------------------
-- 处理断线重连时加倍与不加倍的信息
function DDZRoom:DealPlusInfo(plusInfos)
	for k, v in ipairs(plusInfos) do
		if v ~= 0 then
			local seatIndex = k - 1
			local player = self.playerMgr:GetPlayerBySeatID(seatIndex)
			if player then
				if seatIndex == self.parent.bankerSeatId then
					-- 地主
					local rsp = {}
					rsp.seatId = seatIndex
					rsp.isBack = v == 1
					LuaEvent.AddEvent(EEventType.DDZLordAddDouble, rsp)
					LuaEvent.AddEvent(EEventType.DDZLordAddDouble, nil)
				else
					-- 农民
					local rsp = {}
					rsp.seatId = seatIndex
					rsp.isPlus = v == 1
					LuaEvent.AddEvent(EEventType.DDZFarmerAddDouble, rsp)
				end
			end
		end
	end
end

-- 重写拷贝距离信息数据
function DDZRoom:SetPBGeoRsp(PBGeoRsp)
	if PBGeoRsp ~= nil then
		self.PBGeoRsp = {}
		local distance = {}
		for key, PBGeoItem in pairs(PBGeoRsp.Items) do
			if next(PBGeoItem) ~= nil then
				local data = {}
				data.uIdA = PBGeoItem.srcUid
				data.uIdB = PBGeoItem.destUid
				data.distance = PBGeoItem.distance
				distance[key] = data
			end
		end
		self.PBGeoRsp.distance = distance
	end
end