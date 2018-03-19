--------------------------------------------------------------------------------
-- 	 File		: ThirtyTwoNormalLogic.lua
--   author		: zx
--   function	: 32张 回放逻辑基类
--   date		: 2018-01-16
--   Copyright	: Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.SystemLogic.BaseRecordPlayCardLogic"
require "Logic.RoomLogic.Room.CThirtyTwoRoom"
require "Logic.RoomLogic.CPlayer"
require "Logic.CardLogic.CCard"

ThirtyTwoRecordLogic = class("ThirtyTwoRecordLogic", BaseRecordPlayCardLogic)

function ThirtyTwoRecordLogic:ctor()
	self:BaseCtor()
end

function ThirtyTwoRecordLogic:Init()
	ProtoManager.InitThirtyTwoProto(Common_PlayID.ThirtyTwo)
	
	self:BaseInit()
	--房间管理
	self.roomObj = CThirtyTwoRoom.New()
	self.roomObj:Init(self)

	self.remain = 32 --剩余牌数
	self.oldRemain = 32 --记录发牌前的牌数
	self.isRegisterUpdate = false
end

function ThirtyTwoRecordLogic:Destroy()
	self:BaseDestroy()

	self.isRegisterUpdate = false

	self.roomObj:Destroy()
	self.roomObj = nil

	self:ClearTask()
end

function ThirtyTwoRecordLogic:GetType()
	return PlayLogicTypeEnum.ThirtyTwo_Record
end

-- 广播消息
function ThirtyTwoRecordLogic:PublicEvent(eventId,rsp)
	DwDebug.Log("ThirtyTwoRecordLogic", "PublicEvent eventId:", eventId, "rsp:", rsp)
	if eventId == GAME_CMD.SC_ROOM_INFO then
		self:RecordRoomInfoPush(rsp)
	elseif eventId == GAME_CMD.SC_RECORD_GAME_INFO_PUSH then
		self:RecordGameInfoPush(rsp)
	elseif eventId == GAME_CMD.SC_USE_BET_PUSH then
		self:RecordUseBet(rsp)
	elseif eventId == GAME_CMD.SC_SYSTEM_DEAL_CARD_PUSH then
		self:RecordSystemDealCard(rsp)
	elseif eventId == GAME_CMD.SC_USE_OPEN_PUSH then
		self:RecordUseOpen(rsp)
	elseif eventId == GAME_CMD.SC_NOTIFY_USE_SHUFFLE then
		self:RecordUseShuffle(rsp)
	elseif eventId == GAME_CMD.SC_SMALL_RESULT_PUSH then
		self:RecordSmallResultPush(rsp)
	end
end

-- 房间信息重置
function ThirtyTwoRecordLogic:CleanRoom()
	-- 清理房间显示
	if nil ~= self.roomObj then
		self.roomObj:CleanRoom()
	end
end

-- 房间信息
function ThirtyTwoRecordLogic:RecordRoomInfoPush(rsp)
	if rsp then
		-- 回放没有离线概念
		for i,v in ipairs(rsp.playerInfo or {}) do
			if v then
				v.onlineStatus = true
			end
		end
		
		self.roomObj:InitConfigBySvr(rsp)
		LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum, rsp.currentGamepos, rsp.totalGameNum)
		--房间信息播放后需要立即播放游戏信息发牌
		self.loopTimeDelta = 3

		if rsp and rsp.playerNum and rsp.playerNum > 0 then
			self.roomObj.playerMgr:SetMaxSize(rsp.playerNum)
			LuaEvent.AddEventNow(EEventType.InitHeadByPlayerNum,rsp.playerNum)
		else
			LuaEvent.AddEventNow(EEventType.InitHeadByPlayerNum,4)
		end
	end
end

-- 游戏信息
function ThirtyTwoRecordLogic:RecordGameInfoPush(rsp)
	self.bankerSeatId = rsp.bankerSeatId
	-- 显示庄家
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.bankerSeatId)
	if player and player.seatPos then
		LuaEvent.AddEvent(EEventType.PlayerShowZhuangJia, player.seatPos, true)
	end
end

-- 下注
function ThirtyTwoRecordLogic:RecordUseBet(rsp)
	if nil == rsp or nil == rsp.seatId or nil == rsp.score then
		DwDebug.LogError("ThirtyTwoRecordLogic", "ThirtyTwoRecordLogic:RecordUseBet Error:", rsp)
		return
	end
	local bteNumTable = {}
	bteNumTable.seatId = rsp.seatId
	bteNumTable.score = rsp.score
	LuaEvent.AddEvent(EEventType.SetBetNum, bteNumTable,true)
end

-- 发牌
function ThirtyTwoRecordLogic:RecordSystemDealCard(rsp)
	if rsp.pos then
		self:ClearTask()

		self.oldRemain = self.remain
		LuaEvent.AddEventNow(EEventType.PK32FanPai, rsp.pos)
		self.UIReady = false
		self.FaPai = TimerTaskSys.AddTimerEventByLeftTime(
			function ()
				LuaEvent.AddEventNow(EEventType.PK32HideFanPai)
				LuaEvent.AddEventNow(EEventType.ThirtyTwo_InitHandCards, LogicUtil.PK32GetInitHandCardsSeatPos(self.bankerSeatId, rsp.pos))
			end, 
			2, nil)

		self.FaPaiEnd = TimerTaskSys.AddTimerEventByLeftTime(
			function ()
				self.UIReady = true
			end, 
			4, nil)
	end

	if nil ~= rsp.remain then
		self.remain = rsp.remain
		-- LuaEvent.AddEventNow(EEventType.ShowRemainCardNum, true, rsp.remain)
	end
end

function ThirtyTwoRecordLogic:ClearTask()
	TimerTaskSys.RemoveTask(self.FaPai)
	TimerTaskSys.RemoveTask(self.FaPaiEnd)
end

-- 开牌
function ThirtyTwoRecordLogic:RecordUseOpen(rsp)
	LuaEvent.AddEvent(EEventType.ThirtyTwo_OpenCards, rsp)
end

-- 洗牌
function ThirtyTwoRecordLogic:RecordUseShuffle(rsp)
	LuaEvent.AddEvent(EEventType.StartShuffleCards)
	-- 牌数重置为32
	self.remain = 32
	LuaEvent.AddEventNow(EEventType.ShowRemainCardNum, true, self.remain)
end

-- 小结算
function ThirtyTwoRecordLogic:RecordSmallResultPush(rsp)
	if rsp then
		self:PausePlay(true)
		self.roomObj.playerMgr:AllPlayerWaiting()
		self.roomObj:AddSmallResult(rsp.now, rsp)
		self.roomObj:ChangeState(RoomStateEnum.Idle)
		self.roomObj:ChangeState(RoomStateEnum.SmallRoundOver)
		LuaEvent.AddEventNow(EEventType.RefreshShowPlayBtnRoot)
	end
end

--获取剩余牌数
function ThirtyTwoRecordLogic:GetRemain(isOld)
	if isOld then
		return self.oldRemain
	else
		return self.remain
	end
end

return ThirtyTwoRecordLogic