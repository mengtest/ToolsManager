--------------------------------------------------------------------------------
--   File      : MJNormalPlayCardLogic.lua
--   author    : guoliang
--   function   : 正常玩法逻辑基类
--   date      : 2017-11-5
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.DWBaseModule"
require "Logic.RoomLogic.BaseObjectState"
require "Logic.MJCardLogic.MJCardTypeEnum"
require "Logic.MJCardLogic.CMJCard"
require "Logic.RoomLogic.CPlayer"
require "Logic.RoomLogic.Room.CMJRoom"
require "Logic.SystemLogic.BasePlayCardLogic"
require "LuaSys.AnimManager"

MJNormalPlayCardLogic = class("MJNormalPlayCardLogic", BasePlayCardLogic)

function MJNormalPlayCardLogic:ctor()
	self:BaseCtor()
	--是否发牌动画中
	self.isPlayDealCard = false
	--是否摊牌中
	self.isPlayTaiPaiAction = false
end

function MJNormalPlayCardLogic:Init()
	self:BaseInit()
	-- 测试
	self.roomId = PlayGameSys.testRoomID
	self.isCreate = PlayGameSys.testIsCreate
	self.isReconnect = PlayGameSys.testIsReconnect
	--房间管理
	self.roomObj = CMJRoom.New()
	self.roomObj:Init(self)
end

function MJNormalPlayCardLogic:Destroy()
	self:CleanRoom()
	self:BaseDestroy()
	self.roomObj:Destroy()
	self.roomObj = nil
end

function MJNormalPlayCardLogic:GetType()
	return PlayLogicTypeEnum.MJ_Normal
end

function MJNormalPlayCardLogic:IsMyTurn()
	if self.roomObj and self.roomObj.curWorkingID and self.roomObj.curWorkingID == DataManager.GetUserID() then
		return true
	end

	return false
end

-------------------------------事件消息--------------------------

function MJNormalPlayCardLogic:RegisterEvent()
	LuaEvent.AddHandle(EEventType.Net_Common_Errno, self.Reconnect,self)
end
function MJNormalPlayCardLogic:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.Net_Common_Errno, self.Reconnect,self)
end

function MJNormalPlayCardLogic:Reconnect(eventid, p1, p2)
	if p1 and p2 then
		if (p2 == GAME_ERRNO.ERRNO_PLAY_CARD_FAIL) then
			WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
		elseif p2 == GAME_ERRNO.ERRNO_PLAYER_NO_ROOM or p2 == GAME_ERRNO.ERRNO_ROOM_NOT_EXIST then
			PlayGameSys.QuitToMainCity()
		end
	end
end

-------------------------------- 网络消息推送 -----------------------------------
-- 网络消息注册
function MJNormalPlayCardLogic:RegisteNetPush()
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.CS_PREPARE, self.HandlePreparePush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_PLAYER_ONLINE_PUSH, self.HandlePlayerOnlinePush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_START_GAMER_PUSH, self.HandleStartGamePush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_SEAT_INFO_PUSH, self.HandleSeatInfoPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_ROOM_INFO, self.HandleRoomInfoPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_GAME_INFO_PUSH, self.HandleGameInfoPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_NOTIFY_PLAY_PUSH, self.HandleReactPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_CARD_PUSH, self.HandleReceiveCardPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_WHOS_TURN_PUSH, self.HandleWhosTurnPush, self)

	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_PLAY_CARD, self.HandlePlayCardPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_HU, self.HandleHuPaiPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_GANG, self.HandleGangPaiPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_PENG, self.HandlePengPaiPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_CHI, self.HandleChiPaiPush, self)


	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_SCORE_PUSH, self.HandleScorePush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_SMALL_RESULT_PUSH, self.HandleSmallResultPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_BIG_RESULT_PUSH, self.MJHandleBigResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_BIG_RESULT_PUSH, self.MJHandleBigResultPushImmediately, self)

	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.CS_PLAYER_VOTE,self.HandleNotifyDismissAskPush,self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_NOTIFY_VOTE_PUSH, self.HandleNotifyDismissVotePush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_NOTIFY_DISMISS_RESULT_PUSH, self.HandleNotifyDismissResultPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_DISMISS, self.HandleRoomDismissedPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.CS_ASK_QUIT_ROOM, self.HandleQuitRoomPush, self)

	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_SHOW_ALL_CARDS_PUSH, self.HandleShowAllCardsPush, self, 3)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_HISTORY_SCORE_PUSH, self.HandleHistoryScorePush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_DISTANCE_RSP, self.HandleLBSNotify, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_NOTIFY_KICK_PUSH, self.HandleNotifyKickPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_LIUJU, self.HandleLiuJuPush, self, 2)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_DISTANCE_TIPS_PUSH, self.HandleDistanceTipsPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_SHAOZHUANG, self.HandleShaoZhuangPush, self)
	self:RegisteHandleToIntervalTaskQueue(GAME_CMD.SC_ASK_KICK_OUT_REPLY, self.HandleASKKickOutReplay, self)
