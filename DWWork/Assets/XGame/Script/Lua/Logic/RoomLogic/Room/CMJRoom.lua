--------------------------------------------------------------------------------
-- 	 File      : CMJRoom.lua
--   author    : guoliang
--   function   : 麻将房间
--   date      : 2017-11-5
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.Room.CBaseRoom"
require "Logic.RoomLogic.Module.MJModule.MJRoomStateMgrModule"

CMJRoom = class("CMJRoom", CBaseRoom)

function CMJRoom:ctor()
	self.roomStateMgr = MJRoomStateMgrModule.New()-- 房间状态管理
	self:BaseCtor()
end


-- 清理房间显示
function CMJRoom:CleanRoom()

	--清理玩家手牌
	LuaEvent.AddEventNow(EEventType.MJClearHandCards,nil,nil)
	LuaEvent.AddEventNow(EEventType.MJRecordClearHandCards,nil,nil)
	LuaEvent.AddEventNow(EEventType.MJ_ClearAllOutCards)

	-- 清理动画
	LuaEvent.AddEventNow(EEventType.MJClearAnimation)

	--如果是在游戏中，需要先重置玩家的头像
	for i=1,4 do
		local player = self.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
		if player then
			LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,player)
		end
	end
end


--初始化所有的游戏信息
function CMJRoom:RefreshGameInfo(gameInfo)
	self:BaseRefreshGameInfo(gameInfo)

	local roomStatus = self.roomStateMgr:GetCurStateType()
	if roomStatus == RoomStateEnum.Playing or roomStatus == RoomStateEnum.Idle then
		self:InitHandCards()
	end

end

-- 开始游戏初始化房间
function CMJRoom:InitGameInfo(gameInfo)

end

function CMJRoom:InitHandCards()
	--派牌刷新
	for i,v in ipairs(self.reconnectGameInfo.playerCard) do
		-- 如果有碰牌什么的，发消息显示
		if v.actionNotice then
			LuaEvent.AddEventNow(EEventType.MJOpeTip, v.actionNotice)
		end
		if v.paiPai and (v.paiPai > 0 or v.paiPai == -1) then
			local player = self.playerMgr:GetPlayerByPlayerID(v.userId)
			local card_item = CMJCard.New()
			card_item:Init(v.paiPai, 1)
			local cardList = {card_item}
			player.cardMgr:AddCards(cardList)
			-- 通知 手牌控件 更新显示
			LuaEvent.AddEventNow(EEventType.MJInCard, player, card_item)
		end
	end

	-- 初始化各个玩家打出的牌和自己吃碰杠牌
	local player
	for k,v in pairs(self.reconnectGameInfo.playerCard) do
		player = self.playerMgr:GetPlayerByPlayerID(v.userId)
		if player then
			for k1,v1 in pairs(v.outputInfo) do
				LuaEvent.AddEventNow(EEventType.MJ_PlayNormalOutMJCardShow, player.seatPos, {v1, false,false})
			end

			for k1,v1 in pairs(v.opeList) do
				if v1.opeId == 1 then
					--牌展示
					local provider = self.playerMgr:GetPlayerByPlayerID(v1.chuPai.chiWithPlayerId)
					LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos} ,v1.chuPai.pai)
					--吃谁的

				elseif v1.opeId == 2 then
					--牌展示
					local provider = self.playerMgr:GetPlayerByPlayerID(v1.pengPai.pengWithPlayerId)
					LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos},v1.pengPai.pai)
					--碰谁的

				elseif v1.opeId == 3 then
					--牌展示
					local provider = self.playerMgr:GetPlayerByPlayerID(v1.gangPai.gangWithPlayerId)

					if v1.gangPai.gangWithPlayerId == v.userId then
						LuaEvent.AddEventNow(EEventType.MJ_PlayOwnSupplyGangShow,player.seatPos,v1.gangPai.pai)
					else
						LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,provider.seatPos},v1.gangPai.pai)
					end

					--杠谁的

				end
			end
		end

		-- 吃碰胡通知

	end
end

function CMJRoom:InitPlayerStatus(player)
	-- 玩家上下线 会推送信息 需要 更新玩家状态
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

-- 座位-玩家映射
function CMJRoom:SeatTranslate(seatinfo)
	if self.roomInfo then
		local seatDistance = seatinfo.seatId - self.selfSeatInfo.seatId
		if seatDistance == 1 or seatDistance == -3 then
			self.seatConfig[seatinfo.userId] = SeatPosEnum.East
			self.seatConfig[SeatPosEnum.East] = seatinfo.userId
		elseif math.abs(seatDistance) == 2 then
			self.seatConfig[seatinfo.userId] = SeatPosEnum.North
			self.seatConfig[SeatPosEnum.North] = seatinfo.userId
		elseif seatDistance == 3 or seatDistance == -1 then
			self.seatConfig[seatinfo.userId] = SeatPosEnum.West
			self.seatConfig[SeatPosEnum.West] = seatinfo.userId
		end
	end
end

-- 重写拷贝距离信息数据
function CMJRoom:SetPBGeoRsp(PBGeoRsp)
	if PBGeoRsp ~= nil then
		self.PBGeoRsp = {}
		local distance = {}
		for key, PBGeoItem in pairs(PBGeoRsp.distance) do
			if next(PBGeoItem) ~= nil then
				local data = {}
				data.seatIdA = PBGeoItem.seatIdA
				data.seatIdB = PBGeoItem.seatIdB
				data.distance = PBGeoItem.distance
				distance[key] = data
			end
		end
		self.PBGeoRsp.distance = distance
	end
end