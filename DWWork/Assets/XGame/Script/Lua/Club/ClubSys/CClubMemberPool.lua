--------------------------------------------------------------------------------
-- 	File        : CClubMemberPool.lua
--  @Author     : zhisong
--  function    : 玩家池
--  @Date       : 2018-02-01 15:56:00
--  copyright   : Copyright 2018 DW Inc.
--  @Last Modified by:   zhisong
--  @Last Modified time: 2018-02-01 15:56:00
--------------------------------------------------------------------------------

require "Club.ClubSys.CClubMember"

CClubMemberPool = class("CClubMemberPool")

function CClubMemberPool:ctor()
    self.m_pool = {}
end

function CClubMemberPool:Destroy()
    self.m_pool = nil
end

function CClubMemberPool:NewElement(id, ...)
    if self.m_pool and self.m_pool[id] then
        self.m_pool[id]:ctor(...)
        return self.m_pool[id]
    else
        local mem = CClubMember.New(...)

        if mem then
            self.m_pool[id] = mem
            return mem
        else 
            return nil
        end
    end
end

function CClubMemberPool:Remove(key)
    if self.m_pool and self.m_pool[key] then
        self.m_pool[key] = nil
    end
end

function CClubMemberPool:GetElement(id)
    if self.m_pool then
        return self.m_pool[id]
    end
end




