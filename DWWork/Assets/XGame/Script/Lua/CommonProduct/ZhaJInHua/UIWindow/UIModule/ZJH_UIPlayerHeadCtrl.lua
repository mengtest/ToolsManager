--------------------------------------------------------------------------------
-- 	 File      : ZJH_UIPlayerHeadCtrl.lua
--   author    : jianing
--   function   : 扎金花 牌局玩家头像显示控制
--   date      : 2018-02-02
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

require "Logic.UICtrlLogic.UIPlayerHeadCtrl"

ZJH_UIPlayerHeadCtrl = class("ZJH_UIPlayerHeadCtrl",UIPlayerHeadCtrl)

function UIPlayerHeadCtrl:Init(rootTrans,luaWindowRoot)
	self.rootTrans = rootTrans
	self.luaWindowRoot = luaWindowRoot
	-- 屏蔽准备中位置
	self.headPreparePos = {}
	self.headPlayPos = {}

	self.southNodeTrans = luaWindowRoot:GetTrans(rootTrans,"south_node")
	local southplayPosTrans = self.luaWindowRoot:GetTrans(self.southNodeTrans,"play_pos")
	self.southHeadTrans = self:LoadHeadItem(southplayPosTrans, "south")
	self:InitHeadItem(self.southHeadTrans,nil)
	self.luaWindowRoot:SetActive(self.southHeadTrans,false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.southNodeTrans, "SoreAnim_1"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.southNodeTrans, "SoreAnim_2"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.southNodeTrans, "BombAnim"),false)
	self.headPlayPos[1] = southplayPosTrans.position

	self.eastNodeTrans = luaWindowRoot:GetTrans(rootTrans,"east_node")
	local eastplayPosTrans = self.luaWindowRoot:GetTrans(self.eastNodeTrans,"play_pos")
	self.eastHeadTrans = self:LoadHeadItem(eastplayPosTrans, "east")
	self:InitHeadItem(self.eastHeadTrans,nil)
	self.luaWindowRoot:SetActive(self.eastHeadTrans,false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.eastNodeTrans, "SoreAnim_1"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.eastNodeTrans, "SoreAnim_2"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.eastNodeTrans, "BombAnim"),false)
	self.headPlayPos[2] = eastplayPosTrans.position

	self.northNodeTrans = luaWindowRoot:GetTrans(rootTrans,"north_node")
	local northplayPosTrans = self.luaWindowRoot:GetTrans(self.northNodeTrans,"play_pos")
	self.northHeadTrans = self:LoadHeadItem(northplayPosTrans, "north")
	self:InitHeadItem(self.northHeadTrans,nil)
	self.luaWindowRoot:SetActive(self.northHeadTrans,false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.northNodeTrans, "SoreAnim_1"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.northNodeTrans, "SoreAnim_2"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.northNodeTrans, "BombAnim"),false)
	self.headPlayPos[3] = northplayPosTrans.position

	self.westNodeTrans = luaWindowRoot:GetTrans(rootTrans,"west_node")
	local westplayPosTrans = self.luaWindowRoot:GetTrans(self.westNodeTrans,"play_pos")
	self.westHeadTrans = self:LoadHeadItem(westplayPosTrans, "west")
	self:InitHeadItem(self.westHeadTrans,nil)
	self.luaWindowRoot:SetActive(self.westHeadTrans,false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.westNodeTrans, "SoreAnim_1"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.westNodeTrans, "SoreAnim_2"),false)
	self.luaWindowRoot:SetActive(luaWindowRoot:GetTrans(self.westNodeTrans, "BombAnim"),false)
	self.headPlayPos[4] = westplayPosTrans.position

	self:Destroy()
	self:RegisterEvent()
	UpdateSecond:Add(self.UpdateSec, self)
	self.timerList = {}
end