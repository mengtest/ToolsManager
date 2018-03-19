require "Tools.EZToggleGroup"

local m_luaWindowRoot

hallshare_ui_window = {

}
local _s = hallshare_ui_window

function hallshare_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function hallshare_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		_s.InitWindowDetail()
	end
end

function hallshare_ui_window.CreateWindow()
	_s.Register()
end

function hallshare_ui_window.InitWindowDetail()
end

-- 点击事件
function hallshare_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "close_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	elseif click_name == "share_btn" then
		HallSys.ShareGame()
	end
end

function hallshare_ui_window.Register()
end

function hallshare_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

function hallshare_ui_window.OnDestroy()
	_s.UnRegister()
end