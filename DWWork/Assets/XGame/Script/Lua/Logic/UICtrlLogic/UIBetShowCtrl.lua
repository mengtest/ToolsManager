--------------------------------------------------------------------------------
-- 	 File       : UIBetShowCtrl.lua
--   author     : zhanghaochun
--   function   : 显示押注分数控件
--   date       : 2018-01-17
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

UIBetShowCtrl = class("UIBetShowCtrl")

function UIBetShowCtrl:ctor(rootTrans, luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot

	self:InitComponents()
	self:InitValues()
	self:InitItems()
	self:SetDefaultShow()
	self:RegisterEvent()
end

function UIBetShowCtrl:InitComponents()
end

function UIBetShowCtrl:InitValues()
	self.transList = {}
end

function UIBetShowCtrl:InitItems()
	local south = self.luaWindowRoot:GetTrans(self.rootTrans, "south_node")
	local southBet = self.luaWindowRoot:GetTrans(south, "BetRoot")
	local east = self.luaWindowRoot:GetTrans(self.rootTrans, "east_node")
	local eastBet = self.luaWindowRoot:GetTrans(east, "BetRoot")
	local north = self.luaWindowRoot:GetTrans(self.rootTrans, "north_node")
	local northBet = self.luaWindowRoot:GetTrans(north, "BetRoot")
	local west = self.luaWindowRoot:GetTrans(self.rootTrans, "west_node")
	local westBet = self.luaWindowRoot:GetTrans(west, "BetRoot")

	self.transList[SeatPosEnum.South] = southBet
	self.transList[SeatPosEnum.East] = eastBet
	self.transList[SeatPosEnum.North] = northBet
	self.transList[SeatPosEnum.West] = westBet
end

function UIBetShowCtrl:SetDefaultShow()
	self:HideAllBetShow()
end

function UIBetShowCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.SetBetNum, self.DealBetNumEvent, self)
	LuaEvent.AddHandle(EEventType.BetTipState, self.DealBetTipStateEvent, self)
	LuaEvent.AddHandle(EEventType.HidePaiXing, self.HideAllEvent, self)
	
end

function UIBetShowCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.SetBetNum, self.DealBetNumEvent, self)
	LuaEvent.RemoveHandle(EEventType.BetTipState, self.DealBetTipStateEvent, self)
	LuaEvent.RemoveHandle(EEventType.HidePaiXing, self.HideAllEvent, self)
end

----------------------------------事件-------------------------------------------
function UIBetShowCtrl:DealBetTipStateEvent(evendId, p1, p2)
	local seatId = p1
	local state = p2
	local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerBySeatID(seatId)
	if player then
		local SeatPos = player.seatPos
		self:ShowBetTipBySeatPos(SeatPos, state)
	end
end

--p1是数据 p2 表示 是否播放声音
function UIBetShowCtrl:DealBetNumEvent(evendId, p1, p2)
	if p1 == nil then
		return
	end

	local bteNumTable = p1
	local seatId = bteNumTable.seatId
	local num = bteNumTable.score

	local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerBySeatID(seatId)
	if player and player.seatPos then
		if not not p2 then
			AudioManager.ThirtyTwo_PlayBaoFen(player.seatInfo.sex, num)
		end
		self:ShowBetScoreBySeatPos(player.seatPos, num)
	end
end

function UIBetShowCtrl:HideAllEvent(evendId, p1, p2)
	self:HideAllBetShow()
end
--------------------------------------------------------------------------------

----------------------------------接口------------------------------------------
function UIBetShowCtrl:SetBetNumStateBySeatPos(seatPos, state)
	local trans = self.transList[seatPos]
	if trans == nil then return end

	local tipTrans = self.luaWindowRoot:GetTrans(trans, "BetScore")
	self.luaWindowRoot:SetActive(tipTrans, state)
end

function UIBetShowCtrl:SetBetTipStateBySeatPos(seatPos, state)
	local trans = self.transList[seatPos]
	if trans == nil then return end

	local betNumTrans = self.luaWindowRoot:GetTrans(trans, "xiazhu")
	self.luaWindowRoot:SetActive(betNumTrans, state)
end

function UIBetShowCtrl:HideBetBySeatPos(seatPos)
	local trans = self.transList[seatPos]
	if trans == nil then return end

	self:SetBetNumStateBySeatPos(seatPos, false)
	self:SetBetTipStateBySeatPos(seatPos, false)
end

function UIBetShowCtrl:HideAllBetShow()
	for k, v in pairs(self.transList) do
		self:HideBetBySeatPos(k)
	end
end

function UIBetShowCtrl:ShowBetScoreBySeatPos(seatPos, score)
	local trans = self.transList[seatPos]
	if trans == nil then return end
 
	self:SetBetNumStateBySeatPos(seatPos, true)
	self:SetBetTipStateBySeatPos(seatPos, false)
	local textTrans = self.luaWindowRoot:GetTrans(trans, "BetScore")
	self.luaWindowRoot:SetLabel(textTrans, score .. "分")
end

function UIBetShowCtrl:ShowBetTipBySeatPos(seatPos, state)
	local trans = self.transList[seatPos]
	if trans == nil then return end

	self:SetBetNumStateBySeatPos(seatPos, false)
	self:SetBetTipStateBySeatPos(seatPos, state)
end

function UIBetShowCtrl:Destroy()
	self:UnRegisterEvent()
	self:HideAllBetShow()

	self.transList = {}
	self.rootTrans = nil
	self.luaWindowRoot = nil
end
--------------------------------------------------------------------------------
