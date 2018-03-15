--------------------------------------------------------------------------------
--   File      : LuaUserNetWork.lua
--   author    : guoliang
--   function   : tcp 网络底层（用户通信服）
--   date      : 2017-09-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

LuaUserNetWork = {}
local _s = LuaUserNetWork
_s.delayTime = 0
--区域ID
_s.PlayID = 25000

--用户通讯服网络节点名
_s.userNetName = "UserLogicNet"


local m_handle_dic = {}
local m_sendcb_dic = {}

-- C# 奇数 lua 偶数
local m_seq = 9

function LuaUserNetWork.Register()
	--构造函数
	UpdateSecond:Add(_s.UpdateSecond)
end


local function _RegisterHandle(cmd, func, obj)
	if not cmd then
		DwDebug.Log("register nil cmd")
		return
	end
	-- body
	local handleDic = m_handle_dic
	if not handleDic[cmd] then
		handleDic[cmd] = Event(cmd, true)
	end
	handleDic[cmd]:Add(func, obj)
end

function LuaUserNetWork.RegisterHandle(cmd, func, obj)
	_RegisterHandle(cmd, func, obj)
end


local function _UnRegisterHandle(cmd, func, obj)
	local handleDic = m_handle_dic
	
	if not handleDic[cmd] then
		return 
	end
	handleDic[cmd]:Remove(func, obj)
end

function LuaUserNetWork.UnRegisterHandle(cmd, func, obj)
	_UnRegisterHandle(cmd, func, obj)
end


function LuaUserNetWork.HandleMsg(netName,tcmd, tseq, areaID,errno, isSuc, data)
	local successCodeNum = ProtoManager.GetSuccesErrorCodeNum(areaID)
	local head = {}
	head.seq = tseq
	head.cmd = tcmd
	head.errno = errno
	if head.errno ~= -1 then --  注意-1是客户端制造的错误编号
		head.errno = successCodeNum
	end

	-- body
	local body = nil
	if data then
		body = ProtoManager.Decode(tcmd,data)
		if WrapSys.IsEditor then
			DwDebug.Log(body)
		end
	else
		DwDebug.Log("data is empty !!! tcmd : " .. tcmd)
		data = "" --C#传byte[0]会为nil
		body = ProtoManager.Decode(tcmd,data)
		if WrapSys.IsEditor then
			DwDebug.Log(body)
		end
	end

	if tcmd ~= ProtoManager.GetHeatBeatCMD(areaID) and tcmd ~= ProtoManager.GetRspCMD(areaID) then
		DwDebug.LogWarning("LuaUserNetWork","LuaUserNetWork.HandleMsg: head = ", head, "\nbody = ", body or "nil")
	end

	local sendCb = m_sendcb_dic
	local handleCb = m_handle_dic
	local cb = sendCb[head.seq]

	DwDebug.Log("LuaUserNetWork.HandleMsg: tcmd = "..tcmd.."seq = "..tseq)
	-- 服务端所有的错误抛出都是通过 cmd = GAME_CMD.SC_COMMON_RSP,先统一处理
	if tcmd == ProtoManager.GetRspCMD(areaID) then
		head.errno = body.code

		if head.errno ~= successCodeNum and body.eventId ~= ProtoManager.GetHeatBeatCMD(areaID) then
			if not _s.CheckIsDisbandMessage(body.message) then
				WindowUtil.LuaShowTips(body.message)
			end
			DwDebug.Log("LuaUserNetWork","LuaUserNetWork.HandleMsg: cmd = "..tcmd..";seq = "..tseq..";playid=".._s.PlayID, ";body", body)
			--通用服务端错误码事件
			LuaEvent.AddEventNow(EEventType.Net_Common_Errno,areaID,head.errno)
		end
		if cb then
			if head.errno == successCodeNum then --成功
				if cb.succ_cb then
					cb.succ_cb(body, head)
				end
			else
				if cb.fail_cb then
					cb.fail_cb(body,head)
				end
			end
			sendCb[head.seq] = nil
		end
	else
		if cb then
			if head.errno == successCodeNum then --成功
				if cb.succ_cb then
					cb.succ_cb(body, head)
				end
			else
				if cb.fail_cb then
					cb.fail_cb(body,head)
				end
			end
			sendCb[head.seq] = nil
		end
	end

	if isSuc then
		local cmd = head.cmd
		local eventList = handleCb[cmd]
		if eventList then
			DwDebug.Log("handleCb  ".. cmd )
			eventList(body,head)
		end
	end
