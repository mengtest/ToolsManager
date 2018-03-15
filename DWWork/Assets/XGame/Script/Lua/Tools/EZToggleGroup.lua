--------------------------------------------------------------------------------
--   File      : EZToggleGroup.lua
--   author    : jianing
--   function  : toggle 单选
--   date      : 2017-10-21
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

EZToggleGroup = class("EZToggleGroup",nil)

function EZToggleGroup:ctor()
	self.list = {}	--组里面trans
end

function EZToggleGroup:Init(m_luaWindowRoot)
	self.m_luaWindowRoot = m_luaWindowRoot
end

--添加一项
function EZToggleGroup:AddTrans(trans, group, select, multiple)
	if trans then
		--已经在一个组里面了 直接修改参数
		local toggle = self:GetToggle(trans)
		local hasToggle = false
		if toggle then
			hasToggle = true
		else
			toggle = {}
			hasToggle = false
		end
		
		toggle.trans = trans
		if select == nil then
			select = false
		end
		toggle.select = select
		toggle.group = group
		-- 复选模式
		toggle.multiple = multiple

		if not hasToggle then
			table.insert(self.list,toggle)
		end
	end
end

--选择之后执行的方法
local function DefaultSelect(trans,isSelect,m_luaWindowRoot)
	local selectTrans = m_luaWindowRoot:GetTrans(trans,"select")
	local unSelectTrans = m_luaWindowRoot:GetTrans(trans,"unSelect")
	m_luaWindowRoot:SetActive(selectTrans, isSelect)
	m_luaWindowRoot:SetActive(unSelectTrans, not isSelect)
end

--自定义一些选中和取消方法 没有就执行上面默认方法
function EZToggleGroup:SetSelectFun(selectFun)
	self.selectFun = selectFun
end

-- 调用选中函数
function EZToggleGroup:DoSelectFun(trans, select)
	if nil ~= self.selectFun then
		self.selectFun(trans, select)
	else
		DefaultSelect(trans, select, self.m_luaWindowRoot)
	end
end

--获得组
function EZToggleGroup:GetToggle(trans)
	for i=1,#self.list do
		local curToggle = self.list[i]
		if curToggle.trans == trans then
			return curToggle
		end
	end
	return nil
end

--刷新显示
function EZToggleGroup:RefreshSelect(group)
	for i=1,#self.list do
		local curToggle = self.list[i]
		if group == nil or group == curToggle.group then
			self:DoSelectFun(curToggle.trans, curToggle.select)
		end
	end
end

--选中
function EZToggleGroup:Select(trans)
	local current = self:GetToggle(trans)
	if not current then
		return
	end

	if current.multiple then
		current.select = not current.select
	else
		if not current.select then
			for i=1,#self.list do
				local curToggle = self.list[i]
				if curToggle.group == current.group then	--只处理同组的
					local isSelect = current.trans == curToggle.trans
					curToggle.select = isSelect
				end
			end
		end
	end

	self:RefreshSelect(current.group)
end

--获取当前选择中的Trans和序号
function EZToggleGroup:GetSelectTrans(group)
	local index = 0
	for i=1,#self.list do
		local curToggle = self.list[i]
		if curToggle.group == group then
			index = index + 1
			if curToggle.select then
				return curToggle.trans, index
			end
		end
	end
end

-- 获取复选模式下选中序号
function EZToggleGroup:GetSelectMultipleTrans(group)
	local list = {}

	local index = 0
	for i=1, #self.list do
		local curToggle = self.list[i]
		if curToggle.group == group then
			index = index + 1
			if curToggle.select then
				list[#list+1] = {trans = curToggle.trans, index = index}
			end
		end
	end

	return list
end