--[[
@author: mid
@date: 2018年1月8日02:12:40
@desc: 扑克小局结算模板
@usage:
1. 在 Enum_LogicType 中添加对应类型
2. 在 cfg 中添加对应字段
3. 根据2中的对应字段新建对应模块lua文件
4. 参照 crmj_single_settlement 写法
]]
-- 窗口数据类型枚举
local Enum_SubLogicType = {
    crpk = 1, -- 崇仁
    lapk = 2, -- 乐安
    yhpk = 3, -- 宜黄
}
local path = "LuaWindow.SingleSettlement.pk."
local modulePath = {
    [Enum_SubLogicType.crpk] = path .. "crpk_single_settlement",
    [Enum_SubLogicType.lapk] = path .. "lapk_single_settlement",
    [Enum_SubLogicType.yhpk] = path .. "yhpk_single_settlement",
}
--[[文件域全局]]
pk_single_settlement_ui_window = {}
local self = pk_single_settlement_ui_window
-- [[重载]]
-- 初始化
function pk_single_settlement_ui_window.Init(ctx)
    self.ctx = ctx
end
-- @parma logicType 参数说明 int型 /10得到扑克类型 %10得到是否回放(非0就是回放)
-- 初始化窗口
function pk_single_settlement_ui_window.InitWindow(openState, openData)
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
        local uiData = roomObj:GetCurrSmallResult()
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
        self.countDownNum = 30 -- 倒计时
        self.cleanList = self.cleanList or {}
        if not self.isReplayMode then
            UpdateSecond:Add(self._counDownEvent, self)
        end
        self.subModule.Init(self)
        self.InitMainUI()
    else
        local playLogicObj = self.playLogicObj
        if playLogicObj and playLogicObj.OnCloseSmallResult then
            playLogicObj:OnCloseSmallResult()
        end
        self._doCleanList()
        -- end
        self.subLogicType = nil
        self.isReplayMode = nil
        self.subModule = nil
        self.uiData = nil
        self.roomObj = nil
        self.playerMgr = nil
        self.countDownNum = nil
        self.playLogicObj = nil
        if not self.isReplayMode then
            UpdateSecond:Remove(self._counDownEvent, self)
        end
    end
end
-- 窗口创建时候
function pk_single_settlement_ui_window.CreateWindow()
    if DataManager.isPrePublish then
        if self.ctx then
            self.ctx:SetActive(self.ctx:GetTrans("btn_share"), false)
        end
    end
    self.m_TimeValue = WrapSys.GetTimeValue()
    LuaEvent.AddHandle(EEventType.ScreenShotEnd, self.ScreenShotEnd)
end
-- 窗口销毁时候
function pk_single_settlement_ui_window.OnDestroy()
    self.m_TimeValue = nil
    self.ctx = nil
    LuaEvent.RemoveHandle(EEventType.ScreenShotEnd, self.ScreenShotEnd)
end
-- 截屏事件
function pk_single_settlement_ui_window.ScreenShotEnd()
    local ctx = self.ctx
    if ctx then
        ctx:SetActive(ctx:GetTrans("node_hideWhenSharing"), true)
    end
end
-- 倒计时事件
function pk_single_settlement_ui_window._counDownEvent()
    local countDownNum = self.countDownNum
    if countDownNum > 0 then
        countDownNum = countDownNum - 1
        if countDownNum <= 0 then
            self._on_btn_close()
        end
        self.ctx:SetLabel(self.ctx:GetTrans("txt_restSeconds"), countDownNum)
        self.countDownNum = countDownNum
    end
