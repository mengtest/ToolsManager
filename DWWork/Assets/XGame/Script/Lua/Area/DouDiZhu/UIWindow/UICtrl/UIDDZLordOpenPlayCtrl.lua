--------------------------------------------------------------------------------
-- 	 File       : UIDDZLordOpenPlayCtrl.lua
--   author     : zhanghaochun
--   function   : 斗地主地主明牌控件
--   date       : 2018-01-30
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------
local CountDownClock = require "CommonProduct.CommonBase.UICtrl.UICountDownClock"

local UIDDZLordOpenPlayCtrl = class("UIDDZLordOpenPlayCtrl")

function UIDDZLordOpenPlayCtrl:ctor(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot

	self:InitValues()
	self:InitComponents()
	self:RegisterEvent()
	self:SetDefaultShow()
end

function UIDDZLordOpenPlayCtrl:InitValues()
	self.countdown = nil
	self.delayTime = 15
end

function UIDDZLordOpenPlayCtrl:InitComponents()
	self.openPlayBtnTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_open")
	self.noopenPlayBtnTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_no_open")
	self.tipLabelTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "tip_label")
	self.countDownTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "countdownRoot")
end

function UIDDZLordOpenPlayCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.DDZNotifyLordOpenPlay, self.DealNotifyDDZLordOpenPlayEvent, self)
	LuaEvent.AddHandle(EEventType.DDZLordOpenPlay, self.DealDDZLordOpenPlayEvent, self)
end

function UIDDZLordOpenPlayCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.DDZNotifyLordOpenPlay, self.DealNotifyDDZLordOpenPlayEvent, self)
	LuaEvent.RemoveHandle(EEventType.DDZLordOpenPlay, self.DealDDZLordOpenPlayEvent, self)
end

function UIDDZLordOpenPlayCtrl:SetDefaultShow()
	self.luaWindowRoot:SetActive(self.rootTrans, true)
	self:SetOpenPlayBtnsState(false)
	self:SetTipState(false)
end
------------------------------------事件-----------------------------------------
function UIDDZLordOpenPlayCtrl:DealNotifyDDZLordOpenPlayEvent(eventID, p1, p2)
	local rsp = p1
	if rsp then
		self:DealNotifyRspInfo(rsp)
	end
end

function UIDDZLordOpenPlayCtrl:DealDDZLordOpenPlayEvent(eventID, p1, p2)
	local rsp = p1
	
	if rsp then
		self:SetTipState(false)
	end
end
--------------------------------------------------------------------------------
----------------------------------对外接口----------------------------------------
function UIDDZLordOpenPlayCtrl:OnBtnClick(clickName)
	if clickName == "btn_open" then
		self:SendLordIsOpenPlayReq(true)
		self:SetOpenPlayBtnsState(false)
		self:RemoveCountDown()
	elseif clickName == "btn_no_open" then
		self:SendLordIsOpenPlayReq(false)
		self:SetOpenPlayBtnsState(false)
		self:RemoveCountDown()
	else
	end
end
function UIDDZLordOpenPlayCtrl:Destroy()
	self:UnRegisterEvent()
	self:RemoveCountDown()

	self.rootTrans = nil
	self.luaWindowRoot = nil
	self.openPlayBtnTrans = nil
	self.noopenPlayBtnTrans = nil
	self.tipLabelTrans = nil
	self.countDownTrans = nil
end
--------------------------------------------------------------------------------
----------------------------------对内接口----------------------------------------
function UIDDZLordOpenPlayCtrl:SetOpenPlayBtnsState(state)
	self.luaWindowRoot:SetActive(self.openPlayBtnTrans, state)
	self.luaWindowRoot:SetActive(self.noopenPlayBtnTrans, state)
end

function UIDDZLordOpenPlayCtrl:SetTipState(state)
	self.luaWindowRoot:SetActive(self.tipLabelTrans, state)
end

function UIDDZLordOpenPlayCtrl:AddCountDown(delayTime)
	self:RemoveCountDown()
	self.countdown = CountDownClock.New(self.luaWindowRoot, self.countDownTrans, delayTime, nil, false)
end

function UIDDZLordOpenPlayCtrl:RemoveCountDown()
	if self.countdown then
		self.countdown:Destroy(false, true)
		self.countdown = nil
	end
end

function UIDDZLordOpenPlayCtrl:DealNotifyRspInfo(rsp)
	local playLogic = PlayGameSys.GetPlayLogic()
	local player = playLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if player and player.seatInfo then
		if player.seatInfo.userId == playLogic.roomObj:GetSouthUID() then
			self:SetOpenPlayBtnsState(true)
			self:AddCountDown(self.delayTime)
		else
			self:SetTipState(true)
		end
	end
end

function UIDDZLordOpenPlayCtrl:SendLordIsOpenPlayReq(isOpen)
	local playLogic = PlayGameSys.GetPlayLogic()
	playLogic:SendLordIsOpenPlayReq(isOpen)
end
--------------------------------------------------------------------------------

return UIDDZLordOpenPlayCtrl