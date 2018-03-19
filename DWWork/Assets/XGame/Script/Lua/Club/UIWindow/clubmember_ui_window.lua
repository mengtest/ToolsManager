--------------------------------------------------------------------------------
--   File	  : clubmember_ui_window.lua
--   author	: zx
--   function  : 俱乐部玩法管理
--   date	  : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "LuaWindow.Module.UINetTipsCtrl"

clubmember_ui_window = {

}

local _s = clubmember_ui_window

local m_luaWindowRoot
local m_state
--窗口是否打开
local m_open = false
local m_clubMemberList
local m_panelScrollView
local m_UIInputComponent
local m_currPage = 0
local m_inmsg = nil
local m_netTipsCtrl = nil

function clubmember_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function clubmember_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, false, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		_s.InitWindowDetail()
	end
end

-- 显示玩法信息
function clubmember_ui_window.InitClubMemberItem(trans, index)
	index = index + 1

	-- 获取数据
	local data = m_clubMemberList[index]
	if nil == data then
		return
	end

	trans.name = "member_" .. index

	-- 设置头像
	local url = (nil == data or nil == data.wx_headimgurl) and "" or data.wx_headimgurl
	local sex = (nil == data or nil == data.wx_sex) and DataManager.GetUserGender() or data.wx_sex
	WindowUtil.LoadHeadIcon(m_luaWindowRoot, m_luaWindowRoot:GetTrans(trans, "head"), ezfunLuaTool.GetSmallWeiXinIconUrl(url, 64), sex, false, RessStorgeType.RST_Never)
	-- 设置名字
	local name = (nil == data or nil == data.wx_nickname) and "" or data.wx_nickname
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "name"), utf8sub(name, 1, 10))
	-- 设置性别
	if sex == 1 then
		m_luaWindowRoot:SetSprite(m_luaWindowRoot:GetTrans(trans,"sex"), "userino_icon_male")
	else
		m_luaWindowRoot:SetSprite(m_luaWindowRoot:GetTrans(trans,"sex"), "userino_icon_female")
	end

	-- 设置id
	local id = (nil == data or nil == data.uid) and "" or data.uid
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "id"), id)

	-- 设置局数
	local count = (nil == data or nil == data.game_number) and "" or data.game_number
	m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "count"), count)

	-- 设置是否部长
	local ownerID = (nil == data or nil == data.owner_uid) and 0 or data.owner_uid
	if ownerID == id then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "chief"), true)
	else
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "chief"), false)
	end
end

function clubmember_ui_window.InitWindowDetail()
	m_currPage = 0
	m_clubMemberList = {}
	m_inmsg = nil

	if m_panelScrollView == nil then
		local m_ScrollViewRoot = m_luaWindowRoot:GetTrans("scrollView")
		local itemPre = m_luaWindowRoot:GetTrans("memberItem").gameObject
		m_panelScrollView = EZfunLimitScrollView.GetOrAddLimitScr(m_ScrollViewRoot)
		m_panelScrollView:InitForLua(m_ScrollViewRoot, itemPre, UnityEngine.Vector2.New(310, 182), UnityEngine.Vector2.New(3, 4), LimitScrollViewDirection.SVD_Vertical, false)
		m_panelScrollView:SetInitItemCallLua("clubmember_ui_window.InitClubMemberItem")
	end

	m_panelScrollView:InitItemCount(#m_clubMemberList, true)

	-- 设置基础信息
	m_UIInputComponent = m_luaWindowRoot:GetTrans("msgInput"):GetComponent("UIInput")
	m_UIInputComponent.value = ""


	if not m_netTipsCtrl then
		m_netTipsCtrl = UINetTipsCtrl.New()
		m_netTipsCtrl:Init(
			m_luaWindowRoot,
			m_luaWindowRoot:GetTrans("animation_ui_root"),
			_s.PreRequestData
		)
	end
	m_netTipsCtrl:StartWork()
end

function clubmember_ui_window.InitData(params)
	if not params then
		return
	end

	for i,v in ipairs(params.items) do
		table.insert(m_clubMemberList, v)
	end
end

-- 保存数据
function clubmember_ui_window._handSearch()
	m_inmsg = m_UIInputComponent.value

	if nil == m_inmsg or "" == m_inmsg then
		m_inmsg = nil
	end

	_s.RefreshClubList()
end

-- 点击事件处理
function clubmember_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name

	if click_name == "btnSearch" then
		_s._handSearch()

	elseif string.find(click_name, "member_") then
		local index = tonumber(string.sub(click_name, string.len("member_") + 1))
		if nil ~= m_clubMemberList[index] then
			-- 找到玩家信息
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_state, "Club.UIWindow.clubmemberdetail_ui_window", false , m_clubMemberList[index])
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

function clubmember_ui_window.PreRequestData()
	--请求数据回调里调用下两句
	WebNetHelper.RequestClubMemList(m_state, 1, m_currPage + 1, 20, m_inmsg,
		function(body, head)

			if m_open then
				local recv_data
				if not body or not body.data or not body.data.list then
					return
				else
					recv_data = body.data.list
				end

				m_currPage = m_currPage + 1
				_s.InitData(recv_data)
				m_panelScrollView:InitItemCount(#m_clubMemberList, false)

				-- 不是最后一页，才注册预加载，最后一页就没必要再请求数据了
				if recv_data.page_index and recv_data.page_count and recv_data.page_index < recv_data.page_count then
					m_panelScrollView:InitPreLoadDataForLua(10, "clubmember_ui_window.PreRequestData")
				end
				m_netTipsCtrl:StopWork(true)
			end
		end,
		function(body, head)
			if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
				WindowUtil.LuaShowTips(body.errmsg)
			end
			m_netTipsCtrl:StopWork(false)
		end
		)
end

function clubmember_ui_window.RefreshClubList(...)
	m_currPage = 0
	m_clubMemberList = {}

	_s.PreRequestData()
end

function clubmember_ui_window.ClubQuieOrDisBand(eventID, clubID)
	if m_state == clubID then
		_s.InitWindow(false, 0)
	end
end

function clubmember_ui_window.RegisterEventHandle()
	LuaEvent.AddHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.AddHandle(EEventType.UI_RefreshClubList, _s.RefreshClubList)
	LuaEvent.AddHandle(EEventType.UserNetReconnectSuccess, _s.RefreshClubList)
end

function clubmember_ui_window.UnRegisterEventHandle()
	LuaEvent.RemoveHandle(EEventType.ClubQuieOrDisBand, _s.ClubQuieOrDisBand)
	LuaEvent.RemoveHandle(EEventType.UI_RefreshClubList, _s.RefreshClubList)
	LuaEvent.RemoveHandle(EEventType.UserNetReconnectSuccess, _s.RefreshClubList)
end

function clubmember_ui_window.CreateWindow()
	_s.RegisterEventHandle()
end

function clubmember_ui_window.OnDestroy()
	_s.UnRegisterEventHandle()

	if m_netTipsCtrl then
		m_netTipsCtrl:Destroy()
		m_netTipsCtrl = nil
	end

	m_clubMemberList = {}
	m_panelScrollView = nil
	m_UIInputComponent = nil
	m_luaWindowRoot = nil
end
