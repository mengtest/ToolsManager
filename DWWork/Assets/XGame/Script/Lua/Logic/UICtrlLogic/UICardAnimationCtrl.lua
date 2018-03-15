--------------------------------------------------------------------------------
-- 	 File      : UICardAnimationCtrl.lua
--   author    : guoliang
--   function   : UI扑克动画控制
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UICardAnimationCtrl = class("UICardAnimationCtrl",nil)

local PlayerHeadPosConfig = 
{
	[Common_PlayID.chongRen_510K] = 
	{
		[SeatPosEnum.South] = Vector3.New(-576, -63, 0),
		[SeatPosEnum.East] = Vector3.New(578, 111, 0),
		[SeatPosEnum.North] = Vector3.New(432, 268, 0),
		[SeatPosEnum.West] = Vector3.New(-576, 144, 0),
	},
	[Common_PlayID.DW_DouDiZhu] = 
	{
		[SeatPosEnum.South] = Vector3.New(-568, -81, 0),
		[SeatPosEnum.East] = Vector3.New(518, 186, 0),
		[SeatPosEnum.North] = Vector3.New(432, 268, 0),
		[SeatPosEnum.West] = Vector3.New(-568, 160, 0),
	},
}

local DealingCardConfig = 
{
	[Common_PlayID.chongRen_510K] = "dealingcard",
	[Common_PlayID.DW_DouDiZhu] = "dealingcard3",
}

function UICardAnimationCtrl:ctor()
	self.m_animTable = {}
	self.headAnim = nil
end

function UICardAnimationCtrl:Init(rootTrans,luaWindowRoot)
	self.rootTrans = rootTrans
	self.deatCardPosTrans = luaWindowRoot:GetTrans(rootTrans,"deal_card_pos")
	self.friendAnimRootTrans = luaWindowRoot:GetTrans(rootTrans,"friend_card_pos")
	self.centerPosTrans = luaWindowRoot:GetTrans(rootTrans, "center_pos")
	self.luaWindowRoot = luaWindowRoot

	self.playID = PlayGameSys.GetNowPlayId()
	-- 设置默认
	self.playerHeadPos = PlayerHeadPosConfig[Common_PlayID.chongRen_510K]
	if self.playID == Common_PlayID.DW_DouDiZhu then
		self.playerHeadPos = PlayerHeadPosConfig[Common_PlayID.DW_DouDiZhu]
	end

	self:RegisterEvent()

	self.dealingCardName = DealingCardConfig[Common_PlayID.chongRen_510K]
	if self.playID == Common_PlayID.DW_DouDiZhu then
		self.dealingCardName = DealingCardConfig[Common_PlayID.DW_DouDiZhu]
	end
end

function UICardAnimationCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.ShowCardDealCardAction,self.ShowCardDealCardAction,self)
	LuaEvent.AddHandle(EEventType.PlayCardClassAnim,self.PlayCardClassAnim,self)
	LuaEvent.AddHandle(EEventType.ShowCardFriendCardAction,self.ShowCardFriendCardAction,self)
	LuaEvent.AddHandle(EEventType.ShowCardFriendCardDismiss,self.ShowCardFriendCardDismiss,self)
	LuaEvent.AddHandle(EEventType.PlayPlayerHeadAinm, self.PlayPlayerHeadAinm, self)
end

function UICardAnimationCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.ShowCardDealCardAction,self.ShowCardDealCardAction,self)
	LuaEvent.RemoveHandle(EEventType.PlayCardClassAnim,self.PlayCardClassAnim,self)
	LuaEvent.RemoveHandle(EEventType.ShowCardFriendCardAction,self.ShowCardFriendCardAction,self)
	LuaEvent.RemoveHandle(EEventType.ShowCardFriendCardDismiss,self.ShowCardFriendCardDismiss,self)
	LuaEvent.RemoveHandle(EEventType.PlayPlayerHeadAinm, self.PlayPlayerHeadAinm, self)
end

