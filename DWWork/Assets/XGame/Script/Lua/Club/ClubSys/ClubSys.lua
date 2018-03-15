--------------------------------------------------------------------------------
-- 	 File      : ClubSys.lua
--   author    : zhisong
--   function  : 俱乐部数据管理系统
--   date      : 2018年1月26日 16:48:33
--   copyright : Copyright 2018 DW Inc.
--------------------------------------------------------------------------------
require "Club.ClubSys.CClub"
require "Club.ClubSys.DicAndList"
require "Club.ClubSys.CClubMemberPool"

ClubSys = {
}

local this = ClubSys

--==============================--
--desc:模块初始化
--time:2018-01-26 05:04:38
--@return
--==============================--
function ClubSys.Init()
    this.m_club_data = DicAndList.New("m_id")
    this.m_member_pool = CClubMemberPool.New()
end

--==============================--
--desc:退出到登陆界面被调用
--time:2018-01-26 05:03:47
--@return
--==============================--
function ClubSys.Reset()
    this.ClearCache()
    this.Init()
end

--==============================--
--desc:清理缓存
--time:2018-02-02 10:59:38
--@return
--==============================--
function ClubSys.ClearCache()
    if this.m_club_data then
        this.m_club_data:Destroy()
    end
    if this.m_member_pool then
        this.m_member_pool:Destroy()
    end
end

-- 取自己所在的俱乐部列表
function ClubSys.GetClubs()
    if this.m_club_data then
        return this.m_club_data:GetList()
    end

    return {}
end

-- 根据俱乐部id取自己所在的俱乐部
function ClubSys.GetClubById(id)
    if this.m_club_data then
        return this.m_club_data:GetElement(id)
    end
end

--==============================--
--desc:新创建一个俱乐部成员，member_pool里也会保存一份引用
--time:2018-02-02 11:00:13
--@args:
--@return
--==============================--
function ClubSys.NewMember(...)
    if this.m_member_pool then
        return this.m_member_pool:NewElement(...)
    end
end


--==============================--
--desc:进入大厅响应函数，一般用作注册俱乐部相关网络消息监听
--time:2018-01-30 04:45:31
--@return
--==============================--
function ClubSys.OnEnterHall()
    this.RegisterHandle()
    this.SendEnterClubHall(true)
end

--==============================--
--desc:退出大厅响应函数，一般用作注销俱乐部相关网络消息监听
--time:2018-01-30 04:41:44
--@return
--==============================--
function ClubSys.OnExitHall()
    this.TaintClubData()
    this.SendEnterClubHall(false)
    this.UnRegisterHandle()
end


--==============================--
--desc:注册网络推送
--time:2018-02-02 11:02:28
--@return
--==============================--
function ClubSys.RegisterHandle()
    LuaUserNetWork.RegisterHandle(USER_SVR_CMD.SC_PLAYER_STATUS_NOTIFY,this.NotifyUpdateMember)
    LuaUserNetWork.RegisterHandle(USER_SVR_CMD.SC_ROOM_STATUS_NOTIFY,this.NotifyUpdateRoom)
    LuaUserNetWork.RegisterHandle(USER_SVR_CMD.BC_GROUP_GET_NEW_MEMBER,this.NotifyNewMember)
    LuaUserNetWork.RegisterHandle(USER_SVR_CMD.BC_QUIT_GROUP,this.NotifyMemberQuit)
    LuaUserNetWork.RegisterHandle(USER_SVR_CMD.BC_GROUP_KICKOUT,this.NotifyMemberKickedOut)
    LuaUserNetWork.RegisterHandle(USER_SVR_CMD.BC_GROUP_DISMISSED,this.NotifyClubDismissed)
    LuaUserNetWork.RegisterHandle(USER_SVR_CMD.SC_NEW_MAIL_NOTIFY,this.NotifyNewMail)
end

