--------------------------------------------------------------------------------
-- 	 File       : PlayerDDZFarmerAddDoubleState.lua
--   author     : zhanghaochun
--   function   : 斗地主，农民是加倍
--   date       : 2018-02-26
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
PlayerDDZFarmerAddDoubleState = class("PlayerDDZFarmerAddDoubleState", BaseObjectState)

function PlayerDDZFarmerAddDoubleState:ctor()
 	
end


function PlayerDDZFarmerAddDoubleState:Enter(data)
	--
	local playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,self:GetType())
	end
end

function PlayerDDZFarmerAddDoubleState:Leave()
	--
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,PlayerStateEnum.Idle)
	end
end

function PlayerDDZFarmerAddDoubleState:Update()
	
end

function PlayerDDZFarmerAddDoubleState:GetType()
	return PlayerStateEnum.FamerAddPlus
end