--------------------------------------------------------------------------------
-- 	 File      : chat_ui_window.lua
--   author    : jianing
--   function  : 聊天窗口 
--   date      : 2017-11-9
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Tools.EZToggleGroup"
require "LuaWindow.Chat.ChatVoiceQueueController"
require "LuaWindow.Chat.ChatHistoryItem"
require "LuaWindow.Chat.ChatHistoryData"
require "LuaSys.TimerTaskSys"
local utils = require "utils"
local quickInfo = require "LuaWindow.Chat.QuickVoiceInfo"

chat_ui_window = {}
local _s = chat_ui_window

local m_luaWindowRoot
local m_state

--内容框
local chatpanel_root
local msg_show_root
local recoredTooShort
local m_toggleTrans = {}
local m_content_panel_table = {}

local m_chat_toggle_group
local m_label_input
local m_UIInputComponent
local m_panel_history
local m_history_grid
local m_panel_imoj
local m_panel_quickvoice
local m_btn_send
local m_player_msg_table = {}
local m_player_msg_show_root_table = {}
local m_timer_text_table = {}
local m_timer_quick_table = {}
local m_timer_voice_table = {}
local m_btn_bg
local m_itemObjs = {}
local m_PlayerPos = {}

local m_ScrollView_Quick	--快捷语音滚动
local m_ScrollView_History  --历史记录

local nowTabIndex = 3 --当前tab序列

local m_nowPlayId = 0
local m_nowPlaySound = nil

local isFangYan = false

function chat_ui_window.Init(luaWindowRoot)
	m_luaWindowRoot = luaWindowRoot
	ChatVoiceQueueController.init()
	ChatHistoryData.init()
end

function chat_ui_window:CreateWindow()
	chatpanel_root 			= m_luaWindowRoot:GetTrans("chatpanel_root")
	msg_show_root 			= m_luaWindowRoot:GetTrans("msg_show_root")
	m_btn_bg				= m_luaWindowRoot:GetTrans("btn_bg")
	recoredTooShort 		= m_luaWindowRoot:GetTrans("recoredTooShort")
	m_panel_history			= m_luaWindowRoot:GetTrans("ScrollView_History")
	m_panel_imoj			= m_luaWindowRoot:GetTrans("ScrollView_Imoj")
	m_panel_quickvoice		= m_luaWindowRoot:GetTrans("ScrollView_Quick")
	m_label_input			= m_luaWindowRoot:GetTrans("msgInput")
	m_UIInputComponent		= m_label_input:GetComponent("UIInput")
	m_btn_send				= m_luaWindowRoot:GetTrans("btn_send")
	m_history_grid 			= m_luaWindowRoot:GetTrans("Grid_history")

	for i=1,4 do
		local playermsgshowroot = m_luaWindowRoot:GetTrans("Panel_player" .. i)
		if playermsgshowroot then
			m_player_msg_show_root_table[i] = playermsgshowroot

			local playertable = {}
			playertable.msg_voice_img = m_luaWindowRoot:GetTrans(playermsgshowroot,"Sprite_voice_player")
			playertable.msg_bg_img = m_luaWindowRoot:GetTrans(playermsgshowroot,"Sprite_msg_bg_player")
			playertable.msg_text = m_luaWindowRoot:GetTrans(playermsgshowroot,"Label_msg_text_player")
			playertable.msg_context = m_luaWindowRoot:GetTrans(playermsgshowroot,"Label_context_text_player")

			playertable.msg_imoj_parent = m_luaWindowRoot:GetTrans(playermsgshowroot,"myEmojiContent_player")
			playertable.msg_imoj_bg = m_luaWindowRoot:GetTrans(playermsgshowroot,"imoj_bg_player")
			
			m_luaWindowRoot:SetActive(playertable.msg_voice_img,false)
			m_luaWindowRoot:SetActive(playertable.msg_bg_img,false)
			m_luaWindowRoot:SetActive(playertable.msg_imoj_parent,false)
			m_luaWindowRoot:SetActive(playertable.msg_context,false)
			m_luaWindowRoot:SetActive(playertable.msg_text,false)
			m_player_msg_table[i] = playertable
		end
	end

	m_content_panel_table = {}
	table.insert(m_content_panel_table,m_panel_history)
	table.insert(m_content_panel_table,m_panel_imoj)
	table.insert(m_content_panel_table,m_panel_quickvoice)

	_s.createChatToggleGroup()
	_s.RegisterEventHandle()
	_s.firstShow = true