--==============================--
--desc:注销网络推送
--time:2018-02-02 11:02:40
--@return
--==============================--
function ClubSys.UnRegisterHandle()
    LuaUserNetWork.UnRegisterHandle(USER_SVR_CMD.SC_PLAYER_STATUS_NOTIFY,this.NotifyUpdateMember)
    LuaUserNetWork.UnRegisterHandle(USER_SVR_CMD.SC_ROOM_STATUS_NOTIFY,this.NotifyUpdateRoom)
    LuaUserNetWork.UnRegisterHandle(USER_SVR_CMD.BC_GROUP_GET_NEW_MEMBER,this.NotifyNewMember)
    LuaUserNetWork.UnRegisterHandle(USER_SVR_CMD.BC_QUIT_GROUP,this.NotifyMemberQuit)
    LuaUserNetWork.UnRegisterHandle(USER_SVR_CMD.BC_GROUP_KICKOUT,this.NotifyMemberKickedOut)
    LuaUserNetWork.UnRegisterHandle(USER_SVR_CMD.BC_GROUP_DISMISSED,this.NotifyClubDismissed)
    LuaUserNetWork.UnRegisterHandle(USER_SVR_CMD.SC_NEW_MAIL_NOTIFY,this.NotifyNewMail)
end

-- 俱乐部新增新成员
function ClubSys.NotifyNewMember(rsp, head)
    DwDebug.LogWarning("xxx", "ClubSys.NotifyNewMember ", rsp, head)
    if rsp and rsp.uid and rsp.group_id then
        if rsp.uid == DataManager.GetUserID() then
            this.TaintClubData()
            LuaEvent.AddEventNow(EEventType.RefreshClubHallWindow, rsp.group_id)
        else
            if this.m_member_pool then
                local club = this.GetClubById(rsp.group_id)
                if club then
                    local mem = ClubSys.NewMember(rsp.uid, rsp)
                    club:AddNewMember(mem)
                    LuaEvent.AddEventNow(EEventType.RefreshClubMember,rsp.uid, rsp.group_id)
                end
            end
        end
    end
end

-- 有成员退出俱乐部
function ClubSys.NotifyMemberQuit(rsp, head)
    DwDebug.LogWarning("xxx", "ClubSys.NotifyMemberQuit ", rsp, head)
    if rsp and rsp.quit_uid and rsp.group_id then
        if rsp.quit_uid == DataManager.GetUserID() then
            this.m_club_data:Remove(rsp.group_id)
            LuaEvent.AddEventNow(EEventType.RefreshClubHallWindow, rsp.group_id)
        else
            local club = this.GetClubById(rsp.group_id)
            if club then
                club:RemoveMember(rsp.quit_uid)
                -- 可能只是退出一个俱乐部，其他俱乐部还有这个成员
                -- this.m_member_pool:Remove(rsp.quit_uid)
                LuaEvent.AddEventNow(EEventType.RefreshClubMember,rsp.quit_uid, rsp.group_id)
            end
        end
    end
end

-- 有成员被踢出俱乐部
function ClubSys.NotifyMemberKickedOut(rsp, head)
    DwDebug.LogWarning("xxx", "ClubSys.NotifyMemberKickedOut ", rsp, head)
    if rsp and rsp.kick_uid and rsp.group_id then
        if rsp.kick_uid == DataManager.GetUserID() then
            local club = this.GetClubById(rsp.group_id)
            if club then
                GameStateQueueTask.AddTask(function ()
                    WindowUtil.ShowErrorWindow(3, string.format("您被请出了%s俱乐部", club.m_name))
                end,1,EGameStateType.MainCityState)
            end

            this.m_club_data:Remove(rsp.group_id)
            LuaEvent.AddEventNow(EEventType.ClubQuieOrDisBand, rsp.group_id)
            LuaEvent.AddEventNow(EEventType.RefreshClubHallWindow, rsp.group_id)
        else
            local club = this.GetClubById(rsp.group_id)
            if club then
                club:RemoveMember(rsp.kick_uid)
                -- 可能只是踢出一个俱乐部，其他俱乐部还有这个成员
                -- this.m_member_pool:Remove(rsp.kick_uid)
                LuaEvent.AddEventNow(EEventType.RefreshClubMember,rsp.kick_uid, rsp.group_id)
            end
        end
    end
end

