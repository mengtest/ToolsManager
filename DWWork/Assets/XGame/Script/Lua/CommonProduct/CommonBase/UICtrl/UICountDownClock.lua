--------------------------------------------------------------------------------
-- 	 File      : UICountDownPart.lua
--   author    : zhanghaochun
--   function  : 斗地主倒计时时钟
--   date      : 2018年2月1日 18:22:57
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

local UICountDownClock = class("UICountDownClock")

function UICountDownClock:ctor(luaWindowRoot, rootTrans, delayTime, callBack, autoDis)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	self.delayTime = delayTime
	self.callBack = callBack
	self.autoDis = autoDis
	self.trans = self:InstantiateObj(self.rootTrans)

	self:InitComponents()
	self:SetContent()
	UpdateSecond:Add(self.UpdateSec, self)
end

function UICountDownClock:InstantiateObj(rootTrans)
	local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "count_down_clock", rootTrans, RessStorgeType.RST_Never)
	if not resObj then
		return nil
	end

	resObj.name = "count_down_clock"
	if self.luaWindowRoot then
		self.luaWindowRoot:SetActive(resObj.transform, true)
	end
	return resObj.transform
end

function UICountDownClock:InitComponents()
	if self.trans ~= nil then
		self.label = self.luaWindowRoot:GetTrans(self.trans, "count_label")
	end
end

function UICountDownClock:SetContent()
	if self.luaWindowRoot and self.label and self.delayTime and self.delayTime >= 0 then
		self.luaWindowRoot:SetLabel(self.label, self.delayTime)
	end
end

function UICountDownClock:UpdateSec()
	if self.delayTime then
		self.delayTime = self.delayTime - 1
		if self.delayTime >= 0 then
			self:SetContent()
		else
			self:Destroy(true, self.autoDis)	
		end
	end
end

function UICountDownClock:Destroy(stillCallBack, isDis)
	UpdateSecond:Remove(self.UpdateSec,self)
	if self.callBack and stillCallBack then
		self.callBack()
	end

	if self.luaWindowRoot and isDis then
		self.luaWindowRoot:SetActive(self.trans, false)
		
		self.rootTrans = nil
		self.luaWindowRoot = nil
		self.delayTime = nil
		self.callBack = nil
		self.trans = nil

		self.label = nil
	end

	
end

return UICountDownClock

