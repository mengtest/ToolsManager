--------------------------------------------------------------------------------
--   File      : LayoutGroup.lua
--   author    : zx
--   function  : 排版组件
--   date      : 2017-12-01
--------------------------------------------------------------------------------
local Vector3 = UnityEngine.Vector3
LayoutGroup = class("LayoutGroup",nil)

function LayoutGroup:ctor()
	-- 保存组件列表
	self.cellList = {}
end

--[[
	初始化
	
	startPos -> 起始位置
	cellSize -> 默认单元格的大小
	offset -> 默认单元格之间的间隙
	lineRange -> 设置一行范围
	lineSize -> 设置换行大小
]]
function LayoutGroup:Init(startPos, cellSize, offset, lineRange, lineSize)
	-- self.luaWindowRoot = luaWindowRoot
	self.startPos = startPos
	self.cellSize = cellSize
	self.offset = offset or Vector3.zero
	self.lineRange = lineRange
	self.lineSize = lineSize
end

--[[
	添加组件
	trans -> 起始位置
	cellSize -> 默认单元格的大小
	offset -> 单元格之间的间隙
]]
function LayoutGroup:AddCell(trans, sortOrder, cellSize, offset)
	local index = #self.cellList + 1
	local data = {}
	data.trans = trans
	data.sortOrder = sortOrder or index
	data.cellSize = cellSize or self.cellSize
	data.offset = offset or self.offset

	-- table.insert(self.cellList, data)
	self.cellList[index] = data
end

--[[
	添加组件
	luaWindowRoot -> luaWindowRoot}
	cfg ->{ key = {sortOrder, cellSize, offset, isShow}}
]]
function LayoutGroup:AddCellsByCfg(luaWindowRoot, cfg, parent)
	if nil == luaWindowRoot or nil == cfg then
		return
	end

	for k,v in pairs(cfg) do
		local trans = nil
		if parent then
			trans = luaWindowRoot:GetTrans(parent, k)
		else
			trans = luaWindowRoot:GetTrans(k)
		end
		-- local trans = luaWindowRoot:GetTrans(parent, k)
		if nil ~= trans then
			self:AddCell(trans, v.sortOrder, v.cellSize, v.offset)
			luaWindowRoot:SetActive(trans, v.isShow)
		end
	end

	self:AdjustSize()
end

-- 调整位置
function LayoutGroup:AdjustSize()
	-- 排序先
	local sortFunc = function (a, b)
		return a.sortOrder < b.sortOrder
	end
	table.sort( self.cellList, sortFunc )

	-- 调整位置
	local pos = self.startPos
	local range = Vector3.zero
	local count = #self.cellList
	for i=1,count do
		local data = self.cellList[i]
		local trans = data.trans
		if trans.gameObject.activeSelf then
			local half = data.cellSize * 0.5
			data.trans.localPosition = pos + half

			if nil == self.lineRange then
				pos = pos + data.cellSize + data.offset
			else
				-- 超出range范围换行
				range.x = range.x + math.abs(data.cellSize.x + data.offset.x)
				range.y = range.y + math.abs(data.cellSize.y + data.offset.y)
				if range.x >= self.lineRange.x and range.y >= self.lineRange.y then
					if 0 == self.lineSize.x then
						pos.x = self.startPos.x
						pos.y = pos.y + self.lineSize.y
					else
						pos.y = self.startPos.y
						pos.x = pos.x + self.lineSize.x
					end
					range = Vector3.zero
				else
					pos = pos + data.cellSize + data.offset
				end
			end
		end
	end
end