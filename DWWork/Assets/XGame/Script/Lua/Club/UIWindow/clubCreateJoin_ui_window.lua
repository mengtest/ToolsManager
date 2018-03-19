--[[
	@author: mid
	@date: 2018年1月30日11:08:19
	@desc: 创建和加入俱乐部界面
	@usage:
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubCreateJoin_ui_window", false , 1)
]]
clubCreateJoin_ui_window = {}
local self = clubCreateJoin_ui_window
require "LuaWindow.Module.UINetTipsCtrl"
local m_netTipsCtrl = nil
-- 是否本地测试
local IS_LOCAL_TESTED = true
local enum_tab = {
    enum_tab_create = 1,
    enum_tab_join = 2
}
local cfg_tabUI = {
    [enum_tab.enum_tab_create] = "create_panel",
    [enum_tab.enum_tab_join] = "join_panel"
}
-- 重置数据
function clubCreateJoin_ui_window.resetData()
    self.tab_index = enum_tab.enum_tab_create
    self.cache_str = ""
    -- self.ctx:GetTrans("msgInput"):GetComponent("UIInput").value = "最多输入20个汉字"
end
function clubCreateJoin_ui_window.Init(ctx)
    self.ctx = ctx -- luawindowRoot
    self.resetData()
end
function clubCreateJoin_ui_window.InitWindow(open, state, data)
    self.ctx:InitCamera(open, false, false, true, -1)
    self.ctx:BaseIniwWindow(open, state)
    if open then
        self.tab_index = data
        self.updateMainUI()
        UpdateBeat:Add(self.checkTextState)
    else
        -- 关闭界面时候 清除数据
        self.resetData()
        UpdateBeat:Remove(self.checkTextState)
    end
end
function clubCreateJoin_ui_window.checkTextState()
    -- DwDebug.Log("checkTextState",tostring(self.input_msg_cmpt.value))
    self.ctx:SetActive(self.hintTxt_sendApplyMsg, tostring(self.input_msg_cmpt.value) == "")
end
-- 窗口创建时候
function clubCreateJoin_ui_window.CreateWindow()
end
-- 窗口销毁时候
function clubCreateJoin_ui_window.OnDestroy()
    self.ctx = nil
    if m_netTipsCtrl then
        m_netTipsCtrl:Destroy()
        m_netTipsCtrl = nil
    end
end
-- 主界面
function clubCreateJoin_ui_window.updateMainUI()
    -- self.default_str = "最多输入20个汉字"
    self.input_msg_cmpt = self.ctx:GetTrans("msgInput"):GetComponent("UIInput")
    -- self.input_msg_cmpt.value = ""
    self.hintTxt_sendApplyMsg = self.ctx:GetTrans("hintTxt_sendApplyMsg")
    self.UpdateSubUI()
end
-- 更新子界面
function clubCreateJoin_ui_window.UpdateSubUI()
    local ctx = self.ctx
    for enum, cfg in pairs(cfg_tabUI) do
        local is_show_panel = (enum == self.tab_index)
        ctx:SetActive(ctx:GetTrans(cfg), is_show_panel)
        if is_show_panel then
            self.cur_panel = ctx:GetTrans(cfg)
            if enum == enum_tab.enum_tab_create then
                self.updateCreateUI()
            elseif enum == enum_tab.enum_tab_join then
                self.updateJoinUI()
            end
        end
    end
end
-- 刷新 创建俱乐部界面
function clubCreateJoin_ui_window.updateCreateUI()
end
-- 刷新 加入俱乐部界面
function clubCreateJoin_ui_window.updateJoinUI()
    local ctx = self.ctx
    -- 隐藏加入俱乐部申请消息界面
    ctx:SetActive(ctx:GetTrans(self.cur_panel, "applyJoinMsg_panel"), false)
    self.UpdateInputUI()
end
-- 更新输入数据
function clubCreateJoin_ui_window.UpdateInputUI()
    for i = 1, 6 do
        self.ctx:SetLabel(self.ctx:GetTrans("txt_showStr_" .. i), string.sub(self.cache_str, i, i) or "")
    end
end
-- 按钮事件
function clubCreateJoin_ui_window.HandleWidgetClick(go)
    local str = go.name
    if self["_on_" .. str] then
        self["_on_" .. str]()
    elseif string.find(str, "key_") then
        WrapSys.AudioSys_PlayEffect("Common/UI/uiclick")
        self._onkeyEvent(str)
    else
        DwDebug.Log("还没定义按钮" .. str .. "的回调")
    end
