--------------------------------------------------------------------------------
-- 	 File      : WSKRecordPlayCardLogic.lua
--   author    : guoliang
--   function   : 510k游戏回放逻辑 
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.BaseRecordPlayCardLogic"
require "Logic.CardLogic.CardTypeEnum"
require "Logic.CardLogic.CCard"
require "Logic.CardLogic.LogicContainer.WSK.CRWSKCardPlayLogicContainer"
require "Logic.RoomLogic.CPlayer"
require "Logic.RoomLogic.Room.CWSKRoom"
require "LuaSys.AnimManager"

WSKRecordPlayCardLogic = class("WSKRecordPlayCardLogic", BaseRecordPlayCardLogic)
function WSKRecordPlayCardLogic:ctor()
	self:BaseCtor()
end

function WSKRecordPlayCardLogic:Init()
	--初始化玩法所需PB
	--ProtoManager.InitWSKProto(Common_PlayID.chongRen_510K)
	self:BaseInit()
	self.playInterval = 0.5

	--房间管理
	self.roomObj = CWSKRoom.New()
	self.roomObj:Init(self)
	-- 玩牌逻辑容器
	self.cardLogicContainer = CRWSKCardPlayLogicContainer.New()
	self.cardLogicContainer:Init()

	self.isRegisterUpdate = false
end


function WSKRecordPlayCardLogic:Destroy()
	self:BaseDestroy()

	self.roomObj:Destroy()
	self.isRegisterUpdate = false
	self.roomObj = nil
	self.cardLogicContainer:Destroy()
	self.cardLogicContainer = nil
end

function WSKRecordPlayCardLogic:GetType()
	return PlayLogicTypeEnum.WSK_Record
end

-------------------------------------------基础播放功能--------------------------------------------------------


-- 查找一局开始的消息编号
function WSKRecordPlayCardLogic:FindRoundStartIndex(targetRound)
	for k,v in pairs(self.recordBody.cells) do
		if v.eventId == GAME_CMD.SC_ROOM_INFO then
			local eventId,roomInfo = self:DecordRecordItem(k)
			if roomInfo and roomInfo.currentGamepos == targetRound then
				return k
			end
		end
	end
end

-- 广播消息
function WSKRecordPlayCardLogic:PublicEvent(eventId,rsp)
	if eventId == GAME_CMD.SC_ROOM_INFO then
		self:RecordRoomInfoPush(rsp)
	elseif eventId == GAME_CMD.SC_RECORD_GAME_INFO_PUSH then
		self:RecordGameInfoPush(rsp)
	elseif eventId == GAME_CMD.SC_SVR_PLAY_CARD_PUSH then
		self:RecordPlayCardPush(rsp)
	elseif eventId == GAME_CMD.SC_CLEAR_CARDS_PUSH then
		self:RecordClearCardsPush(rsp)
	elseif eventId == GAME_CMD.SC_SMALL_RESULT_PUSH then
		self:RecordSmallResultPush(rsp)
	end
end


function WSKRecordPlayCardLogic:CleanRoom()
	-- 清理房间显示
	self.roomObj:CleanRoom()

	--清理玩家手牌
	LuaEvent.AddEventNow(EEventType.RecordHandCardClear,nil,nil)

	--如果是在游戏中，需要先重置玩家的头像
	for i=1,4 do
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
		if player then
			LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,player)
		end
	end
end


--回放房间信息
function WSKRecordPlayCardLogic:RecordRoomInfoPush(rsp)
	if rsp then
		-- 回放没有离线概念
		for i,v in ipairs(rsp.playerInfo or {}) do
			if v then
				v.onlineStatus = true
			end
		end
		self.roomInfo = rsp
		self.roomObj:InitConfigBySvr(rsp)
		LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum,rsp.currentGamepos,rsp.totalGameNum)
		--房间信息播放后需要立即播放游戏信息发牌
		self.loopTimeDelta = 3 
	end  
end
-- 回放游戏信息推送
function WSKRecordPlayCardLogic:RecordGameInfoPush(rsp)
	self.gameInfo = rsp
	local gameInfo = rsp
	if gameInfo then
		local seatPos = 0
		local player
		-- 初始化各个玩家手牌
		for k,v in pairs(gameInfo.handPai) do
			seatPos = k -1
			player = self.roomObj.playerMgr:GetPlayerBySeatID(seatPos)
			if player then
				--保存手牌
				self.roomObj.playerMgr:InitPlayerHandCards(player.seatInfo.userId,v.pai)
			end
		end

		-- 是否打独
		if gameInfo.isAlone then
			local alonePlayer = self.roomObj.playerMgr:GetPlayerBySeatID(gameInfo.bankerSeatId)
			AnimManager.PlayPlayerHeadAnim(true, alonePlayer.seatPos)
			LuaEvent.AddEventNow(EEventType.PlayerShowAlone,alonePlayer.seatPos,true)
		else
			local bankerPlayer = self.roomObj.playerMgr:GetPlayerBySeatID(gameInfo.bankerSeatId)
			AnimManager.PlayPlayerHeadAnim(true, bankerPlayer.seatPos)
			LuaEvent.AddEventNow(EEventType.PlayerShowZhuangJia, bankerPlayer.seatPos, true)
			-- 朋友亮牌显示
			LuaEvent.AddEventNow(EEventType.PlayerShowFriendCard, gameInfo.friendPai, true)
		end
		
		--开始发牌
		LuaEvent.AddEventNow(EEventType.RecordInitHandCards,true,nil)
		--房间进入游戏状态
		self.roomObj:ChangeState(RoomStateEnum.Playing)

	end  
