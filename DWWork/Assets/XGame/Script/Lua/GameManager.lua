--------------------------------------------------------------------------------
-- 	 File      : GameManager.lua
--   author    : shandong   shandong@ezfun.cn
--   version   : 1.0
--   date      : 2016-06-28 10:05:38.
--   copyright : Copyright 2016 EZFun Inc.
--------------------------------------------------------------------------------
-- 这个类是静态类,里面所有东西都静态的
require "define"
require "Global.List"
require "Global.Event"
require "Main"
require "functions"
require "Global.System_class"
require "LuaSys.ezfunLuaTool"
require "LuaSys.baseObject"
require "LuaSys.LuaCsharpFuncSys"
require "Global.LuaUtil"
require "NetWork.WebEvent"
require "LuaSys.DataManager"
require "LuaSys.WindowUtil"
require "LuaSys.CommonEnum"
require "NetWork.WebNetHelper"
--Debug = require "utils"

GameManager = {}
local self = GameManager
local ctrlMap = {}
local ctrlKeyList = {}
local Register_Num = 0

TablePrintUtil = require"utils"
DwDebug = require "DwDebug"

function GameManager.Register()
	GameManager.AddLuaGameSys("LuaSys.DataManager")
	GameManager.AddLuaGameSys("NetWork.ProtoManager")
	GameManager.AddLuaGameSys("LuaSys.LuaEvent")
	GameManager.AddLuaGameSys("NetWork.LuaNetWork")
	GameManager.AddLuaGameSys("NetWork.LuaHttpNetWork")
	GameManager.AddLuaGameSys("NetWork.LuaUserNetWork")
	GameManager.AddLuaGameSys("LuaSys.LuaTableSys")
	GameManager.AddLuaGameSys("LuaSys.MemSys")
	GameManager.AddLuaGameSys("LuaSys.TimerTaskSys")
	GameManager.AddLuaGameSys("LuaSys.DelayTaskSys")
	GameManager.AddLuaGameSys("LuaSys.LoginSys")
	GameManager.AddLuaGameSys("LuaSys.HallSys")
	GameManager.AddLuaGameSys("GameState.GameStateMgr")
	GameManager.AddLuaGameSys("LuaSys.PlayGameSys")
	GameManager.AddLuaGameSys("LuaSys.PlayRecordSys")
	GameManager.AddLuaGameSys("LuaSys.NimChatSys")
	GameManager.AddLuaGameSys("LuaSys.AudioManager")
	GameManager.AddLuaGameSys("LuaSys.CIAPSys")
	GameManager.AddLuaGameSys("LuaSys.UserNetSys")
	GameManager.AddLuaGameSys("Club.ClubSys.ClubSys")
end


function GameManager.Init()
	GameManager.Register()
	DwDebug.Log("--------------------GameManager Init-------------------")
	for k,v in pairs(ctrlMap) do
		if v.Init then
			v.Init()
		end
	end
end

function GameManager.AddLuaGameSys(ctrlName)
	local ctrlPath = ctrlName
	-- 字符串中间包含了.  表示路径中已经是绝对路径了
	if not string.find(ctrlPath, "%.", 0) then
		ctrlPath = "LuaSys." .. ctrlPath
	else
		ctrlName = string.findLast(ctrlPath, "%.")
	end

	require (ctrlPath)
	--从全变量中获取对应的系统的表
	Register_Num = Register_Num + 1
	ctrlKeyList[ctrlName] = Register_Num
	ctrlMap[Register_Num] = _G[ctrlName]
	if ctrlMap[Register_Num].Register then
		ctrlMap[Register_Num].Register()
	end
end


function GameManager.GetLuaGameSys(ctrlName)
	if ctrlKeyList[ctrlName] then
		return ctrlMap[ctrlKeyList[ctrlName]]
	end
end

--这个接口在玩家退出到登陆界面时候会调用 需要重置数据
function GameManager.Reset()
	for ctrlIndex,ctrlObj in pairs(ctrlMap) do
		if ctrlObj.Reset then
			ctrlObj:Reset()
		end
	end
end
--帧更新后面再考虑一下是LuaScriptMgr直接进这儿还是先进Main.lua
function GameManager.Update(deltatime, unscaledDeltaTime)
	Update(deltatime, unscaledDeltaTime)
	for ctrlIndex,ctrlObj in pairs(ctrlMap) do
		if ctrlObj.Update then
			ctrlObj.Update()
		end
	end
end

function GameManager.LateUpdate()
	LateUpdateBeat()
	CoUpdateBeat()
	for ctrlIndex,ctrlObj in pairs(ctrlMap) do
		if ctrlObj.LateUpdate then
			ctrlObj.LateUpdate()
		end
	end
end

function GameManager.FixedUpdate(fixedTime)
	FixedUpdateBeat()
end

function GameManager.Release()
	for ctrlIndex,ctrlObj in pairs(ctrlMap) do
		if ctrlObj.Release then
			ctrlObj.Release()
		end
	end
end

function GameManager.SetGameTimeScale(timeScale)
	UnityEngine.Time.timeScale = timeScale or 1
end



