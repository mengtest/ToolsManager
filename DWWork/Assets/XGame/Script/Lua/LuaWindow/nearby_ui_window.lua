local m_luaWindowRoot
local m_state
local m_playerList = {}
local m_currPage = 0

require "LuaWindow.Module.UINetTipsCtrl"
local m_netTipsCtrl = nil

--窗口是否打开
local m_open = false

nearby_ui_window = {}
local _s = nearby_ui_window

function nearby_ui_window.Init(LuaWindowRoot)
    m_luaWindowRoot = LuaWindowRoot
end

function nearby_ui_window.InitWindow(open, state, params)
    m_luaWindowRoot:InitCamera(open, false, false, true, -1)
    m_luaWindowRoot:BaseIniwWindow(open, state)
    m_open = open
    if open then
        -- _s.InitWindowDetail()
        m_state = state
        m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("nobody"), false)
        m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("player_list"), false)
        m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("unauthorized"), false)
        m_playerList = {}
        m_currPage = 0

        nearby_ui_window.PreRequestData()
    else
        -- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("content"), false)
        m_playerList = {}
        m_currPage = 0
    end
end

function nearby_ui_window.CreateWindow()
end

function nearby_ui_window.InitWindowDetail(params)
    -- if m_state == 2 then
    --     m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("content"), "unauthorized", true)
    --     return
    -- elseif not params or not params.items or #params.items <= 0 then
    --     m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("content"), "nobody", true)
    --     return
    -- else
    --     m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans("content"), "player_list", true)
    -- end
    -- hide all
    -- m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("content"), true)

    -- m_currPage = 1
    _s.InitData(params)
    _s.InitScrollView()

    local item_count = #m_playerList
    if item_count > 0 then
        _s.m_panel_scrollView:InitItemCount(#m_playerList, true)
        -- 后边还有数据，注册预加载函数
        if params.page_index and params.page_count and params.page_index < params.page_count then
            _s.m_panel_scrollView:InitPreLoadDataForLua(10, "nearby_ui_window.PreRequestData")
        end
    end
end

function nearby_ui_window.InitData(params)
    if not params then
        return
    end

    for i, v in ipairs(params.items) do
        table.insert(m_playerList, v)
    end
end

function nearby_ui_window.InitScrollView()
    if _s.m_panel_scrollView == nil then
        local rootTrans = m_luaWindowRoot:GetTrans("scrollview")
        _s.m_panel_scrollView = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
        local item = m_luaWindowRoot:GetTrans("player_card").gameObject
        _s.m_panel_scrollView:InitForLua(rootTrans, item, UnityEngine.Vector2.New(0, 0), UnityEngine.Vector2.New(1, 5), LimitScrollViewDirection.SVD_Vertical, false)

        local initFunc = function(trans, item_index)
            _s.InitTrans(trans, item_index + 1, m_playerList[item_index + 1])
        end
        _s.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(initFunc)

        _s.m_panel_scrollView:SetInitItemCallLua(_s.m_initFuncSeq)
    end
end

function nearby_ui_window.InitTrans(root, index, data)
    -- DwDebug.Log("附近的人 列表项", index, data)

    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "name"), data.wx_nickname)
    -- DwDebug.Log("附近的人 列表项更新结束 1")
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "user_id"), tostring(data.uid))
    -- DwDebug.Log("附近的人 列表项更新结束 2")
    m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(root, "distance"), data.geo_dist)
    -- DwDebug.Log("附近的人 列表项更新结束 3")
    m_luaWindowRoot:ShowChild(m_luaWindowRoot:GetTrans(root, "gender"), tostring(data.wx_sex), true)
    -- DwDebug.Log("附近的人 列表项更新结束 4")
    WindowUtil.LoadHeadIcon(m_luaWindowRoot, m_luaWindowRoot:GetTrans(root, "head"), ezfunLuaTool.GetSmallWeiXinIconUrl(data.wx_headimgurl, 64), data.wx_sex, false, RessStorgeType.RST_Never)

    -- DwDebug.Log("附近的人 列表项更新结束 5")

    root.name = "player_" .. index
end

function nearby_ui_window.PreRequestData()
    local function request_fun()
        WebNetHelper.GetNearByPlayers(
            m_currPage + 1,
            20,
            function(body, head)
                if m_open then
                    local recv_data
                    if not body or not body.data or not body.data.list then
                        return
                    else
                        recv_data = body.data.list
                    end

                    m_currPage = m_currPage + 1
                    if m_currPage == 1 then
                        if #recv_data.items == 0 then
                            m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("nobody"), true)
                            m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("player_list"), false)
                            m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("unauthorized"), false)
                        else
                            m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("player_list"), true)
                            m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("nobody"), false)
                            m_luaWindowRoot:SetActive(m_luaWindowRoot:GetTrans("unauthorized"), false)
                        end
                    end
                    _s.InitData(recv_data)
                    _s.InitScrollView()
                    -- DwDebug.LogError("xxx", "nearby people ", recv_data)
                    _s.m_panel_scrollView:InitItemCount(#m_playerList, m_currPage == 1)
                    -- 不是最后一页，才注册预加载，最后一页就没必要再请求数据了
                    if recv_data.page_index and recv_data.page_count and recv_data.page_index < recv_data.page_count then
                        _s.m_panel_scrollView:InitPreLoadDataForLua(10, "nearby_ui_window.PreRequestData")
                    end
                end
                m_netTipsCtrl:StopWork(true)
            end,
            function(body, head)
                print("request nearby error ")

                m_netTipsCtrl:StopWork(false)
            end
        )
    end
    --请求数据回调里调用下两句
    if not m_netTipsCtrl then
        m_netTipsCtrl = UINetTipsCtrl.New()

        m_netTipsCtrl:Init(m_luaWindowRoot, m_luaWindowRoot:GetTrans("netTipsRoot"), request_fun)
    end

    if #m_playerList == 0 then
        m_netTipsCtrl:StartWork()
    else
        -- request_fun()
        m_netTipsCtrl:StartWork()
    end
end

function nearby_ui_window.HandleWidgetClick(gb)
    local click_name = gb.name
    if click_name == "close_btn" or click_name == "back_btn" then
        _s.InitWindow(false, 0)
    else
        if m_netTipsCtrl then
            m_netTipsCtrl:HandleWidgetClick(gb)
        end
    end
end

function nearby_ui_window.UnRegister()
    m_luaWindowRoot = nil
end

function nearby_ui_window.OnDestroy()
    LuaCsharpFuncSys.UnRegisterFunc(_s.m_initFuncSeq)
    _s.m_initFuncSeq = nil
    _s.m_panel_scrollView = nil
    if m_netTipsCtrl then
        m_netTipsCtrl:Destroy()
        m_netTipsCtrl = nil
    end
end
