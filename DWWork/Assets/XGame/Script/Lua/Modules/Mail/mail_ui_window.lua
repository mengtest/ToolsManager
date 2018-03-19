--[[
	@author: mid
	@date: 2018年1月30日11:08:19
    @desc: 大厅里的消息列表界面
    @usage:
    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Modules.Mail.mail_ui_window", false , nil)
]]
local UITools = require "Tools.UITools"
mail_ui_window = {}
local self = mail_ui_window
require "LuaWindow.Module.UINetTipsCtrl"
local m_netTipsCtrl = nil
-- 常量
-- 一页数据有多少条
local CONST_COUNTS_PER_PAGE = 10
-- 是否本地测试
local IS_LOCAL_TESTED = false
-- 类型和图片资源配置表
local CategoryStringCfg = {
    [ENUM_MAIL_CATEGORY_TYPE.SysMsg] = "系统通知",
    [ENUM_MAIL_CATEGORY_TYPE.BuyCardNotice] = "购卡通知",
    [ENUM_MAIL_CATEGORY_TYPE.ActiveiNotice] = "活动通知",
    [ENUM_MAIL_CATEGORY_TYPE.ClubMsg] = "俱乐部通知",
    [ENUM_MAIL_CATEGORY_TYPE.ClubApplyJoinNotice] = "俱乐部通知"
}

-- 开始时候和结束时候 重置数据
function mail_ui_window.resetData()
    self.cached_listData = {} -- 当前数据列表
    self.cached_map_page2list = {} -- 所有页码的缓存表
    self.data_diff = 0
    self.data_curPageIndex = 1 -- 当前页码
    self.last_pageIndex = 1 -- 上一个页码 -- 好像用不上了
    self.page_size = CONST_COUNTS_PER_PAGE * 2 -- 页面大小
    -- self.category = 102 --俱乐部类型
    -- self.category = 101 --俱乐部类型
    -- 选中状态相关
    self.curSelectIndex = 0
    self.last_sel_item_node_tran = nil
    self.last_sel_index = nil
    self.all_new_num = 0
    self.group_new_num = 0
    self.client_cache_open_num = 0
    self.request_times_map = {}
end
-- 初始化 第一个执行的
function mail_ui_window.Init(ctx)
    self.ctx = ctx -- luawindowRoot
    self.resetData()
end
-- 第二个执行
function mail_ui_window.InitWindow(open, state)
    self.ctx:InitCamera(open, false, false, true, -1)
    self.ctx:BaseIniwWindow(open, state)
    self.open = open
    if open then
        self.updateMainUI()
        LuaEvent.AddHandle(EEventType.RefreshMailUIList, self.RefreshList)
    else
        LuaEvent.RemoveHandle(EEventType.RefreshMailUIList, self.RefreshList)
        -- 设置红点数据
        -- DwDebug.Log("设置红点数据")
        -- DwDebug.Log(self.group_new_num > 0)
        -- DwDebug.Log(self.client_cache_open_num , self.all_new_num)
        DataManager.SetIsShowMailRedPoint(self.client_cache_open_num < self.all_new_num or self.group_new_num > 0)
        DataManager.GetIsShowClubApplyMsgRedPoint(self.group_new_num > 0)
        LuaEvent.AddEventNow(EEventType.RefreshHallMailBtnRedPoint)
        -- 关闭界面时候 清除数据
        self.resetData()
    end
end
-- 第3个 只执行一次
function mail_ui_window.CreateWindow()
end
-- 窗口销毁时候
function mail_ui_window.OnDestroy()
    self.ctx = nil
    self.uiScrollViewObj = nil
    if m_netTipsCtrl then
        m_netTipsCtrl:Destroy()
        m_netTipsCtrl = nil
    end
end
-- 主界面
function mail_ui_window.updateMainUI()
    -- 初始化列表
    self.initListUI()
    self.updateMailUI()
    -- 请求列表数据 之后 刷新UI
    self.webRequestListDataByPageIndex(self.data_curPageIndex)
