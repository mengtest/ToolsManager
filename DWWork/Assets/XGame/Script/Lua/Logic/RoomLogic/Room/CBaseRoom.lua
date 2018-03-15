--------------------------------------------------------------------------------
-- 	 File      : CBaseRoom.lua
--   author    : guoliang
--   function   : 房间基础类（提供基础的功能函数和成员）
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.Module.CardPlayRecordModule"
require "Logic.RoomLogic.Module.PlayerMgrModule"
require "Logic.RoomLogic.Module.PlayerPointMgrModule"

CBaseRoom = class("CBaseRoom", nil)

function CBaseRoom:ctor()
	self:BaseCtor()
end

function CBaseRoom:BaseCtor()
	self.playerMgr = PlayerMgrModule.New()-- 玩家管理
	self.pointMgr = PlayerPointMgrModule.New()-- 积分管理
	self.cardPlayRecordMgr = CardPlayRecordModule.New()-- 出牌记录管理
	-- 客户端座位
	self.seatConfig = {}
	--服务器下发lbs数据 对应message PBGeo
	self.PBGeoRsp = {}
	-- 小局结算缓存
	self.smallResultMap = {}

	self.currResultRound = 0
end


function CBaseRoom:Init(parent)
	self.parent = parent

	self.roomStateMgr:Init(self)
	self.playerMgr:Init(self)
	self.pointMgr:Init(self)
	self.cardPlayRecordMgr:Init(self)

	--工作玩家
	self.curWorkingID = -1
end

--从服务端初始化房间配置信息
function CBaseRoom:InitConfigBySvr(roomInfo)
	self:AdjustSvrRoomInfo(roomInfo)
	local curStateType = self.roomStateMgr:GetCurStateType()
	if curStateType == nil then -- 第一次进入游戏房间
		-- 初始化座位配置
		self:InitSeatConfig()
		-- 房间进入初始化状态
		self.roomStateMgr:ChangeState(RoomStateEnum.Init)
	else -- 在房间里断线重连
		--开始在玩，连回来发现本局一局结束了，不会推送游戏信息
		if curStateType == RoomStateEnum.Playing and self.roomInfo.status == RoomStateEnum.Idle then 
			self.roomStateMgr:ChangeState(RoomStateEnum.Init)
		else--修复开局时未收到对应的玩家座位信息推送，游戏开始后需要根据房间信息刷新玩家
			self:StateInit()
			self:ClearOtherPlayers()
			for k,v in pairs(self.roomInfo.playerInfo) do
				if next(v) ~= nil then
					self:CreateSinglePlayerQuickInPlaying(v)
				end
			end
		end
	end

	--加入语言聊天
	NimChatSys.ApplyJoinTeam(roomInfo.msgGroupId)
end

function CBaseRoom:AdjustSvrRoomInfo(roomInfo)
	self.roomInfo = roomInfo
	if roomInfo.status == RoomStateEnum.Dismissing then --服务端的解散状态,导致客户端重启登录时不知道啥情况
--		self:UpdateCurrentStatus(RoomStateEnum.Playing)
	elseif roomInfo.status == RoomStateEnum.GameOver or roomInfo.status == RoomStateEnum.SmallRoundOver then--重连的小局结算和大结算不能直接转换
		self:UpdateCurrentStatus(RoomStateEnum.Idle)
	end
end
--删除其他玩家及其显示(自己除外)
function CBaseRoom:ClearOtherPlayers()
	local existUserIds = {}
	local selfUseId = self:GetSouthUID()
	local playerList = self.playerMgr:GetPlayerlist()
	for k,v in pairs(playerList) do
		if k and k ~= selfUseId then
			table.insert(existUserIds,k)
		end
	end
	--删除玩家及其UI显示
	for k,v in pairs(existUserIds) do
		self:DeleteSinglePlayer(v)
	end
end


-- 自己是否房主
function CBaseRoom:CheckSelfIsRoomOwner()
	if self.roomInfo then
		return self.roomInfo.fangZhu == DataManager.GetUserID()
	else
		return false
	end
end

-- 检测玩家UserID是否是房主
function CBaseRoom:CheckUserIDIsRoomOwner(userID)
	if type(userID) ~= "number" then
		DwDebug.LogError("userID is not a number")
		return false
	end
	if self.roomInfo then
		return self.roomInfo.fangZhu == userID
	else
		return false
	end
end

