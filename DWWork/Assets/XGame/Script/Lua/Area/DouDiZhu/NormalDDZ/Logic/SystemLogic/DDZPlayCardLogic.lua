--------------------------------------------------------------------------------
-- 	 File       : DDZPlayCardLogic.lua
--   author     : zhanghaochun
--   function   : 正常玩法逻辑基类
--   date       : 2018-01-26
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

require "Logic.DWBaseModule"
require "Logic.RoomLogic.BaseObjectState"
require "CommonProduct.CommonBase.NewCardTypeEnum"
require "Logic.CardLogic.CCard"
require "Logic.RoomLogic.CPlayer"
require "Area.DouDiZhu.Base.Logic.Room.DDZRoom"
require "Logic.SystemLogic.BasePlayCardLogic"
require	"LuaSys.AnimManager"

local DDZContainer = require "Area.DouDiZhu.NormalDDZ.Container.NormalDiZhuContainer"

DDZPlayCardLogic = class("DDZPlayCardLogic", BasePlayCardLogic)

function DDZPlayCardLogic:ctor()
	self:BaseCtor()
	
	--是否发牌动画中
	self.isPlayDealCard = false
	--是否摊牌中
	self.isPlayTaiPaiAction = false
end

-------------------------------------对外接口------------------------------------
function DDZPlayCardLogic:Init()
	self:BaseInit()

	-- 房间管理
	self.roomObj = DDZRoom.New()
	self.roomObj:Init(self)
	-- 设置房间最大人数(斗地主特殊)
	self.roomObj.playerMgr:SetMaxSize(3)
	
	-- 玩牌逻辑容器
	self.cardLogicContainer = DDZContainer.New()
	--self.cardLogicContainer:Init()

	self.bankerSeatId = -1 -- 地主座位号
	-- 农民不加倍的数量
	self.farmerNoPlusNum = 0

	-- 我的倍数信息
	self.MyPBMultiple = {}
	self:ResetPBMultiple()
end

function DDZPlayCardLogic:GetType()
	return PlayLogicTypeEnum.DDZ_Normal
end

-- 注册事件
function DDZPlayCardLogic:RegisterEvent()
	LuaEvent.AddHandle(EEventType.Net_Common_Errno, self.DealCommonErrorEvent, self)
end

-- 取消注册事件
function DDZPlayCardLogic:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.Net_Common_Errno, self.DealCommonErrorEvent, self)
end

-- 注册网络消息推送
function DDZPlayCardLogic:RegisteNetPush()
	LuaNetWork.RegisterHandle(GAME_CMD.CS_PREPARE, self.HandlePreparePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_PLAYER_ONLINE_PUSH, self.HandlePlayerOnlinePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SEAT_INFO_PUSH, self.HandleSeatInfoPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ROOM_INFO, self.HandleRoomInfoPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_GAME_INFO_PUSH, self.HandleGameInfoPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_VOTE_PUSH, self.HandleNotifyDismissVotePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_DISMISS_RESULT_PUSH, self.HandleNotifyDismissResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_DISMISS, self.HandleRoomDismissedPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_QUIT_ROOM_PUSH, self.HandleQuitRoomPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_KICK_PUSH, self.HandleNotifyKickPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SYSTEM_DEAL_CARD_PUSH, self.HandleSystemDealCardPush, self, 4)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SMALL_RESULT_PUSH, self.HandleSmallResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_BIG_RESULT_PUSH, self.HandleBigResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ASK_LBS_REPLY, self.HandleLBSNotify, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_HISTORY_SCORE_PUSH, self.HandleHistoryScorePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_DISTANCE_TIPS_PUSH, self.HandleDistanceTipsPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ASK_KICK_OUT_REPLY, self.HandleASKKickOutReplay, self)

	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_PLAY_CARD_PUSH, self.HandleNotifyPlayCardPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SVR_PLAY_CARD_PUSH, self.HandleSvrPlayCardPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_CLEAR_CARDS_PUSH, self.HandleClearCardsPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SHOW_ALL_CARDS_PUSH, self.HandleShowAllCardsPush, self)

	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_IS_WANT_LORD_PUSH, self.HandleIsWantLordPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_LORD_PUSH, self.HandleLordPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_BOTTOM_CARDS_PUSH, self.HandleBottomCardsPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_IS_OPEN_PLAY_PUSH, self.HandleNotifyIsOpenPlayPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_LORD_OPEN_PLAY_PUSH, self.HandleLordOpenPlayPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_FARMER_ADD_TIMES_PUSH, self.HandleNotifyFarmerAddTimesPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_FARMER_ADD_TIMES_PUSH, self.HandleFarmerAddTimesPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_LORD_SUB_TIMES_PUSH, self.HandleNotifyLordSubTimesPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_LORD_SUB_TIMES, self.HandleLordSubTimesPush, self)
