--[[
	@author: mid
	@date: 2018年1月30日11:08:19
    @desc: 俱乐部里的申请加入界面
    @usage: WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubApplyMsg_ui_window", false , nil)
]]
require "LuaWindow.Module.UINetTipsCtrl"
--网络提示组件
local m_netTipsCtrl
local UITools = require "Tools.UITools"
clubApplyMsg_ui_window = {}
local self = clubApplyMsg_ui_window
require "LuaWindow.Module.UINetTipsCtrl"
local m_netTipsCtrl = nil
-- 常量
-- 一页数据有多少条
local CONST_COUNTS_PER_PAGE = 10
-- 是否本地测试
local IS_LOCAL_TESTED = false
-- 开始时候和结束时候 重置数据
function clubApplyMsg_ui_window.resetData()
    self.cached_listData = {} -- 当前数据列表
    self.cached_map_page2list = {} -- 所有页码的缓存表
    self.data_diff = 0
    self.data_curPageIndex = 1 -- 当前页码
    self.page_size = CONST_COUNTS_PER_PAGE * 2
    self.all_new_num = 0
    self.group_new_num = 0
    self.category = ENUM_MAIL_CATEGORY_TYPE.ClubApplyJoinNotice --申请入群(俱乐部)通知
end
-- 初始化 第1个执行的
function clubApplyMsg_ui_window.Init(ctx)
    DwDebug.Log("@------------Init")
    self.ctx = ctx -- luawindowRoot
    self.resetData()
end
-- 第2个执行
function clubApplyMsg_ui_window.InitWindow(open, state)
    DwDebug.Log("@------------InitWindow")
    self.ctx:InitCamera(open, false, false, true, -1)
    self.ctx:BaseIniwWindow(open, state)
    self.open = open
    if open then
        self.updateMainUI()
    else
        -- 关闭界面时候 清除数据
        self.resetData()
        LuaEvent.AddEventNow(EEventType.RefreshClubMailRedPoint)
        LuaEvent.AddEventNow(EEventType.RefreshMailUIList)
    end
end
-- 第3个执行 只执行一次
function clubApplyMsg_ui_window.CreateWindow()
    DwDebug.Log("@------------CreateWindow")
end
-- 窗口销毁时候
function clubApplyMsg_ui_window.OnDestroy()
    self.ctx = nil
    self.uiScrollViewObj = nil
    if m_netTipsCtrl then
        m_netTipsCtrl:Destroy()
        m_netTipsCtrl = nil
    end
end
-- 主界面
function clubApplyMsg_ui_window.updateMainUI()
    -- 初始化列表
    self.initListUI()
    -- 请求列表数据 之后 刷新UI
    self.webRequestListDataByPageIndex(self.data_curPageIndex)
end
-- 初始化列表UI
function clubApplyMsg_ui_window.initListUI()
    local ctx = self.ctx
    local listTrans = ctx:GetTrans("list")
    local itemTrans = ctx:GetTrans("apply_item")
    ctx:SetActive(itemTrans, false)
    -- 如果没有列表对象
    if not self.uiScrollViewObj then -- self.uiScrollViewObj 列表对象
        self.uiScrollViewObj = UITools.CreateScrollView(listTrans, itemTrans, 0, 0, 0, 0, 1, false, "clubApplyMsg_ui_window.updateListItem")
        self.uiScrollViewObj.SelectIndex = 0
        self.uiScrollViewObj:SetSelectOrUnSelectFuncByName("clubApplyMsg_ui_window.updateListItem")
    end
    self.uiScrollViewObj:InitItemCount(0, false)
    self.ctx:SetActive(self.ctx:GetTrans("txt_listIsEmpty"), false)