end

function chat_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, -1, false)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		m_nowPlayId = state
		if m_nowPlayId == Common_PlayID.chongRen_510K 
			or m_nowPlayId == Common_PlayID.leAn_510K 
			or m_nowPlayId == Common_PlayID.yiHuang_510K then
				m_nowPlayId = Common_PlayID.chongRen_510K
		elseif m_nowPlayId == Common_PlayID.chongRen_MJ 
			or m_nowPlayId == Common_PlayID.leAn_MJ
			or m_nowPlayId == Common_PlayID.yiHuang_MJ then
				m_nowPlayId = Common_PlayID.chongRen_MJ
		end

		_s.InitWindowDetail()
	end
end

function chat_ui_window:OnDestroy()
	_s.RemoveEventHandle()
	m_ScrollView_Quick = nil
	m_ScrollView_History = nil
	ChatHistoryData.Clear()
	ChatVoiceQueueController.Clear()
end

function chat_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.Chat_ReceiveMessage, _s.ReceiveMessage, nil)
	LuaEvent.AddHandle(EEventType.Chat_ReceiveVoiceMessage, _s.ReceiveVoiceMessage, nil)
	LuaEvent.AddHandle(EEventType.Chat_RecoredTooShort, _s.RecoredTooShort, nil)
	LuaEvent.AddHandle(EEventType.Chat_PlayVoice, _s.PlayVoiceAudio)
	LuaEvent.AddHandle(EEventType.Chat_PlayVoiceEnd, _s.PlayVoiceAudioEnd)
	LuaEvent.AddHandle(EEventType.PlayerHeadPosChange, _s.playerPosChange)
	LuaEvent.AddHandle(EEventType.Chat_PlayVoice,_s.VoiceStartPlayCB)

	LuaEvent.AddHandle(EEventType.Chat_ui_window_Show,_s.ShowChatpanel)
end

function chat_ui_window.RemoveEventHandle()
	LuaEvent.RemoveHandle(EEventType.Chat_ReceiveMessage, _s.ReceiveMessage, nil)
	LuaEvent.RemoveHandle(EEventType.Chat_ReceiveVoiceMessage, _s.ReceiveVoiceMessage, nil)
	LuaEvent.RemoveHandle(EEventType.Chat_RecoredTooShort, _s.RecoredTooShort, nil)
	LuaEvent.RemoveHandle(EEventType.Chat_PlayVoice, _s.PlayVoiceAudio)
	LuaEvent.RemoveHandle(EEventType.Chat_PlayVoiceEnd, _s.PlayVoiceAudioEnd)
	LuaEvent.RemoveHandle(EEventType.PlayerHeadPosChange, _s.playerPosChange)
	LuaEvent.RemoveHandle(EEventType.Chat_PlayVoice,_s.VoiceStartPlayCB)

	LuaEvent.RemoveHandle(EEventType.Chat_ui_window_Show,_s.ShowChatpanel)
end

function chat_ui_window.InitWindowDetail( )
	m_luaWindowRoot:SetActive(chatpanel_root,false)
	_s.InitScrollViewHistory()
	_s.InitScrollViewQuick()
end

function chat_ui_window.ShowChatpanel(eventId,p1,p2)
	m_luaWindowRoot:SetActive(chatpanel_root,true)
	_s.SwitchTab(nowTabIndex)
	_s.InitScrollViewHistory()

	if isFangYan ~= DataManager.CheckNowIsFangYan() then
		_s.InitScrollViewQuick()
	end
end

