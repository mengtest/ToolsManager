--------------------------------------------------------------------------------
-- 	 File      : common_error_ui_window.lua
--   author    : guoliang
--   function   : 通用错误弹窗
--   date      : 2017-10-9
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

common_error_ui_window = {}
local _s = common_error_ui_window


local m_luaWindowRoot
local m_state

local m_labelContentTrans
local m_labelContentSecTrans
local m_labelOKTrans
local m_labelNoTrans
local m_errorParams

function common_error_ui_window.Init(luaWindowRoot)
	m_luaWindowRoot = luaWindowRoot
end

function common_error_ui_window:PreCreateWindow()

end

function common_error_ui_window:CreateWindow()
	m_labelContentTrans = m_luaWindowRoot:GetTrans("label_content")
	m_labelContentSecTrans = m_luaWindowRoot:GetTrans("label_content_sec")
	m_labelOKTrans = m_luaWindowRoot:GetTrans("label_ok")
	m_labelNoTrans = m_luaWindowRoot:GetTrans("label_no")
end

function common_error_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, -1, false)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		m_errorParams = WindowUtil.PopErrorParams()
		_s.InitWindowDetail()
	end
end

function common_error_ui_window:OnDestroy()

end

function common_error_ui_window.HandleWidgetClick(gb)
	local clickName = gb.name
	if clickName == "btn_no" then
		_s.HandleNoClick()
	elseif clickName == "btn_yes" then
		_s.HandleYesClick()
	elseif clickName == "btn_close" then
		_s.HandleNoClick()
	elseif clickName == "btn_ok" then
		_s.HandleOKClick()
	end
end

function common_error_ui_window.InitWindowDetail()
	if m_errorParams.winType == 1 then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_yes"),false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_no"),false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_ok"),true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_close"),true)
	elseif m_errorParams.winType == 2 then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_yes"),true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_no"),true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_ok"),false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_close"),true)
	elseif m_errorParams.winType == 3 then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_yes"),false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_no"),false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_ok"),true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_close"),false)
	elseif m_errorParams.winType == 4 then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_yes"),true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_no"),true)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_ok"),false)
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_close"),false)
	end

	m_luaWindowRoot:SetLabel(m_labelContentTrans, m_errorParams.errorWindow_content or "")

	if m_errorParams.errorWindow_secContent then
		m_luaWindowRoot:SetActive(m_labelContentSecTrans,true)
		m_luaWindowRoot:SetLabel(m_labelContentSecTrans, m_errorParams.errorWindow_secContent)
	else
		m_luaWindowRoot:SetActive(m_labelContentSecTrans,false)
	end

	if m_errorParams.errorWindow_okSpName then
		m_luaWindowRoot:SetSprite(m_labelOKTrans, m_errorParams.errorWindow_okSpName)
	else
		m_luaWindowRoot:SetSprite(m_labelOKTrans,"leaveRoomLayer_text_confirm")
	end

	if m_errorParams.errorWindow_noSpName then
		m_luaWindowRoot:SetSprite(m_labelNoTrans, m_errorParams.errorWindow_noSpName)
	else
		m_luaWindowRoot:SetSprite(m_labelNoTrans,"leaveRoomLayer_text_cancel")
	end
end

function common_error_ui_window.CloseWindow()
	m_errorParams = WindowUtil.PopErrorParams(true)
	if nil == m_errorParams then
		_s.InitWindow(false, 0)
	else
		_s.InitWindowDetail()
	end
end

function common_error_ui_window.HandleYesClick()
	if m_errorParams.errorWindow_yesFunc then
		m_errorParams.errorWindow_yesFunc()
	end
	_s.CloseWindow()
end

function common_error_ui_window.HandleOKClick()
	if m_errorParams.errorWindow_okFunc then
		m_errorParams.errorWindow_okFunc()
	end
	_s.CloseWindow()
end

function common_error_ui_window.HandleNoClick()
	if m_errorParams.errorWindow_noFunc then
		m_errorParams.errorWindow_noFunc()
	end
	_s.CloseWindow()
end



