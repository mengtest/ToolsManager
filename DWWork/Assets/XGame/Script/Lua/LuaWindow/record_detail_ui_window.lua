local m_luaWindowRoot
local m_state
local m_data
record_detail_ui_window = {}
local _s = record_detail_ui_window
require "LuaWindow.Module.UINetTipsCtrl"
local m_netTipsCtrl = nil

function record_detail_ui_window.Init(LuaWindowRoot)
    m_luaWindowRoot = LuaWindowRoot
end

function record_detail_ui_window.InitWindow(open, state, params)
    DwDebug.Log("InitWindow ", open, state)
    m_luaWindowRoot:InitCamera(open, false, false, true, -1)
    m_luaWindowRoot:BaseIniwWindow(open, state)
    if open then
        if state ~= 0 then
            m_state = state
            m_param = params
            _s.InitWindowDetail(m_state)
        end
    end
end

function record_detail_ui_window.TryOpenWindow(archive_id)
    PlayRecordSys.SendRecordDetailReq(
        archive_id,
        function()
            -- WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, archive_id, "record_detail_ui_window", false , nil)
        end
    )
end

function record_detail_ui_window.CreateWindow()
end

function record_detail_ui_window.InitWindowDetail(archive_id)
    if not m_netTipsCtrl then
        m_netTipsCtrl = UINetTipsCtrl.New()
        m_netTipsCtrl:Init(
            m_luaWindowRoot,
            m_luaWindowRoot:GetTrans("netTipsRoot"),
            function()
                DwDebug.Log(" ----------------- archive_id " .. archive_id)
                -- WebNetHelper.RequestMsgList(nil, requset_page_index, self.page_size, sucCB, failCB)
                PlayRecordSys.SendRecordDetailReq(
                    archive_id,
                    function()
                        _s.InitData()
                        m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("game_name"), m_data[1].play_name .. "(" .. m_data[1].total_game_num .. "局)")
                        if PlayRecordSys.curReqRecordInfo then
                            m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("room_id"), PlayRecordSys.curReqRecordInfo.room_id)
                        end
                        _s.InitScrollView()
                        m_netTipsCtrl:StopWork(true)
                        -- WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, archive_id, "record_detail_ui_window", false , nil)
                    end,
                    function()
                        m_netTipsCtrl:StopWork(false)
                    end
                )
            end
        )
    else
        m_netTipsCtrl:RefreshNetReqAction(
            function()
                PlayRecordSys.SendRecordDetailReq(
                    archive_id,
                    function()
                        _s.InitData()
                        m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("game_name"), m_data[1].play_name .. "(" .. m_data[1].total_game_num .. "局)")
                        if PlayRecordSys.curReqRecordInfo then
                            m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans("room_id"), PlayRecordSys.curReqRecordInfo.room_id)
                        end
                        _s.InitScrollView()
                        m_netTipsCtrl:StopWork(true)
                        -- WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, archive_id, "record_detail_ui_window", false , nil)
                    end,
                    function()
                        m_netTipsCtrl:StopWork(false)
                    end
                )
            end
        )
    end
    m_netTipsCtrl:StartWork()
end

function record_detail_ui_window.InitData()
    m_data = PlayRecordSys.GetRecordDetail(m_state)
    --m_data = InitTestData()
    if not m_data then
        return
    end
end

function record_detail_ui_window.InitScrollView()
    if not m_data then
        return
    end
    local timeCount = #m_data
    if _s.m_panel_scrollView == nil then
        local rootTrans = m_luaWindowRoot:GetTrans("record_scrollview")
        _s.m_panel_scrollView = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
        local item = m_luaWindowRoot:GetTrans("record").gameObject
        _s.m_panel_scrollView:InitForLua(rootTrans, item, UnityEngine.Vector2.New(5, 6), UnityEngine.Vector2.New(1, 4), LimitScrollViewDirection.SVD_Vertical, false)
        local initFunc = function(trans, item_index)
            _s.InitTrans(trans, item_index + 1, m_data[item_index + 1])
        end
        _s.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(initFunc)
        _s.m_panel_scrollView:SetInitItemCallLua(_s.m_initFuncSeq)
    end
    _s.m_panel_scrollView:InitItemCount(timeCount, true)
end

function record_detail_ui_window.InitTrans(root, index, data)
    root.name = "record_" .. index
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "round"), "(第" .. data.game_num .. "局)")
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "time_str"), os.date("%Y-%m-%d %H:%M", data.settlement_time))
    local name_root = m_luaWindowRoot:GetTrans(root, "player_name")
    local score_root = m_luaWindowRoot:GetTrans(root, "player_score")
    local node, is_self
    local playerNum = #data.players
    for i = 1, playerNum do
        is_self = DataManager.GetUserID() == data.players[i].user_id
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

function record_detail_ui_window.HandleWidgetClick(gb)
    local click_name = gb.name
    if click_name == "replay_btn" then
        --call replay
        local index = tonumber(string.sub(gb.transform.parent.name, string.len("record_") + 1))
        local view_record_succ_cb = nil
        if m_param then
            view_record_succ_cb = function ()
                MainCityState.AddEnterCallBack(
                    function ()
                        -- ClubSys.OpenClub(self.m_group_id)
                        -- WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "Club.UIWindow.myclub_ui_window", false , nil)
                        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 2, "Club.UIWindow.statistics.club_statistics_ui_window", false , m_param)
                        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_state, "record_detail_ui_window", false, m_param)
                    end
                )
            end
        else 
            view_record_succ_cb = function ()
                MainCityState.AddEnterCallBack(
                    function ()
                        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 0, "record_ui_window", false, nil)
                        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, m_state, "record_detail_ui_window", false, nil)
                    end
                )                
            end
        end
        
        PlayRecordSys.PlayRecordReplayInfoByIndex(index, view_record_succ_cb)
    elseif click_name == "close_btn" or click_name == "back_btn" then
        WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
        _s.InitWindow(false, 0)
    end
end

function record_detail_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function record_detail_ui_window.OnDestroy()
    LuaCsharpFuncSys.UnRegisterFunc(_s.m_initFuncSeq)
    _s.m_initFuncSeq = nil
    _s.m_panel_scrollView = nil
    if m_netTipsCtrl then
        m_netTipsCtrl:Destroy()
        m_netTipsCtrl = nil
    end
end