--初始化快捷语音
function chat_ui_window.InitScrollViewQuick()
	if m_ScrollView_Quick == nil then
		local m_ScrollViewRoot = m_luaWindowRoot:GetTrans("ScrollView_Quick_root")
		local itemPre = m_luaWindowRoot:GetTrans("quickvoiceitemPre").gameObject
		m_ScrollView_Quick = EZfunLimitScrollView.GetOrAddLimitScr(m_ScrollViewRoot)
		m_ScrollView_Quick:InitForLua(m_ScrollViewRoot,itemPre,UnityEngine.Vector2.New(490, 60), UnityEngine.Vector2.New(1 , 9), LimitScrollViewDirection.SVD_Vertical, false)
		m_ScrollView_Quick:SetInitItemCallLua("chat_ui_window.InitQuickScrollViewItem")
	end

	isFangYan = DataManager.CheckNowIsFangYan()
	m_ScrollView_Quick:InitItemCount(#quickInfo.GetInfoList(m_nowPlayId), true)
end

--设置快捷语音单项Item
function chat_ui_window.InitQuickScrollViewItem(trans, index)
	index = index + 1
	local quickInfoItem = quickInfo.GetQuickInfo(m_nowPlayId,index,DataManager.CheckNowIsFangYan())
	trans.name = "quickvoiceitem"..index
	if quickInfoItem then
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans,"Label_text"),quickInfoItem.text,false)
	end
end

--收到播放历史记录语音请求
function chat_ui_window.VoiceStartPlayCB(eventid, data)
	if data == nil or not data.isPlayed then
		return
	end
	local index = ChatHistoryData.GetIndexByData(data)
	local itemTrans = m_ScrollView_History:GetUseTransformByIndex(index - 1)
	if itemTrans then
		ChatHistoryItem:OnPlayVoice(m_luaWindowRoot,itemTrans,data)
	end

	DwDebug.LogError("VoiceStartPlayCB "..data.msgVoicePath)
 	NimChatSys.PlayVoiceAudio(data.msgVoicePath)
end

function chat_ui_window.PlayVoiceAudioEnd(eventid, file)
	ChatHistoryItem:OnStopVoice()
end

--初始化历史记录
function chat_ui_window.InitScrollViewHistory()
	if m_ScrollView_History == nil then
		local m_ScrollViewRoot = m_luaWindowRoot:GetTrans("ScrollView_History_root")
		local itemPre = m_luaWindowRoot:GetTrans("historyitem").gameObject
		m_ScrollView_History = EZfunLimitScrollView.GetOrAddLimitScr(m_ScrollViewRoot)
		m_ScrollView_History:InitForLua(m_ScrollViewRoot,itemPre,UnityEngine.Vector2.New(490, 60), UnityEngine.Vector2.New(1 , 9), LimitScrollViewDirection.SVD_Vertical, false)
		m_ScrollView_History:SetInitItemCallLua("chat_ui_window.InitHistoryScrollViewItem")
	end

	local count = ChatHistoryData.GetHistoryCount()
	m_ScrollView_History:InitItemCount(count, true)
	m_ScrollView_History.EndIndex = count - 1
end

--添加单条历史记录
function chat_ui_window.InitHistoryScrollViewItem(trans, index)
	index = index + 1
	local historyinfo = ChatHistoryData.GetHistoryByIndex(index)
	trans.name = "historyitem"..index
	if historyinfo then
		_s.initHistoryItem(trans, historyinfo, index)
	end
end

--设置单条数据
function chat_ui_window.initHistoryItem(trans, historyinfo, index)
	ChatHistoryItem:init(trans, m_luaWindowRoot)
	ChatHistoryItem:setData(historyinfo, index)
end

function chat_ui_window.HandleWidgetClick(gb)
	local clickName = gb.name
	if string.find(clickName, "toggle_") then
		local index = string.sub(clickName, 8)
		_s.SwitchTab(tonumber(index))
	elseif clickName == "btn_send" then
		_s.HandleSendClick()
	elseif string.find(clickName, "emoji_1") then
		_s.HandleImojClick(clickName)
	elseif string.find(clickName, "quickvoiceitem") then
		_s.HandleQuickVoiceClick(clickName)
	elseif clickName == "btn_bg" then
		_s.HandleOnBg()
	elseif string.find(clickName, "historyitem") then
		_s.HandleOnHistoryItem(gb)
	end
