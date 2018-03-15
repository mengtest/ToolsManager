--------------------------------------------------------------------------------
--   File      : LoginState.lua
--   author    : guoliang
--   function   : 登录场景状态
--   date      : 2017-11-11
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

LoginState = {}
local _s = LoginState

function LoginState.Init()

end

function LoginState.Enter()
	WrapSys.AudioSys_PlayMusic("Music/backMusic")
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
