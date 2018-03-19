--------------------------------------------------------------------------------
-- 	 File      : DelayTaskSys.lua
--   author    : guoliang
--   function   : 延迟任务调度系统（用于缓存某些需要操作，由调用者唤醒）
--   date      : 2017-11-2
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
DelayTaskSys = {}
local _s = DelayTaskSys
_s.m_taskList = {}
_s.m_curTaskIndex = 1

function DelayTaskSys.Init()
	
end

function DelayTaskSys.Reset()
	_s.m_taskList = {}
	_s.m_curTaskIndex = 1
end

function DelayTaskSys.Destroy()
	_s.m_taskList = {}
	_s.m_curTaskIndex = 1
end

function DelayTaskSys.ExecuteTask(taskID)
	if taskID then
		local task = _s.m_taskList[taskID]
		if task then
			if task.bObj then
				if task.self then
					task.func(task.self,task.param)
				else
					DwDebug.LogError("DelayTaskSys Task Owner is nil")
				end
			else
				task.func(task.param)
			end
			_s.m_taskList[taskID] = nil
		end
	end
end

function DelayTaskSys.AddTask(func,self,...)
	if func == nil then
		DwDebug.LogError("DelayTaskSys.AddDelayTask fail")
		return
	end
	_s.m_curTaskIndex = _s.m_curTaskIndex + 1
	if self then
		_s.m_taskList[_s.m_curTaskIndex] = {func = func,self = self,bObj = true,param = ...}
	else
		_s.m_taskList[_s.m_curTaskIndex] = {func = func,bObj = false,param = ...}
	end

	return _s.m_curTaskIndex
end

function DelayTaskSys.RemoveTask(taskID)
	if taskID then
		_s.m_taskList[taskID] = nil
	end
end