end

-- 网络消息移除
function MJNormalPlayCardLogic:UnRegisteNetPush()
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.CS_PREPARE, self.HandlePreparePush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_PLAYER_ONLINE_PUSH, self.HandlePlayerOnlinePush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_START_GAMER_PUSH, self.HandleStartGamePush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_ROOM_INFO, self.HandleRoomInfoPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_SEAT_INFO_PUSH, self.HandleSeatInfoPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_GAME_INFO_PUSH, self.HandleGameInfoPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_NOTIFY_PLAY_PUSH, self.HandleReactPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_CARD_PUSH, self.HandleReceiveCardPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_WHOS_TURN_PUSH, self.HandleWhosTurnPush, self)

	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_PLAY_CARD, self.HandlePlayCardPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_HU, self.HandleHuPaiPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_GANG, self.HandleGangPaiPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_PENG, self.HandlePengPaiPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.CS_SC_GAMEPLAY_CHI, self.HandleChiPaiPush, self)

	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_SCORE_PUSH, self.HandleScorePush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_SMALL_RESULT_PUSH, self.HandleSmallResultPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_BIG_RESULT_PUSH, self.MJHandleBigResultPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_BIG_RESULT_PUSH, self.MJHandleBigResultPushImmediately, self)

	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.CS_PLAYER_VOTE,self.HandleNotifyDismissAskPush,self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_NOTIFY_VOTE_PUSH, self.HandleNotifyDismissVotePush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_NOTIFY_DISMISS_RESULT_PUSH, self.HandleNotifyDismissResultPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_DISMISS, self.HandleRoomDismissedPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.CS_ASK_QUIT_ROOM, self.HandleQuitRoomPush, self)

	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_SHOW_ALL_CARDS_PUSH, self.HandleShowAllCardsPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_HISTORY_SCORE_PUSH, self.HandleHistoryScorePush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_DISTANCE_RSP, self.HandleLBSNotify, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_NOTIFY_KICK_PUSH, self.HandleNotifyKickPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_LIUJU, self.HandleLiuJuPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_DISTANCE_TIPS_PUSH, self.HandleDistanceTipsPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_GAMEPLAY_SHAOZHUANG, self.HandleShaoZhuangPush, self)
	self:UnRegisteHandleFromIntervalTaskQueue(GAME_CMD.SC_ASK_KICK_OUT_REPLY, self.HandleASKKickOutReplay, self)
end

-- 玩家准备推送
function MJNormalPlayCardLogic:HandlePreparePush(rsp,head)
	if rsp then
		if rsp.readyType == 0 then
			self.roomObj.playerMgr:PlayerPrepared(rsp.userId)
			local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
			if player then
				WindowUtil.LuaShowTips("玩家 ".. player.seatInfo.nickName .."已经准备就绪")
			end
		else
			self.roomObj.playerMgr:PlayerIdle(rsp.userId)
		end
	end
end

function MJNormalPlayCardLogic:HandleRoomInfoPush(rsp,head)
	if rsp then
		if not rsp.disbanding then
		    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "disband_room_ui_window", true , nil)
    	end

		--开始在玩，连回来发现本局一局结束了，不会推送游戏信息
		local curStateType = self.roomObj.roomStateMgr:GetCurStateType()
		if curStateType == RoomStateEnum.Playing and rsp.status ~= RoomStateEnum.Playing then
			self:CleanRoom()
		end

		self.super.super.HandleRoomInfoPush(self,rsp,head)
	end
end


-- 开始游戏推送
function MJNormalPlayCardLogic:HandleStartGamePush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp and self.roomObj.roomInfo and rsp.roomId == self.roomObj.roomInfo.roomId then
		self.roomObj:ChangeState(RoomStateEnum.Playing)
	end

	self.ProxyOnRecvGameInfo = self.OnFirstRecvGameInfo

	--开局前检查人数是否合法
	if self.roomObj.playerMgr:CheckPlayerNumIsLegal(self.roomObj.roomInfo.playerNum) == false then
		WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
	end
end

--游戏信息推送
function MJNormalPlayCardLogic:HandleGameInfoPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	self:CleanRoom()
	self:ProxyOnRecvGameInfo(rsp, head)
	-- self:OnRecvGameInfo(rsp, head)
end

function MJNormalPlayCardLogic:ProxyOnRecvGameInfo(rsp, head)

	self:OnNormalRecvGameInfo(rsp, head)
end

-- 断线重连刷新游戏信息
function MJNormalPlayCardLogic:OnNormalRecvGameInfo(rsp, head)
	-- 断线重连才清掉task队列
	-- self:ClearIntervalTaskQueue()
	self:InitGameData(rsp)
	self:HandleGameInfo(rsp, head)
end

-- 开局播放发牌动画
function MJNormalPlayCardLogic:OnFirstRecvGameInfo(rsp, head)
	self.ProxyOnRecvGameInfo = self.OnNormalRecvGameInfo
	if rsp then
		self:InitGameData(rsp)
		LuaEvent.AddEventNow(EEventType.MJPlayFaPaiAction,
			{__cname = "temp_table", OnActionFinish = function()
				self:HandleGameInfo(rsp, head)
			end}
			)
	end
