--------------------------------------------------------------------------------
-- 	 File       : DDZRoomStateMgrModule.lua
--   author     : zhanghaochun
--   function   : 斗地主房间状态管理组件
--   date       : 2018年01月26日
--   copyright  : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.Module.RoomStateMgrModule"
require "Logic.RoomLogic.RoomState.RoomInitState"
require "Logic.RoomLogic.RoomState.RoomIdleState"
require "Logic.RoomLogic.RoomState.RoomPlayingState"
require "Logic.RoomLogic.RoomState.RoomSmallRoundOverState"
require "Logic.RoomLogic.RoomState.RoomGameOverState"
require "Logic.RoomLogic.RoomState.RoomDismissingState"
require "Logic.RoomLogic.RoomState.RoomWantLordState"
require "Logic.RoomLogic.RoomState.RoomLordSelectOpenPlayState"
require "Logic.RoomLogic.RoomState.RoomFarmerAddPlusState"
require "Logic.RoomLogic.RoomState.RoomLordSubPlusState"

DDZRoomStateMgrModule = class("DDZRoomStateMgrModule", RoomStateMgrModule)

function DDZRoomStateMgrModule:ctor()
	self.curState = nil
	self.stateList = {}
	
	-- 需要修改
	local newState = RoomInitState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomIdleState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomPlayingState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomSmallRoundOverState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomGameOverState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomDismissingState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomWantLordState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomLordSelectOpenPlayState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomFarmerAddPlusState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomLordSubPlusState.New()
	self.stateList[newState:GetType()] = newState
end

function DDZRoomStateMgrModule:CheckStateTypeIllegal(stateType)
	return stateType < RoomStateEnum.Init or stateType > RoomStateEnum.LordSubPlus
end

function DDZRoomStateMgrModule:StateTransferAction()
	if self.lastSate then
		if self.lastSate:GetType() == RoomStateEnum.Idle then
			--刷新窗口显示状态
			LuaEvent.AddEventNow(EEventType.RefreshPlayWindowStatus,true)
		end
	end
end