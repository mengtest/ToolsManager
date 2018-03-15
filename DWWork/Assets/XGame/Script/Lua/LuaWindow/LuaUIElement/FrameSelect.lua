--------------------------------------------------------------------------------
-- 	 File      : FrameSelect.lua
--   author    : zs
--   function   : UI框选功能
--   date      : 2017年11月6日
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------


FrameSelect = class("FrameSelect", nil)

function FrameSelect:ctor(gb)
	self.gameObject = gb
	self.transform = gb.transform
end

function FrameSelect:Init(luaComRoot)
	-- self.m_luaComRoot = luaComRoot
end

local function GetIndexByName(gb)
	if string.find(gb.name, "_") ~= nil then
		local firstSplit = string.find(gb.name, "_")
		local id = tonumber(string.sub(gb.name, 1, firstSplit - 1))

		-- UICamera.currentTouch.current可能是各种ui，不乏123_*名字的，判断下组件
		if id then
			local com_base = gb:GetComponent("ComponentBaseLua")
			if com_base and com_base.m_fileName and com_base.m_fileName == "FrameSelect" and com_base.enabled == true then
				return id
			end
		end
	end	
	return nil
end

function FrameSelect:OnClick()
	local index = GetIndexByName(self.gameObject)
	if index then
		LuaEvent.AddEventNow(EEventType.PKClickCard, self.gameObject.transform, index)
	end
end

local dragingTrans = nil
-- 开始拖拽
function FrameSelect:OnDragStart()
	-- 如果之前的拖动还没有走完，直接返回
	if self.m_startIndex then
		return
	end

	-- 虽然是同一个gameObject，但是会被频繁改名，所以必须要每次重新取
	local index = GetIndexByName(self.gameObject)
	if index then
		-- m_startIndex为正在进行拖拽的标志
		self.m_startIndex = index
		self.m_endIndex = index
		dragingTrans = self.transform
		LateUpdateBeat:Add(self.Update, self)
	end
end

-- 需要一直刷新选中状态，必须在ondrag做
function FrameSelect:OnDrag(delta)
	if not UICamera.currentTouch.current or not self.m_startIndex then
		return
	end

	local index = GetIndexByName(UICamera.currentTouch.current)
	-- 当前触摸的物体是牌才
	if index and self.m_startIndex then
		self.m_endIndex = index
		LuaEvent.AddEvent(EEventType.ShadowCards, self.m_startIndex, self.m_endIndex)
	end
end

function FrameSelect:OnDragEnd()
	self:Reset()
end

function FrameSelect:OnPress(flag)
	if not flag then
		self:Reset()
	end
end

function FrameSelect:Reset()
	if self.m_startIndex and self.m_endIndex then
		LuaEvent.AddEventNow(EEventType.SelectCards, self.m_startIndex, self.m_endIndex)
		self.m_startIndex = nil
		LateUpdateBeat:Remove(self.Update, self)
	end
end

-- 检测点击事件
function FrameSelect:Update()
	if self.m_startIndex and self.m_endIndex then
		if not WrapSys.TouchIng() or dragingTrans ~= self.transform then
--			DwDebug.LogError("Drag", "Update", WrapSys.TouchIng(), "dragingTrans ~= self.transform", dragingTrans ~= self.transform)
			self:Reset()
		end
	end
end

return FrameSelect