end

-- 初始化手牌，播发牌动画要用
function MJNormalPlayCardLogic:InitGameData(rsp)
	self.roomObj:ChangeState(RoomStateEnum.Playing)
	--庄家
	local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.zhuangJiaId)
	if player then
		LuaEvent.AddEventNow(EEventType.PlayerShowZhuangJia,player.seatPos,true)
	end
	--待出牌和风向提示
	if rsp.outCardNotice then
		player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.outCardNotice.fengXiangUserId)
		if player then
			AnimManager.PlayPlayerHeadAnim(true, player.seatPos)
			LuaEvent.AddEventNow(EEventType.MJUpdateWindDir,player.seatPos)
		end
		-- 设置下当前出牌的人
		if rsp.outCardNotice.fengXiangUserId == rsp.outCardNotice.outCardUserId then
			self.roomObj:SaveWorkingPlayerID(rsp.outCardNotice.outCardUserId)
		end
	end
	--牌桌剩余牌张数
	LuaEvent.AddEventNow(EEventType.MJ_ShowRemainCardNum,true,rsp.remainPaiNum)
	-- 初始化手牌数据
	for i,v in pairs(rsp.playerCard) do
		if next(v) ~= nil then
			local player = self.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
			if player then
				--先清理缓存手牌
				player.cardMgr:Clear()
			end
			-- 玩家自己手牌
			self.roomObj.playerMgr:InitPlayerHandCards(v.userId,v)
		end
	end
	-- 初始化手牌，生成gameobject
	LuaEvent.AddEventNow(EEventType.MJInitHandCards)

end

-- 这里会刷新一些牌局信息，已出手牌和已碰杠胡的牌
function MJNormalPlayCardLogic:HandleGameInfo(rsp, head)
	if rsp then
		self.roomObj:UpdateCurrentRound(rsp.currentGamepos)
		LuaEvent.AddEventNow(EEventType.RefreshRoomRoundNum, rsp.currentGamepos, rsp.totalGameNum)
		LuaEvent.AddEventNow(EEventType.MJShowDeskCenterCtrl,true)
		self:MJHandleGameInfoPush(rsp,head)
		-- 切换玩家状态,可以出牌或者可以碰杠胡的玩家
		self.roomObj.playerMgr:PlayerMonopolizePlaying(rsp.outCardNotice.outCardUserId)
	end
end

-- 游戏信息推送(重连后整个游戏信息推送)
function MJNormalPlayCardLogic:MJHandleGameInfoPush(rsp,head)
	self:BaseHandleGameInfoPush(rsp, head)
end

-- 碰/吃/杠/胡 推送
function MJNormalPlayCardLogic:HandleReactPush(rsp)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if self.roomObj and rsp then
		--LuaEvent.AddEventNow()
		--如果不是自己，进入等待；如果是自己，抛出事件，参数带上推送的数据
		LuaEvent.AddEventNow(EEventType.MJOpeTip, rsp)
		-- 切换玩家状态,Warning：一炮多响的情况下，会有多个玩家在playing状态，目前没有问题就先不管啦
		-- 玩家状态只有显示等待动画这个逻辑用到了
		self.roomObj.playerMgr:PlayerMonopolizePlaying(rsp.userId)
	end
end

-- 派牌
function MJNormalPlayCardLogic:HandleReceiveCardPush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		--暂时在派牌中检查玩家数量合法
		-- 刷新牌堆牌张数
		LuaEvent.AddEventNow(EEventType.MJ_ShowRemainCardNum,true,rsp.remainPaiNum)
		--是->判断是否有actionNotice,如果没有则只刷新显示，如果有则抛出杠胡事件
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)

		local card_item = CMJCard.New()
		card_item:Init(rsp.pai, 1)
		-- 通知 手牌控件 更新显示
		LuaEvent.AddEventNow(EEventType.MJInCard, player, card_item)

		-- 判断是否自己，是的话判断下自摸杠胡等等
		if rsp.userId == DataManager.GetUserID() then
			--自己播派牌声音
--          AudioManager.PlayCommonSound(UIAudioEnum.mj_fapai)
			if rsp.actionNotice then
				-- 有 操作提示数据 则通知 操作提示控件 更新显示
				LuaEvent.AddEventNow(EEventType.MJOpeTip, rsp.actionNotice)
			end
		end

		-- 切换玩家状态
		self.roomObj.playerMgr:PlayerMonopolizePlaying(rsp.userId)
	end
end

