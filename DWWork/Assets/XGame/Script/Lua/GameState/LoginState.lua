--------------------------------------------------------------------------------
--   File      : LoginState.lua
--   author    : guoliang
--   function   : 登录场景状态
--   date      : 2017-11-11
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "LuaSys.AreaSys"

LoginState = {}
local _s = LoginState

function LoginState.Init()

end

function LoginState.Enter()
	local areaName = AreaLuaSys.GetNowAreaName()
	--小包测试
	if areaName ~= nil and areaName ~= "" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "login_ui_window", false , nil)
	else
		WrapSys.AreaUpdateSys_SetAreaPlay(10002,"DouDiZhu")
		WrapSys.AreaUpdateSys_StartWork(function()
			_s.NormalEnter()
		end)
	end
end


function LoginState.NormalEnter()
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