end

-- 取消注册网络消息推送
function DDZPlayCardLogic:UnRegisteNetPush()
	LuaNetWork.UnRegisterHandle(GAME_CMD.CS_PREPARE, self.HandlePreparePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_PLAYER_ONLINE_PUSH, self.HandlePlayerOnlinePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SEAT_INFO_PUSH, self.HandleSeatInfoPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_ROOM_INFO, self.HandleRoomInfoPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_GAME_INFO_PUSH, self.HandleGameInfoPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_VOTE_PUSH, self.HandleNotifyDismissVotePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_DISMISS_RESULT_PUSH, self.HandleNotifyDismissResultPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_DISMISS, self.HandleRoomDismissedPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_QUIT_ROOM_PUSH, self.HandleQuitRoomPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_KICK_PUSH, self.HandleNotifyKickPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SYSTEM_DEAL_CARD_PUSH, self.HandleSystemDealCardPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SMALL_RESULT_PUSH, self.HandleSmallResultPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_BIG_RESULT_PUSH, self.HandleBigResultPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_ASK_LBS_REPLY, self.HandleLBSNotify, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_HISTORY_SCORE_PUSH, self.HandleHistoryScorePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_DISTANCE_TIPS_PUSH, self.HandleDistanceTipsPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_ASK_KICK_OUT_REPLY, self.HandleASKKickOutReplay, self)

	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_PLAY_CARD_PUSH, self.HandleNotifyPlayCardPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SVR_PLAY_CARD_PUSH, self.HandleSvrPlayCardPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_CLEAR_CARDS_PUSH, self.HandleClearCardsPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SHOW_ALL_CARDS_PUSH, self.HandleShowAllCardsPush, self)

	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_IS_WANT_LORD_PUSH, self.HandleIsWantLordPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_LORD_PUSH, self.HandleLordPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_BOTTOM_CARDS_PUSH, self.HandleBottomCardsPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_IS_OPEN_PLAY_PUSH, self.HandleNotifyIsOpenPlayPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_LORD_OPEN_PLAY_PUSH, self.HandleLordOpenPlayPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_FARMER_ADD_TIMES_PUSH, self.HandleNotifyFarmerAddTimesPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_FARMER_ADD_TIMES_PUSH, self.HandleFarmerAddTimesPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_LORD_SUB_TIMES_PUSH, self.HandleNotifyLordSubTimesPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_LORD_SUB_TIMES, self.HandleLordSubTimesPush, self)
end

function DDZPlayCardLogic:Destroy()
	self:BaseDestroy()
	self.roomObj:Destroy()
	self.roomObj = nil
end
--------------------------------------------------------------------------------

-------------------------------------事件函数------------------------------------
function DDZPlayCardLogic:DealCommonErrorEvent(evendID, p1, p2)
	if p1 and p2 then
		if p2 == 10513 or p2 == 10516 then
	    	PlayGameSys.QuitToMainCity()
		--elseif p2 == 10540 or p2 == 10541 
			--or 10543 or p2 == 10544 or p2 == 10545 
			--or p2 == 10555 then
			--WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
		elseif p2 >= 10540 and p2 <= 10556 then
			WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
		end
	end
end
--------------------------------------------------------------------------------

