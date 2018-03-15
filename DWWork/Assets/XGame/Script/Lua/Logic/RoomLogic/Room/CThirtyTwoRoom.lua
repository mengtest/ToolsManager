--------------------------------------------------------------------------------
--   File      : CThirtyTwoRoom.lua
--   author    : jianing
--   function  : 32张房间
--   date      : 2018-01-15 
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.Room.CBaseRoom"
require "Logic.RoomLogic.Module.ThirtyTwo.ThirtyTwoRoomStateMgrModule"

CThirtyTwoRoom = class("CThirtyTwoRoom", CBaseRoom)

function CThirtyTwoRoom:ctor()
	self.roomStateMgr = ThirtyTwoRoomStateMgrModule.New()-- 房间状态管理
	self:BaseCtor()
end

-- 清理房间显示
function CThirtyTwoRoom:CleanRoom()
	LuaEvent.AddEventNow(EEventType.HideHandCard)	--清理牌
	LuaEvent.AddEventNow(EEventType.HidePaiXing)	--清理牌型
	LuaEvent.AddEventNow(EEventType.PK32HideKaiPaiBtn)	--清理状态按钮
	LuaEvent.AddEventNow(EEventType.ThirtyTwo_ShowBetBtns,false)	--清理下注按钮
	LuaEvent.AddEventNow(EEventType.PK32HideCountDown)	--倒计时
	LuaEvent.AddEventNow(EEventType.PK32HideFanPai)	--发牌
	LuaEvent.AddEventNow(EEventType.ThirtyTwo_HideCuoPai)	--搓牌

	self:ResetPlayerStatus()
	--self:BaseCtor()
end

--断线重连回来刷新所有的游戏信息
function CThirtyTwoRoom:RefreshGameInfo(gameInfo)
	self.reconnectGameInfo = gameInfo
	local roomStatus = self.roomStateMgr:GetCurStateType()

	-- self:CleanRoom() 已经在房间信息里面清状态了
	if roomStatus == RoomStateEnum.Playing then -- 房间游戏中
		--self:ResetPlayerStatus()
		local isInBet = self:CheckInBet()
		if not isInBet then
			self:InitHandCards()
		end

		--刷新押注分和状态
		if self.reconnectGameInfo then
			for i,v in pairs(self.reconnectGameInfo.players) do
				if next(v) ~= nil then
					local player = self.playerMgr:GetPlayerBySeatID(v.seatId)
					if player and player.seatInfo and v.seatId ~= self.reconnectGameInfo.bankerSeatId then
						if v.score ~= 0 then --有分数 不管什么状态 都显示出分数
							local bteNumTable = {}
							bteNumTable.seatId = v.seatId
							bteNumTable.score = v.score
							--显示押注分 不播放声音
							LuaEvent.AddEvent(EEventType.SetBetNum,bteNumTable,false)
						elseif isInBet then --押注阶段 显示押注中
							LuaEvent.AddEvent(EEventType.BetTipState,v.seatId,true)
						end
					end
				end
			end
		end
	end

end

--检查刷新下注界面
function CThirtyTwoRoom:CheckInBet()
	local isInBet = false
	local showBetBtn = false
	if self.reconnectGameInfo then
		for i,v in pairs(self.reconnectGameInfo.players) do
			if next(v) ~= nil then
				local player = self.playerMgr:GetPlayerBySeatID(v.seatId)
				if player
					and player.seatInfo 
					and not v.hasBet 
					and i - 1 ~= self.reconnectGameInfo.bankerSeatId then   --有玩家未下注
					isInBet = true
					--显示押注按钮
					if player.seatInfo.userId == self:GetSouthUID() then
						LuaEvent.AddEvent(EEventType.ThirtyTwo_ShowBetBtns,true,self.reconnectGameInfo.betCountdown)
						showBetBtn = true
					end
				end
			end
		end
	end

	--显示倒计时
	if isInBet and not showBetBtn then
		LuaEvent.AddEvent(EEventType.PK32ShowCountDown,self.reconnectGameInfo.betCountdown)
	end

	return isInBet
end

--刷新头像
function CThirtyTwoRoom:ResetPlayerStatus()
	for i=1,4 do
		local player = self.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
		if player then
			LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,player)

			-- 服务器可以确定，座位id0必然是庄家
			if i == 1 then
				LuaEvent.AddEvent(EEventType.PlayerShowZhuangJia,player.seatPos,true)
			end
		end
	end
end

--刷新所有玩家的牌状态
function CThirtyTwoRoom:InitHandCards()
	for i,v in pairs(self.reconnectGameInfo.players) do
		if next(v) ~= nil then
			local player = self.playerMgr:GetPlayerBySeatID(v.seatId)
			if player and player.seatInfo then
				--先清理缓存手牌
				player.cardMgr:Clear()
				-- 玩家自己手牌
				self.playerMgr:InitPlayerHandCards(player.seatInfo.userId,v.pai)

				-- 检测是否开牌了
				if v.hasOpen then
					-- 显示牌型
					LuaEvent.AddEvent(EEventType.ShowPaiXing, v,false)
				else
					-- 显示开牌按钮 显示搓牌状态
					if player.seatInfo.userId == self:GetSouthUID() then
						LuaEvent.AddEvent(EEventType.ThirtyTwo_ShowCuoPai,false,v.pai)
						LuaEvent.AddEventNow(EEventType.PK32ShowKaiPaiBtn,self.reconnectGameInfo.openCountdown,true)
					else --显示隐藏牌型
						LuaEvent.AddEvent(EEventType.HidePaiXing, v)
					end
				end
			end
		end
	end
	--显示倒计时
	LuaEvent.AddEventNow(EEventType.PK32ShowCountDown,self.reconnectGameInfo.openCountdown)
	--刷新牌
	LuaEvent.AddEventNow(EEventType.ThirtyTwo_RefreshHandCards)
end

--刷新在线状态
function CThirtyTwoRoom:InitPlayerStatus(player)
	if player then
		local roomStatus = self.roomStateMgr:GetCurStateType()
		if roomStatus == RoomStateEnum.Playing then
			LuaEvent.AddEventNow(EEventType.OnlineRefresh,player.seatPos,player.seatInfo.onlineStatus)
			player:ChangeState(PlayerStateEnum.Playing)
		else
			player:ChangeState(PlayerStateEnum.Init)
		end
	end
end

-- 重写拷贝距离信息数据
function CThirtyTwoRoom:SetPBGeoRsp(PBGeoRsp)
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