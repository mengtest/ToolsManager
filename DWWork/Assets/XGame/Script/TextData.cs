using UnityEngine;
using System.Collections;

[LuaWrap]
public enum EnumText
{
	ET_NONE = 0,
    ET_CHRNAMEILLEGAL = 11,             // 名字非法
	ET_ErrSure = 51,
	ET_ErrCancel = 52,
	ET_ErrAgain = 53,
	ET_ErrReconnectSvr = 54,
    ET_Task = 66,
    ET_Fuben = 67,
	ET_GameFail = 1001,
	ET_ZHAN = 1002,
	ET_USED = 1003,
	ET_USE = 1004,
	ET_SUMMON_DESCRIB = 1005,
	ET_ACTIVE = 1012,
	ET_INACTIVE = 1013,
	ET_MAX_HP = 1014,
	ET_DODGE = 1015,
	ET_ATTACK = 1016,
	ET_MAGIC = 1017,
	ET_DEFENSE = 1018,
	ET_MAGIC_DEFENSE = 1019,
	ET_HIT = 1020,
	ET_CRIT = 1021,
	ET_Pause = 1022,
	ET_OK = 1023,
	ET_NO = 1024,
    ET_StarUp = 1025,
	ET_WakeUp = 1026,
	ET_FujiangWearLevelGreen = 1027,
	ET_FujiangWearLevelRed = 1028,
	ET_Equip = 1029,
	ET_EquipBind = 1030,
	ET_Combainformula = 1031,
	ET_CardPersonCountryWei = 1032,	
	ET_CardPersonCountryShu,
	ET_CardPersonCountryWu,
	ET_CardPersonCountryHaojie,
	ET_CardPersonQualityWhite,
	ET_CardPersonQualityGreen,
	ET_CardPersonQualityBlue,
	ET_CardPersonQualityPurple,
	ET_MSTIME = 1040,
	ET_LevelEndCoin = 1046,
	ET_LevelEndExp,
	ET_LevelEndAgain,
	ET_LevelEndBackMain,
	ET_LevelEndRewardTime,
	ET_LevelEndRewardHurt,
	ET_LevelEndRewardHP,
	ET_LevelEndGiveUp,
	ET_LevelEndRes,
	ET_BuyPoint = 1055,
	ET_ShakeMoney = 1056,
	ET_LevelUnlock = 1057,
	ET_FightPower = 1058,
	ET_Qulity = 1059,
	ET_Combains = 1060,
    ET_LOCKED = 1061,
	ET_StongLevel = 1062,
	ET_Life = 1063,
	ET_CritNormal = 1064,
	ET_CostMoney = 1065,
	ET_CurrentTime = 1066,
	ET_ByPhysicalTime = 1067,
	ET_NextPhysical = 1068,
	ET_RecoverPower = 1069,
	ET_RecoverSpace = 1070,
	ET_FullPower = 1071,
	ET_BuyPowerTimes = 1072,
	ET_ShowDeTail = 1073,
	ET_Buy = 1074,
	ET_CantEquip = 1078,
	ET_CanEquip = 1079,
	ET_NoEquip = 1080,
	ET_CanCombain = 1081,
	ET_CantCombain = 1082,
    ET_BuyDiamondCost = 1083,
	ET_NextLevel = 1084,
	ET_PowerUp = 1085,
	ET_StarUpAndUnLock = 1088,
	ET_ExtraEffect = 1089,
	ET_ShengxingDialog = 1090,
	ET_CantLevelUp = 1091,
	ET_CantStarUp = 1092,
	ET_CantLevelUpMoney = 1093,
	ET_CantPersonCantLevelUp = 1094,
	ET_LevelTop = 1095,
	ET_WakeupSkill = 1096,
	ET_NewSkill = 1097,
	ET_WEISHANGZHEN = 1104,
	ET_KickOff = 1123,
    ET_0  = 1124,               // 零
    ET_1  = 1125,
    ET_2  = 1126,
    ET_3  = 1127,
    ET_4  = 1128,
    ET_5  = 1129,
    ET_6  = 1130,
    ET_7  = 1131,
    ET_8  = 1132,
    ET_9  = 1133,
    ET_10  = 1134,              // 十
    ET_SkipGuideConfirm = 1160,             // 确定跳过新手引导

	//rankwindow
	ET_RankWindowTitle = 1201,
	ET_RankWindowExit,
	ET_RankWindowMyRank,
	ET_RankWindowCombatRank,
	ET_RankWindowRankNum = 1205,
	ET_RankWindowRankName,
	ET_RankWindowRankLv,
	ET_RankWindowRankCombat,
	ET_RankWindowRankDontRank,
	ET_RankWindowRankPvpRank = 1210,
	ET_RankWindowRankLvRank,
    ET_RankWindow3V3Rank = 1238,
    ET_RankWindow1V1Rank = 1239,
    ET_RankWindowCharm = 1240,
    ET_RankWindowGuoGuan = 1241,
    ET_RankWindowQianRenZhan = 1244,

