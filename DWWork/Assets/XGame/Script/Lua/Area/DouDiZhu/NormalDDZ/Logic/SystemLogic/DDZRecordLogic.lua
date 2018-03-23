--------------------------------------------------------------------------------
-- 	 File       : DDZRecordLogic.lua
--   author     : zhanghaochun
--   function   : 斗地主 回放逻辑基类
--   date       : 2018-01-26
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.BaseRecordPlayCardLogic"
require "CommonProduct.CommonBase.NewCardTypeEnum"
require "Area.DouDiZhu.Base.Logic.Room.DDZRoom"
require "Logic.RoomLogic.CPlayer"
require "Logic.CardLogic.CCard"
require "LuaSys.AnimManager"

local DDZContainer = require "Area.DouDiZhu.NormalDDZ.Container.NormalDiZhuContainer"

DDZRecordLogic = class("DDZRecordLogic", BaseRecordPlayCardLogic)

function DDZRecordLogic:ctor()
	self:BaseCtor()
end

function DDZRecordLogic:Init()
	self:BaseInit()
	--房间管理
	self.roomObj = DDZRoom.New()
	self.roomObj:Init(self)
	-- 设置房间最大人数(斗地主特殊)
	self.roomObj.playerMgr:SetMaxSize(3)

	-- 玩牌逻辑容器
	self.cardLogicContainer = DDZContainer.New()

	self.isRegisterUpdate = false

	-- 地主座位号
	self.bankerSeatId = -1 -- 地主座位号
	-- 我的倍数信息
	self.MyPBMultiple = {}
	self:ResetPBMultiple()
	LuaEvent.AddEvent(EEventType.DDZMultiplePBInfo, self.MyPBMultiple)
end

-----------------------------------对外接口--------------------------------------
function DDZRecordLogic:GetType()
	return PlayLogicTypeEnum.DDZ_Normal_Record
end

-- 广播消息
function DDZRecordLogic:PublicEvent(eventId,rsp)
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

function DDZRecordLogic:CleanRoom()
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

function DDZRecordLogic:Destroy()
	self:BaseDestroy()

	self.isRegisterUpdate = false

	self.roomObj:Destroy()
	self.roomObj = nil
	
	self.cardLogicContainer:Destroy()
	self.roomObj = nil
end
--------------------------------------------------------------------------------

--回放房间信息
function DDZRecordLogic:RecordRoomInfoPush(rsp)
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
function DDZRecordLogic:RecordGameInfoPush(rsp)
	self.gameInfo = rsp
	local gameInfo = rsp
	if gameInfo then
		-- 记录地主座位号
		self.bankerSeatId = gameInfo.bankerSeatId
		local seatPos = 0
		local player
		-- 初始化各个玩家手牌
		for k,v in pairs(gameInfo.handPai) do
			seatPos = k -1
			player = self.roomObj.playerMgr:GetPlayerBySeatID(seatPos)
			if player then
				--保存手牌
				self.roomObj.playerMgr:InitPlayerHandCards(player.seatInfo.userId,v.pai)
				LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards, player.seatPos,#v.pai)
			end
		end

		-- 广播地主
		for i = 1, 4 do
			local seat = i - 1
			local player = self.roomObj.playerMgr:GetPlayerBySeatID(seat)
			if player then
				if self.bankerSeatId == seat then
					LuaEvent.AddEvent(EEventType.DDZLordFind, player.seatPos, true)
				else
					LuaEvent.AddEvent(EEventType.DDZLordFind, player.seatPos, false)
				end
			end
		end
		-- 广播底牌
		local baseCardsRsp = self:ToPBWildCards(gameInfo)
		LuaEvent.AddEvent(EEventType.DDZShowBaseCards, rsp)
		
		--开始发牌
		LuaEvent.AddEventNow(EEventType.RecordInitHandCards,true,nil)
		--房间进入游戏状态
		self.roomObj:ChangeState(RoomStateEnum.Playing)
	end  
end

