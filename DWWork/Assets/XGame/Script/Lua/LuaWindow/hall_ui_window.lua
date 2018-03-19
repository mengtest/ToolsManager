--------------------------------------------------------------------------------
--   File      : hall_ui_window.lua
--   author    : jianing
--   function  : 大厅
--   date      : 2017-10-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Tools.LayoutGroup"

hall_ui_window = {}
local _s = hall_ui_window

local m_luaWindowRoot
local m_state
local m_open = false

local txt_userId
local txt_roomNum
local txt_horn

local img_headIcon

-- [[去除广告列表相关]]
-- local scrollContent
-- local icon_1
-- local icon_2
-- local icon_3
-- local nowIndex2Url = {}
-- local adUrlLen = 0
-- local scrollIcos = {}

local itemWide = 192
local isPlaying = false
local isPlaying_SysHorn = false
local sysHornLabelIndex = 0
local nowIndex = 0

local tickTime = 0

local topCfg = {
	btn_help = {isShow = true, sortOrder = 1},
	btn_mail = {isShow = true, sortOrder = 3},
	btn_agent = {isShow = not DataManager.isPrePublish, sortOrder = 2}
}

local bottomCfg = {
	btn_nearby = {isShow = true, sortOrder = 5},
	btn_activity = {isShow = not DataManager.isPrePublish, sortOrder = 4},
	btn_record = {isShow = true, sortOrder = 3},
	btn_share = {isShow = not DataManager.isPrePublish, sortOrder = 2},
	btn_set = {isShow = true, sortOrder = 1}
	-- btn_store = { isShow = true, sortOrder = 5},
}

function hall_ui_window.Init(LuaWindowRoot)
	m_luaWindowRoot = LuaWindowRoot
end

function hall_ui_window.InitWindow(open, state)
	m_luaWindowRoot:InitCamera(open, false, false, true, -1)
	m_luaWindowRoot:BaseIniwWindow(open, state)
	m_open = open
	if open then
		m_state = state
		-- [[去除广告列表相关]]
		-- _s.ResetScroll()
		-- UpdateSecond:Add(_s.UpdateSecond)
		_s.InitWindowDetail()
		UpdateSecond:Add(_s.UpdateSysHorn)
		_s.UpdateSysHorn()
	else
		-- UpdateSecond:Remove(_s.UpdateSecond)
		isPlaying_SysHorn = false
		UpdateSecond:Remove(_s.UpdateSysHorn)
	end
end

function hall_ui_window.CreateWindow()
	img_headIcon = m_luaWindowRoot:GetTrans("img_headIcon")
	txt_userId = m_luaWindowRoot:GetTrans("txt_userId")
	txt_horn = m_luaWindowRoot:GetTrans("txt_horn")
	txt_roomNum = m_luaWindowRoot:GetTrans("txt_roomNum")
	-- [[去除广告列表相关]]
	-- scrollContent = m_luaWindowRoot:GetTrans("scrollContent")
	-- icon_1 = m_luaWindowRoot:GetTrans("icon_1")
	-- icon_2 = m_luaWindowRoot:GetTrans("icon_2")
	-- icon_3 = m_luaWindowRoot:GetTrans("icon_3")
	if DataManager.isPrePublish then
		m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("btn_add"), false)
	end
	_s.Register()
end

-- function hall_ui_window.ResetScroll()
--  isPlaying = false
--  nowIndex = 0
--  scrollIcos = {}
--  local childCount = scrollContent.childCount
--  for i=1,childCount do
--      table.insert(scrollIcos,scrollContent:GetChild(i-1))
--  end
-- end

