--------------------------------------------------------------------------------
-- 	 File       : UIBetCtrl.lua
--   author     : zhanghaochun
--   function   : 下注控件
--   date       : 2018-01-16
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

require "Logic.UICtrlLogic.UICountDownPart"

UIBetCtrl = class("UIBetCtrl")

local BetConfigs = 
{
	[Common_PlayID.ThirtyTwo] = {1, 5, 10, 30},
}

local BetNumImageConfigs = 
{
	[Common_PlayID.ThirtyTwo] = "pk32_num_"
}

--点击间隔限制
local m_ClickTimeValue = WrapSys.GetTimeValue()

function UIBetCtrl:ctor(rootTrans, luaWindowRoot, countdownRoot, playID)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	self.countdownRoot = countdownRoot
	self.playID = playID

	self:InitValues()
	self:InitComponents()
	self:InitBtnTransList()
	self:SetDefaultShow()

	self:RegisterEvent()
end

function UIBetCtrl:InitValues()
	self.curBetConfig = BetConfigs[self.playID]
	if self.curBetConfig == nil then
		logError("Don't have this config. Please check")
		self.curBetConfig = BetConfigs[Common_PlayID.ThirtyTwo]
	end

	self.imageStart = BetNumImageConfigs[self.playID]
	if self.curBetConfig == nil then
		logError("Dont't have this image name config. Please check")
		self.imageStart = BetNumImageConfigs[Common_PlayID.ThirtyTwo]
	end

	self.btnTransList = {}

	self.countDown = nil
	self.countDownTime = 0
end

function UIBetCtrl:InitComponents()
end

function UIBetCtrl:InitBtnTransList()
	self.luaWindowRoot:SetActive(self.rootTrans, true)
	local minNum = math.min(#self.curBetConfig, self.rootTrans.childCount)
	for i = 1, minNum do
		local trans = self.luaWindowRoot:GetTrans(self.rootTrans, "btn_bet_" .. i)
		local numTrans_1 = self.luaWindowRoot:GetTrans(trans, "txt_score_1")
		local numTrans_2 = self.luaWindowRoot:GetTrans(trans, "txt_score_2")

		local integer, remainder = Division(self.curBetConfig[i], 10)
		
		if integer == 0 then
			self.luaWindowRoot:SetActive(numTrans_1, false)
		else
			self.luaWindowRoot:SetActive(numTrans_1, true)
			self.luaWindowRoot:SetSprite(numTrans_1, self.imageStart .. integer)
		end
		self.luaWindowRoot:SetSprite(numTrans_2, self.imageStart .. remainder)
		table.insert(self.btnTransList, trans)
	end
end

function UIBetCtrl:SetDefaultShow()
	self:SetAllBetBtnsState(false)
	
	for i = #self.btnTransList + 1, self.rootTrans.childCount do
		local trans = self.luaWindowRoot:GetTrans(self.rootTrans "btn_bet_" .. i)
		self.luaWindowRoot:SetActive(trans, false)
	end
end
-------------------------------------------------------------------------------------------------------
function UIBetCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.ThirtyTwo_ShowBetBtns, self.ShowBetBtns, self)

end

function UIBetCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.ThirtyTwo_ShowBetBtns, self.ShowBetBtns, self)

end

function UIBetCtrl:ShowBetBtns(eventId,p1,p2)
	local isShow = p1
	local limitTime = p2
	if isShow then
		self:OpenBetOpBtns(limitTime)
	else
		self:CloseBetOpBtns()
	end
end
---------------------------------对外接口----------------------------------------

---------------------------------对外接口----------------------------------------
-- 打开下注按钮
-- countDownTime : 倒计时的时间， 0 不进行倒计时
function UIBetCtrl:OpenBetOpBtns(countDownTime)
	self.countDownTime = countDownTime

	if self.countDownTime ~= 0 then
		LuaEvent.AddEventNow(EEventType.PK32ShowCountDown,self.countDownTime)
		self:CreateCountDown()
	end
	self:SetAllBetBtnsState(true)
end

-- 关闭下注按钮
function UIBetCtrl:CloseBetOpBtns()
	self:SetAllBetBtnsState(false)
	if self.countDownEnd and self.countDownEnd ~= -1 then
		TimerTaskSys.RemoveTask(self.countDownEnd)
		self.countDown = -1
	end
end

-- 点击了下注按钮
function UIBetCtrl:ClickBetBtn(go)
	if m_ClickTimeValue.Value > 0 then
		return
	end

	local name = go.name
	local score = 0
	for k, v in ipairs(self.btnTransList) do
		if v ~= nil and v.name == name then
			score = self.curBetConfig[k]
			break
		end
	end

	if score ~= 0 then
		self:SendBetReq(score)
		m_ClickTimeValue.Value = 2
	end
end

function UIBetCtrl:Destroy()
	for k, v in ipairs(self.btnTransList) do
		v = nil
	end
	self.btnTransList = {}

	self.rootTrans = nil
	self.luaWindowRoot = nil
	self.playID = nil
	self.curBetConfig = nil

	self:UnRegisterEvent()
end
--------------------------------------------------------------------------------

---------------------------------私有函数----------------------------------------
function UIBetCtrl:SetAllBetBtnsState(state)
	for k, v in ipairs(self.btnTransList) do
		self.luaWindowRoot:SetActive(v, state)
	end
end

function UIBetCtrl:CreateCountDown()
	if self.countDownEnd and self.countDownEnd ~= -1 then
		TimerTaskSys.RemoveTask(self.countDownEnd)
		self.countDown = -1
	end

	self.countDownEnd = TimerTaskSys.AddTimerEventByLeftTime(
		function ()
			self:CloseBetOpBtns()
			self.countDownEnd = -1
		end, 
		self.countDownTime)
end

-- 发送请求
function UIBetCtrl:SendBetReq(score)
	local playLogic = PlayGameSys.GetPlayLogic()
	if self.playID == Common_PlayID.ThirtyTwo then
		playLogic:SendASKBet(score)
	end
end
--------------------------------------------------------------------------------