--提示玩家出牌推送（服务端广播给4个人）
function MJNormalPlayCardLogic:HandleWhosTurnPush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		--谁家出牌
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.outCardUserId)
		if player then
			self.roomObj:SaveWorkingPlayerID(rsp.outCardUserId)
			-- 轮到玩家出牌,这个协议只有切换出牌权的时候才会发，出现碰杠胡不会推送,所以不能放在这，
			-- 这点我怪服务器，哈哈哈哈
			-- self.roomObj.playerMgr:PlayerMonopolizePlaying(rsp.outCardUserId)
		end

		--风向标指向
		player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.fengXiangUserId)
		if player then
			AnimManager.PlayPlayerHeadAnim(true, player.seatPos)
			LuaEvent.AddEventNow(EEventType.MJUpdateWindDir,player.seatPos)
		end
	end
end

-- 玩家出牌推送
function MJNormalPlayCardLogic:HandlePlayCardPush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		if rsp.userId ~= DataManager.GetUserID() then
			self:HandlePlayCard(rsp.userId,rsp.pai)
		else
		end
	end
end

-- 处理出牌逻辑
function MJNormalPlayCardLogic:HandlePlayCard(user_id, pai)
	local player = self.roomObj.playerMgr:GetPlayerByPlayerID(user_id)
	if player then
		-- 刷新手牌显示
		AudioManager.MJ_PlayBaoPai(player.seatInfo.sex,pai)
		if user_id == DataManager.GetUserID() then
			LuaEvent.AddEventNow(EEventType.MJOutSelfCard, player, pai)
			-- LuaEvent.AddEventNow(EEventType.MJRemoveSelfCard, player, {pai})
		else
			LuaEvent.AddEventNow(EEventType.MJRemoveOtherCard, player, 1)
		end
		AudioManager.PlayCommonSound(UIAudioEnum.chuPai)
		AnimManager.PlayChuPaiAnim(pai, player.seatPos)
		-- 刷新出牌显示
		LuaEvent.AddEventNow(EEventType.MJ_PlayNormalOutMJCardShow, player.seatPos, {pai, true,true})
		-- 出牌进入等待
		self.roomObj.playerMgr:PlayerWaiting(user_id)
		self.roomObj:SaveWorkingPlayerID(0)
		-- 如果是上家,则加个倒计时，3s内没有出现风向推送则播放等待其他玩家动画
		local self_player = self.roomObj.playerMgr:GetPlayerByPlayerID(DataManager.GetUserID())
		if  (self_player.seatInfo.fengxiang == 1 and player.seatInfo.fengxiang == 4) or
			(player.seatInfo.fengxiang + 1 == self_player.seatInfo.fengxiang) then
			self_player:ChangeState(PlayerStateEnum.ExpectPushCard)
		end
	end
end

-- 玩家胡牌推送
function MJNormalPlayCardLogic:HandleHuPaiPush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		if player then
			if MJHuPaiType.IsMJZiMo(rsp.huType) then
				AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.ziMo)
				AnimManager.PlayMJCardAnim(MJAnimEnum.ZIMO, player.seatPos)
			else
				AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.huLe)
				AnimManager.PlayMJCardAnim(MJAnimEnum.HU, player.seatPos)
				-- 抢杠胡，把杠的那张牌去掉
				if rsp.huType == MJHuPaiType.QiangGang then
					local other = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.actionerId)
					LuaEvent.AddEventNow(EEventType.MJ_PlayQiangGangShow, other.seatPos, rsp.huPai)
				end
			end
		end
		-- 不用干什么，等摊牌推送，也不能干什么，支持一炮多响的，可能需要等其他玩家胡牌
	end
end

-- 玩家杠牌推送
function MJNormalPlayCardLogic:HandleGangPaiPush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		-- logError("gang " .. ToString(rsp))
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		local is_self = rsp.userId == DataManager.GetUserID()
		-- 刷新显示
		if rsp.gangPai.gangType == 1 then
			AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.anGang)
			AnimManager.PlayMJCardAnim(MJAnimEnum.GANG, player.seatPos)
			local provider = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.gangPai.gangWithPlayerId)
			LuaEvent.AddEventNow(EEventType.MJ_PlayOwnSupplyGangShow, player.seatPos,rsp.gangPai.pai)
			-- 刷新手牌显示
			if is_self then
				LuaEvent.AddEventNow(EEventType.MJRemoveSelfCard, player, rsp.gangPai.pai)
			else
				LuaEvent.AddEventNow(EEventType.MJRemoveOtherCard, player, 4)
			end
		elseif rsp.gangPai.gangType == 2 then
			AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.gang)
			AnimManager.PlayMJCardAnim(MJAnimEnum.GANG, player.seatPos)
			local delete_hand_card = {rsp.gangPai.pai[1],rsp.gangPai.pai[1],rsp.gangPai.pai[1]}
			local provider = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.gangPai.gangWithPlayerId)
			LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow, {player.seatPos,provider.seatPos}, rsp.gangPai.pai)
			-- 刷新手牌显示
			if is_self then
				LuaEvent.AddEventNow(EEventType.MJRemoveSelfCard, player, delete_hand_card)
			else
				LuaEvent.AddEventNow(EEventType.MJRemoveOtherCard, player, 3)
			end
		elseif rsp.gangPai.gangType == 3 then
			AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.gang)
			AnimManager.PlayMJCardAnim(MJAnimEnum.GANG, player.seatPos)
			local delete_hand_card = {rsp.gangPai.pai[1]}
			LuaEvent.AddEventNow(EEventType.MJ_PlayOwnSupplyGangShow,player.seatPos,rsp.gangPai.pai)
			-- 刷新手牌显示
			if is_self then
				LuaEvent.AddEventNow(EEventType.MJRemoveSelfCard, player, delete_hand_card)
			else
				LuaEvent.AddEventNow(EEventType.MJRemoveOtherCard, player, 1)
			end
		end
		-- 去掉别人已出牌里的显示,只限明杠
		if rsp.gangPai.gangType == 2 then
			local other = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.gangPai.gangWithPlayerId)
			LuaEvent.AddEventNow(EEventType.MJ_NormalOutMJCardCatch,other.seatPos,rsp.gangPai.pai[1])
		end

		-- 有操作就进入等待
		self.roomObj.playerMgr:PlayerWaiting(rsp.userId)
	end
