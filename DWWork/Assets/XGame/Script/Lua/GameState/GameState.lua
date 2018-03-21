
GameState = {}
local _s = GameState
_s.isEnterCompleted = false
_s.curPlayID = 0


function GameState.Init()

end

function GameState.Enter()
	-- logError("enter gamestate state ,time = " .. TimerSys.Instance:GetCurrentDateMTimes())
	WrapSys.AudioSys_PlayMusic("Music/backMusic_Room")
	
	local logicCtrl = PlayGameSys.GetPlayLogic()
	if logicCtrl then
		local logicCtrlType = logicCtrl.GetType()
		_s.curPlayID = PlayGameSys.GetNowPlayId()
		if logicCtrlType == PlayLogicTypeEnum.WSK_Normal then
			_s.InitPKUI()
			_s.InitPKSound(_s.curPlayID)

			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "play_ui_window", false , nil)
			--渲染完成
			logicCtrl:SendUIReady()
		elseif logicCtrlType == PlayLogicTypeEnum.WSK_Record then
			_s.InitPKUI()
			_s.InitPKSound(_s.curPlayID)
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "record_play_ui_window", false , nil)
			LuaEvent.AddEventNow(EEventType.RecordUIReady)
		elseif logicCtrlType == PlayLogicTypeEnum.MJ_Normal then
			_s.InitMJUI()
			_s.InitMJSound(_s.curPlayID)
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "mj_play_ui_window", false , nil)
			--渲染完成
			logicCtrl:SendUIReady()
		elseif logicCtrlType == PlayLogicTypeEnum.MJ_Record then
			_s.InitMJUI()
			_s.InitMJSound(_s.curPlayID)
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "mj_record_play_ui_window", false , nil)
			LuaEvent.AddEventNow(EEventType.RecordUIReady)
		elseif logicCtrlType == PlayLogicTypeEnum.ThirtyTwo_Normal then	--三十二张
			_s.InitPKUI()
			--_s.InitPKSound(_s.curPlayID)
			WrapSys.AudioSys_LoadBanks("ThirtyTwo")
			
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "pk32_play_ui_window", false , nil)
			--渲染完成
			logicCtrl:SendUIReady()
		elseif logicCtrlType == PlayLogicTypeEnum.ThirtyTwo_Record then	--三十二张回放
			_s.InitPKUI()
			--_s.InitPKSound(_s.curPlayID)
			WrapSys.AudioSys_LoadBanks("ThirtyTwo")

			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "pk32_record_play_ui_window", false , nil)
			LuaEvent.AddEventNow(EEventType.RecordUIReady)
		elseif logicCtrlType == PlayLogicTypeEnum.DDZ_Normal then    -- 斗地主
			WrapSys.AudioSys_LoadBanks("DouDiZhu")
			WrapSys.AudioSys_LoadBanks("50K")
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Area.DouDiZhu.UIWindow.play_ddz_ui_window", false , nil)
			--渲染完成
			logicCtrl:SendUIReady()
		elseif logicCtrlType == PlayLogicTypeEnum.DDZ_Normal_Record then    -- 斗地主回放
			WrapSys.AudioSys_LoadBanks("DouDiZhu")
			WrapSys.AudioSys_LoadBanks("50K")
			WrapSys.EZFunWindowMgr_SetWindowStatus(EZFunWindowEnum.luaWindow, true, 1, "Area.DouDiZhu.UIWindow.ddz_record_play_ui_window", false , nil)
			LuaEvent.AddEventNow(EEventType.RecordUIReady)
		end
	end
	_s.isEnterCompleted = true

	GameStateQueueTask.EnterEnd()
end

function GameState.InitPKSound(playID)
	WrapSys.AudioSys_LoadBanks("50K")
	if playID == Common_PlayID.chongRen_510K then
		WrapSys.AudioSys_LoadBanks("50K_ChongRen")
	end
end

function GameState.InitMJSound(playID)
	WrapSys.AudioSys_LoadBanks("MaJiang")

	if playID == Common_PlayID.chongRen_MJ then
		WrapSys.AudioSys_LoadBanks("MaJiang_ChongRen")
	end
end

function GameState.UnloadSound()
	if _s.curPlayID == Common_PlayID.chongRen_510K then
		WrapSys.AudioSys_UnLoadBank("50K")
		WrapSys.AudioSys_UnLoadBank("50K_ChongRen")
	elseif _s.curPlayID == Common_PlayID.leAn_510K then
		WrapSys.AudioSys_UnLoadBank("50K")
	elseif _s.curPlayID == Common_PlayID.yiHuang_510K then
		WrapSys.AudioSys_UnLoadBank("50K")
	elseif _s.curPlayID == Common_PlayID.chongRen_MJ then
		WrapSys.AudioSys_UnLoadBank("MaJiang")
		WrapSys.AudioSys_UnLoadBank("MaJiang_ChongRen")
	elseif _s.curPlayID == Common_PlayID.leAn_MJ then
		WrapSys.AudioSys_UnLoadBank("MaJiang")
	elseif _s.curPlayID == Common_PlayID.yiHuang_MJ then
		WrapSys.AudioSys_UnLoadBank("MaJiang")
	elseif _s.curPlayID == Common_PlayID.ThirtyTwo then
		WrapSys.AudioSys_UnLoadBank("ThirtyTwo")
	elseif _s.curPlayID == Common_PlayID.DW_DouDiZhu then
		WrapSys.AudioSys_UnLoadBank("50K")
		WrapSys.AudioSys_UnLoadBank("DouDiZhu")
	end
end

function GameState.InitPKUI()
	local resobj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "card_item", RessStorgeType.RST_Never, false)
	if resobj then
		resobj:SetActive(false)
	end
end

function GameState.InitMJUI()
	local resobj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "mjcard_out_show_item", RessStorgeType.RST_Never, false)
	if resobj then
		resobj:SetActive(false)
	end
	resobj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "mjcard_south_item", RessStorgeType.RST_Never, false)
	if resobj then
		resobj:SetActive(false)
	end
	resobj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "mjcard_north_item", RessStorgeType.RST_Never, false)
	if resobj then
		resobj:SetActive(false)
	end
	resobj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "mjcard_west_item", RessStorgeType.RST_Never, false)
	if resobj then
		resobj:SetActive(false)
	end
	resobj = WrapSys.ResourceMgr_GetInstantiateAsset(RessType.RT_UIItem, "mjcard_east_item", RessStorgeType.RST_Never, false)
	if resobj then
		resobj:SetActive(false)
	end
end



function GameState.Update()
	
end

function GameState.Leave()
	_s.isEnterCompleted = false
	_s.UnloadSound()
	_s.curPlayID = 0
end

function GameState.GetType()
	return EGameStateType.GameState
end