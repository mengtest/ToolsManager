--------------------------------------------------------------------------------
-- 	 File       : PK32UIKaiPaiCtrl.lua
--   author     : zhisong
--   function   : 开牌控件
--   date       : 2018年1月18日 10:33:09
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

require "Logic.UICtrlLogic.UICountDownPart"

PK32UIKaiPaiCtrl = class("PK32UIKaiPaiCtrl")

--点击间隔限制
local m_ClickTimeValue = WrapSys.GetTimeValue()

function PK32UIKaiPaiCtrl:ctor(rootTrans, luaWindowRoot)
	self.m_root = rootTrans
	self.m_luaWindowRoot = luaWindowRoot
	self:RegisterEvent()
end

function PK32UIKaiPaiCtrl:Destroy()
	self:CleanCountDown()
	self:UnRegisterEvent()
end
-------------------------------------------------------------------------------------------------------
function PK32UIKaiPaiCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.PK32ShowPrepareBtn, self.ShowPrepareBtn, self)
	LuaEvent.AddHandle(EEventType.PlayerPrepared, self.HandlePlayerPrepared, self)
	LuaEvent.AddHandle(EEventType.PK32ShowShuffleBtn, self.ShowShuffleBtn, self)
	LuaEvent.AddHandle(EEventType.PK32ShowKaiPaiBtn, self.InitCountDown, self)
	LuaEvent.AddHandle(EEventType.DealCardEnd, self.ShowKaiPaiBtn, self)
	LuaEvent.AddHandle(EEventType.PK32HideKaiPaiBtn, self.HideKaiPaiBtn, self)
end

function PK32UIKaiPaiCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.PK32ShowPrepareBtn, self.ShowPrepareBtn, self)
	LuaEvent.RemoveHandle(EEventType.PlayerPrepared, self.HandlePlayerPrepared, self)
	LuaEvent.RemoveHandle(EEventType.PK32ShowShuffleBtn, self.ShowShuffleBtn, self)
	LuaEvent.RemoveHandle(EEventType.PK32ShowKaiPaiBtn, self.InitCountDown, self)
	LuaEvent.RemoveHandle(EEventType.DealCardEnd, self.ShowKaiPaiBtn, self)
	LuaEvent.RemoveHandle(EEventType.PK32HideKaiPaiBtn, self.HideKaiPaiBtn, self)
end

---------------------------------按键响应函数----------------------------------------
function PK32UIKaiPaiCtrl:HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "kaipai_btn" then
		self:ClickKaiPaiBtn()
	elseif click_name == "prepare_btn" then

	elseif click_name == "shuffle_btn" then
		self:SendShuffle()
	end
end
---------------------------------私有函数----------------------------------------
function PK32UIKaiPaiCtrl:HandlePlayerPrepared(event_id, p1, p2)
	if p1 ~= nil and type(p1) == "string" and p2 then
		if p1 == SeatPosEnum.South then
			self:ShowPrepareBtn(EEventType.PK32ShowPrepareBtn, false)
		end
	end
end

function PK32UIKaiPaiCtrl:ShowPrepareBtn(event_id, p1, p2)
	self.m_luaWindowRoot:SetActive(self.m_luaWindowRoot:GetTrans(self.m_root, "prepare_btn"), not not p1)
end

function PK32UIKaiPaiCtrl:ShowShuffleBtn(event_id, p1, p2)
	self.m_luaWindowRoot:SetActive(self.m_luaWindowRoot:GetTrans(self.m_root, "shuffle_btn"), not not p1)
end

function PK32UIKaiPaiCtrl:InitCountDown(event_id, p1, p2)
	if p1 ~= nil and type(p1) == "number" then
		LuaEvent.AddEventNow(EEventType.PK32ShowCountDown,p1)
		self:CreateCountDown(p1)
	end
	if p2 ~= nil and type(p2) == "boolean" and p2 then
		self:ShowKaiPaiBtn()
	end
end

function PK32UIKaiPaiCtrl:ShowKaiPaiBtn(event_id, p1, p2)
	self.m_luaWindowRoot:SetActive(self.m_luaWindowRoot:GetTrans(self.m_root, "kaipai_btn"), true)
	self:SetBtnGray(false)
end

function PK32UIKaiPaiCtrl:HideKaiPaiBtn(event_id, p1, p2)
	self.m_luaWindowRoot:SetActive(self.m_luaWindowRoot:GetTrans(self.m_root, "kaipai_btn"), false)
	self:CleanCountDown()
end

function PK32UIKaiPaiCtrl:SetBtnGray(enable)
	--self.m_luaWindowRoot:SetGray(self.m_luaWindowRoot:GetTrans(self.m_root, "kaipai_btn"), enable, true)
end

function PK32UIKaiPaiCtrl:ClickKaiPaiBtn()
	self:SendKaiPai()
end

function PK32UIKaiPaiCtrl:CreateCountDown(remain_time)
	self:CleanCountDown()
	self.countDownEnd = TimerTaskSys.AddTimerEventByLeftTime(
		function ()
			self:SetBtnGray(true)
			self.countDownEnd = -1
		end, remain_time)
end

function PK32UIKaiPaiCtrl:CleanCountDown()
	if self.countDownEnd and self.countDownEnd ~= -1 then
		TimerTaskSys.RemoveTask(self.countDownEnd)
		self.countDownEnd = -1
	end
end

-- 发送请求
function PK32UIKaiPaiCtrl:SendKaiPai()
	if m_ClickTimeValue.Value > 0 then
		return
	end
	m_ClickTimeValue.Value = 1

	local playLogic = PlayGameSys.GetPlayLogic()
	if playLogic and playLogic.SendASKOpen then
		playLogic:SendASKOpen()
	end
end

-- 发送请求
function PK32UIKaiPaiCtrl:SendPrepare()
	if m_ClickTimeValue.Value > 0 then
		return
	end
	m_ClickTimeValue.Value = 1

	local playLogic = PlayGameSys.GetPlayLogic()
	if playLogic and playLogic.SendPrepare then
		playLogic:SendPrepare()
	end
end

-- 发送请求
function PK32UIKaiPaiCtrl:SendShuffle()
	if m_ClickTimeValue.Value > 0 then
		return
	end
	m_ClickTimeValue.Value = 1

	local playLogic = PlayGameSys.GetPlayLogic()
	if playLogic and playLogic.SendASKShuffle then
		playLogic:SendASKShuffle(true)
	end
end

--------------------------------------------------------------------------------