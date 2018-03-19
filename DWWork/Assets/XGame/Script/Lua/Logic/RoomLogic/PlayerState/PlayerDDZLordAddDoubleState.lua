--------------------------------------------------------------------------------
-- 	 File       : PlayerDDZLordAddDoubleState.lua
--   author     : zhanghaochun
--   function   : 斗地主，地主是加倍
--   date       : 2018-02-26
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
PlayerDDZLordAddDoubleState = class("PlayerDDZLordAddDoubleState", BaseObjectState)

function PlayerDDZLordAddDoubleState:ctor()
 	
end


function PlayerDDZLordAddDoubleState:Enter(data)
	--
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,self:GetType())
	end
end

function PlayerDDZLordAddDoubleState:Leave()
	--
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,PlayerStateEnum.Idle)
	end
end

function PlayerDDZLordAddDoubleState:Update()
	
end

function PlayerDDZLordAddDoubleState:GetType()
	return PlayerStateEnum.LordSubPlus
end
