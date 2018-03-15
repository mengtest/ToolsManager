local m_luaWindowRoot
local m_state
local m_params
local m_gameList = {}
local m_currPage = 0
--窗口是否打开
local m_open = false

require "LuaWindow.Module.UINetTipsCtrl"
local m_netTipsCtrl = nil

record_ui_window = {}
local _s = record_ui_window

function record_ui_window.Init(LuaWindowRoot)
    m_luaWindowRoot = LuaWindowRoot
end

function record_ui_window.InitWindow(open, state)
    m_luaWindowRoot:InitCamera(open, false, false, true, -1)
    m_luaWindowRoot:BaseIniwWindow(open, state)
    m_open = open
    if open then
        -- m_state = state
		-- _s.InitWindowDetail(params)
		_s.PreRequestData()
    else
        if _s.m_panel_scrollView  then
            _s.m_panel_scrollView:InitItemCount(0,true)
        end
        m_currPage = 0
        m_gameList = {}
        if m_netTipsCtrl then
            m_netTipsCtrl:StopWork(true)
        end
    end
end

function record_ui_window.TryOpenWindow()
    PlayRecordSys.SendRecordListReq(
        1,
        function(body)
            WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_Window, true, 1, "record_ui_window", body)
        end
    )
end

function record_ui_window.CreateWindow()
end

function record_ui_window.InitWindowDetail(params)
    if params then
        m_params = params
    elseif m_params then
        params = m_params
    else
        _s.InitWindow(false, 0)
        return
    end
    m_currPage = 1
    _s.InitData(params)
    _s.InitScrollView()

    local item_count = #m_gameList
    if item_count > 0 then
        _s.m_panel_scrollView:InitItemCount(#m_gameList, true)
        -- 后边还有数据，注册预加载函数
        if params.page_index and params.page_count and params.page_index < params.page_count then
            _s.m_panel_scrollView:InitPreLoadDataForLua(10, "record_ui_window.PreRequestData")
        end
    end
end

function record_ui_window.InitData(params)
    if params then
        for i, v in pairs(params.items) do
            table.insert(m_gameList, v)
        end
    end
end

function record_ui_window.InitScrollView()
    if _s.m_panel_scrollView == nil then
        local rootTrans = m_luaWindowRoot:GetTrans("record_scrollview")
        _s.m_panel_scrollView = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
        local item = m_luaWindowRoot:GetTrans("record").gameObject
        _s.m_panel_scrollView:InitForLua(rootTrans, item, UnityEngine.Vector2.New(5, 6), UnityEngine.Vector2.New(1, 4), LimitScrollViewDirection.SVD_Vertical, false)

        local initFunc = function(trans, item_index)
            _s.InitTrans(trans, item_index + 1, m_gameList[item_index + 1])
        end
        _s.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(initFunc)

        _s.m_panel_scrollView:SetInitItemCallLua(_s.m_initFuncSeq)
    end
end

function record_ui_window.InitTrans(root, index, data)
    root.name = "record_" .. index
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "room_id"), data.room_id)
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "game_desc"), "[8F6F56]" .. data.play_name .. "([CE4418]" .. data.total_game_num .. "[-]局)")
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "time_str"), os.date("%Y-%m-%d %H:%M", data.room_create_time))

    if data.jifen >= 0 then
        m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "self_score"), "win", true)
        m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(root, "self_score"), "win"), data.jifen)
    else
        m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "self_score"), "lose", true)
        m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(m_luaWindowRoot:GetTrans(root, "self_score"), "lose"), data.jifen)
    end
    m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "flag"), "win", data.jifen >= 0)

    local name_root = m_luaWindowRoot:GetTrans(root, "player_name")
    local score_root = m_luaWindowRoot:GetTrans(root, "player_score")
    local node, is_self
    local playerNum = #data.players
    for i = 1, playerNum do
        is_self = data.user_id == data.players[i].user_id
        node = m_luaWindowRoot:GetTrans(name_root, "name_" .. i)
        m_luaWindowRoot:SetActive(node, true)
        m_luaWindowRoot:ShowChild(node, "self", is_self)
        node = m_luaWindowRoot:GetTrans(node, is_self and "self" or "other")
        m_luaWindowRoot:SetActive(node, true)
        m_luaWindowRoot:SetLabel(node, utf8sub(data.players[i].nickname, 1, 10))
        node = m_luaWindowRoot:GetTrans(score_root, "score_" .. i)
        m_luaWindowRoot:SetActive(node, true)
        m_luaWindowRoot:ShowChild(node, "self", is_self)
        node = m_luaWindowRoot:GetTrans(node, is_self and "self" or "other")
        m_luaWindowRoot:SetActive(node, true)
        m_luaWindowRoot:SetLabel(node, data.players[i].jifen)
    end
    if playerNum < 4 then
        for i = playerNum + 1, 4 do
            node = m_luaWindowRoot:GetTrans(name_root, "name_" .. i)
            m_luaWindowRoot:SetActive(node, false)
            node = m_luaWindowRoot:GetTrans(node, is_self and "self" or "other")
            m_luaWindowRoot:SetActive(node, false)
            node = m_luaWindowRoot:GetTrans(score_root, "score_" .. i)
            m_luaWindowRoot:SetActive(node, false)
            node = m_luaWindowRoot:GetTrans(node, is_self and "self" or "other")
            m_luaWindowRoot:SetActive(node, false)
        end
    end
