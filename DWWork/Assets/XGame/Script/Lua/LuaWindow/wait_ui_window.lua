--------------------------------------------------------------------------------
-- 	 File       : wait_ui_window.lua
--   author     : zhanghaochun
--   function   : 等待窗口
--   date       : 2017-12-29
--   copyright  : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

wait_ui_window = {}

local _s = wait_ui_window
_s.isRegisterTimer = false

local m_luaWindowRoot
local m_state

local coverTrans
local labelTrans
local colliderTrans
local backgroundTrans

local smallBackGround
local bigContent

local ShowTime = 0
local timer = 0
--是否显示底框
local showBg = true

function wait_ui_window.Init(luaWindowRoot)
	m_luaWindowRoot = luaWindowRoot
end

function wait_ui_window.CreateWindow()
	coverTrans = m_luaWindowRoot:GetTrans("cover")
	labelTrans = m_luaWindowRoot:GetTrans("label")
	colliderTrans = m_luaWindowRoot:GetTrans("collider")
	backgroundTrans = m_luaWindowRoot:GetTrans("background")
	smallBackGround = m_luaWindowRoot:GetTrans("smallBackGround")
	bigContent = m_luaWindowRoot:GetTrans("bigContent")
end

function wait_ui_window.InitWindow(open, state, params)
	m_luaWindowRoot:InitCamera(open, false, 1030, false)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		state = state or 0
		m_state, ShowTime = Division(state, 10000)

		--默认显示
		if params == nil then
			params = true
		end
		showBg = params

		_s.InitWindowDetail()
	else
		_s.RemoveTimer()
		timer = 0
	end
end

function wait_ui_window.InitWindowDetail()
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(labelTrans, "label3"), false)
	if m_state == 1 then
		m_luaWindowRoot:SetActive(colliderTrans, true)
		m_luaWindowRoot:SetActive(coverTrans, false)
		m_luaWindowRoot:ShowChild(labelTrans, "label1", false)
	elseif m_state == 2 then
		m_luaWindowRoot:SetActive(colliderTrans, true)
		m_luaWindowRoot:SetActive(coverTrans, true)
		m_luaWindowRoot:SetActive(backgroundTrans, true)
		m_luaWindowRoot:ShowChild(labelTrans, "label1", true)
	elseif m_state == 3 then
		m_luaWindowRoot:SetActive(colliderTrans, true)
		m_luaWindowRoot:SetActive(coverTrans, true)
		m_luaWindowRoot:SetActive(backgroundTrans, false)
		m_luaWindowRoot:ShowChild(labelTrans, "label1", true)
	elseif m_state == 4 then
		m_luaWindowRoot:SetActive(colliderTrans, true)
		m_luaWindowRoot:SetActive(coverTrans, true)
		m_luaWindowRoot:SetActive(backgroundTrans, true)
		m_luaWindowRoot:ShowChild(labelTrans, "label2", true)
	elseif m_state == 5 then
		m_luaWindowRoot:SetActive(colliderTrans, true)
		m_luaWindowRoot:SetActive(coverTrans, true)
		m_luaWindowRoot:SetActive(backgroundTrans, true)
		m_luaWindowRoot:ShowChild(labelTrans, "label3", true)
	end

	_s.AddTimer()

	m_luaWindowRoot:SetActive(smallBackGround, showBg)
	m_luaWindowRoot:SetActive(bigContent, not showBg)
end

function wait_ui_window.AddTimer()
	timer = 0
	if ShowTime > 0 then
		if not _s.isRegisterTimer then
			UpdateSecond:Add(_s.UpdateFunction)
			_s.isRegisterTimer = true
		end
	else
		_s.RemoveTimer()
	end
end

function wait_ui_window.RemoveTimer()
	if _s.isRegisterTimer then
		_s.isRegisterTimer = false
		UpdateSecond:Remove(_s.UpdateFunction)
	end
end

function wait_ui_window.UpdateFunction()
	timer = timer + 1
	if timer >= ShowTime then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 0, "wait_ui_window", true , nil)
	end
end


function wait_ui_window.OnDestroy()
	coverTrans = nil
	labelTrans = nil
	colliderTrans = nil
	backgroundTrans = nil
end
