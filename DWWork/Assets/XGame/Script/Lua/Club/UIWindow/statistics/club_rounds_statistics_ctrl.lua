--------------------------------------------------------------------------------
-- 	 File       : club_rounds_statistics_ctrl.lua
--   author     : zhisong
--   function   : 局数统计窗口组件
--   date       : 2018年2月24日
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------
require "Club.UIWindow.statistics.page_scrollview_ctrl"

club_rounds_statistics_ctrl = class("club_rounds_statistics_ctrl", page_scrollview_ctrl)

-------------------------------------------------------------------------------------------------------
function club_rounds_statistics_ctrl:ctor(lua_window_root)
    self:InitParams({
        m_root = lua_window_root:GetTrans("round_statistics"),
        m_show_num = UnityEngine.Vector2.New(1, 6)
    })
    self.super.ctor(self, lua_window_root)
end

function club_rounds_statistics_ctrl:Destroy()
    self.super.Destroy(self)
end
--------------------------------------------------------------------------------

-- 初始化ui，此模块显示总入口
function club_rounds_statistics_ctrl:InitUI(group_id)
    self.m_group_id = group_id
    self:HighLightDateBtn(1)
	self:TryRenderUI(false, 1, 20)
end

-- 按键响应函数
function club_rounds_statistics_ctrl:HandleWidgetClick(gb)
    self.super.HandleWidgetClick(self, gb)
    local click_name = gb.name
    if click_name == "high_light_today_btn" then
        if self.m_cur_index ~= 1 then
            self:HighLightDateBtn(1)
            self:TryRenderUI(false, 1, 20)
        end
    elseif click_name == "high_light_last_three_btn" then
        if self.m_cur_index ~= 2 then
            self:HighLightDateBtn(2)
            self:TryRenderUI(false, 1, 20)
        end
    elseif click_name == "high_light_last_fifth_btn" then
        if self.m_cur_index ~= 3 then
            self:HighLightDateBtn(3)
            self:TryRenderUI(false, 1, 20)
        end
    end
end

function club_rounds_statistics_ctrl:HighLightDateBtn(index)
    self.m_cur_index = index
    if index == 1 then
        self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(self.m_params.m_root, "range_btns"), "high_light_today", true)
    elseif index == 2 then
        self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(self.m_params.m_root, "range_btns"), "high_light_last_three", true)
    elseif index == 3 then
        self.m_lua_win_root:ShowChild(self.m_lua_win_root:GetTrans(self.m_params.m_root, "range_btns"), "high_light_last_fifth", true)
    end
end

function club_rounds_statistics_ctrl:InitTrans(root, index, data)
	root.name = "record_" .. index

	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "cost_cards"), data.total_card_number)
	self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "remain_cards"), data.now_card_number)
    self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "game_name"), data.play_name)
    self.m_lua_win_root:SetLabel(self.m_lua_win_root:GetTrans(root, "round_count"), data.total_game_number)
end

function club_rounds_statistics_ctrl:Request(succ_cb, fail_cb, page_index, page_size)
    WebNetHelper.RequestStatisticsPlayed(page_index, page_size, self.m_group_id, self.m_cur_index, succ_cb, fail_cb)
end

function club_rounds_statistics_ctrl:InitData(data)
	if not self.m_data_list then
		self.m_data_list = {}
    end
    
    if data.total then
        table.insert(self.m_data_list, {
            total_card_number = data.total.total_card_number or 0,
            now_card_number = data.total.now_card_number or 0,
            total_game_number = data.total.total_game_number or 0,
            play_name = "总合计"
        })
    end

	for i,v in ipairs(data.list.items or {}) do 
		table.insert(self.m_data_list, v)
	end
end