end
-- 初始化列表UI
function mail_ui_window.initListUI()
    local ctx = self.ctx
    local listTrans = ctx:GetTrans("list")
    local itemTrans = ctx:GetTrans("mail_item")
    ctx:SetActive(itemTrans, false)
    -- 如果没有列表对象
    if not self.uiScrollViewObj then -- self.uiScrollViewObj 列表对象
        self.uiScrollViewObj = UITools.CreateScrollView(listTrans, itemTrans, 0, 0, 0, 0, 1, false, "mail_ui_window.updateListItem")
        self.uiScrollViewObj.SelectIndex = 0
        self.uiScrollViewObj:SetSelectOrUnSelectFuncByName("mail_ui_window.updateListItem")
    end
    self.uiScrollViewObj:InitItemCount(0, false)
    self.ctx:SetActive(self.ctx:GetTrans("txt_listIsEmpty"), false)
end
-- 请求某一页的数据
function mail_ui_window.webRequestListDataByPageIndex(requset_page_index)
    local function sucCB(body, head)
        -- TimerTaskSys.AddTimerEventByLeftTime(
        -- function()
        if (not body) or (not body.data) or (not body.data.list) then
            return
        end
        if not self.open then
            -- 防止还没回包就关闭界面了 回包后错误
            return
        end
        -- 红点相关
        self.all_new_num = body.data.all_new_num
        self.group_new_num = body.data.group_new_num
        -- DwDebug.Log("邮件和俱乐部申请列表红点数据", self.all_new_num, self.group_new_num)
        -- for k, v in pairs(body.data) do
        --     print(k, v)
        -- end

        DataManager.SetIsShowMailRedPoint(self.all_new_num > 0)
        DataManager.SetIsShowClubApplyMsgRedPoint(self.group_new_num > 0)
        LuaEvent.AddEventNow(EEventType.RefreshHallMailBtnRedPoint)
        LuaEvent.AddEventNow(EEventType.RefreshClubMailRedPoint)

        local list_data = body.data.list -- 列表数据
        local page_index = list_data.page_index
        self.page_count = list_data.page_count --记录最新的页码总数
        self.cached_map_page2list[page_index] = list_data
        -- DwDebug.Log("列表数据内容")
        -- DwDebug.Log("大厅邮件列表web请求数据回包,第" .. list_data.page_index .. "页", "列表长度" .. #list_data.items)
        -- DwDebug.Log("当前页码", self.data_curPageIndex)
        -- DwDebug.Log("页码总数", self.page_count)
        -- 当前维护的数据页码
        local data_curPageIndex = self.data_curPageIndex
        local is_in_preCurNext = false
        -- 去除无用数据
        for _page_index, _list_data in pairs(self.cached_map_page2list) do
            if ((_page_index == self.data_curPageIndex) or (_page_index == self.data_curPageIndex + 1) or (_page_index == self.data_curPageIndex - 1)) then
                is_in_preCurNext = true
            else
                self.cached_map_page2list[_page_index] = nil
                _list_data = nil
            end
        end
        -- 得出当前列表数据
        local t = {}
        for _page_index, _list_data in pairs(self.cached_map_page2list) do
            local items = _list_data.items
            for i = 1, #items do
                t[#t + 1] = items[i]
            end
        end
        self.cached_listData = t
        -- DwDebug.Log("新的数据长度", #self.cached_listData, self.cached_listData)
        -- DwDebug.Log("是否属于上一页当前页下一页的数据", tostring(is_in_preCurNext))
        if is_in_preCurNext then
            local is_first_update = (self.data_curPageIndex == 1 and #self.cached_listData == self.page_size)
            -- DwDebug.Log("是否第一次初始化列表", tostring(is_in_preCurNext))
            if self.uiScrollViewObj then
                self.data_diff = math.max(self.data_curPageIndex - 2, 0)
                self.data_diff = self.data_diff * self.page_size
                -- DwDebug.Log("数据起始偏移量 ", tostring(self.data_diff))
                self.uiScrollViewObj:InitItemCount(self.data_diff + #self.cached_listData, is_first_update) -- 如果是第一页 就为true 并且缓存数据长度为一页的长度
                self.ctx:SetActive(self.ctx:GetTrans("txt_listIsEmpty"), self.data_diff + #self.cached_listData == 0)
                self.updateMailUI()
            end
            if is_first_update then
                mail_ui_window.PageIndexChangeCallback(1)
                self.updateMailUI()
                -- 如果第一条信息非已读 并且不是俱乐部类型 则发送已读协议给后端
                -- DwDebug.Log("如果第一条信息非已读 并且不是俱乐部类型 则发送已读协议给后端")
                -- DwDebug.Log(self.cached_listData[1])
                if self.cached_listData[1] then
                    if not self.cached_listData[1].is_process and self.cached_listData[1].category ~= ENUM_MAIL_CATEGORY_TYPE.ClubApplyJoinNotice then
                        local function sucCB(body, head)
                            if body.errcode == 0 then
                                -- DwDebug.Log("标记为已读成功")
                                self.client_cache_open_num = self.client_cache_open_num + 1
                            else
                            end
                        end
                        local function failCB(body, head)
                            -- DwDebug.Log("标记为已读web返回失败")
                        end
                        -- DwDebug.Log("如果第一条信息非已读 并且不是俱乐部类型 则发送已读协议给后端 Send")
                        WebNetHelper.RequestReadMailMessageById(id, sucCB, failCB)
                    end
                end
            end
        end
        if self.ctx and self.ctx.m_open then
            m_netTipsCtrl:StopWork(true)
        end
        --     end,
        --     0.6
        -- )
    end
    local function failCB(body, head)
        -- DwDebug.Log("WebNetHelper.RequestMsgList fail callback 大厅邮件列表web请求数据回包 失败 ")
        m_netTipsCtrl:StopWork(false)
    end
    -- 测试判断
    if IS_LOCAL_TESTED then
        -- 测试数据
        TimerTaskSys.AddTimerEventByLeftTime(
            function()
                sucCB(self.genTest(requset_page_index))
            end,
            0.01
        )
    else
        local function request_fun()
            WebNetHelper.RequestMsgList(nil, requset_page_index, self.page_size, sucCB, failCB)
        end
        -- DwDebug.Log("self.category : " .. self.category)
        if not m_netTipsCtrl then
            m_netTipsCtrl = UINetTipsCtrl.New()
            m_netTipsCtrl:Init(self.ctx, self.ctx:GetTrans("netTipsRoot"), request_fun)
        end
        -- local is_first_update = (self.data_curPageIndex == 1 and #self.cached_listData == self.page_size)
        if #self.cached_listData == 0 then
            self.ctx:GetTrans("netTipsRoot").localPosition = UnityEngine.Vector3.New(0, 0, 0)
            m_netTipsCtrl:StartWork()
        else
            -- request_fun()
            self.ctx:GetTrans("netTipsRoot").localPosition = UnityEngine.Vector3.New(130, 0, 0)
            m_netTipsCtrl:StartWork()
        end
    end
end
-- 刷新邮件内容
function mail_ui_window.updateMailUI()
    -- DwDebug.Log(self.cached_listData, self.uiScrollViewObj.SelectIndex+1)
    local selected_data = self.cached_listData[self.uiScrollViewObj.SelectIndex + 1 - self.data_diff]
    -- DwDebug.Log("刷新邮件内容", self.uiScrollViewObj.SelectIndex + 1, self.data_diff, selected_data)
    if selected_data then
        -- DwDebug.Log(" --------------------- --------------------- --------------------- 显示节点")
        local group_id, group_name, uid, wx_nickname, wx_headimgurl, addit_msg, wx_sex, new_content
        if IS_LOCAL_TESTED then
            group_id = selected_data.group_id
            group_name = selected_data.group_name
            uid = selected_data.uid
            wx_nickname = selected_data.wx_nickname
            wx_headimgurl = selected_data.wx_headimgurl
            addit_msg = selected_data.addit_msg
            wx_sex = selected_data.wx_sex
            new_content = selected_data.content
        else
            -- elseif selected_data.content and selected_data.category == ENUM_MAIL_CATEGORY_TYPE.SysMsg then
            -- DwDebug.Log("=============== content为空")
            -- DwDebug.Log(selected_data)
            -- selected_data.new_content = selected_data.content
            -- end
            -- if selected_data.content and selected_data.category == ENUM_MAIL_CATEGORY_TYPE.ClubApplyJoinNotice then
            -- DwDebug.LogError("xxx", type(selected_data.content))
            local pbCmdId = MAP_Mail_EventType_2_pbCmdId[selected_data.event_type]
            -- DwDebug.Log("selected_data.event_type" .. selected_data.event_type)
            local pbData = ProtoManager.Decode(pbCmdId, selected_data.content)
            if pbData then
                -- DwDebug.Log("pb数据", pbData, "pbCmdId", pbCmdId)
                if pbCmdId == WebEvent.MailCGroupCardWarn then
                    -- DwDebug.Log("--------------------------------------------- 1")
                    selected_data.new_content = pbData.alert
                elseif pbCmdId == WebEvent.MailGroupApplyJoin then
                    -- DwDebug.Log("--------------------------------------------- 2")
                    group_id = pbData.group_id
                    group_name = pbData.group_name
                    uid = pbData.uid
                    wx_nickname = pbData.wx_nickname
                    wx_headimgurl = pbData.wx_headimgurl
                    addit_msg = pbData.addit_msg
                    wx_sex = pbData.wx_sex
                    selected_data.new_content = wx_nickname .. '  申请加入 "' .. group_name .. '"  俱乐部,\n'
                    if addit_msg ~= "" then
                        selected_data.new_content = selected_data.new_content .. "附加信息为:\n        " .. addit_msg
                    end
                elseif (pbCmdId == WebEvent.MailCommon) then
                    -- DwDebug.Log("邮件的内容为", selected_data)
                    -- DwDebug.Log("--------------------------------------------- 3")
                    -- DwDebug.Log("pb数据 ========== ", pbData)
                    if not pbData.message then
                    -- DwDebug.Log("非法邮件数据1", selected_data)
                    end
                    selected_data.new_content = pbData.message
                else
                    -- DwDebug.Log("--------------------------------------------- 4")
                end
            else
                -- DwDebug.Log("pbData 为空")
            end
        end

        local ctx = self.ctx
        -- 激活整个节点
        local mail_node = ctx:GetTrans("mail_node")
        ctx:SetActive(mail_node, true)
        -- 邮件标题
        ctx:SetLabel(ctx:GetTrans(mail_node, "txt_msgCategory"), CategoryStringCfg[selected_data.category])
        -- 邮件内容
        if selected_data.new_content then
            ctx:SetLabel(ctx:GetTrans("txt_msgContent"), selected_data.new_content)
        else
            -- DwDebug.Log(" selected_data.new_content 不存在")
        end
        -- 邮件时间
        ctx:SetLabel(ctx:GetTrans("txt_msgTime"), os.date("%Y-%m-%d %H:%M", selected_data.send_time))
        -- 如果是俱乐部申请消息 则显示查看按钮
        local is_show_btn
        if IS_LOCAL_TESTED then
            is_show_btn = (string.find(selected_data.content, "btn") ~= nil)
        else
            -- local
            is_show_btn = selected_data.category == ENUM_MAIL_CATEGORY_TYPE.ClubApplyJoinNotice -- 类型是否是消息通知类型
            is_show_btn = is_show_btn and (not selected_data.is_process) -- 检测是否已读
        end
        ctx:SetActive(ctx:GetTrans("btn_check"), is_show_btn)
    else
        -- DwDebug.Log(" --------------------- --------------------- --------------------- 隐藏节点")
        self.ctx:SetActive(self.ctx:GetTrans("mail_node"), false)
    end
end
-- 刷新项列表内容
function mail_ui_window.updateListItem(item_node_tran, item_index)
    item_index = item_index + 1 -- 从1开始
    -- DwDebug.Log("-------- 刷新列表子项  " .. item_index)
    -- DwDebug.Log("-------- 刷新列表子项  " .. item_index .. "偏移量" .. self.data_diff, self.cached_listData)
    -- 根据 index 来识别第几个item和名字
    item_node_tran.name = "itemBtn_bg" .. item_index
    -- 当前列表项数据
    local item_data = self.cached_listData[item_index - self.data_diff]
    -- DwDebug.Log(" ------- 列表项数据 item_data ")
    -- for k, v in pairs(item_data) do
    --     print(k, v)
    -- end
    -- DwDebug.Log("item_data.content 内容" .. tostring(item_data.content))

    -- 刷新列表项内容
    local is_selected = (self.curSelectIndex + 1 == item_index)
    -- DwDebug.Log("@@是否选中状态" .. tostring(is_selected))
    -- DwDebug.Log("@@是否选中状态" .. tostring(self.uiScrollViewObj.SelectIndex) .. " " .. item_index)
    local ctx = self.ctx
    local selected_node = ctx:GetTrans(item_node_tran, "selected")
    local normal_node = ctx:GetTrans(item_node_tran, "normal")
    ctx:SetActive(selected_node, is_selected)
    ctx:SetActive(normal_node, not is_selected)
    local root_node = is_selected and selected_node or normal_node
    ctx:SetLabel(ctx:GetTrans(root_node, "txt_msgCategory"), CategoryStringCfg[item_data.category])
    -- ctx:SetLabel(ctx:GetTrans(root_node, "txt_msgCategory"), CategoryStringCfg[item_data.category] .. item_index)
    -- if not is_selected then
    --     ctx:SetActive(ctx:GetTrans(root_node, "bg_mail_read"), item_data.is_process)
    --     ctx:SetActive(ctx:GetTrans(root_node, "bg_mail_unread"), not item_data.is_process)
    -- else
    --     ctx:SetActive(ctx:GetTrans(root_node, "bg_mail_read"), true)
    --     ctx:SetActive(ctx:GetTrans(root_node, "bg_mail_unread"), false)
    -- end
    -- DwDebug.Log("item_data.is_process " .. tostring(item_data.is_process))
    ctx:SetActive(ctx:GetTrans(root_node, "bg_mail_read"), item_data.is_process)
    ctx:SetActive(ctx:GetTrans(root_node, "bg_mail_unread"), not item_data.is_process)
end

-- 列表上拉回调
function mail_ui_window.PageUpPullCallback()
    self.webRequestListDataByPageIndex(self.data_curPageIndex - 1)
end
-- 列表下拉回调
function mail_ui_window.PageDownPullCallback()
    self.webRequestListDataByPageIndex(self.data_curPageIndex + 1)
end

-- 这里返回的 curPageIndex 是 UI上的当前页数
function mail_ui_window.PageIndexChangeCallback(curPageIndex)
    -- DwDebug.Log("列表换页回调 现在的UI页码是 " .. curPageIndex)
    if self.last_pageIndex == curPageIndex then
        -- DwDebug.Log("当前页")
    elseif self.last_pageIndex == curPageIndex - 1 then
        -- DwDebug.Log("下一页")
    elseif self.last_pageIndex == curPageIndex + 1 then
        -- DwDebug.Log("上一页")
    else
        -- DwDebug.Log("error")
    end
    -- DwDebug.Log("当前数据页码", self.data_curPageIndex)
    self.data_curPageIndex = curPageIndex
    self.last_pageIndex = curPageIndex
    self.page_upPull_index = -1
    self.page_downPull_index = -1
    local is_exist_pre = curPageIndex > 1 and self.page_count > 1
    if is_exist_pre then
        self.page_upPull_index = CONST_COUNTS_PER_PAGE * (curPageIndex - 1) + self.page_size
    end
    local is_exist_next = curPageIndex < self.page_count
    if is_exist_next then
        self.page_downPull_index = CONST_COUNTS_PER_PAGE
    end
    -- 注册分页功能
    self.uiScrollViewObj:RegisterPageFunctions(
        self.page_size,
        self.page_upPull_index,
        self.page_downPull_index,
        "mail_ui_window.PageIndexChangeCallback", -- 页码改变通知
        "mail_ui_window.PageUpPullCallback", -- 上拉通知
        "mail_ui_window.PageDownPullCallback" -- 下拉通知
    )
end
-- 按钮事件
function mail_ui_window.HandleWidgetClick(go)
    local str = go.name
    local itemCommonStr = string.sub(go.name, 1, 10) -- itemBtn_bg
    -- DwDebug.Log("HandleWidgetClick :" .. itemCommonStr)
    if self["_on_" .. str] then
        self["_on_" .. str]()
    elseif self["_on_" .. itemCommonStr] then
        self["_on_" .. itemCommonStr](go.transform, tonumber(string.sub(go.name, 11, -1))) --"itemBtn_bg + itemIndex"
    else
        -- DwDebug.Log("还没定义按钮" .. str .. "的回调")
        if m_netTipsCtrl then
            m_netTipsCtrl:HandleWidgetClick(go)
        end
    end
end
-- 关闭按钮
function mail_ui_window._on_btn_close()
    WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
    self.InitWindow(false, 0)
end
-- 列表item里的回调 需要节点对象反查父节点的节点名字得出对应的数据序号
function mail_ui_window._on_itemBtn_bg(item_node_tran, item_index)
    -- DwDebug.Log("-------------选中回调" .. item_index .. ";")
    -- item_index = item_index + 1
    -- DwDebug.Log(self.cached_listData[item_index - self.data_diff])
    -- 通知web该消息已读
    local id = self.cached_listData[item_index - self.data_diff].id -- 消息id

    -- 如果非已读 则发送已读协议给后端
    if not self.cached_listData[item_index - self.data_diff].is_process and self.cached_listData[item_index - self.data_diff].category ~= ENUM_MAIL_CATEGORY_TYPE.ClubApplyJoinNotice then
        local function sucCB(body, head)
            if body.errcode == 0 then
                -- DwDebug.Log("标记为已读成功")
                self.client_cache_open_num = self.client_cache_open_num + 1
                for k, v in pairs(self.cached_listData) do
                    if v.id == body.data.id then
                        v.is_process = true
                    end
                end
                self.uiScrollViewObj:InitItemCount(self.data_diff + #self.cached_listData, false) -- 刷新
            end
        end

        local function failCB(body, head)
            -- DwDebug.Log("标记为已读web返回失败")
        end

        WebNetHelper.RequestReadMailMessageById(id, sucCB, failCB)
    end

    self.curSelectIndex = item_index - 1
    self.uiScrollViewObj.SelectIndex = item_index - 1 -- todo 看是以消息id为key 还是以列表中的index为key
    -- if self.last_sel_item_node_tran then
    -- self.updateListItem(self.last_sel_item_node_tran, self.last_sel_index-1)
    -- end

    -- self.uiScrollViewObj:InitItemCount(self.data_diff + #self.cached_listData, false) -- 如果是第一页 就为true 并且缓存数据长度为一页的长度

    -- self.updateListItem(item_node_tran, item_index - 1)
    self.updateMailUI()
end
-- 查看按钮
function mail_ui_window._on_btn_check(go)
    -- 跳转到俱乐部消息界面
    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubApplyMsg_ui_window", false, nil)
end
function mail_ui_window.RefreshList()
    -- self.webRequestListDataByPageIndex(1)
    self.resetData()
    self.updateMainUI()
end
self.request_times_map = {}
-- 单元测试数据
function mail_ui_window.genTest(requset_page_index)
    local request_time = self.request_times_map[requset_page_index]
    if request_time then
        self.request_times_map[requset_page_index] = self.request_times_map[requset_page_index] + 1
    else
        self.request_times_map[requset_page_index] = 1
        request_time = 1
    end
    local body = {
        data = {
            list = {
                page_index = requset_page_index,
                page_size = CONST_COUNTS_PER_PAGE * 2,
                page_count = 5,
                total = 30,
                items = {
                    [1] = {
                        category = 1,
                        content = "测试数据 类型1 第1条",
                        send_time = "2018年2月2日11:20:41",
                        event_type = "事件类型必须和事件ID",
                        id = 1, -- add
                        is_process = false -- add
                    }
                }
            }
        }
    }
    for i = 1, body.data.list.page_size do
        local num = i + (body.data.list.page_index - 1) * CONST_COUNTS_PER_PAGE * 2
        local t = {}
        t.category = i % 2 + 1
        t.content = "类型" .. CategoryStringCfg[t.category] .. "第" .. body.data.list.page_index .. "页,第" .. i .. "条,总 " .. num .. (t.category == 1 and "btn" or "") .. " 第" .. request_time .. "次请求"
        t.send_time = "2018年2月2日11:20:41"
        t.event_type = "aaa"
        t.id = i * 10
        t.is_process = false
        body.data.list.items[i] = t
    end
    return body, head
end