    ET_RankWindowEnd = 1300,

	//PVP texts
	ET_PVPLost = 1400,
	ET_PVPWin = 1401,
	ET_PVPRanking1 = 1402,
	ET_PVPRanking2 = 1403,
	ET_OutOfBuyTimes = 1044,
	ET_DuelImme = 1405,
	ET_Refresh = 1406,
	ET_PVPResetCD = 1407,
	ET_PVPBuyTimes = 1408,
	ET_PVPCDing = 1409,
	ET_PVPOutOfChance = 1410,
	ET_PVPIntroduce1 = 1411,
	ET_PVPIntroduce2 = 1412,
	ET_PVPIntroduce3 = 1413,
	//
	ET_PVPTimeD = 1428,
	ET_PVPTimeH = 1429,
	ET_PVPTimeM = 1430,
	ET_PVPTimeS = 1431,
    ET_PVPFujiang = 1433,
    ET_PVPWeapon = 1434,
    ET_PVPFightPower = 1435,
    ET_PVPNoRecord = 1436,
    ET_PVPGoBuyDiamond = 1440,

	//Lottery window
	ET_LotteryGoldBox = 1501,
	ET_LotteryDiamondBox,
	ET_LotteryDesc,
	ET_LotterySecDesc,
	ET_LotteryDraw = 1505,
	ET_LotteryDrawTen,
	ET_LotteryDrawAgain,
	ET_LotterySure,
	ET_LottertCardRate,
	ET_LotteryGainItem = 1510,
	ET_LotteryCenter,
	ET_LotteryHigh,
	ET_LotteryVeryHigh,
	ET_LotteryFree,
	ET_LotteryFreeNums = 1515,
	ET_LotteryGetDisableCard,
	ET_LotteryGetEnbaleCard,
	ET_LotteryDrawAgainTen,
	ET_LotteryNotOpenGold,
	ET_LotteryNotOpenDiamond = 1520,
	ET_LotteryNoGoldFreeNumbers,
	ET_LotteryHelpTitle,
	ET_LotteryHelpContent,
	ET_LotteryTenDiamondTips,
    ET_LotteryTips = 1540,
    ET_LotteryEnd = 1600,

	//WeaponSys  
	ET_WS_MAX_HP = 1703,
	ET_WS_ATTACK_UP = 1704,
	ET_WS_MAGIC_UP = 1705,
	ET_WS_DEFENSE_UP = 1706,
	ET_WS_MAGIC_DEFENSE_UP = 1707,
	ET_WS_HIT_UP = 1708,
	ET_WS_DODGE_UP = 1709,
	ET_WS_CRIT_UP = 1710,
	ET_WS_StrongWeapon = 1715,
    ET_WS_OWN = 1720,
    ET_WS_NOOWN = 1721,
	ET_MAX_HP_UP = 1722,
	ET_ATTACK_UP = 1723,
	ET_MAGIC_UP = 1724,
	ET_DEFENSE_UP = 1725,
	ET_MAGIC_DEFENSE_UP = 1726,
	ET_HIT_UP = 1727,
	ET_DODGE_UP = 1728,
	ET_CRIT_UP = 1729,
	ET_WS_NOTENOUGH = 1730,
	ET_WS_TOP_LEVEL = 1731,
	ET_WS_CAN_WEAE = 1735,

	ET_WS_STRONG_OK = 1739,
    ET_WS_SETMAINWEAPON = 1740,
    ET_WS_SETFUWEAPON = 1741,
    ET_WS_NOOPENEQUIP = 1745,
    ET_WS_NOTMETIAL = 1747,
    ET_WS_NOENOUGHLEVEL = 1760,
    ET_WS_STRONG = 1761,
    ET_WS_LEVELUP = 1762,
    ET_WS_SETDEFFWEAPONONE = 1763,
	ET_WS_Weapon1,
	ET_WS_Weapon2			= 1765,
    ET_WS_HASWeaponMain = 1766,
    ET_WS_HASWeaponOff = 1767,
	ET_WS_ConbainMaterialNotEnough = 1772,
    ET_WS_SETDEFFWEAPONTWO = 1774,
	ET_WS_CouldCobain = 1775,
	ET_WS_Cobain = 1776,
	ET_WS_NoConditon = 1777,
	ET_WS_CouldUnLock = 1778,
	//combat value window
	ET_CVWTitle = 1601,
	ET_CVWTotalValue,
	ET_CVWEquipSys,
	ET_CVWEquipVale,
	ET_CVWEquipDesc = 1605,
	ET_CVWEquipBtn,
	ET_CVWSkillSys,
	ET_CVWSkillValue,
	ET_CVWSkillDesc,
	ET_CVWSkillBtn = 1610,
	ET_CVWWeaponSys,
	ET_CVWWeaponValue,
	ET_CVWWeaponDesc,
	ET_CVWWeaponBtn,
	ET_CVWCardSys = 1615,
	ET_CVWCardValue,
	ET_CVWCardDesc,
	ET_CVWCardBtn,
	ET_CVWScoreSSS,
	ET_CVWScoreS = 1620,
	ET_CVWScoreA,
	ET_CVWScoreB,
	ET_CVWScoreC,
	ET_PhalanxSys = 1633,
	ET_PhalanxValue,
	ET_PhalanxDesc,
	ET_PhalanxBtn,
	ET_CVWEnd = 1700,

