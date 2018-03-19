--------------------------------------------------------------------------------
--   File      : QuickVoiceInfo.lua
--   author    : jianing
--   function   : 快捷语音配置内容 配置QuickVoice.xls表
--   date      : 2017-03-06
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

local QuickVoiceInfo = {}
local TypeQuickVoiceList = {}	--按照类型配置存储

--获取单条配置
function QuickVoiceInfo.GetQuickInfo(playId,index)
	return QuickVoiceInfo.GetInfoList(playId)[index]
end

--查表 有可能有玩法ID转换
function QuickVoiceInfo.GetInfoList(playId)
	return QuickVoiceInfo.GetInfoListByType(playId)
end

--根据玩法ID查表
function QuickVoiceInfo.GetInfoListByType(playId)
	local QuickVoiceList = LuaTableSys.GetTableDic("ResQuickVoiceList")
	if not TypeQuickVoiceList[playId] then
		local tempyList = {}
		for key,value in pairs(QuickVoiceList) do
			if value.ID.Type == playId then
				table.insert(tempyList, value)
			end
		end
		table.sort(tempyList, function (a, b)
			return a.ID.Index < b.ID.Index
		end)
		TypeQuickVoiceList[playId] = tempyList
	end
	return TypeQuickVoiceList[playId]
end

return QuickVoiceInfo