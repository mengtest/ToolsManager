--------------------------------------------------------------------------------
--   File      : CIAPSys.lua
--   author    : guoliang
--   function   : 支付系统
--   date      : 2017-09-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "NetWork.WebNetHelper"

CIAPSys = {}
local _s = CIAPSys
_s.isRegisteUpdate = false
_s.storeItemList = {}--商品列表
_s.orderList = {}--订单列表（包括服务端未验证的)
_s.activeOrderList = {}

_s.delayTime = 0
_s.curWorkProductID = nil
function CIAPSys.Init()
   
end

function CIAPSys.Reset()

end

function CIAPSys.Destroy()
    
end


function CIAPSys.RequestShopItemList(sucCB)
    _s.activeOrderList = {}
    WebNetHelper.RequestStoreList(function (rsp,head)
        if rsp and rsp.data then
            _s.storeItemList = rsp.data.list
            if sucCB then
                sucCB()
            end
        end
    end)

end

function CIAPSys.GetOrderNumByCheckStrMd5(checkStr)
    local checkStr_md5 = LuaUtil.MD5(checkStr)
    for k,v in pairs(_s.orderList) do
        if v.checkStr_md5 == checkStr_md5 then
            return v.orderNum
        end
    end
    return nil
end


--客户端发起购买
function CIAPSys.SendBuyItem(saleIndex)
    if WrapSys.IsIOS then
        if saleIndex > 0 and saleIndex <= #_s.storeItemList then
            if _s.storeItemList[saleIndex] then
                 --WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.wait_ui_window, true, 2, "", true , nil)
				 WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, 20000+20, "wait_ui_window")
                 WrapSys.CIAPSys_SendBuyDiamond(_s.storeItemList[saleIndex].product_id)
                _s.curWorkProductID = _s.storeItemList[saleIndex].product_id
            end
        else
            WindowUtil.LuaShowTips("商品不存在")
            LuaEvent.AddEventNow(EEventType.UI_StoreWindowBtnGray,false)
        end
    else
        WindowUtil.LuaShowTips("后续开放，敬请期待")
        LuaEvent.AddEventNow(EEventType.UI_StoreWindowBtnGray,false)
    end
end

--检查App Store 是否成功返回购买回执
function CIAPSys.CheckCBStr(cbStr)
    local strArray = LuaUtil.StringSplit(cbStr, ",")
    if strArray[1] == "1" then
        if #strArray >= 4 then
            local svrPid = strArray[2]
            local serialNumber = strArray[3]
            local checkStr = strArray[4]

            return svrPid,serialNumber,checkStr
        end
    end
end
--客户端发起内购的appstore 返回回调
function CIAPSys.HandleBuyIAPCB(cbStr)
    DwDebug.Log("HandleBuyIAPCB "..cbStr)
    --当前下发的回执对应的订单号
    local svrPid,serialNumber,checkStr = _s.CheckCBStr(cbStr)
    if svrPid and checkStr then
        local checkStr_md5 = LuaUtil.MD5(checkStr)
        if _s.orderList[checkStr_md5] == nil then
            _s.orderList[checkStr_md5] = {}
        end
        _s.orderList[checkStr_md5].svrPid = svrPid
        _s.orderList[checkStr_md5].serialNumber = serialNumber
        _s.orderList[checkStr_md5].checkStr = checkStr

        _s.activeOrderList[checkStr_md5] = _s.orderList[checkStr_md5]

        CIAPSys.SendCheckInfoToSvr(svrPid,checkStr,checkStr_md5)
    elseif cbStr == "0" then -- 用户主动放弃购买调用或者本次购买失败
        CIAPSys.BuyFail(cbStr)
    end

end

