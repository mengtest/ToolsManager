--------------------------------------------------------------------------------
--   File      : userInfo_ui_window.lua
--   author    : jianing
--   function  : 查看玩家信息
--   date      : 2017-10-25
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

userInfo_ui_window = {
	
}

local _s = userInfo_ui_window

local m_luaWindowRoot
local m_state

local playLogic

local m_headIconTrans
local label_name
local label_position
local label_ip
local sprite_sex
local btn_distance
local btn_kick_out

local emojiTrans_1 = {}
local emojiTrans_2 = {}

local mySelfEmojiIndex = {1,4,5,11,12}

local m_playInfo
local m_isMySelf = false

function userInfo_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function userInfo_ui_window.InitParam(playInfo)
	m_playInfo = playInfo
end


function userInfo_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

function userInfo_ui_window.CreateWindow()
	m_headIconTrans = m_luaWindowRoot:GetTrans("ico_head")
	label_id = m_luaWindowRoot:GetTrans("label_id")
	label_name = m_luaWindowRoot:GetTrans("label_name")
	label_position = m_luaWindowRoot:GetTrans("label_position")
	label_ip = m_luaWindowRoot:GetTrans("label_ip")

	sprite_sex = m_luaWindowRoot:GetTrans("sprite_sex")
	btn_distance = m_luaWindowRoot:GetTrans("btn_distance")
	btn_kick_out = m_luaWindowRoot:GetTrans("btn_kick_out")

	emojiTrans_1 = {}
	emojiTrans_2 = {}
	for i=1,5 do
		local btnEmoji = m_luaWindowRoot:GetTrans("btn_emoji_"..i)
		local emoji_1 = m_luaWindowRoot:GetTrans(btnEmoji,"emoji_1")
		local emoji_2 = m_luaWindowRoot:GetTrans(btnEmoji,"emoji_2")
		table.insert(emojiTrans_1,emoji_1)
		table.insert(emojiTrans_2,emoji_2)
	end

	playLogic = PlayGameSys.GetPlayLogic()

	_s.Register()
end

function userInfo_ui_window.InitWindowDetail()
	local m_seatInfo = m_playInfo.seatInfo
	m_isMySelf = m_seatInfo.userId == DataManager.GetUserID()
	WindowUtil.LoadHeadIcon(m_luaWindowRoot,m_headIconTrans, m_seatInfo.headUrl, m_seatInfo.sex, false,RessStorgeType.RST_Never)
	m_luaWindowRoot:SetLabel(label_name,utf8sub(m_seatInfo.nickName,1,10) )
	m_luaWindowRoot:SetLabel(label_id,ToString(m_seatInfo.userId))
	m_luaWindowRoot:SetLabel(label_ip,m_seatInfo.ip)
	if m_seatInfo.loginAddress and m_seatInfo.loginAddress ~= "" then
		m_luaWindowRoot:SetLabel(label_position,m_seatInfo.loginAddress)
	elseif m_seatInfo.LoginAddress and m_seatInfo.LoginAddress ~= "" then
		m_luaWindowRoot:SetLabel(label_position,m_seatInfo.LoginAddress)
	else
		m_luaWindowRoot:SetLabel(label_position,"未知")
	end

	if m_seatInfo.sex == true or m_seatInfo.sex == 1 then
		m_luaWindowRoot:SetSprite(sprite_sex,"userino_icon_male")
	else
		m_luaWindowRoot:SetSprite(sprite_sex,"userino_icon_female")
	end
	_s.SetKickOutBtnShow()

	--设置不同表情
	for i=1,5 do
		m_luaWindowRoot:SetActive(emojiTrans_1[i],m_isMySelf)
		m_luaWindowRoot:SetActive(emojiTrans_2[i],not m_isMySelf)
	end
end

function userInfo_ui_window.SetKickOutBtnShow()
	local logicType = PlayGameSys.GetPlayLogicType()
	if logicType == PlayLogicTypeEnum.WSK_Record or 
	   logicType == PlayLogicTypeEnum.MJ_Record then
		m_luaWindowRoot:SetActive(btn_kick_out, false)
		return
	end

	if playLogic then
		local btnState = false
		if m_playInfo.seatInfo.userId == DataManager.GetUserID() then
			btnState = false
		else
			btnState = playLogic.roomObj:CheckSelfIsRoomOwner()
		end
		m_luaWindowRoot:SetActive(btn_kick_out, btnState)
	else
		m_luaWindowRoot:SetActive(btn_kick_out, false)
	end
end

function userInfo_ui_window.SendAskLBS()
	if playLogic then
		playLogic:SendAskLBS()
	else
		WindowUtil.LuaShowTips("不能发送")
	end
end

function userInfo_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnClose" or click_name == "bg_window" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false,0)
	elseif click_name == "btn_distance" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.SendAskLBS()
	elseif click_name == "btn_kick_out" then
		_s.OnKickOutBtnClick()
	elseif string.find(click_name, "btn_emoji_") then
		local index = tonumber(string.sub(click_name, string.len("btn_emoji_") + 1))
		
		local emojiInfo = {}
		emojiInfo.fromSeatPos = SeatPosEnum.South
		emojiInfo.toSeatPos = m_playInfo.seatPos
		
		if SeatPosEnum.South == m_playInfo.seatPos then
			emojiInfo.emojiID = mySelfEmojiIndex[index]
		else
			emojiInfo.emojiID = index
		end
		NimChatSys.SendImojMessage(emojiInfo.emojiID,m_playInfo.seatInfo.userId)
		-- _s.InitWindow(false,0)
		--LuaEvent.AddEventNow(EEventType.PlayEmoji,emojiInfo)
	end
end

function userInfo_ui_window.OnKickOutBtnClick()
	if playLogic then
		if not playLogic.gameStarted then
			WindowUtil.ShowErrorWindow(2, "\n确认请出房间？", nil, 
			function()
				-- 确认回调
				playLogic:SendASKKickOut(m_playInfo.seatInfo.userId, function() 
					_s.InitWindow(false,0)
				end)
			end, nil, "")
		else
			WindowUtil.LuaShowTips("牌局已开始，不能请出房间")
		end
		
	else
		WindowUtil.LuaShowTips("不能请出房间")
	end
end

function userInfo_ui_window.Register()
	--LuaEvent.AddHandle(EEventType.LBSNotify,_s.LBSNotifyRsp)
end

function userInfo_ui_window.UnRegister()
    --LuaEvent.RemoveHandle(EEventType.LBSNotify,_s.LBSNotifyRsp)
end

function userInfo_ui_window.OnDestroy()
   m_luaWindowRoot = nil
	emojiTrans_1 = {}
	emojiTrans_2 = {}
	playLogic = nil

    _s.UnRegister()
end