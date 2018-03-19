--------------------------------------------------------------------------------
-- 	 File      : WSKNormalPlayCardLogic.lua
--   author    : guoliang
--   function   : 正常玩法逻辑基类
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.DWBaseModule"
require "Logic.RoomLogic.BaseObjectState"
require "Logic.CardLogic.CardTypeEnum"
require "Logic.CardLogic.CCard"
require "Logic.CardLogic.LogicContainer.WSK.CRWSKCardPlayLogicContainer"
require "Logic.RoomLogic.CPlayer"
require "Logic.RoomLogic.Room.CWSKRoom"
require "Logic.SystemLogic.BasePlayCardLogic"
require	"LuaSys.AnimManager"

WSKNormalPlayCardLogic = class("WSKNormalPlayCardLogic", BasePlayCardLogic)

function WSKNormalPlayCardLogic:ctor()
	self:BaseCtor()
	
	self.isAllOpen = true
	--是否发牌动画中
	self.isPlayDealCard = false
	--是否找朋友翻牌动画中
	self.isPlayFriendCardShow = false
	--是否摊牌中
	self.isPlayTaiPaiAction = false
end

function WSKNormalPlayCardLogic:Init()
	--初始化玩法所需PB
	--ProtoManager.InitWSKProto(Common_PlayID.chongRen_510K)
	
	self:BaseInit()
	-- 测试
	self.roomId = PlayGameSys.testRoomID
	self.isCreate = PlayGameSys.testIsCreate
	self.isReconnect = PlayGameSys.testIsReconnect
	--房间管理
	self.roomObj = CWSKRoom.New()
	self.roomObj:Init(self)
	-- 玩牌逻辑容器
	self.cardLogicContainer = CRWSKCardPlayLogicContainer.New()
	self.cardLogicContainer:Init()

	--初始化数据
	local WSKData = DataManager.WSKData
	if WSKData then
		self.isAllOpen = WSKData.isAllOpen
		--print("wsk is isAllOpen:" .. tostring(self.isAllOpen))
		self.roundNum = WSKData.roundNum
		self.payType = WSKData.payType
		--print(self.payType)
		self.msgGrouperId = WSKData.msgGrouperId
		self.msgGroupId = WSKData.msgGroupId
	end
end

function WSKNormalPlayCardLogic:Destroy()
	self:BaseDestroy()

	self.roomObj:Destroy()
	self.roomObj = nil
	self.cardLogicContainer:Destroy()
	self.cardLogicContainer = nil
end

function WSKNormalPlayCardLogic:GetType()
	return PlayLogicTypeEnum.WSK_Normal
end
-------------------------------事件消息--------------------------

function WSKNormalPlayCardLogic:RegisterEvent()
	LuaEvent.AddHandle(EEventType.ExcecuteDelayDealCardAction,self.ExcecuteDelayDealCardAction,self)
	LuaEvent.AddHandle(EEventType.Net_Common_Errno,self.HanleCommonError,self)
end
function WSKNormalPlayCardLogic:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.ExcecuteDelayDealCardAction,self.ExcecuteDelayDealCardAction,self)
	LuaEvent.RemoveHandle(EEventType.Net_Common_Errno,self.HanleCommonError,self)
end
function WSKNormalPlayCardLogic:ExcecuteDelayDealCardAction(eventId,p1,p2)
	if self.delayTaskList.notifyIsAlone then
		DelayTaskSys.ExecuteTask(self.delayTaskList.notifyIsAlone.delayTaskID)
	end
end

--检查是否在播放发牌动画，如果有，延迟的事件需要提前处理，并且停止发牌动画，重新刷新手牌
function WSKNormalPlayCardLogic:CheckDelayDealCardAction()
	if self.delayTaskList.notifyIsAlone then
		DelayTaskSys.ExecuteTask(self.delayTaskList.notifyIsAlone.delayTaskID)
		LuaEvent.AddEventNow(EEventType.SelfHandCardInit,false)
	end
end

--处理通用错误码
function WSKNormalPlayCardLogic:HanleCommonError(eventId,p1,p2)
	if p1 and p2 then
		if (p1 == Common_PlayID.chongRen_WSK and p2 == 10050) then
      		WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
    	elseif p2 == 10013 or p2 == 10313 or p2 == 10413 or 
     		p2 == 10016 or p2 == 10316 or p2 == 10416 then
	     	-- 这是房间不存在或者玩家不在任何房间内错误码,协议里写错了，所以这里直接写值
	    	PlayGameSys.QuitToMainCity()
	    end
	end
