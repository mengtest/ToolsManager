require "Tools.EZToggleGroup"

local m_luaWindowRoot
-- local m_state
-- local m_EZToggleGroup

local scrollContent
local icons = {}
local nowIndex2Url = {}
local itemHeigh = 520
local nowIndex = 0
local tickTime = 0
local adUrlLen = 0
local isPlaying = false

local maskPanel

require "LuaWindow.Module.UINetTipsCtrl"
local m_netTipsCtrl = nil

activity_ui_window = {}
local _s = activity_ui_window

function activity_ui_window.Init(LuaWindowRoot)
    m_luaWindowRoot = LuaWindowRoot
end

function activity_ui_window.InitWindow(open, state)
    m_luaWindowRoot:InitCamera(open, false, false, true, -1)
    m_luaWindowRoot:BaseIniwWindow(open, state)
    if open then
        _s.PreLoadData()
        -- _s.InitWindowDetail()

        UpdateSecond:Add(_s.UpdateSecond)
    else
        UpdateSecond:Remove(_s.UpdateSecond)
    end
end

function activity_ui_window.PreLoadData()
    if not m_netTipsCtrl then
        m_netTipsCtrl = UINetTipsCtrl.New()
        m_netTipsCtrl:Init(
            m_luaWindowRoot,
            m_luaWindowRoot:GetTrans("netTipsRoot"),
            function()
                WebNetHelper.requestNotice(
                    0,
                    function(body, head)
                        if body and body.data then
                            local data = body.data

                            local list_activity = data.list_activity
                            local list_ad = data.list_ad
                            local list_horn = data.list_horn

                            if #list_activity > 0 then
                                DataManager.SetSysActivityData(list_activity)
                            end
                            if #list_ad > 0 then
                                DataManager.SetSysAdData(list_ad)
                            end
                            if #list_horn > 0 then
                                DataManager.SetSysHornData(list_horn)
                            end
                        end
                        _s.UpdateUIWithData()
                        m_netTipsCtrl:StopWork(true)
                    end,
                    function(body, head)
                        DwDebug.Log("WebNetHelper requestNotice failed")
                        m_netTipsCtrl:StopWork(false)
                    end
                )
            end
        )
	end
	if DataManager.GetSysActivityData() then
		_s.UpdateUIWithData()
    else
        m_luaWindowRoot:SetActive(maskPanel, false)
		m_netTipsCtrl:StartWork()
	end
end

function activity_ui_window.UpdateUIWithData()
    m_luaWindowRoot:SetActive(maskPanel, true)
    nowIndex = 0
    -- m_luaWindowRoot:PlayAndStopTweens(scrollContent, false)
    scrollContent.localPosition = Vector3.New(0, 0, 0)
    for i = 1, 3 do
        icons[i].localPosition = Vector3.New(0, 1040 - 520*i, 0)
    end
	isPlaying = false
	_s.RefreshNoticeActivity()
end

function activity_ui_window.CreateWindow()
    scrollContent = m_luaWindowRoot:GetTrans("scrollContent")
    for i = 1, 3 do
        icons[i] = m_luaWindowRoot:GetTrans("icon_" .. i)
    end
    maskPanel = m_luaWindowRoot:GetTrans("maskPanel")
    _s.Register()
end

function activity_ui_window.InitWindowDetail()
    isPlaying = false
    _s.RefreshNoticeActivity()
end

-- if adUrlLen == 10
-- 0 - 9 to 1 - 10
-- -10 - -1 to 1 - 10
function activity_ui_window._toTableIndex(index)
    index = index % adUrlLen
    if index < 0 then
        return index + adUrlLen + 1
    else
        return index + 1
    end
end

-- if adUrlLen == 10
-- 0 - 9 to 0 - 9
-- -10 - -1 to 0 - 9
function activity_ui_window._toArrayIndex(index)
    index = index % adUrlLen
    if index < 0 then
        return index + adUrlLen
    else
        return index
    end
end

-- array转到nowIndex
function activity_ui_window._toNowIndex(index)
    local tableIndex = _s._toArrayIndex(nowIndex)
    return nowIndex + index - tableIndex
end

