--[[
@author: mid
@date: 2018年1月8日11:44:27
@desc: 崇仁麻将模块
@usage:
必须包括以下方法
1. Init传入self
2. InitList 列表
3. 如果有生成对象 要添加到清理队列中去
]]
local crmj = {}
local self = nil
local cfg_itemName = "mj_item"
-- 解析数据
function crmj.Init(_self)
    if not _self then
        DwDebug.Log("crmj.Init error: _self")
        return
    end
    self = _self
    local players_data = self.uiData.playerMes
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
    table.sort(players_data, function(a, b) return a.seatSort < b.seatSort end)
end
-- 初始化列表
function crmj.InitList(self)
    local ctx = self.ctx
    ctx:SetActive(ctx:GetTrans(cfg_itemName), false)
    ctx:SetActive(ctx:GetTrans("Other"), false)
    -- local is_show_jiangMa = self.uiData.jiangMa.jiangMaType ~= 0
    -- crmj.update_JiangMa(self, is_show_jiangMa)
    local players_data = self.uiData.playerMes
    for i = 1, 4 do
        crmj._InitItem(self, i, players_data[i])
    end
end
-- 初始化玩家列表 子项
function crmj._InitItem(self, index, data)
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
    ctx:SetActive(ctx:GetTrans(node, "img_banker"), data.zhuangJia)
    -- 房主标记
    ctx:SetActive(ctx:GetTrans(node, "img_owner"), data.fangZhu)
    -- 名字
    ctx:SetLabel(ctx:GetTrans(node, "txt_name"), data.nickName)
    -- Id
    ctx:SetLabel(ctx:GetTrans(node, "txt_playerId"), "ID:" .. data.userId)
    -- 总积分
    ctx:SetLabel(ctx:GetTrans(node, "txt_total"), data.zongJifen)
    -- 背景高亮 分数>0时候 显示背景高亮 其他情况不显示
    local isWin = data.jifen > 0
    ctx:SetActive(ctx:GetTrans(node, "node_highlight"), isWin)
    -- 赢积分
    ctx:SetActive(ctx:GetTrans(node, "win"), isWin)
    -- 输积分
    ctx:SetActive(ctx:GetTrans(node, "lose"), not isWin)
    if isWin then
        ctx:SetLabel(ctx:GetTrans(node, "win"), "+" .. data.jifen)
    else
        ctx:SetLabel(ctx:GetTrans(node, "lose"), data.jifen)
    end
    -- 自摸标记
    ctx:SetActive(ctx:GetTrans(node, "zimo"), data.huType == MJHuPaiType.ZiMo or data.huType == MJHuPaiType.GangShangHua)
    -- 胡牌标记
    ctx:SetActive(ctx:GetTrans(node, "hu"), data.huType == MJHuPaiType.Hu or data.huType == MJHuPaiType.QiangGang)
    -- 放炮标记
    ctx:SetActive(ctx:GetTrans(node, "pao"), data.fangPao)
    -- 抢杠胡
    ctx:SetActive(ctx:GetTrans(node, "qiangganghu"), data.huType == MJHuPaiType.QiangGang)
    -- 杠上花
    ctx:SetActive(ctx:GetTrans(node, "gangshanghua"), data.huType == MJHuPaiType.GangShangHua)
    -- 番型节点
    local is_exist_fanxing = data.fanXingId > 0
    local fanxing_tran = ctx:GetTrans(node, "fanxing")
    ctx:SetActive(fanxing_tran, is_exist_fanxing)
    if is_exist_fanxing then
        local fanxingId = ctx:GetTrans(node, "fanxing_id")
        ctx:SetSprite(fanxingId, "fanxing_" .. data.fanXingId)
        ctx:MakePixelPerfect(fanxingId)
    end
    crmj._GenMJNodes(self, node, data)
end
-- 生成麻将节点
function crmj._GenMJNodes(self, node, data)
    local ctx = self.ctx
    local cards = ctx:GetTrans(node, "cards")
    local _scale = 0.63
    cards.localScale = UnityEngine.Vector3.New(_scale, _scale, 0)
    cards.localPosition = UnityEngine.Vector3.New(-323, 78, 0)
    local opeList = data.opeList
    local normalPai = data.normalPai
    local huPai = data.huPai
    local group_nodes = {}
    local normal_nodes = {}
    local function getGroupType(ope)
        local opeId = ope.opeId
        local type = -1
        local ids = {}
        if opeId == 1 then
            type = 2
            ids = ope.chiPai.pai
        elseif opeId == 2 then
            type = 1
            ids = ope.pengPai.pai
        elseif opeId == 3 then
            local gangType = ope.gangPai.gangType
            if gangType == 1 then
                type = 4
            elseif gangType == 2 then
                type = 3
            elseif gangType == 3 then
                type = 3
            end
            ids = ope.gangPai.pai
        end
        return type, ids
    end
    -- 生成组合
    local group_num = 0
    local spacing = 70 * 3 + 5
    for i = 1, #opeList do
        local ope = opeList[i]
        local type, ids = getGroupType(ope)
        if type ~= -1 then
            local group_tran = LogicUtil.GenCardGroup(ctx, type, ids, 300)
            group_num = group_num + 1
            group_tran.name = "CardGroup_" .. i
            group_tran.parent = cards
            group_tran.localScale = UnityEngine.Vector3.New(1, 1, 1)
            group_tran.localPosition = UnityEngine.Vector3.New((group_num - 1) * spacing, -1, 0)
            ctx:SetActive(group_tran, true)
            self.AddToCleanList({group_tran})
        else
            DwDebug.Log("ope type error")
        end
    end
    local start_x = group_num * (70 * 3 + 5) + 5 + (group_num > 0 and 5 or 0)
    local _trans = LogicUtil.GenCardArray(ctx, cards, start_x, normalPai, 300)
    self.AddToCleanList(_trans)
    if huPai > 0 then
        local _hu = LogicUtil.GenCardArray(ctx, cards, start_x + (#normalPai + 0.5) * 70 + 5, huPai, 300)
        self.AddToCleanList(_hu)
    end
end
return crmj
