--[[
	@author: zx
	@date: 2017.09.26
	@desc: 麻将牌动作基类
]]

MJGameAction = class("MJGameAction", nil)
-- MJGameAction.Name = "MJGameAction"

function MJGameAction:ctor(uiSeatID, luaWindowRoot)
	self.uiSeatID = uiSeatID
	self.luaWindowRoot = luaWindowRoot
end

-- 动作执行初始化
function MJGameAction:Play(handler)
	if nil == handler then
		DwDebug.Log("nil == handler")
		return
	end

	self.handler = handler
	-- print("self.handler play:" .. self.handler.__cname)
end

-- 更新入口
function MJGameAction:Update()
end

-- 设置到完成阶段
function MJGameAction:_ToComplate()
end

-- 结束动作
function MJGameAction:Kill( complate )
	if complate then
		self:_ToComplate()
	end
end

-- 接口类
function MJGameAction:OnActionFinish( action )
end

function MJGameAction:Finish()
	local handler = self.handler
	if handler ~= nil then
		if handler.OnActionFinish then
			handler:OnActionFinish(self)
		end
	end

	MJGameActionManager.OnFinish(self)
end