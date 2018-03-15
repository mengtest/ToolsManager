--------------------------------------------------------------------------------
--   File      : LuaHttpNetWork.lua
--   author    : guoliang
--   function   : http 网络底层
--   date      : 2017-09-20
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
LuaHttpNetWork ={}
local _s = LuaHttpNetWork
_s.delayTime = 0
local m_handle_dic = {}
local m_sendcb_dic = {}
local m_wait_dic = {}

_s.loginCount = 0

_s.m_seq = 1

function LuaHttpNetWork.Register()

end

function LuaHttpNetWork.HandleMsg(msgKey,tcmd, terrno,seq,data)
	local numKey = tonumber(msgKey)	--c#传递的是string
	DwDebug.Log("LuaHttpNetWork.HandleMsg: msgKey = "..numKey)
	local head = {}
	head.msgKey = numKey
	head.cmd = tcmd
	head.errno = terrno

	local sendCb = m_sendcb_dic
	-- body
	local body = nil
	local isSuc = false	
	if terrno == -1 then
		isSuc = false
		WindowUtil.LuaShowTips("请求数据失败，请检查网络设置",1)
	else
		if data == nil then
			DwDebug.Log("data is nil")
			data = ""
		end
		body = ProtoManager.Decode(numKey,data)
		if body then 
			if WrapSys.IsEditor then
				DwDebug.Log(body)
				-- DwDebug.LogWarning("xxx", "LuaHttpNetWork.HandleMsg: msgKey",numKey,"seq",seq, "body", body)
			end
		else
			DwDebug.Log("body Decode fail msgKey = "..numKey)
		end

		if body and body.errcode then
			head.errno = body.errcode
			if head.errno == 0 then
				isSuc = true
				WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, false, 0, "tips_ui_window")
			else
				_s.HandleCommonFail(body,head)
				if body.errmsg then
					if head.errno ~= 40200 then 
						WindowUtil.LuaShowTips(body.errmsg,1)
					end
				end
			end
		end
	end
	
	local cb = sendCb[numKey]
	if seq > 0 then
		cb = sendCb[seq]
	else 
		cb = sendCb[numKey]
	end
	if cb then
--		print("http callback now")
		if isSuc then
			if cb.succ_cb then
				cb.succ_cb(body, head)
			end
		elseif cb.fail_cb then
			cb.fail_cb(body, head)
		end
		if seq > 0 then
			sendCb[seq] = nil
		else
			sendCb[numKey] = nil
		end
	end

	--释放登录计数
	_s.ReleaseLoginCount(numKey)
end

function LuaHttpNetWork._OnFailedFunc()
	LoginSys.LoginOutFunc()
end

function LuaHttpNetWork.HandleCommonFail(body,head)
    DwDebug.Log("LoginScene._OnWebLoginCallback  Login  Failed " ..head.errno)
	if head then
		if head.errno == 40000 or head.errno == 40100 then
			_s._OnFailedFunc()
		elseif head.errno == 40200 then
			WindowUtil.ShowErrorWindow(3, body.errmsg)
		elseif head.errno == 40300 then
			WindowUtil.LuaShowTips("已有房间，尝试重连中...")
		elseif head.errno == -1 then
			WindowUtil.LuaShowTips("登录失败，请检查网络设置")
		else
			WindowUtil.LuaShowTips(body.errmsg)
		end
	end
end
--有网状态下如果登录失败3次重启工作线程
function LuaHttpNetWork.CheckLoginCount(msgKey)
	if msgKey == -1 then
		if WrapSys.CNetSys_CheckNetWorkIsReachable() then
			_s.loginCount = _s.loginCount + 1
			if _s.loginCount > 3 then
				DwDebug.LogError("CNetSys_RestartHttpWork ".. _s.loginCount)
				_s.loginCount = 0
				WrapSys.CNetSys_RestartHttpWork()
			end
		end
	end
end

function LuaHttpNetWork.ReleaseLoginCount(msgKey)
	if msgKey == -1 and _s.loginCount > 0 then
		DwDebug.Log("ReleaseLoginCount ".. _s.loginCount)
		_s.loginCount = _s.loginCount - 1
	end
end


local function _SendMsg(msgKey,url, param,isPost ,succesFunc, failedFunc,hasCb,timeOut, use_seq)	
		--登录计数
	_s.CheckLoginCount(msgKey)
	_s.m_seq = _s.m_seq + 1
	if _s.m_seq > 10000 then
		_s.m_seq = 1
	end

	local sendCb = m_sendcb_dic

	if hasCb then
		local cb = {}
		cb.succ_cb = succesFunc
		cb.fail_cb = failedFunc
		if use_seq then
			sendCb[_s.m_seq] = cb
		else 
			sendCb[msgKey] = cb
		end
	end
	local timeOutVal = timeOut or 8
	-- DwDebug.LogWarning("xxx", "LuaHttpNetWork.SendMsg: msgKey",msgKey,"seq", use_seq and _s.m_seq or 0, "url", url)
	WrapSys.CNetSys_SendHttpLuaNet(tostring(msgKey),url,param,isPost, use_seq and _s.m_seq or 0,timeOutVal)--预留seq
end

local m_logic_wait_call = false
function LuaHttpNetWork.SendMsg(msgKey,url,param, isPost,succesFunc, failedFunc,isForce,timeOut, use_seq)
	DwDebug.Log(msgKey..url)
	if m_logic_wait_call and not isForce then
		DwDebug.Log("LuaHttpNetWork m_logic_wait_call = true")
		return
	end

	if not isForce then
		m_logic_wait_call = true
	end

	DwDebug.Log("LuaHttpNetWork","LuaHttpNetWork.SendMsg: ", msgKey, url, param)

	local succ_cb
	local fail_cb
	local hasCb = true

	succ_cb = function(body,head) 
		m_logic_wait_call = false
		if succesFunc then
			succesFunc(body,head)
		end
	end

	fail_cb = function(body,head) 
		m_logic_wait_call = false
		if failedFunc then
			failedFunc(body,head)
		end
	end


	_SendMsg(msgKey,url,param, isPost,succ_cb, fail_cb,hasCb,timeOut, use_seq)
end
--- 重置清空标记
function  LuaHttpNetWork.Reset()
	m_logic_wait_call = false
end


HttpRequest = LuaHttpNetWork.SendMsg