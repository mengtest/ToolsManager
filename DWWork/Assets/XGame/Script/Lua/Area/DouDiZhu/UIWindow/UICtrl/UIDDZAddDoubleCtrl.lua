--------------------------------------------------------------------------------
-- 	 File       : UIDDZAddDoubleCtrl.lua
--   author     : zhanghaochun
--   function   : 斗地主加倍控件
--   date       : 2018-01-30
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------
local CountDownClock = require "CommonProduct.CommonBase.UICtrl.UICountDownClock"

local UIDDZAddDoubleCtrl = class("UIDDZAddDoubleCtrl")

function UIDDZAddDoubleCtrl:ctor(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot

	self:InitValues()
	self:InitComponents()
	self:RegisterEvent()
	self:SetDefaultShow()
end

function UIDDZAddDoubleCtrl:InitValues()
	self.countdown = nil
	self.curType = 0
	self.delayTime = 15
end

function UIDDZAddDoubleCtrl:InitComponents()
	self.doubleBtnTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_double")
	self.nodoubleBtnTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_no_double")
	self.tipLabelTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "tip_label")
	self.countDownTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "countdownRoot")

	self.addDoubleTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "AddDoubleRoot")
end

function UIDDZAddDoubleCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.DDZNotifyFarmerAddDouble, self.DealNotifyDDZFarmerAddDoubleEvent, self)
	LuaEvent.AddHandle(EEventType.DDZNotifyLordAddDouble, self.DealNotifyDDZLordAddDoubleEvent, self)

	LuaEvent.AddHandle(EEventType.DDZFarmerAddDouble, self.DealPlayerAddDoubleEvent, self)
	LuaEvent.AddHandle(EEventType.DDZLordAddDouble, self.DealPlayerAddDoubleEvent, self)
end

function UIDDZAddDoubleCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.DDZNotifyFarmerAddDouble, self.DealNotifyDDZFarmerAddDoubleEvent, self)
	LuaEvent.RemoveHandle(EEventType.DDZNotifyLordAddDouble, self.DealNotifyDDZLordAddDoubleEvent, self)

	LuaEvent.RemoveHandle(EEventType.DDZFarmerAddDouble, self.DealPlayerAddDoubleEvent, self)
	LuaEvent.RemoveHandle(EEventType.DDZLordAddDouble, self.DealPlayerAddDoubleEvent, self)
end

function UIDDZAddDoubleCtrl:SetDefaultShow()
	self.luaWindowRoot:SetActive(self.rootTrans, true)
	self:SetDoubleBtnsState(false)
	self:SetTipState(false)
	self:ClosePlayerAddDoubleStatus()
end

-------------------------------------事件----------------------------------------
function UIDDZAddDoubleCtrl:DealNotifyDDZFarmerAddDoubleEvent(eventID, p1, p2)
	local rsp = p1
	if rsp then
		self:DealNetRspInfo(1, rsp)
	end
end

function UIDDZAddDoubleCtrl:DealNotifyDDZLordAddDoubleEvent(eventID, p1, p2)
	local rsp = p1
	if rsp then
		self:DealNetRspInfo(2, rsp)
	end
end

function UIDDZAddDoubleCtrl:DealPlayerAddDoubleEvent(eventID, p1, p2)
	local rsp = p1
	if rsp then
		self:SetPlayerAddDoubleStatus(rsp)
		self:SetDoubleLabelByRsp(eventID, rsp)
	else
		self:ClosePlayerAddDoubleStatus()
	end

	self:SetTipState(false)
end
--------------------------------------------------------------------------------

----------------------------------对外接口----------------------------------------
function UIDDZAddDoubleCtrl:OnBtnClick(clickName)
	if clickName == "btn_double" then
		self:SendAddDoubleReq(true)
		self:SetDoubleBtnsState(false)
		self:RemoveCountDown()
	elseif clickName == "btn_no_double" then
		self:SendAddDoubleReq(false)
		self:SetDoubleBtnsState(false)
		self:RemoveCountDown()
	else
	end
end
function UIDDZAddDoubleCtrl:Destroy()
	self:UnRegisterEvent()
	self:RemoveCountDown()
	
	self.rootTrans = nil
	self.luaWindowRoot = nil
	self.doubleBtnTrans = nil
	self.nodoubleBtnTrans = nil
	self.tipLabelTrans = nil
	self.countDownTrans = nil
