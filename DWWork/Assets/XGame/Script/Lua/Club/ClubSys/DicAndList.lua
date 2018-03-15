--------------------------------------------------------------------------------
-- 	File        : DicAndList.lua
--  @Author     : zhisong
--  function    : 即是字典又是列表
--  @Date       : 2018-01-31 15:37:17
--  copyright   : Copyright 2018 DW Inc.
--  @Last Modified by:   zhisong
--  @Last Modified time: 2018-01-31 15:37:17
--------------------------------------------------------------------------------

DicAndList = class("DicAndList")

function DicAndList:ctor(key_string)
    self.m_key_string = key_string
    self.m_dic = {}
    self.m_list = {}
    self.m_public_list = {}
    self.m_key_index = {}
    self.m_private_data_version = 0
    self.m_public_data_version = 0
end

function DicAndList:Destroy()
    self.m_key_string = nil
    self.m_dic = nil
    self.m_list = nil
    self.m_public_list = nil
    self.m_key_index = nil
end

function DicAndList:Clear()
    self.m_dic = {}
    self.m_list = {}
    self.m_public_list = {}
    self.m_key_index = {}
    self.m_private_data_version = 0
    self.m_public_data_version = 0
end

function DicAndList:Add(ele)
    if ele and ele[self.m_key_string] then
        self:MarkPrivateDataVersion()
        self.m_dic[ele[self.m_key_string]] = ele
        table.insert( self.m_list, ele)
        self.m_key_index[ele[self.m_key_string]] = #self.m_list
    else 
        DwDebug.LogError("DicAndList try to add a wrong element.", self.m_key_string, ele)
    end
end

function DicAndList:Remove(key)
    if self.m_dic[key] then
        self:MarkPrivateDataVersion()
        self.m_dic[key] = nil
        table.remove(self.m_list, self.m_key_index[key])
        for i,v in pairs(self.m_key_index) do
            if v > self.m_key_index[key] then
                self.m_key_index[i] = v - 1
            end
        end
        self.m_key_index[key] = nil
    else 
        DwDebug.LogWarning("DicAndList try to remove a nonexistent element.", key, self.m_dic, list)
    end
end

function DicAndList:IsEmpty()
    return not self.m_list or #self.m_list <= 0
end

-- 获取列表长度
function DicAndList:Length()
    if nil == self.m_list then
        return 0
    end

    return #self.m_list
end

function DicAndList:GetList()
    if not self:PublicDataAvailable() then
        self.m_public_list = ShallowCopy(self.m_list)
        self:MarkPublicDataVersion()
    end
    return self.m_public_list
end

function DicAndList:GetElement(key)
    return self.m_dic[key]
end

-- 记录内部数据版本增加
function DicAndList:MarkPrivateDataVersion()
    self.m_private_data_version = self.m_private_data_version + 1
end

-- 同步外部数据版本为内部数据版本号
function DicAndList:MarkPublicDataVersion()
    self.m_public_data_version = self.m_private_data_version
end

-- 保存的房间列表(内部数据)和浅拷贝出来给外部用的房间列表(外部数据)是同一个版本
-- 即判断外部数据版本和内部数据版本是否一样
function DicAndList:PublicDataAvailable()
    return self.m_public_data_version == self.m_private_data_version
end



