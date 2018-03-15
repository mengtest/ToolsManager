--------------------------------------------------------------------------------
--   File	  : CreateRoomCtrl.lua
--   author	: zx
--   function  : 创建房间控件
--   date	  : 2018-01-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Tools.EZToggleGroup"
require "Tools.LayoutGroup"
require "Tools.TabGroup"
require "LuaSys.CreateRoomConfig"
require "3rd.MathBit"

CreateRoomCtrl = class("CreateRoomCtrl",nil)

local Vector3 = UnityEngine.Vector3
local Vector2 = UnityEngine.Vector2
-------------------------------------------------------------------------------
-- 读取默认配置
local function readGameConfig(playID)
	local def = CreateRoomConfig.GameToggleDefValue[playID]
	local str = PlayerDataRefUtil.GetString("CreateRoomConfig" .. playID, ToString(def))
	local code, ret = pcall(loadstring(string.format("do local _=%s return _ end", str)))
	return code and ret or def
end

-- 保存默认配置
local function saveGameConfig(playID, config)
	PlayerDataRefUtil.SetString("CreateRoomConfig" .. playID, ToString(config))
end
--------------------------------------------------------------------------------

function CreateRoomCtrl:ctor(luaWindowRoot)
	self.m_luaWindowRoot = luaWindowRoot
	self.clubID = 0
	self.playID = 0
	self.m_EZToggleGroup = nil --单选框集合
	self.m_tabGroup = nil --页签组件
	self.m_gameTabList = {} --页签列表
	self.m_panelRoot = nil -- 界面根节点
	self.m_panelComponent = nil --界面组件
	self.m_springPanelComponent = nil -- 滑动组件
	self.m_gamePanelTable = {} --保存已加载的界面
	self.m_toggleList = {} --保存加载toggle组件
end

-------------------------------------------------------------------------------
function CreateRoomCtrl:_initGameTab(trans, index)
	local playID = self.m_gameTabList[index].playID

	local tabConfig = Common_PlayText[playID]
	if nil == tabConfig or nil == tabConfig.name then
		DwDebug.LogError("init tab no foud game playID:"..playID)
		return
	end

	-- 设置文本
	self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "Label"), tabConfig.name, false)
	self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "UnLabel"), tabConfig.name, false)
end

function CreateRoomCtrl:_isSelect(index)
	return self.m_gameTabList[index+1].playID == self.playID
end

-- 房卡关联人数和平均支付
function CreateRoomCtrl:_adjustRoomCard(playID)
	local toggleConfig = CreateRoomConfig.GameToggleConfig[playID]
	if nil == toggleConfig then
		DwDebug.LogError("adjust room card no foud game playID:" .. playID)
		return nil
	end

	local name = "game" .. playID
	local renShuTrans, renShuSelIndex = self.m_EZToggleGroup:GetSelectTrans(name .. "RenShu_Node")
	local zhiFuTrans, zhiFuSelIndex = self.m_EZToggleGroup:GetSelectTrans(name .. "ZhiFu_Node")

	local renShuValue = nil
	if nil == renShuTrans or nil == renShuSelIndex then
		if playID == Common_PlayID.DW_DouDiZhu then
			renShuValue = 3
		else
			renShuValue = 4
		end
	elseif playID == Common_PlayID.DW_DouDiZhu then
		renShuValue = 3
	else
		renShuValue = toggleConfig["RenShu_Node"][renShuSelIndex].value
	end

	local zhiFuValue = nil
	if 0 ~= self.clubID then
		-- 群主支付
		zhiFuValue = 3
	elseif nil == zhiFuTrans or nil == zhiFuSelIndex then
		-- 房主支付
		zhiFuValue = 1
	else
		zhiFuValue = toggleConfig["ZhiFu_Node"][zhiFuSelIndex].value
	end

	local roomCard = 1
	if 1 == zhiFuValue or 3 == zhiFuValue then
		roomCard = renShuValue
	end

	local panel = self.m_luaWindowRoot:GetTrans(name .. "Panel")
	local config = toggleConfig["JuShu_Node"]
	for i,data in ipairs(config) do
		local trans = self.m_luaWindowRoot:GetTrans(panel, "toggle_JuShu_Node" .. i)
		local label = string.format(data.subLabel, roomCard * i)
		self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "selectsubLabel"), label, false)
		self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "unselectsubLabel"), label, false)
	end
end