end
--------------------------------------------------------------------------------
----------------------------------私有接口----------------------------------------
function UIDDZAddDoubleCtrl:SetDoubleBtnsState(state)
	self.luaWindowRoot:SetActive(self.doubleBtnTrans, state)
	self.luaWindowRoot:SetActive(self.nodoubleBtnTrans, state)
end

function UIDDZAddDoubleCtrl:SetTipState(state)
	self.luaWindowRoot:SetActive(self.tipLabelTrans, state)
end

-- tType 1 : 农民加倍 2 : 地主加倍 
function UIDDZAddDoubleCtrl:DealNetRspInfo(tType, rsp)
	self.curType = tType
	local playLogic = PlayGameSys.GetPlayLogic()
	local player = playLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if player and player.seatInfo then
		if player.seatInfo.userId == playLogic.roomObj:GetSouthUID() then
			self:SetDoubleBtnsState(true)
			self:AddCountDown(self.delayTime)
		else
			self:SetTipState(true)
		end
	end
end

function UIDDZAddDoubleCtrl:SetDoubleLabelByRsp(eventID, rsp)
	if eventID == EEventType.DDZFarmerAddDouble then
		if rsp.isPlus then
			local playLogic = PlayGameSys.GetPlayLogic()
			local player = playLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
			if player and player.seatInfo.userId == playLogic.roomObj:GetSouthUID() then
				--self:SetDoubleLabelState(true)
				LuaEvent.AddEvent(EEventType.DDZShowDouble)
			end
			
			-- 如果我是地主也要显示x2
			local myPlayer = playLogic.roomObj.playerMgr:GetPlayerByPlayerID(playLogic.roomObj:GetSouthUID())
			if myPlayer and myPlayer.seatInfo.seatId == playLogic.bankerSeatId then
				--self:SetDoubleLabelState(true)
				LuaEvent.AddEvent(EEventType.DDZShowDouble)
			end
		end
	elseif eventID == EEventType.DDZLordAddDouble then
		if rsp.isBack then
			--self:SetDoubleLabelState(rsp.isBack)
			LuaEvent.AddEvent(EEventType.DDZShowDouble)
		end
	end
end

function UIDDZAddDoubleCtrl:SetPlayerAddDoubleStatus(rsp)
	local playLogic = PlayGameSys.GetPlayLogic()
	local player = playLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	
	local trans = self:GetAddDoubleTrans(player.seatPos)
	if trans then
		self.luaWindowRoot:SetActive(trans, true)
		local isAdd = false
		if rsp.isPlus ~= nil then
			isAdd = rsp.isPlus
		end
	
		if rsp.isBack ~= nil then
			isAdd = rsp.isBack
		end
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans, "double"), isAdd)
		self.luaWindowRoot:SetActive(self.luaWindowRoot:GetTrans(trans, "nodouble"), not isAdd)
	end
end

function UIDDZAddDoubleCtrl:GetAddDoubleTrans(seatPos)
	if seatPos == SeatPosEnum.South then
		return self.luaWindowRoot:GetTrans(self.addDoubleTrans, "south")
	elseif seatPos == SeatPosEnum.East then
		return self.luaWindowRoot:GetTrans(self.addDoubleTrans, "east")
	elseif seatPos == SeatPosEnum.West then
		return self.luaWindowRoot:GetTrans(self.addDoubleTrans, "west")
	else
		return nil
	end
end

function UIDDZAddDoubleCtrl:ClosePlayerAddDoubleStatus()
	local count = self.addDoubleTrans.childCount
	for i = 1, count do
		local trans = self.addDoubleTrans:GetChild(i-1)
		self.luaWindowRoot:SetActive(trans, false)
	end
end

function UIDDZAddDoubleCtrl:AddCountDown(delayTime)
	self:RemoveCountDown()
	self.countdown = CountDownClock.New(self.luaWindowRoot, self.countDownTrans, delayTime, nil, false)
end

function UIDDZAddDoubleCtrl:RemoveCountDown()
	if self.countdown then
		self.countdown:Destroy(false, true)
		self.countdown = nil
	end
end

function UIDDZAddDoubleCtrl:SendAddDoubleReq(isAdd)
	local playLogic = PlayGameSys.GetPlayLogic()
	if self.curType == 1 then
		playLogic:SendFarmerAddTimesReq(isAdd)
	else
		playLogic:SendLordSubTimesReq(isAdd)
	end
end
--------------------------------------------------------------------------------

return UIDDZAddDoubleCtrl