------------------------------------网络消息推送----------------------------------
--房间信息推送
function DDZPlayCardLogic:HandleRoomInfoPush(rsp, head)
	if rsp then
		if rsp.status ~= 4 then 
		    --小局结算要清桌 不然会出现没翻牌的情况 提前到这
		    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "disband_room_ui_window", true , nil)
    	end
		
		self.super.super.HandleRoomInfoPush(self,rsp,head)

		if rsp.status == 0 then
			-- 清理手牌
			LuaEvent.AddEvent(EEventType.SelfHandCardClear,nil,nil)
			if self.bankerSeatId ~= -1 then
				-- 清牌清桌
				self.roomObj:CleanRoom()
				-- 取消显示地主明的牌
				LuaEvent.AddEvent(EEventType.DDZCloseLordOpenPlayCard)
				-- 倍数信息更新
				self:ResetPBMultiple()
				LuaEvent.AddEvent(EEventType.DDZMultiplePBInfo, self.MyPBMultiple)
				-- 重置头像
				for i=1,4 do
					local player = self.roomObj.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
					if player then
						player.playRestNumSound = false
						LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,player)
					end
				end
			end
		end
	end
end

-- 游戏信息推送
function DDZPlayCardLogic:HandleGameInfoPush(rsp,head)
	self:BaseHandleGameInfoPush(rsp,head)
	
	self.isPlayDealCard = false
	self.isPlayTaiPaiAction = false
	self.farmerNoPlusNum = 0
end

-- 系统发牌推送(只有自己的)
function DDZPlayCardLogic:HandleSystemDealCardPush(rsp, head)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end

	-- 清牌清桌
	self.roomObj:CleanRoom()
	-- 取消抢地主的显示
	LuaEvent.AddEvent(EEventType.DDZLordFind, SeatPosEnum.South, false)
	-- 取消显示地主明的牌
	LuaEvent.AddEvent(EEventType.DDZCloseLordOpenPlayCard)
	-- 倍数信息更新
	self:ResetPBMultiple()
	LuaEvent.AddEvent(EEventType.DDZMultiplePBInfo, self.MyPBMultiple)
	-- 重置头像
	for i=1,4 do
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(i-1)--座位号从0开始的
		if player then
			player.playRestNumSound = false
			LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,player)
		end
	end
	--保存手牌
	self.roomObj.playerMgr:InitPlayerHandCards(DataManager.GetUserID(),rsp.pai)
	--房间进入游戏状态
	self.roomObj:ChangeState(RoomStateEnum.Playing)
	-- 玩家手牌初始化展示动画
	self.isPlayDealCard = true
	LuaEvent.AddEventNow(EEventType.SelfHandCardInit,true,nil)
	LuaEvent.AddEventNow(EEventType.ShowCardDealCardAction,true)
	--清理工作玩家
	self.roomObj:SaveWorkingPlayerID(-1)

	--开局前检查人数是否合法
	if self.roomObj.playerMgr:CheckPlayerNumIsLegal(3) == false then
		error("cur player num is not enough when start game")
		WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
	end

	-- 隐藏底牌显示
	LuaEvent.AddEvent(EEventType.DDZShowBaseCards, nil, 1)
	-- 游戏已经开始
	self.gameStarted = true
end

-- 提示抢地主
function DDZPlayCardLogic:HandleIsWantLordPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		if self.isPlayDealCard then
			local taskID = DelayTaskSys.AddTask(self.NotifyWantLordFunc, self, rsp)
			self:AddDelayTask("NotifyWantLordFunc", taskID, 2.3)
		else
			self:NotifyWantLordFunc(rsp)
		end
	end
end

-- 广播地主
function DDZPlayCardLogic:HandleLordPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		-- 广播玩家抢地主的分数
		LuaEvent.AddEvent(EEventType.DDZWantLordScoreEvent, rsp)

		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		-- 报分
		if player then
			local score = 0
			if rsp.isRob then
				score = rsp.score
			end
			AudioManager.DDZ_PlayBaoFen(player.seatInfo.sex, score)
			self.roomObj.playerMgr:PlayerWaiting(player.seatInfo.userId)
		end

		-- 清空当前工作的玩家ID
		self.roomObj:SaveWorkingPlayerID(-1)

		if rsp.isRob and rsp.score == 3 then
			-- 三分必是地主
			self.bankerSeatId = rsp.seatId
			if player then
				LuaEvent.AddEvent(EEventType.DDZLordFind, player.seatPos, true)
			end
		end
		self:DealMultiplePBs(rsp.multiples)

		self.roomObj:ChangeState(RoomStateEnum.Playing)		
	end
