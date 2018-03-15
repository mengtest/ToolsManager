--[[
@author: mid
@date: 2018年1月8日01:00:13
@desc: 乐安麻将模块
@usage:
必须包括以下方法
1. Init传入self
2. InitList 列表 self
3. 如果有生成对象 要添加到清理队列中去
]]
local lamj = {}
local cfg_itemName = "la_yh_item"
local cfg_fanXingStr =
    {
        [1] = "天胡",
        [2] = "地胡",
        [3] = "平胡",
        [4] = "碰碰胡",
        [5] = "假胡字一色",
        [6] = "真胡字一色",
        [7] = "假清一色",
        [8] = "真清一色",
        [9] = "七对",
        [10] = "十三烂",
        [11] = "七星十三烂",
        [12] = "碰碰胡清一色",
        [13] = "碰碰胡字一色",
        [14] = "七对清一色",
        [15] = "七对胡字一色",
        [16] = "四归一七对",
        [17] = "八归一七对",
        [18] = "十二归一七对",
        [19] = "四归一",
        [20] = "八归一",
        [21] = "十二归一",
        [22] = "清一色\n假胡四归一",
        [23] = "清一色\n真胡四归一",
        [24] = "清一色\n假胡八归一",
        [25] = "清一色\n真胡八归一",
        [26] = "清一色\n假胡十二归一",
        [27] = "清一色\n真胡十二归一",
        [28] = "字一色\n假胡四归一",
        [29] = "字一色\n真胡四归一",
        [30] = "字一色\n假胡八归一",
        [31] = "字一色\n真胡八归一",
        [32] = "字一色\n假胡十二归一",
        [33] = "字一色真胡\n十二归一",
        [34] = "清一色真胡\n四归一七对",
        [35] = "清一色真胡\n八归一七对",
        [36] = "清一色真胡\n十二归一七对",
        [37] = "字一色真胡\n四归一七对",
        [38] = "字一色真胡\n八归一七对",
        [39] = "字一色真胡\n十二归一七对",
    }
local cfg_fanxingScale =
    {
        [20] = 0.3, -- 两行 0.3
        [2] = 0.7, -- 2个字 0.7
        [3] = 0.52, -- 3个字 0.52
        [4] = 0.45, -- 4个字 0.51
        [5] = 0.35, -- 5个字 0.51
        [6] = 0.3, -- 6个字 0.34
    }

local self = nil
-- 初始化
function lamj.Init(_self)
    if not _self then
        DwDebug.Log("lamj.Init error: _self")
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
            players_data[i] = nil
        end
    end
    table.sort(players_data, function(a, b) return a.seatSort < b.seatSort end)
end

-- 初始化列表
function lamj.InitList(self)
    self.ctx:SetActive(self.ctx:GetTrans(cfg_itemName), false)
    local is_show_jiangMa = self.uiData.jiangMa.jiangMaType ~= 0
    DwDebug.Log("jiangMaType"..tostring(self.uiData.jiangMa.jiangMaType)..tostring(is_show_jiangMa))
    lamj.update_JiangMa(self, is_show_jiangMa)
    local players_data = self.uiData.playerMes
    for i = 1, 4 do
        lamj._InitItem(self, i, players_data[i])
    end