end
-------------------------------- 网络消息推送 -----------------------------------


-- 网络消息注册
function WSKNormalPlayCardLogic:RegisteNetPush()
	LuaNetWork.RegisterHandle(GAME_CMD.CS_PREPARE, self.HandlePreparePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_PLAYER_ONLINE_PUSH, self.HandlePlayerOnlinePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_START_GAMER_PUSH, self.HandleStartGamePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SEAT_INFO_PUSH, self.HandleSeatInfoPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ROOM_INFO, self.HandleRoomInfoPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_GAME_INFO_PUSH, self.HandleGameInfoPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_VOTE_PUSH, self.HandleNotifyDismissVotePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_DISMISS_RESULT_PUSH, self.HandleNotifyDismissResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_DISMISS, self.HandleRoomDismissedPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_QUIT_ROOM_PUSH, self.HandleQuitRoomPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_KICK_PUSH, self.HandleNotifyKickPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SYSTEM_DEAL_CARD_PUSH, self.HandleSystemDealCardPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_IS_ALONE_PUSH, self.HandleNotifyIsAlonePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ALONE_PLAY_PUSH, self.HandleAlonePlayPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_SHOW_CARD_PUSH, self.HandleNotifyShowCardPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SVR_SHOW_CARD_PUSH, self.HandleSvrShowCardPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_NOTIFY_PLAY_CARD_PUSH, self.HandleNotifyPlayCardPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SVR_PLAY_CARD_PUSH, self.HandleSvrPlayCardPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_CLEAR_CARDS_PUSH, self.HandleClearCardsPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SHOW_ALL_CARDS_PUSH, self.HandleShowAllCardsPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_SMALL_RESULT_PUSH, self.HandleSmallResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_BIG_RESULT_PUSH, self.HandleBigResultPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ASK_LBS_REPLY, self.HandleLBSNotify, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_HISTORY_SCORE_PUSH, self.HandleHistoryScorePush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_DISTANCE_TIPS_PUSH, self.HandleDistanceTipsPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_REPLY_NOT_ALONE_PUSH, self.HandleNotAlonePlayPush, self)
	LuaNetWork.RegisterHandle(GAME_CMD.SC_ASK_KICK_OUT_REPLY, self.HandleASKKickOutReplay, self)
end

-- 网络消息移除
function WSKNormalPlayCardLogic:UnRegisteNetPush()
	LuaNetWork.UnRegisterHandle(GAME_CMD.CS_PREPARE, self.HandlePreparePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_PLAYER_ONLINE_PUSH, self.HandlePlayerOnlinePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_START_GAMER_PUSH, self.HandleStartGamePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SEAT_INFO_PUSH, self.HandleSeatInfoPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_ROOM_INFO, self.HandleRoomInfoPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_GAME_INFO_PUSH, self.HandleGameInfoPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_VOTE_PUSH, self.HandleNotifyDismissVotePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_DISMISS_RESULT_PUSH, self.HandleNotifyDismissResultPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_DISMISS, self.HandleRoomDismissedPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_QUIT_ROOM_PUSH, self.HandleQuitRoomPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_KICK_PUSH, self.HandleNotifyKickPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SYSTEM_DEAL_CARD_PUSH, self.HandleSystemDealCardPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_IS_ALONE_PUSH, self.HandleNotifyIsAlonePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_ALONE_PLAY_PUSH, self.HandleAlonePlayPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_SHOW_CARD_PUSH, self.HandleNotifyShowCardPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SVR_SHOW_CARD_PUSH, self.HandleSvrShowCardPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_NOTIFY_PLAY_CARD_PUSH, self.HandleNotifyPlayCardPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SVR_PLAY_CARD_PUSH, self.HandleSvrPlayCardPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_CLEAR_CARDS_PUSH, self.HandleClearCardsPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SHOW_ALL_CARDS_PUSH, self.HandleShowAllCardsPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_SMALL_RESULT_PUSH, self.HandleSmallResultPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_BIG_RESULT_PUSH, self.HandleBigResultPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_ASK_LBS_REPLY, self.HandleLBSNotify, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_HISTORY_SCORE_PUSH, self.HandleHistoryScorePush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_DISTANCE_TIPS_PUSH, self.HandleDistanceTipsPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_REPLY_NOT_ALONE_PUSH, self.HandleNotAlonePlayPush, self)
	LuaNetWork.UnRegisterHandle(GAME_CMD.SC_ASK_KICK_OUT_REPLY, self.HandleASKKickOutReplay, self)
