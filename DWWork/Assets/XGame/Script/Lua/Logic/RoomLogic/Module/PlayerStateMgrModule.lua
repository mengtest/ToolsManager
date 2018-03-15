--------------------------------------------------------------------------------
-- 	 File      : PlayerStateMgrModule.lua
--   author    : guoliang
--   function   : 玩家状态管理组件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.PlayerState.PlayerChoosedFriendState"
require "Logic.RoomLogic.PlayerState.PlayerFindFriendState"
require "Logic.RoomLogic.PlayerState.PlayerCallAloneState"
require "Logic.RoomLogic.PlayerState.PlayerFinishedState"
require "Logic.RoomLogic.PlayerState.PlayerIdleState"
require "Logic.RoomLogic.PlayerState.PlayerPlayingState"
require "Logic.RoomLogic.PlayerState.PlayerPreparedState"
require "Logic.RoomLogic.PlayerState.PlayerWaitingState"
require "Logic.RoomLogic.PlayerState.PlayerWatchFriendState"
require "Logic.RoomLogic.PlayerState.PlayerInitState"
require "Logic.RoomLogic.PlayerState.PlayerDestroyState"
require "Logic.RoomLogic.PlayerState.PlayerExpectPushCardState"
require "Logic.RoomLogic.PlayerState.PlayerDDZWantLordState"
require "Logic.RoomLogic.PlayerState.PlayerDDZLordIsOpenPlayState"
require "Logic.RoomLogic.PlayerState.PlayerDDZFarmerAddDoubleState"
require "Logic.RoomLogic.PlayerState.PlayerDDZLordAddDoubleState"

PlayerStateMgrModule = class("PlayerStateMgrModule", DWBaseModule)


function PlayerStateMgrModule:ctor()
	self.curState = nil
	self.stateList = {}

	local newState = PlayerCallAloneState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerChoosedFriendState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerDestroyState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerFindFriendState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerFinishedState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerIdleState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerInitState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerPlayingState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerPreparedState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerWaitingState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerWatchFriendState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerExpectPushCardState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerDDZWantLordState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerDDZLordIsOpenPlayState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerDDZFarmerAddDoubleState.New()
	self.stateList[newState:GetType()] = newState

	newState = PlayerDDZLordAddDoubleState.New()
	self.stateList[newState:GetType()] = newState
end

function PlayerStateMgrModule:Init(parent)
	self.parent = parent
	for k,v in pairs(self.stateList) do
		v:Init(parent)
	end
end


function PlayerStateMgrModule:ChangeState(stateType,data)
	if stateType < PlayerStateEnum.None or stateType > PlayerStateEnum.Max then
		return
	end
	DwDebug.Log("PlayerState","PlayerStateMgrModule:ChangeState "..stateType)
	if self.curState then
		if self.curState:GetType() ~= stateType then				
			self.curState:Leave()
		else
			return
		end
	end
	self.curState = self.stateList[stateType]

	self.curState:Enter(data)
	
end

function PlayerStateMgrModule:GetType()
	if self.curState then
		return self.curState:GetType()
	end
end

function PlayerStateMgrModule:Update()
	if self.curState then
		self.curState:Update()
	end
end

function PlayerStateMgrModule:Destroy()
	self.curState = nil
	self.stateList = {}
end