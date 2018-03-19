--------------------------------------------------------------------------------
-- 	 File      : RoomIdleState.lua
--   author    : guoliang
--   function   : 房间空闲状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomIdleState = class("RoomIdleState",BaseObjectState)

function RoomIdleState:ctor()

end

function RoomIdleState:Init(parent)
	self.parent = parent
end

function RoomIdleState:Enter(data)
	--
	
end

function RoomIdleState:Leave()


end

function RoomIdleState:GetType()
	return RoomStateEnum.Idle
end