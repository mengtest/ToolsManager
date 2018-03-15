--------------------------------------------------------------------------------
-- 	 File      : RoomStateMgrModule.lua
--   author    : guoliang
--   function   : 房间状态管理组件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

RoomStateMgrModule = class("RoomStateMgrModule", DWBaseModule)


function RoomStateMgrModule:ctor()

end

function RoomStateMgrModule:Init(parent)
	self.parent = parent

	for i,state in pairs(self.stateList) do
		if state then
			state:Init(parent)
		end
	end
end


function RoomStateMgrModule:Update()
	if curState then
		curState:Update()
	end
end

function RoomStateMgrModule:CheckStateTypeIllegal(stateType)

end

function RoomStateMgrModule:ChangeState(stateType)
	if self:CheckStateTypeIllegal(stateType) then
		return
	end

	if self.curState then
		if self.curState:GetType() ~= stateType then
			self.lastSate = self.curState				
			self.curState:Leave()
		else
			return
		end
	end

	self.curState = self.stateList[stateType]
	
	self.curState:Enter()
	
	-- 状态转换后处理
	self:StateTransferAction()
end


function RoomStateMgrModule:StateTransferAction()
end

function RoomStateMgrModule:GetCurStateType()
	if self.curState then
		return self.curState.GetType()
	else
		return
	end

end

function RoomStateMgrModule:Destroy()
	self.curState = nil
	self.stateList = {}
end