end

-- 玩家碰牌推送
function MJNormalPlayCardLogic:HandlePengPaiPush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		local provider = rsp.pengPai.pengWithPlayerId
		local other = self.roomObj.playerMgr:GetPlayerByPlayerID(provider)
		if player then
			AudioManager.MJ_PlayCaoZuo(player.seatInfo.sex,MJ_CaoZuoEnum.peng)
			AnimManager.PlayMJCardAnim(MJAnimEnum.PENG, player.seatPos)
			-- 刷新手牌显示
			if rsp.userId == DataManager.GetUserID() then
				LuaEvent.AddEventNow(EEventType.MJRemoveSelfCard, player, {rsp.pengPai.pai[1], rsp.pengPai.pai[1]})
			else
				LuaEvent.AddEventNow(EEventType.MJRemoveOtherCard, player, 2)
			end
			-- 刷新显示
			LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,{player.seatPos,other.seatPos},rsp.pengPai.pai)
		end
		-- 去掉别人被碰的牌
		LuaEvent.AddEventNow(EEventType.MJ_NormalOutMJCardCatch,other.seatPos,rsp.pengPai.pai[1])

		-- 有操作就进入等待
		self.roomObj.playerMgr:PlayerWaiting(rsp.userId)
		-- 切换玩家状态
		self.roomObj.playerMgr:PlayerMonopolizePlaying(rsp.userId)
	end
end

-- 玩家吃牌推送
function MJNormalPlayCardLogic:HandleChiPaiPush(rsp,head)
	if rsp then
		-- 崇仁麻将没有吃牌，协议也是错的
		-- local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		-- -- 刷新手牌显示
		-- if rsp.userId == DataManager.GetUserID() then
		--  LuaEvent.AddEventNow(EEventType.MJRemoveSelfCard, player, rsp.chiPai.pai)
		-- else
		--  LuaEvent.AddEventNow(EEventType.MJRemoveOtherCard, player, #rsp.chiPai.pai)
		-- end
		-- -- 刷新显示
		-- LuaEvent.AddEventNow(EEventType.MJ_PlayOwnOutMJCardsShow,player.seatPos,rsp.chiPai.pai)
		-- -- 去掉别人被吃的牌
		-- local other = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.chiPai.chiWithPlayerId)
		-- --  这里是随便选的第一张牌，正确应该是会传吃的哪张牌，崇仁麻将没有吃牌，所以不管
		-- LuaEvent.AddEventNow(EEventType.MJ_NormalOutMJCardCatch,other.seatPos,rsp.chiPai.pai[1])
	end
end

-- 积分推送
function MJNormalPlayCardLogic:HandleScorePush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		LuaEvent.AddEventNow(EEventType.MJShowScoreTwn,rsp)
	end
end

--小局结算推送
function MJNormalPlayCardLogic:HandleSmallResultPush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		self:BaseHandleSmallResultPush(rsp, head)
		if rsp.currentGamepos >= rsp.totalGameNum then
			self.isAllRoundEnd = true
		end

		TimerTaskSys.AddTimerEventByLeftTime(
			function ()
				self:SmallResultFunc(rsp)
			end,0.1)
	end
end

--小局结算处理函数
function MJNormalPlayCardLogic:SmallResultFunc(rsp)
	if self.roomObj then
		for k,v in pairs(rsp.playerMes) do
			if next(v) ~= nil then
				local player = self.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
				if player then
					player.seatInfo.totalJiFen = v.zongJifen
				end
			end
		end

		-- 小局结算数据存放
		self.roomObj:AddSmallResult(rsp.currentGamepos,rsp)
		--更新局数
		self.roomObj:UpdateCurrentRound(rsp.nextGamepos)
		-- 切换房间状态
		self.roomObj:ChangeState(RoomStateEnum.SmallRoundOver)

		--刷新头像分数
		local uiData = rsp
		local players = uiData.playerMes
		local total = uiData.totalGameNum
		local next_pos = uiData.nextGamepos
		for i = 1, #players do
			local player = players[i]
			if next(player) ~= nil then
				local m_player = PlayGameSys.GetPlayerByPlayerID(player.userId)
				if m_player then
					LuaEvent.AddEvent(EEventType.RefreshPlayerTotalScore, m_player.seatPos, player.zongJifen)
				end
			end
		end
	end