end

-- 玩家出牌推送
function WSKRecordPlayCardLogic:RecordPlayCardPush(rsp)
	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		self.roomObj.playerMgr:PlayerWaiting(rsp.userId)
		if not rsp.isSkip then
			-- 添加到一轮出牌队列
			self.cardLogicContainer:AddOneOutCards(rsp)
			-- 更新出牌者炸弹赔率
			if rsp.bombOdds > 0 then
				self.roomObj.pointMgr:AddPlayBombNum(player.seatPos,rsp.bombOdds, true)
			end
			-- 桌面积分刷新
			if self.roomObj.curDeskScore ~=  rsp.desktopScore then
				self.roomObj.curDeskScore = rsp.desktopScore
				LuaEvent.AddEventNow(EEventType.RefreshCurTablePoint,rsp.desktopScore)
			end

			-- 朋友关系显示处理 
			if rsp.containFriendPai then
				LuaEvent.AddEvent(EEventType.PlayerShowFriend,SeatPosEnum.South,true)
				local friendSeatId = rsp.friendShip[self.roomObj.selfSeatInfo.seatId+1]--tabel从1开始，座位从0开始
				local friendPlayer = self.roomObj.playerMgr:GetPlayerBySeatID(friendSeatId)
				if friendPlayer then
					LuaEvent.AddEvent(EEventType.PlayerShowFriend,friendPlayer.seatPos,true)
				end
			end

			-- 几游显示
			if rsp.overOrder > 0 then
				if player then
					LuaEvent.AddEvent(EEventType.PlayerShowSort,player.seatPos,rsp.overOrder)
					-- 已经出完，需要清理显示的手牌数
					LuaEvent.AddEvent(EEventType.PlayerShowRemainHandCards,player.seatPos,0)
					player:ChangeState(PlayerStateEnum.Finished)
				end
			end

			-- 剩余手牌
			if rsp.restNum > 0 then
				if player then
					LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards,player.seatPos,rsp.restNum)
				end
			end
			AudioManager.PlayCommonSound(UIAudioEnum.chuPai)
			AudioManager.WSK_PlayBaoPai(rsp)
			AnimManager.PlayCardClassAnim(rsp, PlayGameSys.GetNowPlayId())
		else
			--不出音效
			self:WSK_PlayCaoZuoSound(rsp.seatId,WSK_CaoZuoEnum.buChu)
		end
		--消息通知（玩家手牌消失，出牌展示，桌面积分刷新）
		LuaEvent.AddEventNow(EEventType.PlayOutCardAction,rsp,false)

		-- 让下家提前进入游戏状态
		local nextSeatID = rsp.seatId + 1
		if nextSeatID > 3 then
			nextSeatID = 0
		end
		local nextPlayer = self.roomObj.playerMgr:GetPlayerBySeatID(nextSeatID)
		if nextPlayer and nextPlayer.playStateMgr:GetType() ~= PlayerStateEnum.Finished then
			nextPlayer:ChangeState(PlayerStateEnum.Playing)
			self.roomObj:SaveWorkingPlayerID(nextPlayer.seatInfo.userId)
		end
	end
end

--清牌推送（一轮打完后，由服务端清理桌面牌，结算分数）
function WSKRecordPlayCardLogic:RecordClearCardsPush(rsp,head)
	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		--消息通知(本轮手牌积分结算)
		self.roomObj.pointMgr:AddPlayPoint(player.seatPos,rsp.desktopScore, true)
		--清除桌面积分显示
		self.roomObj.curDeskScore = 0
		LuaEvent.AddEventNow(EEventType.RefreshCurTablePoint,self.roomObj.curDeskScore)

		--一轮结束重置一轮打牌模块
		LuaEvent.AddEventNow(EEventType.RoundEndClear,nil,nil)
		--清桌
		LuaEvent.AddEventNow(EEventType.ClearDesk,nil,nil)
	end
end


--小局结算推送
function WSKRecordPlayCardLogic:RecordSmallResultPush(rsp,head)
	if rsp then
		self:PausePlay(true)
		self.roomObj.playerMgr:AllPlayerWaiting()
		self.roomObj:AddSmallResult(rsp.now,rsp)
		self.roomObj:ChangeState(RoomStateEnum.SmallRoundOver)
	end
end

function WSKRecordPlayCardLogic:WSK_PlayCaoZuoSound(seatId,caoZuoEnum)
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(seatId)
	if player then
		AudioManager.WSK_PlayCaoZuo(player.seatInfo.sex,caoZuoEnum)
	end
end