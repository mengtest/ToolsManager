--------------------------------------------------------------------------------
--   File      : ZhaJInHuaNormalLogic.lua
--   author    : jianing
--   function   :扎金花正常玩法逻辑基类
--   date      : 2018-02-01
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Logic.DWBaseModule"
require "Logic.RoomLogic.BaseObjectState"
require "Logic.CardLogic.CardTypeEnum"
require "Logic.CardLogic.CCard"
require "Logic.RoomLogic.CPlayer"
require "Logic.RoomLogic.Room.CThirtyTwoRoom"
require "Logic.SystemLogic.BasePlayCardLogic"
require "LuaSys.AnimManager"

ZhaJInHuaNormalLogic = class("ZhaJInHuaNormalLogic", BasePlayCardLogic)

--剩余牌数

function ZhaJInHuaNormalLogic:ctor()
	self:BaseCtor()
end

function ZhaJInHuaNormalLogic:Init()

	ProtoManager.InitThirtyTwoProto(Common_PlayID.ThirtyTwo)
	self:BaseInit()
	--房间管理
	self.roomObj = CThirtyTwoRoom.New()
	self.roomObj:Init(self)
end

function ZhaJInHuaNormalLogic:Destroy()
	self:BaseDestroy()
	self.roomObj:Destroy()
	self.roomObj = nil
end

function ZhaJInHuaNormalLogic:GetType()
	return PlayLogicTypeEnum.ThirtyTwo_Normal
end
-------------------------------事件消息--------------------------

function ZhaJInHuaNormalLogic:RegisterEvent()
	LuaEvent.AddHandle(EEventType.Net_Common_Errno,self.HanleCommonError,self)
end
function ZhaJInHuaNormalLogic:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.Net_Common_Errno,self.HanleCommonError,self)
end

--处理通用错误码
function ZhaJInHuaNormalLogic:HanleCommonError(eventId,p1,p2)
	-- if p1 and p2 then
	-- 	if p2 == 10613 or p2 == 10616 then
	--     	PlayGameSys.QuitToMainCity()
	-- 	elseif p2 == 10640 or p2 == 10641 
	-- 		or 10643 or p2 == 10644 or p2 == 10645 then
	-- 		WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
	-- 	end
	-- end
end

-------------------------------- 网络消息推送 -----------------------------------

-- 网络消息注册
function ZhaJInHuaNormalLogic:RegisteNetPush()
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
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SYSTEM_DEAL_CARD_PUSH, self.HandleSystemDealCardPush, self, 4.3)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SMALL_RESULT_PUSH, self.HandleSmallResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_BIG_RESULT_PUSH, self.HandleBigResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ASK_LBS_REPLY, self.HandleLBSNotify, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_HISTORY_SCORE_PUSH, self.HandleHistoryScorePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_DISTANCE_TIPS_PUSH, self.HandleDistanceTipsPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ASK_KICK_OUT_REPLY, self.HandleASKKickOutReplay, self)

	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_HINT_BET, self.HandleHintBetPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_USE_BET_PUSH, self.HandleUseBetPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_USE_OPEN_PUSH, self.HandleUseOpenPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_HINT_SHUFFLE, self.HandleHintShufflePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_USE_SHUFFLE, self.HandleUseShufflePush, self, 2)
end

-- 网络消息移除
function ZhaJInHuaNormalLogic:UnRegisteNetPush()
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

	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_HINT_BET, self.HandleHintBetPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_USE_BET_PUSH, self.HandleUseBetPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_USE_OPEN_PUSH, self.HandleUseOpenPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_HINT_SHUFFLE, self.HandleHintShufflePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_USE_SHUFFLE, self.HandleUseShufflePush, self)
end

-- 大结算推送
function ZhaJInHuaNormalLogic:HandleBigResultPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		PlayGameSys.QuitToMainCity()
		return
	end

	if rsp then
		self.roomObj.playerMgr:AllPlayerWaiting()
		self.roomObj:SaveBigResult(rsp)
		-- 终止网络
		--PlayGameSys.CloseNetWork()

		if self:IsRoomDismissed() or not self.m_showingSmallResult or self.m_jumpShowTotalSettlement then
			self.roomObj:ChangeState(RoomStateEnum.GameOver)
		end
	end
end

