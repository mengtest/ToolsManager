--------------------------------------------------------------------------------
-- 	 File      : DWBaseModule.lua
--   author    : guoliang
--   function   : 模块基类
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

DWBaseModule = class("DWBaseModule", nil)

function DWBaseModule:ctor()
	
end

function DWBaseModule:Init(parent)
	self.parent = parent
end

function DWBaseModule:Update()
	
end

function DWBaseModule:Destroy()
	
end