-- 俱乐部解散
function ClubSys.NotifyClubDismissed(rsp, head)
    DwDebug.LogWarning("ClubSys.NotifyClubDismissed ", rsp, head)
    if rsp and rsp.group_id then
        local club = this.GetClubById(rsp.group_id)
        if club then
            if club.m_owner_uid == DataManager.GetUserID() then
                GameStateQueueTask.AddTask(function ()
                    WindowUtil.ShowErrorWindow(3, string.format("您解散了%s俱乐部，剩余房卡稍后退还到游戏大厅", club.m_name))
                end,1,EGameStateType.MainCityState)
                WindowUtil.LuaShowTips()
            else
                GameStateQueueTask.AddTask(function ()
                    WindowUtil.ShowErrorWindow(3, string.format("您所在的俱乐部%s已解散", club.m_name))
                end,1,EGameStateType.MainCityState)
            end
        end
        this.m_club_data:Remove(rsp.group_id)
        LuaEvent.AddEventNow(EEventType.ClubQuieOrDisBand, rsp.group_id)
        LuaEvent.AddEventNow(EEventType.RefreshClubHallWindow, rsp.group_id)
    end
end

--==============================--
--desc:处理成员更新推送
--time:2018-02-02 11:02:58
--@rsp:推送包体
--@head:推送包头
--@return
--==============================--
function ClubSys.NotifyUpdateMember(rsp, head)
    DwDebug.LogWarning("xxx", "NotifyUpdateMember", rsp, head)
    if this.m_member_pool and rsp and rsp.member and rsp.member.uid then
        local mem = this.m_member_pool:GetElement(rsp.member.uid)
        if mem then
            mem:StatusChangeNotify(rsp.member)
        end
        LuaEvent.AddEventNow(EEventType.RefreshClubMember, rsp.member.uid,rsp.gids)
    end
end

--==============================--
--desc:处理房间更新推送
--time:2018-02-02 11:03:29
--@rsp:推送包体
--@head:推送包头
--@return
--==============================--
function ClubSys.NotifyUpdateRoom(rsp, head)
    DwDebug.LogWarning("xxx", "NotifyUpdateRoom", rsp, head)
    if this.m_club_data and rsp.groupId and rsp.roomId then
        local club = this.m_club_data:GetElement(rsp.groupId)
        if club then
            club:UpdateRoomInfo(rsp)
        end
        LuaEvent.AddEventNow(EEventType.RefreshClubRoom, rsp.roomId,rsp.groupId)
    end
end

--==============================--
--desc:上报通讯服 进入/退出俱乐部大厅
--time:2018-02-02 11:03:55
--@is_entry:true进入，false退出
--@return
--==============================--
function ClubSys.SendEnterClubHall(is_entry)
    LuaUserNetWork.SendMsg(USER_SVR_CMD.CS_HALL_STATUS_REPORT,{isEntry = is_entry},nil,nil,true)
end
--==============================--
--desc:上报通讯服 进入/退出特定俱乐部
--time:2018-02-02 11:05:12
--@is_entry:true进入，false退出
--@club_id:特定俱乐部的id
--@return
--==============================--
function ClubSys.SendEnterClubUI(is_entry, club_id)
    LuaUserNetWork.SendMsg(USER_SVR_CMD.CS_UI_STATUS_REPORT,{isEntry = is_entry, groupId = club_id},nil,nil,true)
end

-- 标记俱乐部列表数据已经无效
function ClubSys.TaintClubData()
    this.m_club_data_tainted = true
end

--==============================--
--desc:尝试获取俱乐部列表
--time:2018-02-02 11:06:23
--@succ_cb:成功回调
--@fail_cb:失败回调
--@force_request:无论本地有没有数据都去重新请求
--@return
--==============================--
function ClubSys.TryGetClubList(succ_cb, fail_cb, force_request)
    if not this.m_club_data:IsEmpty() and not force_request and not this.m_club_data_tainted then
        if succ_cb then
            succ_cb()
        end
    else
        -- this.Reset()
        this.RequestClubList(0,
            function (rsp, head)
                -- body
                DwDebug.LogWarning("xxx", "club list = ", rsp)
                this.m_club_data_tainted = false

                if not rsp or not rsp.data or not rsp.data.list then
                    return
                end

                this.RefreshClubList(rsp.data.list)

                if succ_cb then
                    succ_cb(rsp, head)
                end
            end,
            function (rsp, head)
                if fail_cb then
                    fail_cb(rsp, head)
                end
            end
        )
    end
