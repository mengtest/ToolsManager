------------------------------------------------------------------
-- 文件名:	QueueTask.lua
-- 版  权:	(C) 
-- 创建人:	jianing
-- 日  期:	2018-01-03   15:48
-- 版  本:	1.0
-- 描  述:	队列任务
------------------------------------------------------------------
QueueTask = class("QueueTask", nil)

function QueueTask:ctor()
	self:Clear()
end

function QueueTask:Init()

end

function QueueTask:Clear()
	self.tasks = {}
	self.tempTask = nil
	self.tickTime = 0
	self.canUpdate = false
end

function QueueTask:Update()
	if not self.canUpdate then
		return
	end

	--加个超时 防止卡住一直不播放
	self.tickTime = self.tickTime - UnityEngine.Time.deltaTime
	if self.tempTask then
		if self.tickTime <= 0 then
			self.tempTask.func()
			self.tempTask = nil
		end
		return
	end
	
	if #self.tasks == 0 then
		self.canUpdate = false
	else
		self.tempTask = self.tasks[1]
		table.remove(self.tasks, 1)
		self.tickTime = self.tempTask.time
	end
end

--加入队列
function QueueTask:AddToQueue(_func,_time)
	local localTask = 
	{
		func = _func,
		time = _time
	}
	table.insert(self.tasks,localTask)

	self.canUpdate = true
end

return QueueTask
