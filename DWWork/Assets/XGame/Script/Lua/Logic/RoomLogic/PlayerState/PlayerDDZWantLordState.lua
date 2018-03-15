--------------------------------------------------------------------------------
-- 	 File       : PlayerDDZWantLordState.lua
--   author     : zhanghaochun
--   function   : 玩家叫地主状态
--   date       : 2018-02-26
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
PlayerDDZWantLordState = class("PlayerDDZWantLordState", BaseObjectState)

function PlayerDDZWantLordState:ctor()
 	
end


function PlayerDDZWantLordState:Enter(data)
	--
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,self:GetType())
	end
end

function PlayerDDZWantLordState:Leave()
	--
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,PlayerStateEnum.Idle)
	end
end

function PlayerDDZWantLordState:Update()
	
end

function PlayerDDZWantLordState:GetType()
	return PlayerStateEnum.WantLord
end