end

function LuaUserNetWork.CheckIsDisbandMessage(tipMsg)
	if tipMsg == "用户未登陆" or tipMsg == "用户未登录" then
		return true
	end
	
	return false
end

local function _SendMsg(cmd, mbody, succesFunc, failedFunc,hasCb)	
	local needLockScreen = true
	local seq = 0
	m_seq = m_seq + 2
	seq = m_seq
	
	if cmd == ProtoManager.GetHeatBeatCMD(_s.PlayID) or not hasCb then -- 心跳包和不需要回包的消息不需要锁屏
		needLockScreen = false
	else

	end
	
	if cmd ~= ProtoManager.GetHeatBeatCMD(_s.PlayID) and cmd ~= ProtoManager.GetRspCMD(_s.PlayID) then
		DwDebug.LogWarning("LuaUserNetWork","LuaUserNetWork.SendMsg: cmd = "..cmd.."seq = "..seq..";playid=".._s.PlayID, ";body", mbody)
	end
	
	local sendCb = m_sendcb_dic

	local data = nil
	if mbody then
--		if cmd ~= ProtoManager.GetHeatBeatCMD(_s.PlayID) then
--			print("send msg body "..ToString(mbody))
--		end
		data = ProtoManager.Encode(cmd, mbody)

		--print("send decode msg body "..ToString(ProtoManager.Decode(cmd,data)))
	end
	if hasCb then
		local cb = {}
		cb.succ_cb = succesFunc
		cb.fail_cb = failedFunc
		sendCb[seq] = cb
	end
	local sendFunc = WrapSys.CNetSys_SendLuaNet
	if data then
		sendFunc(_s.userNetName,_s.PlayID,cmd, seq, hasCb, needLockScreen,data)--区域ID临时这样用，后面再优化
	else
		sendFunc(_s.userNetName,_s.PlayID,cmd, seq, hasCb,needLockScreen)
	end
end

---
---如果 succesFunc 和 failedFunc都为nil 那么就表示这个包可抛弃
---
---注意 回报的协议号一定要先调用WrapSys.AddLuaCMD(cmd) 要不回报body 为空
local m_logic_wait_call = false
function LuaUserNetWork.SendMsg(cmd, mbody, succesFunc, failedFunc, isForce)
	DwDebug.Log("LuaUserNetWork.SendMsg: cmd = "..cmd .. tostring(m_logic_wait_call) .. tostring(isForce))	
	if m_logic_wait_call and not isForce then
		return
	end
	if not  isForce then
		m_logic_wait_call = true
	end

	local succ_cb
	local fail_cb
	local hasCb = false
	if succesFunc or failedFunc then
		hasCb = true
		succ_cb = function(body, head) 
			m_logic_wait_call = false
			if succesFunc then
				succesFunc(body,head)
			end
		end

		fail_cb = function(body, head) 
			m_logic_wait_call = false
			if failedFunc then
				failedFunc(body,head)
			end
		end
	else
		_s.delayTime = 1
	end

	_SendMsg(cmd, mbody, succ_cb, fail_cb,hasCb)
end
--- 重置清空标记
function  LuaUserNetWork.Reset()
	m_logic_wait_call = false
end

function LuaUserNetWork.UpdateSecond()
	if _s.delayTime > 0 then
		_s.delayTime = _s.delayTime - 1
		if _s.delayTime <= 0 then
			m_logic_wait_call = false
		end
	end
end


SendUserNetMsg = LuaUserNetWork.SendMsg
