--------------------------------------------------------------------------------
-- 	 File      : ClubTitleUIStateMgr.lua
--   author    : zhisong
--   function  : 简单状态机
--   date      : 2018年2月5日
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Club.SimpleStateMachine"

ClubTitleUIStateMgr = class("ClubTitleUIStateMgr", SimpleStateMachine)


function ClubTitleUIStateMgr:ctor(parent)
	self.super.ctor(self, parent)

	newState = RoomDismissingState.New()
	self.stateList[newState:GetType()] = newState

	newState = RoomGameOverState.New()
	self.stateList[newState:GetType()] = newState

	for i,state in pairs(self.stateList) do
		if state then
			state:Init(parent)
		end
	end
end