end

--房间信息推送
function WSKNormalPlayCardLogic:HandleRoomInfoPush(rsp,head)
	if rsp then
		if rsp.status ~= 4 then
		    --小局结算要清桌 不然会出现没翻牌的情况 提前到这
		    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "disband_room_ui_window", true , nil)
		end
		
		--跳过结算 又 跳过发牌 这里清理
		if rsp.status == 0 or rsp.status == 2 then
			self.roomObj:ClearSelfHandCard()
		end

    	--小局结算的时候 清桌
    	if rsp.status == 0 then
			--清理玩家手牌
			LuaEvent.AddEvent(EEventType.SelfHandCardClear,nil,nil)
		end

		self.super.super.HandleRoomInfoPush(self,rsp,head)
	end
end


--游戏信息推送
function WSKNormalPlayCardLogic:HandleGameInfoPush(rsp,head)
	self:BaseHandleGameInfoPush(rsp,head)

	--重置动画标志位
	self.isPlayDealCard = false
	self.isPlayFriendCardShow = false
	self.isPlayTaiPaiAction = false
end


-- 系统发牌推送(只有自己的)
function WSKNormalPlayCardLogic:HandleSystemDealCardPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		-- 清牌清桌
		self.roomObj:CleanRoom()
		self.roomObj:ClearSelfHandCard()
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
		if self.roomObj.playerMgr:CheckPlayerNumIsLegal(4) == false then
			error("cur player num is not enough when start game")
			WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
		end
		self.gameStarted = true
	end
end

--提示玩家是否打独推送
function WSKNormalPlayCardLogic:HandleNotifyIsAlonePush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		--房间进入叫牌打独状态
		if self.isPlayDealCard then
			local taskID = DelayTaskSys.AddTask(self.NotifyAskAloneFunc,self,rsp)
			self:AddDelayTask("notifyIsAlone",taskID,0)
		else
			self:NotifyAskAloneFunc(rsp)
		end
	end
end

function WSKNormalPlayCardLogic:NotifyAskAloneFunc(rsp)
	if self.isPlayDealCard then
		self.isPlayDealCard = false
		self:RemoveDelayTask("notifyIsAlone")
	end
	self.roomObj:ChangeState(RoomStateEnum.CallAlone)
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if player then
		self.roomObj.playerMgr:PlayerCallAloneTips(player.seatInfo.userId)
		self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
	else
		--清理工作玩家
		self.roomObj:SaveWorkingPlayerID(-1)
	end
end

--玩家打独推送
function WSKNormalPlayCardLogic:HandleAlonePlayPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		--如果有其他玩家已经打独了
		--立即执行延迟的提示打独任务
		self:CheckDelayDealCardAction()

		self.roomObj.playerMgr:PlayerWaiting(rsp.userId)
		local player = self.roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
		if player then
			self.roomObj:SetAlonePlayerID(rsp.userId)
			LuaEvent.AddEvent(EEventType.PlayerShowAlone,player.seatPos,true)
		end
		-- 声音播放
		self:WSK_PlayCaoZuoSound(rsp.seatId,WSK_CaoZuoEnum.daDu)

		LuaEvent.AddEventNow(EEventType.Alone_Play,rsp)
		--房间进入游戏状态
		self.roomObj:ChangeState(RoomStateEnum.Playing)
	end
end

--玩家不打独推送
function WSKNormalPlayCardLogic:HandleNotAlonePlayPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		-- 声音播放
		self:WSK_PlayCaoZuoSound(rsp.seatId,WSK_CaoZuoEnum.buDaDu)
	end
end