end
-- 请求某一页的数据
function clubApplyMsg_ui_window.webRequestListDataByPageIndex(requset_page_index)
    local function sucCB(body, head)
        -- TimerTaskSys.AddTimerEventByLeftTime(
        --     function()
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
        DwDebug.Log("邮件和俱乐部申请列表红点数据", self.all_new_num, self.group_new_num)
        DataManager.SetIsShowMailRedPoint(self.all_new_num > 0)
        DataManager.SetIsShowClubApplyMsgRedPoint(self.group_new_num > 0)
        LuaEvent.AddEventNow(EEventType.RefreshHallMailBtnRedPoint)
        LuaEvent.AddEventNow(EEventType.RefreshClubMailRedPoint)

        local list_data = body.data.list -- 列表数据
        local page_index = list_data.page_index
        self.page_count = list_data.page_count --记录最新的页码总数
        self.cached_map_page2list[page_index] = list_data
        DwDebug.Log("列表数据内容")
        for k, v in pairs(list_data.items) do
            print(k, v)
        end
        DwDebug.Log("当前页码", self.data_curPageIndex)
        DwDebug.Log("页码总数", self.page_count)
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
        DwDebug.Log("新的数据长度", #self.cached_listData, self.cached_listData)
        DwDebug.Log("是否属于上一页当前页下一页的数据", tostring(is_in_preCurNext))
        if is_in_preCurNext then
            local is_first_update = (self.data_curPageIndex == 1 and #self.cached_listData == self.page_size)
            DwDebug.Log("是否第一次初始化列表", tostring(is_in_preCurNext))
            if self.uiScrollViewObj then
                self.data_diff = math.max(self.data_curPageIndex - 2, 0)
                self.data_diff = self.data_diff * self.page_size
                DwDebug.Log("数据起始偏移量 ", tostring(self.data_diff))
                self.uiScrollViewObj:InitItemCount(self.data_diff + #self.cached_listData, is_first_update) -- 如果是第一页 就为true 并且缓存数据长度为一页的长度
                self.ctx:SetActive(self.ctx:GetTrans("txt_listIsEmpty"), self.data_diff + #self.cached_listData == 0)
            end
            if is_first_update then
                clubApplyMsg_ui_window.PageIndexChangeCallback(1)
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
        DwDebug.Log(" 俱乐部申请加入列表 数据回包 错误 WebNetHelper.RequestMsgList fail callback ")
        m_netTipsCtrl:StopWork(false)
    end
    local function request_fun()
        WebNetHelper.RequestMsgList(self.category, requset_page_index, self.page_size, sucCB, failCB)
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
        if not m_netTipsCtrl then
            m_netTipsCtrl = UINetTipsCtrl.New()
            m_netTipsCtrl:Init(self.ctx, self.ctx:GetTrans("netTipsRoot"), request_fun)
        end
        if #self.cached_listData == 0 then
            m_netTipsCtrl:StartWork()
        else
            request_fun()
            m_netTipsCtrl:StartWork()
        end
    end
end
-- 刷新列表项内容
function clubApplyMsg_ui_window.updateListItem(item_node_tran, item_index)
    item_index = item_index + 1 -- 从1开始
    DwDebug.Log("-------- 刷新列表子项  " .. item_index)
    DwDebug.Log("-------- 刷新列表子项  " .. item_index .. "偏移量" .. self.data_diff, self.cached_listData)
    -- 根据 index 来识别第几个item和名字
    item_node_tran.name = "itemBtn_bg" .. item_index
    -- 当前列表项数据
    local item_data = self.cached_listData[item_index - self.data_diff]
    local group_id = 0
    local group_name = "测试俱乐部"
    local uid = 100039
    local wx_nickname = "pbData 为空"
    local wx_headimgurl = ""
    local addit_msg = "附加信息"
    local wx_sex = 0 --女性
    if IS_LOCAL_TESTED then
        group_id = item_data.group_id
        group_name = item_data.group_name
        uid = item_data.uid
        wx_nickname = item_data.wx_nickname
        wx_headimgurl = item_data.wx_headimgurl
        addit_msg = item_data.addit_msg
        wx_sex = item_data.wx_sex
    else
        if item_data.content then
            local pbData = ProtoManager.Decode(WebEvent.MailGroupApplyJoin, item_data.content)
            if pbData then
                DwDebug.Log("pb数据", pbData)
                group_id = pbData.group_id
                group_name = pbData.group_name
                uid = pbData.uid
                wx_nickname = pbData.wx_nickname
                wx_headimgurl = pbData.wx_headimgurl
                addit_msg = pbData.addit_msg
                wx_sex = pbData.wx_sex
            else
                DwDebug.Log("pbData 为空")
            end
        else
            DwDebug.Log("content为空")
        end
    end
    local ctx = self.ctx
    -- 头像
    WindowUtil.LoadHeadIcon(ctx, ctx:GetTrans(item_node_tran, "img_head"), wx_headimgurl, wx_sex, false, RessStorgeType.RST_Never)
    -- 昵称
    ctx:SetLabel(ctx:GetTrans(item_node_tran, "txt_name"), utf8sub(wx_nickname, 1, 10))
    -- id
    ctx:SetLabel(ctx:GetTrans(item_node_tran, "txt_id"), uid)
    -- 俱乐部名字 申请对象
    ctx:SetLabel(ctx:GetTrans(item_node_tran, "txt_applyTarget"), group_name)
    -- 附加信息
    ctx:SetLabel(ctx:GetTrans(item_node_tran, "txt_applyMsg"), addit_msg)
end
-- 列表上拉回调
function clubApplyMsg_ui_window.requestListDataPrePage()
    self.webRequestListDataByPageIndex(self.data_curPageIndex - 1)
end
-- 列表下拉回调
function clubApplyMsg_ui_window.requestListDataNextPage()
    self.webRequestListDataByPageIndex(self.data_curPageIndex + 1)
end
-- 这里返回的 curPageIndex 是 UI上的当前页数
function clubApplyMsg_ui_window.PageIndexChangeCallback(curPageIndex)
    DwDebug.Log("列表换页回调 现在的UI页码是 " .. curPageIndex)
    if self.last_pageIndex == curPageIndex then
        DwDebug.Log("当前页")
    elseif self.last_pageIndex == curPageIndex - 1 then
        DwDebug.Log("下一页")
    elseif self.last_pageIndex == curPageIndex + 1 then
        DwDebug.Log("上一页")
    else
        DwDebug.Log("error")
    end
    DwDebug.Log("当前数据页码", self.data_curPageIndex)
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
        "clubApplyMsg_ui_window.PageIndexChangeCallback", -- 页码改变通知
        "clubApplyMsg_ui_window.PageUpPullCallback", -- 上拉通知
        "clubApplyMsg_ui_window.PageDownPullCallback" -- 下拉通知
    )
end
-- 按钮事件
function clubApplyMsg_ui_window.HandleWidgetClick(go)
    local str = go.name
    local itemCommonStr = string.sub(go.name, 1, 8) -- itemBtn_
    local itemSpecialStr = string.sub(go.name, 9, -1) -- itemBtn_

    DwDebug.Log("HandleWidgetClick :" .. itemCommonStr)
    DwDebug.Log("HandleWidgetClick :" .. itemSpecialStr)
    DwDebug.Log("点击控件名 :" .. str)
    if self["_on_" .. str] then
        self["_on_" .. str]()
    elseif itemCommonStr == "itemBtn_" then
        DwDebug.Log("item名字")
        DwDebug.Log("item名字" .. go.transform.parent.parent.name)
        local index = tonumber(string.sub(go.transform.parent.parent.name, 11, -1)) -- itemBtn_bg
        self["_on_btn_" .. itemSpecialStr](index) --"itemBtn_bg + itemIndex"
    else
        -- DwDebug.Log("还没定义按钮" .. str .. "的回调")
        if m_netTipsCtrl then
            m_netTipsCtrl:HandleWidgetClick(go)
        end
    end
end
-- 关闭按钮
function clubApplyMsg_ui_window._on_btn_close()
    WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
    self.InitWindow(false, 0)
end
-- 列表item里的回调 需要节点对象反查父节点的节点名字得出对应的数据序号
-- 同意按钮
function clubApplyMsg_ui_window._on_btn_agree(item_index)
    DwDebug.Log("同意回调", item_index)
    local item_data = self.cached_listData[item_index - self.data_diff]
    self._sendResult(item_data, 1, item_index)
end
-- 拒绝按钮
function clubApplyMsg_ui_window._on_btn_reject(item_index)
    DwDebug.Log("拒绝回调", item_index)
    local item_data = self.cached_listData[item_index - self.data_diff]
    self._sendResult(item_data, 2, item_index)
end
-- 发送审核结果
function clubApplyMsg_ui_window._sendResult(item_data, result)
    DwDebug.Log("发送审核结果", result, item_data)
    local group_id = 0
    local group_name = "测试俱乐部"
    local uid = 100039
    local wx_nickname = "pbData 为空"
    local wx_headimgurl = ""
    local addit_msg = "附加信息"
    local wx_sex = 0 --女性
    if IS_LOCAL_TESTED then
        group_id = item_data.group_id
        group_name = item_data.group_name
        uid = item_data.uid
        wx_nickname = item_data.wx_nickname
        wx_headimgurl = item_data.wx_headimgurl
        addit_msg = item_data.addit_msg
        wx_sex = item_data.wx_sex
    else
        if item_data.content then
            local pbData = ProtoManager.Decode(WebEvent.MailGroupApplyJoin, item_data.content)
            if pbData then
                DwDebug.Log("pb数据", pbData)
                group_id = pbData.group_id
                group_name = pbData.group_name
                uid = pbData.uid
                wx_nickname = pbData.wx_nickname
                wx_headimgurl = pbData.wx_headimgurl
                addit_msg = pbData.addit_msg
                wx_sex = pbData.wx_sex
                local function sucCB(body, head)
                    if body.errcode == 0 then
                        DwDebug.Log(" web 返回成功 发送审核结果 ")
                        -- 返回id 从列表中删除了这个item
                        local id = body.data.id
                        for i = 1, #self.cached_listData do
                            if id == self.cached_listData[i].id then
                            -- table.remove(self.cached_listData,i)
                            end
                        end
                        WindowRoot.ShowTips("已经处理")
                        -- WindowRoot.ShowTips("已经处理")
                        self.webRequestListDataByPageIndex(self.data_curPageIndex)
                    end
                end
                local function failCB(body, head)
                    DwDebug.Log(" web 返回失败 发送审核结果  ")
                    if body.errcode == 90203 then
                        -- local id = body.data.id
                        self.webRequestListDataByPageIndex(self.data_curPageIndex)
                    end
                end
                WebNetHelper.RequestHandleApplyClub(group_id, uid, result, item_data.id, sucCB, failCB)
            else
                DwDebug.Log("pbData 为空")
            end
        else
            DwDebug.Log("content为空")
        end
    end
end
-- 单元测试数据
function clubApplyMsg_ui_window.genTest(requset_page_index)
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
                        is_read = false, -- add
                        -- 申请通知特有
                        headUrl = "",
                        wx_sex = 1,
                        nickName = "昵称1",
                        userId = "玩家id",
                        txt_applyTarget = "申请对象",
                        txt_applyMsg = "玩家申请附加信息"
                    }
                }
            }
        }
    }
    -- for j = 1, 3 do
    for i = 1, body.data.list.page_size do
        -- local num = ((j-1)*CONST_COUNTS_PER_PAGE+i)
        local num = i + (body.data.list.page_index - 1) * CONST_COUNTS_PER_PAGE * 2
        local t = {}
        t.category = i % 2 + 1
        t.content = "测试数据 类型 申请消息" .. "第" .. body.data.list.page_index .. "页,第" .. i .. "条"
        t.send_time = "2018年2月2日11:20:41"
        t.event_type = "aaa" .. num
        t.id = i * 10
        t.is_read = false
        t.headUrl = "https://www.baidu.com/link?url=5tUMkYzM4scLsx2gTA0iKL3Rpzeb-3U30IB9nUclEP222opZ47eB1ncgpKQ739wokSfcglRytP_EQCAPhoi7RNMgB_UQzHN5BvpSNmEdCxi&wd=&eqid=9c0b924b0000cc5e000000065a7800e2"
        t.wx_sex = i % 2
        t.wx_nickname = "aaa" .. num
        t.uid = 10000 + i
        t.group_name = "天天不打麻将"
        t.addit_msg = "申请消息附加消息" .. i
        body.data.list.items[i] = t
    end
    return body, head
end
