--------------------------------------------------------------------------------
-- 	 File      : PlayerPreparedState.lua
--   author    : guoliang
--   function   : 玩家已准备状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerPreparedState = class("PlayerPreparedState",BaseObjectState)

function PlayerPreparedState:ctor()
 	
end


function PlayerPreparedState:Enter(data)
	-- 重置玩家头像状态，进入新的一局
	-- LuaEvent.AddEventNow(EEventType.ResetPlayerGameHeadStatus,self.parent)
	--显示已准备UI
	LuaEvent.AddEventNow(EEventType.PlayerPrepared,self.parent.seatPos,true)
end

function PlayerPreparedState:Leave()
	--隐藏已准备UI
	LuaEvent.AddEventNow(EEventType.PlayerPrepared,self.parent.seatPos,false)
end

function PlayerPreparedState:Update()
	
end

function PlayerPreparedState:GetType()
	return PlayerStateEnum.Prepared
end