end

-- 广播底牌
function DDZPlayCardLogic:HandleBottomCardsPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		-- 清空当前工作的玩家ID
		self.roomObj:SaveWorkingPlayerID(-1)
		self.bankerSeatId = rsp.bankerSeatId
		-- 广播地主
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(self.bankerSeatId)
		if player then
			LuaEvent.AddEvent(EEventType.DDZLordFind, player.seatPos, true)
		end
		-- 广播底牌
		LuaEvent.AddEvent(EEventType.DDZShowBaseCards, rsp)

		-- 地主是自己，需要加牌
		if rsp.bankerUserId == DataManager.GetUserID() and player then
			local playerCardsCount = #player.cardMgr:GetHandCards()
			local cards = {}
			for k, v in ipairs(rsp.pai) do
				local card = CCard.New()
				card:Init(v, playerCardsCount + k)
				table.insert(cards, card)
			end
			player.cardMgr:AddCards(cards)
			LuaEvent.AddEventNow(EEventType.SelfHandCardInit,false,nil)
		end

		-- 牌的数量
		for i = 1, 4 do
			local seatID = i-1
			local player = self.roomObj.playerMgr:GetPlayerBySeatID(seatID)--座位号从0开始的
			if player then
				if self.bankerSeatId == seatID then
					LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards, player.seatPos,20)
				else
					LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards, player.seatPos,17)
				end
			end
		end

		self.roomObj:ChangeState(RoomStateEnum.Playing)
	end
end

-- 提示地主是否明牌
function DDZPlayCardLogic:HandleNotifyIsOpenPlayPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then

		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player and player.seatInfo then
			self.roomObj.playerMgr:DDZLordOpenCardState(player.seatInfo.userId)
			-- 记录当前工作玩家的id
			self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
		end
		LuaEvent.AddEvent(EEventType.DDZNotifyLordOpenPlay, rsp)
	end
end

-- 广播地主是否名牌
function DDZPlayCardLogic:HandleLordOpenPlayPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		-- 清空当前工作的玩家ID
		self.roomObj:SaveWorkingPlayerID(-1)
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(self.bankerSeatId)
		if player then
			self.roomObj.playerMgr:PlayerWaiting(player.seatInfo.userId)
			AudioManager.DDZ_PlayOpenCard(player.seatInfo.sex, rsp.isOpen)
		end
		LuaEvent.AddEvent(EEventType.DDZLordOpenPlay, rsp)
		self:DealMultiplePBs(rsp.multiples)
		
		if rsp.isOpen then
			LuaEvent.AddEvent(EEventType.DDZShowDouble)
		end
		
		self.roomObj:ChangeState(RoomStateEnum.Playing)
	end
end

-- 提示农民加倍
function DDZPlayCardLogic:HandleNotifyFarmerAddTimesPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player and player.seatInfo then
			self.roomObj.playerMgr:DDZFarmerAddDoubleState(player.seatInfo.userId)
			-- 记录当前工作的玩家ID
			self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
		end
		LuaEvent.AddEvent(EEventType.DDZNotifyFarmerAddDouble, rsp)
	end
end

-- 广播农民加倍
function DDZPlayCardLogic:HandleFarmerAddTimesPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		-- 清空当前工作的玩家ID
		self.roomObj:SaveWorkingPlayerID(-1)
		LuaEvent.AddEvent(EEventType.DDZFarmerAddDouble, rsp)
		
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player then
			self.roomObj.playerMgr:PlayerWaiting(player.seatInfo.userId)
			AudioManager.DDZ_PlayDouble(player.seatInfo.sex, rsp.isPlus)
		end
		
		if not rsp.isPlus then
			self.farmerNoPlusNum = self.farmerNoPlusNum + 1
		end

		if self.farmerNoPlusNum == 2 then
			-- 有两个农民不加倍
			LuaEvent.AddEvent(EEventType.DDZLordAddDouble, nil)
		end
		self:DealMultiplePBs(rsp.multiples)

		self.roomObj:ChangeState(RoomStateEnum.Playing)
	end
