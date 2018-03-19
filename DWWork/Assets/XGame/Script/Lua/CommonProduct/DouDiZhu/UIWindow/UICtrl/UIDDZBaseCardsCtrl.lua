--------------------------------------------------------------------------------
-- 	 File       : UIDDZBaseCardsCtrl.lua
--   author     : zhanghaochun
--   function   : 斗地主底牌控件
--   date       : 2018-01-30
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

local UIDDZBaseCardsCtrl = class("UIDDZBaseCardsCtrl")

local CardDefaultNumConfig = 
{
	[Common_PlayID.DW_DouDiZhu] = 3,
}

local CardXOffsetConfig = 
{
	[Common_PlayID.DW_DouDiZhu] = 160,
}

function UIDDZBaseCardsCtrl:ctor(rootTrans, luaWindowRoot, playID)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	self.playID = playID	

	self:InitValues()
	self:InitComponents()
	self:RegisterEvent()
	self:SetDefaultShow()
end

function UIDDZBaseCardsCtrl:InitValues()
	self.transList = {}
	self.defaultNum = CardDefaultNumConfig[self.playID]
	if self.defaultNum == nil then
		self.defaultNum = 3
	end
	self.xOffset = CardXOffsetConfig[self.playID]
	if self.xOffset == nil then
		self.xOffset = 160
	end
end

function UIDDZBaseCardsCtrl:InitComponents()
end

function UIDDZBaseCardsCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.DDZShowBaseCards, self.DealDDZShowBaseCardsEvent, self)
end

function UIDDZBaseCardsCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.DDZShowBaseCards, self.DealDDZShowBaseCardsEvent, self)
end

function UIDDZBaseCardsCtrl:SetDefaultShow()
end

------------------------------------事件-----------------------------------------
function UIDDZBaseCardsCtrl:DealDDZShowBaseCardsEvent(eventID, p1, p2)
	local rsp = p1
	local otherInfo = p2
	if rsp then
		self:ShowBaseCardInfo(rsp.pai)
	else
		if otherInfo == 1 then
			-- 隐藏牌
			self:HideBaseCard()
		elseif otherInfo == 2 then
			-- 显示牌背
			self:ShowBaseCardBack()
		end
	end
end

--------------------------------------------------------------------------------

----------------------------------对外接口----------------------------------------
function UIDDZBaseCardsCtrl:Destroy()
	self:UnRegisterEvent()

	for k, v in ipairs(self.transList) do
		self.luaWindowRoot:SetActive(v, false)
		v = nil
	end
	self.transList = {}

	self.luaWindowRoot = nil
	self.rootTrans = nil 
end
--------------------------------------------------------------------------------
----------------------------------对内接口----------------------------------------
function UIDDZBaseCardsCtrl:CheckTransListNum(newNum)
	local curNum = #self.transList
	local minNum = math.min(curNum, newNum)

	self:HideAllCard()

	if newNum > curNum then
		for i = curNum +1, newNum do
			local obj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "card_item_small", RessStorgeType.RST_Never, false)
			local trans = obj.transform
			LogicUtil.EnableTouch(trans, false)
			trans.parent = self.rootTrans
			trans.localScale = Vector3.zero
			self.luaWindowRoot:SetActive(trans, true)

			trans.name = i .. "_BaseCardItem"
			table.insert(self.transList, trans)
		end
	end
end

function UIDDZBaseCardsCtrl:HideAllCard()
	for k, v in ipairs(self.transList) do
		v.localScale = Vector3.zero
	end
end

function UIDDZBaseCardsCtrl:ShowBaseCardInfo(ids)
	if ids == nil then return end

	self:CheckTransListNum(#ids)
	local cards = {}
	for k, v in ipairs(ids) do
		local item = CCard.New()
		item:Init(v, k)
		table.insert(cards, item)
	end
	for k, v in ipairs(self.transList) do
		local pos = Vector3.zero
		pos.x = (k - 1) * self.xOffset
		v.localPosition = pos
		v.localScale = Vector3.one
		LogicUtil.InitPKCard_3(self.luaWindowRoot, v, cards[k], k, false)
	end
end

function UIDDZBaseCardsCtrl:ShowBaseCardBack()
	self:CheckTransListNum(self.defaultNum)
	for k, v in ipairs(self.transList) do
		local pos = Vector3.zero
		pos.x = (k - 1) * self.xOffset
		v.localPosition = pos
		v.localScale = Vector3.one
		LogicUtil.InitPKCard_3(self.luaWindowRoot, v, nil, k, false)
	end
end

function UIDDZBaseCardsCtrl:HideBaseCard()
	for k, v in ipairs(self.transList) do
		v.localScale = Vector3.zero
	end
end
--------------------------------------------------------------------------------

return UIDDZBaseCardsCtrl