local m_luaWindowRoot
local m_state
local m_currTrans
local m_bgIndex

local m_MJBgIndex
local m_MJPaiIndex
local m_MJSingleClickIndex
local m_LocalismIndex

setting_ui_window = {

}
local _s = setting_ui_window

-- 方言支持配置
local LocalismConfig =
{
	[Common_PlayID.chongRen_MJ] = true,
	[Common_PlayID.chongRen_510K] = true,
}

function setting_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function setting_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

function setting_ui_window.CreateWindow()
end

function setting_ui_window.InitWindowDetail()
	_s.InitGameResultInfo()
end

--
function setting_ui_window.GetRootTrans()
	local name = ""
	if 0 == m_state then
		name = "hall_setting"
	elseif 1 == m_state then
		name = "pk_room_setting"
	elseif 2 == m_state then
		name = "mj_room_setting"
	elseif 3 == m_state then
		name = "pk32_room_setting"
	end

	m_currTrans = m_luaWindowRoot:GetTrans(name)
	m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("animation_ui_root"), name, true)
end

--
function setting_ui_window.InitGameResultInfo()
	_s.GetRootTrans()

	WrapSys.InitMusicVolumnSlider(m_luaWindowRoot:GetTrans(m_currTrans, "music_setting"))
	WrapSys.InitEffectVolumnSlider(m_luaWindowRoot:GetTrans(m_currTrans, "sound_setting"))

	if m_state == 0 then
		local root = m_luaWindowRoot:GetTrans(m_currTrans, "self_info")
		local nickName = DataManager.GetUserNickName()

		nickName = utf8sub(nickName,1,10)

		local userID = DataManager.GetUserID()
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "name"), nickName)
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "uid"), "ID:" .. userID)
		if WrapSys.Constants_RELEASE then
			m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "game_version"), "当前版本：" .. Version.Instance:GetVersion(1))
		else
			m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "game_version"), "当前版本：" .. Version.Instance:GetVersion(1) .. "(测试服环境)")
		end
		DwDebug.LogError("当前版本：" .. Version.Instance:GetVersion(1))
		WindowUtil.LoadHeadIcon(m_luaWindowRoot,m_luaWindowRoot:GetTrans(root, "headIco"), DataManager.GetUserHeadUrl(), DataManager.GetUserGender(), false,RessStorgeType.RST_Always)
	elseif 1 == m_state then
		_s.SetBgHighLight(LogicUtil.GetPKBgType())
		_s.SetLocalismHighLight(DataManager.GetLocalism())
	elseif 2 == m_state then
		_s.SetMJBgHighLight(LogicUtil.GetMJBgType())
		_s.SetMJPaiHighLight(LogicUtil.GetMJPaiType())
		_s.SetMJSingleClick(LogicUtil.GetMJSingleClickType())
		_s.SetLocalismHighLight(DataManager.GetLocalism())
	elseif 3 == m_state then
		_s.SetLocalismHighLight(DataManager.GetLocalism())
	end
end

-- 点击事件
function setting_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "ok_btn" then
		_s.SaveSetting()
	elseif click_name == "logout_btn" then
		LoginSys.LoginOut()
	elseif click_name == "close_btn" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	elseif string.find(click_name, "pk_value_") then
		local index = tonumber(string.sub(click_name, string.len("pk_value_") + 1))
		if index and index ~= m_bgIndex then
			_s.SetBgHighLight(index)
		end
		_s.SaveSetting()
	elseif string.find(click_name, "mj_value_") then
		local index = tonumber(string.sub(click_name, string.len("mj_value_") + 1))
		if index and index ~= m_MJBgIndex then
			_s.SetMJBgHighLight(index)
		end
		_s.SaveSetting()
	elseif string.find(click_name, "pai_value_") then
		local index = tonumber(string.sub(click_name, string.len("pai_value_") + 1))
		if index and index ~= m_MJPaiIndex then
			_s.SetMJPaiHighLight(index)
		end
		_s.SaveSetting()
	elseif string.find(click_name, "single_value_") then
		local index = tonumber(string.sub(click_name, string.len("single_value_") + 1))
		if index and index ~= m_MJSingleClickIndex then
			_s.SetMJSingleClick(index)
		end
		_s.SaveSetting()
	elseif string.find(click_name, "localism_value_") then
		local index = tonumber(string.sub(click_name, string.len("localism_value_") + 1))
		if index and index ~= m_LocalismIndex then
			_s.SetLocalismHighLight(index)
		end
		_s.SaveSetting()
	end