end

-- 提示地主反加倍
function DDZPlayCardLogic:HandleNotifyLordSubTimesPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player and player.seatInfo then
			self.roomObj.playerMgr:DDZLordAddDoubleState(player.seatInfo.userId)
			-- 记录当前工作玩家的ID
			self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
		end
		LuaEvent.AddEvent(EEventType.DDZNotifyLordAddDouble, rsp)
	end
end

-- 广播地主反加倍
function DDZPlayCardLogic:HandleLordSubTimesPush(rsp)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		-- 清空当前工作的玩家ID
		self.roomObj:SaveWorkingPlayerID(-1)
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player then
			self.roomObj.playerMgr:PlayerWaiting(player.seatInfo.userId)
			AudioManager.DDZ_PlayDouble(player.seatInfo.sex, rsp.isBack)
		end

		LuaEvent.AddEvent(EEventType.DDZLordAddDouble, rsp)
		LuaEvent.AddEvent(EEventType.DDZLordAddDouble, nil)

		self:DealMultiplePBs(rsp.multiples)

		self.roomObj:ChangeState(RoomStateEnum.Playing)
	end
end

-- 小局结算推送
function DDZPlayCardLogic:HandleSmallResultPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		LuaEvent.AddEvent(EEventType.ReceiveSmallResultPush, rsp)
		self:BaseHandleSmallResultPush(rsp, head)
		if rsp.now >= rsp.total then
			self.isAllRoundEnd = true
		end

		-- 缓存我的倍数信息
		for k, v in ipairs(rsp.players) do
			if v.userId == DataManager.GetUserID() then
				self.MyPBMultiple = v.multiple
				LuaEvent.AddEvent(EEventType.DDZMultiplePBInfo, self.MyPBMultiple)
			end
		end

		-- 清空地主
		self.bankerSeatId = -1
		
		AnimManager.DDZPlaySpringAnim(rsp)
		AudioManager.DDZ_PlaySpringAudio(rsp)
		if self.isPlayTaiPaiAction then
			local taskID = DelayTaskSys.AddTask(self.SmallResultFunc,self,rsp)
			self:AddDelayTask("SmallResultPush",taskID,4)
		else
			local taskID = DelayTaskSys.AddTask(self.SmallResultFunc,self,rsp)
			self:AddDelayTask("SmallResultPush",taskID,2)
		end
	end
end

--提示玩家出牌推送（服务端广播给4个人）
function DDZPlayCardLogic:HandleNotifyPlayCardPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	-- 取消加倍的显示
	LuaEvent.AddEvent(EEventType.DDZLordAddDouble, nil)
	if not self:CheckRoomInfoLegal() then
		return
	end
	
	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player then
			--玩家状态转换
			self.roomObj.playerMgr:PlayerOutCardTips(player.seatInfo.userId,rsp.token,rsp.isForce)
			self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
		end
	end
end

-- 玩家出牌推送
function DDZPlayCardLogic:HandleSvrPlayCardPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
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
			if rsp.userId ~= DataManager.GetUserID() then
				AudioManager.DDZ_PlayCardInTableByRsp(rsp)
				AudioManager.DDZ_PlayBaoPai(rsp)
				AnimManager.DDZPlayAnimByPaiClass(rsp, PlayGameSys.GetNowPlayId())
			else
			end
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
		local myPlayer = self.roomObj.playerMgr:GetPlayerByPlayerID(DataManager.GetUserID())
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
function DDZPlayCardLogic:HandleClearCardsPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		local taskID = DelayTaskSys.AddTask(self.ClearCardsPushFunc,self,rsp)
		self:AddDelayTask("ClearCardsPush",taskID,0.3)
	end
end

--摊牌推送
function DDZPlayCardLogic:HandleShowAllCardsPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		local taskID = DelayTaskSys.AddTask(self.HandleShowAllCardsFunc,self,rsp)
		self:AddDelayTask("ShowAllCardsPush",taskID,1)
	end
end
--------------------------------------------------------------------------------