-- 清理房间显示
function CBaseRoom:CleanRoom()
	--一轮结束重置一轮打牌模块
	LuaEvent.AddEventNow(EEventType.RoundEndClear,nil,nil)
	--清桌
	LuaEvent.AddEventNow(EEventType.ClearDesk,nil,nil)
	-- 当前桌面分
	LuaEvent.AddEventNow(EEventType.RefreshCurTablePoint,0)
	--先清理
	self.pointMgr:ClearForNewRound()
end

--断线重连回来刷新所有的游戏信息
function CBaseRoom:BaseRefreshGameInfo(gameInfo)
	self.reconnectGameInfo = gameInfo
end

--断线重连回来刷新所有的游戏信息
function CBaseRoom:RefreshGameInfo(gameInfo)
	self:BaseRefreshGameInfo(gameInfo)
end


--更新当前小局
function CBaseRoom:UpdateCurrentRound(newRound)
	if self.roomInfo then
		self.roomInfo.currentGamepos = newRound
	end
end

--更新房间当前状态
function CBaseRoom:UpdateCurrentStatus(status)
	if self.roomInfo then
		self.roomInfo.status = status
	end
end

-- 得到当前房间状态
function CBaseRoom:GetCurRoomStatus()
	if nil ~= self.roomInfo then
		return self.roomInfo.status
	end

	-- 不存在返回RoomStateEnum.Idle
	return RoomStateEnum.Idle
end

--根据服务端初始化房间状态
function CBaseRoom:StateInit()
	if self.roomInfo ~= nil then
		self.roomStateMgr:ChangeState(self.roomInfo.status)
	else
		self.roomStateMgr:ChangeState(RoomStateEnum.Idle)-- 服务端没有下发信息 默认给空闲
	end
end

-- 外部调用接口
function CBaseRoom:ChangeState(state)
	self.roomStateMgr:ChangeState(state)
end

-- 得到房间的总局数
function CBaseRoom:GetAllRoundNum()
	if self.roomInfo ~= nil then
		return self.roomInfo.totalGameNum
	else
		return 0
	end
end


function CBaseRoom:Update()
	
end

--创建缓存玩家
function CBaseRoom:CreatePlayers()
	if self.roomInfo ~= nil and self.roomInfo.playerInfo ~= nil then
		for k,v in pairs(self.roomInfo.playerInfo) do
			if v.userId ~= 0 then
				self.playerMgr:AddPlayer(v)
			end
		end
		self.selfPlayer = self.playerMgr:GetPlayerByPlayerID(self:GetSouthUID())
		self.selfSeatInfo = self.selfPlayer.seatInfo
		-- 因为自己的位置可能在其他玩家位置的后面，不能确定其他玩家的朝向,只有等自己信息初始化后才能确定座位位置
		for k,v in pairs(self.roomInfo.playerInfo) do
			-- AAAAAAAAAAAAAAAAAAAAAAA为何要给初始化出来的数据，直接不给啊
			if v.userId ~= 0 then
				local player = self.playerMgr:GetPlayerByPlayerID(v.userId)
				if v.userId ~= self:GetSouthUID() then
					self:SeatTranslate(v)
				end
				player.seatPos = self.seatConfig[v.userId]
			end
		end

		self.playerMgr:RoomInit()
	end
end

--立刻创建一个玩家(邀请状态)
function CBaseRoom:CreateSinglePlayerQuick(seatInfo)
	if seatInfo and seatInfo.userId ~= 0 then
		if self.selfPlayer == nil then
			self.selfPlayer = self.playerMgr:GetPlayerByPlayerID(self:GetSouthUID())
			self.selfSeatInfo = self.selfPlayer.seatInfo
		end

		self:SeatTranslate(seatInfo)
		local player = self.playerMgr:AddPlayer(seatInfo)
		if player then
			--先设置座位号才能进入状态切换
			player.seatPos = self.seatConfig[seatInfo.userId]
			self:InitPlayerStatus(player)
		end
	end
end