end
-- 主界面
function pk_single_settlement_ui_window.InitMainUI()
    local uiData = self.uiData
    local ctx = self.ctx
    local roomObj = self.roomObj
    local isReplayMode = self.isReplayMode
    ctx:SetLabel(ctx:GetTrans("txt_roomId"), uiData.id)
    ctx:SetLabel(ctx:GetTrans("txt_settleTime"), uiData.time)
    ctx:SetLabel(ctx:GetTrans("txt_gameName"), uiData.name)
    -- DwDebug.Log(string.gsub(uiData.playDes, " ", "\n"))
    -- DwDebug.Log(ctx:GetTrans("txt_gameRule"))
    -- ctx:SetLabel(ctx:GetTrans("txt_gameRule"), string.gsub(uiData.playDes, " ", "\n"))
    -- uiData.playDes = string.gsub(uiData.playDes, " ", "\n") -- 会很多行
    ctx:SetLabel(ctx:GetTrans("txt_gameRule"), uiData.doc)
    ctx:SetLabel(ctx:GetTrans("txt_roundProgress"), uiData.now .. "/" .. uiData.total .. "r")
    ctx:SetActive(ctx:GetTrans("txt_restSeconds"), not self.isReplayMode)
    ctx:SetActive(ctx:GetTrans("btn_close"), not self.isReplayMode)
    local is_show_total = roomObj:GetBigResult() and true or false
    ctx:SetActive(ctx:GetTrans("btn_totalSettlement"), is_show_total)
    local is_show_back = not PlayRecordSys.CheckNextRecordReplayInfo() and self.isReplayMode
    ctx:SetActive(ctx:GetTrans("btn_back"), is_show_back)
    ctx:SetActive(ctx:GetTrans("btn_continue"), not (is_show_back or is_show_total))
    -- 隐藏所有列表项
    local items = ctx:GetTrans("items")
    for i=1,items.childCount do
        DwDebug.Log(items:GetChild(i-1))
        ctx:SetActive(items:GetChild(i-1),false)
    end
    -- 刷新列表
    self.subModule.InitList(self)
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
function pk_single_settlement_ui_window.HandleWidgetClick(go)
    local str = go.name
    if self["_on_" .. str] then
        self["_on_" .. str]()
    else
        DwDebug.Log("还没定义按钮" .. str .. "的回调")
    end
end
-- 分享截屏
function pk_single_settlement_ui_window._on_btn_share()
    if self.m_TimeValue.Value > 0 then
        WindowUtil.LuaShowTips("您点击太快,请稍后再试")
        return
    end
    self.m_TimeValue.Value = 5
    self.ctx:SetActive(self.ctx:GetTrans("node_hideWhenSharing"), false)
    HallSys.ShareTotalSettlement(false)
end
-- 返回按钮
function pk_single_settlement_ui_window._on_btn_back()
    PlayGameSys.QuitToMainCity()
end
-- 关闭按钮
function pk_single_settlement_ui_window._on_btn_close()
    self._goToTotalSettlement()
    self._normalCloseLogic()
end
-- 继续按钮
function pk_single_settlement_ui_window._on_btn_continue()
    --是否回放
    if not self.isReplayMode then
        self.playLogicObj:SendPrepare(0)-- 发送准备
        self._normalCloseLogic()
    else
        self._normalCloseLogic()
        LuaEvent.AddEventNow(EEventType.RecordPlayNextRound)-- 刷新轮次
    end
end
-- 总结算按钮
function pk_single_settlement_ui_window._on_btn_totalSettlement()
    self._goToTotalSettlement()
    self.InitWindow(false, 0)
end
-- [[子逻辑函数]]
-- 关闭房间相关逻辑
-- 刷新积分 更新对战流水 更新状态
function pk_single_settlement_ui_window._normalCloseLogic()
    local uiData = self.uiData
    local players = uiData.players
    local total = uiData.total
    local next_pos = uiData.next
    for i = 1, #players do
        local player = players[i]
        if next(player) ~= nil then
            LuaEvent.AddEvent(EEventType.RefreshPlayerTotalScore, player.seatPos, player.totalScore)
        end
    end

    --LuaEvent.AddEvent(EEventType.RefreshRoomRoundNum,next_pos,total)
    local roomObj = self.roomObj
    roomObj.pointMgr:ClearForNewRound()
    roomObj:ChangeState(RoomStateEnum.Idle)

    roomObj.roomInfo.currentGamepos = next_pos
    
    self.InitWindow(false, 0)
end
-- 状态机切换
function pk_single_settlement_ui_window._goToTotalSettlement()
    local roomObj = self.roomObj
    if roomObj:GetBigResult() then
        roomObj:ChangeState(RoomStateEnum.GameOver)
    else
        if self.uiData.now >= self.uiData.total then
            WindowUtil.LuaShowTips("总结算信息准备中,请稍候...")
        end
    end
end
-- 添加到清理列表中去
function pk_single_settlement_ui_window.AddToCleanList(trans_table)
    local list = self.cleanList
    for i = 1, #trans_table do
        list[#list + 1] = trans_table[i]
    end
end
-- 清理生成的麻将节点
function pk_single_settlement_ui_window._doCleanList()
    local list = self.cleanList
    local ctx = self.ctx
    if not list then return end
    for i = 1, #list do
        ctx:SetActive(list[i], false)
    end
end
