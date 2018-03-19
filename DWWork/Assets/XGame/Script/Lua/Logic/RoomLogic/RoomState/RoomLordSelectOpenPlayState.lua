--------------------------------------------------------------------------------
-- 	 File       : RoomLordSelectOpenPlayState.lua
--   author     : zhanghaochun
--   function   : 房间地主选择明牌状态
--   date       : 2018-01-30
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomLordSelectOpenPlayState = class("RoomLordSelectOpenPlayState",BaseObjectState)

function RoomLordSelectOpenPlayState:ctor()
 	
end

function RoomLordSelectOpenPlayState:Init(parent)
	self.parent = parent
end

function RoomLordSelectOpenPlayState:Enter(data)

end

function RoomLordSelectOpenPlayState:Leave()
	
end

function RoomLordSelectOpenPlayState:GetType()
	return RoomStateEnum.LordIsOpenPlay
end