function UICardAnimationCtrl:PlayCardClassAnim(eventId, p1, p2)
	local animName, pos = p1, p2

	if pos == nil then
		pos = UnityEngine.Vector3.New(0,0,0)
	end

	local trans = self:TryGetRecycledGB(animName)
	if trans then
		trans.localPosition = pos
	else 
		local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_Animation, animName, self.rootTrans, RessStorgeType.RST_Never)
		if not resObj then
			return
		end	
		trans = resObj.transform
		trans.localPosition = pos
		table.insert(self.m_animTable[animName], trans)
	end
	self.luaWindowRoot:SetActive(trans, true)
end

function UICardAnimationCtrl:TryGetRecycledGB(animName)
	if not self.m_animTable[animName] then
		self.m_animTable[animName] = {}
		return nil
	end

	for i,v in ipairs(self.m_animTable[animName]) do
		if v and v.gameObject and not v.gameObject.activeSelf then
			return v
		end
	end

	return nil
end

function UICardAnimationCtrl:InitAnimation(parentTrans,animationName)
	local trans = self.luaWindowRoot:GetTrans(parentTrans,animationName)
	if trans == nil then
		local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_Animation, animationName, parentTrans,RessStorgeType.RST_Never)
		if not resObj then
			return
		end
		trans = resObj.transform
		trans.parent = parentTrans
		trans.name = animationName
		trans.localScale = UnityEngine.Vector3.New(1,1,1)
	end
	self.luaWindowRoot:SetActive(trans,true)

	return trans
end

function UICardAnimationCtrl:ShowCardDealCardAction(eventId,p1,p2)
	local isPlay = p1
	if isPlay then
		self:InitAnimation(self.deatCardPosTrans, self.dealingCardName)
	else
		local trans = self.luaWindowRoot:GetTrans(self.deatCardPosTrans,self.dealingCardName)
		if trans then
			self.luaWindowRoot:SetActive(trans,false)
		end
	end
end

function UICardAnimationCtrl:ShowCardFriendCardAction(eventId,p1,p2)
	local isPlay = p1
	if isPlay then
		self:InitAnimation(self.luaWindowRoot:GetTrans(self.friendAnimRootTrans,"animation_pos"),"turnover")
	else
		local trans = self.luaWindowRoot:GetTrans(self.friendAnimRootTrans,"turnover")
		if trans then
			self.luaWindowRoot:SetActive(trans,false)
		end
	end 
end

function UICardAnimationCtrl:ShowCardFriendCardDismiss(eventId,p1,p2)
	local isPlay = p1
	local cardID = p2
	local cardTrans = self.luaWindowRoot:GetTrans(self.friendAnimRootTrans,"friend_card")
	if cardTrans then
		self.luaWindowRoot:SetActive(cardTrans,isPlay)
		if isPlay then
			local cardItem = CCard.New()
			cardItem:Init(cardID,0)
			LogicUtil.InitCardItem(self.luaWindowRoot,cardTrans,cardItem,100)
			self.luaWindowRoot:PlayTweensInChildren(self.luaWindowRoot:GetTrans(self.friendAnimRootTrans,"card_pos"),true)
		end

	end
end

-- 播放玩家头像动画
function UICardAnimationCtrl:PlayPlayerHeadAinm(eventId, show, seatPos)
	if nil == show then
		return
	end

	if show then
		if nil == seatPos then
			return
		end

		if nil == self.headAnim then
			self.headAnim = self:InitAnimation(self.centerPosTrans, "biankuang")
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

function UICardAnimationCtrl:ClearAnimation()
	local trans = self.luaWindowRoot:GetTrans(self.deatCardPosTrans, self.dealingCardName)
	if trans then
		self.luaWindowRoot:SetActive(trans,false)
	end
	self.m_animTable = {}

	if nil == self.headAnim then
		self.luaWindowRoot:SetActive(self.headAnim, false)
		self.headAnim = nil
	end
end

-- 销毁
function UICardAnimationCtrl:Destroy()
	self:ClearAnimation()
	self:UnRegisterEvent()
end