--------------------------------------------------------------------------------
-- 	 File      : NimChatSys.lua
--   author    : jianing
--   function  : nim语音聊天系统(主要和ChatInterface.cs对接)
--   date      : 2017-11-08
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
local utils = require "utils"

NimChatSys = {}
local _s = NimChatSys

local nimAppKey = "3364b7a1dac9ee0a82415e937dfd4499"
local nimAppSecret = "87196bd8b591"
local m_imID,m_token

local m_recordTime = 0

local needJointTeamID = ""	--需要加入的房间

local m_isPlayVoice = false --是否正在播放声音

function NimChatSys.Init()
	if Constants.EnableIM then
		needJointTeamID = ""
		--ChatInterface.getInstance():InitIMSDK(nimAppKey,nimAppSecret)
		ChatInterface.getInstance():initLuaFunc()

		LuaEvent.AddHandle(EEventType.NimIsNotInTeam, _s.NimIsNotInTeam, nil)
	end
end

function NimChatSys.Reset()
	
end

function NimChatSys.Destroy()
	LuaEvent.RemoveHandle(EEventType.NimIsNotInTeam, _s.NimIsNotInTeam, nil)
end


--登陆IM 服务器下发的UID和im_token
function NimChatSys.LoginIm(imID,token)
--	error("NimChatSys step 1 ,begin"..WrapSys.GetCurrentDateTime())
	if imID == nil or token == nil then
		DwDebug.LogError("NimChatSys.LoginIm imID is nil or token is nil")
		return
	end
	m_imID = imID
	m_token = token
	ChatInterface.getInstance():loginIm(nimAppKey,nimAppSecret,imID,token)
end

--登出
function NimChatSys.LogoutIm()
	ChatInterface.getInstance():logoutIm()
end

--加入IM房间
function NimChatSys.ApplyJoinTeam(teamid)
	if ChatInterface.getInstance():getIMLoginStatus() then
		needJointTeamID = ""
		ChatInterface.getInstance():applyJoinTeam(teamid)
	else
		needJointTeamID = teamid
	end
end

--创建房间
function NimChatSys.CreateChatTeam(teamName)
	ChatInterface.getInstance():createChatTeam(teamName)
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+10, "wait_ui_window")
end

--查询群信息并且解散群
function NimChatSys.QueryAllMyTeamAndDismiss()
	ChatInterface.getInstance():queryAllMyTeamAndDismiss()
end

--发送文字消息
function NimChatSys.SendTextMessage(msg)
	if not NimChatSys.GetLoginState() then
		return
	end

	ChatInterface.getInstance():sendTextMessage(_s.GetNowGroupId(),msg)
end

--录音
function NimChatSys.RecordAudio(isStart)
	if not NimChatSys.GetLoginState() then
		return
	end

	if isStart then
		if m_recordTime == 0 then
			ChatInterface.getInstance():startRecordAudio()
			m_recordTime = UnityEngine.Time.time
			AudioManager.PauseVoice()
		end
	else
		if m_recordTime ~= 0 and UnityEngine.Time.time - m_recordTime < 0.8 then
		  	_s.RecordAudioCancel()
		  	WindowUtil.LuaShowTips("录音时间太短")
		else
			ChatInterface.getInstance():endRecordAudio()
			AudioManager.ResumeVoice()
		end
		m_recordTime = 0
	end
end

--录音结果返回
function NimChatSys.StartRecordAudioCallBack(code)
	if code ~= 200 then
		if code == 103 then --正在采集中，操作失败
		  	_s.RecordAudioCancel()
		end

		WindowUtil.LuaShowTips("录音失败，请检查录音权限或尝试重启")
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, false, 1, "recoredAudio_ui_window", false , nil)
		m_recordTime = 0
		AudioManager.ResumeVoice()
	end
end


--取消录音
function NimChatSys.RecordAudioCancel()
	ChatInterface.getInstance():OnRecordAudioCancel()
	AudioManager.ResumeVoice()
	m_recordTime = 0
end

--发送语言消息
function NimChatSys.SendAudioMessage(path)
	if not NimChatSys.GetLoginState() then
		return
	end
	ChatInterface.getInstance():sendAudioMessage(_s.GetNowGroupId(), path)
end

--播放语言消息
function NimChatSys.PlayVoiceAudio(path)
	--if not WrapSys.isMuted() then
	if not m_isPlayVoice then
		m_isPlayVoice = true
		ChatInterface.getInstance():playVoiceAudio(path)
		AudioManager.PauseVoice()
	end
	--end
end

--取消录音
function NimChatSys.OnPlayAudioStop()
	--防止太快了吧
	TimerTaskSys.AddTimerEventByLeftTime(function ()
		ChatInterface.getInstance():OnPlayAudioStop()
		AudioManager.ResumeVoice()
	end, 0.2, nil)
end

