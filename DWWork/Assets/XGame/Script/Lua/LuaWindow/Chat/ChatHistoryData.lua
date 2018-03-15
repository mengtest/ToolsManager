
ChatHistoryData = {}
local _s = ChatHistoryData

function ChatHistoryData.init()
	_s.historyData = {}
end

function ChatHistoryData.Clear()
	_s.historyData = {}
end

function ChatHistoryData.AddData(datatable)
	datatable.isFirst = true
	--用户信息存在里面 防止玩家离开房间 读不到
	local playerinfo = PlayGameSys.GetPlayerByPlayerID(tonumber(datatable.senderID))
	if playerinfo then
		datatable.playerinfo = playerinfo
		table.insert(_s.historyData ,datatable)
	end

	--目前存20条吧
	if #_s.historyData > 20 then
		table.remove(_s.historyData,1)
	end
end

function ChatHistoryData.GetAllData()
	return _s.historyData
end

function ChatHistoryData.GetHistoryCount( )
	return #_s.historyData
end

function ChatHistoryData.GetHistoryByIndex(index)
	return _s.historyData[tonumber(index)]
end

--寻找语音消息的index
function ChatHistoryData.GetIndexByData( data )
	for i=1,#_s.historyData do
		if _s.historyData[i].msgVoicePath == data.msgVoicePath then
			return i
		end
	end

	return 0
end