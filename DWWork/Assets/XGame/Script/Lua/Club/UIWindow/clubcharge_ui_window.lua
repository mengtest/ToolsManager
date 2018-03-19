--------------------------------------------------------------------------------
--   File	  : clubcharge_ui_window.lua
--   author	: zx
--   function  : 俱乐部玩法管理
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

clubcharge_ui_window = {

}

local _s = clubcharge_ui_window

local m_luaWindowRoot
local m_state

local m_clubData = nil

local ChargeCard =
{
	[1] = 100,
	[2] = 300,
	[3] = 500,
}

function clubcharge_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function clubcharge_ui_window.InitWindow(open, state, params)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	if open then
		m_state = state
		_s.InitWindowDetail(params)
	end
end

function clubcharge_ui_window.InitWindowDetail(params)
	m_clubData = params
	if nil == m_clubData then
		_s.InitWindow(false, 0, nil)
		return
	end

	-- 俱乐部房卡
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("clubCard"), m_clubData.card_number)

	-- 大厅房卡
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("hallCard"), DataManager.GetRoomCardNum())

	for i=1,#ChargeCard do
		m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("value_label_" .. i), tostring(ChargeCard[i]))
	end

	_s.SetHighLight(0)
end

-- 点击事件处理
function clubcharge_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if string.find(click_name, "value_") then
		local index = tonumber(string.sub(click_name, string.len("value_") + 1))
		if index then
			_s.SetHighLight(index)
		end

	elseif click_name == "btnClose" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	end
end

-- 选中充值
function clubcharge_ui_window.SetHighLight(index)
	local root = m_luaWindowRoot:GetTrans("highlights")

	if not root then
		return
	end

	m_luaWindowRoot:ShowChild(root, tostring(index), true)

	-- 越界
	if index <= 0 or index > #ChargeCard then
		return
	end

	local chargeCard = ChargeCard[index]
	if chargeCard > DataManager.GetRoomCardNum() then
		WindowUtil.LuaShowTips("大厅房卡不足，请先充值！")
		return
	end

	if nil == m_clubData then
		return
	end

	WindowUtil.ShowErrorWindow(4, string.format("\n向俱乐部 \"%s\" 充值%s张", m_clubData.name, chargeCard), nil, function()
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)

		WebNetHelper.RequestRechargeClub(m_clubData.id, chargeCard, function (body, head)
			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			if 0 == body.errcode then
				DataManager.SetRoomCardNum(body.data.hall_card_number)
				LuaEvent.AddEventNow(EEventType.RefreshUserInfo)
				LuaEvent.AddEventNow(EEventType.UI_RefreshClubList)
				_s.InitWindow(false, 0)
			else
				WindowUtil.LuaShowTips(body.errmsg)
			end
		end,
		function (body, head)
			LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
				WindowUtil.LuaShowTips(body.errmsg)
			else
				WindowUtil.LuaShowTips("充值请求超时,请稍候重试")
			end
		end)

	end, nil, "")
end

-- 解散或退出，关闭界面
function clubcharge_ui_window.ClubQuieOrDisBand(eventID, clubID)
	if nil == m_clubData or nil == clubID or m_clubData.id == clubID then
		_s.InitWindow(false, 0)
	end
end

function clubcharge_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.AddHandle(EEventType.UserNetReconnectSuccess, _s.ClubQuieOrDisBand)
end

function clubcharge_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.RemoveHandle(EEventType.UserNetReconnectSuccess, _s.ClubQuieOrDisBand)
end

function clubcharge_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function clubcharge_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	m_luaWindowRoot = nil
end
