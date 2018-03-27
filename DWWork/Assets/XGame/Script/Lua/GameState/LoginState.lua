--------------------------------------------------------------------------------
--   File      : LoginState.lua
--   author    : guoliang
--   function   : 登录场景状态
--   date      : 2017-11-11
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "LuaSys.AreaLuaSys"

LoginState = {}
local _s = LoginState

function LoginState.Init()

end

function LoginState.Enter()
	WrapSys.AudioSys_PlayMusic("Music/backMusic")

	local areaName = AreaLuaSys.GetNowAreaName()
	if areaName == nil or areaName == "" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "area_ui_window", false , nil)
	else
		_s.NormalEnter()
	end
end

function LoginState.NormalEnter()
	GameStateMgr.SetCurState(_s)
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "login_ui_window", false , nil)
	if not DataManager.isPrePublish then
		if LoginSys.CheckCanAutoLogin() then
			LoginSys.Login(100)
		end
	end
end

function LoginState.Leave()

end

function LoginState.GetType()
	return EGameStateType.LoginState
end