	//VIP
	ET_VipWindowTitle = 1901,
	ET_VipLv,
	ET_VipLvDesc,
	ET_VipNextLvDesc,
	ET_VipLvRights = 1905,
	ET_VipSweepLocked,
	ET_VipSweepTen,
	ET_VipPhysicalBuyTimes,
	ET_VipSweepTickets,
	ET_VipSkillPoints = 1910,
	ET_VipGoldenTimes,
	ET_VipElieTimes,
	ET_VipArenaTimes, 
	ET_VipShopOpen1,
	ET_VipShopOpen2 = 1915,
	ET_VipShopOpen3,
	ET_VipNZBZTimes,
	ET_VipNZBZReward,
	ET_VipAvaterGet,
	ET_VipCludePre = 1920,
	ET_VipFlowerNum = 1922,
	ET_VipFriendMaxNum,
	ET_ViPFriendHelpNum,
	ET_VipFishTime = 1925,
	ET_VipTreasureOpen = 1928,
    ET_vipACAWNaiBei = 1929,
    ET_vipACAWJiuMei = 1930,
    ET_vipACAWNaiBeiSD = 1931,
    //ET_vipACAWGuoGuan = 1932,
    ET_vipACAWGuoGuanSD = 1933,
    ET_vipSTAvatarUseDiamond = 1934, //开启时装强化材料钻石补全
    ET_vipSTWingUseDiamond = 1935, //开启战旗强化材料钻石补全
    ET_vipSendFlower99,
	ET_vipAutoRelease,
	ET_vipRobTimes,
	ET_vipTaResetTimes,
    ET_vipEscorttimes = 1940,
	ET_vipStoreSpecialOff = 1959,
	ET_VIPEnd = 2000,

	//Store
	ET_StoreArena = 2001,
	ET_StoreHave,
	ET_StoreRefresh,
	ET_StoreNextRefreshTime,
	ET_StoreTommorowRefreshTime = 2005,
	ET_StoreExpedtion,
	ET_StoreNormal,
	ET_StoreWanderMan,
	ET_StoreBlackMarket,
	ET_StoreCountry = 2010,
	ET_StoreArenaTapTips1,
	ET_StoreArenaTapTips2,
	ET_StoreArenaTapTips3,
	ET_StoreArenaTapTips4,
	ET_StoreExpedtionTapTips1 = 2015,
	ET_StoreExpedtionTapTips2,
	ET_StoreExpedtionTapTips3,
	ET_StoreExpedtionTapTips4,
	ET_StoreLv1TapTips1,
	ET_StoreLv1TapTips2 = 2020,
	ET_StoreLv1TapTips3,
	ET_StoreLv1TapTips4,
	ET_StoreLv2TapTips1,
	ET_StoreLv2TapTips2,
	ET_StoreLv2TapTips3 = 2025,
	ET_StoreLv2TapTips4,
	ET_StoreLv3TapTips1,
	ET_StoreLv3TapTips2,
	ET_StoreLv3TapTips3,
	ET_StoreLv3TapTips4 = 2030,
	ET_StoreLv4TapTips1,
	ET_StoreLv4TapTips2,
	ET_StoreLv4TapTips3,
	ET_StoreLv4TapTips4,
	ET_StoreFindLv2		= 2035,
	ET_StoreFindLv3,
	ET_StoreFindLv4,
	ET_StoreRefreshDesc,
	ET_StoreDisappearLeftTime,
	ET_StoreNotEnoughCoin = 2040,
	ET_StoreNotEnoughDiamond,
	ET_StoreCanNotBuyCurrency,
	ET_StoreBuyNum,
	ET_StoreSureBuy,
	ET_StoreDispearTime = 2045,
	ET_StoreFlower = 2047,
	ET_StoreFlowerTapTips1,
	ET_StoreFlowerTapTips2,
	ET_StoreFlowerTapTips3,
	ET_StoreFlowerTapTips4,
    ET_StoreZhuanpan = 2055,
    ET_StoreZhuanpanTapTips1,
    ET_StoreZhuanpanTapTips2,
    ET_StoreZhuanpanTapTips3,
    ET_StoreZhuanpanTapTips4,
    ET_StoreRTPVP = 2062,
    ET_StoreRTPVPTapTips1,
    ET_StoreRTPVPTapTips2,
    ET_StoreRTPVPTapTips3,
    ET_StoreRTPVPTapTips4,
	ET_StoreSpecialOff = 2067,
	ET_StoreSpecialOffTips1,
	ET_StoreSpecialOffTips2,
	ET_StoreSpecialOffTips3,
	ET_StoreSpecialOffTips4,
    ET_StoreRTPVP1V1 = 2073,
    ET_StoreRTPVP1V1TapTips1,
    ET_StoreRTPVP1V1TapTips2,
    ET_StoreRTPVP1V1TapTips3,
    ET_StoreRTPVP1V1TapTips4,
	ET_StoreEnd = 2100,

