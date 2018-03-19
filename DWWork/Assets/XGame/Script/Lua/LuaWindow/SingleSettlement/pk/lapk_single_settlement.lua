--[[
@author: mid
@date: 2018年1月8日11:37:10
@desc: 乐安扑克模块
@usage:
必须包括以下方法
1. Init传入self
2. InitList 列表
3. 如果有生成对象 要添加到清理队列中去
]]
local lapk = {}
local self = nil
local cfg_itemName = "crwsk_item"
-- 解析数据
function lapk.Init(_self)
    if not _self then
        DwDebug.Log("crmj.Init error: _self")
        return false
    end
    self = _self
    local players_data = self.uiData.players
    local playerMgr = self.playerMgr
    for i = 1, #players_data do
        local _item = players_data[i]
        if next(_item) then
            -- 排序 需要得到对应的客户端座位 seatPos
            _item.seatSort = SeatPosSort[playerMgr:GetPlayerByPlayerID(_item.userId).seatPos]
            -- 派发总积分变化的时候
            _item.seatPos = playerMgr:GetPlayerByPlayerID(_item.userId).seatPos
            -- 截取名字长度
            _item.nickName = utf8sub(_item.nickName, 1, 10)
        else
            _item.seatSort = -1
        end
    end
    -- 根据从小到大的规则排序
    table.sort(players_data, function(a, b) return a.seatSort < b.seatSort end)
    return true
end
-- 初始化列表
function lapk.InitList(self)
    self.ctx:SetActive(self.ctx:GetTrans(cfg_itemName), false)
    local uiData = self.uiData
    local players_data = uiData.players
    -- 如果打独 就都不显示庄家
    if uiData.isAlone then
        for i = 1, #players_data do
            players_data[i].isBanker = false
        end
    end
    local is_exist_friend = (not uiData.isAlone) and uiData.dataValid
    local myUserId = DataManager.GetUserID()
    if is_exist_friend then
        local friendSeatId = -1
        for i = 1, #players_data do
            if players_data[i].userId == myUserId then
                friendSeatId = players_data[i].friendSeatId
                players_data[i].isFriend = true
            else
                players_data[i].isFriend = false
            end
        end
        local userId = self.playerMgr:GetPlayerBySeatID(friendSeatId).seatInfo.userId
        for i = 1, #players_data do
            if players_data[i].userId == userId then
                players_data[i].isFriend = true
            end
        end
    else
        for i = 1, #players_data do
            players_data[i].isFriend = false
        end
    end

    for i = 1, 4 do
        lapk._InitItem(self, i, players_data[i])
    end
end
-- 初始化玩家列表 子项
function lapk._InitItem(self, index, data)
    local ctx = self.ctx
    local node_name = "player_" .. index
    local node = ctx:GetTrans(node_name, true)
    -- 本地化
    if not node then
        local go = GameObject.Instantiate(ctx:GetTrans(cfg_itemName).gameObject)
        go.name = node_name
        node = go.transform
        node.parent = ctx:GetTrans("list")
        node.localScale = UnityEngine.Vector3.New(1, 1, 1)
        node.localPosition = UnityEngine.Vector3.New(0, -120 * (index - 1), 0)
        ctx:SetActive(node, true)
    end
    -- 如果没数据 设置隐藏
    if not data then
        DwDebug.Log("not data")
        ctx:SetActive(node, false)
        return
    end
    local player_data = self.playerMgr:GetPlayerByPlayerID(data.userId)
    if not player_data then
        DwDebug.Log("not player_data")
        return
    end
    -- 头像
    WindowUtil.LoadHeadIcon(ctx, ctx:GetTrans(node, "img_head"), player_data.seatInfo.headUrl, player_data.seatInfo.sex, false, RessStorgeType.RST_Never)
    -- 庄家标记
    ctx:SetActive(ctx:GetTrans(node, "img_banker"), data.isBanker)
    -- 房主标记
    ctx:SetActive(ctx:GetTrans(node, "img_owner"), data.isFangZhu)
    -- 名字
    ctx:SetLabel(ctx:GetTrans(node, "txt_name"), utf8sub(data.nickName, 1, 10))
    -- Id
    ctx:SetLabel(ctx:GetTrans(node, "txt_playerId"), "ID:" .. data.userId)
    -- 总积分
    ctx:SetLabel(ctx:GetTrans(node, "txt_total"), data.totalScore)
    -- 背景高亮 分数>0时候 显示背景高亮 其他情况不显示
    local winScore = data.winScore
    local isWin = winScore > 0
    ctx:SetActive(ctx:GetTrans(node, "node_highlight"), isWin)
    -- 赢积分
    ctx:SetActive(ctx:GetTrans(node, "win"), isWin)
    -- 输积分
    ctx:SetActive(ctx:GetTrans(node, "lose"), not isWin)
    if isWin then
        ctx:SetLabel(ctx:GetTrans(node, "win"), "+" .. winScore)
    else
        ctx:SetLabel(ctx:GetTrans(node, "lose"), winScore)
    end
    -- [[crwsk 特有]]
    -- 打独标识
    ctx:SetActive(ctx:GetTrans(node, "img_isAlone"), data.isAlone)
    -- 好友标识
    ctx:SetActive(ctx:GetTrans(node, "img_isFriend"), data.isFriend)
    -- 名次
    for i = 1, 4 do
        ctx:SetActive(ctx:GetTrans(node, tostring(i)), i == data.overOrder)
    end
    -- 双围
    ctx:SetActive(ctx:GetTrans(node, "smash"), data.isDouble)
    -- 分数
    local score_point = data.winScore - data.bombScore
    ctx:SetLabel(ctx:GetTrans(node, "score_point"), score_point > 0 and "+" .. score_point or tostring(score_point))
    if data.bombOdds > 0 then
        ctx:SetActive(ctx:GetTrans(node, "bomb"), true)
        ctx:SetLabel(ctx:GetTrans(node, "bomb_times"), "x" .. data.bombOdds)
    else
        ctx:SetActive(ctx:GetTrans(node, "bomb"), false)
    end
end

return lapk