end
-- 关闭按钮
function clubCreateJoin_ui_window._on_btn_close()
    WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
    self.InitWindow(false, 0)
end
-- 关闭加入信息面板
function clubCreateJoin_ui_window._on_btn_closePanel()
    WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
    self.ctx:SetActive(self.ctx:GetTrans(self.cur_panel, "applyJoinMsg_panel"), false)
    -- self.ctx:GetTrans("msgInput"):GetComponent("UIInput").value = "最多输入20个汉字"
end
-- 页签按钮 展示加入界面
function clubCreateJoin_ui_window._on_tabBtn_join(go)
    self.tab_index = enum_tab.enum_tab_join
    self.UpdateSubUI()
end
-- 页签按钮 展示创建界面
function clubCreateJoin_ui_window._on_tabBtn_create(go)
    self.tab_index = enum_tab.enum_tab_create
    self.UpdateSubUI()
end
-- 创建俱乐部按钮
function clubCreateJoin_ui_window._on_btn_createClub(go)
    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubCreateClub_ui_window", false, nil)
    -- WebNetHelper.RequestCreateClub(name,g_img,owner_weixin,owner_mobile,sucCB,failCB)
    self.InitWindow(false, 0)
end
-- 键盘事件
function clubCreateJoin_ui_window._onkeyEvent(click_name)
    local str = string.sub(click_name, 5, -1)
    local cache_str = self.cache_str
    if str == "del" then -- 删除
        if #cache_str > 0 then
            cache_str = string.sub(cache_str, 1, -2)
        end
    elseif str == "cls" then -- 清空
        cache_str = ""
    else
        -- 增加数量限制 不超过8个
        if #cache_str < 6 then
            cache_str = cache_str .. str
            if #cache_str == 6 then
                DwDebug.Log("------ 检测俱乐部号码是否合法 申请加入id为 " .. cache_str .. "的俱乐部")
                self._tryShowApplyJoinMsgPanel(cache_str)
            end
        end
    end
    self.cache_str = cache_str
    self.UpdateInputUI()
end
-- 发送申请消息
function clubCreateJoin_ui_window._on_btn_sendApplyMsg()
    local applyMsgStr = self.ctx:GetTrans("msgInput"):GetComponent("UIInput").value
    -- if applyMsgStr == self.default_str then
    -- applyMsgStr = ""
    -- end
    DwDebug.Log("发送申请消息 当前的消息文本为 " .. applyMsgStr .. " 俱乐部id为 " .. self.cache_str)
    local function sucCB(body, head)
        DwDebug.Log("发送申请消息 web 返回 成功 数据为", body)
        WindowRoot.ShowTips("已成功发送申请消息")
        self.ctx:SetActive(self.ctx:GetTrans(self.cur_panel, "applyJoinMsg_panel"), false)
    end
    local function failCB(body, head)
        DwDebug.Log("发送申请消息 web 返回 失败 数据为", body)
        self.ctx:SetActive(self.ctx:GetTrans(self.cur_panel, "applyJoinMsg_panel"), false)
    end
    WebNetHelper.RequestApplyJoinClub(tonumber(self.cache_str), 0, applyMsgStr, sucCB, failCB)
end
-- 打开输入界面
function clubCreateJoin_ui_window._tryShowApplyJoinMsgPanel(cache_str)
    DwDebug.Log("检测俱乐部号码是否合法 俱乐部id为 " .. cache_str)
    self.cache_str = cache_str
    local function sucCB(body, head)
        DwDebug.Log("检测俱乐部号码是否合法 web 返回 成功 数据为", body)
        if body.errcode == 0 then
            self.ctx:SetActive(self.ctx:GetTrans(self.cur_panel, "applyJoinMsg_panel"), true)
        end
        -- m_netTipsCtrl:StopWork(true)
    end
    local function failCB(body, head)
        DwDebug.Log("检测俱乐部号码是否合法 web 返回 失败 数据为", body)
        -- m_netTipsCtrl:StopWork(true)

    end

    local function request_fun()
        WebNetHelper.RequestApplyJoinClub(tonumber(self.cache_str), 1, "", sucCB, failCB)
    end

    request_fun()
    -- if not m_netTipsCtrl then
    --     m_netTipsCtrl = UINetTipsCtrl.New()
    --     m_netTipsCtrl:Init(self.ctx, self.ctx:GetTrans("netTipsRoot"), request_fun)
    -- end
    -- m_netTipsCtrl:StartWork()
end
