--[[
	@author: zx
	@date: 2017.09.27
	@desc: 麻将出牌动作
]]

MJChuPaiAction = class("MJChuPaiAction", MJGameAction)
-- MJChuPaiAction.Name = "MJChuPaiAction"

local ActionStep = 
{
	-- START,
	UP = 1,
	MOVE = 2,
	DOWN = 3,
	END = 4,
}

function MJChuPaiAction:Play(handler, nodeList, startTransform, endPos, endIndex, moveTime)
	self.super.Play(self, handler)

	self.nodeList = nodeList
	self.startTransform = startTransform
	self.endPos = endPos
	self.endIndex = endIndex
	self.moveTime = moveTime

	-- 单位：帧数
	local time = 3
	self.time = time

	if nil == nodeList or nil == startTransform or nil == endPos or nil == endIndex or nil == moveTime then
		DwDebug.Log("some args is nil:")
		self:Finish()
		return
	end

	self.startPos = startTransform.localPosition
	self.dMove = MJGameActionManager.PositionBySeatPos[self.uiSeatID] / (time)
	self.step = ActionStep.UP

	self.shouPaiMoveAction = nil
	self.waitCount = 0

	if SeatPosEnum.South == self.uiSeatID then
		LuaEvent.AddEventNow(EEventType.ActionOperationMask, true)
	end
end

-- 设置到完成阶段
function MJChuPaiAction:_ToComplate()
	if nil ~= self.shouPaiMoveAction then
		self.shouPaiMoveAction:Kill(true)
	end

	local trans = self.startTransform
	trans.localEulerAngles = UnityEngine.Vector3.New(0,0,0)
	trans.localPosition = self.endPos
end

function MJChuPaiAction:OnActionFinish( action )
	if nil ~= self.handler then
		DwDebug.Log("handler" .. self.handler.__cname)
	end
	self.waitCount = self.waitCount - 1
end

function MJChuPaiAction:Update()
	local step = self.step
	local trans = self.startTransform
	if ActionStep.END ~= step and 0 ~= self.time then
		self.time = self.time - 1
		local pos = trans.localPosition
		pos = pos + self.dMove
		trans.localPosition = pos
	end
	if ActionStep.UP == step then
		if 0 == self.time then
			local uiSeatID = self.uiSeatID
			trans:Rotate(MJGameActionManager.RotateBySeatPos[uiSeatID])
			local time = self.moveTime
			self.time = time
			local dis = (self.endPos - self.startPos)
			self.dMove = dis / (time)
			-- 设置到最上层
			local depth = -math.abs(LogicUtil.MJCardDepth[uiSeatID])
			LogicUtil.InitMJBgCardItem(self.luaWindowRoot, trans, nil, 14 * depth)
			self.step = ActionStep.MOVE
		end
	elseif ActionStep.MOVE == step then
		local uiSeatID = self.uiSeatID
		if 5 >= self.time and nil == self.shouPaiMoveAction then
			self.waitCount = 1
			-- AudioSystem.Play("LiPai")
			self.shouPaiMoveAction = MJGameActionManager.Play(uiSeatID, MJShouPaiMoveAction, self, self.nodeList, 5)
		end
		if 0 == self.time then
			local time = 3
			self.time = time
			local manager = MJGameActionManager
			self.dRotate = (manager.RotateBySeatPos[uiSeatID] ) / (-time)
			self.dMove = (manager.PositionBySeatPos[uiSeatID] ) / (-time)
			-- 平移完成设置层级
			local depth = LogicUtil.MJCardDepth[uiSeatID]
			LogicUtil.InitMJBgCardItem(self.luaWindowRoot, trans, nil, self.endIndex * depth)
			self.step = ActionStep.DOWN
		else

		end
	elseif ActionStep.DOWN == step then
		trans:Rotate(self.dRotate)
		if 0 == self.time then
			self.step = ActionStep.END
		end
	elseif ActionStep.END == step then
		if self.waitCount == 0 then
			if SeatPosEnum.South == self.uiSeatID then
				LuaEvent.AddEventNow(EEventType.ActionOperationMask, false)
			end

			self:Finish()
		end
	end
end