	//task
	ET_CompleteTask = 2201,
	ET_MainTask = 2202,
	ET_DailyTask = 2203,

	ET_PackHp			= 2902,
	ET_PackPhysicAtt,
	ET_PackMagicAtt,
	ET_PackPhysicDef	= 2905,
	ET_PackMagicDef,
	ET_PackHit,
	ET_PackDodge,
	ET_PackCrit,

	ET_PackHp1 	= 2929,
	ET_PackPhysicAtt1,
	ET_PackMagicAtt1,
	ET_PackPhysicDef1,
	ET_PackMagicDef1,
	ET_PackHit1,
	ET_PackDodge1,
	ET_PackCrit1,

	//pack
	ET_PACK_Combain = 3001,
	ET_PACK_Detail,
	ET_PACK_Use,
    ET_PACK_MAX_HP = 3005,
    ET_PACK_ATTACK_UP,
    ET_PACK_MAGIC_UP,
    ET_PACK_DEFENSE_UP,
    ET_PACK_MAGIC_DEFENSE_UP,
    ET_PACK_HIT_UP,
    ET_PACK_DODGE_UP,
    ET_PACK_CRIT_UP,
    ET_PACK_NEEDLEVEL = 3013,

	//card sys
	ET_CardTeamA 			= 3101,
	ET_CardTeamB,
	ET_CardTeamC,
	ET_CardAllCountry,
	ET_CardCountryWei		= 3105,
	ET_CardCountrySu,
	ET_CardCountryWu,
	ET_CardCountryHj,
	ET_CardCombatValue,
	ET_CardUse				= 3110,
	ET_CardUsed,
	ET_CardDsableLabel,
	ET_CardCall,
	ET_CardCallHero,
	ET_CardProperty			= 3115,
	ET_CardPpHp,
	ET_CardPpPhysicAtt,
	ET_CardPpMagicAtt,
	ET_CardPpPhysicDefense,
	ET_CardPpMagicDefense	= 3120,
	ET_CardPpHit,
	ET_CardPpDodge,
	ET_CardPpCrit,
	ET_CardCallDesc,
	ET_CardProperty1		= 3125,
	ET_CardAdvUp,
	ET_CardStarUp,
	ET_CardPropertyBtn,
	ET_CardSkillBtn,
	ET_CardShangZhen		= 3130,
	ET_CardXiaZhen,
	ET_CardGetPath,
	ET_CardBack,
	ET_CardAdvUpDesc,
	ET_CardLvUpDesc			= 3135,
	ET_CardNoEnoughCoin,
	ET_CardCancel,
	ET_CardEquipDesc,
	ET_CardEquip,
	ET_CardStarUpDesc		= 3140,
	ET_CardSure,
	ET_CardBuffSkill,
	ET_CardCaptain,
	ET_CardZhan,
	ET_CardEnough			= 3145,
	ET_CardFilter,
	ET_CardClickShangZhen,
	ET_CardWinTitle,
	ET_CardHeCheng,
	ET_CardKeHeCheng		= 3150,
	ET_CardWuZhuangBei,
	ET_CardWeiZhuangBei,
	ET_CardArena,
	ET_CardWakeUp,
	ET_CardEquipOwnerNum	= 3155,
	ET_CardEquipGetPath,
	ET_CardHelp,
	ET_CardHelpinfo,
	ET_CardWan,
	ET_CardFriend			= 3160,
	ET_CardAddHeroCombat,
	ET_CardCurrentCombat,
	ET_CardOpenLv,
	ET_CardKnown			= 3165,
	ET_CardFightCombat,
	ET_CardFull,
	ET_CardHasUsed,
	ET_CardEquipMatNotEnough,
	ET_CardFubenNoOpen		= 3170,
	ET_CardAdvTopLv,
	ET_CardHasEmptyPos,
	ET_CardZDL,
    ET_CardStartFight,
    ET_CardOk               = 3175,
	ET_CardMaxAdvLv,
	ET_CardMaxStarLv,
	ET_CardNextStarLvAtt,
	ET_CardNeedLv,
	ET_CardCombainformula	= 3180,
	ET_CardCombatScore      = 3182,
	ET_CardSysEnd 			= 3200,

    ET_CardEasy             = 3311,
    ET_CardNormal,
    ET_CardHard,
    ET_CardXiuluo,
	//Diamond

	ET_YueKaName = 2806,
	ET_DiamondDouble = 2812,
	ET_DiamondExtraGet = 2813,
	ET_DiamondEveryDayGet = 2814,
	ET_DiamondPrice,

