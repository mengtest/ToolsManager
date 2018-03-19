--------------------------------------------------------------------------------
-- 	 File      : PlayerInitState.lua
--   author    : guoliang
--   function   : 玩家初始化
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerInitState = class("PlayerInitState",BaseObjectState)

function PlayerInitState:ctor()
 	
end


function PlayerInitState:Enter(data)
	--显示初始化
	LuaEvent.AddEventNow(EEventType.RefreshPlayerHead,self.parent.seatPos,self.parent.seatInfo)

	local playCardLogic = PlayGameSys.GetPlayLogic()
	local roomStateType = playCardLogic.roomObj.roomStateMgr:GetCurStateType()

	--回放就不要出现离线状态了
	if not PlayGameSys.GetIsRecord() then
		LuaEvent.AddEventNow(EEventType.OnlineRefresh,self.parent.seatPos,self.parent.seatInfo.onlineStatus)
	end

	if roomStateType == RoomStateEnum.Idle then --游戏还没开始
		if self.parent.seatInfo.readyStatus then
			self.parent:ChangeState(PlayerStateEnum.Prepared)
		else
			self.parent:ChangeState(PlayerStateEnum.Idle)
		end
	elseif roomStateType == RoomStateEnum.Playing then
		self.parent:ChangeState(PlayerStateEnum.Waiting)
	else
		self.parent:ChangeState(PlayerStateEnum.Waiting)
	end
end

function PlayerInitState:Leave()
	
end

function PlayerInitState:Update()
	
end

function PlayerInitState:GetType()
	return PlayerStateEnum.Init
end