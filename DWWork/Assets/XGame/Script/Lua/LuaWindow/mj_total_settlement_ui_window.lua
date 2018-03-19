--[[
@author: mid
@date: 2018年1月8日12:03:16
@desc: 总结算模板
@usage:
1. 在 Enum_SubLogicType 中添加对应类型
2. 在 modulePath 中添加对应字段
3. 根据2中的对应字段新建对应模块lua文件
4. 参照 crmj_total_settlement 写法
]]
-- 窗口数据类型枚举
local Enum_SubLogicType = {
    CRMJ = 1, -- 崇仁麻将
    LAMJ = 2, -- 乐安麻将
    YHMJ = 3, -- 宜黄麻将
}
local path = "LuaWindow.TotalSettlement.mj."
local modulePath = {
    [Enum_SubLogicType.CRMJ] = path .. "crmj_total_settlement",
    [Enum_SubLogicType.LAMJ] = path .. "lamj_total_settlement",
    [Enum_SubLogicType.YHMJ] = path .. "yhmj_total_settlement",
}
--[[文件域全局]]
mj_total_settlement_ui_window = {}
local self = mj_total_settlement_ui_window
-- [[重载]]
-- 初始化部分
function mj_total_settlement_ui_window.Init(ctx)
    self.ctx = ctx
end
-- 窗口初始化-- @parma logicType 参数说明 int型 /10得到扑克类型 %10得到是否回放(非0就是回放)
function mj_total_settlement_ui_window.InitWindow(openState, openData)
    -- 初始化局部变量
    local ctx = self.ctx
    ctx:InitCamera(openState, false, false, true, -1)
    ctx:BaseIniwWindow(openState, openData)
    if openState then
        local playLogicObj = PlayGameSys.GetPlayLogic()
        if not playLogicObj then
            DwDebug.Log("not playLogic")
            return
        end
        local roomObj = playLogicObj.roomObj
        if not roomObj then
            DwDebug.Log("not roomObj")
            return
        end
        local uiData = roomObj:GetBigResult()
        if not uiData then
            DwDebug.Log("not uiData")
            return
        end
        -- 记得置空 self开头的
        self.subLogicType, self.isReplayMode = math.floor(openData / 10), (not (openData % 10 == 0))
        self.subModule = require(modulePath[self.subLogicType])
        if not self.subModule then
            DwDebug.Log("not define subModule")
            return
        end
        self.uiData = uiData
        self.roomObj = roomObj
        self.playerMgr = roomObj.playerMgr
        self.playLogicObj = playLogicObj
        self.cleanList = self.cleanList or {}
        self.subModule.Init(self)
        self.InitMainUI()
    else
        local playLogicObj = self.playLogicObj
        if playLogicObj and playLogicObj.OnCloseSmallResult then
            playLogicObj:OnCloseSmallResult()
        end
        self.cleanSubModule()
        -- end
        self.subLogicType = nil
        self.isReplayMode = nil
        self.subModule = nil
        self.uiData = nil
        self.roomObj = nil
        self.playerMgr = nil
        self.playLogicObj = nil
    end
end
-- 窗口创建时候
function mj_total_settlement_ui_window.CreateWindow()
    if DataManager.isPrePublish then
        if self.ctx then
            self.ctx:SetActive(self.ctx:GetTrans("btn_share"), false)
        end
    end
    self.m_TimeValue = WrapSys.GetTimeValue()
    LuaEvent.AddHandle(EEventType.ScreenShotEnd, self.ScreenShotEnd)

    self.ctx:LoadImag(self.ctx:GetTrans("QrCode"), LoginSys.QRPicPath, "", false, RessStorgeType.RST_Never)
end
-- 窗口销毁时候
function mj_total_settlement_ui_window.OnDestroy()
    self.m_TimeValue = nil
    self.ctx = nil
    LuaEvent.RemoveHandle(EEventType.ScreenShotEnd, self.ScreenShotEnd)
end
-- 初始化界面
function mj_total_settlement_ui_window.InitMainUI()
    local uiData = self.uiData
    local ctx = self.ctx
    local roomObj = self.roomObj
    local isReplayMode = self.isReplayMode
    ctx:SetLabel(ctx:GetTrans("txt_roomId"), uiData.roomId)
    ctx:SetLabel(ctx:GetTrans("txt_time"), uiData.settlementTime)
    DwDebug.Log("总结算时间  "..tostring(uiData.settlementTime))
    ctx:SetLabel(ctx:GetTrans("txt_gameName"), uiData.roomName)

     --显示局数(其他地方数据不准确 先从小结算拿吧) 用roomInfo里面的 在小局结算的时候 就+1了
    if self.roomObj and self.roomObj:GetCurrSmallResult() then
        local smallResult = self.roomObj:GetCurrSmallResult()
        if smallResult.currentGamepos and smallResult.totalGameNum then
            ctx:SetLabel(ctx:GetTrans("txt_roomRound"), "局数:"..smallResult.currentGamepos .."/"..smallResult.totalGameNum)
        end
    end

    ctx:SetLabel(ctx:GetTrans("txt_gameRule"), uiData.playDes)
    ctx:SetActive(ctx:GetTrans("btn_close"), not self.isReplayMode)
    -- 刷新列表
    self.subModule.InitList(self)
    -- 分享模块可视状态
    self._setShareViewVisible(true)
end
-- [[事件]]
-- 事件回调 截屏结束
function mj_total_settlement_ui_window.ScreenShotEnd()
    self._setShareViewVisible(true)
end
--[[
按钮事件:
share_btn 分享按钮
back_btn  返回按钮
close_btn 关闭按钮
continue_btn 继续按钮
settlement_btn 总结算按钮
]]
-- 按钮事件分发
function mj_total_settlement_ui_window.HandleWidgetClick(go)
    local str = go.name
    if self["_on_" .. str] then
        self["_on_" .. str]()
    else
        DwDebug.Log("还没定义按钮" .. str .. "的回调")
    end
end
-- 分享截屏
function mj_total_settlement_ui_window._on_btn_share()
    if self.m_TimeValue.Value > 0 then
        WindowUtil.LuaShowTips("您点击太快,请稍后再试")
        return
    end
    self.m_TimeValue.Value = 5
    self._setShareViewVisible(false)
    -- 调用大厅系统中的分享接口 分享接口可能要独立抽出来 平台模块
    HallSys.ShareTotalSettlement(false)
end
-- 关闭逻辑
local function _close_logic()
    PlayGameSys.QuitToMainCity()
    self.InitWindow(false,0)
end
-- 返回按钮
function mj_total_settlement_ui_window._on_btn_back()
    _close_logic()
end
-- 关闭按钮
function mj_total_settlement_ui_window._on_btn_close()
    _close_logic()
end
-- 分享模块可视状态
function mj_total_settlement_ui_window._setShareViewVisible(isNormalShow)
    local ctx = self.ctx
    if ctx then
        ctx:SetActive(ctx:GetTrans("node_hideWhenSharing"),isNormalShow)
        ctx:SetActive(ctx:GetTrans("node_showWhenSharing"),not isNormalShow)
    end
end
-- 添加到清理函数
-- function mj_total_settlement_ui_window.AddToClean(fun)
--     local list = self._cleanList
--     for i = 1, #list do
--         list[#list + 1] = fun
--     end
-- end
-- 清理生成的麻将节点
function mj_total_settlement_ui_window.cleanSubModule()
    local subModule = self.subModule
    if subModule then
        if subModule.clean then
            subModule.clean()
        end
    end
end