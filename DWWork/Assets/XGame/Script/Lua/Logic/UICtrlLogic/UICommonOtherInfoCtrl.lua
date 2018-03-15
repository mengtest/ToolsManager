--------------------------------------------------------------------------------
--   File      : UICommonOtherInfoCtrl.lua
--   author    : mid
--   function  : 麻将和扑克通用 房间其他信息节点
--   date      : 2017-11-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

-- local math_floor = math.floor

UICommonOtherInfoCtrl = class("UICommonOtherInfoCtrl",nil)

local refresh_battery = function ( )
	if WrapSys.IsEditor then
		LuaEvent.AddEventNow(EEventType.BatteryRefreh)
		-- print("refresh_battery")
	else
		WrapSys.PlatInterface_RefreshBattery()
	end
end

local refresh_net = function ( )
	if WrapSys.IsEditor then
		LuaEvent.AddEventNow(EEventType.NetWorkTypeRefresh)
		-- print("refresh_net")
		LuaEvent.AddEventNow(EEventType.NetWorkStrengthRefresh)
	else
		WrapSys.PlatInterface_RefreshNetStatus()
	end
end

function UICommonOtherInfoCtrl:Init(rootTran,luaWin)
	self.wgts = {}

	self.m_luaWindowRoot = luaWin
	self.m_rootTran = rootTran
	self.m_isPlaying = true
	self.m_detlaSecond_10 = 0
	self.m_detlaSecond_60 = 0

	self:Initwgts()
	self:RegEvent()

	UpdateBeat:Add(self.UpdatePerFrame,self)
end

function UICommonOtherInfoCtrl:Destroy()
	self:UnRegEvent()
	UpdateBeat:Remove(self.UpdatePerFrame,self)
end

function UICommonOtherInfoCtrl:Initwgts()
	local m_luaWindowRoot = self.m_luaWindowRoot
	local m_rootTran = self.m_rootTran

	m_luaWindowRoot:SetActive(m_rootTran,true)

	self.label_wifi = m_luaWindowRoot:GetTrans(m_rootTran,"label_wifi")
	m_luaWindowRoot:SetActive(self.label_wifi,false)
	self.label_4G = m_luaWindowRoot:GetTrans(m_rootTran,"label_4G")
	-- fillAmount
	self.power_value = m_luaWindowRoot:GetTrans(m_rootTran,"ico_power_value")

	self.label_time = m_luaWindowRoot:GetTrans(m_rootTran,"label_time")

	self.label_roomID = m_luaWindowRoot:GetTrans(m_rootTran,"label_roomID")
end

function UICommonOtherInfoCtrl:RegEvent()
	LuaEvent.AddHandle(EEventType.RefreshTime,self.RefreshTime,self)
	LuaEvent.AddHandle(EEventType.BatteryRefreh,self.BatteryRefreh,self)
	LuaEvent.AddHandle(EEventType.NetWorkTypeRefresh,self.NetWorkTypeRefresh,self)
	LuaEvent.AddHandle(EEventType.NetWorkStrengthRefresh,self.NetWorkStrengthRefresh,self)
	LuaEvent.AddHandle(EEventType.RefreshRoomNum,self.RefreshRoomNum,self)
	LuaEvent.AddHandle(EEventType.RefreshOtherInfo,self.RefreshOtherInfo,self)
end

function UICommonOtherInfoCtrl:UnRegEvent()
	LuaEvent.RemoveHandle(EEventType.RefreshTime,self.RefreshTime,self)
	LuaEvent.RemoveHandle(EEventType.BatteryRefreh,self.BatteryRefreh,self)
	LuaEvent.RemoveHandle(EEventType.NetWorkTypeRefresh,self.NetWorkTypeRefresh,self)
	LuaEvent.RemoveHandle(EEventType.NetWorkStrengthRefresh,self.NetWorkStrengthRefresh,self)
	LuaEvent.RemoveHandle(EEventType.RefreshRoomNum,self.RefreshRoomNum,self)
	LuaEvent.RemoveHandle(EEventType.RefreshOtherInfo,self.RefreshOtherInfo,self)
	self.m_luaWindowRoot = nil
	self.m_rootTran = nil
	self.m_isPlaying = false
	self.m_detlaSecond_10 = 0
	self.m_detlaSecond_60 = 0
