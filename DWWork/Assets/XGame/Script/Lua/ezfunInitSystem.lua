function initSingleSystem(systemName)
	WrapSys.WindowBaseLua_LoadLua(systemName)
	local gameObject=UnityEngine.GameObject.New()
	gameObject.transform.parent = UnityEngine.GameObject.Find("systemRoot").transform
	gameObject.name = systemName
	WrapSys.EZFunTools_AddBaseLua(gameObject)
end
function InitAllSystem(luaWindowRoot)
	-- flower system
	initSingleSystem("sendflowers_sys")
	sendflowers_sys.SetLuaWindowRoot(luaWindowRoot)
	sendflowers_sys.InitPara()
end