	ET_DiamondSoonToVip = 2817,
	ET_DiamondMoreCharge = 2818,
	ET_DiamondCongratuTuhao = 2819,
	ET_DiamondGet = 2820,
	ET_DiamondBuy = 2835,
	ET_DiamondFirstBuyFree,
	ET_DiamondBuyFree,
	ET_YueKaLeftDays,
	ET_YueKaDesc,

	//skill
	ET_S_CURRWEAPON = 3201,
	ET_S_ACTIVE = 3202,
	ET_S_NEXT_BUFF_PREREQUISITE = 3203,
	ET_S_BUFF_TITLE = 3204,
	ET_S_NORMAL_SKILL_TITLE = 3205,
	ET_S_SPECIAL_SKILL_TITLE = 3206,
	ET_S_QTE = 3207,
	ET_S_UNLOCKED = 3208,
	ET_S_UNLOCKINFO = 3209,
	ET_S_SKILL_BASE_DESC = 3210,
	ET_S_QTE_BASE_DESC = 3211,
	ET_S_SKILLLEVELBUFF = 3212,
	ET_S_PlayerLevelUnlockSkill = 3213,
	ET_S_LOCKED = 3214,
	ET_S_DPS = 3215,
	ET_S_NEXTLVL = 3216,
	ET_S_POINT = 3217,
	ET_S_COST_MONEY = 3218,
	ET_S_COST_POINT = 3219,
	ET_S_LVLUP = 3220,
	ET_S_SKILL = 3221,
	ET_S_IKNOW = 3222,
	ET_S_NOBUFF = 3223,
	ET_S_LEVEL_MAXED = 3224,
	ET_S_MAIN_WEAPON = 3225,
	ET_S_OFF_WEAPON = 3226,
	ET_S_PHY_ATTACK_BUFF = 3227,
	ET_S_MAG_ATTACK_BUFF = 3228,
	ET_S_CRIT_BUFF = 3229,
    ET_S_Unlock = 3232,
    ET_S_NeedWeaponToUnlock = 3233,

	ET_PortectGril3ErrTips = 3408,

    ET_SignIn_Title           = 3601,
    ET_SignIn_SignNum         = 3602,
    ET_SignIn_VipRewards      = 3603,
    ET_SignIn_HasSign         = 3604,
    ET_SignIn_RewardsTips     = 3605,
    ET_SignIn_TodayReward     = 3606,
    ET_SignIn_TodayGain       = 3607,
    ET_SignIn_TodayVipExtra   = 3608,
    ET_SignIn_VipRewardsTips  = 3609,
    ET_SignIn_HelpInfo        = 3610,
    ET_SignIn_HelpBtn         = 3611,
    ET_SignIn_HelpTitle       = 3612,
    ET_SignIn_ShortHelp       = 3613,

    ET_HONGBAO = 3901,

    ET_DELFRIEND = 4001,
    ET_ALLDEL = 4002,
    ET_ADDFRIEND = 4003,
    ET_DELFRIENDSUCCEED = 4004,
    ET_LASTTIME = 4005,
    ET_MYSTERY = 4006,
    ET_FRIENDNUM = 4007,
    ET_NOFRIENDTOADD = 4008,
    ET_NOFRIENDADD = 4009,
    ET_IDERRORINPUT = 4115,
    ET_SENDINVITEFINISHED = 4116,
    ET_IDERROREMPTY = 4118,
    ET_SENDADDFRIEND = 4119,

	ET_ChargeInquiry 		= 7001,
	ET_CI_LeftMoney,
	ET_CI_Note,
	ET_CI_AutoSelect,
	ET_CI_CheckTotal1		= 7005,
	ET_CI_CheckTotal2,
	ET_CI_SumbitBtn,
	ET_CI_SumbitNote,
	ET_CI_SumbitYes,
	ET_CI_SumbitNo			= 7010,
	ET_CI_YueKaNote,
	ET_CI_WinTitle,

	ET_WeaponConbainPreConditonNot = 61401,
	ET_MainCityCompeleteAllLv1Normal		= 62102,
	ET_MainCityCompeleteAllLv1Hard,
	ET_MainCityCompeleteAllLv2Easy,
	ET_MainCityPlayerLvTo24				= 62105,
	ET_MainCityPlayerLvTo30,
	ET_MainCityFunctionNotOpen,
	ET_MainCityOpenNeedLv,
    
