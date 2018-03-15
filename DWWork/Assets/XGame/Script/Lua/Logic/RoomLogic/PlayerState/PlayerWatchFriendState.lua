--------------------------------------------------------------------------------
-- 	 File      : PlayerWatchFriendState.lua
--   author    : guoliang
--   function   : 玩家观看朋友出牌状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerWatchFriendState = class("PlayerWatchFriendState",BaseObjectState)

function PlayerWatchFriendState:ctor()
 	
end


function PlayerWatchFriendState:Enter(data)
	--

end

function PlayerWatchFriendState:Leave()
	--

end

function PlayerWatchFriendState:Update()
	
end

function PlayerWatchFriendState:GetType()
	return PlayerStateEnum.WatchFriend
end