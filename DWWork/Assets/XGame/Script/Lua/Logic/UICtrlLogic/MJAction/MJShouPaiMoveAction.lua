--[[
	@author: zx
	@date: 2017.09.26
	@desc: 手牌移动动画
]]

MJShouPaiMoveAction = class("MJShouPaiMoveAction", MJGameAction)
-- MJShouPaiMoveAction.Name = "MJShouPaiMoveAction"

function MJShouPaiMoveAction:Play(handler, nodeList, time )
	self.super.Play(self, handler)

	time = time or 1
	self.time = time

	self.moveList = {}

	if nil == nodeList then
		self:Finish()
		return
	end

	local uiSeatID = self.uiSeatID
	local depth = LogicUtil.MJCardDepth[uiSeatID]
	local size = LogicUtil.MJCardSize[uiSeatID] * -1
	for dir,list in pairs(nodeList) do
		local dMove = size * dir
		for i,node in ipairs(list) do
			local data = {}
			data.trans = node.trans
			data.dMove = dMove / time
			data.endPos = data.trans.localPosition + dMove
			table.insert(self.moveList, data)
			-- 设置深度
			self.luaWindowRoot:ChangeDepth(node.trans, -depth * dir)
		end
	end

end

function MJShouPaiMoveAction:Update()
	if self.time == 0 then
		self:Finish()
	else
		self.time = self.time - 1

		for i,v in ipairs(self.moveList) do
			local trans = v.trans
			local pos = trans.localPosition
			pos = pos + v.dMove
			trans.localPosition = pos
		end
	end
end

function MJShouPaiMoveAction:_ToComplate()
	for i,v in ipairs(self.moveList) do
		if nil ~= v.trans then
			v.trans.localPosition = v.endPos
		end
	end
end