	//World Boss
	ET_WB 								= 80001,
	ET_WB_LV40OPEN,
	ET_WB_ActivityBeginLeftTime,
	ET_WB_ActivityEndLeftTime,
	ET_WB_RANKTitle						= 80005,
	ET_WB_RANK,
	ET_WB_PlayerName,
	ET_WB_Damage,
	ET_WB_ClickShanZhen,
	ET_WB_BackMainCity					= 80010,
	ET_WB_AttWorldBoss,
	ET_WB_ResurrectionLeftTime,
	ET_WB_ResurrectionNow,
	ET_WB_WorldBossName,
	ET_WB_ThisACRank					= 80015,
	ET_WB_WorldBossKiller,
	ET_WB_WinDesc,
	ET_WB_WinDesc1,
	ET_WB_FailDesc,
	ET_WB_WinDesc2						= 80020,
	ET_WB_PhysicalDef,
	ET_WB_MagicDef,
	ET_WB_HelpTips,
	ET_WB_HelpTipsDesc1,
	ET_WB_HelpTipsDesc2					= 80025,
	ET_WB_HelpTipsDesc3,
	ET_WB_HelpTipsDesc4,
	ET_WB_ExitDesc,
	ET_WB_ActivityBroadCast,
	ET_WB_ActivityBoradCast1			= 80030,
	ET_WB_ActivityBroadCast2,
	ET_WB_ActivityFailDesc3,
	ET_WB_DONOT_JOIN,
	ET_WB_FailDesc1						= 80040,
	ET_WB_CommonInLeft,
	ET_WB_BossAppearName,
	ET_WB_FriendHelpName,
	ET_WB_CardCardName,

    ET_LotteryHalfDiamond       = 500024,

	ET_CutSceneSubtitle01 = 590010,
	ET_CutSceneSubtitle02 = 590020,
	ET_CutSceneSubtitle03 = 590030,
	ET_CutSceneSubtitle04 = 590040,
	ET_CutSceneSubtitle05 = 590050,
	ET_CutSceneSubtitle06 = 590060,
	ET_CutSceneSubtitle07 = 590070,
	ET_CutSceneSubtitle08 = 590035,
	ET_CutSceneSubtitle09 = 590075,
	ET_CutSceneSubtitle10 = 590080,

    ET_CountryTip = 590096,

	ET_CutSceneTeachMoveKey = 90051,
	ET_CutSceneTeachAttack = 90052,
	ET_CutSceneTeachSkill = 90053,
	ET_CutSceneTeachStoneThrow = 90054,
	ET_CutSceneTeachRoll = 90055,

	//figure
	ET_FigureChangeImage = 100102,
	ET_FigureChangeName,
	ET_FigureHeroExp,
	ET_FigureCountId,
	ET_FigureFactionName,
	ET_FigureFactionID,
	ET_FigureExitFaction,
	ET_FigureSystemSetting,
	ET_FigureVipPrivilege,

	//openNewFunction
	ET_OpenNewFunctionLevelTips	=100201,
	ET_OpenNewFunctionLevel,
	ET_OpenNewFunctionDif1,
	ET_OpenNewFunctionDif2,
	ET_OpenNewFunctionDif3		=100205,
	ET_OpenNewFunctionDif4,


