--------------------------------------------------------------------------------
--   File      : roomDetail_ui_window.lua
--   author    : jianing
--   function  : 房间流水信息
--   date      : 2017-10-27
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
roomDetail_ui_window = {}

local _s = roomDetail_ui_window

local m_luaWindowRoot
local m_state

local tab_paiju, tab_liushui, tab_fanxing  -- tab
local liushui, paiju, fanxing  -- tab_panel
local liushui_scrollview, fanxing_scrollview, paiju_scrollview  -- list
local liushui_item, fanxing_item, paiju_item  -- item

local txt_roomId, txt_des

local cur_tab

local ico_headTrans = {} --头像列表

local data_liushui_scrollview_items = {} --历史记录列表 对应EventUseHistoryScore.items
local data_playerList = {} --玩家id列表

local m_FanXingCfg

function roomDetail_ui_window.Init(LuaWindowRoot)
    m_luaWindowRoot = LuaWindowRoot
    m_FanXingCfg = require "Config.fanXingCfg"
end

function roomDetail_ui_window.InitWindow(open, state)
    m_luaWindowRoot:InitCamera(open, false, false, true, -1)
    m_luaWindowRoot:BaseIniwWindow(open, state)

    if open then
        m_state = state
        _s.InitWindowDetail()
        _s.Register()
    else
        cur_tab = nil
        _s.UnRegister()
    end
end

function roomDetail_ui_window.CreateWindow()
    tab_paiju = m_luaWindowRoot:GetTrans("tab_paiju")
    tab_liushui = m_luaWindowRoot:GetTrans("tab_liushui")
    tab_fanxing = m_luaWindowRoot:GetTrans("tab_fanxing")

    liushui = m_luaWindowRoot:GetTrans("liushui")
    paiju = m_luaWindowRoot:GetTrans("paiju")
    fanxing = m_luaWindowRoot:GetTrans("fanxing")

    txt_roomId = m_luaWindowRoot:GetTrans(paiju, "txt_roomId")
    txt_des = m_luaWindowRoot:GetTrans(paiju, "txt_des")

    liushui_item = m_luaWindowRoot:GetTrans("liushui_item")
    m_luaWindowRoot:SetActive(liushui_item, false)

    fanxing_item = m_luaWindowRoot:GetTrans("fanxing_item")
    m_luaWindowRoot:SetActive(fanxing_item, false)

    paiju_item = m_luaWindowRoot:GetTrans("paiju_item")
    m_luaWindowRoot:SetActive(paiju_item, false)

    local headTrans
    for i = 1, 4 do
        headTrans = m_luaWindowRoot:GetTrans("ico_head_" .. i)
        m_luaWindowRoot:SetActive(headTrans, false)
        table.insert(ico_headTrans, headTrans)
    end
end

function roomDetail_ui_window.InitWindowDetail()
    _s.SelectTab(tab_paiju)
    local playCardLogic = PlayGameSys.GetPlayLogic()
    if playCardLogic and playCardLogic.roomObj and playCardLogic.roomObj.playerMgr then
        data_playerList = playCardLogic.roomObj.playerMgr.seatList
    end
    _s.InitPlayHead()
end

--刷新对局流水
function roomDetail_ui_window.Refreshliushui()
    local playCardLogic = PlayGameSys.GetPlayLogic()
    if playCardLogic then
        playCardLogic:SendAskHistoryScore()
    end
end

-- 刷新番型
function roomDetail_ui_window.Refreshfanxing()
    local game_id = 1
    _s.InitFanXingScrollView()
end

--收到对局流水回复
function roomDetail_ui_window.HistoryScorePush(eventId, p1, p2)
    data_liushui_scrollview_items = p1.items
    _s.InitLiuShuiScrollView()
end

