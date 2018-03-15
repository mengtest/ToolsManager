--------------------------------------------------------------------------------
-- 	 File      : PlayerDestroyState.lua
--   author    : guoliang
--   function   : 玩家销毁
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerDestroyState = class("PlayerDestroyState",BaseObjectState)

function PlayerDestroyState:ctor()
 	
end


function PlayerDestroyState:Enter(data)
	--移除玩家的UI显示
	LuaEvent.AddEventNow(EEventType.RefreshPlayerHead,self.parent.seatPos,nil)
end

function PlayerDestroyState:Leave()
	--

end

function PlayerDestroyState:Update()
	
end

function PlayerDestroyState:GetType()
	return PlayerStateEnum.Destroy
end