    // guild
    ET_GuildCreateDiamondLack = 100301,     // 创建公会钻石不足
    ET_GuildCreateMoneyLack = 100302,       // 创建公会金币不足
    ET_GuildNameTooLong = 100303,           // 公会名太长
    ET_GuildNameText = 100304,              // 公会名（最多四字）
    ET_NoGuildName = 100305,                // 请输入公会名
    ET_GuildNameIllegal = 100306,           // 公会名无效
    ET_GuildApplySuc = 100307,              // 申请加入公会成功
    ET_Guild308 = 100308,                   // 解散
    ET_Guild309 = 100309,                   // 解散公会
    ET_Guild310 = 100310,                   // 退出
    ET_Guild311 = 100311,                   // 退出公会
    ET_Guild312 = 100312,                   // 确定解散？
    ET_Guild313 = 100313,                   // 确定退出？
    ET_Guild314 = 100314,                   // 踢出公会
    ET_Guild315 = 100315,                   // 确定要踢出#STR0#?
    ET_Guild316 = 100316,                   // 确定要把#STR0#提升为#STR1#?
    ET_Guild317 = 100317,                   // 确定要把#STR0#转让给#STR1#?
    ET_Guild318 = 100318,                   // 会长
    ET_Guild319 = 100319,                   // 副会长
    ET_Guild320 = 100320,                   // 成员
    ET_Guild485 = 100485,                   // 长老
    ET_Guild321 = 100321,                   // 设置招募
    ET_Guild322 = 100322,                   // 发布招募
    ET_Guild323 = 100323,                   // 允许自动加入
    ET_Guild324 = 100324,                   // 批准
    ET_Guild325 = 100325,                   // 一键拒绝
    ET_Guild326 = 100326,                   // 小时前
    ET_Guild327 = 100327,                   // 天前
    ET_Guild328 = 100328,                   // 个月前
    ET_Guild329 = 100329,                   // 退出了公会，道不同不相为谋！
    ET_Guild330 = 100330,                   // 被踢出了公会，大家引以为戒！
    ET_Guild331 = 100331,                   // 加入了公会，打天下时又多了一个兄弟，大家热烈欢迎！
    ET_Guild332 = 100332,                   // 被任命为#STR0#！
    ET_Guild333 = 100333,                   // 被撤销#STR0#职务，大家引以为戒！
    ET_Guild334 = 100334,                   // 没有符合条件的公会，加入失败
    ET_Guild335 = 100335,                   // 入会条件
    ET_Guild336 = 100336,                   // 要求战力
    ET_Guild337 = 100337,                   // 未达到入会条件
    ET_Guild338 = 100338,                   // 恭喜你已经加入#STR0#公会
    ET_Guild339 = 100339,                   // 你加入#STR0#公会的申请已提交
    ET_Guild340 = 100340,                   // 对不起没有找到符合要求的公会。
    ET_Guild341 = 100341,                   // 小于1小时
    ET_Guild342 = 100342,                   // 个请求
    ET_Guild343 = 100343,                   // 年前
    ET_Guild344 = 100344,                   // 在线
    ET_Guild345 = 100345,                   // 【
    ET_Guild346 = 100346,                   // 】
    ET_Guild347 = 100347,                   // (
    ET_Guild348 = 100348,                   // )
    ET_Guild349 = 100349,                   // 下次发布
    ET_Guild350 = 100350,                   // 招募成员
    ET_Guild351 = 100351,                   // 降低职务 
    ET_Guild352 = 100352,                   // 提升职务
    ET_Guild353 = 100353,                   // 转让会长 
    ET_Guild354 = 100354,                   // 加入公会
    ET_Guild355 = 100355,                   // 申请提交
    ET_Guild356 = 100356,                   // 撤销职务 
    ET_Guild357 = 100357,                   // 申请退回
    ET_Guild358 = 100358,                   // 不填写则不限制战斗力
    ET_Guild359 = 100359,                   // 招募发布成功
    ET_Guild360 = 100360,                   // 公告字数限制
    ET_Guild362 = 100362,                   // 公会招募成员，天下需要你我一起打！
    ET_Guild363 = 100363,                   // 会长太懒，没有撰写公会公告    
    ET_Guild364 = 100364,                   // 提升职务成功
    ET_Guild365 = 100365,                   // 降低职务成功
    ET_Guild366 = 100366,                   // 转让会长成功
    ET_Guild367 = 100367,                   // 踢出公会成功
    ET_Guild368 = 100368,                   // #STR0#把公会会长转让给了#STR1#
    ET_Guild369 = 100369,	                // 公会信息
    ET_Guild370 = 100370,	                // 公会藏宝阁
    ET_Guild371 = 100371,	                // 公会副本
    ET_Guild372 = 100372,	                // 公会战利品
    ET_Guild407 = 100407,	                // 公会副本日志
    ET_Guild408 = 100408,	                // 公会副本日志
    ET_Guild410 = 100410,	                // 公会商店
    ET_Guild414 = 100414,	                // 申请
    ET_Guild415 = 100415,               	// 已申请
    ET_Guild416 = 100416,	                // 申请队列
    ET_Guild419 = 100419,	                // #STR0#后自动分配
    ET_Guild421 = 100421,	                // 你现在在#STR0#的申请队列中.排名第#STR1#.确认要放弃申请改为加入#STR2#的申请队列？
    ET_Guild422 = 100422,	                // 确认申请
    ET_Guild426 = 100426,	                // 公会商店
    ET_Guild427 = 100427,	                // 创建成功
    ET_Guild428 = 100428,	                // 剩余#STR0#步
    ET_Guild435 = 100435,               	// 购买一次
    ET_Guild436 = 100436,	                // 购买全部
    ET_Guild437 = 100437,           	    // 钻石可购买一次
    ET_Guild438 = 100438,	                // 第#STR0#关奖励
    ET_Guild439 = 100439,	                // 在藏宝阁中通过第3关
    ET_Guild442 = 100442,       	        // 已到购买上限
    ET_Guild446 = 100446,       	        // 在藏宝阁中通过第3关
    ET_Guild450 = 100450,                   // 投掷次数已用完
	ET_Guild452	= 100452,                   // 未达到#STR0#级，公会还没有开启
    ET_Guild455 = 100455,                   // 公会矿场
    ET_Guild456 = 100456,                   // 今日挖矿次数
    ET_Guild467 = 100467,                   // 已购买与剩余购买次数
    ET_Guild468 = 100468,                   // 周奖励以邮件形式发放，公会成员都有
    ET_Guild469 = 100469,                   // 公会#STR0#级开放
    ET_Guild486 = 100486,                   // 设为成员
    ET_Guild487 = 100487,                   // 设为长老
    ET_Guild488 = 100488,                   // 设为副会长
    ET_Guild489 = 100489,                   // 设置职务错误提示
    ET_GuildGuard = 1100117,                // 公会护镖


    ET_GemBonusAnger = 100520,
	ET_GemBonusHP,
	ET_GemBonusAtt,
	ET_GemBonusDef,
	ET_GemBonusPhysicDef,
	ET_GemBonusHit,
	ET_GemBonusDodge,
	ET_GemBonusCrit,

