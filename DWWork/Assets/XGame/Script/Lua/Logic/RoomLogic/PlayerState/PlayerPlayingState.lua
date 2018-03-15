--------------------------------------------------------------------------------
-- 	 File      : PlayerPlayingState.lua
--   author    : guoliang
--   function   : 玩家出牌状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerPlayingState = class("PlayerPlayingState",BaseObjectState)

function PlayerPlayingState:ctor()
 	
end


function PlayerPlayingState:Enter(data)
	local  playLogicType = PlayGameSys.GetPlayLogicType()
	if playLogicType == PlayLogicTypeEnum.WSK_Normal or playLogicType == PlayLogicTypeEnum.WSK_Record then
		self:WSKEnter()
	elseif playLogicType == PlayLogicTypeEnum.MJ_Normal or playLogicType == PlayLogicTypeEnum.MJ_Record then
		self:MJEnter()
	elseif playLogicType == PlayLogicTypeEnum.DDZ_Normal or playLogicType == PlayLogicTypeEnum.DDZ_Normal_Record then
		self:DDZEnter()
	end

end

function PlayerPlayingState:Leave()
	local  playLogicType = PlayGameSys.GetPlayLogicType()
	if playLogicType == PlayLogicTypeEnum.WSK_Normal or playLogicType == PlayLogicTypeEnum.WSK_Record then
		self:WSKLeave()
	elseif playLogicType == PlayLogicTypeEnum.MJ_Normal or playLogicType == PlayLogicTypeEnum.MJ_Record then
		self:MJLeave()
	elseif playLogicType == PlayLogicTypeEnum.DDZ_Normal or playLogicType == PlayLogicTypeEnum.DDZ_Normal_Record then
		self:DDZLeave()
	end
end

--510k玩家进入游戏状态
function PlayerPlayingState:WSKEnter()
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	-- 清除桌面自己上一手出牌
	LuaEvent.AddEventNow(EEventType.ClearPlayerOutCard,self.parent.seatPos,false)

	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
--		LuaEvent.AddEvent(EEventType.OtherPlayerOperateTipShow,self.parent.seatInfo.nickName.."正在出牌...")
		
	else
		if self.parent.outInfo then
			LuaEvent.AddEventNow(EEventType.SelfOutCard,true,self.parent.outInfo.isForce)
		end
	end
	AnimManager.PlayPlayerHeadAnim(true, self.parent.seatPos)
	LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,self:GetType())
end

--510K玩家离开游戏状态
function PlayerPlayingState:WSKLeave()
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.OtherPlayerOperateTipShow,nil)
	else
		LuaEvent.AddEventNow(EEventType.SelfOutCard,false,nil)
	end
	LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,PlayerStateEnum.Idle)
end

-- 斗地主进入游戏状态
function PlayerPlayingState:DDZEnter()
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	-- 清除桌面自己上一手出牌
	LuaEvent.AddEventNow(EEventType.ClearPlayerOutCard,self.parent.seatPos,false)

	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
--		LuaEvent.AddEvent(EEventType.OtherPlayerOperateTipShow,self.parent.seatInfo.nickName.."正在出牌...")
	else
		if self.parent.outInfo then
			LuaEvent.AddEventNow(EEventType.SelfOutCard,true,self.parent.outInfo.isForce)
		end
	end
	AnimManager.PlayPlayerHeadAnim(true, self.parent.seatPos)
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,self:GetType())
	end
end

-- 斗地主离开游戏状态
function PlayerPlayingState:DDZLeave()
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if self.parent.seatInfo.userId ~= playCardLogic.roomObj:GetSouthUID() then
		LuaEvent.AddEventNow(EEventType.OtherPlayerOperateTipShow,nil)
	else
		LuaEvent.AddEventNow(EEventType.SelfOutCard,false,nil)
	end
	LuaEvent.AddEventNow(EEventType.RefreshPlayerStatusTimer,self.parent.seatPos,PlayerStateEnum.Idle)
end


--麻将进入游戏
function PlayerPlayingState:MJEnter()
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if playLogicType == PlayLogicTypeEnum.MJ_Normal then
		if self.parent.seatInfo.userId == playCardLogic.roomObj:GetSouthUID() then
			-- 此状态持续15s以上过的话，则显示等待文字动画
			local play_logic = PlayGameSys.GetPlayLogic()
			self.taskID = DelayTaskSys.AddTask(AnimManager.PlayWaitTip,false,true)
			play_logic:AddDelayTask("ShowPlayingWaitingText",self.taskID,15)
		end
	end
end

-- 麻将离开游戏
function PlayerPlayingState:MJLeave()
	local  playCardLogic = PlayGameSys.GetPlayLogic()
	if playLogicType == PlayLogicTypeEnum.MJ_Normal then
		if self.parent.seatInfo.userId == playCardLogic.roomObj:GetSouthUID() then
			-- 清掉等待文字动画
			local play_logic = PlayGameSys.GetPlayLogic()
			AnimManager.PlayWaitTip(false)
			play_logic:RemoveDelayTask("ShowPlayingWaitingText")
			-- play_logic:RemoveDelayTask会去DelayTaskSys里边清掉task
			-- DelayTaskSys.RemoveTask(self.taskID)	
		end
	end
end

function PlayerPlayingState:Update()
	
end

function PlayerPlayingState:GetType()
	return PlayerStateEnum.Playing
end

