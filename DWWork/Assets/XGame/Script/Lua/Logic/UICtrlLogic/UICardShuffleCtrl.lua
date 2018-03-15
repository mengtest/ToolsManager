--------------------------------------------------------------------------------
-- 	 File       : UICardShuffleCtrl.lua
--   author     : zhanghaochun
--   function   : 扑克洗牌控件
--   date       : 2018-01-18
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

UICardShuffleCtrl = class("UICardShuffleCtrl")

local PKCardsShuffleNum = 
{
	[Common_PlayID.ThirtyTwo] = 32,
}

function UICardShuffleCtrl:ctor(rootTrans, luaWindowRoot, playID)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	self.playID = playID

	self:InitValues()
	self:RegisterEvent()
end

function UICardShuffleCtrl:InitValues()
	self.PKDefaultNum = PKCardsShuffleNum[self.playID]
	if self.PKDefaultNum == nil then
		logError("Not have this play config. Please config PKCardsShuffleNum or check init playID, playID is : " .. tostring(self.playID) .. ". Now Set Default Num 32")
		self.PKDefaultNum = 32
	end
	self.itemList = {}


	self.isRegisterTimer = false
	-- 洗牌的次数
	self.shuffleTime = 2
	-- 当前洗牌次数
	self.curTime = 1
	self.curShuffleStep = 1
	self.timer = 0

	self.OutSpacePos = Vector3.New(0, 10000, 0)
end

function UICardShuffleCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.StartShuffleCards, self.DealStartShuffleCardsEvent, self)
end

function UICardShuffleCtrl:UnRegisterEvnet()
	LuaEvent.RemoveHandle(EEventType.StartShuffleCards, self.DealStartShuffleCardsEvent, self)
end

-----------------------------------接口------------------------------------------
function UICardShuffleCtrl:StartShuffle(shuffleTime)
	if self.isRegisterTimer then
		UpdateBeat:Remove(self.ShuffleUpdateFunc, self)
		self.isRegisterTimer = false
	end
	self.shuffleTime = shuffleTime
	self.timer = 0
	self.curShuffleStep = 1
	self.curTime = 1
	
	-- 检查牌的初始数量，并显示出来
	self:CheckCardsItemNum(self.PKDefaultNum)

	-- 设置牌的初始位置
	self:SetCardsStartPos()
	
	if not self.isRegisterTimer then
		UpdateBeat:Add(self.ShuffleUpdateFunc, self)
		self.isRegisterTimer = true
	end
end

function UICardShuffleCtrl:Destroy()
	self:Hide()
	self:UnRegisterEvnet()
	
	self.itemList = {}

	self.rootTrans = nil
	self.luaWindowRoot = nil
end
--------------------------------------------------------------------------------

------------------------------------事件-----------------------------------------
function UICardShuffleCtrl:DealStartShuffleCardsEvent(evendId, p1, p2)
	self:StartShuffle(4)
end
--------------------------------------------------------------------------------

----------------------------------私有接口----------------------------------------
function UICardShuffleCtrl:CheckCardsItemNum(num)
	local curNum = #self.itemList
	local minNum = math.min(curNum, num)

	if num > curNum then
		-- 创建新的牌
		for i = curNum + 1, num, 1 do
			local obj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "card_item", RessStorgeType.RST_Never, false)
			local trans = obj.transform
			LogicUtil.EnableTouch(trans, false)
			trans.parent = self.rootTrans
			trans.localScale = Vector3.one
			self.luaWindowRoot:SetActive(trans, true)
			trans.name = i .. "_ShuffleCardItem"

			LogicUtil.InitPKCard(self.luaWindowRoot, trans)

			local item = {}
			item.trans = trans
			item.Xoffset = 0
			table.insert(self.itemList, item)
		end
	elseif num < curNum then
		-- 隐藏不需要的牌
		for i = num + 1, curNum, 1 do
			--self.luaWindowRoot:SetActive(self.itemList[i].trans, false)
			--self.itemList[i].trans.localPosition = self.OutSpacePos
			self.itemList[i].trans.localScale = Vector3.zero
		end
	else
		return
	end
end

function UICardShuffleCtrl:Hide()
	for k, v in ipairs(self.itemList) do
		--self.luaWindowRoot:SetActive(v.trans, false)
		--v.trans.localPosition = self.OutSpacePos
		v.trans.localScale = Vector3.zero
	end
end

function UICardShuffleCtrl:SetCardsStartPos()
	for k, v in ipairs(self.itemList) do
		v.trans.localScale = Vector3.one
		v.trans.localPosition = self:GetIndexPos(k)
	end
end

function UICardShuffleCtrl:GetIndexPos(index)
	local pos = Vector3.zero
	pos.x = -((index - 1) * 0.5)
	pos.y = (index - 1) * 0.6
	return pos
end

function UICardShuffleCtrl:SetRandom()
	local listLen = #self.itemList
	for k, v in ipairs(self.itemList) do
		local index = math.random(1, listLen)
		if k ~= index then
			self.itemList[k], self.itemList[index] = self.itemList[index], self.itemList[k]
		end
	end

	for k, v in ipairs(self.itemList) do
		local offset = math.random(-100, 100)
		v.Xoffset = offset
	end
end

function UICardShuffleCtrl:ShuffleUpdateFunc()
	
	self.timer = self.timer + UnityEngine.Time.timeScale
	if self.timer >= 0.1 then
		if self.curShuffleStep == 1 then
			self:SetRandom()
			for k, v in ipairs(self.itemList) do
				LogicUtil.InitPKCard(self.luaWindowRoot, v.trans, nil, k)
				local toPos = v.trans.localPosition
				toPos.x = v.Xoffset
				self.luaWindowRoot:PlayTweenPos(v.trans, v.trans.localPosition, toPos, 0.1, false)
			end
		elseif self.curShuffleStep == 4 then
			for k, v in ipairs(self.itemList) do
				self.luaWindowRoot:PlayTweenPos(v.trans, v.trans.localPosition, self:GetIndexPos(k), 0.1, false)
			end
		elseif self.curShuffleStep == 7 then
			self.curTime = self.curTime + 1
		end
		
			
		self.curShuffleStep = self.curShuffleStep + 1
		if self.curShuffleStep > 7 then
			self.curShuffleStep = 1
		end
		self.timer = 0
	end

	if self.curTime > self.shuffleTime then
		-- 洗牌结束
		self.isRegisterTimer = false
		UpdateBeat:Remove(self.ShuffleUpdateFunc, self)
		LuaEvent.AddEventNow(EEventType.CardsShuffleEnd)
		self:Hide()
	end
end
--------------------------------------------------------------------------------

