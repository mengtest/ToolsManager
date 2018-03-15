--------------------------------------------------------------------------------
-- 	 File       : UIDDZWantLordCtrl.lua
--   author     : zhanghaochun
--   function   : 斗地主叫地主控件
--   date       : 2018-01-30
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

local CountDownClock = require "CommonProduct.CommonBase.UICtrl.UICountDownClock"

local UIDDZWantLordCtrl = class("UIDDZWantLordCtrl")

function UIDDZWantLordCtrl:ctor(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot

	self:InitValues()
	self:InitComponents()
	self:RegisterEvent()
	self:SetDefaultShow()
end

function UIDDZWantLordCtrl:InitValues()
	self.countdown = nil
	self.delayTime = 15
end

function UIDDZWantLordCtrl:InitComponents()
	self.noLordBtnTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_no_lord")
	self.noLordBtnBGTrans = self.luaWindowRoot:GetTrans(self.noLordBtnTrans, "btn_bg")
	self.oneLordBtnTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_onescore_lord")
	self.twoLordBtnTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_twoscore_lord")
	self.threeLordBtnTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_threescore_lord")
	self.countDownTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "countDownRoot")
	
	self.ScoreRootTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "ScoreRoot")
end

function UIDDZWantLordCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.NotifyWantLordEvent, self.DealWantLordEventFunc, self)
	LuaEvent.AddHandle(EEventType.DDZWantLordScoreEvent, self.DealWantLordScoreEvent, self)
	LuaEvent.AddHandle(EEventType.DDZLordFind, self.DealLordFindEvent, self)
end

function UIDDZWantLordCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.NotifyWantLordEvent, self.DealWantLordEventFunc, self)
	LuaEvent.RemoveHandle(EEventType.DDZWantLordScoreEvent, self.DealWantLordScoreEvent, self)
	LuaEvent.RemoveHandle(EEventType.DDZLordFind, self.DealLordFindEvent, self)
end

function UIDDZWantLordCtrl:SetDefaultShow()
	self.luaWindowRoot:SetActive(self.rootTrans, true)
	self:SetWantLordBtnsState(false)
	self:CloseWantLordScoreShow()
end

------------------------------------事件----------------------------------------
-- 通知抢地主事件
function UIDDZWantLordCtrl:DealWantLordEventFunc(eventID, p1, p2)
	local rsp = p1
	if rsp then
		local playLogic = PlayGameSys.GetPlayLogic()
		local player = playLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
		if player and player.seatInfo then
			if player.seatInfo.userId == playLogic.roomObj:GetSouthUID() then
				self:SetBtnsStateByRSP(rsp)
				self:AddCountDown(self.delayTime)
			end
		end
	end
end

-- 抢地主广播
function UIDDZWantLordCtrl:DealWantLordScoreEvent(eventID, p1, p2)
	local rsp = p1
	if rsp then
		self:SetWantLordScoreByRsp(rsp)
	end
end

-- 地主找到广播
function UIDDZWantLordCtrl:DealLordFindEvent(eventID, p1, p2)
	local seatPos = p1
	if seatPos ~= nil then
		self:CloseWantLordScoreShow()
	end
end
--------------------------------------------------------------------------------

----------------------------------对外接口----------------------------------------
function UIDDZWantLordCtrl:OnBtnClick(clickName)
	if clickName == "btn_no_lord" then
		self:SendWantLordReq(0)
		self:SetWantLordBtnsState(false)
		self:RemoveCountDown()
	elseif clickName == "btn_onescore_lord" then
		self:SendWantLordReq(1)
		self:SetWantLordBtnsState(false)
		self:RemoveCountDown()
	elseif clickName == "btn_twoscore_lord" then
		self:SendWantLordReq(2)
		self:SetWantLordBtnsState(false)
		self:RemoveCountDown()
	elseif clickName == "btn_threescore_lord" then
		self:SendWantLordReq(3)
		self:SetWantLordBtnsState(false)
		self:RemoveCountDown()
	else
	end
end

function UIDDZWantLordCtrl:Destroy()
	self:UnRegisterEvent()
	self:RemoveCountDown()

	self.rootTrans = nil
	self.luaWindowRoot = nil
	self.noLordBtnTrans = nil
	self.noLordBtnBGTrans = nil
	self.oneLordBtnTrans = nil
	self.twoLordBtnTrans = nil
	self.threeLordBtnTrans = nil
	self.countDownTrans = nil
end
--------------------------------------------------------------------------------

----------------------------------对内接口----------------------------------------
function UIDDZWantLordCtrl:SetWantLordBtnsState(state)
	self.luaWindowRoot:SetActive(self.noLordBtnTrans, state)
	self.luaWindowRoot:SetActive(self.oneLordBtnTrans, state)
	self.luaWindowRoot:SetActive(self.twoLordBtnTrans, state)
	self.luaWindowRoot:SetActive(self.threeLordBtnTrans, state)