end

function UICommonOtherInfoCtrl:UpdatePerFrame(deltatime,unscaledDeltaTime)

	if not self.m_isPlaying then
		return
	end

	-- 10s 刷一次 网络类型和网络状态
	self.m_detlaSecond_10 = self.m_detlaSecond_10 + deltatime
	if self.m_detlaSecond_10 > 10 then
		refresh_net()
		self.m_detlaSecond_10 = 0
	end

	-- 一分钟 刷一次 时间和电量
	self.m_detlaSecond_60 = self.m_detlaSecond_60 + deltatime
	if self.m_detlaSecond_60 > 60 then
		self:RefreshTime()
		refresh_battery()
		self.m_detlaSecond_60 = 0
	end

end

-- 开始刷新所有其他信息
function UICommonOtherInfoCtrl:RefreshOtherInfo()
	-- print("UICommonOtherInfoCtrl RefreshOtherInfo")
	self.m_luaWindowRoot:SetActive(self.m_rootTran,true)
	self:RefreshTime()
	self:RefreshRoomNum()
	refresh_net()
	refresh_battery()
end

-- 刷新时间
function UICommonOtherInfoCtrl:RefreshTime()
	-- print(" UICommonOtherInfoCtrl RefreshTime")
	self.m_luaWindowRoot:SetLabel(self.label_time,WrapSys.TimerSys_GetCurrentTimeStrHM())
end

-- 刷新电量
function UICommonOtherInfoCtrl:BatteryRefreh()
	-- print("UICommonOtherInfoCtrl BatteryRefreh")
	local bat = WrapSys.GetBattery()
	if WrapSys.IsEditor then
		bat = 33
	end
	self.m_luaWindowRoot:SetFillSprite(self.power_value, bat/100, true)
end

-- 刷新信号类型
function UICommonOtherInfoCtrl:NetWorkTypeRefresh()
	-- print("UICommonOtherInfoCtrl NetWorkTypeRefresh")
	local isWifi, is4G = false,false
	local _type = WrapSys.GetNetType()

	if not self.printType then
		self.printType = true
	end
	if _type == 5 then
		isWifi = true
	elseif _type == 3 then
		is4G = true
	end

	if WrapSys.IsEditor then
		isWifi = false
		is4G = true
	end

	-- isWifi = false
	-- is4G = true
	local lv = p1 -- 1-4 1为空 4为三格
	self.m_luaWindowRoot:SetActive(self.label_wifi,isWifi)
	self.m_luaWindowRoot:SetActive(self.label_4G,is4G)
end

local function getWifiLv( )
	local lv = WrapSys.GetWifiRssi()
--  print("wifi信号强度")
--  print(lv)
	if WrapSys.IsEditor then
		lv = 3
	end
	if lv == 0 then
		lv = 1
	elseif lv == 1 then
		lv = 2
	elseif lv == 2 or lv == 3 then
		lv = 3
	elseif lv == 4 or lv == 5 then
		lv = 4
	end
	return lv
end

local function get4GLv( )
	local lv = WrapSys.GetWifiRssi()
--  print("4G信号强度")
--  print(lv)
	if WrapSys.IsEditor then
		lv = 3
	end
	if lv == 0 then
		lv = 0
	elseif lv == 1 then
		lv = 1
	elseif lv == 2 or lv == 3 then
		lv = 2
	elseif lv == 4 then
		lv = 3
	elseif lv == 5 then
		lv = 4
	end
	return lv
end

-- 刷新信号强度
function UICommonOtherInfoCtrl:NetWorkStrengthRefresh(eventId)
--  print("UICommonOtherInfoCtrl NetWorkStrengthRefresh")
	if self.label_wifi and self.label_wifi.gameObject.activeSelf then
		self.m_luaWindowRoot:SetSprite(self.label_wifi,"wifi-"..getWifiLv())
	end
	if self.label_4G and self.label_4G.gameObject.activeSelf then
		self.m_luaWindowRoot:SetSprite(self.label_4G,"xinhao-"..get4GLv())
	end
end

-- 刷新房间号
function UICommonOtherInfoCtrl:RefreshRoomNum( )
	self.m_luaWindowRoot:SetLabel(self.label_roomID,"房间号 "..DataManager.GetRoomID())
end
