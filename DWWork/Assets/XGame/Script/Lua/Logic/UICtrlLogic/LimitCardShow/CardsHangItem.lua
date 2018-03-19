--------------------------------------------------------------------------------
-- 	 File       : CardsHangItem.lua
--   author     : zhanghaochun
--   function   : 指定数量的扑克牌显示控件
--   date       : 2018-01-15
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

CardsHangItem = class("CardsHangItem")

function CardsHangItem:ctor(rootTrans, seatPos, luaWindowRoot)
	self.rootTrans = rootTrans
	self.seatPos = seatPos
	self.luaWindowRoot = luaWindowRoot

	self:InitValues()
	self:InitComponents()
end

function CardsHangItem:InitValues()
	self.transList = {}
	self.index = 1
	self.cardsInfo = {}
	self.distance = 140
end

function CardsHangItem:InitComponents()
	self.CenterTrans = self.luaWindowRoot:GetTrans(self.rootTrans, "CenterPos")
end
---------------------------------对外接口----------------------------------------
-- 发牌
function CardsHangItem:GiveACard()
	if self.index > #self.transList then return end
	
	local card = self.cardsInfo[self.index]
	local transItem = self.transList[self.index]
	transItem.trans.localScale = Vector3.one

	LogicUtil.InitPKCard(self.luaWindowRoot, transItem.trans, card, self.index)

	AudioManager.PlayCommonSound(UIAudioEnum.faPai)
	local fromPos = self.CenterTrans.localPosition
	local toPos = Vector3.zero
	toPos.x = (self.index - 1) * self.distance
	self.luaWindowRoot:PlayTweenPos(transItem.trans, fromPos, toPos, 0.2, false)
	self.index = self.index + 1
end

-- 显示牌背
function CardsHangItem:SetCardsBack(cardsNum)
	self:RecoveryCardsBack(cardsNum)
end

-- 显示牌面
function CardsHangItem:SetCardsFront(cards)
	self:RecoveryCardsFront(cards)
end

-- 恢复牌背显示
function CardsHangItem:RecoveryCardsBack(cardsNum)
	self:CheckTransNum(cardsNum)
	
	for k, v in ipairs(self.transList) do
		self.luaWindowRoot:PlayAndStopTweens(v.trans, false)
		local pos = Vector3.zero
		pos.x = (k - 1) * self.distance
		v.trans.localPosition = pos
		v.trans.localScale = Vector3.one
		LogicUtil.InitPKCard(self.luaWindowRoot, v.trans, nil, k)
	end
end

-- 恢复牌面显示
function CardsHangItem:RecoveryCardsFront(cards)
	self:CheckTransNum(#cards)

	for k, v in ipairs(self.transList) do
		self.luaWindowRoot:PlayAndStopTweens(v.trans, false)
		local pos = Vector3.zero
		pos.x = (k - 1) * self.distance
		v.trans.localPosition = pos
		v.trans.localScale = Vector3.one
		LogicUtil.InitPKCard(self.luaWindowRoot, v.trans, cards[k], k)
	end
end

-- 隐藏牌
function CardsHangItem:HideCards()
	self:HideAllCard()
end

-- 设置牌的数量
function CardsHangItem:SetCardsNum(num)
	self:CheckTransNum(num)
end

-- 
function CardsHangItem:Reset(cardsInfo)
	self.cardsInfo = cardsInfo or {}
	self.index = 1
end
--------------------------------------------------------------------------------

---------------------------------私有方法----------------------------------------
function CardsHangItem:CheckTransNum(newNum)
	local curNum = #self.transList
	local minNum = math.min(curNum, newNum)

	if newNum > curNum then
		-- 创建新的牌
		for i = curNum + 1, newNum do
			local obj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "card_item", RessStorgeType.RST_Never, false)
			local trans = obj.transform
			LogicUtil.EnableTouch(trans, false)
			trans.parent = self.rootTrans
			trans.localScale = Vector3.one
			self.luaWindowRoot:SetActive(trans, true)
			trans.name = i .. "_HandCardItem"
			
			local cardTrans = {}
			cardTrans.trans = trans
			cardTrans.info = nil
			table.insert(self.transList, cardTrans)
		end
	elseif newNum < curNum then
		-- 隐藏不需要的牌
		for i = newNum + 1, curNum do
			self.transList[i].trans.localScale = Vector3.zero
		end
	else
		return
	end
end

function CardsHangItem:HideAllCard()
	for k, v in ipairs(self.transList) do
		v.trans.localScale = Vector3.zero
	end
end
--------------------------------------------------------------------------------

-- 销毁
function CardsHangItem:Destroy()
	self:HideAllCard()
	self.transList = {}
	
	self.rootTrans = nil
	self.luaWindowRoot = nil
	self.seatPos = nil
end