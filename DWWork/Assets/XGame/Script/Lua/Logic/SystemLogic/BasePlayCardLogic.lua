--------------------------------------------------------------------------------
-- 	 File      : BasePlayCardLogic.lua
--   author    : guoliang
--   function   : 玩法逻辑基类（公共基础函数，由于服务端不同玩法的proto完全独立，协议无法共用）
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "NetWork.IntervalTaskQueue"

BasePlayCardLogic = class("BasePlayCardLogic", nil)

function BasePlayCardLogic:ctor()
	--构造函数
end

function BasePlayCardLogic:BaseCtor()
	self.roundNum = 8
	self.payType = 1
	self.UIIsReady = false
	self.msgGrouperId = ""
	self.msgGroupId = ""
	self.isAllRoundEnd = false -- 所有轮结束 （收到最后一局小结算置位，考虑到小结算处理会延迟）
	self.gameStarted = false
	self.m_intervalTaskQueue = IntervalTaskQueue.New(self, LuaNetWork.RegisterHandle, LuaNetWork.UnRegisterHandle)
	--是否开始游戏登陆（如果同时出现两次发送，服务端会cut掉连接，而且重连时客户端第二次登陆回调会走加入房间，失败后整个逻辑停止）
	self.isStartLogining = false
end

function BasePlayCardLogic:BaseInit()
	--延迟任务列表
	self.delayTaskList = {}
	--注册事件消息回调
	self:RegisterEvent()
	--注册网络消息回调
	self:RegisteNetPush()

	self.isLoginSvrSuc = false
	--游戏信息数据不合法次数
	self.GameInfoIllegalTime = 0
	--玩家数据不合法次数
	self.playNumIllegalTime = 0
	-- 房间信息数据不合法次数
	self.roomInfoIllegalTime = 0

	--注册定时器
	UpdateBeat:Add(self.Update,self)
end

function BasePlayCardLogic:EnableStartLoginingFlag(isStart)
	self.isStartLogining = isStart
end

-- 添加逻辑延迟任务  delayTask-延迟任务  delayTime 延迟时间，0表示任务挂起
function BasePlayCardLogic:AddDelayTask(taskName,delayTaskID,delayTime)
	if self.delayTaskList[taskName] then
		DelayTaskSys.RemoveTask(self.delayTaskList[taskName].delayTaskID)
	end
	if delayTime == nil then
		delayTime = 0
	end
	self.delayTaskList[taskName] = {delayTaskID = delayTaskID,delayTime = delayTime,startTime = 0}
end

function BasePlayCardLogic:UpdateDelayTask()
	local removeList= {}
	 for k,v in pairs(self.delayTaskList) do
 		if v.delayTime > 0 then
			v.startTime = v.startTime + UnityEngine.Time.deltaTime
			if v.startTime >= v.delayTime then
				DelayTaskSys.ExecuteTask(v.delayTaskID)
				removeList[k] = v.delayTaskID
			end
		end
	end
	for k,v in pairs(removeList) do
		self.delayTaskList[k] = nil
	end
end

function BasePlayCardLogic:RemoveDelayTask(taskName)
	if self.delayTaskList[taskName] then
		DelayTaskSys.RemoveTask(self.delayTaskList[taskName].delayTaskID)
		self.delayTaskList[taskName] = nil
	end
end

function BasePlayCardLogic:ClearUnWorkTasks()
	for k,v in pairs(self.delayTaskList) do
		DelayTaskSys.RemoveTask(v.delayTaskID)
	end
	self.delayTaskList = {}
end

function BasePlayCardLogic:BaseDestroy()
	UpdateBeat:Remove(self.Update,self)
	self:ClearUnWorkTasks()
	self:UnRegisterEvent()
	self:UnRegisteNetPush()
	if self.m_intervalTaskQueue then
		self.m_intervalTaskQueue:Destroy()
		self.m_intervalTaskQueue = nil
	end
end

function BasePlayCardLogic:GetType()
	return PlayLogicTypeEnum.None
end

function BasePlayCardLogic:Update()
	self:UpdateDelayTask()
end

function BasePlayCardLogic:GetRoomBaseInfoCopyStr()
	local result = ""
	local gameType = Common_PlayText[PlayGameSys.GetNowPlayId()]
	if gameType == nil then
		return result
	end

	result = result .. "【" .. gameType.name .. "】" .. "房间号：" .. DataManager.GetRoomID() .. "，" .. self.roomObj:GetAllRoundNum() .. "局，"
	local playerNum = self.roomObj:GetRoomPlayerCount()
	local roomTotalNum = self.roomObj:GetRoomMaxCount()
	local numStr = tostring(playerNum) .. "缺" .. (roomTotalNum - playerNum)

	result = result .. numStr .. "，"

	local roomDes = DataManager.GetRoomDes()
	result = result .. string.gsub(roomDes, " ", "，")

	return result
