--------------------------------------------------------------------------------
-- 	 File      : RoomPlayingState.lua
--   author    : guoliang
--   function   : 房间游戏状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomPlayingState = class("RoomPlayingState",BaseObjectState)

function RoomPlayingState:ctor()
 	
end

function RoomPlayingState:Init(parent)
	self.parent = parent
end

function RoomPlayingState:Enter(data)
	--所有玩家进入空闲
	self.parent.playerMgr:AllPlayerWaiting()
	--房间其他逻辑

	--开始游戏 刷新房间局数
	LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum,self.parent.roomInfo.currentGamepos,self.parent.roomInfo.totalGameNum)
end

function RoomPlayingState:Leave()
	--

end

function RoomPlayingState:GetType()
	return RoomStateEnum.Playing
end