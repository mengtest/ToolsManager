--------------------------------------------------------------------------------
-- 	 File      : PlayerFindFriendState.lua
--   author    : guoliang
--   function   : 玩家主动找朋友状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerFindFriendState = class("PlayerFindFriendState",BaseObjectState)

function PlayerFindFriendState:ctor()
 	
end


function PlayerFindFriendState:Enter(data)
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.OtherPlayerOperateTipShow,self.parent.seatInfo.nickName.."正在找朋友...")
	else
		LuaEvent.AddEventNow(EEventType.SelfFindFriend,true,nil)
	end
	AnimManager.PlayPlayerHeadAnim(true, self.parent.seatPos)
	LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,self:GetType())
end

function PlayerFindFriendState:Leave()
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.OtherPlayerOperateTipShow,nil)
	else
		LuaEvent.AddEventNow(EEventType.SelfFindFriend,false,nil)
	end
	--空闲状态会清除倒计时状态
	LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,PlayerStateEnum.Idle)
end

function PlayerFindFriendState:Update()
	
end

function PlayerFindFriendState:GetType()
	return PlayerStateEnum.FindFriend
end