end

-- 大结算推送
function MJNormalPlayCardLogic:MJHandleBigResultPush(rsp,head)
	if not self:CheckGameCoreDataLegal() then
		PlayGameSys.QuitToMainCity()
		return
	end
	if rsp then
		-- self:ClearIntervalTaskQueue()
		self.super.super.HandleBigResultPush(self, rsp, head)
	end
end

-- 来大结算的时候必须要立即关掉连接，不然还会发心跳包
function MJNormalPlayCardLogic:MJHandleBigResultPushImmediately(rsp, head)
	if not self:CheckGameCoreDataLegal() then
		WindowUtil.LuaShowTips("牌局已经结束")
		return
	end
	PlayGameSys.CloseNetWork()
end

--摊牌推送
function MJNormalPlayCardLogic:HandleShowAllCardsPush(rsp,head)
	-- 切换玩家状态
	self.roomObj.playerMgr:AllPlayerWaiting()

	self:ShowAllCards(rsp)

	-- local taskID = DelayTaskSys.AddTask(self.ShowAllCards,self,rsp)

	-- self:AddDelayTask("ShowAllCards",taskID,2)
end
-- 摊牌显示
function MJNormalPlayCardLogic:ShowAllCards(rsp)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		-- 暗杠翻开来
		LuaEvent.AddEventNow(EEventType.MJAnGangTanPai)

		local playerList = {}
		-- 如果是自摸，则去掉手牌里胡的那张牌
		if MJHuPaiType.IsMJZiMo(rsp.huType) and rsp.huByUserId then
			local player_index,pai_index
			for i,v in ipairs(rsp.normalPai) do
				if next(v) ~= nil then
					if v.userId == rsp.huByUserId then
						for index,pai_id in ipairs(v.normalPai) do
							if pai_id == rsp.huPai then
								player_index = i
								pai_index = index
								break
							end
						end
					end
					if player_index and pai_index then
						break
					end
				end
			end

			if player_index and pai_index then
				table.remove(rsp.normalPai[player_index].normalPai, pai_index)
			end
		end
		-- 初始化一下摊出来的手牌
		self.roomObj.playerMgr:InitTanpai(rsp.normalPai)
		-- 通知手牌控件刷新摊牌显示
		LuaEvent.AddEventNow(EEventType.MJInitTanPaiHandCards)
		-- 处理胡的那张牌
		if not MJHuPaiType.IsMJNoneOrLiuJu(rsp.huType) then
			-- 手牌显示：加上胡的那张牌
			for i,v in ipairs(rsp.huUserIds) do
				local card_item = CMJCard.New()
				card_item:Init(rsp.huPai, 1)
				local player = self.roomObj.playerMgr:GetPlayerByPlayerID(v)
				local cardList = {card_item}
				player.cardMgr:AddCards(cardList)
				LuaEvent.AddEventNow(EEventType.MJInCard, player, card_item)

				table.insert(playerList, {player = player, huType = huType, other = nil})
			end
			-- 出牌显示：去掉别人放炮的那张牌
			if rsp.huType == MJHuPaiType.Hu and rsp.huByUserId then
				local other = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.huByUserId)
				LuaEvent.AddEventNow(EEventType.MJ_NormalOutMJCardCatch,other.seatPos,rsp.huPai)

				table.insert(playerList, {player = nil, huType = huType, other = other})
			end
		end

		if 0 ~= #playerList then
			-- 头像显示胡、放炮
			LuaEvent.AddEventNow(EEventType.ShowPlayerHuType, playerList)
		end
	end
end

--历史积分推送
function MJNormalPlayCardLogic:HandleHistoryScorePush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		-- 为了和扑克用同一个窗口，转一下数据
		local unified_data = self.UnifyHistoryScoreData(rsp)

		if unified_data then
			LuaEvent.AddEventNow(EEventType.HistoryScorePush, unified_data)
		end
	end
end

-- 流局推送
function MJNormalPlayCardLogic:HandleLiuJuPush(rsp, head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		if rsp.shaoZhuangType ~= 0 then
			local p1 = {true,true}
			LuaEvent.AddEventNow(EEventType.MJ_LiuJuShaoZhuang, p1)
		end
	end
end

-- 烧庄推送
function MJNormalPlayCardLogic:HandleShaoZhuangPush(rsp, head)
	if not self:CheckGameCoreDataLegal() then
		return
	end
	if rsp then
		if rsp.shaoZhuangType ~= 0 then
			local p1 = {false,true}
			LuaEvent.AddEventNow(EEventType.MJ_LiuJuShaoZhuang, p1)
		end
	end
end

--解散房间 - 玩家解散操作推送（同意或者拒绝）
function MJNormalPlayCardLogic:HandleNotifyDismissAskPush(rsp, head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		-- logError("vote push " .. ToString(rsp))
		LuaEvent.AddEventNow(EEventType.UI_DisbandRoom_VoteRefresh,rsp)
	end
end

-- 解散房间-提示玩家投票推送
function MJNormalPlayCardLogic:HandleNotifyDismissVotePush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		self.roomObj:SaveDismissInfo(rsp)
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "disband_room_ui_window", true , nil)
	end
end

-- 房间解散结果推送
function MJNormalPlayCardLogic:HandleNotifyDismissResultPush(rsp,head)
	if rsp then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "disband_room_ui_window", true,nil)
		-- zs TODOTODO
		-- if rsp.end then
		if true then
			-- 解散成功，置标志位，如果需要做更多事情，则加一个房间state
			self:SetRoomDismissed()
		end
	end
