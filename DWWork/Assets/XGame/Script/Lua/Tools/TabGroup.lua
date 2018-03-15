--------------------------------------------------------------------------------
--   File      : TabGroup.lua
--   author    : zx
--   function  : 通用页签组件
--   date      : 2017-12-22
--------------------------------------------------------------------------------

TabGroup = class("TabGroup",nil)

-- 
function TabGroup:ctor(luaWindowRoot)
	self.luaWindowRoot = luaWindowRoot
	self.panelScrollView = nil
end

-- 初始化
--[[
	rootTrans -> 根节点
	tabName -> 页签资源名
	cellSize -> 大小
	offset -> 偏移
	direction -> 滑动方向
	initCallBack -> 初始化回调
	selectFunc -> 是否选中判断
	count -> 列表数量
--]]
function TabGroup:Init(rootTrans, itemName, cellSize, offset, direction, initCallBack, selectFunc, handle, count)
	if self.panelScrollView == nil then
		local item = self.luaWindowRoot:GetTrans(itemName).gameObject
		if nil == item then
			error("tab group init no found" .. itemName)
		end

		self.panelScrollView = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
		self.panelScrollView:InitForLua(rootTrans, item, cellSize, offset, direction, false)
		
		local selectTabs = function (trans, index)
			local isSelect = nil ~= selectFunc and selectFunc(handle, index) or false

			local selectTrans = self.luaWindowRoot:GetTrans(trans,"select")
			local unSelectTrans = self.luaWindowRoot:GetTrans(trans,"unSelect")
			self.luaWindowRoot:SetActive(selectTrans, isSelect)
			self.luaWindowRoot:SetActive(unSelectTrans, not isSelect)
			local arrow = self.luaWindowRoot:GetTrans(trans, "arrow")
			if nil ~= arrow then
				self.luaWindowRoot:SetActive(arrow, isSelect)
			else
				--print("not found arrow")
			end
		end

		local initFunc = function (trans, item_index)
			local index = item_index + 1

			trans.name = "tab_" .. item_index
			
			if nil ~= initCallBack then
				initCallBack(handle, trans, index)
			end

			selectTabs(trans, item_index, isSelect)
		end
		self.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(initFunc)
		self.panelScrollView:SetInitItemCallLua(self.m_initFuncSeq)

		self.m_selectFuncSeq = LuaCsharpFuncSys.RegisterFunc(selectTabs)
		self.panelScrollView:SetSelectOrUnSelectFunc(self.m_selectFuncSeq)
	end
	self.panelScrollView:InitItemCount(count, true)
	self.panelScrollView.SelectIndex = 0
end

-- 选中用页签
function TabGroup:Select(index)
	self.panelScrollView.SelectIndex = index - 1
end