--提示亮牌推送(找朋友提示)
function WSKNormalPlayCardLogic:HandleNotifyShowCardPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		--房间进入找朋友状态
		self.roomObj:ChangeState(RoomStateEnum.FindFriend)

		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player then
			self.roomObj.playerMgr:PlayerFindFriendTips(player.seatInfo.userId)
			self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
		else
			--清理工作玩家
			self.roomObj:SaveWorkingPlayerID(-1)
		end
	end
end
-- 玩家亮牌推送(找朋友亮牌)
function WSKNormalPlayCardLogic:HandleSvrShowCardPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		--如果有其他玩家已经打独了
		--立即执行延迟的提示打独任务
		self:CheckDelayDealCardAction()

		self.roomObj.playerMgr:PlayerWaiting(rsp.userId)

		self.roomObj:SetFriendCardID(rsp.userId,rsp.friendPai)

		--房间进入游戏状态
		self.roomObj:ChangeState(RoomStateEnum.Playing)

		-- 声音播放
		self:WSK_PlayCaoZuoSound(rsp.seatId,WSK_CaoZuoEnum.ZhaoPengYou)

		self.isPlayFriendCardShow = true

		--播放翻牌动画
		LuaEvent.AddEventNow(EEventType.ShowCardFriendCardAction, true)
	end
end
--提示玩家出牌推送（服务端广播给4个人）
function WSKNormalPlayCardLogic:HandleNotifyPlayCardPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		if self.isPlayFriendCardShow then
			local taskID = DelayTaskSys.AddTask(self.NotifyPlayCardFunc,self,rsp)
			self:AddDelayTask("notifyPlayCard",taskID,1)
		else
			--local taskID = DelayTaskSys.AddTask(self.NotifyPlayCardFunc,self,rsp)
			--self:AddDelayTask("notifyPlayCard",taskID,1)
			self:NotifyPlayCardFunc(rsp)
		end
	end
end

--出牌提示功能函数
function WSKNormalPlayCardLogic:NotifyPlayCardFunc(rsp)
	if self.roomObj then
		if rsp then
			if self.isPlayFriendCardShow then
				-- 朋友亮牌显示
				local friendPai = self.roomObj.friendCardID
				LuaEvent.AddEventNow(EEventType.ShowCardFriendCardAction,false)
				LuaEvent.AddEventNow(EEventType.ShowCardFriendCardDismiss,true,friendPai)

				local taskID = DelayTaskSys.AddTask(self.ShowFriendCardDismissEndFunc,self,rsp)
				self:AddDelayTask("ShowFriendCardDismissEnd",taskID,2)

				self.isPlayFriendCardShow = false
			else
				self.roomObj.canOutCard = true
				local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
				if player then
					-- 如果是自己出牌，并且不是必须出牌 ，需要提前计算出牌提示AI结果
					if not rsp.isForce and DataManager.GetUserID() == player.seatInfo.userId  then
						local lastOutCardInfo = self.cardLogicContainer:GetLastOutCardInfo()
						if lastOutCardInfo then
							local srcCardIds = lastOutCardInfo.pai
							local srcCards = {}
							for k,v in pairs(srcCardIds) do
								cardItem = CCard.New()
								cardItem:Init(v,k)
								table.insert(srcCards,cardItem)
							end
							local handCards = player.cardMgr:GetHandCards()
							local introduceCount = self.cardLogicContainer:CreateAIResult(handCards,srcCards)
							DwDebug.Log("NotifyPlayCardFunc introduceCount = "..introduceCount)
							if introduceCount <= 0 then
								WindowUtil.LuaShowTips("当前找不到可以大过上家的牌")
								self.roomObj.canOutCard = false
							end
						end
					end
					--玩家状态转换
					self.roomObj.playerMgr:PlayerOutCardTips(player.seatInfo.userId,rsp.token,rsp.isForce)
					self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
				end
			end
			
		end
	end
end



-- 找朋友亮牌动画结束回调函数
function WSKNormalPlayCardLogic:ShowFriendCardDismissEndFunc(rsp)
	if rsp then
		local friendPai = self.roomObj.friendCardID
		LuaEvent.AddEvent(EEventType.PlayerShowFriendCard,friendPai,true)
		-- 如果必须出牌，意味着新的一轮开始
		if rsp.isForce then
			LuaEvent.AddEventNow(EEventType.ClearDesk,nil,nil)
		end

		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player then
			LuaEvent.AddEventNow(EEventType.PlayerShowZhuangJia, player.seatPos, true)

			self.roomObj.playerMgr:PlayerOutCardTips(player.seatInfo.userId,rsp.token,rsp.isForce)
			self.roomObj:SaveWorkingPlayerID(player.seatInfo.userId)
		end
	end
