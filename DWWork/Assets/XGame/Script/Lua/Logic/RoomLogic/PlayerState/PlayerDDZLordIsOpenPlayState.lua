--------------------------------------------------------------------------------
-- 	 File       : PlayerDDZLordIsOpenPlayState.lua
--   author     : zhanghaochun
--   function   : 斗地主，地主是否明牌
--   date       : 2018-02-26
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
PlayerDDZLordIsOpenPlayState = class("PlayerDDZLordIsOpenPlayState", BaseObjectState)

function PlayerDDZLordIsOpenPlayState:ctor()
 	
end


function PlayerDDZLordIsOpenPlayState:Enter(data)
	--
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,self:GetType())
	end
end

function PlayerDDZLordIsOpenPlayState:Leave()
	--
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,PlayerStateEnum.Idle)
	end
end

function PlayerDDZLordIsOpenPlayState:Update()
	
end

function PlayerDDZLordIsOpenPlayState:GetType()
	return PlayerStateEnum.LordIsOpenPlay
end