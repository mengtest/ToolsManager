--------------------------------------------------------------------------------
-- 	 File      : TimerTaskSys.lua
--   author    : guoliang
--   function   : 定时任务调度系统（用于定时执行某些需要操作,系统维护调用，调用者可以通过索引移除未执行的定时任务）
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
TimerTaskSys = {}
local _s = TimerTaskSys
_s.m_taskList = {}
_s.m_curTaskIndex = 1

function TimerTaskSys.Init()
	
end

function TimerTaskSys.Reset()

end

function TimerTaskSys.Update()
	local currentTimeStamp = UnityEngine.Time.realtimeSinceStartup
	for k,v in pairs(_s.m_taskList) do
		if v~=nil then
			if currentTimeStamp >= v.excutTime then
				local fun = v.fun
				if v.bObj then
					if v.self then
						fun(v.self,v.param)
					else
						DwDebug.LogError("TimerTaskSys Task Owner is nil")
					end
				else
					fun(v.param)
				end
				_s.RemoveTask(k)
			end
		end
	end
end

function TimerTaskSys.Destroy()
	_s.m_taskList = {}
	_s.m_curTaskIndex = 1
end

function TimerTaskSys.AddTimerEventByLeftTime(func,time,self,...)
	if func == nil or time < 0 then
		DwDebug.LogError("TimerTaskSys AddTimerEventByLeftTime fail")
		return
	end
	_s.m_curTaskIndex = _s.m_curTaskIndex + 1
	local excuteTimeStamp = UnityEngine.Time.realtimeSinceStartup + time
	if self then
		_s.m_taskList[_s.m_curTaskIndex] = {fun = func,excutTime = excuteTimeStamp,self = self,bObj = true,param = ...}
	else
		_s.m_taskList[_s.m_curTaskIndex] = {fun = func,excutTime = excuteTimeStamp,bObj = false,param = ...}
	end
	return _s.m_curTaskIndex
end

function TimerTaskSys.RemoveTask(taskID)
	if taskID then
		_s.m_taskList[taskID] = nil
	end
end

function TimerTaskSys.GetTask( taskID )
	if taskID then
		return _s.m_taskList[taskID]
	end
end