end

-- 玩家出牌推送
function WSKNormalPlayCardLogic:HandleSvrPlayCardPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
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
					LuaEvent.AddEventNow(EEventType.PlayerShowFriend,friendPlayer.seatPos,true)
				end
			end

			-- 几游显示
			if rsp.overOrder > 0 then
				if player then
					--self:WSK_PlayCaoZuoSound(rsp.seatId,WSK_CaoZuoEnum.woDaWanLe)  实际是我快打完了 不能用在这
					-- 已经出完，需要清理显示的手牌数
					LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards,player.seatPos,0)

					local taskID = DelayTaskSys.AddTask(self.DelayShowSortFunc,self,{seatPos = player.seatPos,sort = rsp.overOrder})
					self:AddDelayTask("DelayShowSort",taskID,1)
					--LuaEvent.AddEventNow(EEventType.PlayerShowSort,player.seatPos,rsp.overOrder)
				end
			end

			-- 剩余手牌
			if rsp.restNum > 0 then
				if player then
					LuaEvent.AddEventNow(EEventType.PlayerShowRemainHandCards,player.seatPos,rsp.restNum)
					if not player.playRestNumSound then
						player.playRestNumSound = true
						self:WSK_PlayCaoZuoSound(rsp.seatId,WSK_CaoZuoEnum.woPaiBuDuoLe)
					end
				end
			end
			if rsp.userId ~= DataManager.GetUserID() then
				AudioManager.PlayCommonSound(UIAudioEnum.chuPai)
				AudioManager.WSK_PlayBaoPai(rsp)
				AnimManager.PlayCardClassAnim(rsp, PlayGameSys.GetNowPlayId())
			end
		else
			self:WSK_PlayCaoZuoSound(rsp.seatId,WSK_CaoZuoEnum.buChu)
		end
		--消息通知（玩家手牌消失，出牌展示，桌面积分刷新）
		LuaEvent.AddEventNow(EEventType.PlayOutCardAction,rsp,false)

	end
end

function WSKNormalPlayCardLogic:DelayShowSortFunc(data)
	if data then
		LuaEvent.AddEventNow(EEventType.PlayerShowSort,data.seatPos,data.sort)
	end
end

--清牌推送（一轮打完后，由服务端清理桌面牌，结算分数）
function WSKNormalPlayCardLogic:HandleClearCardsPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		local player = self.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player then
			--消息通知(本轮手牌积分结算)
			if rsp.desktopScore > 0 then
				self.roomObj.pointMgr:AddPlayPoint(player.seatPos,rsp.desktopScore, true)
			end
		end

		--清除桌面积分显示
		self.roomObj.curDeskScore = 0
		LuaEvent.AddEventNow(EEventType.RefreshCurTablePoint,self.roomObj.curDeskScore)

		local taskID = DelayTaskSys.AddTask(self.ClearCardsPushFunc,self,rsp)
		self:AddDelayTask("ClearCardsPush",taskID,0.3)
	end
end

function WSKNormalPlayCardLogic:ClearCardsPushFunc(rsp)
	-- error("ClearCardsPushFunc")
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

--摊牌推送
function WSKNormalPlayCardLogic:HandleShowAllCardsPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		-- LuaEvent.AddEventNow(EEventType.SelfHandCardClear)
		-- --清桌
		-- LuaEvent.AddEventNow(EEventType.ClearDesk,nil,nil)
		
		-- local player
		-- for k,v in pairs(rsp.normalPai) do
		-- 	player = self.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
		-- 	if player then
		-- 		LuaEvent.AddEventNow(EEventType.TanPaiAction,player.seatPos,v.normalPai)
		-- 	end
		-- end
		-- self.isPlayTaiPaiAction = true
		local taskID = DelayTaskSys.AddTask(self.HandleShowAllCardsFunc,self,rsp)
		self:AddDelayTask("ShowAllCardsPush",taskID,1)
	end
end