end

-------------------------------事件消息--------------------------

function BasePlayCardLogic:RegisterEvent()

end

function BasePlayCardLogic:UnRegisterEvent()

end

-------------------------------------网络消息毁掉处理区 -----------------------------------------------
function BasePlayCardLogic:RegisteNetPush()
end

function BasePlayCardLogic:UnRegisteNetPush()
end

function BasePlayCardLogic:RegisteHandleToIntervalTaskQueue(cmd, func, obj, interval)
	if self.m_intervalTaskQueue then
		self.m_intervalTaskQueue:RegisteHandle(cmd, func, obj, interval)
	else
		LuaNetWork.RegisterHandle(cmd, func, obj)
	end
end

function BasePlayCardLogic:UnRegisteHandleFromIntervalTaskQueue(cmd, func, obj)
	if self.m_intervalTaskQueue then
		self.m_intervalTaskQueue:UnRegisteHandle(cmd, func, obj)
	else
		LuaNetWork.UnRegisterHandle(cmd, func, obj)
	end
end

function BasePlayCardLogic:ClearIntervalTaskQueue()
	if self.m_intervalTaskQueue then
		self.m_intervalTaskQueue:Clear()
	end
end

-- 玩家准备推送
function BasePlayCardLogic:HandlePreparePush(rsp,head)

	if rsp then
		if rsp.readyType == 0 then
			self.roomObj.playerMgr:PlayerPrepared(rsp.userId)
		else
			self.roomObj.playerMgr:PlayerIdle(rsp.userId)
		end
	end
end

-- 玩家上下线推送
function BasePlayCardLogic:HandlePlayerOnlinePush(rsp,head)
	if rsp then
		self.roomObj.playerMgr:PlayerOnlineOrOffline(rsp.userId,rsp.onlineStatus)
	end
end
-- 开始游戏推送(暂时不用)
function BasePlayCardLogic:HandleStartGamePush(rsp,head)

end

--座位信息推送(自己登陆，其他玩家后加入，玩家上线会推送)
function BasePlayCardLogic:HandleSeatInfoPush(rsp,head)
	if rsp then
		if self.UIIsReady and self.roomObj.roomInfo and rsp.userId ~= DataManager.GetUserID() then -- UI窗口已经打开且房间信息已经下发直接初始化玩家
			self.roomObj:CreateSinglePlayerQuick(rsp)
		else
			self.roomObj.playerMgr:AddPlayer(rsp)
		end
	end
end

--同时检查两项重要数据合法性
function BasePlayCardLogic:CheckGameCoreDataLegal()
	local roomInfoLegal = self:CheckRoomInfoLegal()
	local gameInfoLegal = self:CheckGameInfoLegal()
	return roomInfoLegal and gameInfoLegal
end

--检查游戏数据是否合法
function BasePlayCardLogic:CheckGameInfoLegal()
	local isLegal = (self.roomObj ~= nil  and self.roomObj.reconnectGameInfo ~= nil)
	if not isLegal then
		self.GameInfoIllegalTime = self.GameInfoIllegalTime + 1
		DwDebug.LogError("CheckGameInfoLegal fail")
	end
	return isLegal
end

--检查房间数据是否存在
function BasePlayCardLogic:CheckRoomInfoLegal()
	local isLegal = (self.roomObj ~= nil and self.roomObj.roomInfo ~= nil)

	if not isLegal then
		self.roomInfoIllegalTime = self.roomInfoIllegalTime + 1
		DwDebug.LogError("CheckRoomInfoLegal fail")
	end
	return isLegal
end


function BasePlayCardLogic:CheckPlayExist(userId)
	local player = self.roomObj.playerMgr:GetPlayerByPlayerID(userId)
	local isLegal = (player ~= nil)
	if not isLegal then
		if self.roomObj ~= nil and self.roomObj.roomInfo ~= nil and self.roomObj.roomInfo.playerNum ~= self.roomObj.playerMgr:GetPlayerRealNum() then -- 人数不合法
			self.playNumIllegalTime = self.playNumIllegalTime + 1
			if self.playNumIllegalTime >= 1 then
				self.playNumIllegalTime = 0
				WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
			end
		end
		DwDebug.LogError("CheckPlayExist fail userId = ".. tostring(userId))
	end
	return isLegal
end