end

-- 保存设置
function setting_ui_window.SaveSetting()
	-- 方言设置
	if not (m_LocalismIndex == DataManager.GetLocalism()) then
		DataManager.SetLocalism(m_LocalismIndex)
	end

	if 1 == m_state then
		if not (m_bgIndex == LogicUtil.GetPKBgType()) then
			LogicUtil.SetPKBgType(m_bgIndex)
		end

		LuaEvent.AddEvent(EEventType.ChangePlayCanvas, m_bgIndex)
	elseif 2 == m_state then
		if not (m_MJBgIndex == LogicUtil.GetMJBgType()) then
			LogicUtil.SetMJBgType(m_MJBgIndex)
		end

		if not (m_MJPaiIndex == LogicUtil.GetMJPaiType()) then
			LogicUtil.SetMJPaiType(m_MJPaiIndex)
		end

		-- 保存单双击设置
		if not (m_MJSingleClickIndex == LogicUtil.GetMJSingleClickType()) then
			LogicUtil.SetMJSingleClickType(m_MJSingleClickIndex)
		end

		LuaEvent.AddEvent(EEventType.ChangePlayCanvas, m_MJBgIndex, m_MJPaiIndex)
	end
	-- _s.InitWindow(false, 0)
end

-- 扑克桌面选中
function setting_ui_window.SetBgHighLight(index)
	local ground = m_luaWindowRoot:GetTrans(m_currTrans, "wsk_ground_setting")
	ground = m_luaWindowRoot:GetTrans(ground, "ground_values")
	local root = m_luaWindowRoot:GetTrans(ground, "highlights")

	if not root then
		return
	end

	m_bgIndex = index
	m_luaWindowRoot:ShowChild(root, tostring(index), true)
end

-- 麻将桌面选中
function setting_ui_window.SetMJBgHighLight(index)
	local ground = m_luaWindowRoot:GetTrans(m_currTrans, "mj_ground_setting")
	ground = m_luaWindowRoot:GetTrans(ground, "ground_values")
	local root = m_luaWindowRoot:GetTrans(ground, "highlights")

	if not root then
		return
	end

	m_MJBgIndex = index
	m_luaWindowRoot:ShowChild(root, tostring(index), true)
end

-- 麻将牌选中
function setting_ui_window.SetMJPaiHighLight(index)
	local ground = m_luaWindowRoot:GetTrans(m_currTrans, "mj_ground_setting")
	ground = m_luaWindowRoot:GetTrans(ground, "pai_values")
	local root = m_luaWindowRoot:GetTrans(ground, "highlights")

	if not root then
		return
	end

	m_MJPaiIndex = index
	m_luaWindowRoot:ShowChild(root, tostring(index), true)
end

-- 麻将单双击选中
function setting_ui_window.SetMJSingleClick(index)
	local ground = m_luaWindowRoot:GetTrans(m_currTrans, "mj_ground_setting")
	ground = m_luaWindowRoot:GetTrans(ground, "single_click_values")
	local root = m_luaWindowRoot:GetTrans(ground, "highlights")

	if not root then
		return
	end

	m_MJSingleClickIndex = index
	m_luaWindowRoot:ShowChild(root, tostring(index), true)
end

-- 方言选中
function setting_ui_window.SetLocalismHighLight(index)
	local ground = m_luaWindowRoot:GetTrans(m_currTrans, "localism_values")
	local playID = PlayGameSys.GetNowPlayId()

	local root = m_luaWindowRoot:GetTrans(ground, "highlights")

	if not root then
		return
	end

	if not LocalismConfig[playID] then
		m_LocalismIndex = 2
		m_luaWindowRoot:SetGray(ground, true, true)
		m_luaWindowRoot:SetUntouchable(ground, true)
	else
		m_LocalismIndex = index
		m_luaWindowRoot:SetGray(ground, false, true)
		m_luaWindowRoot:Settouched(ground, true)
	end

	m_luaWindowRoot:ShowChild(root, tostring(m_LocalismIndex), true)
end

function setting_ui_window.UnRegister()
	m_luaWindowRoot = nil
end

function setting_ui_window.OnDestroy()
	_s.UnRegister()
end