-- 玩家出牌推送
function DDZRecordLogic:RecordPlayCardPush(rsp)
	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		self.roomObj.playerMgr:PlayerWaiting(rsp.userId)
		if not rsp.isSkip then
			-- 添加到一轮出牌队列
			self.cardLogicContainer:AddOneOutCards(rsp)
			-- 剩余手牌
			if rsp.restNum >= 0 then
				if player then
					LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards,player.seatPos,rsp.restNum)
					if not player.playRestNumSound then
						player.playRestNumSound = true
						--self:WSK_PlayCaoZuoSound(rsp.seatId,WSK_CaoZuoEnum.woPaiBuDuoLe)
					end
					
					AudioManager.DDZ_PlayCardNum(player.seatInfo.sex, rsp.restNum)
				end
			end
			AudioManager.DDZ_PlayCardInTableByRsp(rsp)
			AudioManager.DDZ_PlayBaoPai(rsp)
			AnimManager.DDZPlayAnimByPaiClass(rsp, PlayGameSys.GetNowPlayId())
		else
			AudioManager.DDZ_PlayBaoPai(rsp)
			AnimManager.DDZPlayAnimByPaiClass(rsp, PlayGameSys.GetNowPlayId())
		end
		--消息通知（玩家手牌消失，出牌展示，桌面积分刷新）
		LuaEvent.AddEventNow(EEventType.PlayOutCardAction,rsp,false)

		local paiClass = rsp.paiClass
		if paiClass == 10 or paiClass == 34 then
			if not self.MyPBMultiple.isBombLimit then
				LuaEvent.AddEvent(EEventType.DDZShowDouble)
			end
		end

		-- 缓存自己的倍数描述信息
		local myPlayer = self.roomObj.playerMgr:GetPlayerByPlayerID(self.roomObj:GetSouthUID())
		if myPlayer and myPlayer.seatInfo then
			local multipleInfo = rsp.multiples[myPlayer.seatInfo.seatId + 1]
			if multipleInfo ~= nil then
				self.MyPBMultiple = multipleInfo
				LuaEvent.AddEvent(EEventType.DDZMultiplePBInfo, self.MyPBMultiple)
			end
		end
	end
end

--清牌推送（一轮打完后，由服务端清理桌面牌，结算分数）
function DDZRecordLogic:RecordClearCardsPush(rsp, head)
	if rsp then
		--一轮结束重置一轮打牌模块
		LuaEvent.AddEventNow(EEventType.RoundEndClear,nil,nil)
		--清桌
		LuaEvent.AddEventNow(EEventType.ClearDesk,nil,nil)
	end
end

--小局结算推送
function DDZRecordLogic:RecordSmallResultPush(rsp,head)
	if rsp then
		self:PausePlay(true)
		self.roomObj.playerMgr:AllPlayerWaiting()
		self.roomObj:AddSmallResult(rsp.now,rsp)
		self.roomObj:ChangeState(RoomStateEnum.SmallRoundOver)
	end
end

--------------------------------------------------------------------------------
-- 广播的底牌信息(gameInfo是回放信息)
function DDZRecordLogic:ToPBWildCards(gameInfo)
	local rsp = {}
	rsp.bankerUserId = gameInfo.bankerUserId
	rsp.bankerSeatId = gameInfo.bankerSeatId
	rsp.baseScore = gameInfo.baseScore
	rsp.pai = gameInfo.wildCards

	return rsp
end

function DDZRecordLogic:ResetPBMultiple()
	if self.MyPBMultiple == nil then
		self.MyPBMultiple = {}
	end

	self.MyPBMultiple.base = 0
	self.MyPBMultiple.open = 0
	self.MyPBMultiple.spring = 0
	self.MyPBMultiple.antiSpring = 0
	self.MyPBMultiple.bomb = 0
	self.MyPBMultiple.isBombLimit = false
	self.MyPBMultiple.common = 0
	self.MyPBMultiple.plus = 0
	self.MyPBMultiple.back = 0
	self.MyPBMultiple.sum = 0
end

return DDZRecordLogic