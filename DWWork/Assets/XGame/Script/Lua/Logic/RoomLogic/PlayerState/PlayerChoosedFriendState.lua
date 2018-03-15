--------------------------------------------------------------------------------
-- 	 File      : PlayerChoosedFriendState.lua
--   author    : guoliang
--   function   : 玩家被其他人选为朋友状态(崇仁510K选中的朋友隐藏朋友关系，不需要该状态)
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerChoosedFriendState = class("PlayerChoosedFriendState",BaseObjectState)

function PlayerChoosedFriendState:ctor()
 	
end


function PlayerChoosedFriendState:Enter(data)
	--

end

function PlayerChoosedFriendState:Leave()
	--

end

function PlayerChoosedFriendState:Update()
	
end

function PlayerChoosedFriendState:GetType()
	return PlayerStateEnum.ChoosedFriend
end