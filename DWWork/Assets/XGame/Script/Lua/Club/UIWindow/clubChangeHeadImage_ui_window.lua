--[[
	@author: mid
	@date: 2018年1月30日11:08:19
    @desc: 修改俱乐部头像界面
    @usage: WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubChangeHeadImage_ui_window", false , nil)
]]
clubChangeHeadImage_ui_window = {}
local self = clubChangeHeadImage_ui_window
-- 是否本地测试
local IS_LOCAL_TESTED = true
-- 重置数据
function clubChangeHeadImage_ui_window.resetData()
    self.selected_index = 0
    self.data_clubInfo = nil
end
function clubChangeHeadImage_ui_window.Init(ctx)
    self.ctx = ctx -- luawindowRoot
    self.resetData()
end
--[[
local data = {
    ["id"] = 100001,
    ["name"] = "测试_俱乐部名称"
    ["g_img"] = 1,
    ["owner_weixin"] = "测试_微信号",
    ["owner_mobile"] = "手机号",
}
]]
function clubChangeHeadImage_ui_window.InitWindow(open, state, data)
    self.ctx:InitCamera(open, false, false, true, -1)
    self.ctx:BaseIniwWindow(open, state)
    if open then
        DwDebug.Log(" -------------  clubChangeHeadImage_ui_window ")
        DwDebug.Log(data)
        self.data_clubInfo = data
        if data then
            self.selected_index = data.g_img
        else
            self.selected_index = 1
        end
        self.updateMainUI()
    else
        -- 关闭界面时候 清除数据
        self.resetData()
    end
end
-- 窗口创建时候
function clubChangeHeadImage_ui_window.CreateWindow()
end
-- 窗口销毁时候
function clubChangeHeadImage_ui_window.OnDestroy()
    self.ctx = nil
end
-- 主界面
function clubChangeHeadImage_ui_window.updateMainUI()
    local ctx = self.ctx
    for i = 1, 4 do
        local is_selected = (i == self.selected_index)
        ctx:SetActive(ctx:GetTrans(ctx:GetTrans("btn_clubHead_" .. i), "selected"), is_selected)
    end
end
-- 按钮事件
function clubChangeHeadImage_ui_window.HandleWidgetClick(go)
    local str = go.name
    if self["_on_" .. str] then
        WrapSys.AudioSys_PlayEffect("Common/UI/uiclick")
        self["_on_" .. str]()
    elseif string.find(str, "btn_clubHead_") then
        WrapSys.AudioSys_PlayEffect("Common/UI/uiclick")
        self._changeClubHead(string.sub(str, 14, -1))
    else
        DwDebug.Log("还没定义按钮" .. str .. "的回调")
    end
end
-- 关闭按钮
function clubChangeHeadImage_ui_window._on_btn_close()
    WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
    self.InitWindow(false, 0)
end
-- 确定按钮
function clubChangeHeadImage_ui_window._on_btn_ok()
    local data = self.data_clubInfo
    DwDebug.Log("_on_btn_ok")
    DwDebug.Log(data)
    if not data then
        LuaEvent.AddEventNow(EEventType.UI_CreateClubChangeHead, self.selected_index) -- 通知创建界面修改UI
    else
        local function sucCB(body, head)
            if body.errcode == 0 then
                DwDebug.Log("更改头像 web 返回 成功 ,结果成功 数据为", body)
                WindowRoot.ShowTips("更换头像成功")

                LuaEvent.AddEventNow(EEventType.UI_ChangeClubData) -- 通知大厅修改UI
                LuaEvent.AddEventNow(EEventType.UI_RefreshClubList)
            else
                DwDebug.Log("更改头像 web 返回 成功 ,结果失败,数据为", body.errmsg)
            end
        end
        local function failCB(body, head)
            DwDebug.Log("更改头像 web 返回 失败 数据为", body)
        end
        -- self.selected_index
        ClubSys.UpdateClub(data.id, data.name, self.selected_index, data.owner_weixin, data.owner_mobile, sucCB, failCB)
    end

    self.InitWindow(false, 0)
end
-- 修改俱乐部头像选中状态
function clubChangeHeadImage_ui_window._changeClubHead(index)
    DwDebug.Log("更改头像为 index = " .. index)
    self.selected_index = tonumber(index)
    self.updateMainUI()
end