------------------------------------网络消息发送----------------------------------
--登陆成功
function DDZPlayCardLogic:LoginSuc(bReconect,isCreate)
	self.isLoginSvrSuc = true
	-- 发送创建房间或者加入房间
	if bReconect then
		self:SendReconnect()
	else
		if isCreate then
			self:SendCreateRoom()
		else
			self:SendJoinRoom(self.roomId)
		end
	end
end

-- 心跳包
function DDZPlayCardLogic:SendHeartBeat()
	if not self.isLoginSvrSuc then
		return
	end
	local body = {}
	body.userId = DataManager.GetUserID()
	SendMsg(GAME_CMD.CS_HEART_BEAT,body,function(rsp,head) 
	end,function(rsp,head)
		-- 失败
		DwDebug.LogError("recv CS_HEART_BEAT fail seq = "..head.seq .."time = "..WrapSys.GetCurrentDateTime())
	end,true)
end

--连接服务器成功后发送登录消息进行验证
function DDZPlayCardLogic:SendLoginSvr(loginAddress,loginLng,loginLat)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.accessToken = DataManager.GetToken()
	body.secretString = LuaUtil.MD5(body.userId,body.accessToken,DataManager.SecretKey)
	body.loginAddress = DataManager.GetLoginLocationStr()
	body.loginLng = WrapSys.GetLongitude()
	body.loginLat = WrapSys.GetLatitude()

	DwDebug.Log("==============SendLoginSvr body.loginAddress:", body.loginAddress, "body.loginLng:", body.loginLng, "body.loginLat:", body.loginLat)

	SendMsg(GAME_CMD.CS_LOGIN,body,function(rsp,head) 
		self:LoginSuc(self.isReconnect,self.isCreate)
		self.isReconnect = false
		PlayGameSys.RegisterHeartBeat()
	end,function(rsp,head)
		if nil ~= rsp.message and "" ~= rsp.message then
			WindowUtil.LuaShowTips(rsp.message)
		else
			WindowUtil.LuaShowTips("登录连接超时,请稍候重试")
		end

		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		LuaEvent.AddEventNow(EEventType.UI_CreateRoomResult,false)
		
		if GameStateMgr.GetCurStateType() == EGameStateType.LoginState then
			--如果网络出现问题，重连失败需要释放建立的逻辑和登录状态
			PlayGameSys.ReleasePlayLogic()
			PlayGameSys.CloseNetWork()
			LoginSys.ResetLoginStatus()
		end
	end,true)
end

-- 创建房间
function DDZPlayCardLogic:SendCreateRoom()
	local room_config = DataManager.GetRoomConfig()
	if not room_config then
		DwDebug.LogError("创建房间参数错误")
		LuaEvent.AddEventNow(EEventType.UI_CreateRoomResult,false)
		return
	end

	SendMsg(GAME_CMD.CS_CREATE_ROOM,room_config,function(rsp,head)
		HallSys.isCreateOrJoinRooming = false
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		--创建成功
		--开始切换到游戏场景
		GameStateMgr.GoToGameScene()
		--WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
	end,function(rsp,head)
		HallSys.isCreateOrJoinRooming = false
		if nil ~= rsp.message and "" ~= rsp.message then
			WindowUtil.LuaShowTips(rsp.message)
		else
			WindowUtil.LuaShowTips("创建房间超时,请稍候重试")
		end
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		LuaEvent.AddEventNow(EEventType.UI_CreateRoomResult,false)
		PlayGameSys.ReleasePlayLogic()
		PlayGameSys.CloseNetWork()
	end,true)
end

-- 加入房间
function DDZPlayCardLogic:SendJoinRoom(roomId)
	local  body = {}
	body.roomId = roomId
	SendMsg(GAME_CMD.CS_JOIN_ROOM,body,function(rsp,head)
		HallSys.isCreateOrJoinRooming = false
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		--开始切换到游戏场景
		GameStateMgr.GoToGameScene()
	end,function(rsp,head)
		HallSys.isCreateOrJoinRooming = false
		if nil ~= rsp.message and "" ~= rsp.message then
			WindowUtil.LuaShowTips(rsp.message)
		else
			WindowUtil.LuaShowTips("加入房间超时,请稍候重试")
		end
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		PlayGameSys.ReleasePlayLogic()
		PlayGameSys.CloseNetWork()
	end,true)
