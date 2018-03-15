--------------------------------------------------------------------------------
-- 	 File      : PlayerWaitingState.lua
--   author    : guoliang
--   function   : 玩家等待其他人出牌状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerWaitingState = class("PlayerWaitingState",BaseObjectState)

function PlayerWaitingState:ctor()
 	
end


function PlayerWaitingState:Enter(data)
	--

end

function PlayerWaitingState:Leave()
	--

end

function PlayerWaitingState:Update()
	
end

function PlayerWaitingState:GetType()
	return PlayerStateEnum.Waiting
end