function hall_ui_window.InitWindowDetail()
	m_luaWindowRoot:SetLabel(txt_userId, "ID:" .. DataManager.GetUserID())
	m_luaWindowRoot:SetLabel(txt_roomNum, DataManager.GetRoomCardNum())

	WindowUtil.LoadHeadIcon(m_luaWindowRoot, img_headIcon, DataManager.GetUserHeadUrl(), DataManager.GetUserGender(), false, RessStorgeType.RST_Always)

	local vector3 = UnityEngine.Vector3
	local cellSize = vector3.New(0, 0, 0)
	local offset = vector3.New(-29.7 - 105.23, 0, 0)

	local topGroup = LayoutGroup.New()
	topGroup:Init(vector3.New(535, 310.7, 0), cellSize, offset)
	topGroup:AddCellsByCfg(m_luaWindowRoot, topCfg)

	local bottomGroup = LayoutGroup.New()
	bottomGroup:Init(vector3.New(488, -284.5, 0), cellSize, offset)
	bottomGroup:AddCellsByCfg(m_luaWindowRoot, bottomCfg)

	_s.RefreshDailyShare(nil, HallSys.CheckRedDailyShare(), nil)
	_s.RefreshHallMailBtnRedPoint()
end

function hall_ui_window.InitSysHorn()
	local list = DataManager.GetSysHornData()
	if list and #list > 0 then
		m_luaWindowRoot:SetLabel(txt_horn, list[1].content)
	end
end

function hall_ui_window.InitSysAd()
	-- local list = DataManager.GetSysAdData()
	-- if not list then return end
	-- adUrlLen = #list
	-- if list and adUrlLen >0 then
	--  local url
	--  local _next = list[2] and 2 or adUrlLen
	--  url = list[adUrlLen].img_url
	--  m_luaWindowRoot:LoadUISprite(icon_1,"anti_gambling")
	--  m_luaWindowRoot:LoadImag(icon_1, url, "anti_gambling",false,RessStorgeType.RST_Never)
	--  url = list[1].img_url
	--  m_luaWindowRoot:LoadUISprite(icon_2,"anti_gambling")
	--  m_luaWindowRoot:LoadImag(icon_2, url, "anti_gambling",false,RessStorgeType.RST_Never)
	--  url = list[_next].img_url
	--  m_luaWindowRoot:LoadUISprite(icon_3,"anti_gambling")
	--  m_luaWindowRoot:LoadImag(icon_3, url, "anti_gambling",false,RessStorgeType.RST_Never)
	--  for i=1, adUrlLen do
	--      nowIndex2Url[i] = list[i].img_url
	--  end
	-- end
end

-- [[去除广告列表相关]]
--播放列表滑动动画
-- local function PlayTween(isAdd)
--  if isPlaying or scrollIcos == nil or #scrollIcos == 0 then
--      return
--  end
--  local posX = itemWide*nowIndex
--  if isAdd then
--      nowIndex = nowIndex + 1
--  else
--      nowIndex = nowIndex - 1
--  end
--  toPosX = itemWide*nowIndex

--  m_luaWindowRoot:PlayTweenPos(scrollContent,0.8,true,posX,toPosX)
--  isPlaying = true

