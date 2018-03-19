--------------------------------------------------------------------------------
-- 	 File      : AudioManager.lua
--   author    : jianing
--   function  : 声音系统
--   date      : 2017-10-28
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
require "LuaSys.AudioEnum"

AudioManager = {}
local _s = AudioManager

function AudioManager.Init()
	-- WrapSys.AudioSys_LoadBanks("50K")
	-- WrapSys.AudioSys_LoadBanks("MaJiang")
	-- WrapSys.AudioSys_LoadBanks("50K_ChongRen")
	-- WrapSys.AudioSys_LoadBanks("MaJiang_ChongRen")
	LuaEvent.AddHandle(EEventType.OpenOrCloseWindow,_s.PlayWindowSound,nil)
end

--播放窗口开关声音
function AudioManager.PlayWindowSound(eventID,open,windowName)
	if open then
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangOpen")
	else
		WrapSys.AudioSys_PlayEffect("Common/UI/tankuangClose")
	end
end

--根据声音配置 播放声音
function AudioManager.PlayBySoundCfg(ResSoundcfg)
	if ResSoundcfg and ResSoundcfg.soundEvent and #ResSoundcfg.soundEvent > 0 then
		local randomIndex = math.random(#ResSoundcfg.soundEvent)
		return WrapSys.AudioSys_PlayEffect(ResSoundcfg.soundEvent[randomIndex].name)
	end
end

--获取声音配置
function AudioManager.GetResSoundCfg(index,playType,sex)
	sex = AudioManager.CheckSex(sex)

	local language = _s.GetNowLanguage(index,playType,sex)

	local ResSoundcfg = LuaTableSys.GetEntry("ResSoundList", {Index= index, Type = playType,Sex = sex,Language = language})
	return ResSoundcfg
end

--获取当前语言(0 普通话 方言根据玩法区分)
function AudioManager.GetNowLanguage(index,playType,sex)
	if playType == 0 or sex == 0 then	 --通用资源
		return 0
	end

	local isFangYan = DataManager.CheckNowIsFangYan()
	if isFangYan then
		return playType
	else
		return 0
	end
end

--获取声音配置时长
function AudioManager.GetResSoundTime(index,playType,sex)
	local ResSoundcfg = _s.GetResSoundCfg(index,playType,sex)
	if ResSoundcfg and ResSoundcfg.soundEvent and #ResSoundcfg.soundEvent > 0 then
		return ResSoundcfg.soundEvent[1].time/1000
	end
	return 2
end

--播放声音
function AudioManager.PlaySound(index,playType,sex)
	if index == nil or playType == nil or sex == nil then
		error("AudioManager.PlaySound is null, index :" .. tostring(index == null) .. " ,playType " .. tostring(playType == null) .. ", sex :".. tostring(sex == null))
		return
	end

	return _s.PlayBySoundCfg(_s.GetResSoundCfg(index,playType,sex))
end

--播放通用声音
function AudioManager.PlayCommonSound(index)
	_s.PlaySound(index,0,0,0)
end

--其他玩家表情
function AudioManager.GetOterEmojiEnm(emojiID)
	if emojiID == 1 then
		return 4
	elseif emojiID == 2 then
		return 1
	elseif emojiID == 3 then
		return 5
	elseif emojiID == 4 then
		return 2
	elseif emojiID == 5 then
		return 3
	end
	return 1
end

--播放互动表情
function AudioManager.PlayEmojiAudio(isMy,emojiID)
	if isMy then	--自己目前没声音
	else
		_s.PlayCommonSound(_s.GetOterEmojiEnm(emojiID))
	end
end
-------------------------------------------------------------------50K-------------------------------------------------------------
--操作声音 传入WSK_CaoZuoEnum
function AudioManager.WSK_PlayCaoZuo(sex,caoZuoEnum)
	if caoZuoEnum == nil then
		return
	end
	_s.PlaySound(caoZuoEnum,Common_PlayID.chongRen_510K,sex)
end

function AudioManager.WSK_PlayBaoPai_2(cards, cardType, userId)
	local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(userId)
	local sex = player.seatInfo.sex
	
	local index = 0
	if cardType == CardTypeEnum.CT_SINGLE then
		-- 单牌类型
		index = 100 + cards[1].logicValue
	elseif cardType == CardTypeEnum.CT_DOUBLE then
		-- 对牌类型
		index = 200 + cards[1].logicValue
	elseif cardType == CardTypeEnum.CT_THREE then
		-- 3张
		index = 300 + cards[1].logicValue
	elseif cardType == CardTypeEnum.CT_THREE_LINE_TAKE_ONE then
		-- 三带一单
		index = 401
	elseif cardType == CardTypeEnum.CT_THREE_LINE_TAKE_TWO then
		-- 三带一对
		index = 501
	elseif cardType == CardTypeEnum.CT_SINGLE_LINE then
		-- 顺子
		index = 601
		_s.WSK_PlayEffect(WSK_EffectEnum.shunZi)
	elseif cardType == CardTypeEnum.CT_DOUBLE_LINE then	
		-- 连对
		index = 701 --连对
		_s.WSK_PlayEffect(WSK_EffectEnum.lianDui)
	elseif cardType == CardTypeEnum.CT_THREE_LINE then
		-- 飞机
		index = 801 --飞机
		_s.WSK_PlayEffect(WSK_EffectEnum.feiJin)
	elseif cardType == CardTypeEnum.CT_FIVE_TEN_K or cardType == CardTypeEnum.CT_FIVE_TEN_K_BIG then
		-- 510K
		index = 901 --510K
	elseif cardType == CardTypeEnum.CT_BOMB_FOUR or cardType == CardTypeEnum.CT_BOMB_FIVE or
		   cardType == CardTypeEnum.CT_BOMB_SIX or cardType == CardTypeEnum.CT_BOMB_SEVEN or
		   cardType == CardTypeEnum.CT_BOMB_EIGTH then
		-- 炸弹
		index = 1001 --炸弹
		_s.WSK_PlayEffect(WSK_EffectEnum.bomb)
	elseif cardType == CardTypeEnum.CT_DOUBLE_SAME_KING then
		-- 纯色双王
		if cards[1].ID == 83 then
			index = 216
		else
			index = 217
		end
	elseif cardType == CardTypeEnum.CT_THREE_KING then
		-- 三王
		index = 2001 --3王
		_s.WSK_PlayEffect(WSK_EffectEnum.wangZha)
	elseif cardType == CardTypeEnum.CT_FOUR_KING then
		-- 四王
		index = 2101 --4王
		_s.WSK_PlayEffect(WSK_EffectEnum.wangZha)
	elseif cardType == CardTypeEnum.CT_THREE_FIVE_TEM_K or
		   cardType == CardTypeEnum.CT_FOUR_FIVE_TEM_K then	
		-- 多副510K
		index = 1001 --炸弹
		_s.WSK_PlayEffect(WSK_EffectEnum.bomb)

	elseif cardType == CardTypeEnum.CT_DOUBLE_SAME_HongXin_5 then
		-- 红心5
		index = 2701 --炸弹
	end

	_s.PlaySound(index,Common_PlayID.chongRen_510K,sex)
end

--出牌声音 根据服务器回包判断
function AudioManager.WSK_PlayBaoPai(rsp)
	local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
	local sex = player.seatInfo.sex
	
	local paiClass = rsp.paiClass	--牌分类
	local paiMask = rsp.paiMask --牌掩码

	local index = 0
	if paiClass == 1 or paiClass == 2 or paiClass == 3 then
		if paiMask >=0 and paiMask <= 14 then --3到大王
			index = 100 + (paiMask + 3)
		elseif paiMask >=15 and paiMask <= 29 then
			index = 200 + (paiMask - 12) --对子 3到大王
		elseif paiMask >=30 and paiMask <= 42 then
			index = 300 + (paiMask - 27) --3个
		end
	elseif paiClass == 4 then
		index = 401 --3带1
	elseif paiClass == 5 then
		index = 501 --3带2
	elseif paiClass == 6 then
		index = 601 --顺子
		_s.WSK_PlayEffect(WSK_EffectEnum.shunZi)
	elseif paiClass == 7 then
		index = 701 --连对
		_s.WSK_PlayEffect(WSK_EffectEnum.lianDui)
	elseif paiClass == 8 then
		index = 801 --飞机
		_s.WSK_PlayEffect(WSK_EffectEnum.feiJin)
	elseif paiClass == 9 then
		index = 901 --510K
	elseif paiClass >= 10 and paiClass < 15 then
		index = 1001 --炸弹
		_s.WSK_PlayEffect(WSK_EffectEnum.bomb)
	elseif paiClass == 15 then -- 纯色双王
		if rsp.pai and rsp.pai[1] == 83 then
			index = 216
		else
			index = 217
		end
	elseif paiClass >= 16 and paiClass <= 19 then
		index = 1001 --炸弹
		_s.WSK_PlayEffect(WSK_EffectEnum.bomb)
	elseif paiClass == 20 then
		index = 2001 --3王
		_s.WSK_PlayEffect(WSK_EffectEnum.wangZha)
	elseif paiClass == 21 then
		index = 2101 --4王
		_s.WSK_PlayEffect(WSK_EffectEnum.wangZha)
	elseif paiClass >= 22 and paiClass <=25 then
		index = 1001 --炸弹
		_s.WSK_PlayEffect(WSK_EffectEnum.bomb)
		
	elseif paiClass >= 27 and paiClass <=29 then	--红心5 暂时这样处理
		index = 2701 --炸弹
	end
	_s.PlaySound(index,Common_PlayID.chongRen_510K,sex)
end

-- 通用效果音
function AudioManager.WSK_PlayEffect(id,sex)
	local sexType = sex
	if sexType == nil then
		sexType = 0
	end
	_s.PlaySound(id,Common_PlayID.chongRen_510K,sexType)
end

----------------------------------------------------MJ------------------------------------------------------------------------
--操作声音 传入MJ_CaoZuoEnum
function AudioManager.MJ_PlayCaoZuo(sex,caoZuoEnum)
	if caoZuoEnum == nil then
		return
	end
	_s.PlaySound(caoZuoEnum,Common_PlayID.chongRen_MJ,sex)
end

-- 麻将报牌声音
function AudioManager.MJ_PlayBaoPai(sex,cardID)
	_s.PlaySound(cardID + 100,Common_PlayID.chongRen_MJ,sex)
end
-----------------------------------------------------32张----------------------------------------------------------------
--报牌
function AudioManager.ThirtyTwo_PlayBaoPai(sex,paiClass)
	sex = AudioManager.CheckSex( sex )
	local soundPath = ""
	if sex == 1 then
		soundPath = "ThirtyTwo/Nan/BaoPai/" .. paiClass
	else
		soundPath = "ThirtyTwo/Nv/BaoPai/" .. paiClass
	end
	WrapSys.AudioSys_PlayEffect(soundPath)
end

--报点
function AudioManager.ThirtyTwo_PlayBaoDian(sex,dian)
		sex = AudioManager.CheckSex( sex )
	local soundPath = ""
	if sex == 1 then
		soundPath = "ThirtyTwo/Nan/BaoDian/" .. dian
	else
		soundPath = "ThirtyTwo/Nv/BaoDian/" .. dian
	end
	WrapSys.AudioSys_PlayEffect(soundPath)
end

--报分
function AudioManager.ThirtyTwo_PlayBaoFen(sex,fen)
		sex = AudioManager.CheckSex( sex )
	local soundPath = ""
	if sex == 1 then
		soundPath = "ThirtyTwo/Nan/BaoFen/" .. fen
	else
		soundPath = "ThirtyTwo/Nv/BaoFen/" .. fen
	end
	WrapSys.AudioSys_PlayEffect(soundPath)
end

---------------------------------------------------斗地主---------------------------------------------------------------------
-- 抢地主报分
function AudioManager.DDZ_PlayBaoFen(sex, score)
	sex = AudioManager.CheckSex(sex)
	local path = ""
	if sex == 1 then
		path = "DouDiZhu/Nan/WantLord/"
	else
		path = "DouDiZhu/Nv/WantLord/"
	end
	
	local index = ""
	if score == 0 then
		local random = math.random(1, 2)
		index = tostring(score) .. random
	elseif score == 1 or score == 2 then
		index = tostring(score) .. 0
	elseif score == 3 then
		local random = math.random(1, 3)
		index = tostring(score) .. random
	else
	end

	if index ~= "" then
		WrapSys.AudioSys_PlayEffect(path .. index)
	end
end

-- 地主明牌声音
function AudioManager.DDZ_PlayOpenCard(sex, isOpen)
	sex = AudioManager.CheckSex(sex)
	local path = ""
	if sex == 1 then
		path = "DouDiZhu/Nan/Open/"
	else
		path = "DouDiZhu/Nv/Open/"
	end

	local index = ""
	if isOpen == true then
		index = 1
	elseif isOpen == false then
		index = 0
	else
	end

	if index ~= "" then
		WrapSys.AudioSys_PlayEffect(path .. index)
	end
end

-- 斗地主加倍声音
function AudioManager.DDZ_PlayDouble(sex, isDouble)
	sex = AudioManager.CheckSex(sex)
	local path = ""
	if sex == 1 then
		path = "DouDiZhu/Nan/Double/"
	else
		path = "DouDiZhu/Nv/Double/"
	end

	local index = ""
	if isDouble == true then
		index = 1
	elseif isDouble == false then
		index = 0
	else
	end

	if index ~= "" then
		WrapSys.AudioSys_PlayEffect(path .. index)
	end
end

-- 斗地主报牌
function AudioManager.DDZ_PlayBaoPai(rsp)
	local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(rsp.userId)
	local sex = player.seatInfo.sex

	sex = AudioManager.CheckSex(sex)
	local path = ""
	if sex == 1 then
		path = "DouDiZhu/Nan/BaoPai/"
	else
		path = "DouDiZhu/Nv/BaoPai/"
	end

	local index = ""
	if rsp.isSkip then
		-- 过
		local random = math.random(1, 2)
		index = "0" .. random
	else
		-- 有出牌
		local paiClass = rsp.paiClass
		local paiMask = rsp.paiMask --牌掩码
		if paiClass == 1 then
			-- 单张
			index = tostring(100 + paiMask + 3)
		elseif paiClass == 2 then
			-- 对子
			index = tostring(200 + paiMask - 12)
		elseif paiClass == 3 then
			-- 三张
			index = "300"
		elseif paiClass == 4 then
			-- 三带一
			index = "301"
		elseif paiClass == 6 then
			-- 顺子
			index = "600"
			_s.WSK_PlayEffect(WSK_EffectEnum.shunZi)
		elseif paiClass == 7 then
			-- 连对
			index = "700"
			_s.WSK_PlayEffect(WSK_EffectEnum.lianDui)
		elseif paiClass == 10 then
			-- 4炸
			index = "1500"
			_s.WSK_PlayEffect(WSK_EffectEnum.bomb)
		elseif paiClass == 30 then
			-- 三带一对
			index = "302"
		elseif paiClass == 31 then
			-- 飞机
			index = "800"
			_s.WSK_PlayEffect(WSK_EffectEnum.feiJin)
		elseif paiClass == 32 then
			-- 飞机带单
			index = "801"
			_s.WSK_PlayEffect(WSK_EffectEnum.feiJin)
		elseif paiClass == 33 then
			-- 飞机带对
			index = "801"
			_s.WSK_PlayEffect(WSK_EffectEnum.feiJin)
		elseif paiClass == 34 then
			--火箭
			index = "1600"
			AudioManager.PlayCommonSound(UIAudioEnum.Rocket)
		elseif paiClass == 36 then
			-- 四带二
			index = "1101"
		elseif paiClass == 38 then
			-- 四带两对
			index = "1102"
		else
		end
	end

	if index ~= "" then
		WrapSys.AudioSys_PlayEffect(path .. index)
	end
end

--斗地主报牌by NewCardTypeEnum
function AudioManager.DDZ_PlayBaoPaiByCardType(cards, cardType, userId)
	local player = PlayGameSys.GetPlayLogic().roomObj.playerMgr:GetPlayerByPlayerID(userId)
	local sex = player.seatInfo.sex

	sex = AudioManager.CheckSex(sex)
	local path = ""
	if sex == 1 then
		path = "DouDiZhu/Nan/BaoPai/"
	else
		path = "DouDiZhu/Nv/BaoPai/"
	end

	local index = ""
	if cards == nil or cardType == nil then
		-- 过
		local random = math.random(1, 2)
		index = "0" .. random
	else
		if cardType == NewCardTypeEnum.NCCT_SINGLE then
			-- 单牌
			index = 100 + cards[1].logicValue
		elseif cardType == NewCardTypeEnum.NCCT_DOUBLE then
			-- 对子
			index = 200 + cards[1].logicValue
		elseif cardType == NewCardTypeEnum.NCCT_THREE then
			-- 三张
			index = "300"
		elseif cardType == NewCardTypeEnum.NCCT_THREE_ADD_ONE then
			-- 三张带一
			index = "301"
		elseif cardType == NewCardTypeEnum.NCCT_THREE_ADD_DOUBLE then
			-- 三张带一对
			index = "302"
		elseif cardType == NewCardTypeEnum.NCCT_SINGLE_LINE then
			-- 顺子
			index = "600"
			_s.WSK_PlayEffect(WSK_EffectEnum.shunZi)
		elseif cardType == NewCardTypeEnum.NCCT_DOUBLE_LINE then
			-- 连对
			index = "700"
			_s.WSK_PlayEffect(WSK_EffectEnum.lianDui)
		elseif cardType == NewCardTypeEnum.NCCT_THREE_LINE then
			-- 飞机
			index = "800"
			_s.WSK_PlayEffect(WSK_EffectEnum.feiJin)
		elseif cardType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_ONE then
			-- 飞机带单牌
			index = "801"
			_s.WSK_PlayEffect(WSK_EffectEnum.feiJin)
		elseif cardType == NewCardTypeEnum.NCCT_THREE_LINE_ADD_SAME_DOUBLE then
			-- 飞机带相同数量的对牌
			index = "801"
			_s.WSK_PlayEffect(WSK_EffectEnum.feiJin)
		elseif cardType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO then
			-- 四带二
			index = "1101"
		elseif cardType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO_DOUBLE then
			-- 四带两对
			index = "1102"
		elseif cardType == NewCardTypeEnum.NCCT_BOMB then
			-- 炸弹
			index = "1500"
			_s.WSK_PlayEffect(WSK_EffectEnum.bomb)
		elseif cardType == NewCardTypeEnum.NCCT_ROCKET then
			-- 火箭
			index = "1600"
			AudioManager.PlayCommonSound(UIAudioEnum.Rocket)
		else
		end
	end

	if index ~= "" then
		WrapSys.AudioSys_PlayEffect(path .. index)
	end
end

-- 斗地主报剩余的牌数
function AudioManager.DDZ_PlayCardNum(sex, num)
	sex = AudioManager.CheckSex(sex)
	local path = ""
	if sex == 1 then
		path = "DouDiZhu/Nan/CardNum/"
	else
		path = "DouDiZhu/Nv/CardNum/"
	end
	
	local index = ""
	if num == 1 or num == 2 then
		local random = math.random(1, 2)
		index = num * 10 + random
	end

	if index ~= "" then
		WrapSys.AudioSys_PlayEffect(path .. index)
	end
end

-- 播放牌落桌的声音
function AudioManager.DDZ_PlayCardInTableByRsp(rsp)
	local paiClass = rsp.paiClass
	if paiClass == 1 or paiClass == 2 or paiClass == 3 or paiClass == 4 or
		paiClass == 30 or paiClass == 36 or paiClass == 38 then
		AudioManager.PlayCommonSound(UIAudioEnum.ie_brick)
	end
end

function AudioManager.DDZ_PlayCardInTableByCardType(cardType)
	if cardType == NewCardTypeEnum.NCCT_SINGLE or
		cardType == NewCardTypeEnum.NCCT_DOUBLE or
		cardType == NewCardTypeEnum.NCCT_THREE or
		cardType == NewCardTypeEnum.NCCT_THREE_ADD_ONE or
		cardType == NewCardTypeEnum.NCCT_THREE_ADD_DOUBLE or
		cardType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO or
		cardType == NewCardTypeEnum.NCCT_FOUR_ADD_TWO_DOUBLE then
		AudioManager.PlayCommonSound(UIAudioEnum.ie_brick)
	end
end

-- 播放斗地主春天，反春天声音(rsp为下结算信息)
function AudioManager.DDZ_PlaySpringAudio(rsp)
	local overState = rsp.overStatus
	if overState == 4 or overState == 5 then
		AudioManager.PlayCommonSound(UIAudioEnum.Spring)
	end
end

function AudioManager.DDZ_PlayWinOrLose(isWin)
	local path = "DouDiZhu/Common/"
	
	if isWin == true then
		path = path .. "Win"
	elseif isWin == false then
		path = path .. "Lose"
	else
		path = ""
	end

	if path ~= "" then
		WrapSys.AudioSys_PlayEffect(path)
	end
end

---------------------------------------------------tools---------------------------------------------------------------------
function AudioManager.CheckSex( sex )
	if type(sex) == "boolean" then
		if sex then
			sex = 1
		else
			sex = 2
		end
	end
	return sex
end

function AudioManager.PauseVoice()
	WrapSys.PauseVoice(true)
end

function AudioManager.ResumeVoice()
	WrapSys.PauseVoice(false)
end

