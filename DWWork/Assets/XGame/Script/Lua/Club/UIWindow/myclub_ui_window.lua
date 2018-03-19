--------------------------------------------------------------------------------
--   File	  : myclub_ui_window.lua
--   author	: zx
--   function  : 我的俱乐部
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Club.ClubSys.ClubUtil"
require "LuaWindow.Module.UINetTipsCtrl"

myclub_ui_window = {

}

local _s = myclub_ui_window

local m_luaWindowRoot
local m_state
local m_clubList
local m_panelScrollView
local m_open = false
local m_netTipsCtrl = nil

function myclub_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function myclub_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail()
	else
		LuaEvent.AddEventNow(EEventType.RefreshClubHallWindow, 0)
	end
end

-- 显示玩法信息
function myclub_ui_window.InitClubItem(trans, index)
	index = index + 1

	trans.name = "club_" .. index

	-- 获取数据
	local data = m_clubList[index]

	-- 设置俱乐部Icon
	ClubUtil.SetClubIcon(m_luaWindowRoot, m_luaWindowRoot:GetTrans(trans, "icon"), data.g_img)

	-- 俱乐部名称
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "club"), utf8sub(data.name, 1, 10))

	-- 俱乐部id
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "id"), data.id)

	-- 部长名字
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "chief"), string.format("部长 :%s", utf8sub(data.owner_nickname,1,10)))

	-- 人数
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "member"), tostring(data.user_number))

	-- 房卡
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "card"), tostring(data.card_number))

	-- 判断是否为部长
	if ClubUtil.IsClubChief(data.owner_uid) then
		m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(trans, "chiefRoot"), "isChief", true)
	else
		m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(trans, "chiefRoot"), "isChief", false)
	end
end

function myclub_ui_window.InitWindowDetail()
	m_clubList = {}

	if m_panelScrollView == nil then
		local m_ScrollViewRoot = m_luaWindowRoot:GetTrans("ScrollView")
		local itemPre = m_luaWindowRoot:GetTrans("clubItem").gameObject
		m_panelScrollView = EZfunLimitScrollView.GetOrAddLimitScr(m_ScrollViewRoot)
		m_panelScrollView:InitForLua(m_ScrollViewRoot, itemPre, UnityEngine.Vector2.New(1104, 140), UnityEngine.Vector2.New(1, 5), LimitScrollViewDirection.SVD_Vertical, false)
		m_panelScrollView:SetInitItemCallLua("myclub_ui_window.InitClubItem")
	end

	-- 请求俱乐部列表
	m_panelScrollView:InitItemCount(#m_clubList, true)

	_s.GetClubList()
end

-- 点击事件处理
function myclub_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnMore" or click_name == "btnMgr" then
		-- ???优化层级？？？获取item根节点？？？
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		local index = tonumber(string.sub(gb.transform.parent.parent.parent.parent.name, string.len("club_") + 1))
		if nil ~= m_clubList[index] and m_clubList[index].id then
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_clubList[index].id, "Club.UIWindow.clubmanager_ui_window", false , nil)
		end

	elseif click_name == "btnAdd" then
		-- ???优化层级？？？获取item根节点？？？
		local index = tonumber(string.sub(gb.transform.parent.parent.parent.parent.name, string.len("club_") + 1))
		if nil ~= m_clubList[index] then
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "Club.UIWindow.clubcharge_ui_window", false , m_clubList[index])
		end

	elseif click_name == "btnStatistics" then
		local index = tonumber(string.sub(gb.transform.parent.parent.parent.parent.name, string.len("club_") + 1))
		if nil ~= m_clubList[index] and m_clubList[index].id then
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.statistics.club_statistics_ui_window", false , m_clubList[index].id)
		end
	elseif click_name == "btnClose" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
		_s.InitWindow(false, 0)
	else
		if m_netTipsCtrl then
			m_netTipsCtrl:HandleWidgetClick(gb)
		end
	end
end

-- 请求裂变数据
function myclub_ui_window.GetClubList(...)
	if not m_netTipsCtrl then
		m_netTipsCtrl = UINetTipsCtrl.New()
		-- 重新请求数据
		m_netTipsCtrl:Init(
			m_luaWindowRoot,
			m_luaWindowRoot:GetTrans("animation_ui_root"),
			function()
				ClubSys.RequestClubList(0,
					function (body, head)
						if not m_open then
							return
						end

						m_clubList = {}
						if nil ~= body.data and nil ~= body.data.list then
							m_clubList = body.data.list
						end
						m_panelScrollView:InitItemCount(#m_clubList, true)
						m_netTipsCtrl:StopWork(true)
						if 0 == #m_clubList then
							-- 列表数据为空，关闭界面
							_s.InitWindow(false, 0)
						end
					end,
					function (body, head)
						if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
							WindowUtil.LuaShowTips(body.errmsg)
						end
						m_netTipsCtrl:StopWork(false)
					end)
			end)
	end

	m_netTipsCtrl:StartWork()
end

-- 解散或退出，关闭界面
function myclub_ui_window.ClubQuieOrDisBand(eventID, clubID)
	_s.InitWindow(false, 0)
end

function myclub_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.AddHandle(EEventType.UI_RefreshClubList, _s.GetClubList)
	LuaEvent.AddHandle(EEventType.UserNetReconnectSuccess, _s.GetClubList)
end

function myclub_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.RemoveHandle(EEventType.UI_RefreshClubList, _s.GetClubList)
	LuaEvent.RemoveHandle(EEventType.UserNetReconnectSuccess, _s.GetClubList)
end

function myclub_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function myclub_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	if m_netTipsCtrl then
		m_netTipsCtrl:Destroy()
		m_netTipsCtrl = nil
	end

	m_panelScrollView = nil
	m_luaWindowRoot = nil
end
