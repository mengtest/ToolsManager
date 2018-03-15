--------------------------------------------------------------------------------
-- 	 File      : CPlayer.lua
--   author    : guoliang
--   function   : 玩家
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.RoomLogic.Module.PlayerStateMgrModule"
require "Logic.RoomLogic.Module.CardMgr.PKCardMgrModule"
require "Logic.RoomLogic.Module.CardMgr.MJCardMgrModule"

CPlayer = class("CPlayer", nil)

function CPlayer:ctor()
	self.playStateMgr = PlayerStateMgrModule.New()
	self.seatPos = nil  -- 这个是客户端本地座位号，服务端下发的座位号要进行本地映射
	self.playRestNumSound = false
end

function CPlayer:InitModule()
	self.playStateMgr:Init(self)
	self.cardMgr:Init(self)
end

function CPlayer:InitCardMgrByCardKindType()
	local cardKindType = PlayGameSys.GetCardKindType()
	if cardKindType == CardKindType.PK then
		self.cardMgr = PKCardMgrModule.New()
	elseif cardKindType == CardKindType.MJ then
		self.cardMgr = MJCardMgrModule.New()
	else

	end

end
function CPlayer:Init(seatInfo)
	self:InitCardMgrByCardKindType()
	self.seatInfo = seatInfo
	self:InitModule()
end

--根据服务端初始化玩家状态
function CPlayer:StateInit()
	
end

function CPlayer:ChangeState(stateType)
	self.playStateMgr:ChangeState(stateType)
end

function CPlayer:IsState(stateType)
	return self.playStateMgr:GetType() == stateType
end

--初始化手牌
function CPlayer:InitHandCards(handCardIds)
	self.cardMgr:InitCardsFromSvr(handCardIds)
end

function CPlayer:SetFriendID(friendID)
	self.friendID = friendID
end
--设置当前出牌信息
function CPlayer:SetCurOutCardInfo(token,isForce)
	self.outInfo = {}
	self.outInfo.token = token
	self.outInfo.isForce = isForce
end

function CPlayer:Update()
	self.playStateMgr:Update(self)
end


function CPlayer:Destroy()
	self.playStateMgr:Destroy()
	self.playStateMgr = nil
	self.cardMgr:Destroy()
	self.cardMgr = nil
end