-- 初始化选项
function CreateRoomCtrl:_initToggleNode(playID, data, i, node, panelRoot, isSelect)
	local parent = self.m_luaWindowRoot:GetTrans(panelRoot, node)
	local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, CreateRoomConfig.GameToggleItem, parent, RessStorgeType.RST_Never)
	if not resObj then
		DwDebug.LogError("init toggle no foud asset:" .. CreateRoomConfig.GameToggleItem)
		return
	end

	resObj.name = "toggle_" .. node .. i
	local trans = resObj.transform
	self.m_luaWindowRoot:SetActive(trans, true)

	self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "selectlabel"), data.label, false)
	if nil ~= data.subLabel then
		self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "selectsubLabel"), data.subLabel, false)
	else
		self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "selectsubLabel"), "", false)
	end

	self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "unselectlabel"), data.label, false)
	if nil ~= data.subLabel then
		self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "unselectsubLabel"), data.subLabel, false)
	else
		self.m_luaWindowRoot:SetLabel(self.m_luaWindowRoot:GetTrans(trans, "unselectsubLabel"), "", false)
	end

	local groupName = "game" .. playID .. node
	self.m_EZToggleGroup:AddTrans(trans, groupName, isSelect, data.multiple)
	table.insert(self.m_toggleList, trans)

	return trans, i, nil, data.offset
end

-- 初始化界面
function CreateRoomCtrl:_initGamePanel(playID)
	-- 清空之前的选项
	self:_clearAsset()

	local panelTrans = self.m_gamePanelTable[playID]
	if nil ~= panelTrans then
		return panelTrans
	end

	local panelConfig = CreateRoomConfig.GamePanelConfig[playID]
	local defValueConfig = CreateRoomConfig.GameToggleDefValue[playID]
	if nil == panelConfig or nil == defValueConfig then
		DwDebug.LogError("init panel no foud game playID:"..playID)
		return nil
	end

	local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, panelConfig, self.m_panelRoot, RessStorgeType.RST_Never)
	if not resObj then
		DwDebug.LogError("init panel no foud asset:" .. panelConfig)
		return
	end
	resObj.name = "game" .. playID .. "Panel"
	local trans = resObj.transform
	self.m_luaWindowRoot:SetActive(trans, true)

	-- 初始化选项
	local gameConfig = readGameConfig(playID)
	local toggleConfig = CreateRoomConfig.GameToggleConfig[playID]
	if nil == gameConfig or nil == toggleConfig then
		DwDebug.LogError("init panel no foud game playID:"..playID)
		return nil
	end

	local statSize = Vector3.New(49, 0, 0)
	local cellSize = Vector3.New(200, 0, 0)
	local offset = Vector3.New(120, 0, 0)
	local lineRange = Vector3.New(640, 0, 0)
	local lineSize = Vector3.New(0, -65, 0)
	for node,config in pairs(toggleConfig) do
		local toggleGroup = LayoutGroup.New()
		toggleGroup:Init(statSize, cellSize, offset, lineRange, lineSize)
		for i,data in ipairs(config) do
			local isSelect = false
			local def = gameConfig[node] or defValueConfig[node]
			if type(def) == "number" then
				isSelect = MathBit.andOp(MathBit.lShiftOp(1, i - 1), def) > 0
			else
				isSelect = MathBit.andOp(MathBit.lShiftOp(1, i - 1), defValueConfig[node]) > 0
			end
			toggleGroup:AddCell(self:_initToggleNode(playID, data, i, node, trans, isSelect))
		end
		toggleGroup:AdjustSize()
	end

	self.m_EZToggleGroup:RefreshSelect()
	-- 更新房卡显示
	self:_adjustRoomCard(playID)

	self.m_gamePanelTable[playID] = trans
	return trans
end

-- 显示界面
function CreateRoomCtrl:_showPanel(playID)
	self.m_luaWindowRoot:ShowChild(self.m_panelRoot, "game" .. playID .. "Panel", true)
	self.m_panelRoot.localPosition = Vector3.zero
	self.m_panelComponent.clipOffset = Vector2.zero
	
	-- SpringPanel这个组件是uiscrollview动态生成的，初始化时可能没有创建
	if not self.m_springPanelComponent then
		self.m_springPanelComponent = self.m_panelRoot:GetComponent("SpringPanel")
	end

	if self.m_springPanelComponent then
		self.m_springPanelComponent.target = Vector3.zero
	end

	-- 节点调整
	local contentConfig = (0 == self.clubID) and CreateRoomConfig.GameContentConfig[playID] or CreateRoomConfig.ClubGameContentConfig[playID]
	if nil == contentConfig then
		DwDebug.LogError("init panel no foud game contentConfig playID:"..playID)
		return 
	end

	local panelTrans = self.m_gamePanelTable[playID]
	if nil == panelTrans then
		DwDebug.LogError("init panel no foud nil == panelTrans:"..playID)
		return 
	end

	for content, config in pairs(contentConfig) do
		local contentNode = self.m_luaWindowRoot:GetTrans(panelTrans, content)
		contentNode.localPosition = config.Pos
		self.m_luaWindowRoot:ResetSpriteSize(contentNode, config.Size)

		local nodeConfig = config.NodeConfig
		if nil ~= nodeConfig then
			local contentGroup = LayoutGroup.New()
			contentGroup:Init(Vector3.New(-370, 3, 0), Vector3.New(0, -68, 0), Vector3.zero, nil, nil)
			contentGroup:AddCellsByCfg(self.m_luaWindowRoot, nodeConfig, panelTrans)
		end
	end