end

function UIDDZWantLordCtrl:SetBtnsStateByRSP(rsp)
	self:SetWantLordBtnsState(true)

	
	if rsp.isForce then
		self.luaWindowRoot:SetSprite(self.noLordBtnBGTrans, "gameui_button_green")
		self.luaWindowRoot:SetUntouchable(self.noLordBtnTrans, false)
	else
		self.luaWindowRoot:SetSprite(self.noLordBtnBGTrans, "gameui_button_orange")
		self.luaWindowRoot:Settouched(self.noLordBtnTrans, false)
	end
	self.luaWindowRoot:SetGray(self.noLordBtnTrans, rsp.isForce, false)
	
	local canClick = self:CheckRSPScore(rsp.score, 1)
	self.luaWindowRoot:SetGray(self.oneLordBtnTrans, not canClick, false)
	if canClick then
		self.luaWindowRoot:Settouched(self.oneLordBtnTrans, false)
	else
		self.luaWindowRoot:SetUntouchable(self.oneLordBtnTrans, false)
	end

	canClick = self:CheckRSPScore(rsp.score, 2)
	self.luaWindowRoot:SetGray(self.twoLordBtnTrans, not canClick, false)
	if canClick then
		self.luaWindowRoot:Settouched(self.twoLordBtnTrans, false)
	else
		self.luaWindowRoot:SetUntouchable(self.twoLordBtnTrans, false)
	end

	canClick = self:CheckRSPScore(rsp.score, 3)
	self.luaWindowRoot:SetGray(self.threeLordBtnTrans, not canClick, false)
	if canClick then
		self.luaWindowRoot:Settouched(self.threeLordBtnTrans, false)
	else
		self.luaWindowRoot:SetUntouchable(self.threeLordBtnTrans, false)
	end
end

function UIDDZWantLordCtrl:CheckRSPScore(scores, num)
	for k, v in ipairs(scores) do
		if v == num then
			return true
		end
	end
	return false
end

function UIDDZWantLordCtrl:SetWantLordScoreByRsp(rsp)
	local playLogic = PlayGameSys.GetPlayLogic()
	local player = playLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	
	local trans = self:GetWantLordScoreTrans(player.seatPos)
	if trans then
		self.luaWindowRoot:SetActive(trans, true)
		local scoreTrans = self.luaWindowRoot:GetTrans(trans, "score")
		local buyaoTrans = self.luaWindowRoot:GetTrans(trans, "buyao")
		
		if rsp.score == 0 then
			self.luaWindowRoot:SetActive(scoreTrans, false)
			self.luaWindowRoot:SetActive(buyaoTrans, true)
		else
			self.luaWindowRoot:SetActive(scoreTrans, true)
			self.luaWindowRoot:SetActive(buyaoTrans, false)

			local numTrans = self.luaWindowRoot:GetTrans(scoreTrans, "num")
			local spriteName = self:GetWantLordScoreImageName(rsp.score)
			if spriteName ~= "" then
				self.luaWindowRoot:SetSprite(numTrans, spriteName)
			end
		end
	end
end

function UIDDZWantLordCtrl:GetWantLordScoreTrans(seatPos)
	if seatPos == SeatPosEnum.South then
		return self.luaWindowRoot:GetTrans(self.ScoreRootTrans, "south")
	elseif seatPos == SeatPosEnum.East then
		return self.luaWindowRoot:GetTrans(self.ScoreRootTrans, "east")
	elseif seatPos == SeatPosEnum.West then
		return self.luaWindowRoot:GetTrans(self.ScoreRootTrans, "west")
	else
		return nil
	end
end

function UIDDZWantLordCtrl:GetWantLordScoreImageName(score)
	if score == 1 then
		return "ddz_fen_1"
	elseif score == 2 then
		return "ddz_fen_2"
	elseif score == 3 then
		return "ddz_fen_2"
	elseif score == 0 then
		return "ddz_text_buqiang"
	else
		return ""
	end
end

function UIDDZWantLordCtrl:CloseWantLordScoreShow()
	local count = self.ScoreRootTrans.childCount
	for i = 1, count do
		local trans = self.ScoreRootTrans:GetChild(i-1)
		self.luaWindowRoot:SetActive(trans, false)
	end
end

function UIDDZWantLordCtrl:AddCountDown(delayTime)
	self:RemoveCountDown()
	self.countdown = CountDownClock.New(self.luaWindowRoot, self.countDownTrans, delayTime, nil, false)
end

function UIDDZWantLordCtrl:RemoveCountDown()
	if self.countdown then
		self.countdown:Destroy(false, true)
		self.countdown = nil
	end
end

function UIDDZWantLordCtrl:SendWantLordReq(score)
	local playLogic = PlayGameSys.GetPlayLogic()
	playLogic:SendIsWantLordReq(not (score == 0), score)
end
--------------------------------------------------------------------------------

return UIDDZWantLordCtrl