end

--点击播放语音
function chat_ui_window.HandleOnHistoryItem(gb)
	local index = tonumber(string.sub(gb.name, 12))
	if index == nil or index <= 0 then
		DwDebug.LogError("HandleOnHistoryItem " .. itemname)
		return
	end
	--点击到语音 播放
	local historyinfo = ChatHistoryData.GetHistoryByIndex(index)
	if historyinfo and historyinfo.msgType == 2 then
		ChatHistoryItem:TouchOnItem(gb.transform,historyinfo)
		ChatVoiceQueueController.addToQueue(historyinfo)
	end
end

--点击到背景
function chat_ui_window.HandleOnBg()
	m_luaWindowRoot:SetActive(chatpanel_root,false)
end

--初始化 toggle
function chat_ui_window.createChatToggleGroup()
	m_chat_toggle_group = EZToggleGroup.New()
	m_chat_toggle_group:Init(m_luaWindowRoot)

	local chat_group = m_luaWindowRoot:GetTrans(chatpanel_root, "group_chat_title")
	local toggle_1 = m_luaWindowRoot:GetTrans(chat_group,"toggle_1")
	local toggle_2 = m_luaWindowRoot:GetTrans(chat_group,"toggle_2")
	local toggle_3 = m_luaWindowRoot:GetTrans(chat_group,"toggle_3")

	m_chat_toggle_group:AddTrans(toggle_1,1)
	m_chat_toggle_group:AddTrans(toggle_2,1)
	m_chat_toggle_group:AddTrans(toggle_3,1,true)

	m_toggleTrans = {}
	table.insert(m_toggleTrans,toggle_1)
	table.insert(m_toggleTrans,toggle_2)
	table.insert(m_toggleTrans,toggle_3)
end

--切换内容
function chat_ui_window.SwitchTab(index)
	nowTabIndex = index
	for i=1,#m_content_panel_table do
		if index == i then
			m_luaWindowRoot:SetActive(m_content_panel_table[i],true)
		else
			m_luaWindowRoot:SetActive(m_content_panel_table[i],false)
		end
	end
	m_chat_toggle_group:Select(m_toggleTrans[index])
end

--刷新玩家头像位置
function chat_ui_window.playerPosChange(eventid, posTable)
	for i=1,#posTable do
		local pos = {} 
		pos.x = posTable[i].x
		pos.y = posTable[i].y
		pos.z = posTable[i].z
		m_PlayerPos[i] = pos
	end
end

--设置头像位置
function chat_ui_window.setChatNodePosition(uiseat)
	local newpos = nil
	local playerpos = m_PlayerPos[uiseat]
	if playerpos == nil then
		return
	end
	if uiseat == 1 or uiseat == 4 then
		newpos = UnityEngine.Vector3.New(playerpos.x + 0.13, playerpos.y - 0.05, playerpos.z)
	elseif uiseat == 2 or uiseat == 3 then
		newpos = UnityEngine.Vector3.New(playerpos.x - 0.15, playerpos.y - 0.05, playerpos.z)
	end
	m_player_msg_show_root_table[uiseat].position = newpos
end

function chat_ui_window.HandleSendClick()
	if not NimChatSys.GetLoginState() then
		return
	end

	--local label = m_luaWindowRoot:GetTrans("Label_input")
	local inmsg = m_UIInputComponent.value

	local count = _s.getCharCount(inmsg)
	if count > 20 then
		inmsg = string.sub(inmsg, 0, 20)
	elseif count <= 0 then
		WindowUtil.LuaShowTips("请先输入内容！")
		return
	end
	local parserMsg = _s:sensitiveParser(inmsg)
	m_UIInputComponent.value = ""

	local msgtable = {}
	msgtable.type = 1
	msgtable.index = ""
	msgtable.msg = inmsg
	msgtable.path = ""
	msgtable.headanireceiverid = ""
	local msgtablestr = ToString(msgtable)

	_s.SetChatPanel(false)
	_s.ReceiveTextMessage(DataManager.GetUserID(), inmsg)
	NimChatSys.SendTextMessage(msgtablestr)
