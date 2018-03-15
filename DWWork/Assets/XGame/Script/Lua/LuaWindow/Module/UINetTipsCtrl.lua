--------------------------------------------------------------------------------
-- 	 File      : UINetTipsCtrl.lua
--   author    : guoliang
--   function   : 网络提示控件
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

UINetTipsCtrl = class("UINetTipsCtrl",nil)

function UINetTipsCtrl:Init(luaWindowRoot,parentTrans,netReqAction)
	self.luaWindowRoot = luaWindowRoot
	self.parentTrans = parentTrans
	self.netReqAction = netReqAction
	self.loadingTipDelayTime = 0.5
	self.countTime = 0
	self.timeoutTime = 8
    self.isRegsiteUpdate = false

    self:AddUIShowItem(parentTrans)
end

function UINetTipsCtrl:Destroy()
	if self.timerIndex then
		TimerTaskSys.RemoveTask(self.timerIndex)
		self.timerIndex = nil
	end
	self.luaWindowRoot = nil
	self.netReqAction = nil
end

--添加显示节点
function UINetTipsCtrl:AddUIShowItem(parentTrans)
	if parentTrans == nil then
		return
	end
	local netTipsItemTrans = self.luaWindowRoot:GetTrans(parentTrans, "netTips_item")
	if netTipsItemTrans == nil then
		local resObj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "netTips_item", RessStorgeType.RST_Never, false)
		if resObj then
			netTipsItemTrans = resObj.transform
			netTipsItemTrans.parent = parentTrans
			self.luaWindowRoot:SetActive(netTipsItemTrans,true)
			netTipsItemTrans.name = "netTips_item"
			netTipsItemTrans.localScale = UnityEngine.Vector3.New(1,1,1)
			netTipsItemTrans.localPosition = UnityEngine.Vector3.New(0,0,0)
		end
	end

	self.loadingTrans = self.luaWindowRoot:GetTrans(netTipsItemTrans,"loading_tip")
	self.loadFailTrans = self.luaWindowRoot:GetTrans(netTipsItemTrans,"load_failed_tip")
	self:HideAllUIShow()
end
-- 加载中显示
function UINetTipsCtrl:LoadingTipsShow()
	if self.isWaitNetMsg then
		self.luaWindowRoot:SetActive(self.loadingTrans,true)
		self.luaWindowRoot:SetActive(self.loadFailTrans,false)
	else
		self:HideAllUIShow()
	end
end

-- 加载失败显示
function UINetTipsCtrl:LoadFailTipsShow()
	if self.isWaitNetMsg then
		self.luaWindowRoot:SetActive(self.loadingTrans,false)
		self.luaWindowRoot:SetActive(self.loadFailTrans,true)
	else
		self:HideAllUIShow()
	end
end

-- 隐藏组件所有UI
function UINetTipsCtrl:HideAllUIShow()
	self.luaWindowRoot:SetActive(self.loadingTrans,false)
	self.luaWindowRoot:SetActive(self.loadFailTrans,false)
end


-----------------------外部调用接口 ----------------------------
--刷新对应回调处理（用于一个界面有页签切换的情况）
function UINetTipsCtrl:RefreshNetReqAction(netReqAction)
	self.netReqAction = netReqAction
end

--组件开始工作(外部接口)
function UINetTipsCtrl:StartWork(...)
	self.back_params = {...}
	self.isWaitNetMsg = true
	if self.netReqAction then
		self.netReqAction(...)
	end

	if self.timerIndex then
		TimerTaskSys.RemoveTask(self.timerIndex)
		self.timerIndex = nil
	end
	self.timerIndex = TimerTaskSys.AddTimerEventByLeftTime(self.LoadingTipsShow,self.loadingTipDelayTime,self)
end

--组件停止工作(外部接口)
function UINetTipsCtrl:StopWork(isNetMsgSuc)
	if not isNetMsgSuc then
		if self.luaWindowRoot and self.luaWindowRoot.m_open then
			self:LoadFailTipsShow()
		end
	else
		self:HideAllUIShow()
	end
	self.isWaitNetMsg = false
	if self.timerIndex then
		TimerTaskSys.RemoveTask(self.timerIndex)
		self.timerIndex = nil
	end
end
-- 组件点击重新加载
function UINetTipsCtrl:HandleWidgetClick(gb)
	if gb then
		if gb.name == "btn_reload" then
			self:StartWork(unpack(self.back_params))
		end
	end
end
