--------------------------------------------------------------------------------
-- 	 File      : BaseModule.lua
--   author    : shandong   shandong@ezfun.cn
--   version   : 1.0
--   date      : 2016-07-29 11:04:50.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------

BaseLuaModule = class("BaseLuaModule", nil)

function BaseLuaModule:ctor(self, gameobj)
	--gameobj = gameobj[0]
	--构造函数
	self.m_gameObject = gameobj
	self.m_module = gameobj:GetComponent("LuaModule")
	print(gameobj)
end

function BaseLuaModule:BaseCreateModule()
	print("BaseCreateModule")
	if (self.CreateModule) then
		self:CreateModule()
	end
end


function BaseLuaModule:BaseInitModule(isOpen, state)
	print("BaseInitModule")
	if (self.InitModule) then
		self:InitModule(isOpen, state)
	end
end

function  BaseLuaModule:BaseHandleWidgetClick(gameObj)
	if (self.HandleWidgetClick) then
		self:HandleWidgetClick(gameObj)
	end

end

function BaseLuaModule:BaseDestroyModule()

	if (self.DestroyModule) then
		self:DestroyModule();
	end
end

return BaseLuaModule