--------------------------------------------------------------------------------
-- 	 File      : ClubRoomDataMgr.lua
--   author    : zhisong
--   function  : 俱乐部房间数据管理器
--   date      : 2018年1月26日 17:45:51
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Club.ClubSys.CClubRoom"

ClubRoomDataMgr = class("ClubRoomDataMgr")

--==============================--
--desc:构造函数
--time:2018-01-26 05:04:38
--@return
--==============================--
function ClubRoomDataMgr:ctor()
    self.m_room_data = DicAndList.New("m_room_id")
end

--==============================--
--desc:销毁函数,一般都不会调用到，clubsys里去掉引用就会自动回收
--time:2018-01-26 05:03:47
--@return
--==============================--
function ClubRoomDataMgr:Destroy()
    self.m_room_data:Destroy()
end

function ClubRoomDataMgr:GetRooms()
    return self.m_room_data:GetList()
end

function ClubRoomDataMgr:GetRoomByRoomID(room_id)
    return self.m_room_data:GetElement(room_id)
end

function ClubRoomDataMgr:InitRoomList(list)
    self.m_room_data:Clear()
    if not list or not list.items then
        return 
    end

    for i,v in ipairs(list.items or {}) do
        local room = CClubRoom.New(v)
        self.m_room_data:Add(room)
    end
end

function ClubRoomDataMgr:UpdateDetail(room_id, detail)
    local room = self.m_room_data:GetElement(room_id)
    if room then
        room:UpdateDetail(detail)
    end
end

function ClubRoomDataMgr:UpdateRoomInfo(data)
    if not self.m_room_data or not data or not data.roomId or not data.status then
        return
    end

    -- 3为解散房间推送
    if data.status == 3 then
        self.m_room_data:Remove(data.roomId)
    else 
        local room = self.m_room_data:GetElement(data.roomId)
        if room then
            room:UpdateData(data)
        else 
            local room = CClubRoom.New()
            room:UpdateData(data)
            self.m_room_data:Add(room)
        end
    end
end

------------------------------请求数据接口------------------------------
function ClubRoomDataMgr:TryGetClubRoomList(club_id, succ_cb, fail_cb)
    WebNetHelper.RequestRoomList(club_id,1,20,
        function (rsp, head)
            -- body
            DwDebug.LogWarning("xxx", "club room list = ", rsp)
            if rsp and rsp.data and rsp.data.list then
                self:InitRoomList(rsp.data.list)
            end
            if rsp and rsp.data and rsp.data.list and rsp.data.list.page_index and rsp.data.list.page_count and rsp.data.list.page_index >= rsp.data.list.page_count then
                if succ_cb then
                    succ_cb(rsp, head)
                end
            else 
                self:RequestRoomListWithPage(club_id, 2, 50, succ_cb)
            end
        end,
        function (rsp, head)
            DwDebug.LogError("RequestRoomList failed ", rsp, head)
            if fail_cb then
                fail_cb(rsp, head)
            end
        end
    )
end

function ClubRoomDataMgr:RequestRoomListWithPage(club_id, page_index, page_size, succ_cb)
    WebNetHelper.RequestRoomList(club_id,page_index,page_size,
        function (rsp, head)
            -- body
            DwDebug.LogWarning("xxx", "club room list = ", rsp)
            if rsp and rsp.data and rsp.data.list then
                self:InitRoomList(rsp.data.list)
            end

            -- 防止服务端数据错误，只取5次
            if rsp and rsp.data and rsp.data.list and rsp.data.list.page_index and rsp.data.list.page_count and
                page_index < 6 and
                rsp.data.list.page_index < rsp.data.list.page_count then
                self:RequestRoomListWithPage(club_id, page_index + 1, 20, succ_cb)
            else 
                if succ_cb then
                    succ_cb(rsp, head)
                end
            end
        end,
        function (rsp, head)
            DwDebug.LogError("RequestRoomList failed ", rsp, head)
        end
    )        
end



------------------------------请求数据接口------------------------------