--播放
function activity_ui_window.PlayTween(isAdd)
    if isPlaying or icons == nil or #icons == 0 then
        return
    end

    if nil == nowIndex2Url or 1 >= #nowIndex2Url then
        -- 只有一张图片直接返回
        return
    end

    local posY = itemHeigh * nowIndex
    if isAdd then
        nowIndex = nowIndex + 1
    else
        nowIndex = nowIndex - 1
    end
    local toPosY = itemHeigh * nowIndex

    m_luaWindowRoot:PlayTweenPos(scrollContent, 0.8, false, posY, toPosY)
    isPlaying = true

    if _s.timer_PlayTween and _s.timer_PlayTween ~= -1 then
        TimerTaskSys.RemoveTask(_s.timer_PlayTween)
        _s.timer_PlayTween = -1
    end
    _s.timer_PlayTween =
        TimerTaskSys.AddTimerEventByLeftTime(
        function()
            _s.timer_PlayTween = -1
            if m_luaWindowRoot ~= nil and m_luaWindowRoot.m_open then
                isPlaying = false
                if isAdd then
                    icons[1].localPosition = UnityEngine.Vector3.New(0, icons[#icons].localPosition.y - itemHeigh, 0)

                    local temp = icons[1]
                    table.remove(icons, 1)
                    table.insert(icons, temp)

                    local tableIndex = _s._toTableIndex(nowIndex + 1)
                    m_luaWindowRoot:LoadImag(temp, nowIndex2Url[tableIndex], "", false, RessStorgeType.RST_Never,true)
                else
                    icons[#icons].localPosition = UnityEngine.Vector3.New(0, icons[1].localPosition.y + itemHeigh, 0)

                    local temp = icons[#icons]
                    table.remove(icons, #icons)
                    table.insert(icons, 1, temp)

                    local tableIndex = _s._toTableIndex(nowIndex - 1)
                    m_luaWindowRoot:LoadImag(temp, nowIndex2Url[tableIndex], "", false, RessStorgeType.RST_Never,true)
                end
                _s.m_panel_scrollView.SelectIndex = _s._toArrayIndex(nowIndex)
            end
        end,
        0.8
    )

    tickTime = 0
end

function activity_ui_window.PlayTweenToIndex(index)
    DwDebug.Log("activity_ui_window.PlayTweenToIndex")
    if index == nowIndex or icons == nil or #icons == 0 then
        return
    end

    if nil == nowIndex2Url or 1 >= #nowIndex2Url then
        -- 只有一张图片直接返回
        return
    end

    local isAdd, toPosY = true, nil
    local posY = itemHeigh * nowIndex

    if index > nowIndex then
        isAdd = true
        toPosY = posY + itemHeigh
        local tableIndex = _s._toTableIndex(index)
        m_luaWindowRoot:LoadImag(icons[3], nowIndex2Url[tableIndex], "", false, RessStorgeType.RST_Never,true)
    else
        isAdd = false
        toPosY = posY - itemHeigh
        local tableIndex = _s._toTableIndex(index)
        m_luaWindowRoot:LoadImag(icons[1], nowIndex2Url[tableIndex], "", false, RessStorgeType.RST_Never,true)
    end

    m_luaWindowRoot:PlayTweenPos(scrollContent, 0.8, false, posY, toPosY)
    isPlaying = true

    if _s.timer_PlayTweenToIndex and _s.timer_PlayTweenToIndex ~= -1 then
        TimerTaskSys.RemoveTask(_s.timer_PlayTweenToIndex)
        _s.timer_PlayTweenToIndex = -1
    end
    _s.timer_PlayTweenToIndex =
        TimerTaskSys.AddTimerEventByLeftTime(
        function()
            _s.timer_PlayTweenToIndex = -1
            if m_luaWindowRoot ~= nil and m_luaWindowRoot.m_open then
                isPlaying = false

                -- 完成的时候跳到指定位置
                nowIndex = index
                local endPosY = itemHeigh * nowIndex
                scrollContent.localPosition = UnityEngine.Vector3.New(0, endPosY, 0)
                m_luaWindowRoot:PlayTweenPos(scrollContent, 0.01, false, endPosY, endPosY)

                -- 确保已经在中间的是在table中间
                if isAdd then
                    local temp = icons[1]
                    table.remove(icons, 1)
                    table.insert(icons, temp)
                else
                    local temp = icons[#icons]
                    table.remove(icons, #icons)
                    table.insert(icons, 1, temp)
                end

                local startIndex = nowIndex - 2
                endPosY = -endPosY + itemHeigh
                for i = 1, 3 do
                    local temp = icons[i]
                    temp.localPosition = UnityEngine.Vector3.New(0, endPosY, 0)

                    -- 中间的设置暂时跳过，优化闪一下的感觉
                    if i ~= 2 then
                        local tableIndex = _s._toTableIndex(startIndex + i)
                        m_luaWindowRoot:LoadImag(temp, nowIndex2Url[tableIndex], "", false, RessStorgeType.RST_Never,true)
                    end

                    endPosY = endPosY - itemHeigh
                end
            end
        end,
        0.8
    )

    tickTime = 0
end

function activity_ui_window.UpdateSecond()
end

-- 点击事件
function activity_ui_window.HandleWidgetClick(gb)
    local click_name = gb.name
    if click_name == "close_btn" then
        WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
        _s.InitWindow(false, 0)
    elseif string.find(click_name, "tab_") then
        if not isPlaying then
            local index = tonumber(string.sub(click_name, string.len("tab_") + 1))
            DwDebug.Log("click index:" .. index)
            _s.m_panel_scrollView.SelectIndex = index
            _s.PlayTweenToIndex(_s._toNowIndex(index))
        end
    end
end

-- 拖动事件
function activity_ui_window.HandleWidgetDrag(deltaX, deltaY)
    if deltaY > 0 then
        _s.PlayTween(true)
    elseif deltaY < 0 then
        _s.PlayTween(false)
    end
end

function activity_ui_window.SelectTabs(trans, index, isSelect)
    -- nowIndex = index + 1
    local selectTrans = m_luaWindowRoot:GetTrans(trans, "select")
    local unSelectTrans = m_luaWindowRoot:GetTrans(trans, "unSelect")
    m_luaWindowRoot:SetActive(selectTrans, isSelect)
    m_luaWindowRoot:SetActive(unSelectTrans, not isSelect)
end

function activity_ui_window.RefreshNoticeActivity()
    if m_luaWindowRoot == nil or not m_luaWindowRoot.m_open then
        return
    end

    local list = DataManager.GetSysActivityData()
    if not list then
        _s.PreLoadData()
        return
    end

    adUrlLen = #list
    if list and adUrlLen > 0 then
        local url
        local _next = list[2] and 2 or adUrlLen

        url = list[adUrlLen].img_url
        m_luaWindowRoot:LoadImag(icons[1], url, "", false, RessStorgeType.RST_Never,true)

        url = list[1].img_url
        m_luaWindowRoot:LoadImag(icons[2], url, "", false, RessStorgeType.RST_Never,true)

        url = list[_next].img_url
        m_luaWindowRoot:LoadImag(icons[3], url, "", false, RessStorgeType.RST_Never,true)

        for i = 1, adUrlLen do
            nowIndex2Url[i] = list[i].img_url
        end

        if _s.m_panel_scrollView == nil then
            local rootTrans = m_luaWindowRoot:GetTrans("game_tabs")
            local item = m_luaWindowRoot:GetTrans("gameTabItem").gameObject

            _s.m_panel_scrollView = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
            _s.m_panel_scrollView:InitForLua(rootTrans, item, UnityEngine.Vector2.New(236, 104), UnityEngine.Vector2.New(1, 5), LimitScrollViewDirection.SVD_Vertical, false)

            local initFunc = function(trans, item_index)
                local index = item_index + 1
                local content = list[index].content

                trans.name = "tab_" .. item_index
                m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "SLabel"), content)
                m_luaWindowRoot:SetLabel(m_luaWindowRoot:GetTrans(trans, "NLabel"), content)

                local isSelect = _s._toTableIndex(nowIndex) == index
                _s.SelectTabs(trans, item_index, isSelect)
            end
            _s.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(initFunc)
            _s.m_panel_scrollView:SetInitItemCallLua(_s.m_initFuncSeq)
            _s.m_selectFuncSeq = LuaCsharpFuncSys.RegisterFunc(_s.SelectTabs)
            _s.m_panel_scrollView:SetSelectOrUnSelectFunc(_s.m_selectFuncSeq)
        end
        _s.m_panel_scrollView:InitItemCount(adUrlLen, true)
        _s.m_panel_scrollView.SelectIndex = _s._toArrayIndex(nowIndex)

    -- m_EZToggleGroup:RefreshSelect()
    end
end

function activity_ui_window.Register()
    LuaEvent.AddHandle(EEventType.RefreshNoticeActivity, _s.RefreshNoticeActivity)
end

function activity_ui_window.UnRegister()
    m_luaWindowRoot = nil
    LuaEvent.RemoveHandle(EEventType.RefreshNoticeActivity, _s.RefreshNoticeActivity)
end

function activity_ui_window.OnDestroy()
    _s.UnRegister()
    if m_netTipsCtrl then
        m_netTipsCtrl:Destroy()
        m_netTipsCtrl = nil
    end
    _s.m_panel_scrollView = nil
end
