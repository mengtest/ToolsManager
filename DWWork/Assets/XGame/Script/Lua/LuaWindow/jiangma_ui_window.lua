require "Logic.MJCardLogic.CMJCard"

local m_luaWindowRoot
local m_state
local m_data
local m_markCreatedItems = {}

jiangma_ui_window = {

}
local this = jiangma_ui_window

function jiangma_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function jiangma_ui_window.InitWindow(open, state, params)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		this.InitWindowDetail(params)
		-- 播放奖马声音
		AudioManager.MJ_PlayCaoZuo(DataManager.GetUserGender(),MJ_CaoZuoEnum.jiangMa)
	else
		this.DumpItems()
	end
end

function jiangma_ui_window.CreateWindow()
end

function jiangma_ui_window.OnDestroy()
    m_luaWindowRoot = nil

    if this.timer_closeWindow and this.timer_closeWindow ~= -1 then
		TimerTaskSys.RemoveTask(this.timer_closeWindow)
		this.timer_closeWindow = -1
	end
end

function jiangma_ui_window.HandleWidgetClick(gb)
	WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
	local click_name = gb.name
	if click_name == "close_bg" then
		-- this.InitWindow(false, 0)
	elseif click_name == "vistorLoginBtn" then
	end
end

function jiangma_ui_window.InitWindowDetail(params)
	-- m_data = {
	--  jiangMaType = 6,
	--  kaiMa = {4,5,17,19,23,2},
	--  zhongMa = {}
	-- }
	m_data = params

	if not m_data or not m_data.jiangMaType or m_data.jiangMaType <= 0 then
		this.InitWindow(false,0)
		return
	end

	this.timer_closeWindow = TimerTaskSys.AddTimerEventByLeftTime(
		function ()
			this.timer_closeWindow = -1
			this.InitWindow(false, 0)
		end, 
		3, nil)

	this.ProcessData()

	this.SetJiangma()
	this.SetShuMa()
end

function jiangma_ui_window.SetShuMa()
	local root = m_luaWindowRoot:GetTrans("shuma")
	if m_data.kaiMa and #m_data.kaiMa > 0 then
		local offset = (m_data.zhongMa and #m_data.zhongMa > 0) and 24 or 0
		root.localPosition = UnityEngine.Vector3.New(-(offset+68*(m_data.jiangMaType-1))*0.5, 0, 0)
		m_luaWindowRoot:SetActive(root, true)
	else 
		m_luaWindowRoot:SetActive(root, false)
		return
	end

	local grid = m_luaWindowRoot:GetTrans("shuma_grid")

	for i,v in ipairs(m_data.kaiMa) do
		this.CreateMJCardItem(grid, v)
	end

	m_luaWindowRoot:RepositionGrid(grid, true)

	local cover_bg = m_luaWindowRoot:GetTrans("cover_bg")
	cover_bg.localPosition = UnityEngine.Vector3.New(68*(#m_data.kaiMa-1)*0.5, 0, 0)
	m_luaWindowRoot:ResetSpriteSize(cover_bg, 68*#m_data.kaiMa+3)
end

function jiangma_ui_window.SetJiangma()
	local root = m_luaWindowRoot:GetTrans("jiangma")
	if m_data.zhongMa and #m_data.zhongMa > 0 then
		local offset = (m_data.kaiMa and #m_data.kaiMa > 0) and 24 or 0
		root.localPosition = UnityEngine.Vector3.New((offset+68*(m_data.jiangMaType-1))*0.5, 0, 0)
		m_luaWindowRoot:SetActive(root, true)
	else 
		m_luaWindowRoot:SetActive(root, false)
		return
	end

	local grid = m_luaWindowRoot:GetTrans("jiangma_grid")

	for i,v in ipairs(m_data.zhongMa) do
		this.CreateMJCardItem(grid, v)
	end

	m_luaWindowRoot:RepositionGrid(grid, true)

	local highlight_bg = m_luaWindowRoot:GetTrans("highlight_bg")
	highlight_bg.localPosition = UnityEngine.Vector3.New(-68*(#m_data.zhongMa-1)*0.5, 0.75, 0)
	m_luaWindowRoot:ResetSpriteSize(highlight_bg, 68*#m_data.zhongMa+50)
end

function jiangma_ui_window.CreateMJCardItem(root, pai_id)
	local card = LogicUtil.CreateMJCardItem(m_luaWindowRoot, root, pai_id, false, false)
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(card, "ico_mask"), false)

	local mjCard = CMJCard.New()
	mjCard:Init(pai_id, 1)
	LogicUtil.InitMJCardItem(m_luaWindowRoot, card, mjCard, 0, 0)
	card.localScale = UnityEngine.Vector3.New(0.9,0.9,1)
	table.insert( m_markCreatedItems, card)
end

function jiangma_ui_window.DumpItems()
	if not m_markCreatedItems then
		return
	end
	
	local trash = m_luaWindowRoot:GetTrans("trash")
	for i,v in ipairs(m_markCreatedItems) do
		m_luaWindowRoot:SetActive(v, false)
		v.parent = trash
	end

	m_markCreatedItems = {}
end

function jiangma_ui_window.ProcessData()
	if m_data.kaiMa and #m_data.kaiMa > 0 and m_data.zhongMa and #m_data.zhongMa > 0 then
		local new_kaima = {}
		local found = false
		for i1,v1 in ipairs(m_data.kaiMa) do
			found = false
			-- 不用考虑多个同样的牌，中马就必然全中马
			for i2,v2 in ipairs(m_data.zhongMa) do
				if v1 == v2 then
					found = true
				end
			end
			if not found then
				table.insert( new_kaima, v1)
			end
		end
		m_data.kaiMa = new_kaima
	end
end