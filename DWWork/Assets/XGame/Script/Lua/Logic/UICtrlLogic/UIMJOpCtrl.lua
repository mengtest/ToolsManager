--------------------------------------------------------------------------------
-- 	 File      : UIMJOpCtrl.lua
--   author    : mid
--   function   : UI 麻将操作界面
--   date      : 2017-11-10
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "Logic.MJCardLogic.CMJCard"
require "Tools.LayoutGroup"

-- wgts
-- local not_mine
-- local mine
local m_bg
local m_opRoot

-- member
local m_luaWindowRoot
local m_rootTran
local m_data

UIMJOpCtrl = class("UIMJOpCtrl", nil)

function UIMJOpCtrl:Init(rootTran, luaWin)
    m_luaWindowRoot = luaWin
    m_rootTran = rootTran
    m_luaWindowRoot:SetActive(m_rootTran, false)
    self:Initwgts()
    self:RegEvent()
    self.itemsToClear = {}
end

function UIMJOpCtrl:Destroy()
    self:clearItem()
    self:UnRegEvent()
end

function UIMJOpCtrl:Initwgts()
    m_bg = m_luaWindowRoot:GetTrans(m_rootTran, "bg")
    m_opRoot = m_luaWindowRoot:GetTrans(m_rootTran, "ope_list")
    m_luaWindowRoot:SetActive(m_bg, false)
end

function UIMJOpCtrl:RegEvent()
    LuaEvent.AddHandle(EEventType.MJOpeTip, self.ShowUI, self)
    LuaEvent.AddHandle(EEventType.MJOpeTipHideUI, self.HideUI, self)
end

function UIMJOpCtrl:UnRegEvent()
    LuaEvent.RemoveHandle(EEventType.MJOpeTip, self.ShowUI, self)
    LuaEvent.RemoveHandle(EEventType.MJOpeTipHideUI, self.HideUI, self)
    m_luaWindowRoot = nil
    m_data = nil
    m_rootTran = nil
    bg = nil
end

function UIMJOpCtrl:DispatchEvent(gb)
    local click_name = gb.name

    if click_name == "mjOp_btn_pass" then
        if self:GetIsHu() then
            local contentStr = "确定放弃胡牌？"
            local okFunc = function()
                PlayGameSys.GetPlayLogic():SendPass()
                self:HideUI()
            end

            WindowUtil.ShowErrorWindow(2, contentStr, nil, okFunc)
            return
        else
            PlayGameSys.GetPlayLogic():SendPass()
        end
    elseif click_name == "mjOp_btn_hu" then
        PlayGameSys.GetPlayLogic():SendHuPai(self:GetHuData())
    elseif click_name == "mjOp_btn_peng" then
        PlayGameSys.GetPlayLogic():SendPengPai(self:GetPengData())
    elseif string.find(click_name, "mjOp_btn_gang") then
        local index = tonumber(string.sub(click_name, string.len("mjOp_btn_gang") + 1))
        PlayGameSys.GetPlayLogic():SendGangPai(self:GetMineGangData(index))
    end

    self:HideUI()
end

function UIMJOpCtrl:clearItem()
    local list = self.itemsToClear
    for i = 1, #list do
        m_luaWindowRoot:SetActive(list[i], false)
    end
    self.itemsToClear = {}
end

function UIMJOpCtrl:ShowUI(eventId, data)
    self:clearItem()

    self:InitUI(data)

    m_luaWindowRoot:SetActive(m_rootTran, true)
    -- m_luaWindowRoot:SetActive(m_bg, true)
    m_luaWindowRoot:SetActive(m_opRoot, true)
end