--立刻创建一个玩家(游戏中状态)
function CBaseRoom:CreateSinglePlayerQuickInPlaying(seatInfo)
	if seatInfo and seatInfo.userId ~= 0 then
		if self.selfPlayer == nil then
			self.selfPlayer = self.playerMgr:GetPlayerByPlayerID(self:GetSouthUID())
			self.selfSeatInfo = self.selfPlayer.seatInfo
		end
		self:SeatTranslate(seatInfo)
		local player = self.playerMgr:GetPlayerByPlayerID(seatInfo.userId)
		if player == nil then
			player = self.playerMgr:AddPlayer(seatInfo)
			--先设置座位号才能进入状态切换
			player.seatPos = self.seatConfig[seatInfo.userId]
			--先初始化头像
			player:ChangeState(PlayerStateEnum.Init)
			
		else
			player.seatInfo = seatInfo
			--先设置座位号才能进入状态切换
			player.seatPos = self.seatConfig[seatInfo.userId]
		end
		--再根据游戏中的状态刷新
		self:InitPlayerStatus(player)
	end
end



function CBaseRoom:InitPlayerStatus(player)
	
end

--立刻移除一个玩家
function CBaseRoom:DeleteSinglePlayer(userId)
	if type(userId) ~= "number" then return end
	
	local seatPos = self.seatConfig[userId]
	if seatPos then
		self:PlayerLeaveSeat(seatPos,userId)
		self.playerMgr:DelPlayer(userId)
	end
end

function CBaseRoom:GetSouthUID()
	if self.south_uid then
		return self.south_uid 
	end
	if self.roomInfo then
		for i,v in ipairs(self.roomInfo.playerInfo or {}) do
			if v.userId == DataManager.GetUserID() then
				self.south_uid = v.userId
				return v.userId
			end
		end

		for i,v in ipairs(self.roomInfo.playerInfo or {}) do
			if v.seatId == 0 then
				self.south_uid = v.userId
				return v.userId
			end
		end
	end
	DwDebug.LogWarning("CBaseRoom:GetSouthUID can't find south uid,roominfo=", self.roomInfo)
end

-- 初始化座位配置 正南 1 正东 2 正北 3 正西 4
function CBaseRoom:InitSeatConfig()
	--自己永远在正南,所以其他人按照逆时针对号入座
	local selfPlayerID = self:GetSouthUID()
	self.seatConfig[selfPlayerID] = SeatPosEnum.South
	self.seatConfig[SeatPosEnum.South] = selfPlayerID
end

--根据座位号 获取用户信息
function CBaseRoom:GetPlayerBySeatPos(seatPos)
	local playerID = self.seatConfig[seatPos]
	if playerID then
		return self.playerMgr:GetPlayerByPlayerID(playerID)
	else
		return nil
	end
end

-- 座位-玩家映射
function CBaseRoom:SeatTranslate(seatinfo)
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


function CBaseRoom:PlayerLeaveSeat(seatPos, userId)
	--table.remove(self.seatConfig,userId)
	--table.remove(self.seatConfig,seatPos)
	self.seatConfig[seatPos] = nil
	self.seatConfig[userId] = nil
end

-- 当前工作的玩家ID

function CBaseRoom:SaveWorkingPlayerID(userID)
	self.curWorkingID = userID
end

--获取当前局小局结算信息
function CBaseRoom:GetCurrSmallResult()
	if self.smallResultMap[self.currResultRound] then
		return self.smallResultMap[self.currResultRound]
	else
		return self.smallResultMap[self.roomInfo.currentGamepos]
	end
end

--缓存小局结算信息
function CBaseRoom:AddSmallResult(round,resultInfo)
	self.currResultRound = round
	self.smallResultMap[round] = resultInfo
end

--获取大局结算信息
function CBaseRoom:GetBigResult()
	return self.bigResult
end

--缓存大局结算信息
function CBaseRoom:SaveBigResult(resultInfo)
	self.bigResult = resultInfo
end
--缓存解散申请结果信息
function CBaseRoom:SaveDismissInfo(dismissInfo)
	self.dismissInfo = dismissInfo
end

function CBaseRoom:SetPBGeoRsp(PBGeoRsp)
	if PBGeoRsp ~= nil then
		self.PBGeoRsp = PBGeoRsp
	end
end

-- 得到当前房间里的玩家人数
function CBaseRoom:GetRoomPlayerCount()
	return self.playerMgr:GetCount()
end

-- 得到当前房间是几人房
function CBaseRoom:GetRoomMaxCount()
	return self.playerMgr:GetMaxSize()
end

function CBaseRoom:Destroy()
	self.roomStateMgr:Destroy()
	self.roomStateMgr = nil

	self.playerMgr:Destroy()
	self.playerMgr = nil

	self.pointMgr = nil
	
	self.cardPlayRecordMgr = nil

	self.parent = nil
	self.roomInfo = nil
	self.seatConfig = nil
	self.smallResultMap = nil
end