end

function ClubSys.RefreshClubList(recv_data)
    local local_list = this.m_club_data:GetList()

    -- 先将收到的数据全部更新成本地数据
    for recv_i,recv_club in ipairs(recv_data or {}) do
        local local_club = this.m_club_data:GetElement(recv_club.id)
        if local_club then
            local_club:UpdateData(recv_club)
        else
            local club = CClub.New(recv_club)
            this.m_club_data:Add(club)
        end
    end

    -- 再移除过时的本地数据
    for local_i,local_club in ipairs(local_list or {}) do
        local found = false
        for recv_i,recv_club in ipairs(recv_data or {}) do
            if local_club.m_id == recv_club.id then
                found = true
            end
        end

        if not found then
            -- 这里是可以这么直接移除的，local_list是浅拷贝出来的表，移除原来的表里元素不会有关系
            this.m_club_data:Remove(local_club.m_id)
        end
    end
end

-- 请求俱乐部列表,此接口是为了防止在前一个请求还没返回的时候下一个请求又调用进来，这样会把第一个请求的回调清掉，原因是LuaHttpNetWork里的callback用msgkey来当标识
function ClubSys.RequestClubList(state, succ_cb, fail_cb)
    local flag_send_req = true

    if not this.m_event_cb then
        this.m_event_cb = {
            m_succ = Event("req_club_succ_event", true),
            m_fail = Event("req_club_fail_event", true)
        }
        this.m_event_cb.m_succ:Add(succ_cb)
        this.m_event_cb.m_fail:Add(fail_cb)
    else
        -- 不为空即是请求还没回来，则回调加进去就算了
        this.m_event_cb.m_succ:Add(succ_cb)
        this.m_event_cb.m_fail:Add(fail_cb)
        flag_send_req = false
    end

    if not flag_send_req then
        return
    end

    WebNetHelper.RequestClubList(state,
        function(rsp, head)
            if this.m_event_cb then
                this.m_event_cb.m_succ(rsp, head)
                this.m_event_cb = nil
            end
        end
        ,
        function(rsp, head)
            if this.m_event_cb then
                this.m_event_cb.m_fail(rsp, head)
                this.m_event_cb = nil
            end
        end
    )
end

--==============================--
--desc:尝试获取俱乐部详情,俱乐部信息被修改不会推送，这个数据不用缓存
--time:2018-02-02 11:07:22
--@club_id:要获取的俱乐部的id
--@succ_cb:成功回调
--@fail_cb:失败回调
--@return
--==============================--
-- function ClubSys.TryGetClubDetail(club_id, succ_cb, fail_cb)
--     local club = this.m_club_data:GetElement(club_id)
--     if club and club:HasDetail() then
--         if succ_cb then
--             succ_cb()
--         end
--     else
--         WebNetHelper.RequestClubDetail(club_id,
--             function (rsp, head)
--                 -- body
--                 DwDebug.LogWarning("xxx", "club detail = ", rsp)
--                 if club then
--                     club:UpdateDetail(rsp.data)
--                 end
--                 if succ_cb then
--                     succ_cb(rsp, head)
--                 end
--             end,
--             function (rsp, head)
--                 if fail_cb then
--                     fail_cb(rsp, head)
--                 end
--             end
--         )
--     end
-- end

--==============================--
--desc:创建俱乐部web请求
--time:2018-02-02 11:08:15
--@name:
--@g_img:
--@owner_weixin:
--@owner_mobile:
--@succ_cb:
--@fail_cb:
--@return
--==============================--
function ClubSys.CreateClub(name, g_img, owner_weixin, owner_mobile, succ_cb, fail_cb)
    WebNetHelper.RequestCreateClub(name, g_img, owner_weixin, owner_mobile,
        function (rsp, head)
            -- body
            -- this.m_club_data:Add(CClub.New(xxx))
            DwDebug.LogWarning("xxx", "create club rsp =", rsp)
            this.TaintClubData()
            if succ_cb then
                succ_cb(rsp, head)
            end
        end,
        function (rsp, head)
            if fail_cb then
                fail_cb(rsp, head)
            end
        end
    )