--房间信息推送
function ZhaJInHuaNormalLogic:HandleRoomInfoPush(rsp,head)
	if not rsp then
		return
	end

	--判断一下房间状态 由于下面没有刷新gameinfo 这里不清桌了
	if rsp.status ~= 4 then 
		--小局结算要清桌 不然会出现没翻牌的情况 提前到这
		self.roomObj:CleanRoom()
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "disband_room_ui_window", true , nil)
	end

	local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.fangZhu)

	if rsp.isShuffle and rsp.shuffleStatus 
	and rsp.shuffleStatus == 0 
	and rsp.fangZhu == DataManager.GetUserID() then --在选择洗牌的阶段
		if player and player.seatInfo and not player.seatInfo.readyStatus then
			LuaEvent.AddEventNow(EEventType.PK32ShowShuffleBtn,true)
		end
		
	end

	self.super.HandleRoomInfoPush(self,rsp,head)

	--设置庄家（就是房主 不再修改了）
	if player then
		LuaEvent.AddEvent(EEventType.PlayerShowZhuangJia,player.seatPos,true)
	end

	--当前局数大于1
	if rsp.currentGamepos > 1 then
		--刷新窗口显示状态
		LuaEvent.AddEventNow(EEventType.RefreshPlayWindowStatus,true)
	end
end

--游戏信息推送
function ZhaJInHuaNormalLogic:HandleGameInfoPush(rsp,head)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end

	--判断一下状态 
	--准备阶段 解散 也会推送gameinfo 状态会乱
	--这里判断下 如果在游戏中 也没关系 选择完 会重新拉取
	if self.roomObj.roomInfo ~= nil and self.roomObj.roomInfo.status == 4 then 
		return
	end
	
	self.roomObj:ChangeState(RoomStateEnum.Playing)

	self.bankerSeatId = rsp.bankerSeatId --需要房主位置id 先这么传进去
	self:BaseHandleGameInfoPush(rsp,head)

	self.remain = rsp.remain
	LuaEvent.AddEvent(EEventType.ShowRemainCardNum,true,rsp.remain)
end

-- 系统发牌推送(只有自己的)
function ZhaJInHuaNormalLogic:HandleSystemDealCardPush(rsp,head)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end

	self.oldRemain = self.remain
	LuaEvent.AddEventNow(EEventType.PK32FanPai,rsp.pos)

	local taskID = DelayTaskSys.AddTask(self.FaPai,self,rsp)
	self:AddDelayTask("FaPai",taskID,3)
	local taskID2 = DelayTaskSys.AddTask(self.FaPaiEnd,self,rsp)
	self:AddDelayTask("FaPaiEnd",taskID2,4)

	LuaEvent.AddEventNow(EEventType.PK32HideCountDown)
end

-- 玩家准备推送
function ZhaJInHuaNormalLogic:HandlePreparePush(rsp,head)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end

	self.super.HandlePreparePush(self,rsp,head)
	if rsp and rsp.readyType == 0 and rsp.userId == DataManager.GetUserID() then --准备完 关闭洗牌按钮
		LuaEvent.AddEventNow(EEventType.PK32ShowShuffleBtn,false)
	end
end

--提示押注(开始游戏的第一条信息)
function ZhaJInHuaNormalLogic:HandleHintBetPush(rsp)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end
	self.gameStarted = true
	
	--房间进入游戏状态
	self.roomObj:CleanRoom()
	self.roomObj:ChangeState(RoomStateEnum.Playing)

	if rsp.round then
		LuaEvent.AddEventNow(EEventType.RefreshRoomRoundNum,rsp.round,self.roomObj.roomInfo.totalGameNum)
	end

	--开局前检查人数是否合法
	if self.roomObj.roomInfo == nil or
	 self.roomObj.playerMgr:CheckPlayerNumIsLegal(self.roomObj.roomInfo.playerNum) == false then
	 error("cur player num is not enough when start game")
	 WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
	end

	local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.bankerSeatId)


end

--广播玩家押注
function ZhaJInHuaNormalLogic:HandleUseBetPush(rsp)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end

	--自己下注按钮
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if player and player.seatInfo and player.seatInfo.userId == DataManager.GetUserID() then
		LuaEvent.AddEvent(EEventType.ThirtyTwo_ShowBetBtns,false)
	end

	local bteNumTable = {}
	bteNumTable.seatId = rsp.seatId
	bteNumTable.score = rsp.score
	--显示押注分
	LuaEvent.AddEvent(EEventType.SetBetNum,bteNumTable,true)
end

--广播玩家开牌
function ZhaJInHuaNormalLogic:HandleUseOpenPush(rsp)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end

	-- 自己显示开牌按钮
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if player and player.seatInfo and player.seatInfo.userId == DataManager.GetUserID() then
		LuaEvent.AddEventNow(EEventType.PK32HideKaiPaiBtn)
	end
	
	LuaEvent.AddEvent(EEventType.ThirtyTwo_OpenCards,rsp)
end

--提示洗牌(开始游戏的第一条信息 优先于下注) 不符合流程 弃用
function ZhaJInHuaNormalLogic:HandleHintShufflePush(rsp)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end


	self.roomObj:ChangeState(RoomStateEnum.Playing)
	self.roomObj:CleanRoom()

	local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.bankerSeatId)
	if player and player.seatInfo.userId == DataManager.GetUserID() then
		LuaEvent.AddEvent(EEventType.PK32ShowShuffleBtn,true)
	end
end

