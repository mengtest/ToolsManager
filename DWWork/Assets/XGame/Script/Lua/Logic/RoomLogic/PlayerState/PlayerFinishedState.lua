--------------------------------------------------------------------------------
-- 	 File      : PlayerFinishedState.lua
--   author    : guoliang
--   function   : 玩家出完牌状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerFinishedState = class("PlayerFinishedState",BaseObjectState)

function PlayerFinishedState:ctor()
 	
end


function PlayerFinishedState:Enter(data)
	--

end

function PlayerFinishedState:Leave()
	--

end

function PlayerFinishedState:Update()
	
end

function PlayerFinishedState:GetType()
	return PlayerStateEnum.Finished
end