end

-- 界面资源清理
function CreateRoomCtrl:_clearAsset()
	for playID,trans in pairs(self.m_gamePanelTable) do
		self.m_luaWindowRoot:SetActive(trans, false)
	end
	self.m_gamePanelTable = {}

	local count = #self.m_toggleList
	for i=1,count do
		self.m_luaWindowRoot:SetActive(self.m_toggleList[i], false)
	end
	self.m_toggleList = {}
end
-------------------------------------------------------------------------------

-------------------------------------------------------------------------------
function CreateRoomCtrl:Init(clubID, gameList, defPlayID, size, cell)
	self.clubID = clubID
	self.m_panelRoot = self.m_luaWindowRoot:GetTrans("gameScrollView")
	self.m_panelComponent = self.m_panelRoot:GetComponent("UIPanel")
	
	self.m_EZToggleGroup = EZToggleGroup.New()
	self.m_EZToggleGroup:Init(self.m_luaWindowRoot)

	self.m_gameTabList = gameList

	self.playID = defPlayID

	if nil == self.m_tabGroup then
		self.m_tabGroup = TabGroup.New(self.m_luaWindowRoot)
	end

	self.m_tabGroup:Init(self.m_luaWindowRoot:GetTrans("tabRoot"), "createRoom_tab_item", size, cell, LimitScrollViewDirection.SVD_Vertical, self._initGameTab, self._isSelect, self, #self.m_gameTabList)

	self:_initGamePanel(self.playID)
	self:_showPanel(self.playID)

	self.m_EZToggleGroup:RefreshSelect()
end

-- 保存房间配置
function CreateRoomCtrl:SaveCreateRoom()
	local toggleConfig = CreateRoomConfig.GameToggleConfig[self.playID]
	local createConfig = CreateRoomConfig.GameCreateConfig[self.playID]
	if nil == toggleConfig or nil == createConfig then
		DwDebug.LogError("create room no foud game playID:"..self.playID)
		return nil
	end

	local gameConfig = {}
	local config = {}

	local webConfig = {}
	local webNode = 
	{
		JuShu_Node = "gameNum",
		RenShu_Node = "playerNum",
		ZhiFu_Node = "zhiFu",
	}
	local panelName = "game" .. self.playID
	for node,tgConfig in pairs(toggleConfig) do
		local groupName = panelName .. node

		gameConfig[node] = 0
		local selValue = 0
		local selIndex = 0
		local list = self.m_EZToggleGroup:GetSelectMultipleTrans(groupName)
		if nil ~= list and 0 ~= #list then
			for i=1,#list do
				local index = list[i].index
				local config = tgConfig[index]
				if not config or nil == config.value then
					DwDebug.LogError("m_EZToggleGroup:GetSelectTrans groupName is:", groupName, "selIndex is:", index, "node:", node)
				else
					-- gameConfig[node][index] = true
					if type(config.value) == "number" then
						selValue = selValue + config.value
					else
						selValue = config.value
					end
					gameConfig[node] = MathBit.orOp(gameConfig[node], MathBit.lShiftOp(1, index - 1))
				end
			end
		end

		-- 单独设置房主支付
		if 0 ~= self.clubID and "ZhiFu_Node" == node then
			selValue = 3
		end

		if nil ~= createConfig[node] then
			config[createConfig[node]] = selValue
		end
		if nil ~= webNode[node] then
			webConfig[webNode[node]] = selValue
		end
	end



	webConfig.clubGroupId, config.clubGroupId = self.clubID, self.clubID
	-- 保存配置
	saveGameConfig(self.playID, gameConfig)

	return self.playID, config, webConfig
end

-- 点击事件处理
-- gb --> 选中的gameObject
-- name --> 选中物件的名字
function CreateRoomCtrl:HandleWidgetClick(gb, name)
	if name == "tab_collider" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")

		local index = tonumber(string.sub(gb.transform.parent.name, string.len("tab_") + 1))
		index = index + 1
		if nil ~= self.m_gameTabList[index] then
			local playID = self.m_gameTabList[index].playID
			if self.playID ~= playID then
				self:_initGamePanel(playID)
				self:_showPanel(playID)
				self.playID = playID
				self.m_tabGroup:Select(index)
			end
		end
	elseif string.find(name, "toggle_") then
		self.m_EZToggleGroup:Select(gb.transform)
		if string.find(name, "RenShu_Node") or string.find(name, "ZhiFu_Node") then
			self:_adjustRoomCard(self.playID)
		end
	end
end

function CreateRoomCtrl:Destroy()
	self:_clearAsset()

	self.m_tabGroup = nil
	self.m_EZToggleGroup = nil
	self.m_luaWindowRoot = nil
end
-------------------------------------------------------------------------------