end

-------------------------- 网络消息发送  ----------------------------------


--登陆成功
function MJNormalPlayCardLogic:LoginSuc(bReconect,isCreate)
	self.isLoginSvrSuc = true
	-- 发送创建房间或者加入房间
	if bReconect then
		self:SendReconnect()
	else
		if isCreate then
			self:SendCreateRoom()
		else
			DwDebug.Log("self.roomId "..self.roomId)
			self:SendJoinRoom(self.roomId)
		end
	end
end


-- 心跳包
function MJNormalPlayCardLogic:SendHeartBeat()
	if not self.isLoginSvrSuc then
		DwDebug.Log("SendHeartBeat fail because svr is not ready")
		return
	end
	local body = {}
	body.userId = DataManager.GetUserID()
	SendMsg(GAME_CMD.CS_HEART_BEAT,body,function(rsp,head)

	end,function(rsp,head)
		-- 失败
		DwDebug.LogError("CS_HEART_BEAT fail")
	end,true)
end


--连接服务器成功后发送登录消息进行验证
function MJNormalPlayCardLogic:SendLoginSvr(loginAddress,loginLng,loginLat)
	if self.isStartLogining then
		return
	end

	self:EnableStartLoginingFlag(true)

	local body = {}
	body.userId = DataManager.GetUserID()
	body.accessToken = DataManager.GetToken()
	body.secretString = LuaUtil.MD5(body.userId,body.accessToken,DataManager.SecretKey)
	body.loginAddress = DataManager.GetLoginLocationStr()
	body.loginLng = WrapSys.GetLongitude()
	body.loginLat = WrapSys.GetLatitude()

	DwDebug.Log("==============SendLoginSvr body.loginAddress:", body.loginAddress, "body.loginLng:", body.loginLng, "body.loginLat:", body.loginLat)

	SendMsg(GAME_CMD.CS_LOGIN,body,function(rsp,head)
		self:EnableStartLoginingFlag(false)
		self:LoginSuc(self.isReconnect,self.isCreate)
		self.isReconnect = false
		PlayGameSys.RegisterHeartBeat()
	end,function(rsp,head)
		if nil ~= rsp.message and "" ~= rsp.message then
			WindowUtil.LuaShowTips(rsp.message)
		else
			WindowUtil.LuaShowTips("登录连接超时,请稍候重试")
		end

		self:EnableStartLoginingFlag(false)
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
function MJNormalPlayCardLogic:SendCreateRoom()
	local room_config = DataManager.GetRoomConfig()

	if not room_config then
		DwDebug.LogError("创建房间参数错误")
		LuaEvent.AddEventNow(EEventType.UI_CreateRoomResult,false)
		return
	end

	SendMsg(GAME_CMD.CS_CREATE_ROOM,room_config,function(rsp,head)
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		HallSys.isCreateOrJoinRooming = false
		--创建成功
		--开始切换到游戏场景
		GameStateMgr.GoToGameScene()
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
function MJNormalPlayCardLogic:SendJoinRoom(roomId)
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
		LoginSys.ResetLoginStatus()
		if GameStateMgr.GetCurStateType() == EGameStateType.MainCityState then
			PlayGameSys.ReleasePlayLogic()
			PlayGameSys.CloseNetWork()
		end
	end,true)
end

function MJNormalPlayCardLogic:SendReconnect()
	local body = {}
	HallSys.isCreateOrJoinRooming = false
	SendMsg(GAME_CMD.CS_NET_RECONNECT,body,function(rsp,head)
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		--开始切换到游戏场景
		if GameStateMgr.GetCurStateType() ~= EGameStateType.GameState then
			DwDebug.Log("SendReconnect GoToGameScene")
			GameStateMgr.GoToGameScene()
		else
			DwDebug.Log("SendReconnect SendUIReady")
			if GameState.isEnterCompleted then
				self:SendUIReady()
			end
		end
	end,function(rsp,head)
		if nil ~= rsp.message and "" ~= rsp.message then
			WindowUtil.LuaShowTips(rsp.message)
		else
			WindowUtil.LuaShowTips("重连房间超时,请稍候重试")
		end

		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
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
	end,true)
end

--房间不存在的错误码
function MJNormalPlayCardLogic:IsNotRoomErrno(errno)
	return false
end

-----------------------------------------网络请求------------------------------------------

--发送UI渲染就绪
function MJNormalPlayCardLogic:SendUIReady()
	self.UIIsReady = true
	local body = {}
	body.renderStatus = true
	SendMsg(GAME_CMD.CS_RENDER_UI_SUCCESS,body,nil, nil, true)
end

--发送游戏准备
function MJNormalPlayCardLogic:SendPrepare(readyType)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.readyType = readyType or 0
	SendMsg(GAME_CMD.CS_PREPARE, body, nil, nil, true)
end

-- 取消游戏准备
function MJNormalPlayCardLogic:SendUnPrepare()

end

--发送申请解散
function MJNormalPlayCardLogic:SendAskDismiss()
	local body = {}

	SendMsg(GAME_CMD.CS_ASK_DISMISS,body)
end

--发送解散投票
function MJNormalPlayCardLogic:SendDismissVote(isAgree)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.actionValue = isAgree and 1 or 2
	SendMsg(GAME_CMD.CS_PLAYER_VOTE,body)
end



--发送退出房间
function MJNormalPlayCardLogic:SendQuitRoom()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_QUIT_ROOM,body,function ()
		PlayGameSys.QuitToMainCity()
	end,function ()
		WindowUtil.LuaShowTips("退出房间失败")
	end)
