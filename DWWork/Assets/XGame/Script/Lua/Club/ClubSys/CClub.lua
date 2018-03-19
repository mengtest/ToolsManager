--------------------------------------------------------------------------------
-- 	File        : CClub.lua
--  @Author     : zhisong
--  function    : 俱乐部类
--  @Date       : 2018-01-31 11:30:54
--  copyright   : Copyright 2018 DW Inc.
--  @Last Modified by:   zhisong
--  @Last Modified time: 2018-01-31 11:30:54
--------------------------------------------------------------------------------
require "Club.ClubSys.ClubMemberDataMgr"
require "Club.ClubSys.ClubRoomDataMgr"

CClub = class("CClub")

--==============================--
--desc:构造函数
--time:2018-02-02 11:09:07
--@data:构造参数（v1_groupsvr.proto GroupList.Data.List）{id,name,g_img,owner_nickname,user_number,card_number,status}
--@return 
--==============================--
function CClub:ctor(data)
    self:UpdateData(data)
    self.m_mem_mgr = ClubMemberDataMgr.New()
    self.m_room_mgr = ClubRoomDataMgr.New()
end

--==============================--
--desc:销毁函数
--time:2018-02-02 11:12:13
--@return 
--==============================--
function CClub:Destory()
    if self.m_mem_mgr then
        self.m_mem_mgr:Destroy()
    end
    if self.m_room_mgr then
        self.m_room_mgr:Destroy()
    end
end

--==============================--
--desc:更新信息
--time:2018-02-02 11:12:22
--@detail:信息（v1_groupsvr.proto GroupList.Data.List）{id,name,g_img,owner_nickname,user_number,card_number,status}
--@return 
--==============================--
function CClub:UpdateData(data)
    self.m_id = data.id or 0
    self.m_name = utf8sub(data.name or "", 1, 10)
    self.m_g_img = data.g_img or 0
    self.m_owner_weixin = data.owner_weixin or ""
    self.m_owner_nickname = data.owner_nickname or ""
    self.m_user_number = data.user_number or 0
    self.m_card_number = data.card_number or 0
    self.m_status = data.status or 0
    self.m_owner_uid = data.owner_uid or 0
    self.m_main_play = data.main_play or ""
    self.m_has_detail = false
end

--==============================--
--desc:是否有详细信息
--time:2018-02-02 11:14:18
--@return 
--==============================--
-- function CClub:HasDetail()
--     return self.m_has_detail
-- end

--==============================--
--desc:更新房间信息
--time:2018-02-02 11:14:33
--@data:房间信息
--@return 
--==============================--
function CClub:UpdateRoomInfo(data)
    if self.m_room_mgr then
        self.m_room_mgr:UpdateRoomInfo(data)
    end
end

-- 新增成员
function CClub:AddNewMember(mem)
    if self.m_mem_mgr then
        self.m_mem_mgr:AddNewMember(mem)

        if self.m_user_number then
            self.m_user_number = self.m_user_number + 1
        end
    end
end

-- 减少成员
function CClub:RemoveMember(uid)
    if self.m_mem_mgr then
        self.m_mem_mgr:RemoveMember(uid)

        if self.m_user_number then
            self.m_user_number = self.m_user_number - 1
        end
    end
end











