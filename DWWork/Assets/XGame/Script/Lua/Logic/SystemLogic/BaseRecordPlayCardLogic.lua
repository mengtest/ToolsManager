--------------------------------------------------------------------------------
-- 	 File      : BaseRecordPlayCardLogic.lua
--   author    : guoliang
--   function   : 游戏回放逻辑基类
--   date      : 2017-11-13
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.DWBaseModule"
require "Logic.RoomLogic.BaseObjectState"

BaseRecordPlayCardLogic = class("BaseRecordPlayCardLogic", nil)
function BaseRecordPlayCardLogic:BaseCtor()
	self.UIReady = false
	self.playInterval = 1
end

function BaseRecordPlayCardLogic:BaseInit()
	--注册事件消息回调
	self:RegisterEvent()
end


function BaseRecordPlayCardLogic:BaseDestroy()
	self:UnRegisterEvent()
	self:EndPlay()
end

function BaseRecordPlayCardLogic:GetType()
	return PlayLogicTypeEnum.None
end


-------------------Event Start-----------------------------------------

function BaseRecordPlayCardLogic:RegisterEvent()
	LuaEvent.AddHandle(EEventType.RecordUIReady,self.RecordUIReady,self)
	LuaEvent.AddHandle(EEventType.RecordStartPlay,self.RecordStartPlay,self)
	LuaEvent.AddHandle(EEventType.RecordReplayCurRound,self.RecordReplayCurRound,self)
	LuaEvent.AddHandle(EEventType.RecordPausePlay,self.RecordPausePlay,self)
	LuaEvent.AddHandle(EEventType.RecordSetPlaySpeed,self.RecordSetPlaySpeed,self)
end


function BaseRecordPlayCardLogic:UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.RecordUIReady,self.RecordUIReady,self)
	LuaEvent.RemoveHandle(EEventType.RecordStartPlay,self.RecordStartPlay,self)
	LuaEvent.RemoveHandle(EEventType.RecordReplayCurRound,self.RecordReplayCurRound,self)
	LuaEvent.RemoveHandle(EEventType.RecordPausePlay,self.RecordPausePlay,self)
	LuaEvent.RemoveHandle(EEventType.RecordSetPlaySpeed,self.RecordSetPlaySpeed,self)
end


function BaseRecordPlayCardLogic:RecordUIReady(eventId,p1,p2)
	self.UIReady = true
end

function BaseRecordPlayCardLogic:RecordStartPlay(eventId,p1,p2)
	local recordBody = p1
	if recordBody then
		self:StartPlay(recordBody)
	end
end

function BaseRecordPlayCardLogic:RecordReplayCurRound(eventId,p1,p2)
	self:ReplayCurRound()
end

function BaseRecordPlayCardLogic:RecordPausePlay(eventId,p1,p2)
	local isPause = p1
	self:PausePlay(isPause)
end

function BaseRecordPlayCardLogic:RecordSetPlaySpeed(eventId,p1,p2)
	local playSpeed = p1

	--设置时间scale
	GameManager.SetGameTimeScale(playSpeed)
end
----------------- Event End ------------------------------------------------


---------------------播放控制区------------------------------------------

function BaseRecordPlayCardLogic:StartPlay(recordBody)
	self:CleanRoom()

	self.recordBody = recordBody
	self.totalPlayNum = #recordBody.cells
	self.loopTimeDelta = 3 -- 立即播放房间信息
	self.curPlayIndex = 1
	self.isPlaying = true
	--刷新暂停按钮状态
	LuaEvent.AddEventNow(EEventType.RefreshRecordPauseStatus,false)
	LuaEvent.AddEventNow(EEventType.RefreshRecordBtns)

	if not self.isRegisterUpdate then
		self.isRegisterUpdate = true
		UpdateBeat:Add(self.Update,self)
	end
end
-- 暂停播放
function BaseRecordPlayCardLogic:PausePlay(isPause)
	self.isPlaying = not isPause
end
-- 结束播放
function BaseRecordPlayCardLogic:EndPlay()
	if self.isRegisterUpdate then
		self.isRegisterUpdate = false
		UpdateBeat:Remove(self.Update,self)
	end

end

-- 重新播放当前轮
function BaseRecordPlayCardLogic:ReplayCurRound()
	self:StartPlay(self.recordBody)
end

-------------------------------------------播放控制区 end---------------------------------------------------------



-------------------------------------------基础播放功能--------------------------------------------------------


-- 查找一局开始的消息编号
function BaseRecordPlayCardLogic:FindRoundStartIndex(targetRound)

end
-- loop更新
function BaseRecordPlayCardLogic:Update()
	if not self.UIReady then
		return
	end

	if not self.isPlaying then
		return
	end

	self.loopTimeDelta = self.loopTimeDelta + UnityEngine.Time.deltaTime

	if self.loopTimeDelta >= self.playInterval then
		self.loopTimeDelta = 0
		self:PlayRecordItem(self.curPlayIndex)
		self.curPlayIndex = self.curPlayIndex + 1
	end

end
-- 消息pb解析
function BaseRecordPlayCardLogic:DecordRecordItem(index)
	if index <= self.totalPlayNum then
		local recordItem = self.recordBody.cells[index]
		if recordItem then
			local byteData = recordItem.eventData

			if byteData == nil then
				DwDebug.LogError("DecordRecordItem byteData is nil")
			end
			local body = ProtoManager.Decode(recordItem.eventId,recordItem.eventData)
			if body then
				if WrapSys.IsEditor then
					DwDebug.Log(body)
				end
			end
			return recordItem.eventId, body
		end
	end
end
-- 播放一个消息单元
function BaseRecordPlayCardLogic:PlayRecordItem(index)
	local eventId,body = self:DecordRecordItem(index)
	if body then
		self:PublicEvent(eventId, body)
	end
end

-- 广播消息
function BaseRecordPlayCardLogic:PublicEvent(eventId,rsp)
	
end


function BaseRecordPlayCardLogic:CleanRoom()

end