--  WrapSys.AddTimerEVentByLeftTime(0.8, function ()
--      if m_open then
--          isPlaying = false
--          if isAdd then
--              scrollIcos[#scrollIcos].localPosition = UnityEngine.Vector3.New(scrollIcos[1].localPosition.x - itemWide,0,0)

--              local temp = scrollIcos[#scrollIcos]

--              table.remove(scrollIcos,#scrollIcos)
--              table.insert(scrollIcos,1,temp)

--              local fmt_index = -(nowIndex-2)
--              local target_index = fmt_index%adUrlLen
--              if target_index == 0 then
--                  target_index = adUrlLen
--              end
--              local url = nowIndex2Url[target_index]
--              m_luaWindowRoot:LoadImag(temp, url, "", false, RessStorgeType.RST_Never)

--          else
--              scrollIcos[1].localPosition = UnityEngine.Vector3.New(scrollIcos[#scrollIcos].localPosition.x + itemWide,0,0)

--              local temp = scrollIcos[1]
--              table.remove(scrollIcos,1)
--              table.insert(scrollIcos,temp)

--              local fmt_index = -(nowIndex+2)
--              local target_index = fmt_index%adUrlLen
--              if target_index == 0 then
--                  target_index = adUrlLen
--              end
--              local url = nowIndex2Url[target_index]
--              m_luaWindowRoot:LoadImag(temp, url, "", false, RessStorgeType.RST_Never)
--          end

--          -- print("nowIndex "..nowIndex)
--      end
--  end)

--  tickTime = 0
-- end

-- function hall_ui_window.UpdateSecond()
--  tickTime = tickTime + 1
--  if tickTime > 5 then
--      PlayTween(false)
--  end
-- end

function hall_ui_window.UpdateSysHorn()
	local function whenPlayEnd()
		if m_open then
			isPlaying_SysHorn = false
		end
	end

	local function doPlay()
		local lable_width = m_luaWindowRoot:GetLableSize(txt_horn).x
		local mask_width = 580
		local start_x = lable_width
		local end_x = -mask_width - lable_width - 50
		local time = -end_x / 100
		m_luaWindowRoot:PlayTweenPos(txt_horn, time, true, start_x, end_x)

		isPlaying_SysHorn = true
		TimerTaskSys.AddTimerEventByLeftTime(whenPlayEnd, time)
	end

	if not isPlaying_SysHorn then
		if m_open then
			local list = DataManager.GetSysHornData()
			if not list then
				return
			end
			sysHornLabelIndex = sysHornLabelIndex + 1
			if sysHornLabelIndex > #list then
				sysHornLabelIndex = 1
			end
			m_luaWindowRoot:SetLabel(txt_horn, list[sysHornLabelIndex].content)
			doPlay()
		end
	end
end

function hall_ui_window.HandleWidgetClick(gb)
	local click_name = gb.name
	if click_name ~= "btn_copy" then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
	end
	if click_name == "btn_activity" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "activity_ui_window", false, nil)
	elseif click_name == "btn_shop" then
		if WrapSys.IsIOS then
			_s.TryOpenStoreWindow()
		else
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "cardcharge_ui_window", false, nil)
		end
	elseif click_name == "btn_joinRoom" then
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "joinRoom_ui_window", false, nil)
	elseif click_name == "btn_club" then
		ClubSys.OpenClub()
	elseif click_name == "btn_createRoom_cr" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, -1, "createRoom_ui_window", false, nil)
	elseif click_name == "btn_createRoom_yh" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, -3, "createRoom_ui_window", false, nil)
	elseif click_name == "btn_createRoom_la" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, -2, "createRoom_ui_window", false, nil)
	elseif click_name == "btn_createRoom_32" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 7, "createRoom_ui_window", false, nil)
	elseif click_name == "btn_createRoom_ddz" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 8, "createRoom_ui_window", false, nil)
	elseif click_name == "btn_set" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "setting_ui_window", false, nil)
	elseif click_name == "btn_help" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, -1, "help_ui_window", false, nil)
	elseif click_name == "btn_mail" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Modules.Mail.mail_ui_window", false , nil)
		-- _s.RefreshBtnMailRedPoint(false)
	elseif click_name == "btn_agent" then
		require "LuaWindow.bing_agent_ui_window"
		bing_agent_ui_window.TryOpenWindow()
	elseif click_name == "btn_record" then
		-- require "LuaWindow.record_ui_window"
		-- record_ui_window.TryOpenWindow()
		WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_Window, true, 1, "record_ui_window")
	elseif click_name == "btn_nearby" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "nearby_ui_window", false, nil)
		-- _s.TryOpenNearbyWindow()
		--_s.Test()
	elseif click_name == "btn_share" or click_name == "btn_shareRoom" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "hallshare_ui_window", false, nil)
	elseif click_name == "btn_store" then
		_s.TryOpenStoreWindow()
	elseif click_name == "btn_add" then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "cardcharge_ui_window", false, nil)
	elseif click_name == "btn_copy" then
		local idStr = DataManager.GetUserID()
		if idStr == nil or idStr == "" then
			WindowUtil.LuaShowTips("请重新复制")
		else
			WrapSys.PlatInterface_CopyStr(DataManager.GetUserID())
			WindowUtil.LuaShowTips("您的游戏ID已复制")
		end
	end
end

--打开附近的人 窗口
function hall_ui_window.TryOpenNearbyWindow()
	WebNetHelper.GetNearByPlayers(
		1,
		20,
		function(body, head)
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "nearby_ui_window", false, body.data.list)
		end,
		function(body, head)
			--          logError("recv error " .. ToString(body) .. "\n" .. ToString(head))
		end
	)
end

function hall_ui_window.TryOpenStoreWindow()
	--WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "store_ui_window", false , nil)
	CIAPSys.RequestShopItemList(
		function()
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "store_ui_window", false, nil)
		end
	)
