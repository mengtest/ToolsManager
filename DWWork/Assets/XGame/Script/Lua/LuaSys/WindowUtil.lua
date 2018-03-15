--------------------------------------------------------------------------------
-- 	 File      : WindowUtil.lua
--   author    : guoliang
--   function   : 窗口工具类
--   date      : 2017-10-9
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
WindowUtil = {}
local _s = WindowUtil

-- 通用错误弹窗
function WindowUtil.ShowErrorWindow(winType,content,okFunc,yesFunc,noFunc,secContent,okSpriteName,noSpriteName)
	local params = {}
	params.winType = winType
	params.errorWindow_content = content
	params.errorWindow_okFunc = okFunc
	params.errorWindow_yesFunc = yesFunc
	params.errorWindow_noFunc = noFunc
	params.errorWindow_secContent = secContent
	params.errorWindow_okSpName = okSpriteName
	params.errorWindow_noSpName = noSpriteName

	if nil == _s.errorParams then
		_s.errorParams = {}
	end

	table.insert( _s.errorParams,1, params )
	_s.PopErrorWindow()
end

-- 获取列表第一个数据
function WindowUtil.PopErrorParams(needDel)
	if needDel and not isNilOrEmpty(_s.errorParams) then
		table.remove(_s.errorParams, 1)
	end

	if isNilOrEmpty(_s.errorParams) then
		-- _s.currParams = nil
		return nil
	end

	return _s.errorParams[1]
end

--清理窗口数据
function WindowUtil.ClearErrorParams()
	_s.errorParams = nil
end

-- 更新是否有提示
function WindowUtil.PopErrorWindow(eventID, stateType)
	if GameStateMgr.GetCurStateType() == EGameStateType.LoadingState then
		return
	end
	
	if isNilOrEmpty(_s.errorParams) then
		return
	end

	--if not WrapSys.EZFunWindowMgr_CheckWindowOpen(EZFunWindowEnum.luaWindow, "common_error_ui_window") then
		WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "common_error_ui_window", true , nil)
	--end
end

local function chsize(ch)
	if not ch then return 0
	elseif ch >=252 then return 6
	elseif ch >= 248 and ch < 252 then return 5
	elseif ch >= 240 and ch < 248 then return 4
	elseif ch >= 224 and ch < 240 then return 3
	elseif ch >= 192 and ch < 224 then return 2
	elseif ch < 192 then return 1
	end
end

-- 计算utf8字符串字符数, 各种字符都按一个字符计算
-- 例如utf8len("1你好") => 3
function utf8len(str)
	local len = 0
	local aNum = 0 --字母个数
	local hNum = 0 --汉字个数
	local currentIndex = 1
	while currentIndex <= #str do
		local char = string.byte(str, currentIndex)
		local cs = chsize(char)
		currentIndex = currentIndex + cs
		len = len +1
		if cs == 1 then
			aNum = aNum + 1
		elseif cs >= 2 then
			hNum = hNum + 1
		end
	end
	return len, aNum, hNum
end


-- 截取utf8 字符串
-- str:            要截取的字符串
-- startChar:    开始字符下标,从1开始
-- numChars:    要截取的字符长度
function utf8sub(str, startChar, numChars)
	local startIndex = 1
	while startChar > 1 do
		local char = string.byte(str, startIndex)
		startIndex = startIndex + chsize(char)
		startChar = startChar - 1
	end

	local currentIndex = startIndex

	while numChars > 0 and currentIndex <= #str do
		local char = string.byte(str, currentIndex)
		local cs = chsize(char)
		currentIndex = currentIndex + cs
		if cs == 1 then
			numChars = numChars -1
		elseif cs >= 2 then
			numChars = numChars - 2
		end
	end
	return str:sub(startIndex, currentIndex - 1)
end

function WindowUtil.LoadHeadIcon(luaWindowRoot, trans, url, gender, isPixelPerfect,StorgeType)
	local default_image = (gender == 1 or gender == true) and "commonLayer_boy" or "commonLayer_girl"
	luaWindowRoot:LoadUISprite(trans,default_image)
	luaWindowRoot:LoadImag(trans, url, default_image, isPixelPerfect,StorgeType)
end

--str : 显示的字符串
--showType : 0 -- 正常模式   1 -- 点击模式
function WindowUtil.LuaShowTips(str, showType)
	if str == nil then return end
	_s.ShowTipsStr = str
	showType = showType or 0
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.err_tips_ui_window, false, 0, "", false , nil)
	WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, true, showType, "tips_ui_window")
end

-- print(utf8sub("中文1wwwwwwwwwwww",1,10))