end

--发送 "过"
function MJNormalPlayCardLogic:SendPass()
	local body = {}
	body.userId = DataManager.GetUserID()
	SendMsg(GAME_CMD.CS_GAMEPLAY_PASS,body,nil,nil,true)
end

--发送出牌
function MJNormalPlayCardLogic:SendPlayCard(cardID)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.pai = cardID
	SendMsg(GAME_CMD.CS_SC_GAMEPLAY_PLAY_CARD,body, nil,nil,true)
	self:HandlePlayCard(DataManager.GetUserID(), cardID)
	self.roomObj.playerMgr:PlayerWaiting(DataManager.GetUserID())
end

-- 发送胡牌
function MJNormalPlayCardLogic:SendHuPai(hu_type)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.huType = hu_type
	SendMsg(GAME_CMD.CS_SC_GAMEPLAY_HU, body,nil,nil,true)

	-- 一炮多响等待操作
	if nil ~= self.roomObj and nil ~= self.roomObj.playerMgr then
		local self_player = self.roomObj.playerMgr:GetPlayerByPlayerID(DataManager.GetUserID())
		if nil ~= self_player then
			AnimManager.PlayWaitTip(false)
			self_player:ChangeState(PlayerStateEnum.ExpectPushCard)
		end
	end
end

-- 发送杠牌
function MJNormalPlayCardLogic:SendGangPai(gangpai)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.gangPai = gangpai
	SendMsg(GAME_CMD.CS_SC_GAMEPLAY_GANG, body,nil,nil,true)
end

-- 发送碰牌
function MJNormalPlayCardLogic:SendPengPai(pengpai)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.pengPai = pengpai
	SendMsg(GAME_CMD.CS_SC_GAMEPLAY_PENG, body,nil,nil,true)
end

-- 发送吃牌
function MJNormalPlayCardLogic:SendChiPai(chipai)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.chiPai = chipai
	SendMsg(GAME_CMD.CS_SC_GAMEPLAY_CHI, body)
end

--请求历史积分
function MJNormalPlayCardLogic:SendAskHistoryScore()
	local body = {}
	SendMsg(GAME_CMD.CS_HISTORY_SCORE,body)
end

--请求玩家间距离
function MJNormalPlayCardLogic:SendAskLBS(seatId)
	local body = {}
	body.seatId = seatId
	SendMsg(GAME_CMD.CS_DISTANCE_REQ,body)
end

-- 房主请求踢人
function MJNormalPlayCardLogic:SendASKKickOut(userID, callBack)
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
------------------------------功能性函数--------------------------------
function MJNormalPlayCardLogic:MJ_PlayCaoZuoSound(seatId,caoZuoEnum)
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(seatId)
	if player then
		AudioManager.WSK_PlayCaoZuo(player.seatInfo.sex,caoZuoEnum)
	end
end

-- 清理房间
function MJNormalPlayCardLogic:CleanRoom()
	-- 清理房间显示
	self.roomObj:CleanRoom()
end

function MJNormalPlayCardLogic.UnifyHistoryScoreData(original)

	if not original or not original.recordPlyaer then
		DwDebug.LogError("请求历史数据返回空!!!")
		return nil
	end

	local unified = {items = {}}

	for i,player_record in ipairs(original.recordPlyaer) do
		if next(player_record) ~= nil then
			local unified_item = {scores = {}}
			unified_item.userId = player_record.userId

			table.sort(player_record.eachGameJifen, function(a, b) return a.gameNum < b.gameNum end)
			for round, info in ipairs(player_record.eachGameJifen) do
				table.insert(unified_item.scores, info.jifen)
			end

			table.insert(unified.items, unified_item)
		end
	end

	return unified
end
