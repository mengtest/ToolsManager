--------------------------------------------------------------------------------
-- 	 File      : PlayerIdleState.lua
--   author    : guoliang
--   function   : 玩家空闲状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerIdleState = class("PlayerIdleState",BaseObjectState)

function PlayerIdleState:ctor()
 	
end


function PlayerIdleState:Enter(data)
--	DwDebug.Log("PlayerIdleState Enter seatPos " .. self.parent.seatPos)
	--显示准备
	LuaEvent.AddEventNow(EEventType.PlayNotPrepare,self.parent.seatPos,true)
end

function PlayerIdleState:Leave()
--	DwDebug.Log("PlayerIdleState Leave seatPos " .. self.parent.seatPos)
	--隐藏准备
	LuaEvent.AddEventNow(EEventType.PlayNotPrepare,self.parent.seatPos,false)
end

function PlayerIdleState:Update()
	
end

function PlayerIdleState:GetType()
	return PlayerStateEnum.Idle
end