--------------------------------------------------------------------------------
-- 	 File      : RoomCallAloneState.lua
--   author    : guoliang
--   function   : 房间打独叫牌状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomCallAloneState = class("RoomCallAloneState",BaseObjectState)

function RoomCallAloneState:ctor()
 	
end

function RoomCallAloneState:Init(parent)
	self.parent = parent
end

function RoomCallAloneState:Enter(data)
	--

end

function RoomCallAloneState:Leave()
	--

end

function RoomCallAloneState:GetType()
	return RoomStateEnum.CallAlone
end