--------------------------------------------------------------------------------
-- 	 File       : UIDDZHandCardCtrl.lua
--   author     : zhanghaochun
--   function   : UI斗地主手牌显示控制
--   date       : 2018-03-05
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "CommonProduct.CommonBase.UICtrl.UIBaseHandCardCtrl"

local UIDDZHandCardCtrl = class("UIDDZHandCardCtrl", UIBaseHandCardCtrl)

function UIDDZHandCardCtrl:ctor()

end

function UIDDZHandCardCtrl:Init(rootTrans,luaWindowRoot,handCards)
	self:BaseInit(rootTrans, luaWindowRoot, handCards)
	
	self:RegisterEvent()
	self:InitValues()
end

function UIDDZHandCardCtrl:RegisterEvent()
	self:BaseRegisterEvent()    -- 一定要写

	LuaEvent.AddHandle(EEventType.DDZLordOpenPlay, self.DealDDZLordOpenPlayEvent, self)
end

function UIDDZHandCardCtrl:UnRegisterEvent()
	self:BaseUnRegisterEvent()    -- 一定要写

	LuaEvent.RemoveHandle(EEventType.DDZLordOpenPlay, self.DealDDZLordOpenPlayEvent, self)
end

function UIDDZHandCardCtrl:InitValues()
	self:BaseInitValues()     -- 一定要写

	self:ChangeValues()
	self.isOpenCards = false
end

-- 用于修改父累的参数
function UIDDZHandCardCtrl:ChangeValues()
	self.CardResourceName = "card_item_2"
	self.CardUpOffset = 25
	self.totalLen = 1110
end
---------------------------------事件--------------------------------------------
function UIDDZHandCardCtrl:DealDDZLordOpenPlayEvent(eventId, p1, p2)
	local rsp = p1

	local cardLogic = PlayGameSys.GetPlayLogic()
	local lordPlayer = cardLogic.roomObj.playerMgr:GetPlayerBySeatID(rsp.seatId)
	if lordPlayer then
		if lordPlayer.seatInfo.userId == cardLogic.roomObj:GetSouthUID() then
			self.isOpenCards = rsp.isOpen
			if self.isOpenCards then
				local transCount = #self.cardTranItems
				if transCount > 0 then
					local lastCardTrans = self.cardTranItems[#self.cardTranItems].trans
					if lastCardTrans then
						LogicUtil.SetCardOpenTag(self.luaWindowRoot, lastCardTrans, self.isOpenCards)
					end
				end
			end
		else
			self.isOpenCards = false
		end
	else
		self.isOpenCards = false
	end
end
--------------------------------------------------------------------------------

--------------------------------对外接口------------------------------------------
function UIDDZHandCardCtrl:Destroy()
	self:UnRegisterEvent()  -- 一定要这么写
	self:BaseDestroy()    -- 一定要写
end
--------------------------------------------------------------------------------

--------------------------------私有接口-----------------------------------------
function UIDDZHandCardCtrl:SetLastCardLordAndOpenTag()
	local cardLogic = PlayGameSys.GetPlayLogic()
	local lordPlayer = cardLogic.roomObj.playerMgr:GetPlayerBySeatID(cardLogic.bankerSeatId)
	if lordPlayer then
		local isLord = lordPlayer.seatPos == SeatPosEnum.South
		if isLord then
			local transCount = #self.cardTranItems
			if transCount > 0 then
				local lastCardTrans = self.cardTranItems[#self.cardTranItems].trans
				if lastCardTrans then
					LogicUtil.SetCardLordTag(self.luaWindowRoot, lastCardTrans, isLord)
					LogicUtil.SetCardOpenTag(self.luaWindowRoot, lastCardTrans, self.isOpenCards)
				end
			end
		end
	end
end
--------------------------------------------------------------------------------

---------------------------------重写父类的函数-----------------------------------
function UIDDZHandCardCtrl:InitCardShow(luaWindowRoot, trans, card, index)
	LogicUtil.InitCardItem_2(luaWindowRoot, trans, card, index, false, false)
end

function UIDDZHandCardCtrl:PlayCardSelectAudio()
	AudioManager.PlayCommonSound(UIAudioEnum.Playing_cards)
end

function UIDDZHandCardCtrl:AfterInitCards()
	self:SetLastCardLordAndOpenTag()
end

function UIDDZHandCardCtrl:AfterRefreshCards()
	self:SetLastCardLordAndOpenTag()
end

function UIDDZHandCardCtrl:AfterOutCards()
	self:SetLastCardLordAndOpenTag()
end

function UIDDZHandCardCtrl:AfterClearCards()
	self.isOpenCards = false
end
--------------------------------------------------------------------------------

return UIDDZHandCardCtrl