--小局结算推送
function ZhaJInHuaNormalLogic:HandleSmallResultPush(rsp,head)
	if not self:CheckRoomInfoLegal() or not rsp then
		return
	end

	--解散房间了 不显示按钮
	if not self:IsRoomDismissed() then
		--要洗牌 显示洗牌按钮
		if rsp.isShuffle then
			local player = self.roomObj.playerMgr:GetPlayerBySeatID(self.bankerSeatId)
			if player and player.seatInfo.userId == DataManager.GetUserID() then
				LuaEvent.AddEvent(EEventType.PK32ShowShuffleBtn,true)
			end
		end
		LuaEvent.AddEventNow(EEventType.PK32ShowPrepareBtn, true)
	end

	self.roomObj.playerMgr:AllPlayerWaiting()
	self.roomObj:AddSmallResult(rsp.now,rsp)
	self.roomObj:ChangeState(RoomStateEnum.SmallRoundOver)
	self:BaseHandleSmallResultPush(rsp)

	LuaEvent.AddEvent(EEventType.PK32HideCountDown)
end

-------------------------- 网络消息发送  ----------------------------------
--登陆成功
function ZhaJInHuaNormalLogic:LoginSuc(bReconect,isCreate)
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
function ZhaJInHuaNormalLogic:SendHeartBeat()
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
function ZhaJInHuaNormalLogic:SendLoginSvr(loginAddress,loginLng,loginLat)
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
function ZhaJInHuaNormalLogic:SendCreateRoom()
	local room_config = DataManager.GetRoomConfig()
	if not room_config then
		DwDebug.LogError("创建房间参数错误")
		LuaEvent.AddEventNow(EEventType.UI_CreateRoomResult,false)
		return
	end

	SendMsg(GAME_CMD.CS_CREATE_ROOM,room_config,function(rsp,head)
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		--创建成功
		--开始切换到游戏场景
		GameStateMgr.GoToGameScene()
		--WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
	end,function(rsp,head)
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
function ZhaJInHuaNormalLogic:SendJoinRoom(roomId)
	local  body = {}
	body.roomId = roomId
	SendMsg(GAME_CMD.CS_JOIN_ROOM,body,function(rsp,head)
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		--开始切换到游戏场景
		GameStateMgr.GoToGameScene()
	end,function(rsp,head)
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

--重连
function ZhaJInHuaNormalLogic:SendReconnect()
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
	end)
end

--房间不存在的错误码
function ZhaJInHuaNormalLogic:IsNotRoomErrno(errno)
	return errno == 10613
end

--发送UI渲染就绪
function ZhaJInHuaNormalLogic:SendUIReady()
	self.UIIsReady = true
	local body = {}
	body.renderStatus = true
	SendMsg(GAME_CMD.CS_RENDER_UI_SUCCESS,body)
end

--发送游戏准备
function ZhaJInHuaNormalLogic:SendPrepare(readyType)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.readyType = readyType or 0
	SendMsg(GAME_CMD.CS_PREPARE, body, nil, nil, true)
end

--发送申请解散
function ZhaJInHuaNormalLogic:SendAskDismiss()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_DISMISS,body)
end

--发送解散投票
function ZhaJInHuaNormalLogic:SendDismissVote(isAgree)
	local body = {}
	body.isAgree = isAgree
	SendMsg(GAME_CMD.CS_PLAYER_VOTE,body)
end

--发送退出房间
function ZhaJInHuaNormalLogic:SendQuitRoom()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_QUIT_ROOM,body,function (rsp,head)
		PlayGameSys.QuitToMainCity()
	end,function ()
		WindowUtil.LuaShowTips("退出房间失败")
	end)
end

--请求LBS
function ZhaJInHuaNormalLogic:SendAskLBS()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_LBS,body)
end

--请求历史积分
function ZhaJInHuaNormalLogic:SendAskHistoryScore()
	local body = {}
	SendMsg(GAME_CMD.CS_HISTORY_SCORE,body)
end

-- 房主请求踢人
function ZhaJInHuaNormalLogic:SendASKKickOut(userID, callBack)
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

--玩家押注
function ZhaJInHuaNormalLogic:SendASKBet(score)
	local body = {}
	body.score = score
	SendMsg(GAME_CMD.CS_ASK_BET, body,nil,nil,true)
end

--玩家开牌
function ZhaJInHuaNormalLogic:SendASKOpen()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_OPEN, body,nil,nil,true)
end

--庄家洗牌
function ZhaJInHuaNormalLogic:SendASKShuffle(isShuffle)
	local body = {}
	body.isShuffle = isShuffle
	SendMsg(GAME_CMD.CS_ASK_SHUFFLE, body,nil,nil,true)
end

--获取剩余牌数
function ZhaJInHuaNormalLogic:GetRemain(isOld)
	if isOld then
		return self.oldRemain
	else
		return self.remain
	end
end

return ZhaJInHuaNormalLogic