--------------------------------------------------------------------------------
-- 	 File       : RoomWantLordState.lua
--   author     : zhanghaochun
--   function   : 房间农民加倍状态
--   date       : 2018-01-30
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomFarmerAddPlusState = class("RoomFarmerAddPlusState",BaseObjectState)

function RoomFarmerAddPlusState:ctor()
 	
end

function RoomFarmerAddPlusState:Init(parent)
	self.parent = parent
end

function RoomFarmerAddPlusState:Enter(data)

end

function RoomFarmerAddPlusState:Leave()
	
end

function RoomFarmerAddPlusState:GetType()
	return RoomStateEnum.FamerAddPlus
end