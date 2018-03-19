--------------------------------------------------------------------------------
-- 	 File      : ClubMemberDataMgr:lua
--   author    : zhisong
--   function  : 俱乐部成员数据管理器
--   date      : 2018年1月26日 17:45:51
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Club.ClubSys.DicAndList"

ClubMemberDataMgr = class("ClubMemberDataMgr")

--==============================--
--desc:构造函数
--time:2018-01-26 05:04:38
--@return
--==============================--
function ClubMemberDataMgr:ctor()
    self.m_mem_data = DicAndList.New("m_uid")
end

--==============================--
--desc:销毁函数,一般都不会调用到，clubsys里去掉引用就会自动回收
--time:2018-01-26 05:03:47
--@return
--==============================--
function ClubMemberDataMgr:Destroy()
    self.m_mem_data:Destroy()
end

--==============================--
--desc:获取成员列表
--time:2018-02-02 11:47:06
--@return 成员列表
--==============================--
function ClubMemberDataMgr:GetMembers()
    return self.m_mem_data:GetList()
end

--==============================--
--desc:获取对应uid的成员
--time:2018-02-02 11:47:23
--@uid:uid
--@return uid成员
--==============================--
function ClubMemberDataMgr:GetMemberByUID(uid)
    return self.m_mem_data:GetElement(uid)
end

--==============================--
--desc:初始化成员列表
--time:2018-02-02 11:48:03
--@list:成员列表数据
--@return 
--==============================--
function ClubMemberDataMgr:InitMemList(rsp)
    local list
    if not rsp or not rsp.data or not rsp.data.list or not rsp.data.list.items then
        return
    else 
        list = rsp.data.list.items
    end
    for i,v in ipairs(list or {}) do
        local mem = ClubSys.NewMember(v.uid, v)
        self.m_mem_data:Add(mem)
    end
end

--==============================--
--desc:更新成员详细数据
--time:2018-02-02 11:48:26
--@uid:对应uid
--@detail:详细数据
--@return 
--==============================--
function ClubMemberDataMgr:UpdateDetail(uid, detail)
    local mem = self.m_mem_data:GetElement(uid)
    if mem then
        mem:UpdateDetail(detail)
    end
end

-- 删除成员
function ClubMemberDataMgr:RemoveMember(uid)
    self.m_mem_data:Remove(uid)
end

-- 新增成员
function ClubMemberDataMgr:AddNewMember(member)
    self.m_mem_data:Add(member)
end

------------------------------请求数据接口------------------------------
--==============================--
--desc:获取俱乐部内所有成员
--time:2018-02-02 11:48:48
--@club_id:俱乐部id
--@succ_cb:成功回调
--@fail_cb:失败回调
--@return 
--==============================--
function ClubMemberDataMgr:TryGetClubMemberList(club_id, succ_cb, fail_cb)
    self.m_mem_data:Clear()
    WebNetHelper.RequestClubMemList(club_id,1,1,50,nil,
        function (rsp, head)
            -- body
            DwDebug.LogWarning("xxx", "club_id=" .. club_id, "club member list = ", rsp)
            -- 可能需要排序
            self:InitMemList(rsp)
            if rsp and rsp.data and rsp.data.list and rsp.data.list.page_index and rsp.data.list.page_count and rsp.data.list.page_index >= rsp.data.list.page_count then
                if succ_cb then
                    succ_cb(rsp, head)
                end
            else 
                self:RequestMemListWithPage(club_id, 2, 50, succ_cb)
            end
        end,
        function (rsp, head)
            DwDebug.LogError("RequestClubMemList failed ")
            if fail_cb then
                fail_cb(rsp, head)
            end
        end
    )
end

function ClubMemberDataMgr:RequestMemListWithPage(club_id, page_index, page_size, succ_cb)
    -- TimerTaskSys.AddTimerEventByLeftTime(
    --         function()
                WebNetHelper.RequestClubMemList(club_id,1,page_index,page_size,nil,
                    function (rsp, head)
                        -- body
                        DwDebug.LogWarning("xxx", "club_id=" .. club_id, "club member list = ", rsp)
                        -- 可能需要排序
                        self:InitMemList(rsp)
            
                        -- 防止服务端数据错误，只取5次
                        if rsp and rsp.data and rsp.data.list and rsp.data.list.page_index and rsp.data.list.page_count and
                            page_index < 6 and
                            rsp.data.list.page_index < rsp.data.list.page_count then
                            self:RequestMemListWithPage(club_id, page_index + 1, 50, succ_cb)
                        else 
                            if succ_cb then
                                succ_cb(rsp, head)
                            end
                        end
                    end,
                    function (rsp, head)
                        DwDebug.LogError("RequestClubMemList failed ", rsp, head)
                    end
                )        
        --     end,
        --     0
        -- )
end

--==============================--
--desc:获取成员详情,目前不需要，以后可能会增加点击玩家出个弹窗，邀请、跟随什么的
--time:2018-02-02 11:49:40
--@club_id:成员所在俱乐部id
--@look_id:成员uid
--@succ_cb:成功回调
--@fail_cb:失败回调
--@return 
--==============================--
-- function ClubMemberDataMgr:TryGetClubMemberDetail(club_id, look_id, succ_cb, fail_cb)
--     WebNetHelper.RequestClubMemDetail(club_id,look_id,
--         function (rsp, head)
--             -- body
--             DwDebug.LogWarning("xxx", "club member detail = ", rsp)
--             -- 可能需要排序
--             self:UpdateDetail(look_id, rsp.data)
--             if succ_cb then
--                 succ_cb(rsp, head)
--             end
--         end,
--         function (rsp, head)
--             if fail_cb then
--                 fail_cb(rsp, head)
--             end
--         end
--     )
-- end


------------------------------请求数据接口------------------------------