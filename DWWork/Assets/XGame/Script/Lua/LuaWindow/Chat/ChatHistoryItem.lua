--------------------------------------------------------------------------------
-- 	 File      : NimChatSys.lua
--   author    : jianing
--   function  : 单条历史记录刷新 由于会动态销毁创建  这里只做辅助工作 不能作为单独的类
--   date      : 2017-11-21
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
--ChatHistoryItem = class("ChatHistoryItem",nil)
ChatHistoryItem = {}
local timerTask --定时器
local playTrans	--正在播放动画的trans 不允许同时存在2条的

function ChatHistoryItem:init(trans, luaWindow)
	self.luawindow = luaWindow
	self.trans = trans
	self.item_self 			= self.luawindow:GetTrans(trans, "historyitem_self")
	self.item_other			= self.luawindow:GetTrans(trans, "historyitem_other")

	--节点被复用了 之前的定时器要取消
	if playTrans == trans and playTrans then
		TimerTaskSys.RemoveTask(timerTask)
		playTrans = nil
		timerTask = nil
	end
end

function ChatHistoryItem:setData(datatable,index)
	self.datatable = datatable
	local item = nil
	if self.datatable.senderID == DataManager.GetUserID() then
		item = self.item_self
		self.isself = true
	else
		item = self.item_other
		self.isself = false
	end
	self.luawindow:SetActive(self.item_other, not self.isself)
	self.luawindow:SetActive(self.item_self, self.isself)
	self:_getUI(item)
end

function ChatHistoryItem:_getUI(curtrans)
	self.Sprite_msg_bg 		= self.luawindow:GetTrans(curtrans, "Sprite_msg_bg")
	self.msg_head 			= self.luawindow:GetTrans(curtrans, "ico_head")
	self.msg_name 			= self.luawindow:GetTrans(curtrans, "Label_user_name")
	self.msg_voice_img 		= self.luawindow:GetTrans(curtrans, "Sprite_voice")
	self.msg_point_parent 	= self.luawindow:GetTrans(curtrans, "Panel_point")
	self.msg_text 			= self.luawindow:GetTrans(curtrans, "Label_msg_text")
	self.msg_redpoint		= self.luawindow:GetTrans(curtrans, "Sprite_red_point")
	self.msg_context		= self.luawindow:GetTrans(curtrans, "Label_msg_text_saying")
	self.msg_bg_UISprite	= self.Sprite_msg_bg.gameObject:GetComponent("UISprite")
	self:_setUI()
end

function ChatHistoryItem:_setUI( )
	self.luawindow:SetLabel(self.msg_name, self.datatable.senderName, false)
	local playerinfo = self.datatable.playerinfo

	WindowUtil.LoadHeadIcon(self.luawindow, self.msg_head, playerinfo.seatInfo.headUrl, playerinfo.seatInfo.sex,false,RessStorgeType.RST_Never)
	if self.datatable.msgType == 1  then	--文字
		self.luawindow:SetActive(self.msg_text, true)
		self.luawindow:SetActive(self.msg_voice_img, false)
		self.luawindow:SetActive(self.msg_point_parent, false)
		self.luawindow:SetActive(self.msg_redpoint, false)
		self.luawindow:SetActive(self.msg_context, false)

		self.luawindow:SetLabel(self.msg_text, self.datatable.msgContent,false)
		local text = self.msg_text.gameObject:GetComponent("UILabel")
		text.fontSize = 27
		if text.localSize.x > 370 then
			text.fontSize = 21
		end
		local rect = self.Sprite_msg_bg.gameObject:GetComponent("UISprite")
		self.msg_bg_UISprite.width = text.localSize.x + 30
	elseif self.datatable.msgType == 2 then	--语音
		self.luawindow:SetActive(self.msg_text, false)
		self.luawindow:SetActive(self.msg_voice_img, true)
		self.luawindow:SetActive(self.msg_point_parent, false)
		self.luawindow:SetActive(self.msg_redpoint, self.datatable.isFirst)
		self.luawindow:SetActive(self.msg_context, false)
		self.msg_bg_UISprite.width = 70
	end
end

--播放声音动画
function ChatHistoryItem:OnPlayVoice(luawindow,trans,data)
	if data.senderID == DataManager.GetUserID() then
		trans = luawindow:GetTrans(trans, "historyitem_self")
	else
		trans = luawindow:GetTrans(trans, "historyitem_other")
	end
	local msg_context = luawindow:GetTrans(trans, "Label_msg_text_saying")
	local msg_redpoint = luawindow:GetTrans(trans, "Sprite_red_point")
	local msg_bg_UISprite = luawindow:GetTrans(trans, "Sprite_msg_bg").gameObject:GetComponent("UISprite")

	luawindow:SetActive(msg_context, true)
	luawindow:SetActive(msg_redpoint, false)
	msg_bg_UISprite.width = 200

	playTrans = trans
	timerTask = TimerTaskSys.AddTimerEventByLeftTime(function ()
		luawindow:SetActive(msg_context, false)
		msg_bg_UISprite.width = 70

		playTrans = nil
		timerTask = nil
	end, data.timelength + 0.1, nil)
end

function ChatHistoryItem:OnStopVoice()
	if timerTask then
		local task = TimerTaskSys.GetTask(timerTask)
		task.excutTime = 0
	end
end

--点击单条
function ChatHistoryItem:TouchOnItem(trans,datatable)
	if datatable.senderID == DataManager.GetUserID() then
		trans = self.luawindow:GetTrans(trans, "historyitem_self")
	else
		trans = self.luawindow:GetTrans(trans, "historyitem_other")
	end
	self.luawindow:SetActive(self.luawindow:GetTrans(trans, "Sprite_red_point"), false)
	datatable.isFirst = false
end