--刷新房间信息
function roomDetail_ui_window.Refreshpaiju()
    local playCardLogic = PlayGameSys.GetPlayLogic()
    local room = playCardLogic.roomObj
    if room and room.roomInfo then
        m_luaWindowRoot:SetLabel(txt_roomId, room.roomInfo.roomId)
    end
    local des = DataManager.GetRoomDes()
    _s.paiju_list = split(des, " ")
    -- DwDebug.Log("_s.paiju_list")
    -- DwDebug.Log(_s.paiju_list)
    if paiju_scrollview == nil then
        local sv = m_luaWindowRoot:GetTrans("paiju_scrollview")
        local paiju_item = m_luaWindowRoot:GetTrans("paiju_item").gameObject
        paiju_scrollview = EZfunLimitScrollView.GetOrAddLimitScr(sv)
        paiju_scrollview:InitForLua(sv, paiju_item, UnityEngine.Vector2.New(0, 0), UnityEngine.Vector2.New(0, 0), LimitScrollViewDirection.SVD_Vertical, false)
        paiju_scrollview:SetInitItemCallLua("roomDetail_ui_window.InitPaiJuScrollViewItem")
    end

    paiju_scrollview:InitItemCount(#_s.paiju_list, true)
end

function roomDetail_ui_window.InitLiuShuiScrollView()
    -- DwDebug.Log("测试房间详情信息 ")
    if liushui_scrollview == nil then
        local sv = m_luaWindowRoot:GetTrans("liushui_scrollview")
        local liushui_item = m_luaWindowRoot:GetTrans("liushui_item").gameObject
        liushui_scrollview = EZfunLimitScrollView.GetOrAddLimitScr(sv)
        liushui_scrollview:InitForLua(sv, liushui_item, UnityEngine.Vector2.New(0, 0), UnityEngine.Vector2.New(0, 0), LimitScrollViewDirection.SVD_Vertical, false)
        liushui_scrollview:SetInitItemCallLua("roomDetail_ui_window.InitLiuShuiScrollViewItem")
    end

    local count = 0
    if data_liushui_scrollview_items[1] and data_liushui_scrollview_items[1].scores then
        count = #data_liushui_scrollview_items[1].scores
    end
    -- DwDebug.Log("测试房间详情信息 count " .. count)
    DwDebug.Log(data_liushui_scrollview_items)

    liushui_scrollview:InitItemCount(count, true)
end

function roomDetail_ui_window.getCfg()
    _s.cfg = m_FanXingCfg[PlayGameSys.GetNowPlayId()]
    return _s.cfg
end

function roomDetail_ui_window.InitFanXingScrollView()
    if fanxing_scrollview == nil then
        local sv = m_luaWindowRoot:GetTrans("fanxing_scrollview")
        local fanxing_item = m_luaWindowRoot:GetTrans("fanxing_item").gameObject
        fanxing_scrollview = EZfunLimitScrollView.GetOrAddLimitScr(sv)
        fanxing_scrollview:InitForLua(sv, fanxing_item, UnityEngine.Vector2.New(0, 0), UnityEngine.Vector2.New(0, 0), LimitScrollViewDirection.SVD_Vertical, false)
        fanxing_scrollview:SetInitItemCallLua("roomDetail_ui_window.InitFanXingScrollViewItem")
    end

    fanxing_scrollview:InitItemCount(#_s.getCfg(), true)
end

function roomDetail_ui_window.InitPlayHead()
    local playerMgr = PlayGameSys.GetPlayLogic().roomObj.playerMgr
    local sortIndexs = LogicUtil.GetSortIndex(data_playerList)

    local index = 1
    for i = 1, 4 do
        if sortIndexs[i] and data_playerList[sortIndexs[i]] then
            player = playerMgr:GetPlayerByPlayerID(data_playerList[sortIndexs[i]])
            if player then
                m_luaWindowRoot:SetActive(ico_headTrans[index], true)
                WindowUtil.LoadHeadIcon(m_luaWindowRoot, ico_headTrans[index], player.seatInfo.headUrl, player.seatInfo.sex, false, RessStorgeType.RST_Never)
            end
            index = index + 1
        else
            -- 从第四个开始往前依次隐藏
            m_luaWindowRoot:SetActive(ico_headTrans[4 - i + index], false)
        end
    end
end

--获取当前玩家积分
local function GetHistoryScroll(userId, index)
    for key, item in pairs(data_liushui_scrollview_items) do
        if item.userId == userId and item.scores and item.scores[index + 1] then
            return item.scores[index + 1]
        end
    end
    return 0
end

function roomDetail_ui_window.InitLiuShuiScrollViewItem(trans, index)
    local sortIndexs = LogicUtil.GetSortIndex(data_playerList)
    trans.name = "liushui_item_" .. (index + 1)
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "txt_num"), "第" .. (index + 1) .. "局")
    local colume = 1
    local node
    for i = 1, 4 do
        if sortIndexs[i] and data_playerList[sortIndexs[i]] then
            node = m_luaWindowRoot:GetTrans(trans, "txt_" .. colume)
            m_luaWindowRoot:SetActive(node, true)
            m_luaWindowRoot:SetLabel(node, GetHistoryScroll(data_playerList[sortIndexs[i]], index))
            colume = colume + 1
        else
            -- 从第四个开始往前依次隐藏
            node = m_luaWindowRoot:GetTrans(trans, "txt_" .. (4 - i + colume))
            m_luaWindowRoot:SetActive(node, false)
        end
        m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "bg"), index % 2 == 1)
    end
