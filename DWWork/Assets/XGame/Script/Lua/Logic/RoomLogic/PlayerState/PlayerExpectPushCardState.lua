--------------------------------------------------------------------------------
-- 	 File      : PlayerExpectPushCardState.lua
--   author    : guoliang
--   function   : 玩家等待其他人出牌状态
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayerExpectPushCardState = class("PlayerExpectPushCardState",BaseObjectState)

function PlayerExpectPushCardState:ctor()
 	
end


function PlayerExpectPushCardState:Enter(data)
	-- DwDebug.LogError("PlayerState", "PlayerExpectPushCardState:Enter")
	-- 此状态持续3s以上过的话，则显示等待文字动画
	local play_logic = PlayGameSys.GetPlayLogic()
	self.taskID = DelayTaskSys.AddTask(AnimManager.PlayWaitTip,false,true)
	play_logic:AddDelayTask("ShowWaitingText",self.taskID,3)
end

function PlayerExpectPushCardState:Leave()
	-- DwDebug.LogError("PlayerState", "PlayerExpectPushCardState:Leave")
	-- 清掉等待文字动画
	local play_logic = PlayGameSys.GetPlayLogic()
	AnimManager.PlayWaitTip(false)
	play_logic:RemoveDelayTask("ShowWaitingText")
	-- play_logic:RemoveDelayTask会去DelayTaskSys里边清掉task
	-- DelayTaskSys.RemoveTask(self.taskID)
end

function PlayerExpectPushCardState:Update()
	
end

function PlayerExpectPushCardState:GetType()
	return PlayerStateEnum.ExpectPushCard
end
