--------------------------------------------------------------------------------
-- 	 File      : GameStateQueueTask.lua
--   author    : jianing
--   function  : 场景任务队列 目前用于跨场景任务
--   date      : 2018-02-05
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
GameStateQueueTask = {}

local _s = GameStateQueueTask

local tickTime = 0
local canUpdate = false
local m_nowStateType = EGameStateType.StateNone
local m_nextStateType = EGameStateType.StateNone

local QueueTask = {} 	--延时队列任务
local ImmediatelyTask = {}	--立即执行任务

function GameStateQueueTask.Init()
	QueueTask = {}
	ImmediatelyTask = {}
end

--场景转变
function GameStateQueueTask.ActiveState(stateType)
	if m_nowStateType == stateType then
		return
	end
	m_nextStateType = stateType
end

--跨场景完成
function GameStateQueueTask.EnterEnd()
	tickTime = 1	--跨场景 慢点执行
	canUpdate = true

	 _s.DoImmediatelyTask()
	 --放到这里转换 放到上面 场景还没加载完
	 m_nowStateType = m_nextStateType
end

--执行立即任务
function GameStateQueueTask.DoImmediatelyTask()
	if m_nowStateType and ImmediatelyTask[m_nowStateType]  then
		for i,taskFunc in ipairs(ImmediatelyTask[m_nowStateType]) do
			taskFunc()
		end
		ImmediatelyTask[m_nowStateType] = {}
	end
end

--添加场景任务
--taskFunc 要执行的任务
--taskTime 执行时间 防止并发 后面任务要等待时间
--eGameStateType 执行场景 传空 默认主场景
function GameStateQueueTask.AddTask(taskFunc,taskTime,eGameStateType)
	if taskFunc == nil or type(taskFunc) ~= "function" then
		return
	end

	if taskTime == nil then
		taskTime = 0
	end

	if eGameStateType == nil then
		eGameStateType = EGameStateType.MainCityState
	end

	if taskTime == 0 then
		if GameStateMgr.GetCurStateType() == eGameStateType then
			DwDebug.LogError("GameStateQueueTask.AddTask")
			taskFunc()
		else
			if ImmediatelyTask[eGameStateType] == nil then
				ImmediatelyTask[eGameStateType] = {}
			end
			table.insert(ImmediatelyTask[eGameStateType],taskFunc)
		end
	else
		local taskInfo = {}
		taskInfo.taskFunc = taskFunc
		taskInfo.taskTime = taskTime

		if QueueTask[eGameStateType] == nil then
			QueueTask[eGameStateType] = {}
		end
		table.insert(QueueTask[eGameStateType],taskInfo)
		canUpdate = true
	end
end

function GameStateQueueTask.Update()
	if not canUpdate then
		return
	end

	tickTime = tickTime - UnityEngine.Time.deltaTime

	if not m_nowStateType
		or not QueueTask[m_nowStateType]
		or #QueueTask[m_nowStateType] == 0 then
		canUpdate = false
	else
		if tickTime <= 0 then --录音中 不播放了
			local taskInfo = QueueTask[m_nowStateType][1]
			taskInfo.taskFunc()
			table.remove(QueueTask[m_nowStateType], 1)
			tickTime = taskInfo.taskTime + tickTime
		end
	end
end

function GameStateQueueTask.Destroy()
	QueueTask = {}
	ImmediatelyTask = {}
end
