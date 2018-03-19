--[[
	@author: zx
	@date: 2017.10.13
	@desc: 发牌动画
]]

MJFaPaiAction = class("MJFaPaiAction", MJGameAction)
-- MJFaPaiAction.Name = "MJFaPaiAction"

local ActionStep = 
{
	STEP1 = 1, -- 1- 4张牌
	STEP2 = 2, -- 5- 8张牌
	STEP3 = 3, -- 8- 12张牌
	STEP4 = 4, -- 13- 14张牌
	ALL = 5, -- 全部
	END = 6, 
}

function MJFaPaiAction:Play(handler, nodeList, paiIds, time )
	self.super.Play(self, handler)

	self.nodeList = nodeList
	self.paiIds = paiIds

	time = time or 1
	self.timeSpace = time
	self.time = time

	if nil == nodeList then
		self:Finish()
		return
	end

	-- 打乱排序
	if nil ~= paiIds then
		local count = #paiIds
		local data = {}
		for i=1, count do
			if nil == data[i] then
				data[i] = i
			end

			local random = math.random(count)
			if nil == data[random] then
				data[random] = random
			end

			if i ~= random then
				data[i], data[random] = data[random], data[i]
			end
		end

		self.data = data
	end

	for i=1,14 do
		local node = nodeList[i]
		if nil ~= node then
			node.trans.localScale = UnityEngine.Vector3.zero
			-- self.luaWindowRoot:SetActive(node.trans, false)
		end
	end
	self.waitCount = 0
	self.step = ActionStep.STEP1

	LuaEvent.AddEventNow(EEventType.ActionOperationMask, true)
end

function MJFaPaiAction:Update()
	local step = self.step

	if ActionStep.STEP1 == step then
		-- 等待前面的盖牌动画播放完
		if 0 == self.waitCount then
			if 0 == self.time then
				local id = self.uiSeatID .. step
				local time = self.timeSpace
				local nodeList = {unpack(self.nodeList)}
				nodeList[14] = nil

				MJGameActionManager.Play(id, MJGaiPaiAction, self, nodeList, self.paiIds, time)

				self.waitCount = self.waitCount + 1

				-- 增加发牌音
				if self.uiSeatID == SeatPosEnum.South then
					AudioManager.PlayCommonSound(UIAudioEnum.mj_fapai)
				end

				self.step = ActionStep.END
			else
				self.time = self.time - 1
			end
		end
	elseif ActionStep.END == step then
		if self.waitCount == 0 then
			self:_ToComplate()
			self:Finish()
			LuaEvent.AddEventNow(EEventType.ActionOperationMask, false)
		end
	end
end

function MJFaPaiAction:OnActionFinish( action )
	if nil ~= self.handler then
		DwDebug.Log("handler".. self.handler.__cname)
	end
	self.waitCount = self.waitCount - 1
end

function MJFaPaiAction:_ToComplate()
	local paiIds = self.paiIds
	local nodeList = self.nodeList
	if nil == paiIds or nil == self.nodeList then
		return
	end

	for i=1,13 do
		local node = nodeList[i]
		if nil == node then
			DwDebug.LogError("MJFaPaiAction ToComplate False node is nil")
			break
		end

		local paiId = paiIds[i]
		if nil == paiId then
			DwDebug.LogError("MJFaPaiAction ToComplate False paiId is nil")
			break
		end

		paiId = paiId.cardInfo
		if nil ~= paiId then
			local ico = self.luaWindowRoot:GetTrans(node.trans, "ico")
			if ico then
				self.luaWindowRoot:SetSprite(ico, paiId.ico)
			end
		end

		local back = self.luaWindowRoot:GetTrans(node.trans, "Back")
		if back then
			self.luaWindowRoot:SetActive(back, false)
		end
		node.trans.localScale = UnityEngine.Vector3.one
		-- self.luaWindowRoot:SetActive(node.trans, true)
	end

	self.paiIds = nil
	self.nodeList = nil
end