end

function chat_ui_window.getCharCount( msg )
	local lenInByte = #msg
	local count = 0
	local i = 1

	while true do
	    if i <= lenInByte then
	        local curByte = string.byte(msg, i)
	        local byteCount = 1
	        if curByte>0 and curByte<=127 then
	            byteCount = 1
	        elseif curByte>=192 and curByte<223 then
	            byteCount = 2
	        elseif curByte>=224 and curByte<239 then
	            byteCount = 3
	        elseif curByte>=240 and curByte<=247 then
	            byteCount = 4
	        end
	        
	        i = i + byteCount
	        count = count + 1
	    else
	        break
	    end
	end
	return count
end

function chat_ui_window.sensitiveParser(msg)
	local MsgParser = require("LuaWindow.Chat.ChatMessageParser")
	local parserMsg = MsgParser:getString(msg)
	return parserMsg
end

function chat_ui_window.HandleImojClick(imojname)
	local index = string.sub(imojname, 9)
	_s.SetChatPanel(false)
	--_s.ReceiveImojMessage(DataManager.GetUserID(), index,DataManager.GetUserID())
	NimChatSys.SendImojMessage(index,DataManager.GetUserID())
end

function chat_ui_window.RecoredTooShort()
	WindowUtil.LuaShowTips("录音时间太短")
end

--快捷语音
function chat_ui_window.HandleQuickVoiceClick(quickname)
	if not NimChatSys.GetLoginState() then
		return
	end
	
	local index = string.sub(quickname, 15)
	local msgtable = {}
	msgtable.type = 3
	msgtable.index = tonumber(index)
	msgtable.msg = ""
	msgtable.path = ""
	msgtable.headanireceiverid = ""
	local msgtablestr = ToString(msgtable)
	_s.SetChatPanel(false)
	_s.ReceiveQuickVoiceMessage(DataManager.GetUserID(), index)
	NimChatSys.SendTextMessage(msgtablestr)
end

function chat_ui_window.ReceiveMessage(eventid, senderid, msgStr)
	local msgtable = loadstring("return " ..  msgStr)()
	if tonumber(msgtable.type) == 1 then
		_s.ReceiveTextMessage(senderid, msgtable.msg)
	elseif  tonumber(msgtable.type) == 2 then
		_s.ReceiveImojMessage(senderid, msgtable.index,msgtable.toPlayId)
	elseif  tonumber(msgtable.type) == 3 then
		_s.ReceiveQuickVoiceMessage(senderid, msgtable.index)
	elseif  tonumber(msgtable.type) == 4 then
		_s.ReceiveHeadAnimationMessage(senderid, msgtable.headanireceiverid, msgtable.index)
	elseif  tonumber(msgtable.type) == 5 then

	end
end

--清理定时器
function chat_ui_window.ClearTimeTask(seatID)
	--停止同一位置的其他定时器
	if seatID then
		--文字
		TimerTaskSys.RemoveTask(m_timer_text_table[seatID])
		m_timer_text_table[seatID] = nil
		--快捷语音
		TimerTaskSys.RemoveTask(m_timer_quick_table[seatID])
		m_timer_quick_table[seatID] = nil
		--语音
		TimerTaskSys.RemoveTask(m_timer_voice_table[seatID])
		m_timer_voice_table[seatID] = nil
	end
end

--清理全部定时器
function chat_ui_window.ClearAllTimeTask()
	for i=1,4 do
		chat_ui_window.ClearTimeTask(i)
	end
end

