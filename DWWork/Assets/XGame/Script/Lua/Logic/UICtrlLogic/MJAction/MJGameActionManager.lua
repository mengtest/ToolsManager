--[[
	@author: zx
	@date: 2017.09.26
	@desc: 麻将牌动画基类
	不同的动作并行，相同的动作需要等待
	动作有handler, 处理回调
]]

require "Logic.UICtrlLogic.MJAction.MJGameAction"
require "Logic.UICtrlLogic.MJAction.MJFaPaiAction"
require "Logic.UICtrlLogic.MJAction.MJGaiPaiAction"
require "Logic.UICtrlLogic.MJAction.MJShouPaiMoveAction"
require "Logic.UICtrlLogic.MJAction.MJChuPaiAction"
require "Logic.UICtrlLogic.MJAction.MJChuPaiMoveAction"

MJGameActionManager = {}
local _s = MJGameActionManager

-- 全局设置参数
_s.RotateBySeatPos =
{
	[SeatPosEnum.South] = UnityEngine.Vector3.New(0, 0, -10),
	[SeatPosEnum.East] = UnityEngine.Vector3.New(10, 0, 0),
	[SeatPosEnum.North] = UnityEngine.Vector3.New(0, 0, 10),
	[SeatPosEnum.West] = UnityEngine.Vector3.New(-10, 0, 0),
}

_s.PositionBySeatPos =
{
	[SeatPosEnum.South] = UnityEngine.Vector3.New(0, 90, 0),
	[SeatPosEnum.East] = UnityEngine.Vector3.New(0, 32, 0),
	[SeatPosEnum.North] = UnityEngine.Vector3.New(0, 70, 0),
	[SeatPosEnum.West] = UnityEngine.Vector3.New(0, 32, 0),
}

function MJGameActionManager.Init(luaWindowRoot)
	_s.luaWindowRoot = luaWindowRoot
	_s.maskRoot = luaWindowRoot:GetTrans("mask_root")

	_s.datas = {}
	_s.actions = {}

	UpdateBeat:Add(_s.Update)

	_s.Register()
end

function MJGameActionManager.UnInit()
	_s.Kill()

	UpdateBeat:Remove(_s.Update)

	_s.UnRegister()
end

function MJGameActionManager.Play( uiSeatID, ActionFun, ... )
	local keyID = uiSeatID .. tostring(ActionFun.__cname)

	local action = _s.actions[keyID]
	if action == nil then
		action = ActionFun.New(uiSeatID, _s.luaWindowRoot)
		action:Play(...)
		_s.actions[keyID] = action
	else
		local datas = _s.datas[keyID]
		if nil == datas then
			_s.datas[keyID] = {}
			datas = _s.datas[keyID]
		end

		table.insert(datas, { ... })
	end
	return action
end

-- _接收：参数
function MJGameActionManager.OnFinish( action )
	local keyID = action.uiSeatID .. tostring(action.__cname)
	local datas = _s.datas[keyID]
	if nil == datas or 0 == #datas then
		_s.actions[keyID] = nil
	else
		action:Play(datas[1])
		table.remove(datas, 1)
	end
end

-- 控制所有mjaction更新入口
function MJGameActionManager.Update()
	for k,action in pairs(_s.actions) do
		action:Update()
	end
end

function MJGameActionManager.Kill( complate )
	-- 设置默认参数为true
	if nil == complate then
		complate = true
	end

	for k,action in pairs(_s.actions) do
		action:Kill(complate)
	end

	_s.datas = {}
	_s.actions = {}

	LuaEvent.AddEventNow(EEventType.ActionOperationMask, false)
end

function MJGameActionManager.KillBySeat(seat)
	for k,action in pairs(_s.actions) do
		if string.find(k, seat) then
			action:Kill(true)
			_s.actions[k] = nil
		end
	end

	for k,v in pairs(_s.datas) do
		if string.find(k, seat) then
			_s.datas[k] = nil
		end
	end

	if SeatPosEnum.South == seat then
		LuaEvent.AddEventNow(EEventType.ActionOperationMask, false)
	end
end

function MJGameActionManager.ActionOperationMask(eventID, isShow)
	if nil ~= _s.luaWindowRoot and nil ~= _s.maskRoot then
		_s.luaWindowRoot:SetActive(_s.maskRoot, isShow)
	end
end

function MJGameActionManager.Register()
	LuaEvent.AddHandle(EEventType.ActionOperationMask, _s.ActionOperationMask)
end

function MJGameActionManager.UnRegister()
	LuaEvent.RemoveHandle(EEventType.ActionOperationMask, _s.ActionOperationMask)
end