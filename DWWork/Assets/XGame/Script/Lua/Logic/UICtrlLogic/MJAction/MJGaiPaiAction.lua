--[[
	@author: zx
	@date: 2017.10.13
	@desc: 手牌盖牌动画
]]

MJGaiPaiAction = class("MJGaiPaiAction", MJGameAction)
-- MJGaiPaiAction.Name = "MJGaiPaiAction"

function MJGaiPaiAction:Play(handler, nodeList, paiIds, time)
	self.super.Play(self, handler)
	
	local uiSeatID = self.uiSeatID

	self.nodeList = nodeList

	time = time or 1
	self.time = time

	if nil == nodeList then
		DwDebug.Log("some args is nodeList nil:")
		self:Finish()
		return
	end

	-- 盖下去瞬间替换掉牌面
	local count = #nodeList
	for i=1,count do
		local node = nodeList[i]
		local back = self.luaWindowRoot:GetTrans(node.trans, "Back")
		if back then
			self.luaWindowRoot:SetActive(back, true)
		end
		if nil ~= paiIds then
			local paiId = paiIds[i]
			if nil == paiId then
				break
			end

			local cardInfo = paiId.cardInfo
			if nil ~= cardInfo then
				-- ChangeSprite(node.Mask,"pai_"..paiID)
				local ico = self.luaWindowRoot:GetTrans(node.trans, "ico")
				if ico then
					self.luaWindowRoot:SetSprite(ico, cardInfo.ico)
				end
			end
		end
		node.trans.localScale = UnityEngine.Vector3.one
		-- self.luaWindowRoot:SetActive(node.trans, true)
	end

	-- 自己的id播放
	-- if uiSeatID < 20 then
	--  AudioSystem.Play("FaPai")
	-- end
end

function MJGaiPaiAction:Update()
	if self.time == 0 then
		self:_ToComplate()
		self:Finish()
	else
		self.time = self.time - 1
	end
end

function MJGaiPaiAction:_ToComplate()
	local nodeList = self.nodeList
	if nil == nodeList then
		return
	end

	local count = #nodeList
	for i=1,count do
		local node = nodeList[i]
		if nil == node then
			error("MJGaiPaiAction Complate Error node is nil")
			break
		end
		
		local back = self.luaWindowRoot:GetTrans(node.trans, "Back")
		if back then
			self.luaWindowRoot:SetActive(back, false)
		end

		node.trans.localScale = UnityEngine.Vector3.one
		-- self.luaWindowRoot:SetActive(node.trans, true)
	end

	self.nodeList = nil
end