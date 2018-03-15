--------------------------------------------------------------------------------
--   File      : ClubUtil.lua
--   author    : zx
--   function   : 俱乐部共用函数
--   date      : 2018-01-29
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
ClubUtil = {}
local _s = ClubUtil

-- 设置俱乐部Icon
function ClubUtil.SetClubIcon(luaWindowRoot, trans, iconIndex)
	local spriteName = "clubData_headImg_" .. iconIndex
	luaWindowRoot:SetSprite(trans, spriteName)
end

-- 是否是部长
function ClubUtil.IsClubChief(chiefID)
	return DataManager.GetUserID() == chiefID
end

-- 获取修改默认字段
function ClubUtil.GetDefultInfo(key, isClubChief)
	if not isClubChief then
		return ""
	end

	-- 名字
	if 1 == m_state then
		return "不超过5个汉字"
	-- 微信号
	elseif 2 == m_state then
		return "请填写您的微信，以便群内成员联系"
	-- 手机号
	elseif 3 == m_state then
		return "请填写您的手机号，以便群内成员联系"
	end

	return ""
end

_s.shareTimeValue = WrapSys.GetTimeValue()
-- 俱乐部分享
function ClubUtil.OpenShare(parent, luaWindowRoot, id, data)
	if _s.shareTimeValue.Value > 0 then
		WindowUtil.LuaShowTips("您点击太快,请稍后再试")
		return
	end
	_s.shareTimeValue.Value = 5

	-- 分享界面设置
	local func = function (data)
		local resObj = WrapSys.ResourceMgr_GetInsAssetInParent(RessType.RT_UIItem, "clubshare_ui", parent, RessStorgeType.RST_Never)
		if not resObj then
			DwDebug.LogError("init toggle no foud asset:clubshare_ui")
			return
		end

		-- 使用fx层级，单独摄像机描绘
		WindowRoot.SetLayer(resObj, 15)
		local trans = resObj.transform
		luaWindowRoot:SetActive(trans, true)

		luaWindowRoot:SetLabel(luaWindowRoot:GetTrans(trans, "club"), (nil == data or nil == data.name or "" == data.name) and "暂无" or data.name)
		luaWindowRoot:SetLabel(luaWindowRoot:GetTrans(trans, "weixin"), (nil == data or nil == data.owner_weixin or "" == data.owner_weixin) and "暂无" or data.owner_weixin)
		luaWindowRoot:SetLabel(luaWindowRoot:GetTrans(trans, "member"), (nil == data or nil == data.user_number) and "1" or data.user_number)
		luaWindowRoot:SetLabel(luaWindowRoot:GetTrans(trans, "game"), (nil == data or nil == data.main_play or "" == data.main_play) and "暂无" or data.main_play)
		-- 按位拆分
		local id = (nil == data or nil == data.id) and 0 or data.id
		for i=1,6 do
			luaWindowRoot:SetLabel(luaWindowRoot:GetTrans(trans, "id" .. i), id % 10)
			id = math.floor(id / 10)
		end
		luaWindowRoot:SetLabel(luaWindowRoot:GetTrans(trans, "id"), (nil == data or nil == data.id) and "000000" or data.id)
		-- 更新二维码
		luaWindowRoot:LoadImag(luaWindowRoot:GetTrans(trans, "QrCode"), LoginSys.QRPicPath, "", false, RessStorgeType.RST_Never)

		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.err_tips_ui_window, false, 0, "", false , nil)
		local picPath = HallSys.GetSharePath()

		-- local screenWidth, screenHeight = WrapSys.GameRoot_Screen_width(), WrapSys.GameRoot_Screen_height()
		-- local screenWidth = screenHeight * 16 / 9 -- 屏幕16 / 9 - 1280 / 720
		-- local scaleX, scaleY = screenWidth / 1280, screenHeight / 720
		local screenWidth, screenHeight = 1280, 720
		local scaleX, scaleY = 1, 1
		local width = 1136 * scaleX
		local height = 640 * scaleY
		local startX = (screenWidth - width) * 0.5
		local startY = (screenHeight - height) * 0.5

		-- DwDebug.LogError("swidth, sheight", screenWidth, screenHeight)
		-- DwDebug.LogError("width, height", width, height)
		-- DwDebug.LogError("scaleX, scaleY", scaleX, scaleY)
		-- DwDebug.LogError("startX, startY", startX, startY)

		-- startX, startY, width, height = 0, 0, 1280, 720
		WrapSys.GetScreenTexturePic(luaWindowRoot:GetTrans(trans, "UICamera"):GetComponent("Camera"), startX, startY, width, height, picPath
		,function()
			luaWindowRoot:SetActive(trans, false)
			HallSys.WeiXinShareUrl("", "", picPath,""
			,function (isSuccess)
				if isSuccess then
					DwDebug.Log("culb share Success")
				else
					-- DwDebug.Log("culb share Fail")
					WindowUtil.LuaShowTips("分享失败，请重新再试")
				end
			end, false)
		end)
	end

	-- 获取最新数据
	if nil == data then
		LuaEvent.AddEventNow(EEventType.ShowWaitWindow, true)
		WebNetHelper.RequestClubDetail(id,
			function (body, head)
				LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
				if 0 == body.errcode then
					func(body.data)
				else
					WindowUtil.LuaShowTips(body.errmsg)
				end
			end,
			function (body, head)
				if nil ~= body and 0 ~= body.errcode and "" ~= body.errmsg then
					WindowUtil.LuaShowTips(body.errmsg)
				else
					WindowUtil.LuaShowTips("分享超时，请重新再试")
				end
				LuaEvent.AddEventNow(EEventType.ShowWaitWindow, false)
			end)
	else
		func(data)
	end
end