function WSKNormalPlayCardLogic:HandleShowAllCardsFunc(rsp)
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

--小局结算推送
function WSKNormalPlayCardLogic:HandleSmallResultPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		self:BaseHandleSmallResultPush(rsp, head)
		if rsp.now >= rsp.total then
			self.isAllRoundEnd = true
		end
		if self.isPlayTaiPaiAction then
			local taskID = DelayTaskSys.AddTask(self.SmallResultFunc,self,rsp)
			self:AddDelayTask("SmallResultPush",taskID,4)
		else
			local taskID = DelayTaskSys.AddTask(self.SmallResultFunc,self,rsp)
			self:AddDelayTask("SmallResultPush",taskID,2)
		end
		--小结算的时候清理
		self.roomObj:ClearSelfHandCard()
	end
end
--小局结算处理函数
function WSKNormalPlayCardLogic:SmallResultFunc(rsp)
	if self.roomObj then
		if self.isPlayTaiPaiAction then
			self.isPlayTaiPaiAction = false
		end
		-- 检查是否双围
		for k,v in pairs(rsp.players) do
			if v.isDouble then
				local player = self.roomObj.playerMgr:GetPlayerByPlayerID(v.userId)
				if player then
					AudioManager.WSK_PlayCaoZuo(player.seatInfo.sex,WSK_CaoZuoEnum.shuangWei)
					--刷新玩家总积分
					player.seatInfo.totalJiFen = v.totalScore
				end
				break
			end
		end
		self.roomObj:AddSmallResult(rsp.now,rsp)
		-- 清理当前桌面分
		LuaEvent.AddEventNow(EEventType.RefreshCurTablePoint,0)
		self.roomObj:UpdateCurrentRound(rsp.next)
		self.roomObj:ChangeState(RoomStateEnum.SmallRoundOver)
	end
end



-------------------------- 网络消息发送  ----------------------------------
--登陆成功
function WSKNormalPlayCardLogic:LoginSuc(bReconect,isCreate)
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
function WSKNormalPlayCardLogic:SendHeartBeat()
	if not self.isLoginSvrSuc then
		return
	end
	--error("CS_HEART_BEAT send time = "..WrapSys.GetCurrentDateTime())
	local body = {}
	body.userId = DataManager.GetUserID()
	SendMsg(GAME_CMD.CS_HEART_BEAT,body,function(rsp,head) 
		-- error("recv CS_HEART_BEAT sucess seq = "..head.seq .."time = "..WrapSys.GetCurrentDateTime())
	end,function(rsp,head)
		-- 失败
		DwDebug.LogError("recv CS_HEART_BEAT fail seq = "..head.seq .."time = "..WrapSys.GetCurrentDateTime())
	end,true)
end


--连接服务器成功后发送登录消息进行验证
function WSKNormalPlayCardLogic:SendLoginSvr(loginAddress,loginLng,loginLat)
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
function WSKNormalPlayCardLogic:SendCreateRoom()
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
		HallSys.isCreateOrJoinRooming = false
	end,function(rsp,head)
		if nil ~= rsp.message and "" ~= rsp.message then
			WindowUtil.LuaShowTips(rsp.message)
		else
			WindowUtil.LuaShowTips("创建房间超时,请稍候重试")
		end

		HallSys.isCreateOrJoinRooming = false
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		LuaEvent.AddEventNow(EEventType.UI_CreateRoomResult,false)
		PlayGameSys.ReleasePlayLogic()
		PlayGameSys.CloseNetWork()
	end,true)
end
-- 加入房间
function WSKNormalPlayCardLogic:SendJoinRoom(roomId)
	local  body = {}
	body.roomId = roomId
	SendMsg(GAME_CMD.CS_JOIN_ROOM,body,function(rsp,head)
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		--开始切换到游戏场景
		GameStateMgr.GoToGameScene()
		HallSys.isCreateOrJoinRooming = false
	end,function(rsp,head)
		if nil ~= rsp.message and "" ~= rsp.message then
			WindowUtil.LuaShowTips(rsp.message)
		else
			WindowUtil.LuaShowTips("加入房间超时,请稍候重试")
		end

		LuaEvent.AddEventNow(EEventType.ShowWaitWindow,false)
		PlayGameSys.ReleasePlayLogic()
		PlayGameSys.CloseNetWork()
		HallSys.isCreateOrJoinRooming = false
	end,true)
