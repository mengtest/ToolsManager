--------------------------------------------------------------------------------
-- 	 File       : RoomWantLordState.lua
--   author     : zhanghaochun
--   function   : 房间要地主状态
--   date       : 2018-01-30
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomWantLordState = class("RoomWantLordState",BaseObjectState)

function RoomWantLordState:ctor()
 	
end

function RoomWantLordState:Init(parent)
	self.parent = parent
end

function RoomWantLordState:Enter(data)

end

function RoomWantLordState:Leave()
	
end

function RoomWantLordState:GetType()
	return RoomStateEnum.WantLord
end