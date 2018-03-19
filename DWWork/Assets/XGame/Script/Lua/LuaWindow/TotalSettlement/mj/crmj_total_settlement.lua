--[[
@author: mid
@date: 2017年12月25日09:57:11
@desc: 崇仁麻将模块
@usage:
必须包括以下方法
1. Init传入self
2. InitList 列表
3. 如果有生成对象 要添加到清理队列中去
]]
local crmj = {}
local self = nil
local cfg_itemName = "crmj_item"
local cfg_scrollview_item = "crmj_item2"
function crmj.Init(_self)
    if not _self then
        DwDebug.Log("crmj.Init total error: _self")
        return false
    end
    self = _self
    local players_data = self.uiData.playerMes
    local playerMgr = self.playerMgr
    if not playerMgr then
        DwDebug.Log("玩家模块无数据 playerMgr")
        return
    end
    if not players_data then
        DwDebug.Log("协议数据里玩家列表数据为空")
        return
    end
    for i = 1, #players_data do
        local _item = players_data[i]
        if next(_item) then
            -- 排序 需要得到对应的客户端座位 seatPos
            local _player_data = playerMgr:GetPlayerByPlayerID(_item.userId)
            if _player_data then
                _item.seatSort = SeatPosSort[_player_data.seatPos]
                -- 派发总积分变化的时候
                _item.seatPos = _player_data.seatPos
                -- 截取名字长度
                _item.nickName = utf8sub(_item.nickName, 1, 10)
            end
        else
            players_data[i] = nil
        end
    end
    -- 根据从小到大的规则排序
    table.sort(players_data, function(a, b) return a.seatSort < b.seatSort end)
    crmj._list_item_fun_handler = {}
    crmj._scrollviewObjs = {}
    return true
end
-- 初始化列表
function crmj.InitList(self)
    local ctx = self.ctx
    ctx:SetActive(ctx:GetTrans(cfg_itemName), false)
    local max_score = 0
    local players_data = self.uiData.playerMes
    for i = 1, #players_data do
        local score = players_data[i].zongJifen
        if max_score < score then
            max_score = score
        end
    end
    for i = 1, #players_data do
        players_data[i].max_score = max_score
    end
    for i = 1, 4 do
        crmj._InitItem(self, i, players_data[i])
    end
    -- self.AddToClean(function()
    --     LuaCsharpFuncSys.UnRegisterFunc()
    -- end)
end
-- 传入列表节点 子项节点 子项逻辑 整个列表数据 偏移量数值
local function genScrollView(scrollview_trans, item_trans, item_fun, scrollview_data, vec2_1, vec2_2)
    local scrollviewObj = EZfunLimitScrollView.GetOrAddLimitScr(scrollview_trans)
    scrollviewObj:InitForLua(scrollview_trans, item_trans.gameObject, vec2_1, vec2_2, LimitScrollViewDirection.SVD_Vertical, false)
    local function _item_fun(item_node_tran, item_index)
        item_fun(item_node_tran, item_index, scrollview_data[item_index + 1])
    end
    local item_fun_handler = LuaCsharpFuncSys.RegisterFunc(_item_fun)
    scrollviewObj:SetInitItemCallLua(item_fun_handler)
    return scrollviewObj, item_fun_handler