--获取当前房间号
function NimChatSys.GetNowGroupId()
	local playCardLogic = PlayGameSys.GetPlayLogic()
	if playCardLogic then
		return playCardLogic.msgGroupId
	end
	return ""
end

--发送表情
--toPlayId 到达目标ID
function NimChatSys.SendImojMessage(index,toPlayId)
	if not NimChatSys.GetLoginState() then
		return
	end

	local playerinfo = PlayGameSys.GetPlayerByPlayerID(tonumber(toPlayId))
	if playerinfo == nil then
		return
	end

	local msgtable = {}
	msgtable.type = 2
	msgtable.index = index
	msgtable.msg = ""
	msgtable.path = ""
	msgtable.headanireceiverid = ""
	msgtable.toPlayId = toPlayId
	local msgtablestr = ToString(msgtable)
	_s.SendTextMessage(msgtablestr)

	local emojiInfo = {}
	emojiInfo.fromSeatPos = SeatPosEnum.South
	emojiInfo.toSeatPos = playerinfo.seatPos
	emojiInfo.emojiID = index
	LuaEvent.AddEventNow(EEventType.PlayEmoji,emojiInfo)
end

--检查登陆状态
function NimChatSys.GetLoginState()
	local loginSuc = ChatInterface.getInstance():GetLoginState()
	if not loginSuc then
		_s.NoticeNotLogin()
	end
	return loginSuc
end

--是否再录音中
function NimChatSys.CheckIsRecord()
	return m_recordTime > 0
end

function NimChatSys.CheckIsPlayVoice()
	return m_isPlayVoice
end
----------------------------------下面是接收的监听推送----------------------------
--登陆返回
function NimChatSys.IMLoginCallBack(isSuccess)
--	error("NimChatSys step 2 ,IMLoginCallBack "..WrapSys.GetCurrentDateTime())
	if isSuccess then
		--需要加入房间
		if needJointTeamID ~= "" then
			_s.ApplyJoinTeam(needJointTeamID)
		end
	else
		DwDebug.LogError("NimChatSys.IMLoginCallBack : fail")
	end
end

--登出返回
function NimChatSys.IMLogoutCallBack()
	DwDebug.Log("NimChatSys.IMLogoutCallBack : success")
end

--创建群返回
function NimChatSys.CreateTeamCallBack(isSuccess,owerID, teamID)
	-- if isSuccess then
	-- 	HallSys.CreateRoomByIM(true,owerID, teamID)
	-- else
	-- 	local contentStr = "创建房间失败，请稍后再试"
	-- 	WrapSys.HandleErrorWindow_ContentSt = contentStr
	-- 	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 0, "")
	-- 	HallSys.CreateRoomByIM(false)
	-- end
	-- WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.wait_ui_window, false,0,"")
end

--接收到消息
function NimChatSys.ReceiveText(senderid, text)
	if senderid and text then
		LuaEvent.AddEvent(EEventType.Chat_ReceiveMessage, senderid, text)
	end
end

--接收语音
function NimChatSys.ReceiveVoice(senderid, localpath, timeLength)
	--DwDebug.LogError("ReceiveVoice msg handle")
	if senderid and localpath and timeLength and timeLength >= 0 then
		local datatb = {}
		datatb.senderid = senderid
		datatb.localpath = localpath
		datatb.timelength = timeLength/1000
		LuaEvent.AddEvent(EEventType.Chat_ReceiveVoiceMessage, datatb)
	else
		DwDebug.LogError("ReceiveVoice error")
	end
end

--录音结果
function NimChatSys.RecordResult(resultCode,path,timelength)
	if resultCode == 1 then	--成功
		DwDebug.LogError("RecordResult msg success to send")
		local datatb = {}
		datatb.senderid = DataManager.GetUserID()
		datatb.localpath = path
		datatb.timelength = timelength/1000
		LuaEvent.AddEvent(EEventType.Chat_ReceiveVoiceMessage, datatb)
		_s.SendAudioMessage(path)
	elseif resultCode == 2 then -- 失败 
		
	elseif resultCode == 3 then --声音太短
		LuaEvent.AddEvent(EEventType.Chat_RecoredTooShort)
	end
end

--播放完成
function NimChatSys.AudioPlayEnd(resCode,filepath,state)
	m_isPlayVoice = false
	AudioManager.ResumeVoice()
	LuaEvent.AddEvent(EEventType.Chat_PlayVoiceEnd, filepath, nil)
end

--提示需要登录
function NimChatSys.NoticeNotLogin()
	if m_imID and m_token then	
		WindowUtil.LuaShowTips("网络不稳定,正在重新连接聊天服务器")
		--NimChatSys.LoginIm(m_imID,m_token)
	end
end

--没有加入房间提示
function NimChatSys.NimIsNotInTeam( eventId,p1,p2 )
	WindowUtil.LuaShowTips("网络不稳定,正在重新加入聊天服务器")
	NimChatSys.ApplyJoinTeam(_s.GetNowGroupId())
end