--房间信息推送
function BasePlayCardLogic:HandleRoomInfoPush(rsp,head)

	if rsp and rsp.playerNum and rsp.playerNum > 0 then
		self.roomObj.playerMgr:SetMaxSize(rsp.playerNum)
	end
	self.roomObj:InitConfigBySvr(rsp)
	DataManager.SetRoomID(rsp.roomId)
	DataManager.SetRoomDes(rsp.playDes)

	self.msgGrouperId  = rsp.msgGrouperId
	self.msgGroupId  = rsp.msgGroupId

	--房间信息到达时，发现之前有消息先于房间消息，设置完房间信息后需要进行重连
	if self.roomInfoIllegalTime > 0 then
		self.roomInfoIllegalTime = 0
		self.GameInfoIllegalTime = 0
		WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
		return
	end

	if rsp and rsp.playerNum and rsp.playerNum > 0 then
		LuaEvent.AddEventNow(EEventType.InitHeadByPlayerNum, rsp.playerNum)
	else
		LuaEvent.AddEventNow(EEventType.InitHeadByPlayerNum,4)
	end
end



-- 游戏信息推送(重连后整个游戏信息推送)
function BasePlayCardLogic:HandleGameInfoPush(rsp,head)
	if rsp then
		self:BaseHandleGameInfoPush(rsp, head)
	end
end

-- 游戏信息推送(重连后整个游戏信息推送)
function BasePlayCardLogic:BaseHandleGameInfoPush(rsp,head)
	--游戏信息到达时，发现之前有消息先于游戏信息，设置完游戏信息后需要进行重连
	if self.GameInfoIllegalTime > 0 then
		self.GameInfoIllegalTime = 0
		self.roomInfoIllegalTime = 0
		if self.roomObj then
			self.roomObj:BaseRefreshGameInfo(rsp)
		end

		WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)

		return
	end
	self.playNumIllegalTime = 0
	if rsp then
		self.gameStarted = true
		self.isReconnectGameInfo = true
		self:ClearUnWorkTasks()
		self.roomObj:RefreshGameInfo(rsp)
		--self:ClearIntervalTaskQueue()
	end

end

-- 解散房间-提示玩家投票推送
function BasePlayCardLogic:HandleNotifyDismissVotePush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end

	if rsp then
		self.roomObj:SaveDismissInfo(rsp)
		if disband_room_ui_window and disband_room_ui_window.m_open then
			--临时测试
			disband_room_ui_window.InitWindowDetail()
		else
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "disband_room_ui_window", true , nil)
		end
	end
end

-- 房间解散结果推送
function BasePlayCardLogic:HandleNotifyDismissResultPush(rsp,head)
	if rsp then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "disband_room_ui_window", true,nil)
		if rsp.isAgree then
			WindowUtil.LuaShowTips("房间解散成功，结算数据准备中。。。")
			self:SetRoomDismissed()
		else
			--wsk中，重登录进房间如果是解散房间状态，无法知道是找朋友还是打独或者打牌（服务端设计缺陷），目前暂时解决办法是重连一次
			if self.roomObj and self.roomObj.roomInfo and self.roomObj.roomInfo.status == RoomStateEnum.Dismissing then
				WrapSys.CNetSys_ResetAutoConnectt(LuaNetWork.gameNetName)
			end
		end
	end
end

--解散房间通知
function BasePlayCardLogic:HandleRoomDismissedPush(rsp, head)
	if rsp then
		GameStateQueueTask.AddTask(function ()
		WindowUtil.ShowErrorWindow(3,"房间已被解散了！")
		end,1,EGameStateType.MainCityState)

		PlayGameSys.QuitToMainCity()
	end
end

-- 玩家退出房间推送
function BasePlayCardLogic:HandleQuitRoomPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		self.roomObj:DeleteSinglePlayer(rsp.userId)
	end
end

-- 通知踢出
function BasePlayCardLogic:HandleNotifyKickPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		if rsp.userId == DataManager.GetUserID() then
			LoginSys.LoginOut()
		else
			self.roomObj:DeleteSinglePlayer(rsp.userId)
		end
	end
end

function BasePlayCardLogic:OnCloseSmallResult()
	-- DwDebug.LogError("MJPlayLogic", "OnCloseSmallResult ", self.m_showingSmallResult)
	self.m_showingSmallResult = false
end

--小局结算推送
function BasePlayCardLogic:BaseHandleSmallResultPush(rsp,head)
	if rsp then
		-- DwDebug.LogError("MJPlayLogic", "BaseHandleSmallResultPush ", self.m_showingSmallResult)
		self.m_showingSmallResult = true
		self.m_jumpShowTotalSettlement = false
	end
end

