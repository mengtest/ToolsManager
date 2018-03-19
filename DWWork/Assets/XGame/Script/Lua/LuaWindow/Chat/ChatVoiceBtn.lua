ChatVoiceBtn = class("ChatVoiceBtn", nil)

local recoredTimer
local m_recoredTimeValue
local m_isRecording = false
local m_canRecored = true

function ChatVoiceBtn:ctor(gb)
	self.gameObject = gb
	self.transform = gb.transform
end

function ChatVoiceBtn:Init(luaComRoot)
	self.m_luaComRoot = luaComRoot
	m_recoredTimeValue = WrapSys.GetTimeValue()
end

function ChatVoiceBtn:OnPress(isPress)
	if isPress then
		self:OnRecoredStart()
	else
		self:OnRecoredEnd()
	end
end


-- 按下按钮
function ChatVoiceBtn:OnRecoredStart()
	if PlayGameSys.GetPlayLogic() and PlayGameSys.GetPlayLogic().roomObj and PlayGameSys.GetPlayLogic().roomObj:GetBigResult() then
		WindowRoot.ShowTips("当前局已结束,请点击总结算按钮查看详情")
		return
	end

	-- if NimChatSys.CheckIsPlayVoice() then
	--  	WindowRoot.ShowTips("正在播放其他玩家语音，请稍后再发送语音")
	--  	return
	-- end

	if m_recoredTimeValue.Value > 0 then
		WindowUtil.LuaShowTips("录音点击太快，请稍后再试！")
		return
	end
	
	if not NimChatSys.GetLoginState() then
		return
	end
	
	m_recoredTimeValue.Value = 2
	m_isRecording = true
	
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "recoredAudio_ui_window", false , nil)
	NimChatSys.RecordAudio(true)
	recoredTimer = TimerTaskSys.AddTimerEventByLeftTime(function ()
		NimChatSys.RecordAudio(false)
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "recoredAudio_ui_window", false , nil)
	end, 30)
end

--松开按钮
function ChatVoiceBtn:OnRecoredEnd()
	if not m_isRecording then
		return
	end
	m_isRecording = false
	m_recoredTimeValue.Value = 1
	
	if WindowRoot.ClickPointInCollider("btn_speak") then
		NimChatSys.RecordAudio(false)
	else
		NimChatSys.RecordAudioCancel()
	end
	TimerTaskSys.RemoveTask(recoredTimer)
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "recoredAudio_ui_window", false , nil)
end

function ChatVoiceBtn:OnDragOver()
	LuaEvent.AddEventNow(EEventType.RecoredAudioWindowState,true)
end

function ChatVoiceBtn:OnDragOut()
	LuaEvent.AddEventNow(EEventType.RecoredAudioWindowState,false)
end

return ChatVoiceBtn