end

function roomDetail_ui_window.InitFanXingScrollViewItem(trans, index)
    trans.name = "fanxing_item_" .. (index + 1)
    local item_data = _s.getCfg()[index + 1]
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "txt_fanxing"), item_data[1])
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "txt_beishu"), item_data[2])
    m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "bg"), index % 2 == 1)
end

function roomDetail_ui_window.InitPaiJuScrollViewItem(trans, index)
    trans.name = "paiju_item_" .. (index + 1)
    DwDebug.Log("InitPaiJuScrollViewItem ", index)
    if _s.paiju_list[index + 1] then
        m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "txt_paiju"),"● ".. _s.paiju_list[index + 1])
        m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "bg"), index % 2 == 1)
    end
end

function roomDetail_ui_window.RefreshContent()
    if cur_tab == tab_paiju then
        _s.Refreshpaiju()
    elseif cur_tab == tab_liushui then
        _s.Refreshliushui()
    else
        _s.Refreshfanxing()
    end
end

--设置tab
local function setTabSelect(trans, isSelect)
    m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "select_node"), isSelect)
    m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans(trans, "unselect_node"), not isSelect)
end

--点击tab
function roomDetail_ui_window.SelectTab(tabGo)
    if cur_tab == tabGo then
        return
    end

    m_luaWindowRoot:SetActive(tab_fanxing, _s.getCfg() and true or false)

    cur_tab = tabGo
    setTabSelect(tab_paiju, tab_paiju == tabGo)
    setTabSelect(tab_liushui, tab_liushui == tabGo)
    setTabSelect(tab_fanxing, tab_fanxing == tabGo)
    m_luaWindowRoot:SetActive(liushui, tab_liushui == tabGo)
    m_luaWindowRoot:SetActive(paiju, tab_paiju == tabGo)
    m_luaWindowRoot:SetActive(fanxing, tab_fanxing == tabGo)
    _s.RefreshContent()
end

function roomDetail_ui_window.HandleWidgetClick(gb)
    local click_name = gb.name
    if click_name == "tab_paiju" or click_name == "tab_liushui" or click_name == "tab_fanxing" then
        _s.SelectTab(gb.transform)
    elseif click_name == "btnClose" or click_name == "bg_window" then
        WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
        _s.InitWindow(false, 0)
    end
end

function roomDetail_ui_window.Register()
    LuaEvent.AddHandle(EEventType.HistoryScorePush, _s.HistoryScorePush)
end

function roomDetail_ui_window.UnRegister()
    --m_luaWindowRoot = nil
    LuaEvent.RemoveHandle(EEventType.HistoryScorePush, _s.HistoryScorePush)
end

function roomDetail_ui_window.OnDestroy()
    _s.UnRegister()

    m_luaWindowRoot = nil
    ico_headTrans = {}
    paiju_scrollview = nil
    liushui_scrollview = nil
    fanxing_scrollview = nil
    data_liushui_scrollview_items = {}
    data_playerList = {}
end
