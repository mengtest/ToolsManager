
require "GameState.GameState"
require "GameState.MainCityState"
require "GameState.LoginState"
require "GameState.GameStateQueueTask"

GameStateMgr = {}
local _s = GameStateMgr
_s.activeState = nil
_s.activeStateType = EGameStateType.StateNone
_s.lastActiveStateType = EGameStateType.StateNone

--要update 节省下性能
local _GameStateQueueTask = GameStateQueueTask

function GameStateMgr.Init()
	_GameStateQueueTask.Init()
end

function GameStateMgr.ActiveState(stateType)
	DwDebug.Log("active state now ")
	_s.lastActiveStateType = _s.activeStateType

	if _s.activeStateType == stateType then
		LuaEvent.AddEventNow(EEventType.ChangeGameState,stateType,0)
	else
		if stateType == EGameStateType.MainCityState then
			_s.activeState = MainCityState
		elseif stateType == EGameStateType.GameState then
			_s.activeState = GameState
		else
			_s.activeState = nil
		end
		_s.activeStateType = stateType
		LuaEvent.AddEventNow(EEventType.ChangeGameState,stateType,0)
		--清理弹窗
		WindowUtil.ClearErrorParams()

		_GameStateQueueTask.ActiveState(stateType)
	end
end

function GameStateMgr.CancelState()
	_s.activeState = nil
	_s.activeStateType = EGameStateType.StateNone
end

function GameStateMgr.Update()
	if _s.activeState  and _s.activeState.Update then
		_s.activeState.Update()
	end

	_GameStateQueueTask.Update()
end

function GameStateMgr.Destroy()
	if _s.activeState and _s.activeState.Destroy then
		_s.activeState.Destroy()
	end

	_GameStateQueueTask.Destroy()
end

function GameStateMgr.GetCurStateType()
	return _s.activeStateType
end

function GameStateMgr.GetLastStateType()
	return _s.lastActiveStateType
end

function GameStateMgr.SetCurState(state)
	_s.activeState = state
	if state then
		_s.activeStateType = state.GetType()
	else
		_s.activeStateType = EGameStateType.StateNone
	end
end 

function GameStateMgr.GoToMainScene()
	if _s.activeStateType ~= EGameStateType.MainCityState then
		GameStateMgr.ActiveState(EGameStateType.MainCityState)
	end
end

function GameStateMgr.GoToLoginScene()
	if _s.activeStateType ~= EGameStateType.LoginState then
		GameStateMgr.ActiveState(EGameStateType.LoginState)
	end
end

function GameStateMgr.GoToGameScene()
	if _s.activeStateType ~= EGameStateType.GameState then
		GameStateMgr.ActiveState(EGameStateType.GameState)
	else
		DwDebug.Log("GoToGameScene but already in GameState")
	end
end