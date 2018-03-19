--------------------------------------------------------------------------------
-- 	 File      : UICuoPaiCtrl.lua
--   author    : jianing
--   function  : 32张搓牌控件 --目前32张专用
--   date      : 2018年2月23日 10:54:57
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UICuoPaiCtrl = class("UICuoPaiCtrl",nil)

function UICuoPaiCtrl:ctor(rootTrans,luaWindowRoot)
	self.luaWindowRoot = luaWindowRoot
	self.rootTrans = rootTrans

	self.isDrag = false
	self.diffPos = Vector3.zero

	--扑克牌
	self.cardItem1 = self.luaWindowRoot:GetTrans(self.rootTrans, "1_CPCardItem")
	self.cardItem2 = self.luaWindowRoot:GetTrans(self.rootTrans, "2_CPCardItem")
	self.cards = {}
	table.insert(self.cards, self.cardItem1)
	table.insert(self.cards, self.cardItem2)

	self:RegisterEvent()
end

function UICuoPaiCtrl:RegisterEvent()
	LuaEvent.AddHandle(EEventType.ThirtyTwo_ShowCuoPai, self.ShowCuoPai, self)
	LuaEvent.AddHandle(EEventType.ThirtyTwo_HideCuoPai, self.HideCuoPai, self)
end

function UICuoPaiCtrl:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.ThirtyTwo_ShowCuoPai, self.ShowCuoPai, self)
	LuaEvent.RemoveHandle(EEventType.ThirtyTwo_HideCuoPai, self.HideCuoPai, self)
end

--显示搓牌组建 p1 是否显示搓牌动画 p2 牌的数据
function UICuoPaiCtrl:ShowCuoPai(eventId,isShowAnimation,pai)
	--隐藏发完的牌
	LuaEvent.AddEvent(EEventType.HideHandCard,SeatPosEnum.South)

	if isShowAnimation then
		self:ShowBeginAnimation(pai)
	else
		self:ShowCuoPaiState(pai)
	end
end

--
function UICuoPaiCtrl:HandleWidgetPressed(gb,isPress)
	if gb.name == "2_CPCardItem" and isPress then
		self:SetDragCardPos(true)
		self.isDrag = true
	else
		self.isDrag = false
		self.luaWindowRoot:PlayTweenPos(self.cardItem2, self.cardItem2.localPosition, Vector3.zero, 0.2, false)
	end
end

function UICuoPaiCtrl:Update()
	if self.isDrag then
		self:SetDragCardPos()
	end
end

--拖拽图片
function UICuoPaiCtrl:SetDragCardPos(isBegin)
	local pos = WrapSys.GetCurrentTouchPos(self.cardItem2.gameObject.layer)
	if isBegin then
		--位移差
		self.diffPos = self.cardItem2.position - pos
	else
		local targetPos = pos + self.diffPos

		if targetPos.x > - 0.6 then
			targetPos.x = -0.6
		elseif targetPos.x < -1.3 then
			targetPos.x = -1.3
		end

		if targetPos.y > 0 then
			targetPos.y = 0
		elseif targetPos.y < -0.8 then
			targetPos.y = -0.8
		end

		self.cardItem2.position = targetPos
	end
end

--开始的一个动画
function UICuoPaiCtrl:ShowBeginAnimation(pai)
	--设置牌背
	for i=1,#self.cards do
		self.luaWindowRoot:SetActive(self.cards[i], true)
		LogicUtil.InitPKCard(self.luaWindowRoot,self.cards[i],nil,i)
		self.cards[i].localScale = Vector3.New(1,1,0)
	end

	--做动画
	local fromPos = Vector3.New(140,0,0)
	self.cardItem2.localPosition = fromPos

	local toPos = Vector3.zero
	self.luaWindowRoot:PlayTweenPos(self.cardItem2, fromPos, toPos, 0.2, false)

	--显示搓牌状态
	self.tween = TimerTaskSys.AddTimerEventByLeftTime(function ()
		self:ShowCuoPaiState(pai)
		for i=1,#self.cards do
			self.cards[i].localScale = Vector3.New(1.2,1.2,0)
		end
	end, 0.2, nil)
end

--显示搓牌状态
function UICuoPaiCtrl:ShowCuoPaiState(pai)
	if pai == nil or type(pai) ~= "table" or #pai < 2 then
		return
	end
	self.tween = nil

	self.cardItem2.localPosition = Vector3.zero
	--显示正面
	for i=1,#self.cards do
		self.luaWindowRoot:SetActive(self.cards[i], true)

		local card = CCard.New()
		card:Init(pai[i])
		LogicUtil.InitPKCard(self.luaWindowRoot,self.cards[i], card,i)
	end
end

--隐藏搓牌
function UICuoPaiCtrl:HideCuoPai(eventId,p1,p2)
	self.isDrag = false

	if self.tween then
		self.tween = nil
		TimerTaskSys.RemoveTask(self.tween)
	end

	for i=1,#self.cards do
		self.luaWindowRoot:SetActive(self.cards[i], false)
		LogicUtil.InitPKCard(self.luaWindowRoot,self.cards[i],nil,i)
	end
end

function UICuoPaiCtrl:Destroy()
	self:UnRegisterEvent()
end
