--[[
	@author: zx
	@date: 2017.09.27
	@desc: 麻将出牌合并到手牌动作
]]

MJChuPaiMoveAction = class("MJChuPaiMoveAction", MJGameAction)
-- MJChuPaiMoveAction.Name = "MJChuPaiMoveAction"

local ActionStep = 
{
	MOVE = 1,
	END = 2,
}

function MJChuPaiMoveAction:Play(handler, trans, time, nodeList )
	self.super.Play(self, handler)

	if nil == trans or nil == nodeList then
		self:Finish()
		return
	end
	
	time = time or 1
	self.time = time

	local uiSeatID = self.uiSeatID

	local data = {}
	data.trans = trans

	-- 移动到手牌末尾
	-- local dMove = MJGameActionManager.SizeBySeatPos[uiSeatID] * 12 - trans.localPosition
	local dMove = LogicUtil.GetMJCardsPos(13, 14, uiSeatID)
	data.dMove = dMove / time
	data.endPos = dMove

	self.moveData = data
	self.waitCount = 1
	self.step = ActionStep.MOVE

	-- 设置深度
	local depth = LogicUtil.MJCardDepth[uiSeatID]
	self.luaWindowRoot:ChangeDepth(trans, depth)

	if SeatPosEnum.South == self.uiSeatID then
		LuaEvent.AddEventNow(EEventType.ActionOperationMask, true)
	end

	MJGameActionManager.Play(uiSeatID, MJShouPaiMoveAction, self, nodeList, time)
end

function MJChuPaiMoveAction:Update()
	if ActionStep.MOVE == self.step then
		if self.time == 0 then
			self.step = ActionStep.END
		else
			self.time = self.time - 1

			local data = self.moveData
			local trans = data.trans
			local pos = trans.localPosition
			pos = pos + data.dMove
			trans.localPosition = pos
		end
	elseif ActionStep.END == self.step then
		if self.waitCount == 0 then
			if SeatPosEnum.South == self.uiSeatID then
				LuaEvent.AddEventNow(EEventType.ActionOperationMask, false)
			end

			self:Finish()
		end
	end
end

function MJChuPaiMoveAction:_ToComplate()
	local data = self.moveData
	data.trans.localPosition = data.endPos
end

function MJChuPaiMoveAction:OnActionFinish( action )
	self.waitCount = self.waitCount - 1
end