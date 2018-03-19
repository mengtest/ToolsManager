--------------------------------------------------------------------------------
-- 	 File       : RoomLordSubPlusState.lua
--   author     : zhanghaochun
--   function   : 房间地主反加倍状态
--   date       : 2018-01-30
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomLordSubPlusState = class("RoomLordSubPlusState",BaseObjectState)

function RoomLordSubPlusState:ctor()
 	
end

function RoomLordSubPlusState:Init(parent)
	self.parent = parent
end

function RoomLordSubPlusState:Enter(data)

end

function RoomLordSubPlusState:Leave()
	
end

function RoomLordSubPlusState:GetType()
	return RoomStateEnum.LordSubPlus
end