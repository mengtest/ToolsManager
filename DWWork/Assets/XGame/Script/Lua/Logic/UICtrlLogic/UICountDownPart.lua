--------------------------------------------------------------------------------
-- 	 File      : UICountDownPart.lua
--   author    : zhisong
--   function  : 倒计时小组件
--   date      : 2018年1月16日 18:22:57
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UICountDownPart = class("UICountDownPart",nil)

function UICountDownPart:ctor(luaWindowRoot, rootTrans, delay, callback)
	self.m_lua_window_root = luaWindowRoot
	self.m_count_time = delay
	self.m_call_back = callback
	self:InstantiateObj(rootTrans)
	self:SetContent()
	UpdateSecond:Add(self.UpdateSec,self)
end

function UICountDownPart:InstantiateObj(rootTrans)
	local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "count_down_part", rootTrans, RessStorgeType.RST_Never)
	if not resObj then
		return
	end
	resObj.name = "count_down_part"
	if self.m_lua_window_root then
		self.m_label = self.m_lua_window_root:GetTrans(resObj.transform, "count_label")
		self.m_lua_window_root:SetActive(resObj.transform, true)
	end
	self.transform = resObj.transform
	return resObj.transform
end

function UICountDownPart:UpdateSec()
	if self.m_count_time then
		self.m_count_time = self.m_count_time - 1
		if self.m_count_time >= 0 then
			self:SetContent()
		else
			self:Destroy(true)
		end
	end
end

function UICountDownPart:SetContent()
	if self.m_lua_window_root and self.m_label and self.m_count_time and self.m_count_time > 0 then
		self.m_lua_window_root:SetLabel(self.m_label, self.m_count_time)
	end
end

-------------------------------------------------------------------------------------------

function UICountDownPart:Destroy(still_callback)
	UpdateSecond:Remove(self.UpdateSec,self)
	if self.m_lua_window_root then
		self.m_lua_window_root:SetActive(self.transform, false)
	end
	if self.m_call_back and still_callback then
		self.m_call_back()
	end
end
