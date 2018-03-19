--------------------------------------------------------------------------------
-- 	 File      : CWSKRoom.lua
--   author    : guoliang
--   function   : 510K房间
--   date      : 2017-09-26 
--				 modify 2017-11-5
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.Room.CBaseRoom"
require "Logic.RoomLogic.Module.PKRoomStateMgrModule"

CWSKRoom = class("CWSKRoom", CBaseRoom)

function CWSKRoom:ctor()
	self.roomStateMgr = PKRoomStateMgrModule.New()-- 房间状态管理
	self:BaseCtor()
	--是否可以出牌
	self.canOutCard = true
end

function CWSKRoom:SetAlonePlayerID(alonePlayerID)
	self.alonePlayerID = alonePlayerID
end

function CWSKRoom:SetFriendCardID(friendPlayerID,friendCardID)
	self.friendCardID = friendCardID
	self.friendPlayerID = friendPlayerID
end

-- 清理房间显示
function CWSKRoom:CleanRoom()
	self.friendCardID = 0
	self.super.CleanRoom(self)
end

--清理自己的手牌 目前主要是清理牌的tag
function CWSKRoom:ClearSelfHandCard()
	local selfPlayer = self.playerMgr:GetPlayerByPlayerID(self:GetSouthUID())
	if selfPlayer then
		--先清理缓存手牌
		selfPlayer.cardMgr:ClearHandCard()
	end
end

--断线重连回来刷新所有的游戏信息
function CWSKRoom:RefreshGameInfo(gameInfo)
	self:BaseRefreshGameInfo(gameInfo)

	local roomStatus = self.roomInfo.status
	local isGameStart = false

	self:CleanRoom()

	if roomStatus == RoomStateEnum.CallAlone then -- 房间打独中
		self:InitSelfHandCards()
		self:ResetPlayerStatus()
		if gameInfo.isChoiceAlone then
			local player = self.playerMgr:GetPlayerBySeatID(gameInfo.choiceSeatId)
			if player then
				self.playerMgr:PlayerCallAloneTips(player.seatInfo.userId)
				self:SaveWorkingPlayerID(player.seatInfo.userId)
			end
		end
	elseif roomStatus == RoomStateEnum.FindFriend then -- 房间找朋友中
		self:InitSelfHandCards()
		self:ResetPlayerStatus()
		if gameInfo.isChoiceFriend then
			local player = self.playerMgr:GetPlayerBySeatID(gameInfo.choiceSeatId)
			if player then
				self.playerMgr:PlayerFindFriendTips(player.seatInfo.userId)
			 	self:SaveWorkingPlayerID(player.seatInfo.userId)
			end
		end
	elseif roomStatus == RoomStateEnum.Playing then -- 房间游戏中
		self:ResetPlayerStatus()
		-- 是否打独
		if gameInfo.isAlone then
			local alonePlayer = self.playerMgr:GetPlayerBySeatID(gameInfo.bankerSeatId)
			if alonePlayer then
				self:SetAlonePlayerID(alonePlayer.seatInfo.userId)
			end
			LuaEvent.AddEventNow(EEventType.PlayerShowAlone,alonePlayer.seatPos,true)
		else
			-- 朋友亮牌显示
			LuaEvent.AddEventNow(EEventType.PlayerShowFriendCard,gameInfo.friendPai,true)
			-- 亮牌玩家
			local showCardPlayer = self.playerMgr:GetPlayerBySeatID(gameInfo.bankerSeatId)
			if showCardPlayer then
				self:SetFriendCardID(showCardPlayer.seatInfo.userId,gameInfo.friendPai)
				LuaEvent.AddEventNow(EEventType.PlayerShowZhuangJia, showCardPlayer.seatPos, true)
			end
			-- 朋友关系显示处理 
			if gameInfo.friendShow then
				LuaEvent.AddEventNow(EEventType.PlayerShowFriend,SeatPosEnum.South,true)
				local friendSeatId = gameInfo.friendShip[self.selfSeatInfo.seatId+1]--lua从1开始
				local friendPlayer = self.playerMgr:GetPlayerBySeatID(friendSeatId)
				if friendPlayer then
					LuaEvent.AddEventNow(EEventType.PlayerShowFriend,friendPlayer.seatPos,true)
				end
			end
		end
		--在找朋友后面刷牌
		self:InitSelfHandCards()

		LuaEvent.AddEventNow(EEventType.RefreshCurTablePoint,gameInfo.desktopScore)
		-- 桌面已出牌显示
	
		--展示新的桌牌
		for k,v in pairs(gameInfo.desktopPai) do -- 这个是按照出牌顺序的
			-- 添加到一轮出牌队列
			self.parent.cardLogicContainer:AddOneOutCards(v)

			LuaEvent.AddEventNow(EEventType.PlayOutCardAction,v,true)
		end
		--抓分和倍数
		for i=1,4 do
			local player = self.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
			if player then
				-- 玩家手牌抓分
				if gameInfo.catchScore[i] and gameInfo.catchScore[i] > 0 then
					self.pointMgr:AddPlayPoint(player.seatPos,gameInfo.catchScore[i], false)
				end
				--玩家炸弹倍数
				if gameInfo.bombOdds[i] and gameInfo.bombOdds[i] > 0 then
					self.pointMgr:AddPlayBombNum(player.seatPos,gameInfo.bombOdds[i], false)
				end
				-- 几游信息显示
				LuaEvent.AddEventNow(EEventType.PlayerShowSort,player.seatPos,gameInfo.overOrder[i])
				-- 剩余牌数
				if gameInfo.restNum[i] and gameInfo.restNum[i] > 0 then
					LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards,player.seatPos,gameInfo.restNum[i])
				end
			end
		end

		self.canOutCard = true
		--出牌者状态更新
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
						local introduceCount = self.parent.cardLogicContainer:CreateAIResult(handCards,srcCards)
						DwDebug.Log("RefreshGameInfo introduceCount = "..introduceCount)
						if introduceCount <= 0 then
							WindowUtil.LuaShowTips("当前找不到可以大过上家的牌")
							self.canOutCard = false
						end
						--是否可以出牌
					end
				end

				self.playerMgr:PlayerOutCardTips(player.seatInfo.userId,gameInfo.outPaiToken,gameInfo.mustOutPai)
				self:SaveWorkingPlayerID(player.seatInfo.userId)
			end
		end
	end