-- 解散房间标志设置
function BasePlayCardLogic:SetRoomDismissed()
	self.m_roomDismissed = true
	-- 记录是否第一次进入房间来判断是否要弹出房间信息 这里做状态重置 当反复进入同个房间时候还是弹房间信息
	PlayerDataRefUtil.SetFloat("recordFirstEnterRoomIdToJugeWhetherToPopupPaiJuPanel", 1)

	-- 在小结算的时候其他人解散掉了房间，则直接显示总结算
	-- DwDebug.LogError("MJPlayLogic", "SetRoomDismissed ", self.m_showingSmallResult)
	if self.m_showingSmallResult then
		self.m_jumpShowTotalSettlement = true
	end
end

-- 判断是否解散房间 (用于小结算中是否显示大结算的的逻辑判断依据之一)
function BasePlayCardLogic:IsRoomDismissed()
	return self.m_roomDismissed
end

-- 大结算推送
function BasePlayCardLogic:HandleBigResultPush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		PlayGameSys.QuitToMainCity()
		return
	end
	if rsp then
		self.roomObj.playerMgr:AllPlayerWaiting()
		self.roomObj:SaveBigResult(rsp)
		-- 终止网络
		PlayGameSys.CloseNetWork()

		-- DwDebug.LogError("MJPlayLogic", self.m_showingSmallResult, self.m_jumpShowTotalSettlement)
		if not self.m_showingSmallResult or self.m_jumpShowTotalSettlement then
			self.roomObj:ChangeState(RoomStateEnum.GameOver)
		end
	end
end


--历史积分推送
function BasePlayCardLogic:HandleHistoryScorePush(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		LuaEvent.AddEventNow(EEventType.HistoryScorePush,rsp)
	end
end


-- 广播LBS
function BasePlayCardLogic:HandleLBSNotify(rsp,head)
	if not self:CheckRoomInfoLegal() then
		return
	end
	if rsp then
		self.roomObj:SetPBGeoRsp(rsp)
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "locate_ui_window", true , nil)
	end
end

--距离信息提示
function BasePlayCardLogic:HandleDistanceTipsPush(rsp,head)

	if rsp then
		-- 加入房间时，可能先收到这条数据在执行场景切换清理数据
		local taskID = DelayTaskSys.AddTask(self.DelayDistanceTips, self, rsp)
		self:AddDelayTask("DelayDistanceTips" .. taskID, taskID, 1.5)
	end
end

function BasePlayCardLogic:DelayDistanceTips(rsp)
	WindowUtil.ShowErrorWindow(3, rsp.alertStr)
end

-- 房主踢人服务器推送信息
function BasePlayCardLogic:HandleASKKickOutReplay(rsp, head)
	if self.roomObj:GetCurRoomStatus() ~= RoomStateEnum.Idle then return end

	if rsp then
		local uid = rsp.userId
		if uid == DataManager.GetUserID() and not self.roomObj:CheckUserIDIsRoomOwner(uid) then
			-- 你被请出房间
			WindowUtil.ShowErrorWindow(3, "\n你已被房主请出房间", function()
				PlayGameSys.QuitToMainCity()
			end, nil, nil, "")
		else
			if not self:CheckRoomInfoLegal() then
				return
			end
			-- 其他玩家被请出房间
			self.roomObj:DeleteSinglePlayer(uid)
		end

	end
end

------------------------------------网络主动请求消息区----------------------------------------------------
--由于服务端的每个玩法，协议头完全独立，以下函数需要子类覆写
-- 心跳包
function BasePlayCardLogic:SendHeartBeat()

end


--连接服务器成功后发送登录消息进行验证
function BasePlayCardLogic:SendLoginSvr(loginAddress,loginLng,loginLat)

end
-- 创建房间
function BasePlayCardLogic:SendCreateRoom(isAllOpen,roundNum,payType)

end
-- 加入房间
function BasePlayCardLogic:SendJoinRoom(roomId)

end

function BasePlayCardLogic:SendReconnect()

end

--发送UI渲染就绪
function BasePlayCardLogic:SendUIReady()

end

--发送游戏准备
function BasePlayCardLogic:SendPrepare(readyType)

end

-- 取消游戏准备
function BasePlayCardLogic:SendUnPrepare()

end

--发送申请解散
function BasePlayCardLogic:SendAskDismiss()

end

--发送解散投票
function BasePlayCardLogic:SendDismissVote(isAgree)

end

--发送退出房间
function BasePlayCardLogic:SendQuitRoom()

end

--请求LBS
function BasePlayCardLogic:SendAskLBS()

end

--请求历史积分
function BasePlayCardLogic:SendAskHistoryScore()

end
