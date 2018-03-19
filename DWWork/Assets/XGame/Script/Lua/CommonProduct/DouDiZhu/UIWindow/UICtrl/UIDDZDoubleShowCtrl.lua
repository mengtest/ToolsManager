--------------------------------------------------------------------------------
-- 	 File       : UIDDZDoubleShowCtrl.lua
--   author     : zhanghaochun
--   function   : 斗地主加倍显示控件
--   date       : 2018-02-26
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

local UIDDZDoubleShowCtrl = class("UIDDZDoubleShowCtrl")

function UIDDZDoubleShowCtrl:ctor(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot

	self:InitValues()
	self:InitComponents()
	self:RegisterEvent()
	self:SetDefaultShow()
end

function UIDDZDoubleShowCtrl:InitValues()
end

function UIDDZDoubleShowCtrl:InitComponents()
	self.DoubleLabel = self.luaWindowRoot:GetTrans(self.rootTrans, "DoubleLabel")
end

function UIDDZDoubleShowCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.DDZShowDouble, self.DealDDZShowDouble, self)
end

function UIDDZDoubleShowCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.DDZShowDouble, self.DealDDZShowDouble, self)
end

function UIDDZDoubleShowCtrl:SetDefaultShow()
	self.luaWindowRoot:SetActive(self.DoubleLabel, false)
end

-------------------------------------事件----------------------------------------
function UIDDZDoubleShowCtrl:DealDDZShowDouble(eventID, p1, p2)
	self:PlayShowDoubleAnim()
end
--------------------------------------------------------------------------------

----------------------------------对外接口----------------------------------------
function UIDDZDoubleShowCtrl:Destroy()
	self:UnRegisterEvent()
	
	self.rootTrans = nil
	self.luaWindowRoot = nil
end
--------------------------------------------------------------------------------

----------------------------------私有接口----------------------------------------
function UIDDZDoubleShowCtrl:PlayShowDoubleAnim()
	self.luaWindowRoot:SetActive(self.DoubleLabel, true)
	self.luaWindowRoot:PlayAndStopTweens(self.DoubleLabel, true)
end
--------------------------------------------------------------------------------

return UIDDZDoubleShowCtrl