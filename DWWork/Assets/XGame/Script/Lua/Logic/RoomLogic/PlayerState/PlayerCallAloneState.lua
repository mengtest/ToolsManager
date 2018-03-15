--------------------------------------------------------------------------------
-- 	 File      : PlayerCallAloneState.lua
--   author    : guoliang
--   function   : 玩家主动找朋友状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerCallAloneState = class("PlayerCallAloneState",BaseObjectState)

function PlayerCallAloneState:ctor()
 	
end


function PlayerCallAloneState:Enter(data)
	if self.parent.seatInfo.userId ~= DataManager.GetUserID() then
		LuaEvent.AddEventNow(EEventType.OtherPlayerOperateTipShow,self.parent.seatInfo.nickName.."正在叫牌...")
	else
		LuaEvent.AddEventNow(EEventType.SelfCallAlone,true,nil)
	end
	AnimManager.PlayPlayerHeadAnim(true, self.parent.seatPos)
	LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,self:GetType())

end

function PlayerCallAloneState:Leave()
	if self.parent.seatInfo.userId ~= DataManager.GetUserID() then
		LuaEvent.AddEventNow(EEventType.OtherPlayerOperateTipShow,nil)
	else
		LuaEvent.AddEventNow(EEventType.SelfCallAlone,false,nil)
	end
	LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,PlayerStateEnum.Idle)
end

function PlayerCallAloneState:Update()
	
end

function PlayerCallAloneState:GetType()
	return PlayerStateEnum.CallAlone
end