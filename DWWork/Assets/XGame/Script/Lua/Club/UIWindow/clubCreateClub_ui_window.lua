--[[
	@author: mid
	@date: 2018年1月30日11:08:19
    @desc: 创建俱乐部界面 填写信息
    @usage: WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubCreateClub_ui_window", false , nil)
]]
clubCreateClub_ui_window = {}
local self = clubCreateClub_ui_window
-- 是否本地测试
local IS_LOCAL_TESTED = true
-- 重置数据
function clubCreateClub_ui_window.resetData()
    self.cache_str_clubName = ""
    self.cache_str_clubAdminWeChatId = ""
    self.cache_str_clubAdminPhoneNumber = ""
    self.cache_clubHeadImageIndex = 1 -- clubData_headImg_1-4
end
function clubCreateClub_ui_window.Init(ctx)
    self.ctx = ctx -- luawindowRoot
    self.resetData()
end
function clubCreateClub_ui_window.InitWindow(open, state, data)
    self.ctx:InitCamera(open, false, false, true, -1)
    self.ctx:BaseIniwWindow(open, state)
    if open then
        self.cache_clubHeadImageIndex = 1
        self.updateMainUI()
        UpdateBeat:Add(self.checkTextState)
        LuaEvent.AddHandle(EEventType.UI_CreateClubChangeHead, self.updateHeadImg)
    else
        -- 关闭界面时候 清除数据
        self.resetData()

        self.input_clubName.value = ""
        self.input_clubAdminWeChatId.value = ""
        self.input_clubAdminPhoneNumber.value = ""

        UpdateBeat:Remove(self.checkTextState)
        LuaEvent.RemoveHandle(EEventType.UI_CreateClubChangeHead, self.updateHeadImg)
    end
end
-- 窗口创建时候
function clubCreateClub_ui_window.CreateWindow()
end
-- 窗口销毁时候
function clubCreateClub_ui_window.OnDestroy()
    self.ctx = nil
end
-- 主界面
function clubCreateClub_ui_window.updateMainUI()
    local ctx = self.ctx
    -- 俱乐部名字
    self.input_clubName = ctx:GetTrans("input_clubName"):GetComponent("UIInput")
    -- self.txt_clubName = ctx:GetTrans("txt_clubName")
    self.hintTxt_clubName = ctx:GetTrans("hintTxt_clubName")
    -- 俱乐部部长微信号
    self.input_clubAdminWeChatId = ctx:GetTrans("input_clubAdminWeChatId"):GetComponent("UIInput")
    -- self.txt_clubAdminWeChatId = ctx:GetTrans("txt_clubAdminWeChatId")
    self.hintTxt_clubAdminWeChatId = ctx:GetTrans("hintTxt_clubAdminWeChatId")
    -- 俱乐部部长手机号
    self.input_clubAdminPhoneNumber = ctx:GetTrans("input_clubAdminPhoneNumber"):GetComponent("UIInput")
    -- self.txt_clubAdminPhoneNumber = ctx:GetTrans("txt_clubAdminPhoneNumber")
    self.hintTxt_clubAdminPhoneNumber = ctx:GetTrans("hintTxt_clubAdminPhoneNumber")
    -- -- 刷新缓存文本状态
    -- ctx:SetLabel(self.txt_clubName, self.input_clubName.value)
    -- ctx:SetLabel(self.txt_clubAdminWeChatId, self.input_clubAdminWeChatId.value)
    -- ctx:SetLabel(self.txt_clubAdminPhoneNumber, self.input_clubAdminPhoneNumber.value)
    self.updateHeadImg()
end
function clubCreateClub_ui_window.updateHintTxtState()
    -- 刷新提醒文本状态
    local ctx = self.ctx
    ctx:SetActive(self.hintTxt_clubName, self.input_clubName.value == "")
    ctx:SetActive(self.hintTxt_clubAdminWeChatId, self.input_clubAdminWeChatId.value == "")
    ctx:SetActive(self.hintTxt_clubAdminPhoneNumber, self.input_clubAdminPhoneNumber.value == "")
end
-- 按钮事件
function clubCreateClub_ui_window.HandleWidgetClick(go)
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
function clubCreateClub_ui_window._on_btn_close()
    WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
    self.InitWindow(false, 0)
    -- 打开创建俱乐部界面
    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubCreateJoin_ui_window", false, 1)