end

--==============================--
--desc:更新俱乐部web请求
--time:2018-02-02 11:08:25
--@id:
--@name:
--@g_img:
--@owner_weixin:
--@owner_mobile:
--@succ_cb:
--@fail_cb:
--@return
--==============================--
function ClubSys.UpdateClub(id,name,g_img,owner_weixin,owner_mobile, succ_cb, fail_cb)
    WebNetHelper.RequestUpdateClub(id,name,g_img,owner_weixin,owner_mobile,
        function (rsp, head)
            -- body
            DwDebug.LogWarning("xxx", "update club", rsp)
            -- local club = this.m_club_data:GetElement(id)
            -- if club then
            --     club:UpdateDetail(xxx)
            -- end
            -- TODOTODO修改头像会修改房间默认图标，如果是修改图标，就刷新下房间列表
            this.TaintClubData()
            if succ_cb then
                succ_cb(rsp, head)
            end
        end,
        function (rsp, head)
            if fail_cb then
                fail_cb(rsp, head)
            end
        end
    )
end

--==============================--
--desc:解散俱乐部web请求
--time:2018-02-02 11:08:45
--@club_id:
--@succ_cb:
--@fail_cb:
--@return
--==============================--
function ClubSys.DeleteClub(club_id, succ_cb, fail_cb)
    WebNetHelper.RequestDeleteClub(club_id,
        function (rsp, head)
            -- body
            DwDebug.LogWarning("xxx", "delete club", rsp, "club_id", club_id)
            local club = this.GetClubById(club_id)
            if club then
                GameStateQueueTask.AddTask(function ()
                    WindowUtil.ShowErrorWindow(3, string.format("您解散了%s俱乐部，剩余房卡稍后退还到游戏大厅", club.m_name))
                end,1,EGameStateType.MainCityState)
            end
            this.m_club_data:Remove(club_id)
            if succ_cb then
                succ_cb(rsp, head)
            end
        end,
        function (rsp, head)
            if fail_cb then
                fail_cb(rsp, head)
            end
        end
    )
end

-- 退出俱乐部
function ClubSys.QuitClub(id, succ_cb, fail_cb)
    WebNetHelper.RequestQuitClub(id, function(rsp, head)
        DwDebug.LogWarning("xxx", "quit club", rsp, "id:", id)
        local club = this.GetClubById(id)
        if club then
            GameStateQueueTask.AddTask(function ()
                WindowUtil.ShowErrorWindow(3, string.format("您已退出%s俱乐部", club.m_name))
            end,1,EGameStateType.MainCityState)
        end
        this.m_club_data:Remove(id)
        if succ_cb then
            succ_cb(rsp, head)
        end
    end,
    function(rsp, head)
        if fail_cb then
            fail_cb(rsp, head)
        end
    end)
end



--==============================--
--desc:俱乐部入口
--time:2018-02-23
--@return
--==============================--
function ClubSys.OpenClub(club_id)
    WrapSys.EZFunWindowMgr_SetWindowStatusForLua(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000 + 10, "wait_ui_window", false)
    -- 获取是否有俱乐部
    this.TryGetClubList(function (body, head)
        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
        if this.m_club_data:IsEmpty() then
            WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.clubCreateJoin_ui_window", false, 1)
        else
            WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Club.UIWindow.hall.club_hall_ui_window", false , club_id)
        end
    end,
    function (body, head)
        WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
        WindowUtil.LuaShowTips("请求超时，请稍后重试！")
    end)
end


-- 数据请求成功(打开俱乐部窗口的成员列表房间列表请求是否成功)
function ClubSys.RecvData(type, flag)
    if type == 1 then
        this.m_member_recved = true
        if this.m_room_recved then
            DwDebug.LogWarning("xxx", "close wait window")
            WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
        end
    elseif type == 2 then
        this.m_room_recved = true
        if this.m_member_recved then
            DwDebug.LogWarning("xxx", "close wait window")
            WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
        end
    end
end

-- 重置俱乐部窗口成员列表房间列表请求成功标志
function ClubSys.ResetRecvFlag()
    this.m_member_recved = false
    this.m_room_recved = false
end