end
-- 奖马逻辑
function lamj.update_JiangMa(self, is_show)
    local ctx = self.ctx
    local uiData = self.uiData
    ctx:SetActive(ctx:GetTrans("Other"), is_show)
    local node = ctx:GetTrans("la_yh_jiangma_node")
    ctx:SetActive(node, is_show)
    if is_show then
        local mj_node = ctx:GetTrans(node, "mj_node")
        local not_hit_ids = {}
        local all_ids = uiData.jiangMa.kaiMa
        local hit_ids = uiData.jiangMa.zhongMa

        --test
        -- all_ids = {1,2,3,4,5,6}
        -- hit_ids = {1,2,3}

        local hit_ids_map = {}
        for i=1,#hit_ids do
            hit_ids_map[hit_ids[i]] = true
        end
        for i=#all_ids,1,-1 do
            if not hit_ids_map[all_ids[i]] then
                not_hit_ids[#not_hit_ids+1] = all_ids[i]
            end
        end
        local show_ids = {}
        table.sort(not_hit_ids)
        table.sort(show_ids)
        for i=1,#not_hit_ids do
            show_ids[#show_ids+1] = not_hit_ids[i]
        end
        for i=1,#hit_ids do
            show_ids[#show_ids+1] = hit_ids[i]
        end
        local _trans = LogicUtil.GenCardArray(ctx, mj_node,0, show_ids, 300)
        self.AddToCleanList(_trans)
        mj_node.localScale = UnityEngine.Vector3.New(0.76, 0.76, 0.76)
        mj_node.localPosition = UnityEngine.Vector3.New(-259.74, -292, 0)
        for i = 1, 6 do
            local mask_bg = ctx:GetTrans(ctx:GetTrans(node, "mask_node"), i .. "")
            mask_bg.localPosition = UnityEngine.Vector3.New(54*(i-1),-4,0)
            local is_show_mask = (i <= #not_hit_ids)
            ctx:SetActive(mask_bg, is_show_mask)
        end
    end
    lamj.SwitchJiangMaUI(self, is_show)
end
-- 切换奖马相关节点的坐标变化
function lamj.SwitchJiangMaUI(self, is_show)
    local ctx = self.ctx
    local node_jiangma_move_tran = ctx:GetTrans("node_jiangma_move")
    local btn_share_tran = ctx:GetTrans("btn_share")
    if is_show then
        node_jiangma_move_tran.localPosition = UnityEngine.Vector3.New(100, 0, 0)
        btn_share_tran.localPosition = UnityEngine.Vector3.New(122, -287, 0)
    else
        node_jiangma_move_tran.localPosition = UnityEngine.Vector3.New(0, 0, 0)
        btn_share_tran.localPosition = UnityEngine.Vector3.New(-173.8497, -286.81, 0)
    end
end
-- 初始化玩家列表 子项
function lamj._InitItem(self, index, data)
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
    ctx:SetActive(ctx:GetTrans(node, "txt_fanxing"), is_exist_fanxing)
    if is_exist_fanxing then
        local _fanxingStr = cfg_fanXingStr[data.fanXingId]
        DwDebug.Log("番型字符 " .. tostring(_fanxingStr))
        ctx:SetLabel(ctx:GetTrans(node, "txt_fanxing"), _fanxingStr)
        local _type = nil
        if string.find(_fanxingStr, "\n") then
            _type = 20
        else
            _type = utf8len(_fanxingStr)
        end
        if _type then
            DwDebug.Log(_type)
            local _scale = cfg_fanxingScale[_type]
            -- DwDebug.Log("缩放参数"..tostring(_scale))
            ctx:GetTrans(node, "txt_fanxing").localScale = UnityEngine.Vector3.New(_scale, _scale, _scale)
        else
            -- DwDebug.Log("番型字符不存在")
            ctx:SetActive(ctx:GetTrans(node, "txt_fanxing"), false)
        end
    end
    lamj._GenMJNodes(self, node, data)
end
-- 生成麻将节点
function lamj._GenMJNodes(self, node, data)
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
            group_tran.localPosition = UnityEngine.Vector3.New((group_num - 1) * spacing, 0, 0)
            ctx:SetActive(group_tran, true)
            self.AddToCleanList({group_tran})
        else
            DwDebug.Log("ope type error")
        end
    end
    local start_x = group_num * (70 * 3 + 5) + 5 + (group_num>0 and 5 or 0)
    local _trans = LogicUtil.GenCardArray(ctx, cards, start_x, normalPai, 300)
    self.AddToCleanList(_trans)
    if huPai > 0 then
        local _hu = LogicUtil.GenCardArray(ctx, cards, start_x + (#normalPai+0.5) * 70 + 5, huPai, 300)
        self.AddToCleanList(_hu)
    end
end
return lamj
