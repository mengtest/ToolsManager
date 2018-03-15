--------------------------------------------------------------------------------
-- 	 File      : RoomStateMgrModule.lua
--   author    : zhisong
--   function   : 房间状态管理组件
--   date      : 2017年11月11日
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.RoomLogic.Module.RoomStateMgrModule"
require "Logic.RoomLogic.RoomState.RoomInitState"
require "Logic.RoomLogic.RoomState.RoomCallAloneState"
require "Logic.RoomLogic.RoomState.RoomDismissingState"
require "Logic.RoomLogic.RoomState.RoomFindFriendState"
require "Logic.RoomLogic.RoomState.RoomGameOverState"
require "Logic.RoomLogic.RoomState.RoomIdleState"
require "Logic.RoomLogic.RoomState.RoomPlayingState"
require "Logic.RoomLogic.RoomState.RoomSmallRoundOverState"

PKRoomStateMgrModule = class("PKRoomStateMgrModule", RoomStateMgrModule)


function PKRoomStateMgrModule:ctor()

	self.curState = nil
	self.stateList = {}

	local newState = RoomCallAloneState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomDismissingState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomFindFriendState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomGameOverState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomIdleState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomSmallRoundOverState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomPlayingState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomInitState.New()
	self.stateList[newState:GetType()] = newState

end

function PKRoomStateMgrModule:CheckStateTypeIllegal(stateType)
	return stateType < RoomStateEnum.Init or stateType > RoomStateEnum.FindFriend
end

function PKRoomStateMgrModule:StateTransferAction()
	if self.lastSate then
		if self.lastSate:GetType() == RoomStateEnum.Idle then
			--刷新窗口显示状态
			LuaEvent.AddEventNow(EEventType.RefreshPlayWindowStatus,true)
		end
	end
end
