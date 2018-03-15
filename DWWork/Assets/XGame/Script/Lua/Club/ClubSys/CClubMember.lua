--------------------------------------------------------------------------------
-- 	File        : CClubMember.lua
--  @Author     : zhisong
--  function    : 俱乐部成员类
--  @Date       : 2018-01-31 11:30:54
--  copyright   : Copyright 2018 DW Inc.
--  @Last Modified by:   zhisong
--  @Last Modified time: 2018-01-31 11:30:54
--------------------------------------------------------------------------------

CClubMember = class("CClubMember")

function CClubMember:ctor(data)
    self.m_uid = data.uid
    self.m_wx_nickname = utf8sub(data.wx_nickname, 1, 10)
    self.m_wx_headimgurl = ezfunLuaTool.GetSmallWeiXinIconUrl(data.wx_headimgurl, 64)
    self.m_wx_sex = data.wx_sex
    self.m_online_status = data.online_status

    self:CalculatePriority()
end

function CClubMember:Destory()
end

-- function CClubMember:UpdateDetail(detail)
--     self.m_uid = detail.uid
--     self.m_wx_nickname = detail.wx_nickname
--     self.m_wx_headimgurl = detail.wx_headimgurl
--     self.m_wx_sex = detail.wx_sex

--     self:CalculatePriority()
-- end

-- 收到状态变更通知，更新数据
function CClubMember:StatusChangeNotify(member)
    self.m_wx_headimgurl = ezfunLuaTool.GetSmallWeiXinIconUrl(member.headUrl, 64)
    self.m_wx_nickname = utf8sub(member.nickName, 1, 10)
    self.m_online_status = member.status

    self:CalculatePriority()
end

-- 计算排序的优先级，其实最好能把和排序相关的所有属性都算进来，但uid没有固定界限，没法算
function CClubMember:CalculatePriority()
    if self.m_uid == DataManager.GetUserID() then
        self.m_priority = 0 
        return
    end
    -- 排序-》1:在线；2：游戏中，0：离线
    if self.m_online_status == 1 then
        self.m_priority = 1
    elseif self.m_online_status == 2 then
        self.m_priority = 2 
    else
        self.m_priority = 3
    end
end










