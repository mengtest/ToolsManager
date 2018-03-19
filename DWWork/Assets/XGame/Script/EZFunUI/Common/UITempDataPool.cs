/************************************************************
    File      : UITempDataPool.cs
    brief     : UI 窗口之间相互传数据的 数据池, 单次存储
    author    : JanusLiu   janusliu@ezfun.cn
    version   : 1.0
    date      : 2014/12/17 14:59:53
    copyright : Copyright 2014 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnumUITempData
{
	ETD_RankData = 101,
	ETD_LotteryData = 102,
	ETD_GetCardID = 103,
	ETD_LotteryItem = 104,
	ETD_CardSelect = 105,
	ETD_StoreSelectItem = 107,
	ETD_CardEquipIndex = 109,
    ETD_FubenReward = 110,          // 副本奖励用
	ETD_CPlayerPreLv = 111,
    ETD_FightInfo = 112,           // 战斗信息,用于再来一次
    ETD_ExpeditionReward = 113,     // 过关斩将奖励
	ETD_WatchPeople = 114,
	ETD_CallCardID = 115,
	ETD_PlotTalkID = 116,
	ETD_PlotBossID = 117,
    ETD_RewardData = 118,
	ETD_MainCityUI = 119,
	ETD_PvpInfo	   = 120,
    ETD_GuildFuben = 121,         //公会副本破城奖励
    ETD_OpenNewFunction = 122,
    ETD_OpenRandomBagID = 123,      //随机礼包
    ETD_RandomBagRes = 124,
    ETD_OpenHongbaoID = 125,        //春节红包
    ETD_CollectionData = 126,       //采集物

    ETD_WeaponUpLevel = 127,

    ETD_CardOldUP = 128,
    ETD_CardNewUp = 129,

    ETD_NewMount = 130, // 坐骑
    ETD_TaskInfo = 132,// 任务
    ETD_Nav_Target = 133,//自动寻路目标
    ETD_Nav_Title = 134,//自动寻路抬头
}
public class UITempDataPool 
{
	private static Dictionary<int, object> m_tempDataDic = new Dictionary<int, object>();

	public static void InsetData(EnumUITempData enumUITempData, object data)
	{
		if(m_tempDataDic.ContainsKey((int)enumUITempData))
		{
			m_tempDataDic.Remove((int)enumUITempData);
		}

		m_tempDataDic.Add((int)enumUITempData, data);
	}

	public static object GetData(EnumUITempData enumUITempData)
	{
		if(m_tempDataDic.ContainsKey((int)enumUITempData))
		{
			object data = m_tempDataDic[(int)enumUITempData];

			if(!ResourceMgr.CheckCanUIAlwaysStore())
			{
				if (enumUITempData == EnumUITempData.ETD_MainCityUI || enumUITempData == EnumUITempData.ETD_WatchPeople)
				{
					m_tempDataDic.Remove((int)enumUITempData);
				}
			}

			return data;
		}
		return null;
	}
}