end

function CWSKRoom:ResetPlayerStatus()
	for i=1,4 do
		local player = self.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
		if player then
			player:ChangeState(PlayerStateEnum.Waiting)
			LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,player)
		end
	end
end

function CWSKRoom:InitSelfHandCards()
	local selfPlayer = self.playerMgr:GetPlayerByPlayerID(self:GetSouthUID())
	--先清理缓存手牌
	selfPlayer.cardMgr:Clear()
	-- 玩家自己手牌
	self.playerMgr:InitPlayerHandCards(self:GetSouthUID(),self.reconnectGameInfo.handPai)
	-- 展示手牌(会先清理已经显示的手牌，然后初始化新的)
	LuaEvent.AddEventNow(EEventType.SelfHandCardInit,false,nil)
end

function CWSKRoom:InitPlayerStatus(player)
	-- 玩家上下线 会推送信息 需要 查看玩家是否叫牌 或者 亮牌 或者出牌中(服务端动不动刷新座位的尴尬)
	if player then
		local roomStatus = self.roomStateMgr:GetCurStateType()
		if roomStatus == RoomStateEnum.CallAlone then
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			if player.seatInfo.userId == self.curWorkingID then
				player:ChangeState(PlayerStateEnum.CallAlone)
			else
				player:ChangeState(PlayerStateEnum.Waiting)
			end
		elseif roomStatus == RoomStateEnum.FindFriend then
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			if player.seatInfo.userId == self.curWorkingID then
				player:ChangeState(PlayerStateEnum.FindFriend)
			else
				player:ChangeState(PlayerStateEnum.Waiting)
			end
		elseif roomStatus == RoomStateEnum.Playing then
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			if player.seatInfo.userId == self.curWorkingID then
				player:ChangeState(PlayerStateEnum.Playing)
			else
				player:ChangeState(PlayerStateEnum.Waiting)
			end
		else
			player:ChangeState(PlayerStateEnum.Init)
		end
	end
end

-- 重写拷贝距离信息数据
function CWSKRoom:SetPBGeoRsp(PBGeoRsp)
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