--接收文字类型消息
function chat_ui_window.ReceiveTextMessage(senderid, msg)
	local seatID = _s.GetSeatIDByUserID(senderid)
	local playmsgui = m_player_msg_table[seatID]

	if not playmsgui then
		return
	end

	m_luaWindowRoot:SetActive(msg_show_root, true)
	m_luaWindowRoot:SetActive(playmsgui.msg_bg_img, true)
	m_luaWindowRoot:SetActive(playmsgui.msg_context, false)
	m_luaWindowRoot:SetActive(playmsgui.msg_voice_img, false)
	m_luaWindowRoot:SetActive(playmsgui.msg_text, true)
	m_luaWindowRoot:SetLabel(playmsgui.msg_text, msg)

	_s.setChatNodePosition(seatID)
	_s.ClearTimeTask(seatID)

	local text = playmsgui.msg_text.gameObject:GetComponent("UILabel")
	local textsize = text.localSize
	local textw = textsize.x
	local rect = playmsgui.msg_bg_img.gameObject:GetComponent("UISprite")
	rect.width = textw + 30

	local playInfo = PlayGameSys.GetPlayerByPlayerID(tonumber(senderid))
	if playInfo == nil or playInfo.seatInfo == nil then
		return
	end

	--add to history
	local itemdata = {}
	itemdata.msgType = 1
	itemdata.senderID = senderid
	itemdata.msgContent = msg
	itemdata.msgVoicePath = nil
	itemdata.senderName = playInfo.seatInfo.nickName
	ChatHistoryData.AddData(itemdata)
	local count = ChatHistoryData.GetHistoryCount()
	m_ScrollView_History:InitItemCount(count, true)
	m_ScrollView_History.EndIndex = count - 1

	local texttimerindex = TimerTaskSys.AddTimerEventByLeftTime(function (_playmsgui)
		m_luaWindowRoot:SetLabel(_playmsgui.msg_text, "")
		m_luaWindowRoot:SetActive(_playmsgui.msg_bg_img, false)
	end, 3.5, nil,playmsgui)

	m_timer_text_table[seatID] = texttimerindex
end

--收到表情
function chat_ui_window.ReceiveImojMessage(senderid, index,toPlayId)
	local fromPlayInfo = PlayGameSys.GetPlayerByPlayerID(tonumber(senderid))
	local toPlayInfo = PlayGameSys.GetPlayerByPlayerID(tonumber(toPlayId))
	if fromPlayInfo == nil or toPlayInfo == nil then
		DwDebug.LogError("ReceiveImojMessage error senderid :".. tonumber(senderid) .. ", toPlayId:" .. tonumber(toPlayId))
		return
	end

	local seatID = _s.GetSeatIDByUserID(senderid)
	local playmsgui = m_player_msg_table[seatID]
	m_luaWindowRoot:SetActive(playmsgui.msg_bg_img, false)

	local emojiInfo = {}
	emojiInfo.fromSeatPos = fromPlayInfo.seatPos
	emojiInfo.toSeatPos = toPlayInfo.seatPos
	emojiInfo.emojiID = index
	LuaEvent.AddEventNow(EEventType.PlayEmoji,emojiInfo)
end

--接收快捷语音
function chat_ui_window.ReceiveQuickVoiceMessage(senderid, quickvoiceindex)
	local seatID = _s.GetSeatIDByUserID(senderid)
	local playmsgui = m_player_msg_table[seatID]

	if not playmsgui then
		return
	end

	m_luaWindowRoot:SetActive(msg_show_root, true)
	m_luaWindowRoot:SetActive(playmsgui.msg_bg_img, true)
	m_luaWindowRoot:SetActive(playmsgui.msg_text, true)
	m_luaWindowRoot:SetActive(playmsgui.msg_context, false)

	_s.setChatNodePosition(seatID)
	_s.ClearTimeTask(seatID)

	local quickin = quickInfo.GetQuickInfo(m_nowPlayId,tonumber(quickvoiceindex))
	--local msgtablestr = ToString(quickin)
	m_luaWindowRoot:SetLabel(playmsgui.msg_text, quickin.text)

	local text = playmsgui.msg_text.gameObject:GetComponent("UILabel")
	local textsize = text.localSize
	local textw = textsize.x
	local rect = playmsgui.msg_bg_img.gameObject:GetComponent("UISprite")
	rect.width = textw + 30

	local playerinfo = PlayGameSys.GetPlayerByPlayerID(tonumber(senderid))
	if playerinfo then
		if m_nowPlaySound then
			m_nowPlaySound:stop()
		end
		m_nowPlaySound = AudioManager.PlaySound(tonumber(quickvoiceindex),m_nowPlayId,playerinfo.seatInfo.sex)
	
		local playTime = AudioManager.GetResSoundTime(tonumber(quickvoiceindex),m_nowPlayId,playerinfo.seatInfo.sex)
		local quicktimerindex = TimerTaskSys.AddTimerEventByLeftTime(function (_playmsgui)
		m_luaWindowRoot:SetActive(_playmsgui.msg_bg_img, false)
		m_luaWindowRoot:SetLabel(_playmsgui.msg_text, "")
		end, playTime, nil, playmsgui)
		m_timer_quick_table[seatID] = quicktimerindex
	end
