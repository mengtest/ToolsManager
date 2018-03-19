--------------------------------------------------------------------------------
-- 	 File      : BaseObjectState.lua
--   author    : guoliang
--   function   : 物体状态基类
--   date      : 2017-09-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
BaseObjectState = class("BaseObjectState",nil)

function BaseObjectState:ctor()
 	self.parent = nil
end

function BaseObjectState:Init(parent)
	self.parent = parent
end

function BaseObjectState:Enter(data)
	
end

function BaseObjectState:Leave()
	
end

function BaseObjectState:Update()
	
end

function BaseObjectState:GetType()
	return nil
end