end

function WSKNormalPlayCardLogic:SendReconnect()
	self.reconnectGameInfo = false
	HallSys.isCreateOrJoinRooming = false

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
function WSKNormalPlayCardLogic:IsNotRoomErrno(errno)
	return false
end

--发送UI渲染就绪
function WSKNormalPlayCardLogic:SendUIReady()
	self.UIIsReady = true
	local body = {}
	body.renderStatus = true
	SendMsg(GAME_CMD.CS_RENDER_UI_SUCCESS,body,nil, nil, true)
end

--发送游戏准备
function WSKNormalPlayCardLogic:SendPrepare(readyType)
	local body = {}
	body.userId = DataManager.GetUserID()
	body.readyType = readyType or 0
	SendMsg(GAME_CMD.CS_PREPARE, body, nil, nil, true)
end

--发送申请解散
function WSKNormalPlayCardLogic:SendAskDismiss()
	local body = {}

	SendMsg(GAME_CMD.CS_ASK_DISMISS,body)
end

--发送解散投票
function WSKNormalPlayCardLogic:SendDismissVote(isAgree)
	local body = {}
	body.isAgree = isAgree
	SendMsg(GAME_CMD.CS_PLAYER_VOTE,body)
end

--发送退出房间
function WSKNormalPlayCardLogic:SendQuitRoom()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_QUIT_ROOM,body,function (rsp,head)
		PlayGameSys.QuitToMainCity()
	end,function ()
		WindowUtil.LuaShowTips("退出房间失败")
	end)
end

--发送是否打独
function WSKNormalPlayCardLogic:SendReplyIsAlone(isAlone)
	local body = {}
	body.isAlone = isAlone
	SendMsg(GAME_CMD.CS_REPLY_ALONE,body)
	self.roomObj.playerMgr:PlayerWaiting(DataManager.GetUserID())
end

--发送找朋友亮牌
function WSKNormalPlayCardLogic:SendFindFriendCard(cardID)
	local body = {}
	body.friendPai = cardID
	SendMsg(GAME_CMD.CS_PLAYER_SHOW_CARD,body)
end

--发送出牌
function WSKNormalPlayCardLogic:SendOutCard(cardIDs,token)
	if token then
		local body = {}
		body.token = token
		body.pai = cardIDs
		SendMsg(GAME_CMD.CS_PLAYER_PLAY_CARD, body, nil, nil, true)
	end
end

--发送不出牌
function WSKNormalPlayCardLogic:SendNoOutCard(token)
	if token then
		local body = {}
		body.token = token
		SendMsg(GAME_CMD.CS_PLAYER_NOT_PLAY,body,nil, nil, true)
		self.roomObj.playerMgr:PlayerWaiting(DataManager.GetUserID())
	end
end

--请求LBS
function WSKNormalPlayCardLogic:SendAskLBS()
	local body = {}
	SendMsg(GAME_CMD.CS_ASK_LBS,body)
end

--请求历史积分
function WSKNormalPlayCardLogic:SendAskHistoryScore()
	local body = {}
	SendMsg(GAME_CMD.CS_HISTORY_SCORE,body)
end

-- 房主请求踢人
function WSKNormalPlayCardLogic:SendASKKickOut(userID, callBack)
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

--播放声音
function WSKNormalPlayCardLogic:WSK_PlayCaoZuoSound(seatId,caoZuoEnum)
	local player = self.roomObj.playerMgr:GetPlayerBySeatID(seatId)
	if player then
		AudioManager.WSK_PlayCaoZuo(player.seatInfo.sex,caoZuoEnum)
	end
end

--检查自己是否打独
function WSKNormalPlayCardLogic:CheckMyIsAlone()
	if self.roomObj then
		--DwDebug.LogError("self.roomObj.alonePlayerID : " .. self.roomObj.alonePlayerID)
		return self.roomObj.alonePlayerID == DataManager.GetUserID()
	end
	return false
end

--获取当前手牌类型结果
function WSKNormalPlayCardLogic:GetHandCardStruct(handCards)
	self.analysisStruct = BaseWSKCardAnalysisStruct:New()
	self.analysisStruct:Analysis(handCards)

	return self.analysisStruct
end