end

function hall_ui_window.HandleWidgetDrag(deltaX, deltaY)
	if deltaX > 0 then
		PlayTween(true)
	elseif deltaX < 0 then
		PlayTween(false)
	end
end

function hall_ui_window.RefreshUserInfo()
	if m_luaWindowRoot ~= nil and m_open then
		_s.InitWindowDetail()
	end
end

function hall_ui_window.RefreshNoticeSysHorn()
	if m_luaWindowRoot ~= nil and m_open then
		_s.InitSysHorn()
	end
end

function hall_ui_window.RefreshNoticeSysAd()
	if m_luaWindowRoot ~= nil and m_open then
	-- _s.InitSysAd()
	end
end

-- 刷新每日分享
function hall_ui_window.RefreshDailyShare(_wtf, isShow, _wangtefa)
	local btn_share = m_luaWindowRoot:GetTrans("btn_share")
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(btn_share, "red_point"), isShow)
end

-- 刷新邮件红点
function hall_ui_window.RefreshHallMailBtnRedPoint()
	DwDebug.Log("----- 刷新邮件红点 ",DataManager.GetIsShowMailRedPoint())
	local btn_mail = m_luaWindowRoot:GetTrans("btn_mail")
	m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(btn_mail, "red_point"), DataManager.GetIsShowMailRedPoint())
end

function hall_ui_window.Register()
	LuaEvent.AddHandle(EEventType.RefreshUserInfo, _s.RefreshUserInfo)
	LuaEvent.AddHandle(EEventType.RefreshNoticeSysHorn, _s.RefreshNoticeSysHorn)
	-- LuaEvent.AddHandle(EEventType.RefreshNoticeSysAd,_s.RefreshNoticeSysAd)
	LuaEvent.AddHandle(EEventType.RefreshDailyShare, _s.RefreshDailyShare)
	LuaEvent.AddHandle(EEventType.RefreshHallMailBtnRedPoint, _s.RefreshHallMailBtnRedPoint)

end

function hall_ui_window.UnRegister()
	m_luaWindowRoot = nil
	LuaEvent.RemoveHandle(EEventType.RefreshUserInfo, _s.RefreshUserInfo)
	LuaEvent.RemoveHandle(EEventType.RefreshNoticeSysHorn, _s.RefreshNoticeSysHorn)
	-- LuaEvent.RemoveHandle(EEventType.RefreshNoticeSysAd,_s.RefreshNoticeSysAd)
	LuaEvent.RemoveHandle(EEventType.RefreshDailyShare, _s.RefreshDailyShare)
	LuaEvent.RemoveHandle(EEventType.RefreshHallMailBtnRedPoint, _s.RefreshHallMailBtnRedPoint)
end

function hall_ui_window.OnDestroy()
	_s.UnRegister()
	scrollIcos = {}
end

function hall_ui_window.Test()
	require "Logic.SystemLogic.WSK.WSKNormalPlayCardLogic"
	local srcCardIDs = {4,4,12,12,12}
	local nextCardIDs = {8,8,7,7,7}
	local handCardCount = 20
	local srcCards = {}
	local nextCards = {}
	local cardLogicContainer = CRWSKCardPlayLogicContainer.New()
	local cardItem

	for k,v in pairs(srcCardIDs) do
		cardItem = CCard.New()
		cardItem:Init(v,k)
		table.insert(srcCards,cardItem)
	end

	for k,v in pairs(nextCardIDs) do
		cardItem = CCard.New()
		cardItem:Init(v,k)
		table.insert(nextCards,cardItem)
	end

	if cardLogicContainer:CompareCard(srcCards,nextCards,handCardCount) then
		DwDebug.LogError("CompareCard true")
	end

	if cardLogicContainer:CheckCanOutCard(srcCards,handCardCount) then
		DwDebug.LogError("CompareCard true")
	end

	local introduceCount = cardLogicContainer:CreateAIResult(nextCards,srcCards)
	DwDebug.LogError("introduceCount = "..introduceCount)
	if introduceCount <= 0 then
		WindowUtil.LuaShowTips("当前找不到可以大过上家的牌")
	end

end