--------------------------------------------------------------------------------
-- 	 File      : SimpleStateMachine.lua
--   author    : zhisong
--   function  : 简单状态机
--   date      : 2018年2月5日
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------

SimpleStateMachine = class("SimpleStateMachine", DWBaseModule)


function SimpleStateMachine:ctor()
	self.curState = nil
	self.stateList = {}
end

-- 目前看不需要update
-- function SimpleStateMachine:Update()
-- 	if curState then
-- 		curState:Update()
-- 	end
-- end

function SimpleStateMachine:CheckStateTypeIllegal(stateType)
    if self and self.stateList and self.stateList[stateType] then
        return false
    else 
        return true
    end
end

function SimpleStateMachine:ChangeState(stateType)
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
end

function SimpleStateMachine:GetCurStateType()
	if self.curState then
		return self.curState.GetType()
	else
		return
	end
end

function SimpleStateMachine:Destroy()
	self.curState = nil
	self.stateList = {}
end