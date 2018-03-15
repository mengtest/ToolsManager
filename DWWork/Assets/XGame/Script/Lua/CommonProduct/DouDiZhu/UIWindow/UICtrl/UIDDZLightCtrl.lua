--------------------------------------------------------------------------------
-- 	 File       : UIDDZWantLordCtrl.lua
--   author     : zhanghaochun
--   function   : 斗地主报警灯控件
--   date       : 2018-02-25
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

local UIDDZLightCtrl = class("UIDDZLightCtrl")

local LightPosConfig =
{
	[Common_PlayID.DW_DouDiZhu] = 
	{
		[SeatPosEnum.South] = Vector3.New(-488, 0, 0),
		[SeatPosEnum.East] = Vector3.New(448, 263, 0),
		[SeatPosEnum.North] = Vector3.New(-488, 0, 0),
		[SeatPosEnum.West] = Vector3.New(-488, 233, 0),
	},
}

local CardsNumLimitConfig = 
{
	[Common_PlayID.DW_DouDiZhu] = 3,
}

function UIDDZLightCtrl:ctor(rootTrans, luaWindowRoot, playID)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	self.playID = playID

	self:InitValues()
	self:InitComponents()
	self:RegisterEvent()
	self:SetDefaultShow()
end

function UIDDZLightCtrl:InitValues()
	self.LightPos = LightPosConfig[self.playID]
	if self.LightPos == nil then
		self.LightPos = LightPosConfig[Common_PlayID.DW_DouDiZhu]
	end

	self.transList = {}
	self.CardLimit = CardsNumLimitConfig[self.playID]
	if self.CardLimit == nil then
		self.CardLimit = CardsNumLimitConfig[Common_PlayID.DW_DouDiZhu]
	end
end

function UIDDZLightCtrl:InitComponents()
	self.LightRoot = self.luaWindowRoot:GetTrans(self.rootTrans, "Root")
end

function UIDDZLightCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.PlayerShowRemainHandCards, self.DealCardRemainNumEvent, self)
	LuaEvent.AddHandle(EEventType.ReceiveSmallResultPush, self.DealCleanAllLightEvent, self)
	LuaEvent.AddHandle(EEventType.ResetPlayerGameHeadStatus, self.DealCleanAllLightEvent, self)
end

function UIDDZLightCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.PlayerShowRemainHandCards, self.DealCardRemainNumEvent, self)
	LuaEvent.RemoveHandle(EEventType.ReceiveSmallResultPush, self.DealCleanAllLightEvent, self)
	LuaEvent.RemoveHandle(EEventType.ResetPlayerGameHeadStatus, self.DealCleanAllLightEvent, self)
end

function UIDDZLightCtrl:SetDefaultShow()
end
------------------------------------事件----------------------------------------
function UIDDZLightCtrl:DealCardRemainNumEvent(eventID, p1, p2)
	if p1 and p2 then
		local seatPos = p1
		local cardNum = p2
		if seatPos ~= SeatPosEnum.South then
			-- 我的位置不显示红灯
			self:ShowLight(seatPos, cardNum)
		end
	end
end

function UIDDZLightCtrl:DealCleanAllLightEvent(eventID, p1, p2)
	self:CloseAllLight()
end
--------------------------------------------------------------------------------

----------------------------------对外接口----------------------------------------
function UIDDZLightCtrl:Destroy()
	self:CloseAllLight()
	self:UnRegisterEvent()

	self.rootTrans = nil
	self.luaWindowRoot = nil
	self.playID = nil
end
--------------------------------------------------------------------------------

----------------------------------对内接口----------------------------------------
function UIDDZLightCtrl:CloseAllLight()
	for k, v in pairs(self.transList) do
		if v then
			self.luaWindowRoot:SetActive(v, false)
			v = nil
		end
	end
end

function UIDDZLightCtrl:ShowLight(seatPos, num)
	if num <= self.CardLimit and num > 0 then
		if self.transList[seatPos] == nil then
			local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "RedLight", self.LightRoot, RessStorgeType.RST_Never)
			if not resObj then
				return
			end

			local trans = resObj.transform
			trans.localScale = Vector3.one
			trans.localPosition = self.LightPos[seatPos]
			self.luaWindowRoot:SetActive(trans, true)

			self.transList[seatPos] = trans
		else
			local trans = self.transList[seatPos]
			self.luaWindowRoot:SetActive(trans, true)
		end
	else
		local trans = self.transList[seatPos]
		if trans then
			self.luaWindowRoot:SetActive(trans, false)
			self.transList[seatPos] = nil
		end
	end
end
--------------------------------------------------------------------------------


return UIDDZLightCtrl
