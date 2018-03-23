--------------------------------------------------------------------------------
-- 	 File       : UIDDZOperateCardCtrl.lua
--   author     : zhanghaochun
--   function   : 斗地主打牌操作按钮控件
--   date       : 2018-02-1
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------
local CountDownClock = require "CommonProduct.CommonBase.UICtrl.UICountDownClock"

local UIDDZOperateCardCtrl = class("UIDDZOperateCardCtrl")

function UIDDZOperateCardCtrl:ctor(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot

	self:InitValues()
	self:InitComponents()
	self:RegisterEvent()
	self:SetDefaultShow()
end

function UIDDZOperateCardCtrl:InitValues()
	self.delayTime = 15
	self.countdown = nil
end

function UIDDZOperateCardCtrl:InitComponents()
	self.followPlayTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "follow_play")
	self.forcePlayTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "force_play")
	self.noOutPlayTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "noOut_play")

	self.tipCardTypeWrongTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "tip_cardtype_wrong")
	self.tipCardLittleTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "tip_card_little")

	self.countDownTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "countDownRoot")
end

function UIDDZOperateCardCtrl:RegisterEvent()
end

function UIDDZOperateCardCtrl:UnRegisterEvent()
end

function UIDDZOperateCardCtrl:SetDefaultShow()
	self:CloseOperateBtns()
	self:SetCardTypeWrongTipState(false)
	self:SetCardLittleTipState(false)
end

------------------------------------事件----------------------------------------
--------------------------------------------------------------------------------
----------------------------------对外接口----------------------------------------
-- 根据tType显示操作按钮组
-- 1 : 显示跟随出牌操作组  2 : 显示强制出牌组  3 : 显示不能出牌组   其他 : 关闭显示
function UIDDZOperateCardCtrl:ShowOperateBtns(tType)
	self:ShowOpreateBtnsByType(tType)
	if tType == 3 then
		self:SetCardLittleTipState(true)
	end
	self:AddCountDown(self.delayTime)
end

function UIDDZOperateCardCtrl:CloseOperateBtns()
	self:ShowOpreateBtnsByType(4)
	
	self:SetCardTypeWrongTipState(false)
	self:SetCardLittleTipState(false)
	
	self:RemoveCountDown()
end

function UIDDZOperateCardCtrl:SetCardTypeWrongTipState(state)
	if state == nil or state == false then
		state = false
	else
		state = true
	end

	self.luaWindowRoot:SetActive(self.tipCardTypeWrongTrans, state)
	if state then
		self.luaWindowRoot:PlayAndStopTweens(self.tipCardTypeWrongTrans, true)
	end
end

function UIDDZOperateCardCtrl:SetCardLittleTipState(state)
	if state == nil or state == false then
		state = false
	else
		state = true
	end
	
	self.luaWindowRoot:SetActive(self.tipCardLittleTrans, state)
end

function UIDDZOperateCardCtrl:Destroy()
	self:UnRegisterEvent()

	self.rootTrans = nil
	self.luaWindowRoot = nil
	self.followPlayTrans = nil
	self.forcePlayTrans = nil
	self.noOutPlayTrans = nil
	self.tipCardTypeWrongTrans = nil
	self.tipCardLittleTrans = nil
	self.countDownTrans = nil
end
--------------------------------------------------------------------------------

----------------------------------对内接口----------------------------------------

-- 根据tType显示操作按钮组
-- 1 : 显示跟随出牌操作组  2 : 显示强制出牌组  3 : 显示不能出牌组   其他 : 关闭显示
function UIDDZOperateCardCtrl:ShowOpreateBtnsByType(tType)
	self.luaWindowRoot:SetActive(self.followPlayTrans, tType == 1)
	self.luaWindowRoot:SetActive(self.forcePlayTrans, tType == 2)
	self.luaWindowRoot:SetActive(self.noOutPlayTrans, tType == 3)
end

function UIDDZOperateCardCtrl:AddCountDown(delayTime)
	self:RemoveCountDown()
	self.countdown = CountDownClock.New(self.luaWindowRoot, self.countDownTrans, delayTime, nil, false)
end

function UIDDZOperateCardCtrl:RemoveCountDown()
	if self.countdown then
		self.countdown:Destroy(false, true)
		self.countdown = nil
	end
end
--------------------------------------------------------------------------------

return UIDDZOperateCardCtrl