end

--发送UI渲染就绪
function DDZPlayCardLogic:SendUIReady()
	self.UIIsReady = true
	local body = {}
	body.renderStatus = true
	SendMsg(GAME_CMD.CS_RENDER_UI_SUCCESS,body)
end

-- 重连
function DDZPlayCardLogic:SendReconnect()
	self.reconnectGameInfo = false

	local body = {}
	SendMsg(GAME_CMD.CS_NET_RECONNECT,body,function(rsp,head)
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		--开始切换到游戏场景
		if GameStateMgr.GetCurStateType() ~= EGameStateType.GameState then
			GameStateMgr.GoToGameScene()
		else
			if GameState.isEnterCompleted then
				self:SendUIReady()
			end
		end
	end,function(rsp,head)
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)

		if nil ~= rsp.message and "" ~= rsp.message then
			WindowUtil.LuaShowTips(rsp.message)
		else
			WindowUtil.LuaShowTips("重连房间超时,请稍候重试")
		end

		--如果网络出现问题，重连失败需要释放建立的逻辑和登录状态
		if GameStateMgr.GetCurStateType() == EGameStateType.LoginState then
			LoginSys.ResetLoginStatus()
			PlayGameSys.ReleasePlayLogic()
			PlayGameSys.CloseNetWork()
		elseif GameStateMgr.GetCurStateType() == EGameStateType.GameState then
			if head and self:IsNotRoomErrno(head.errno) then
				PlayGameSys.QuitToMainCity()
			end
		elseif GameStateMgr.GetCurStateType() == EGameStateType.MainCityState then
			PlayGameSys.ReleasePlayLogic()
			PlayGameSys.CloseNetWork()
		else
			-- 加入失败,移除心跳注册 （每次登陆验证会检查心跳是否开启）
			PlayGameSys.RemoveHeartBeat()
		end
	end)
end

--房间不存在的错误码
function DDZPlayCardLogic:IsNotRoomErrno(errno)
	return errno == 10613
end

--发送游戏准备
function DDZPlayCardLogic:SendPrepare(readyType)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.readyType = readyType or 0
	SendMsg(GAME_CMD.CS_PREPARE, body, nil, nil, true)
end

-- 发送申请解散
function DDZPlayCardLogic:SendAskDismiss()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_DISMISS,body)
end

--发送解散投票
function DDZPlayCardLogic:SendDismissVote(isAgree)
	local body = {}
	body.isAgree = isAgree
	SendMsg(GAME_CMD.CS_PLAYER_VOTE,body)
end

--请求LBS
function DDZPlayCardLogic:SendAskLBS()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_LBS,body)
end

-- 发出退出房间
function DDZPlayCardLogic:SendQuitRoom()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_QUIT_ROOM,body,function (rsp,head)
		PlayGameSys.QuitToMainCity()
	end,function ()
		WindowUtil.LuaShowTips("退出房间失败")
	end)
end

--请求历史积分
function DDZPlayCardLogic:SendAskHistoryScore()
	local body = {}
	SendMsg(GAME_CMD.CS_HISTORY_SCORE,body)
end

-- 房主请求踢人
function DDZPlayCardLogic:SendASKKickOut(userID, callBack)
	if userID then
		local body = {}
		body.userId = userID
		SendMsg(GAME_CMD.CS_ASK_KICK_OUT, body, function(rsp, head)
			if callBack ~= nil then
				callBack()
			end
		end)
	end
end

--发牌请求
function DDZPlayCardLogic:SendOutCard(cardIDs, token, isSkip)
	if token then
		local body = {}
		body.token = token
		body.pai = cardIDs
		body.isSkip = isSkip
		SendMsg(GAME_CMD.CS_PLAYER_PLAY_CARD, body, nil, nil, true)
	end
end

-- 发送是否要地主
function DDZPlayCardLogic:SendIsWantLordReq(isRob, score)
	local body = {}
	body.isRob = isRob
	body.score = score
	SendMsg(GAME_CMD.CS_REPLY_WANT_LORD, body, nil, nil, true)
end

