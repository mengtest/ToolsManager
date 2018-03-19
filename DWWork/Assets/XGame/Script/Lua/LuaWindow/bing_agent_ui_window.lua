require "LuaWindow.Module.UINetTipsCtrl"

--网络提示组件
local m_netTipsCtrl

bing_agent_ui_window = {}
local self = bing_agent_ui_window
function bing_agent_ui_window.TryOpenWindow()
    -- WebNetHelper.RequestCheckAgent(
    --     function(body, head)
    --         WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_Window, true, 1, "bing_agent_ui_window", body)
    --     end,
    --     function(body, head)
    --         DwDebug.Log("WebNetHelper RequestCheckAgent failed")
    --     end
    -- )
    WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_Window, true, 1, "bing_agent_ui_window")
end
function bing_agent_ui_window.Init(p_ctx)
    self.ctx = p_ctx
    self.cache_str = ""
end
function bing_agent_ui_window.InitWindow(open, state, params)
    local ctx = self.ctx
    ctx:InitCamera(open, false, false, true, -1)
    ctx:BaseIniwWindow(open, state)
    if open then
        self.InitWindowDetial()
    else
        self.cache_str = ""
    end
end

function bing_agent_ui_window.CreateWindow()
    m_netTipsCtrl = UINetTipsCtrl.New()
    m_netTipsCtrl:Init(
        self.ctx,
        self.ctx:GetTrans("netTipsRoot"),
        function()
            WebNetHelper.RequestCheckAgent(
                function(body, head)
                    -- TimerTaskSys.AddTimerEventByLeftTime(
                    --     function()
                    if self.ctx and self.ctx.m_open then
                        m_netTipsCtrl:StopWork(true)
                        if body then
                            local agent_id = body.data.agent_id
                            self.updateIsBind(agent_id)
                        end
                    end
                    -- end,
                    -- 5
                    -- )
                end,
                function(body, head)
                    DwDebug.Log("WebNetHelper RequestCheckAgent failed")
                    m_netTipsCtrl:StopWork(false)
                end
            )
        end
    )
end

function bing_agent_ui_window.InitWindowDetial()
    local ctx = self.ctx
    ctx:SetActive(ctx:GetTrans("binded"), false)
    ctx:SetActive(ctx:GetTrans("not_binded"), false)
    m_netTipsCtrl:StartWork()
end

function bing_agent_ui_window.updateIsBind(agent_id)
    local is_binded = (agent_id ~= 0 and (agent_id ~= nil))
    local ctx = self.ctx
    ctx:SetActive(ctx:GetTrans("binded"), is_binded)
    ctx:SetActive(ctx:GetTrans("not_binded"), not is_binded)
    if is_binded then
        ctx:SetLabel(ctx:GetTrans("agent_id", true), agent_id)
    else
        self.updateNotBindedUI()
    end
end
function bing_agent_ui_window.updateNotBindedUI()
    local ctx = self.ctx
    local is_exist_input = (#self.cache_str > 0)
    ctx:SetActive(ctx:GetTrans("no_input"), not is_exist_input)
    ctx:SetActive(ctx:GetTrans("exist_input"), is_exist_input)
    if is_exist_input then
        ctx:SetLabel(ctx:GetTrans("exist_input"), self.cache_str)
    end
end
function bing_agent_ui_window.HandleWidgetClick(gb)
    local click_name = gb.name
    if click_name == "close_btn" then
        WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
        self.InitWindow(false, 0)
    elseif string.find(click_name, "key_") then
        WrapSys.AudioSys_PlayEffect("Common/UI/uiclick")
        self._onkeyEvent(click_name)
    elseif click_name == "send_btn" then
        WrapSys.AudioSys_PlayEffect("Common/UI/uiclick")
        self.sendBind()
    else
        if m_netTipsCtrl then
            m_netTipsCtrl:HandleWidgetClick(gb)
        end
    end
end
-- 键盘事件
function bing_agent_ui_window._onkeyEvent(click_name)
    local str = string.sub(click_name, 5, -1)
    local ctx = self.ctx
    local cache_str = self.cache_str
    if str == "del" then
        if #cache_str > 0 then
            cache_str = string.sub(cache_str, 1, -2)
        end
    else
        -- 增加数量限制
        if #cache_str < 8 then
            cache_str = cache_str .. str
        end
    end
    self.cache_str = cache_str
    self.updateNotBindedUI()
end
function bing_agent_ui_window.sendBind()
    local str_agent_id = self.cache_str
    WebNetHelper.RequestBindAgent(
        str_agent_id,
        function(body, head)
            if not body then
                return
            end
            local agent_id = body.data.agent_id
            local card_number = body.data.card_number
            local reward_number = body.data.reward_number
            if reward_number > 0 then
                WindowUtil.LuaShowTips("绑定代理成功,获得房卡 x" .. reward_number .. " 张")
            end
            self.updateIsBind(agent_id)
            HallSys.RefreshUserInfo()
        end,
        function(body, head)
            if not body then
                return
            end
            -- 利用错误码飘字
            WindowUtil.LuaShowTips(body.errmsg)
        end
    )
end

function bing_agent_ui_window.OnDestroy()
    if m_netTipsCtrl then
        m_netTipsCtrl:Destroy()
        m_netTipsCtrl = nil
    end
end
