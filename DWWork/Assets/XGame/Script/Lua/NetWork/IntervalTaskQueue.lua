--------------------------------------------------------------------------------
-- 	 File       : IntervalTaskQueue.lua
--   author     : zhisong
--   function   : 间隔执行事件队列(依次执行队列里的事件，如果事件有注册间隔时间，则等待间隔时间后才执行下个事件)
--   date       : 2018年1月18日 17:07:04
--   copyright  : Copyright 2018 DW Inc
--------------------------------------------------------------------------------

require "Global.Queue"

IntervalTaskQueue = class("IntervalTaskQueue", nil)

function IntervalTaskQueue:ctor(logic, orig_register, orig_unregister)
	self.m_logic = logic
	self.m_orig_register = orig_register
	self.m_orig_unregister = orig_unregister
	UpdateBeat:Add(self.UpdateQueue, self)
end

function IntervalTaskQueue:Destroy()
	UpdateBeat:Remove(self.UpdateQueue, self)
	self:Clear()
end

function IntervalTaskQueue:Clear()
	self.wait_time = 0
	if self.m_task_queue then
		self.m_task_queue:clear()
	end
end

function IntervalTaskQueue:RegisteHandle(cmd, func, obj, interval_time)
	if not self.m_task_queue then
		self.m_task_queue = Queue.New(1000)
	end

	if not self.m_registed_func then
		self.m_registed_func = {}
	end

	self.m_registed_func[cmd] = function(logic_self, rsp, head)
		-- local rsp_bk = DeepCopy(rsp)
		-- local head_bk = DeepCopy(head)
		self.m_task_queue:enQueue({
			cmd_code = cmd,
			interval = function()
				if interval_time then
					local param_type = type(interval_time)
					if param_type == "function" then
						return interval_time(rsp, head)
					elseif param_type == "number" then
						return interval_time
					else
						return 0
					end
				else 
					return 0
				end
			end,
			orig_call = function()
				func(logic_self, rsp, head)
			end}
			)
		-- logError("recv " .. cmd .. ";queue size = " .. self.m_task_queue:size())
	end

	if self.m_orig_register then
		self.m_orig_register(cmd, self.m_registed_func[cmd], self.m_logic)
	end
end

function IntervalTaskQueue:UnRegisteHandle(cmd, func, obj)
	if self.m_registed_func[cmd] then
		if self.m_orig_unregister then
			self.m_orig_unregister(cmd, self.m_registed_func[cmd], self.m_logic)
		end
	end
end

function IntervalTaskQueue:UpdateQueue()
	if not self.wait_time then
		self.wait_time = 0
	end
	self.wait_time = self.wait_time + UnityEngine.Time.deltaTime
	if self.curr_interval and self.wait_time < self.curr_interval then
		return
	else 
		self.curr_interval = 0
		self.wait_time = 0
	end

	if self.m_task_queue and not self.m_task_queue:isEmpty() then
		-- logError("queue size " .. self.m_task_queue:size())
		local queue_ele = self.m_task_queue:deQueue()
		while queue_ele do
			self.curr_interval = queue_ele.interval()
			DwDebug.LogError("IntervalTaskQueue", queue_ele.cmd_code, self.curr_interval)
			queue_ele.orig_call()

			if self.curr_interval <= 0 and not self.m_task_queue:isEmpty() then
				queue_ele = self.m_task_queue:deQueue()
			else 
				queue_ele = nil
			end
		end
	end
end


