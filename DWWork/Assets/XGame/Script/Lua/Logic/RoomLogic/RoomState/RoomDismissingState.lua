--------------------------------------------------------------------------------
-- 	 File      : RoomDismissingState.lua
--   author    : guoliang
--   function   : 房间申请解散状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomDismissingState = class("RoomDismissingState",BaseObjectState)

function RoomDismissingState:ctor()
 	
end
function RoomDismissingState:Init(parent)
	self.parent = parent
end

function RoomDismissingState:Enter(data)
	

end

function RoomDismissingState:Leave()
	
end

function RoomDismissingState:GetType()
	return RoomStateEnum.Dismissing
end