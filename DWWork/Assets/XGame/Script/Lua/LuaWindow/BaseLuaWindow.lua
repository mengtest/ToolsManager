--------------------------------------------------------------------------------
-- 	 File      : BaseLuaWindow.lua
--   author    : shandong   shandong@ezfun.cn
--   desc      : 
--   version   : 1.0
--   date      : 2017-05-24 17:13:34.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------

BaseLuaWindow = class("BaseLuaWindow", nil)

function BaseLuaWindow:BaseInit(window)
	self.m_window = window
	if self.Init then
		self:Init(window)
	end
end


function BaseLuaWindow:BaseInitWindow(open, state, param)
	self.m_openState = open
	if self.InitWindow then
		self:InitWindow(open, state, param)
	end
end

function BaseLuaWindow:BasePreCreateWindow()
	if self.PreCreateWindow then
		self:PreCreateWindow()
	end
end


function BaseLuaWindow:BaseCreateWindow()
	if self.CreateWindow then
		self:CreateWindow()
	end
end

function BaseLuaWindow:BaseOnDestroy()
	if self.OnDestroy then
		self:OnDestroy()
	end
end

function BaseLuaWindow:BaseHandleWidgetClick(gb)
	if self.HandleWidgetClick then
		self:HandleWidgetClick(gb)
	end
end

return BaseLuaWindow