-- 向服务端发送内购回执
function CIAPSys.SendCheckInfoToSvr(svrPid,checkStr,checkStr_md5)

    if svrPid and checkStr and checkStr_md5 then
        DwDebug.Log("CIAPSys.SendCheckInfoToSvr "..svrPid .. " checkStr "..checkStr)
        -- 发送内购回执服务端
        WebNetHelper.RequestCheckBuyResult(svrPid,checkStr,function (rsp,head)
            --成功
             _s.BuySuccess(svrPid,checkStr_md5,rsp)
        end,function (rsp,head)
            _s.BuyFail(svrPid)
            --验证超时，开启定时查询
            if head and head.error == -1 then
                if WrapSys.IsIOS then
                    _s.StardCheckUpdate(true)
                end
            end
        end)

    else

    end
    
end
--向服务端查询当前支付状态
function CIAPSys.SendAskCheckResult()
    DwDebug.Log("CIAPSys.SendAskCheckResult ")
    for k,v in pairs(_s.activeOrderList) do
        WebNetHelper.RequestSvrCheckStatus(v,function (rsp,head)
            _s.BuySuccess(v.svrPid,k,rsp)
        end,function ()
            
        end)
    end

end


--支付成功
function CIAPSys.BuySuccess(svrPid,checkStr_md5,rsp)
    if svrPid and checkStr_md5 and rsp then
        DwDebug.Log("CIAPSys.BuySuccess "..svrPid) 

        LuaEvent.AddEventNow(EEventType.UI_StoreWindowBtnGray,false)
        --WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.wait_ui_window, false,0,"")
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")

        if rsp.data then
            if rsp.data.status == 0 then
                local orderInfo = _s.orderList[checkStr_md5]
                if orderInfo then
                    WrapSys.CIAPSys_PurchaseSucceed(orderInfo.svrPid,orderInfo.serialNumber)
                    _s.orderList[checkStr_md5] = nil
                    _s.activeOrderList[checkStr_md5] = nil

                    if #_s.activeOrderList <= 0 then
                        _s.StardCheckUpdate(false)
                    end
                end
                if _s.curWorkProductID and _s.curWorkProductID == svrPid then
                    WindowUtil.LuaShowTips("商品购买成功")
                    _s.curWorkProductID = nil
                end

                if DataManager.GetRoomCardNum() ~= rsp.data.card_number then
                    DataManager.SetRoomCardNum(rsp.data.card_number)
                    LuaEvent.AddEventNow(EEventType.RefreshUserInfo)
                end
            elseif rsp.data.status == 2 then
                --服务端订单处理失败
                WindowUtil.LuaShowTips("服务端正在处理...")
                _s.activeOrderList[checkStr_md5] = nil

                if _s.curWorkProductID and _s.curWorkProductID == svrPid then
                    WindowUtil.LuaShowTips("商品购买成功")
                    LuaEvent.AddEventNow(EEventType.UI_StoreWindowBtnGray,false)
                    --WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.wait_ui_window, false,0,"")
					WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
                    _s.curWorkProductID = nil
                end

                if #_s.activeOrderList <= 0 then
                    _s.StardCheckUpdate(false)
                end
            end
        end
    end
end

--支付失败
function CIAPSys.BuyFail(svrPid)
    DwDebug.Log("CIAPSys.BuyFail")
    if (_s.curWorkProductID and _s.curWorkProductID == svrPid) or svrPid == "0"  then
        LuaEvent.AddEventNow(EEventType.UI_StoreWindowBtnGray,false)
        --WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.wait_ui_window, false,0,"")
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "wait_ui_window")
        _s.curWorkProductID = nil
    end
end

function CIAPSys.StardCheckUpdate(isStart)
    if isStart then
        _s.delayTime = 5
        if not _s.isRegisteUpdate then
            UpdateSecond:Add(_s.UpdateSec)
            _s.isRegisteUpdate = true
        end
    else
        _s.delayTime = 0
         if _s.isRegisteUpdate then
            UpdateSecond:Remove(_s.UpdateSec)
            _s.isRegisteUpdate = false
        end
    end
end

function CIAPSys.UpdateSec()
    if _s.delayTime > 0 then
        _s.delayTime = _s.delayTime - 1
        if _s.delayTime <= 0 then
            _s.delayTime = 5
            --轮询服务器是否已经验证成功
            _s.SendAskCheckResult()
        end
    end
end