--------------------------------------------------------------------------------
-- 	 File      : RoomFindFriendState.lua
--   author    : guoliang
--   function   : 房间找朋友状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomFindFriendState = class("RoomFindFriendState",BaseObjectState)

function RoomFindFriendState:ctor()
 	
end
function RoomFindFriendState:Init(parent)
	self.parent = parent
end

function RoomFindFriendState:Enter(data)

end

function RoomFindFriendState:Leave()
	--

end

function RoomFindFriendState:GetType()
	return RoomStateEnum.FindFriend
end