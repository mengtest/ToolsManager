--------------------------------------------------------------------------------
-- 	 File      : recoredAudio_ui_window.lua
--   author    : leishengchao
--   function  : 聊天窗口 
--   date      : 2017-11-9
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------


recoredAudio_ui_window = {}
local _s = recoredAudio_ui_window

local m_luaWindowRoot
local m_state
local time = 0
local m_sprite = nil
local Label_state1
local Label_state2

function recoredAudio_ui_window.Init(luaWindowRoot)
	m_luaWindowRoot = luaWindowRoot
end

function recoredAudio_ui_window:PreCreateWindow()

end

function recoredAudio_ui_window:CreateWindow()
	m_sprite = m_luaWindowRoot:GetTrans("bgloading").gameObject:GetComponent("UISprite")

	Label_state1 = m_luaWindowRoot:GetTrans("Label_state1")
	Label_state2 = m_luaWindowRoot:GetTrans("Label_state2")

	LuaEvent.AddHandle(EEventType.RecoredAudioWindowState,_s.RecoredAudioWindowState,nil)
end

function recoredAudio_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, -1, false)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		time = 0
		_s.InitWindowDetail()
		_s.SetWindowState(true)
	end
end

function recoredAudio_ui_window:OnDestroy()
	LuaEvent.RemoveHandle(EEventType.RecoredAudioWindowState,_s.RecoredAudioWindowState,nil)
end

function recoredAudio_ui_window.RecoredAudioWindowState(eventId, p1, p2)
	_s.SetWindowState(p1)
end

function recoredAudio_ui_window.SetWindowState(isState1)
	m_luaWindowRoot:SetActive(Label_state1,isState1)
	m_luaWindowRoot:SetActive(Label_state2,not isState1)
end

function recoredAudio_ui_window.InitWindowDetail( )
	m_sprite.fillAmount = 1
end

function recoredAudio_ui_window.Update()
	time = time + UnityEngine.Time.deltaTime
	local percent = time/30
	if m_sprite then
		m_sprite.fillAmount = 1 - percent
	end
end

