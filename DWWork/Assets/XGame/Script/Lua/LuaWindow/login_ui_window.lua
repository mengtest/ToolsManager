
local m_luaWindowRoot
local m_state
local isAgree

local VersionLabel

login_ui_window = {

}

local function ChangeAgreement()
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("Select"), isAgree)
end

local function InitWindowDetail()
	if m_state == 0 then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("animation_ui_rootTmp"),false)
	else
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("animation_ui_rootTmp"),true)
		isAgree = LoginSys.GetAgreement()
		ChangeAgreement()

		m_luaWindowRoot:SetLabel(VersionLabel,"当前版本：" .. Version.Instance:GetVersion(1))
		if DataManager.isPrePublish then
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("loginBtn"),false)
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("vistorLoginBtn"),true)
		else
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("loginBtn"),true)
			m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("vistorLoginBtn"),false)
		end
	end
end

function login_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function login_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		InitWindowDetail()
	end
end

function login_ui_window.CreateWindow()
	VersionLabel = m_luaWindowRoot:GetTrans("VersionLabel")
end

local function WeiXinLoginCallBack(sucsess,data)
	DwDebug.Log("data :" .. data)
	if sucsess then
		if data ~= "isLogined" then
			local weiXinInfo = loadstring("return " .. data)()
			DataManager.SetWeiXinInfo(weiXinInfo)
		end
		LoginSys.Login(1)
	end
end

function login_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name == "loginBtn" then
		if not isAgree then
			WindowUtil.LuaShowTips("请先同意用户协议")
		else
			if WrapSys.IsEditor then
				LoginSys.Login(100)
			else
				WrapSys.WeiXinLogin()
			end
		end
	elseif click_name == "vistorLoginBtn" then
		if not isAgree then
			WindowUtil.LuaShowTips("请先同意用户协议")
		else
			LoginSys.Login(50)
		end
	elseif click_name == "urlToggle" then
		isAgree = not isAgree
		ChangeAgreement()
		LoginSys.SetAgreement(isAgree)
	elseif click_name == "urlBtn" then
		WrapSys.OpenURL("http://game.dw7758.com/user_protocol.html")
	end
end

function login_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function login_ui_window.OnDestroy()
    
end