-- 发送地主是否明牌
function DDZPlayCardLogic:SendLordIsOpenPlayReq(isOpen)
	local body = {}
	body.isOpen = isOpen
	SendMsg(GAME_CMD.CS_REPLY_ALONE_LORD_IS_OPEN_PLAY, body, nil, nil, true)
end

-- 农民加倍请求
function DDZPlayCardLogic:SendFarmerAddTimesReq(isPlus)
	local body = {}
	body.isPlus = isPlus
	SendMsg(GAME_CMD.CS_REPLY_FARMER_ADD_TIMES, body, nil, nil, true)
end

--地主反加倍请求
function DDZPlayCardLogic:SendLordSubTimesReq(isBack)
	local body = {}
	body.isBack = isBack
	SendMsg(GAME_CMD.CS_REPLY_LORD_SUB_TIMES, body, nil, nil, true)
end
--------------------------------------------------------------------------------

-------------------------------------对内接口------------------------------------
-- 小局结算处理函数
function DDZPlayCardLogic:SmallResultFunc(rsp)
	if self.roomObj then
		if self.isPlayTaiPaiAction then
			self.isPlayTaiPaiAction = false
		end
		self.roomObj:AddSmallResult(rsp.now,rsp)
		-- 清理当前桌面分
		LuaEvent.AddEventNow(EEventType.RefreshCurTablePoint,0)
		
		self.roomObj:UpdateCurrentRound(rsp.next)
		self.roomObj:ChangeState(RoomStateEnum.SmallRoundOver)
	end
end

function DDZPlayCardLogic:NotifyWantLordFunc(rsp)
	if self.isPlayDealCard then
		self.isPlayDealCard = false
		self:RemoveDelayTask("NotifyWantLordFunc")
		-- 显示底牌牌背
		LuaEvent.AddEvent(EEventType.DDZShowBaseCards, nil, 2)
		-- 此时都是17张牌
		for i = 1, 4 do
			local seatID = i-1
			local player = self.roomObj.playerMgr:GetPlayerBySeatID(seatID)--座位号从0开始的
			if player then
				LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards, player.seatPos,17)
			end
		end
	end
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if player and player.seatInfo then
		self.roomObj.playerMgr:DDZWantLordState(player.seatInfo.userId)
		if player.seatInfo.userId == DataManager.GetUserID() then
			-- 提示是自己强地主
			LuaEvent.AddEvent(EEventType.NotifyWantLordEvent, rsp)
		end
		-- 记录当前工作的玩家
		self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
	end
end

function DDZPlayCardLogic:ClearCardsPushFunc(rsp)
	if rsp then
		local curRoundIndex = self.cardLogicContainer:GetCurRoundIndex()
		if rsp.roundIndex >= curRoundIndex then
			--一轮结束重置一轮打牌模块
			LuaEvent.AddEventNow(EEventType.RoundEndClear,nil,nil)
			--清桌
			LuaEvent.AddEventNow(EEventType.ClearDesk,nil,nil)
		end
	end
end

function DDZPlayCardLogic:HandleShowAllCardsFunc(rsp)
	LuaEvent.AddEventNow(EEventType.SelfHandCardClear)
	--清桌
	LuaEvent.AddEventNow(EEventType.ClearDesk,nil,nil)
	
	local player
	for k,v in pairs(rsp.normalPai) do
		player = self.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
		if player then
			LuaEvent.AddEventNow(EEventType.TanPaiAction,player.seatPos,v.normalPai)
		end
	end
	self.isPlayTaiPaiAction = true
end
--------------------------------------------------------------------------------
function DDZPlayCardLogic:DealMultiplePBs(multiples)
	local myPlayer = self.roomObj.playerMgr:GetPlayerByPlayerID(DataManager.GetUserID())
	if myPlayer and myPlayer.seatInfo then
		local multipleInfo = multiples[myPlayer.seatInfo.seatId + 1]
		if multipleInfo then
			self.MyPBMultiple = multipleInfo
			LuaEvent.AddEvent(EEventType.DDZMultiplePBInfo, self.MyPBMultiple)
		end
	end
end

function DDZPlayCardLogic:ResetPBMultiple()
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