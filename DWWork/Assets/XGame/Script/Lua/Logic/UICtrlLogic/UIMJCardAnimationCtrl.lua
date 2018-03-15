--------------------------------------------------------------------------------
-- 	 File      : UIMJCardAnimationCtrl.lua
--   author    : guoliang
--   function   : UI扑克动画控制
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UIMJCardAnimationCtrl = class("UIMJCardAnimationCtrl",nil)

function UIMJCardAnimationCtrl:ctor()
	self.m_animTable = {}
	self.arrowAnim = nil
	self.headAnim = nil
end

function UIMJCardAnimationCtrl:Init(rootTrans,luaWindowRoot)
	self.rootTrans = rootTrans

	self.parentNodeTrans = {}
	self.parentNodeTrans[SeatPosEnum.South] = luaWindowRoot:GetTrans(rootTrans, "south_node")
	self.parentNodeTrans[SeatPosEnum.East] = luaWindowRoot:GetTrans(rootTrans, "east_node")
	self.parentNodeTrans[SeatPosEnum.North] = luaWindowRoot:GetTrans(rootTrans, "north_node")
	self.parentNodeTrans[SeatPosEnum.West] = luaWindowRoot:GetTrans(rootTrans, "west_node")

	self.parentNodeTrans["Center"] = luaWindowRoot:GetTrans(rootTrans, "other_node")

	-- 设置默认
	local vector3 = UnityEngine.Vector3
	self.playerHeadPos = 
	{
		[SeatPosEnum.South] = vector3.New(-578, -185, 0),
		[SeatPosEnum.East] = vector3.New(584, 194, 0),
		[SeatPosEnum.North] = vector3.New(420, 274, 0),
		[SeatPosEnum.West] = vector3.New(-578, -28.5, 0),
	}
	-- self.otherParentNodeTran = luaWindowRoot:GetTrans(rootTrans, "other_node")

	self.luaWindowRoot = luaWindowRoot
	self:RegisterEvent()
end

function UIMJCardAnimationCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.MJPlayCardClassAnim, self.PlayCardClassAnim, self)
	LuaEvent.AddHandle(EEventType.MJPlayCardChuPaiAnim, self.PlayCardChuPaiAnim, self)
	LuaEvent.AddHandle(EEventType.MJPlayCardArrowAnim, self.PlayCardArrowAnim, self)
	LuaEvent.AddHandle(EEventType.MJPlayWaitTip, self.PlayWaitTip, self)
	LuaEvent.AddHandle(EEventType.PlayPlayerHeadAinm, self.PlayPlayerHeadAinm, self)
	LuaEvent.AddHandle(EEventType.ChangePlayCanvas, self.ChangePlayCanvas, self)
	LuaEvent.AddHandle(EEventType.MJClearAnimation, self.ClearAnimation, self)
end

function UIMJCardAnimationCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.MJPlayCardClassAnim, self.PlayCardClassAnim, self)
	LuaEvent.RemoveHandle(EEventType.MJPlayCardChuPaiAnim, self.PlayCardChuPaiAnim, self)
	LuaEvent.RemoveHandle(EEventType.MJPlayCardArrowAnim, self.PlayCardArrowAnim, self)
	LuaEvent.RemoveHandle(EEventType.MJPlayWaitTip, self.PlayWaitTip, self)
	LuaEvent.RemoveHandle(EEventType.PlayPlayerHeadAinm, self.PlayPlayerHeadAinm, self)
	LuaEvent.RemoveHandle(EEventType.ChangePlayCanvas, self.ChangePlayCanvas, self)
	LuaEvent.RemoveHandle(EEventType.MJClearAnimation, self.ClearAnimation, self)
end

-- animName string 资源名
-- seatPos 播放方位
function UIMJCardAnimationCtrl:PlayCardClassAnim(eventId, animName, seatPos)
	self:_initAnimation(animName, seatPos)
end

-- 播放出牌动画
function UIMJCardAnimationCtrl:PlayCardChuPaiAnim(eventId, paiID, seatPos)
	if nil == paiID then
		return
	end

	local animName = "chupai"
	local trans = self:_initAnimation(animName, seatPos)
	if nil == trans then
		return
	end

	local mjCard = CMJCard.New()
	mjCard:Init(paiID, paiID)
	self.luaWindowRoot:PlayTweensInChildren(trans, true)
	LogicUtil.InitMJCardItem(self.luaWindowRoot, self.luaWindowRoot:GetTrans(trans, animName), mjCard, 2, -100)
	local bgIndex = LogicUtil.GetMJPaiType()
	LogicUtil.ChangeSingleMJPaiBg(self.luaWindowRoot, bgIndex, SeatPosEnum.South, trans, 1)
