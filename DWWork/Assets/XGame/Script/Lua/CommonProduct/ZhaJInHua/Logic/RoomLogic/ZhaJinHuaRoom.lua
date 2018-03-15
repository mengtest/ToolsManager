--------------------------------------------------------------------------------
--   File      : ZhaJinHuaRoom.lua
--   author    : jianing
--   function  : 扎金花张房间
--   date      : 2018-02-01
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.Room.CBaseRoom"
require "CommonProduct.ZhaJInHua.Logic.RoomLogic.Module.ZhaJinHuaRoomStateMgrModule"

ZhaJinHuaRoom = class("ZhaJinHuaRoom", CBaseRoom)

function ZhaJinHuaRoom:ctor()
	self.roomStateMgr = ZhaJinHuaRoomStateMgrModule.New()-- 房间状态管理
	self:BaseCtor()
end

-- 清理房间显示
function ZhaJinHuaRoom:CleanRoom()
	self:ResetPlayerStatus()
end

--断线重连回来刷新所有的游戏信息
function ZhaJinHuaRoom:RefreshGameInfo(gameInfo)
	self.reconnectGameInfo = gameInfo
	local roomStatus = self.roomStateMgr:GetCurStateType()
	if roomStatus == RoomStateEnum.Playing then -- 房间游戏中
		
	end
end

--刷新头像
function ZhaJinHuaRoom:ResetPlayerStatus()
	for i=1,6 do
		local player = self.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
		if player then
			LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,player)

			if self:CheckUserIDIsRoomOwner(player.seatInfo.userId) then
				LuaEvent.AddEvent(EEventType.PlayerShowZhuangJia,player.seatPos,true)
			end
		end
	end
end

--刷新所有玩家的牌状态
function ZhaJinHuaRoom:InitHandCards()
	for i,v in pairs(self.reconnectGameInfo.players) do
		if next(v) ~= nil then
			local player = self.playerMgr:GetPlayerBySeatID(v.seatId)
			if player and player.seatInfo then
				--先清理缓存手牌
				player.cardMgr:Clear()
				-- 玩家自己手牌
				self.playerMgr:InitPlayerHandCards(player.seatInfo.userId,v.pai)
			end
		end
	end
	--刷新牌
	LuaEvent.AddEventNow(EEventType.ThirtyTwo_RefreshHandCards)
end

--刷新在线状态
function ZhaJinHuaRoom:InitPlayerStatus(player)
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
function ZhaJinHuaRoom:SetPBGeoRsp(PBGeoRsp)
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

-- 座位-玩家映射
function ZhaJinHuaRoom:SeatTranslate(seatinfo)
	local seatDistance = seatinfo.seatId - self.selfSeatInfo.seatId
	local SeatPos = nil
	if seatDistance > 0 then
		SeatPos = SeatPosIndex[seatDistance + 1]
	else
		SeatPos = SeatPosIndex[#SeatPosIndex + seatDistance + 1]
	end
	if SeatPos then
		self.seatConfig[seatinfo.userId] = SeatPos
		self.seatConfig[SeatPos] = seatinfo.userId
	end
end