function UIMJOpCtrl:InitUI(data)
    m_data = data
    if nil == m_data then
        DwDebug.LogError("UIMJOpCtrl:InitUI error data is null")
        return
    end

    local group = LayoutGroup.New()
    local vector3 = UnityEngine.Vector3
    local startSize = vector3.New(540, -100, 0)
    local cellSize = vector3.New(-300, 0, 0)
    local offset = vector3.New(-20, 0, 0)
    local lineRange = Vector3.New(800, 0, 0)
    local lineSize = Vector3.New(0, 130, 0)
    group:Init(startSize, cellSize, offset, lineRange, lineSize)

    local groupScale = vector3.New(0.7, 0.7, 0)
    local isPass = true
    if isPass then
        local go = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "mjOp_btn_pass", m_opRoot, RessStorgeType.RST_Never)
        if not go then
            DwDebug.Log("UIMJOpCtrl:InitUI error :no go mjOp_btn_pass")
            return
        end

        go.name = "mjOp_btn_pass"
        local trans = go.transform
        group:AddCell(trans)

        m_luaWindowRoot:SetActive(trans, true)
        table.insert(self.itemsToClear, trans)
    end

    -- 胡
    if self:GetIsHu() then
        local go = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "mjOp_btn_hu", m_opRoot, RessStorgeType.RST_Never)
        if not go then
            DwDebug.Log("UIMJOpCtrl:InitUI error :no go mjOp_btn_hu")
            return
        end

        go.name = "mjOp_btn_hu"
        local trans = go.transform
        group:AddCell(trans)

        -- 碰手动修改层级
        -- m_luaWindowRoot:SetDepth(trans, 301)
        m_luaWindowRoot:SetActive(trans, true)
        table.insert(self.itemsToClear, trans)
    end

    -- 碰
    if self:GetIsPeng() then
        local go = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "mjOp_btn_peng", m_opRoot, RessStorgeType.RST_Never)
        if not go then
            DwDebug.Log("UIMJOpCtrl:InitUI error :no go mjOp_btn_peng")
            return
        end

        go.name = "mjOp_btn_peng"
        local trans = go.transform
        group:AddCell(trans)

        local pengData = self:GetPengData()
        local cardGroup = LogicUtil.GenCardGroup(m_luaWindowRoot, 1, pengData.pai)
        cardGroup.parent = trans
        cardGroup.localScale = groupScale
        cardGroup.localPosition = vector3.zero
        m_luaWindowRoot:SetActive(cardGroup, true)
        table.insert(self.itemsToClear, cardGroup)

        m_luaWindowRoot:SetActive(trans, true)
        table.insert(self.itemsToClear, trans)
    end

    -- TODO::吃

    -- 杠
    if self:GetIsGang() then
        local len = self:GetMineTurnGangNum()
        for i = 1, len do
            local go = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "mjOp_btn_gang", m_opRoot, RessStorgeType.RST_Never)
            if not go then
                DwDebug.Log("UIMJOpCtrl:InitUI error :no go mjOp_btn_gang")
                return
            end

            go.name = "mjOp_btn_gang" .. i
            local trans = go.transform
            group:AddCell(trans)

            local gang_data = UIMJOpCtrl:GetMineGangData(i)
            local cardGroup = LogicUtil.GenCardGroup(m_luaWindowRoot, self:GetGangType(gang_data), gang_data.pai)
            cardGroup.parent = trans
            cardGroup.localScale = groupScale
            cardGroup.localPosition = vector3.zero

            m_luaWindowRoot:SetActive(cardGroup, true)
            table.insert(self.itemsToClear, cardGroup)

            m_luaWindowRoot:SetActive(trans, true)
            table.insert(self.itemsToClear, trans)
        end
    end

    group:AdjustSize()
end

function UIMJOpCtrl:HideUI(eventId, p1, p2)
    m_luaWindowRoot:SetActive(m_rootTran, false)
    m_luaWindowRoot:SetActive(m_bg, false)
    m_luaWindowRoot:SetActive(m_opRoot, false)

    self:clearItem()
end

-- 获取碰数据
function UIMJOpCtrl:GetPengData()
    if not m_data then
        return
    end
    return m_data.pengPai
end

-- 获取杠数据
function UIMJOpCtrl:GetGangData()
    if not m_data or not m_data.gangPai then
        return nil
    end
    return m_data.gangPai[1]
end

-- 获取胡数据
function UIMJOpCtrl:GetHuData()
    return m_data.huPai
end

-- 获取杠数据
function UIMJOpCtrl:GetMineGangData(index)
    return m_data.gangPai[index]
end

-- 获取杠牌数据
function UIMJOpCtrl:GetGangType(data)
    if data and data.gangType then
        -- 1--°µ¸Ü 2--Ã÷¸Ü 3--×ª½Ç¸Ü
        if MJGangType.AnGang == data.gangType then
            return 4
        elseif MJGangType.MingGang == data.gangType then
            return 3
        elseif MJGangType.ZhuangJiaoGang == data.gangType then
            return 3
        end
    end

    return 3
end

-- 是否有碰牌
function UIMJOpCtrl:GetIsPeng()
    if m_data and m_data.pengPai then
        if m_data.pengPai.pengWithPlayerId ~= 0 then
            return true
        else
            return false
        end
    else
        return false
    end
end

-- 是否有杠牌
function UIMJOpCtrl:GetIsGang()
    if m_data and m_data.gangPai then
        if #m_data.gangPai > 0 then
            return true
        else
            return false
        end
    else
        return false
    end
end

-- 获取杠数量
function UIMJOpCtrl:GetMineTurnGangNum()
    if m_data and m_data.gangPai then
        return #(m_data.gangPai)
    else
        return 0
    end
end

-- 是否存在胡
function UIMJOpCtrl:GetIsHu()
    if m_data and m_data.huPai then
        if not MJHuPaiType.IsMJNoneOrLiuJu(m_data.huPai) then
            return true
        else
            return false
        end
    else
        return false
    end
end