end

function record_ui_window.PreRequestData()
    --请求数据回调里调用下两句
    local function request_fun()
        PlayRecordSys.SendRecordListReq(
            m_currPage + 1,
            function(body)
                if m_open then
                    m_currPage = m_currPage + 1
                    _s.InitData(body)
                    _s.InitScrollView()
                    _s.m_panel_scrollView:InitItemCount(#m_gameList, m_currPage == 1)

                    if m_currPage == 1 then
                        if #m_gameList == 0 then
                            -- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("nobody"), true)
                            -- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("player_list"), false)
                            -- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("unauthorized"), false)
                        else
                            -- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("player_list"), true)
                            -- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("nobody"), false)
                            -- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("unauthorized"), false)
                        end
                    end


                    -- 不是最后一页，才注册预加载，最后一页就没必要再请求数据了
                    if body.page_index and body.page_count and body.page_index < body.page_count then
                        _s.m_panel_scrollView:InitPreLoadDataForLua(10, "record_ui_window.PreRequestData")
                    end
                    m_netTipsCtrl:StopWork(true)
                end
            end,
            function()
                m_netTipsCtrl:StopWork(false)
            end
        )
    end
    if not m_netTipsCtrl then
        m_netTipsCtrl = UINetTipsCtrl.New()
        m_netTipsCtrl:Init(
            m_luaWindowRoot,
            m_luaWindowRoot:GetTrans("netTipsRoot"),
            request_fun
        )
    end
    if #m_gameList == 0 then
        m_netTipsCtrl:StartWork()
    else
        m_netTipsCtrl:StartWork()
        -- request_fun()
    end
end

function record_ui_window.HandleWidgetClick(gb)
    local click_name = gb.name
    if click_name == "detail_btn" then
        local index = tonumber(string.sub(gb.transform.parent.name, string.len("record_") + 1))
        PlayRecordSys.SetCurReqRecord(m_gameList[index])
        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_gameList[index].archive_id, "record_detail_ui_window", false, nil)
        -- record_detail_ui_window.TryOpenWindow(m_gameList[index].archive_id)
    elseif click_name == "close_btn" or click_name == "back_btn" then
        WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
        _s.InitWindow(false, 0)
    else
        if m_netTipsCtrl then
            m_netTipsCtrl:HandleWidgetClick(gb)
        end
    end
end

function record_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function record_ui_window.OnDestroy()
    LuaCsharpFuncSys.UnRegisterFunc(_s.m_initFuncSeq)
    _s.m_initFuncSeq = nil
    _s.m_panel_scrollView = nil
	m_gameList = {}
	if m_netTipsCtrl then
        m_netTipsCtrl:Destroy()
        m_netTipsCtrl = nil
    end
end
