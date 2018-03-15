
ChatVoiceQueueController = class("ChatVoiceQueueController", nil)
local _s = ChatVoiceQueueController
local tickTime = 0
local diffTime = 0.5

function ChatVoiceQueueController.init( )
	_s._voiceQueue = {}
	_s._canUpdate = false

	_s._chatVoicePlayEndCB = function ( )
		_s._chatVoicePlayEnd()
	end
	LuaEvent.AddHandle(EEventType.Chat_PlayVoiceEnd, _s._chatVoicePlayEndCB)
	UpdateBeat:Add(_s.Update,_s)
end

function ChatVoiceQueueController.Clear()
	LuaEvent.RemoveHandle(EEventType.Chat_PlayVoiceEnd, _s._chatVoicePlayEndCB)
end

function ChatVoiceQueueController._chatVoicePlayEnd( )
	tickTime = diffTime
end

function ChatVoiceQueueController.Update()
	if not _s._canUpdate then
		return
	end
	tickTime = tickTime - UnityEngine.Time.deltaTime

	if #_s._voiceQueue == 0 then
		_s._canUpdate = false
	else
		if tickTime <= 0 and not NimChatSys.CheckIsRecord() then
			local data = _s._voiceQueue[1]
			table.remove(_s._voiceQueue, 1)
			LuaEvent.AddEvent(EEventType.Chat_PlayVoice, data)
			tickTime = data.timelength + diffTime
		end
	end
end

function ChatVoiceQueueController.CheckCanAdd(data)
	if _s._voiceQueue == nil then
		return false
	end
	for k,v in pairs(_s._voiceQueue) do
		if v.msgVoicePath == data.msgVoicePath or 
			(v.isPlayed and data.isPlayed) then
			return false
		end
	end

	return true
end

function ChatVoiceQueueController.addToQueue(data)
	if not _s.CheckCanAdd(data) then
		return
	end

	local idata = {}
	idata.msgType = data.msgType
	idata.senderID = data.senderID
	idata.msgContent = data.msgContent
	idata.msgVoicePath = data.msgVoicePath
	idata.senderName = data.senderName
	idata.isPlayed = data.isPlayed
	idata.timelength = data.timelength
	if data.isPlayed then
		if tickTime > diffTime then
			NimChatSys.OnPlayAudioStop()
		end
		table.insert(_s._voiceQueue,1,idata)
	else
		table.insert(_s._voiceQueue,idata)
	end
	_s._canUpdate = true
end
