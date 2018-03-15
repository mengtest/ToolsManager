--------------------------------------------------------------------------------
-- 	 File      : PlayRecordSys.lua
--   author    : guoliang
--   function   : 玩家战绩系统
--   date      : 2017-09-26
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

PlayRecordSys = {}
local  _s = PlayRecordSys

function PlayRecordSys.Init()
	_s.RegisterEvent()
	_s.playInterval = 3
	_s.recordList = {}
	_s.recordDetailList = {}
	_s.replyInfoList = {}
	_s.curReplayIndex = 0
	_s.curRecordDetail = 0
	_s.curPlayDetailID = -1
	-- 当前查看的战绩详情包体
	_s.curReqRecordInfo = nil
end

function PlayRecordSys.RegisterEvent()
	LuaEvent.AddHandle(EEventType.RecordPlayNextRound,_s.PlayNextRecordReplayInfo,nil)
	LuaEvent.AddHandle(EEventType.RecordPlayLastRound,_s.PlayLastRecordReplayInfo,nil)
end

function PlayRecordSys.UnRegisterEvent()
	LuaEvent.RemoveHandle(EEventType.RecordPlayNextRound,_s.PlayNextRecordReplayInfo,nil)
	LuaEvent.RemoveHandle(EEventType.RecordPlayLastRound,_s.PlayLastRecordReplayInfo,nil)
end


function PlayRecordSys.Destroy()
	_s.UnRegisterEvent()
	_s.recordList = nil
	_s.curReqRecordInfo = nil
end

-- 请求战绩列表
function PlayRecordSys.SendRecordListReq(pageIndex,succCB,failCB)
	WebNetHelper.GetPlayRecordList(pageIndex,20,function (rsp,head)
		_s.CacheRecordList(rsp,head)
		if succCB then
			succCB(rsp.data.list)
		end
	end,
	function (rsp,head)
--		WindowUtil.LuaShowTips("请求战绩列表失败")
		if failCB then
			failCB()
		end
	end, true)
end
function PlayRecordSys.SetCurReqRecord(recordInfo)
	_s.curReqRecordInfo = recordInfo
end
-- 请求战绩详情
function PlayRecordSys.SendRecordDetailReq(recordID, succCB, failCB)
	if _s.recordDetailList[recordID] == nil then
		WebNetHelper.GetRecordDetail(recordID,function (rsp,head)
			_s.CacheRecordDetail(rsp,head)
			if succCB then
				succCB()
			end
		end,
		function (rsp,head)
--			WindowUtil.LuaShowTips("请求战绩详情失败")
			if failCB then
				failCB()
			end
		end, true)
	elseif succCB then
		--记录当前请求的战绩详情id，用来跳转回放上一局下一局
		_s.curRecordDetail = recordID
		succCB()

	end
end

-- 请求单局回放信息
function PlayRecordSys.SendRecordDetailRoundReplayInfoReq(deatailID, succ_cb)
	_s.curPlayDetailID = deatailID
	if _s.replyInfoList[deatailID] == nil then
		WebNetHelper.GetRecordDetailRoundReplayInfo(deatailID,
			function (rsp,head)
				_s.CacheRecordReplyInfo(rsp,head)

				if _s.curReqRecordInfo then
					PlayGameSys.GoToRecordGame(_s.curReqRecordInfo.play_id)
					LuaEvent.AddEventNow(EEventType.RecordStartPlay,_s.replyInfoList[_s.curPlayDetailID])

					if succ_cb then
						succ_cb()
					end
				else
--			 		WindowUtil.LuaShowTips("当前战绩详情失效")
				end
			end,
			function (rsp,head)
				_s.curReplayIndex = 0
				_s.curPlayDetailID = -1
--				WindowUtil.LuaShowTips("请求单局回放信息失败")
		end)
	else
		if _s.curReqRecordInfo then
	 		PlayGameSys.GoToRecordGame(_s.curReqRecordInfo.play_id)
			 LuaEvent.AddEventNow(EEventType.RecordStartPlay,_s.replyInfoList[_s.curPlayDetailID])
			 
			 if succ_cb then
				succ_cb()
			 end
		else
--			WindowUtil.LuaShowTips("当前战绩详情失效")
		end
	end
end

function PlayRecordSys.GetRecordList(page)
	return _s.recordList[page]
end

function PlayRecordSys.GetRecordDetail(id)
	return _s.recordDetailList[id]
end

--缓存战绩列表(按页签保存)
function PlayRecordSys.CacheRecordList(rsp,head)
	if rsp then
		-- 按页保存战绩列表
		_s.recordList[rsp.data.list.page_index] = rsp.data.list
	end
end

--缓存战绩详情
function PlayRecordSys.CacheRecordDetail(rsp,head)
	DwDebug.Log("缓存战绩详情  id ",rsp.data.list[1].archive_id)
	if rsp then
		if rsp.data.list and #rsp.data.list > 0 then
			_s.recordDetailList[rsp.data.list[1].archive_id] = rsp.data.list
			--记录当前请求的战绩详情id，用来跳转回放上一局下一局
			_s.curRecordDetail = rsp.data.list[1].archive_id
		end
	end
end

--获取是否有下一局数据
function PlayRecordSys.CheckNextRecordReplayInfo()
	if _s.recordDetailList and
		_s.recordDetailList[_s.curRecordDetail] and
		#_s.recordDetailList[_s.curRecordDetail] > _s.curReplayIndex then
		return true
	else
		return false
	end
end

-- 是否第一局
function PlayRecordSys.CheckLastRecordReplayInfo()
	if _s.curReplayIndex and _s.curReplayIndex > 1 then
		return true
	else
		return false
	end
end

--缓存战绩单局回放信息
function PlayRecordSys.CacheRecordReplyInfo(rsp,head)
	if rsp then
		if rsp.data and rsp.data.id then
			DwDebug.Log("CacheRecordReplyInfo id =" ..rsp.data.id)
			_s.curPlayDetailID = rsp.data.id
			if _s.curReqRecordInfo then
				 ProtoManager.InitProtoByPlayID(_s.curReqRecordInfo.play_id)
				_s.replyInfoList[rsp.data.id] = ProtoManager.Decode(WebEvent.RecordGameSvrInfo,rsp.data.record)
			end
		end
	end
end

-- 通过序号播放单局回放信息
function PlayRecordSys.PlayRecordReplayInfoByIndex(index, succ_cb)
	if _s.recordDetailList and _s.recordDetailList[_s.curRecordDetail] ~= nil and #_s.recordDetailList[_s.curRecordDetail] >= index then
		local detailInfo = _s.recordDetailList[_s.curRecordDetail][index]
		if detailInfo then
			_s.curReplayIndex = index
			_s.SendRecordDetailRoundReplayInfoReq(detailInfo.id, succ_cb)
		end
	end
end

--播放下一局回放信息
function PlayRecordSys.PlayNextRecordReplayInfo(eventId,p1,p2)
	DwDebug.Log("PlayNextRecordReplayInfo")
	if _s.CheckNextRecordReplayInfo() then
		_s.PlayRecordReplayInfoByIndex(_s.curReplayIndex + 1)
	else
		WindowUtil.LuaShowTips("已经是最后一局了")
	end
end


--播放上一局回放信息
function PlayRecordSys.PlayLastRecordReplayInfo(eventId,p1,p2)
	if _s.CheckLastRecordReplayInfo() then
		_s.PlayRecordReplayInfoByIndex(_s.curReplayIndex - 1)
	else
		WindowUtil.LuaShowTips("已经是第一局了")
	end
end


