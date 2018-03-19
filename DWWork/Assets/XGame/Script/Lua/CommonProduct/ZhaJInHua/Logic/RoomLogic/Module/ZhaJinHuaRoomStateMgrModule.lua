--------------------------------------------------------------------------------
-- 	 File      : ZhaJinHuaRoomStateMgrModule.lua
--   author    : jianing
--   function  : 扎金花 房间状态管理组件
--   date      : 2018年02月01日
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.RoomLogic.Module.RoomStateMgrModule"
require "Logic.RoomLogic.RoomState.RoomInitState"
require "Logic.RoomLogic.RoomState.RoomDismissingState"
require "Logic.RoomLogic.RoomState.RoomGameOverState"
require "Logic.RoomLogic.RoomState.RoomIdleState"
require "Logic.RoomLogic.RoomState.RoomPlayingState"
require "Logic.RoomLogic.RoomState.RoomSmallRoundOverState"

ZhaJinHuaRoomStateMgrModule = class("ZhaJinHuaRoomStateMgrModule", RoomStateMgrModule)


function ZhaJinHuaRoomStateMgrModule:ctor()

	self.curState = nil
	self.stateList = {}

	newState = RoomDismissingState.New()
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

function ZhaJinHuaRoomStateMgrModule:CheckStateTypeIllegal(stateType)
	return stateType < RoomStateEnum.Init or stateType > RoomStateEnum.Dismissing
end


function ZhaJinHuaRoomStateMgrModule:StateTransferAction()
	if self.lastSate then
		if self.lastSate:GetType() == RoomStateEnum.Idle then
			--刷新窗口显示状态
			LuaEvent.AddEventNow(EEventType.RefreshPlayWindowStatus,true)
		end
	end
end