end
-- 创建俱乐部
function clubCreateClub_ui_window._on_btn_createClub(go)
    local function sucCB(body, head)
        DwDebug.Log("创建俱乐部 web 返回 成功 数据为", body)
        self.InitWindow(false, 0)
        if WrapSys.EZFunWindowMgr_CheckWindowOpen(EZFunWindowEnum.luaWindow, "Club.UIWindow.hall.club_hall_ui_window") then
            LuaEvent.AddEvent(EEventType.RefreshClubHallWindow, body and body.data and body.data.id)
        else
            -- WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 2, "Club.UIWindow.hall.club_hall_ui_window", false, nil)
        end
        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, body.data.id, "Club.UIWindow.clubgame_ui_window", false , nil)
    end
    local function failCB(body, head)
        DwDebug.Log("创建俱乐部 web 返回 失败 数据为", body)
    end

    local SensitiveWordUtils = require "Global.SensitiveWordUtils"
    local str_clubName = self.input_clubName.value
    -- local str_clubAdminWeChatId = self.input_clubAdminWeChatId.value
    -- local str_clubAdminPhoneNumber = self.input_clubAdminPhoneNumber.value
    local is_clubName_ok = SensitiveWordUtils:checkString(str_clubName)

    if not is_clubName_ok then
        WindowRoot.ShowTips("俱乐部名称含有敏感字眼，请更改")
    end

    -- local is_clubWeChatid_ok = SensitiveWordUtils:checkString(tostring(str_clubAdminWeChatId))
    -- local is_phoneNumber_ok = string.match(tostring(str_clubAdminPhoneNumber), "[1][3,4,5,7,8]%d%d%d%d%d%d%d%d%d")
    -- DwDebug.Log(tostring(self.input_clubName.value),tostring(self.input_clubAdminWeChatId.value))
    -- DwDebug.Log(is_clubName_ok,is_clubWeChatid_ok,is_phoneNumber_ok)

    -- is_clubName_ok = is_clubName_ok and (utf8len(str_clubName) <= 10 and utf8len(str_clubName) > 0)
    -- is_clubWeChatid_ok = is_clubWeChatid_ok and (string.match(tostring(str_clubAdminWeChatId), "[a-zA-Z0-9_]+$") == str_clubAdminWeChatId)
    -- is_clubWeChatid_ok = is_clubWeChatid_ok and (string.len(str_clubAdminWeChatId)>=5 and string.len(str_clubAdminWeChatId) <=19)
    -- local results = {}
    -- if not is_clubName_ok then
    --     results[#results + 1] = "俱乐部名"
    -- end
    -- if not is_clubWeChatid_ok then
    --     results[#results + 1] = "俱乐部微信号"
    -- end
    -- if not is_phoneNumber_ok then
    --     results[#results + 1] = "电话号"
    -- end
    -- local result_str = ""
    -- for i = 1, #results do
    --     if i ~= #results then
    --         result_str = result_str .. results[i] .. ", "
    --     else
    --         result_str = result_str .. results[i]
    --     end
    -- end
    -- if result_str ~= "" then
    --     WindowRoot.ShowTips(result_str .. "  不合法")
    --     return
    -- end

    -- is_clubName_ok = (string.gsub(str_clubName , "^%s*(.-)%s*$", "%1"))
    -- is_clubWeChatid_ok = (string.gsub(str_clubAdminWeChatId , "^%s*(.-)%s*$", "%1"))
    -- is_phoneNumber_ok = (string.gsub(str_clubAdminPhoneNumber , "^%s*(.-)%s*$", "%1"))

    DwDebug.Log(" 创建俱乐部 发送的参数 ", self.input_clubName.value, self.cache_clubHeadImageIndex, self.input_clubAdminWeChatId.value, self.input_clubAdminPhoneNumber.value)
    ClubSys.CreateClub(self.input_clubName.value, self.cache_clubHeadImageIndex, self.input_clubAdminWeChatId.value, self.input_clubAdminPhoneNumber.value, sucCB, failCB)
end
-- 更改俱乐部头像
function clubCreateClub_ui_window._on_btn_changeHead(go)
    WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubChangeHeadImage_ui_window", false, nil)
end
-- 检测是否有文本改变 是否显示输入提醒文本
function clubCreateClub_ui_window.checkTextState()
    self.updateHintTxtState()
end
-- 更新头像
function clubCreateClub_ui_window.updateHeadImg(event_id, index)
    local ctx = self.ctx
    if index then
        self.cache_clubHeadImageIndex = index
    end
    -- DwDebug.Log("-------------------------- 测试 更新头像 "..tostring(index))
    ctx:SetSprite(ctx:GetTrans("img_clubHead"), "clubData_headImg_" .. self.cache_clubHeadImageIndex)
end
