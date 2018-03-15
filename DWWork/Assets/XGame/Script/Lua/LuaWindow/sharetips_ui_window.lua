require "Tools.EZToggleGroup"

local m_luaWindowRoot
local m_state

sharetips_ui_window = {

}
local _s = sharetips_ui_window

function sharetips_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function sharetips_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

function sharetips_ui_window.CreateWindow()
	_s.Register()
end

function sharetips_ui_window.InitWindowDetail()
	local contentStr = string.format("今日分享成功，系统赠送%s张房卡", m_state)
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("Label"), contentStr)
end

-- 点击事件
function sharetips_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "close_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	end
end

function sharetips_ui_window.Register()
end

function sharetips_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

function sharetips_ui_window.OnDestroy()
	_s.UnRegister()
end