    ET_Guild073 = 160073,
    ET_Guild074 = 160074,
    ET_Guild075 = 160075,
    ET_Guild076 = 160076,
    ET_Guild077 = 160077,
	ET_Guild080 = 160080,                   // 公会改名
	ET_Guild081 = 160099,                   // 公会自动转让
    ET_Guild078 = 160078,                   //挖矿
    ET_Guild079 = 160079,                   //挖矿五彩石攒满
    ET_GuildDonte = 1100038,                // 公会捐赠
    ET_GuildDonteDesc = 1100039,            // 公会捐赠描述

    //portrait sys
    ET_Portrait601 = 100601,                //领取兑换成功
    ET_Portrait1150 = 1150,                 //本次改名将花费免费改名卡一张
    ET_Portrait1151 = 1151,                 //本次改名将花费免费100钻石
    ET_Portrait1152 = 1152,                 //免费改名卡： #STR0#
    ET_Portrait1153 = 1153,                 //全部选择
    ET_Portrait1154 = 1154,                 //全部取消
    ET_Portrait1155 = 1155,                 //未选择赠送对象
    ET_Portrait1156 = 1156,                 //没有可赠送对象
    ET_Portrait1157 = 1157,                 //#STR0#张改名卡将花费#STR1#钻石
    ET_Portrait1159 = 1159,                 //是否退出至账号登录界面
    // 阵法
    ET_Phalanx1     = 101001,               // #STR0#级阵位
    ET_Phalanx2     = 101002,           	// 未上阵
    ET_Phalanx3     = 101003,       	    // 无加成
    ET_Phalanx4     = 101004,	            // 升阶阵法
    ET_Phalanx5     = 101005,	            // 阵法加成
    ET_Phalanx6     = 101006,	            // 境界详情
    ET_Phalanx7     = 101007,	            // 一键上阵
    ET_Phalanx8     = 101008,       	    // 手动上阵
    ET_Phalanx9     = 101009,       	    // 境界加成
    ET_Phalanx10    = 101010,	            // 没有境界
    ET_Phalanx11    = 101011,               // 阵位加成
    ET_Phalanx12    = 101012,               // 级
    ET_Phalanx13    = 101013,	            // 升阶阵位
    ET_Phalanx14    = 101014,	            // #STR0#级阵位熟练度：#STR1#
    ET_Phalanx15    = 101015,	            // 1倍暴击
    ET_Phalanx16    = 101016,	            // 2倍暴击
    ET_Phalanx17    = 101017,	            // 3倍暴击
    ET_Phalanx18    = 101018,	            // 4倍暴击
    ET_Phalanx21    = 101021,               // 上阵#STR0#个#STR1#星武将可激活
    ET_Phalanx22    = 101022,               // 熟练度+
    ET_Phalanx26 = 101026,               // 上阵#STR0#个觉醒#STR1#武将可激活

    ET_Chibi_SceneBuff = 171901,            //赤壁场景buff

    ET_CHRNAMELENGTHLIMITE = 108103,        // 名字长度限制
    ET_INPUTCHRNAME = 108105,               // 请输入角色名字


    //国家
    GuildNotice           = 1100001,          //公会公告

    ET_Nav_Title_Nav = 2201001,             //寻路抬头-寻路
    ET_Nav_Title_Follow = 2201002,          //寻路抬头-跟随

	//荆州战场
	JingZhou_MonsterAppear	= 2610750,		//#str0 开始参与战斗了
}


public class TextData {

	public static string GetText(EnumText enumText)
	{
		return GetText((int)enumText);
	}

	public static string GetText(int id)
	{
		string text = "";
		
		TableLoader resourceSys = TableLoader.Instance;
		ezfun_resource.ResText resText = (ezfun_resource.ResText)resourceSys.GetEntry<ezfun_resource.ResTextList> (id);
		if(resText != null)
		{
			text = resText.text.Replace("\\n" , "\n");
		}
		else
		{
			text = "[NO LABEL]" + id; 
		}
		return text;
	}
	
	public static string GetText(EnumText enumText, params string[] stringArray)
	{
		return GetText((int)enumText, stringArray);
	}

	public static string GetText(int id, params string[] stringArray)
	{
		string text = "";
		
		TableLoader resourceSys = TableLoader.Instance;
		ezfun_resource.ResText resText = (ezfun_resource.ResText)resourceSys.GetEntry<ezfun_resource.ResTextList> (id);

        if(resText != null)
        {
            text = resText.text.Replace("\\n" , "\n");
        }
        else
        {
            text = "[NO LABEL]" + id;
        }

        return GetText(text, stringArray);;
	}	

    public static string GetText(string text, params string[] stringArray)
    {
        for(int i = 0; i < stringArray.Length; i ++)
        {
            string replaceStr = string.Format("#STR{0}#", i.ToString());
            text = text.Replace(replaceStr, stringArray[i]);
        }
        return text;
    }   


}
