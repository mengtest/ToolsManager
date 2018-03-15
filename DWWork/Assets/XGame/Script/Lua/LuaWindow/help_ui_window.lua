require "Tools.EZToggleGroup"
require "Tools.TabGroup"

local m_luaWindowRoot
local m_state
local m_EZToggleGroup
local m_tabGroup = nil --页签组件
local m_tabRoot = nil --页签根节点
local m_gameTabList = {} --页签列表
local m_panelRoot = nil -- 界面根节点
local m_gamePanelTable = {} --保存已加载的界面

help_ui_window = {

}
local _s = help_ui_window
local GameType = 
{
	POKE_510K = 1,
	MJ_CongRen = 2,
	All = 3,
}

local tab = ""

local HelpTabConfig = 
{
	[Common_PlayID.chongRen_MJ] = { name = "崇仁麻将", panel = "help_CRMJ_item", ruleSize = 4, sortOrder = 1 },
	[Common_PlayID.chongRen_510K] = { name = "崇仁打盾", panel = "help_CRWSK_item", ruleSize = 3, sortOrder = 4 },
	[Common_PlayID.leAn_MJ] = { name = "乐安麻将", panel = "help_LAMJ_item", ruleSize = 4, sortOrder = 2 },
	[Common_PlayID.leAn_510K] = { name = "乐安打盾", panel = "help_LAWSK_item", ruleSize = 3, sortOrder = 5 },
	[Common_PlayID.yiHuang_MJ] = { name = "宜黄麻将", panel = "help_YHMJ_item", ruleSize = 4, sortOrder = 3 },
	[Common_PlayID.yiHuang_510K] = { name = "宜黄红心5", panel = "help_YHWSK_item", ruleSize = 3, sortOrder = 6 },
	[Common_PlayID.ThirtyTwo] = {  name = "三十二张", panel = "help_32Z_item", ruleSize = 4, sortOrder = 7 },
	[Common_PlayID.DW_DouDiZhu] = { name = "斗地主", panel = "help_DDZ_item", ruleSize = 3, sortOrder = 8},
}

local DefGamePlayID = Common_PlayID.chongRen_MJ

function help_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function help_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	else
		_s.ClearAsset()
	end
end

function help_ui_window.CreateWindow()
end

-- 修改
function help_ui_window.InitGameTab(_, trans, index)
	local playID = m_gameTabList[index].playID

	local tabConfig = HelpTabConfig[playID]
	if nil == tabConfig or nil == tabConfig.name then
		DwDebug.LogError("init tab no foud game playID:"..playID)
		return
	end

	-- 设置文本
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "Label"), tabConfig.name, false)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "UnLabel"), tabConfig.name, false)
end

-- 是否选中页签
function help_ui_window.IsTabSelect(_, index)
	return m_gameTabList[index+1].playID == m_state
end

function help_ui_window.InitWindowDetail()
	m_tabRoot = m_luaWindowRoot:GetTrans("game_tabs")
	m_panelRoot = m_luaWindowRoot:GetTrans("panels")

	m_EZToggleGroup = EZToggleGroup.New()
	m_EZToggleGroup:Init(m_luaWindowRoot)

	m_gameTabList = {}
	if m_state == nil or -1 == m_state then
		for playID, config in pairs(HelpTabConfig) do
			table.insert(m_gameTabList, {playID = playID, sortOrder = config.sortOrder })
		end

		table.sort(m_gameTabList, function (a, b)
			return a.sortOrder < b.sortOrder
		end)

		m_state = DefGamePlayID
	else
		table.insert(m_gameTabList, {playID = m_state, sortOrder = 1 })
	end

	if nil == m_tabGroup then
		m_tabGroup = TabGroup.New(m_luaWindowRoot)
	end
	local vector2 = UnityEngine.Vector2
	m_tabGroup:Init(m_tabRoot, "tab_item", vector2.New(152, 78), vector2.New(1, 5), LimitScrollViewDirection.SVD_Vertical, _s.InitGameTab, _s.IsTabSelect, nil, #m_gameTabList)

	_s.InitGameHelp(m_state)
end

function help_ui_window.InitGameHelp(game_type)
	m_state = game_type

	local name = "game" .. game_type .. "Panel"
	local panel = m_gamePanelTable[game_type]
	if nil == panel then
		local config = HelpTabConfig[game_type]
		if nil == config or nil == config.panel or nil == config.ruleSize then
			DwDebug.LogError("creta panel on found playID")
			return
		end

		local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, config.panel, m_panelRoot, RessStorgeType.RST_Never)
		if not resObj then
			DwDebug.LogError("init panel no foud asset:" .. panelConfig)
			return
		end
		resObj.name = name
		local trans = resObj.transform
		m_luaWindowRoot:SetActive(trans, true)

		-- 初始化规则
		for i=1,config.ruleSize do
			m_EZToggleGroup:AddTrans(m_luaWindowRoot:GetTrans(trans, ("rule_" .. i)), name, i == 1)
		end
		m_EZToggleGroup:RefreshSelect(name)

		m_gamePanelTable[game_type] = trans
	end

	m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("panels"), name, true)
	local _, index = m_EZToggleGroup:GetSelectTrans(name)
	_s.InitRuleHelp(index or 1)
end

function help_ui_window.InitRuleHelp(rule_type)
	local panelRoot = m_luaWindowRoot:GetTrans("game" .. m_state .. "Panel")
	local docs = m_luaWindowRoot:GetTrans(panelRoot, "docs")
	local ruleName = "doc_" .. rule_type
	m_luaWindowRoot:ShowChild(docs, ruleName, true)
	m_luaWindowRoot:PlayTweenPos(m_luaWindowRoot:GetTrans(docs, ruleName), 0.25, false, -20, 0)
end

function help_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "close_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	elseif string.find(click_name, "tab_") then
		local index = tonumber(string.sub(click_name, string.len("tab_") + 1))

		index = index + 1
		if nil ~= m_gameTabList[index] then
			local playID = m_gameTabList[index].playID
			-- DwDebug.Log("playID:", playID)
			-- DwDebug.Log("m_state:", m_state)
			if m_state ~= playID then
				_s.InitGameHelp(playID)
				m_tabGroup:Select(index)
			end
		end
	-- elseif string.find(click_name, "game_") then
	-- 	m_EZToggleGroup:Select(gb.transform)

	-- 	local index = tonumber(string.sub(click_name, string.len("game_") + 1))
	-- 	if m_state ~= index then
	-- 		_s.InitGameHelp(index)
	-- 	end
	elseif string.find(click_name, "rule_") then
		m_EZToggleGroup:Select(gb.transform)
		
		local index = tonumber(string.sub(click_name, string.len("rule_") + 1))
		_s.InitRuleHelp(index)
	end
end

function help_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

-- 情况资源
function help_ui_window.ClearAsset()
	for playID,trans in pairs(m_gamePanelTable) do
		m_luaWindowRoot:SetActive(trans, false)
	end
	m_gamePanelTable = {}
	m_gameTabList = {}

	m_EZToggleGroup = nil
end

function help_ui_window.OnDestroy()
	_s.ClearAsset()

	m_tabGroup = nil
end