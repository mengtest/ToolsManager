--------------------------------------------------------------------------------
-- 	File        : CClubRoom.lua
--  @Author     : zhisong
--  function    : 俱乐部房间类
--  @Date       : 2018-01-31 11:30:54
--  copyright   : Copyright 2018 DW Inc.
--  @Last Modified by:   zhisong
--  @Last Modified time: 2018-01-31 11:30:54
--------------------------------------------------------------------------------

CClubRoom = class("CClubRoom")

function CClubRoom:ctor(data)
    if data then
        self.m_room_id = data.room_id
        self.m_play_id = data.play_id
        self.m_play_name = data.play_name
        self.m_current_player_num = data.current_player_num
        self.m_max_player_num = data.max_player_num
        self.m_total_game_num = data.total_game_num
        self.m_play_des = data.play_des
        self.m_status = data.status
        self.m_group_id = data.group_id
        self.m_tpl_id = data.tpl_id
        self.m_create_time = data.create_time
        self.m_players = data.players

        self:CalculatePriority()
    end
end

function CClubRoom:Destory()
end

function CClubRoom:UpdateData(update_data)
    self.m_room_id = update_data.roomId
    self.m_group_id = update_data.groupId
    -- 解散状态推送的话，只有群id和房间id两个字段是有效的
    if update_data.status ~= 3 then
        self.m_play_id = update_data.playId
        self.m_play_name = update_data.playName
        self.m_current_player_num = #update_data.members
        self.m_total_game_num = update_data.totalGameNum
        self.m_play_des = update_data.playDes
        self.m_status = update_data.status == 2 and 1 or 0
        self.m_players = update_data.members -- headUrl,nickName字段有差别，一个是headimgurl,nickname
        self.m_tpl_id = update_data.tplID
        self.m_create_time = update_data.createTime
        self.m_max_player_num = update_data.maxPlayerNum
        for i,v in ipairs(self.m_players or {}) do
            if v.headUrl then
                v.headimgurl = v.headUrl
                v.headUrl = nil
            end
            if v.nickName then
                v.nickname = v.nickName
                v.nickName = nil
            end
            if v.sex then
                v.sex = 1
            else 
                v.sex = 2
            end
        end
    
        self:CalculatePriority()
    end
end

function CClubRoom:CalculatePriority()
    -- 忒镁，如果有一局游戏100人以上玩会排序会出错
    local lack_of_player = self.m_max_player_num - self.m_current_player_num
    self.m_priority = lack_of_player > 0 and 0 or 1000000000
    self.m_priority = self.m_priority + (self.m_tpl_id == 0 and 999999900 or self.m_tpl_id * 100)
    self.m_priority = self.m_priority + (lack_of_player > 0 and lack_of_player or 99)
end











