local UITools = {}

-- @params:
-- @scrolView_trans: 列表 transform
-- @item_trans: 列表项 transform
-- @itemBox_x:
-- @itemBox_y:
-- @canSeeNum_x:
-- @canSeeNum_y:
-- @layout_type: 1 横着 2 竖着
-- @isShowShadow: 阴影
-- @itemCallbackFunNameStr 列表项刷新函数名
function UITools.CreateScrollView(scrolView_trans, item_trans, itemBox_x, itemBox_y, canSeeNum_x, canSeeNum_y, layout_type, isShowShadow, itemCallbackFunNameStr)
    local direction = nil
    if layout_type == 1 then
        direction = LimitScrollViewDirection.SVD_Vertical
    elseif layout_type == 2 then
        direction = LimitScrollViewDirection.SVD_Horizontal
    end
    local scrollviewObj = EZfunLimitScrollView.GetOrAddLimitScr(scrolView_trans) --生成对象
    scrollviewObj:InitForLua(scrolView_trans, item_trans.gameObject, UnityEngine.Vector2.New(itemBox_x, itemBox_y), UnityEngine.Vector2.New(canSeeNum_x, canSeeNum_y), direction, isShowShadow)
    scrollviewObj:SetInitItemCallLua(itemCallbackFunNameStr) -- 注册函数回调
    return scrollviewObj
end



-- 传入列表节点 子项节点 子项逻辑 整个列表数据 偏移量数值
function UITools.GenScrollView(scrollview_trans, item_trans, item_fun, scrollview_data, vec2_1, vec2_2, refreshIndex, funCbNameStr, funCb)
    local scrollviewObj = EZfunLimitScrollView.GetOrAddLimitScr(scrollview_trans)
    scrollviewObj:InitForLua(scrollview_trans, item_trans.gameObject, vec2_1, vec2_2, LimitScrollViewDirection.SVD_Vertical, false)
    -- local function refreshList(curList_data)
    --     scrollviewObj:InitItemCount(#(curList_data), true)
    -- end
    local function _item_fun(item_node_tran, item_index)
        -- item_fun(item_node_tran, item_index, scrollview_data[item_index + 1])
        item_fun(item_node_tran, item_index)
    end
    local item_fun_handler = LuaCsharpFuncSys.RegisterFunc(_item_fun)
    scrollviewObj:SetInitItemCallLua(item_fun_handler)
    -- refreshList(scrollview_data)
    return scrollviewObj, item_fun_handler
end

return UITools