end

-- 播放箭头动画
function UIMJCardAnimationCtrl:PlayCardArrowAnim(eventId, show, pos)
	if nil == show then
		return
	end

	if show then
		if nil == pos then
			return
		end

		if nil == self.arrowAnim then
			self.arrowAnim = self:_loadAnimation("arrow", "Center")
		end

		self.arrowAnim.position = pos
		self.luaWindowRoot:SetActive(self.arrowAnim, true)
	else
		if nil ~= self.arrowAnim then
			self.luaWindowRoot:SetActive(self.arrowAnim, false)
		end
	end
end

-- 播放提示动画
function UIMJCardAnimationCtrl:PlayWaitTip(eventId, enum, show)
	if nil == show or nil == enum then
		return
	end

	if show then
		if not self.waitAnim then
			self.waitAnim = {}
		end
		if not self.waitAnim[enum] then
			self.waitAnim[enum] = self:_loadAnimation(enum, SeatPosEnum.South)
		end

		self.luaWindowRoot:SetActive(self.waitAnim[enum], true)
	else
		if self.waitAnim and self.waitAnim[enum] then
			self.luaWindowRoot:SetActive(self.waitAnim[enum], false)
		end
	end
end

-- 播放玩家头像动画
function UIMJCardAnimationCtrl:PlayPlayerHeadAinm(eventId, show, seatPos)
	if nil == show then
		return
	end

	if show then
		if nil == seatPos then
			return
		end

		if nil == self.headAnim then
			self.headAnim = self:_loadAnimation("biankuang", "Center")
		end

		self.luaWindowRoot:SetActive(self.headAnim, false)
		self.headAnim.localPosition = self.playerHeadPos[seatPos] or UnityEngine.Vector3.zero
		self.luaWindowRoot:SetActive(self.headAnim, true)
	else
		if nil ~= self.headAnim then
			self.luaWindowRoot:SetActive(self.headAnim, false)
		end
	end
end

function UIMJCardAnimationCtrl:_initAnimation(animName, seatPos)
	if nil == animName or nil == seatPos then
		return
	end

	local trans = self:_tryGetRecycledGB(animName)
	if trans then
		trans.parent = self.parentNodeTrans[seatPos]
		trans.localPosition = UnityEngine.Vector3.zero
	else
		trans = self:_loadAnimation(animName, seatPos)
		table.insert(self.m_animTable[animName], trans)
	end
	self.luaWindowRoot:SetActive(trans, true)

	return trans
end

function UIMJCardAnimationCtrl:_loadAnimation(animName, seatPos)
	local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_Animation, animName, self.rootTrans, RessStorgeType.RST_Never)
	if not resObj then
		return
	end
	trans = resObj.transform
	trans.parent = self.parentNodeTrans[seatPos]
	trans.localPosition = UnityEngine.Vector3.zero
	trans.localScale = UnityEngine.Vector3.one

	return trans
end

function UIMJCardAnimationCtrl:_tryGetRecycledGB(animName)
	if not self.m_animTable[animName] then
		self.m_animTable[animName] = {}
		return nil
	end

	for i,v in ipairs(self.m_animTable[animName]) do
		if v and not v.gameObject.activeSelf then
			return v
		end
	end

	return nil
end

function UIMJCardAnimationCtrl:ChangePlayCanvas(eventId, _, bgIndex)
	local animName = "chupai"
	local tabs = self.m_animTable[animName]
	if nil == tabs then
		return
	end

	local luaWindowRoot = self.luaWindowRoot
	local seatPos = SeatPosEnum.South
	for i,v in ipairs(tabs) do
		LogicUtil.ChangeSingleMJPaiBg(luaWindowRoot, bgIndex, seatPos, v, 1)
	end
end

function UIMJCardAnimationCtrl:ClearAnimation()
	for k,v in pairs(self.m_animTable) do
		self.luaWindowRoot:SetActive(v, false)
	end
	self.m_animTable = {}

	if nil ~= self.arrowAnim then
		self.luaWindowRoot:SetActive(self.arrowAnim, false)
		self.arrowAnim = nil
	end

	if nil ~= self.headAnim then
		self.luaWindowRoot:SetActive(self.headAnim, false)
		self.headAnim = nil
	end
end

-- 销毁
function UIMJCardAnimationCtrl:Destroy()
	self:ClearAnimation()
	self:UnRegisterEvent()
end