end

--互动表情
function chat_ui_window.ReceiveHeadAnimationMessage(datatable)
	LuaEvent.AddEventNow(EEventType.Chat_ReceiveHeadAniMessage, datatable)
end

--收到声音类消息
function chat_ui_window.ReceiveVoiceMessage(eventid ,datatb)
	--add to history
	local player = PlayGameSys.GetPlayerByPlayerID(tonumber(datatb.senderid))
	if not player or not player.seatInfo then
		DwDebug.LogError("chat_ui_window.ReceiveVoiceMessage can not found player:", datatb.senderid)
		return
	end
	DwDebug.LogError("chat_ui_window.ReceiveVoiceMessage ")
	local itemdata = {}
	itemdata.msgType = 2
	itemdata.senderID = datatb.senderid
	itemdata.msgContent = ""
	itemdata.msgVoicePath = datatb.localpath
	itemdata.senderName = player.seatInfo.nickName
	itemdata.timelength = datatb.timelength
	itemdata.isPlayed = false
	ChatVoiceQueueController.addToQueue(itemdata)
	itemdata.isPlayed = true
	ChatHistoryData.AddData(itemdata)
	m_ScrollView_History:InitItemCount(ChatHistoryData.GetHistoryCount(), true)
end

--播放声音
function chat_ui_window.PlayVoiceAudio(eventid, datatb)
	if datatb.isPlayed then
		return
	end
	local seatID = _s.GetSeatIDByUserID(datatb.senderID)
	local playmsgui = m_player_msg_table[seatID]
	m_luaWindowRoot:SetActive(msg_show_root, true)
	m_luaWindowRoot:SetActive(playmsgui.msg_bg_img, true)
	m_luaWindowRoot:SetActive(playmsgui.msg_context, true)
	m_luaWindowRoot:SetActive(playmsgui.msg_text, false)
	m_luaWindowRoot:SetActive(playmsgui.msg_voice_img, true)

	local rect = playmsgui.msg_bg_img.gameObject:GetComponent("UISprite")
	rect.width = 180

	_s.setChatNodePosition(seatID)
	_s.ClearTimeTask(seatID)

	local voicetimerindex = TimerTaskSys.AddTimerEventByLeftTime(function ()
		m_luaWindowRoot:SetActive(msg_show_root, false)
		m_luaWindowRoot:SetActive(playmsgui.msg_bg_img, false)
		m_luaWindowRoot:SetActive(playmsgui.msg_context, false)
		m_luaWindowRoot:SetActive(playmsgui.msg_voice_img, false)
	end, datatb.timelength + 0.1, nil)

	m_timer_voice_table[seatID] = voicetimerindex
	--DwDebug.LogError("PlayVoiceAudio func  "..datatb.msgVoicePath)
	NimChatSys.PlayVoiceAudio(datatb.msgVoicePath)
end

function chat_ui_window.SetChatPanel(status)
	m_luaWindowRoot:SetActive(chatpanel_root,status)
end

--1-4 东南西北
function chat_ui_window.GetSeatIDByUserID(userid)
	local playerinfo = PlayGameSys.GetPlayerByPlayerID(tonumber(userid))
	if playerinfo == nil or playerinfo.seatInfo == nil then
		return
	end

	return SeatPosSort[playerinfo.seatPos]
end