end
-- 初始化玩家列表 子项
function crmj._InitItem(self, index, data)
    local ctx = self.ctx
    local node_name = "player_" .. index
    local node = ctx:GetTrans("list"):Find(node_name)
    -- 本地化
    if not node then
        local go = GameObject.Instantiate(ctx:GetTrans(cfg_itemName).gameObject)
        go.name = node_name
        node = go.transform
        node.parent = ctx:GetTrans("list")
        node.localScale = UnityEngine.Vector3.New(1, 1, 1)
        node.localPosition = UnityEngine.Vector3.New(275 * (index - 1), 0, 0)
        ctx:SetActive(node, true)
    end
    -- 如果没数据 设置隐藏
    if not data then
        DwDebug.Log(" _InitItem not data")
        ctx:SetActive(node, false)
        return
    end
    local playerMgr = self.playerMgr
    local player_data = playerMgr:GetPlayerByPlayerID(data.userId)
    -- 检测
    if not player_data then
        DwDebug.Log("not player_data")
        return
    end
    -- 头像
    WindowUtil.LoadHeadIcon(ctx, ctx:GetTrans(node, "img_head"), player_data.seatInfo.headUrl, player_data.seatInfo.sex, false, RessStorgeType.RST_Never)
    -- 庄家标记
    ctx:SetActive(ctx:GetTrans(node, "img_banker"), false)
    -- 房主标记
    ctx:SetActive(ctx:GetTrans(node, "img_owner"), data.fangZhu)
    -- 名字
    ctx:SetLabel(ctx:GetTrans(node, "txt_name"), data.nickName)
    -- Id
    ctx:SetLabel(ctx:GetTrans(node, "txt_playerId"), "ID:" .. data.userId)
    -- 总积分
    ctx:SetLabel(ctx:GetTrans(node, "txt_total"), data.zongJifen)
    -- 大赢家标记
    ctx:SetActive(ctx:GetTrans(node, "img_bigestWinner"), (data.zongJifen == data.max_score and data.max_score > 0))
    -- 背景高亮 分数>0时候 显示背景高亮 <0显示蓝色背景
    local isWin = data.zongJifen > 0
    ctx:SetActive(ctx:GetTrans(node, "win_bg"), isWin)
    ctx:SetActive(ctx:GetTrans(node, "lose_bg"), not isWin)
    -- 赢积分
    ctx:SetActive(ctx:GetTrans(node, "txt_win_point"), isWin)
    -- 输积分
    ctx:SetActive(ctx:GetTrans(node, "txt_lose_point"), not isWin)
    if isWin then
        ctx:SetLabel(ctx:GetTrans(node, "txt_win_point"), "+" .. data.zongJifen)
    else
        ctx:SetLabel(ctx:GetTrans(node, "txt_lose_point"), data.zongJifen)
    end
    -- 得到列表数据

    local scrollview_data = data.eachGameJifen
    for i = 1, #scrollview_data do
        scrollview_data[i].isWin = isWin
    end
    local scrollviewObj = crmj._scrollviewObjs[index]
    if not scrollviewObj then
    -- 列表逻辑
        local scrollview_trans = ctx:GetTrans(node, "scrollview")
        local item_trans = ctx:GetTrans(cfg_scrollview_item)
        ctx:SetActive(item_trans, false)
        local function item_fun(item_node_tran, item_index, item_data)
            -- print("--item fun test "..tostring(item_data)..tostring(item_node_tran.name)..item_index)
            -- item_data = scrollview_data[item_index]
            ctx:SetActive(ctx:GetTrans(item_node_tran, "win"), item_data.isWin)
            ctx:SetActive(ctx:GetTrans(item_node_tran, "lose"), not item_data.isWin)
            ctx:SetLabel(ctx:GetTrans(ctx:GetTrans(item_node_tran, isWin and "win" or "lose"), "point"), item_data.jifen)
            ctx:SetLabel(ctx:GetTrans(ctx:GetTrans(item_node_tran, isWin and "win" or "lose"), "round_id"), item_index + 1)
        end
        -- 生成列表组件
        local _scrollviewObj, item_fun_handler = genScrollView(scrollview_trans, item_trans, item_fun, scrollview_data, UnityEngine.Vector2.New(5, 6), UnityEngine.Vector2.New(1, 8))
        crmj._scrollviewObjs[index] = _scrollviewObj
        scrollviewObj = _scrollviewObj
        crmj._list_item_fun_handler[#crmj._list_item_fun_handler + 1] = item_fun_handler
    end
    scrollviewObj:InitItemCount(#(scrollview_data), true)
end
-- 清理
function crmj.clean()
    local list = crmj._list_item_fun_handler
    if not list then return end
    for i = 1, #list do
        LuaCsharpFuncSys.UnRegisterFunc(list[i])
    end
    crmj._scrollviewObjs = {}
end
return crmj
