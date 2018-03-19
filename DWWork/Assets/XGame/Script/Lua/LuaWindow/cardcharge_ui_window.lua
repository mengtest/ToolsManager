require "Tools.EZToggleGroup"

local m_luaWindowRoot

cardcharge_ui_window = {

}
local _s = cardcharge_ui_window

function cardcharge_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function cardcharge_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		_s.InitWindowDetail()
	end
end

function cardcharge_ui_window.CreateWindow()
	_s.Register()
end

function cardcharge_ui_window.InitWindowDetail()
end

-- 点击事件
function cardcharge_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "close_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	elseif click_name == "btn_WXCopy" then
		_s.OnWXBtnCopyClick()
	end
end

function cardcharge_ui_window.OnWXBtnCopyClick()
	local copyStr = "lljxmj0" .. math.random(1,2)
	WrapSys.PlatInterface_CopyStr(copyStr)
	WindowUtil.LuaShowTips("客服微信号已复制")
end

function cardcharge_ui_window.Register()
end

function cardcharge_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

function cardcharge_ui_window.OnDestroy()
	_s.UnRegister()
end