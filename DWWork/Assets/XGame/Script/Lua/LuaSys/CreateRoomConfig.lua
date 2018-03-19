
--------------------------------------------------------------------------------
--   File	  : createRoom_config.lua
--   author	: zx
--   function  : 创建房间配置
--   date	  : 2017-10-21
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------
CreateRoomConfig = {}
local _s = CreateRoomConfig

local Vector3 = UnityEngine.Vector3
local Vector2 = UnityEngine.Vector2

_s.GameGroup = 
{
	[0] = 
	{
		Common_PlayID.chongRen_MJ,
		Common_PlayID.chongRen_510K,
		Common_PlayID.leAn_MJ,
		Common_PlayID.leAn_510K,
		Common_PlayID.yiHuang_MJ,
		Common_PlayID.yiHuang_510K,
		Common_PlayID.ThirtyTwo,
		Common_PlayID.DW_DouDiZhu,
		-- Common_PlayID.ZhaJinHua,
	},
	[1] = {Common_PlayID.chongRen_MJ,},
	[2] = {Common_PlayID.chongRen_510K,},
	[3] = {Common_PlayID.leAn_MJ,},
	[4] = {Common_PlayID.leAn_510K,},
	[5] = {Common_PlayID.yiHuang_MJ,},
	[6] = {Common_PlayID.yiHuang_510K,},
	[7] = {Common_PlayID.ThirtyTwo},
	[8] = {Common_PlayID.DW_DouDiZhu},
	[9] = {Common_PlayID.ZhaJinHua},
	[-1] = {Common_PlayID.chongRen_MJ, Common_PlayID.chongRen_510K},
	[-2] = {Common_PlayID.leAn_MJ, Common_PlayID.leAn_510K},
	[-3] = {Common_PlayID.yiHuang_MJ, Common_PlayID.yiHuang_510K},
	-- [-4] = {Common_PlayID.ThirtyTwo},
}
_s.DefGamePlayID = Common_PlayID.chongRen_MJ

-- 标签配置数据
_s.GameTabConfig = 
{
	[Common_PlayID.chongRen_MJ] = { name = "崇仁麻将", sortOrder = 1 },
	[Common_PlayID.chongRen_510K] = { name = "崇仁打盾",  sortOrder = 2 },
	[Common_PlayID.leAn_MJ] = { name = "乐安麻将", sortOrder = 3},
	[Common_PlayID.leAn_510K] = { name = "乐安打盾", sortOrder = 4},
	[Common_PlayID.yiHuang_MJ] = { name = "宜黄麻将", sortOrder = 5},
	[Common_PlayID.yiHuang_510K] = { name = "宜黄红心5", sortOrder = 6},
	[Common_PlayID.ThirtyTwo] = { name = "三十二张", sortOrder = 7},
	[Common_PlayID.DW_DouDiZhu] = {name = "斗地主", sortOrder = 8},
	[Common_PlayID.ZhaJinHua] = {name = "扎金花", sortOrder = 9},
}

-- 界面配置数据
_s.GamePanelConfig = 
{
	[Common_PlayID.chongRen_MJ] = "createRoom_CRMJ_item",
	[Common_PlayID.chongRen_510K] = "createRoom_CRWSK_item",
	[Common_PlayID.leAn_MJ] = "createRoom_LAMJ_item",
	[Common_PlayID.leAn_510K] = "createRoom_CRWSK_item",
	[Common_PlayID.yiHuang_MJ] = "createRoom_YHMJ_item",
	[Common_PlayID.yiHuang_510K] = "createRoom_YHWSK_item",
	[Common_PlayID.ThirtyTwo] = "createRoom_32Z_item",
	[Common_PlayID.DW_DouDiZhu] = "createRoom_DDZ_item",
	[Common_PlayID.ZhaJinHua] = "createRoom_ZhaJinHua_item",
}

_s.GameToggleItem = "createRoom_toggle_item"


-- 组数据
_s.GameContentConfig = 
{
	[Common_PlayID.chongRen_MJ] = 
	{
		content_1 = 
		{
			Size = Vector2.New(772, 260), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, sortOrder = 1},
				RenShu_Node = { isShow = true, sortOrder = 2},
				ZhiFu_Node = { isShow = true, sortOrder = 3},
				ShaoZhuang_Node = { isShow = true, sortOrder = 4},
			}
		},
		content_2 = {Size = Vector2.New(772, 136), Pos = Vector3.New(0, -55, 0)},
		content_3 = {Size = Vector2.New(772, 136), Pos = Vector3.New(0, -203, 0)},
	}, 
	[Common_PlayID.chongRen_510K] = 
	{
		content_1 = 
		{
			Size = Vector2.New(772, 260), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, offset = Vector3.New(0, -60, 0), sortOrder = 1},
				ZhiFu_Node = { isShow = true, sortOrder = 2},
				Mode_Node = { isShow = true, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(772, 188), Pos = Vector3.New(0, -54, 0)},
	},
	[Common_PlayID.leAn_MJ] = 
	{
		content_1 = 
		{
			Size = Vector2.New(772, 200), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, sortOrder = 1},
				RenShu_Node = { isShow = true, sortOrder = 2},
				ZhiFu_Node = { isShow = true, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(772, 198), Pos = Vector3.New(0, 6, 0)},
		content_3 = {Size = Vector2.New(772, 136), Pos = Vector3.New(0, -205, 0)},
	},
	[Common_PlayID.leAn_510K] = 
	{
		content_1 = 
		{
			Size = Vector2.New(772, 260), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, offset = Vector3.New(0, -60, 0), sortOrder = 1},
				ZhiFu_Node = { isShow = true, sortOrder = 2},
				Mode_Node = { isShow = true, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(772, 188), Pos = Vector3.New(0, -54, 0)},
	},
	[Common_PlayID.yiHuang_MJ] = 
	{
		content_1 = 
		{
			Size = Vector2.New(772, 260), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, sortOrder = 1},
				RenShu_Node = { isShow = true, sortOrder = 2},
				ZhiFu_Node = { isShow = true, sortOrder = 3},
				ShaoZhuang_Node = { isShow = true, sortOrder = 4},
			}
		},
		content_2 = {Size = Vector2.New(772, 129), Pos = Vector3.New(0, -55, 0)},
		content_3 = {Size = Vector2.New(772, 330), Pos = Vector3.New(0, -195, 0)},
	},
	[Common_PlayID.yiHuang_510K] = 
	{
		content_1 = 
		{
			Size = Vector2.New(772, 260), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, offset = Vector3.New(0, -60, 0), sortOrder = 1},
				ZhiFu_Node = { isShow = true, sortOrder = 2},
				FeiJi_Node = { isShow = true, sortOrder = 3},
			}
		},
	},
	[Common_PlayID.ThirtyTwo] = 
	{
		content_1 = {Size = Vector2.New(772, 198), Pos = Vector3.New(0, 217, 0),},
		content_2 = 
		{
			Size = Vector2.New(772, 132), Pos = Vector3.New(0, 6, 0),
			NodeConfig = 
			{
				ZhiFu_Node = { isShow = true, sortOrder = 1},
				TianDi9_Node = { isShow = true, sortOrder = 2},
			}
		},
	},
	[Common_PlayID.DW_DouDiZhu] = 
	{
		content_1 = 
		{
			Size = Vector2.New(772, 198), Pos = Vector3.New(0, 246, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, offset = Vector3.New(0, -60, 0), sortOrder = 1},
				ZhiFu_Node = { isShow = true, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(772, 353), Pos = Vector3.New(0, 35, 0)},
	},
	[Common_PlayID.ZhaJinHua] = 
	{
		content_1 = 
		{
			Size = Vector2.New(772, 198), Pos = Vector3.New(0, 246, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, offset = Vector3.New(0, -60, 0), sortOrder = 1},
				ZhiFu_Node = { isShow = true, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(772, 353), Pos = Vector3.New(0, 35, 0)},
		content_3 = {Size = Vector2.New(772, 110), Pos = Vector3.New(0, -327.5, 0)},
	},
	
}

-- 俱乐部组数据
_s.ClubGameContentConfig = 
{
	[Common_PlayID.chongRen_MJ] = 
	{
		content_1 = 
		{
			Size = Vector2.New(840, 200), Pos = Vector3.New(0, 217, 0), 
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, sortOrder = 1},
				RenShu_Node = { isShow = true, sortOrder = 2},
				ZhiFu_Node = { isShow = false, sortOrder = 3},
				ShaoZhuang_Node = { isShow = true, sortOrder = 4},
			}
		},
		content_2 = {Size = Vector2.New(840, 136), Pos = Vector3.New(0, 5, 0), },
		content_3 = {Size = Vector2.New(840, 136), Pos = Vector3.New(0, -143, 0), },
	}, 
	[Common_PlayID.chongRen_510K] = 
	{
		content_1 = 
		{
			Size = Vector2.New(840, 200), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, offset = Vector3.New(0, -60, 0), sortOrder = 1},
				ZhiFu_Node = { isShow = false, sortOrder = 2},
				Mode_Node = { isShow = true, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(840, 188), Pos = Vector3.New(0, 6, 0)},
	},
	[Common_PlayID.leAn_MJ] = 
	{
		content_1 = 
		{
			Size = Vector2.New(840, 140), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, sortOrder = 1},
				RenShu_Node = { isShow = true, sortOrder = 2},
				ZhiFu_Node = { isShow = false, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(840, 198), Pos = Vector3.New(0, 65, 0)},
		content_3 = {Size = Vector2.New(840, 136), Pos = Vector3.New(0, -145, 0)},
	},
	[Common_PlayID.leAn_510K] = 
	{
		content_1 = 
		{
			Size = Vector2.New(840, 200), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, offset = Vector3.New(0, -60, 0), sortOrder = 1},
				ZhiFu_Node = { isShow = false, sortOrder = 2},
				Mode_Node = { isShow = true, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(840, 188), Pos = Vector3.New(0, 6, 0)},
	},
	[Common_PlayID.yiHuang_MJ] = 
	{
		content_1 = 
		{
			Size = Vector2.New(840, 200), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, sortOrder = 1},
				RenShu_Node = { isShow = true, sortOrder = 2},
				ZhiFu_Node = { isShow = false, sortOrder = 3},
				ShaoZhuang_Node = { isShow = true, sortOrder = 4},
			}
		},
		content_2 = {Size = Vector2.New(840, 129), Pos = Vector3.New(0, 5, 0)},
		content_3 = {Size = Vector2.New(840, 330), Pos = Vector3.New(0, -135, 0)},
	},
	[Common_PlayID.yiHuang_510K] = 
	{
		content_1 = 
		{
			Size = Vector2.New(840, 200), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, offset = Vector3.New(0, -60, 0), sortOrder = 1},
				ZhiFu_Node = { isShow = false, sortOrder = 2},
				FeiJi_Node = { isShow = true, sortOrder = 3},
			}
		},
	},
	[Common_PlayID.ThirtyTwo] = 
	{
		content_1 = {Size = Vector2.New(840, 194), Pos = Vector3.New(0, 217, 0),},
		content_2 = 
		{
			Size = Vector2.New(840, 68), Pos = Vector3.New(0, 14, 0),
			NodeConfig = 
			{
				ZhiFu_Node = { isShow = false, sortOrder = 1},
				TianDi9_Node = { isShow = true, sortOrder = 2},
			}
		},
	},
	[Common_PlayID.DW_DouDiZhu] = 
	{
		content_1 = 
		{
			Size = Vector2.New(840, 138), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, sortOrder = 1},
				ZhiFu_Node = { isShow = false, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(840, 353), Pos = Vector3.New(0, 66, 0)},
	},
	[Common_PlayID.ZhaJinHua] = 
	{
		content_1 = 
		{
			Size = Vector2.New(840, 138), Pos = Vector3.New(0, 217, 0),
			NodeConfig = 
			{
				JuShu_Node = { isShow = true, sortOrder = 1},
				ZhiFu_Node = { isShow = false, sortOrder = 3},
			}
		},
		content_2 = {Size = Vector2.New(840, 353), Pos = Vector3.New(0, 66, 0)},
		content_3 = {Size = Vector2.New(840, 110), Pos = Vector3.New(0, -297.5, 0)},
	}
}


-- 选项配置数据
_s.GameToggleConfig = 
{
	[Common_PlayID.chongRen_MJ] = 
	{
		JuShu_Node = { [1] = {label = "8局", subLabel = "(%d张房卡)", value = 8}, [2] = {label = "16局", subLabel = "(%d张房卡)", value = 16}},
		RenShu_Node = { [1] = {label = "4人", value = 4, offset = Vector3.New(10, 0, 0)}, [2] = {label = "3人", value = 3, offset = Vector3.New(10, 0, 0)}, [3] = {label = "2人", value = 2, offset = Vector3.New(10, 0, 0)}},
		ZhiFu_Node = { [1] = {label = "房主支付", value = 1}, [2] = {label = "平均支付", value = 2}},
		ShaoZhuang_Node = { [1] = {label = "不烧庄", value = false}, [2] = {label = "烧庄", value = true}},
		FengDing_Node = { [1] = {label = "封顶10炮", value = 10}, [2] = {label = "不封顶", value = 0}},
		DiHu_Node = { [1] = {label = "地胡庄赔10", value = 1}, [2] = {label = "地胡庄闲都赔10", value = 2}},
		ZuoMa_Node = { [1] = {label = "庄家坐马", value = true}, [2] = {label = "庄家不坐马", value = false}},
		DianPao_Node = { [1] = {label = "点炮可真胡无包赔", value = 1}, [2] = {label = "点炮可真胡有包赔", value = 2}},
	},
	[Common_PlayID.chongRen_510K] = 
	{
		JuShu_Node = { [1] = {label = "4局", subLabel = "(%d张房卡)", value = 4}, [2] = {label = "8局", subLabel = "(%d张房卡)", value = 8}, [3] = {label = "12局", subLabel = "(%d张房卡)", value = 12}},
		ZhiFu_Node = { [1] = {label = "房主支付", value = 1,}, [2] = {label = "平均支付", value = 2}},
		Mode_Node = { [1] = {label = "非全开", value = false }, [2] = {label = "全开", value = true}},
	},
	[Common_PlayID.leAn_MJ] = 
	{
		JuShu_Node = { [1] = {label = "8局", subLabel = "(%d张房卡)", value = 8}, [2] = {label = "16局", subLabel = "(%d张房卡)", value = 16}},
		RenShu_Node = { [1] = {label = "4人", value = 4, offset = Vector3.New(10, 0, 0)}, [2] = {label = "3人", value = 3, offset = Vector3.New(10, 0, 0)}, [3] = {label = "2人", value = 2, offset = Vector3.New(10, 0, 0)}},
		ZhiFu_Node = { [1] = {label = "房主支付", value = 1}, [2] = {label = "平均支付", value = 2}},
		ShaoZhuang_Node = { [1] = {label = "不烧庄", value = false}, [2] = {label = "烧庄", value = true}},
		FengDing_Node = { [1] = {label = "封顶20炮", value = 20, offset = Vector3.New(10, 0, 0)}, [2] = {label = "封顶10炮", value = 10, offset = Vector3.New(10, 0, 0)}, [3] = {label = "不封顶", value = 0, offset = Vector3.New(10, 0, 0)}},
		ZiMo_Node = { [1] = {label = "可点炮，可自摸", value = 0}, [2] = {label = "必须自摸", value = 1}},
		JiangMa_Node = { [1] = {label = "不奖马", value = 0}, [2] = {label = "奖1马", value = 1}, [3] = {label = "奖2马", value = 2}, [4] = {label = "奖3马", value = 3}},
	},
	[Common_PlayID.leAn_510K] = 
	{
		JuShu_Node = { [1] = {label = "4局", subLabel = "(%d张房卡)", value = 4}, [2] = {label = "8局", subLabel = "(%d张房卡)", value = 8}, [3] = {label = "12局", subLabel = "(%d张房卡)", value = 12}},
		ZhiFu_Node = { [1] = {label = "房主支付", value = 1,}, [2] = {label = "平均支付", value = 2}},
		Mode_Node = { [1] = {label = "非全开", value = false }, [2] = {label = "全开", value = true}},
	},
	[Common_PlayID.yiHuang_MJ] = 
	{
		JuShu_Node = { [1] = {label = "8局", subLabel = "(%d张房卡)", value = 8}, [2] = {label = "16局", subLabel = "(%d张房卡)", value = 16}},
		RenShu_Node = { [1] = {label = "4人", value = 4, offset = Vector3.New(10, 0, 0)}, [2] = {label = "3人", value = 3, offset = Vector3.New(10, 0, 0)}, [3] = {label = "2人", value = 2, offset = Vector3.New(10, 0, 0)}},
		ZhiFu_Node = { [1] = {label = "房主支付", value = 1}, [2] = {label = "平均支付", value = 2}},
		ShaoZhuang_Node = { [1] = {label = "不烧庄", value = false}, [2] = {label = "烧庄", value = true}},
		FengDing_Node = { [1] = {label = "封顶20炮", value = 20, offset = Vector3.New(10, 0, 0)}, [2] = {label = "封顶10炮", value = 10, offset = Vector3.New(10, 0, 0)}, [3] = {label = "不封顶", value = 0, offset = Vector3.New(10, 0, 0)}},
		DiHu_Node = { [1] = {label = "地胡庄10闲5", value = 1}, [2] = {label = "地胡庄闲都赔5", value = 2}},
		JiangMa_Node = { [1] = {label = "不奖马", value = 0}, [2] = {label = "奖2马", value = 2}, [3] = {label = "奖4马", value = 4}, [4] = {label = "奖6马", value = 6}},
		GangShangHua_Node = { [1] = {label = "算杠分", value = 1}, [2] = {label = "不算杠分", value = 2}},
		BangYiPao_Node = { [1] = {label = "买马不绑一炮", value = 0}, [2] = {label = "绑一炮", value = 1}, [3] = {label = "不绑炮", value = 2}},
	},
	[Common_PlayID.yiHuang_510K] = 
	{
		JuShu_Node = { [1] = {label = "4局", subLabel = "(%d张房卡)", value = 4}, [2] = {label = "8局", subLabel = "(%d张房卡)", value = 8}, [3] = {label = "12局", subLabel = "(%d张房卡)", value = 12}},
		ZhiFu_Node = { [1] = {label = "房主支付", value = 1,}, [2] = {label = "平均支付", value = 2}},
		FeiJi_Node = { [1] = {label = "多飞机", value = true }, [2] = {label = "2飞机", value = false}},
	},
	[Common_PlayID.ThirtyTwo] = 
	{
		JuShu_Node = { [1] = {label = "8局", subLabel = "(%d张房卡)", value = 8}, [2] = {label = "16局", subLabel = "(%d张房卡)", value = 16}, [3] = {label = "24局", subLabel = "(%d张房卡)", value = 24}},
		RenShu_Node = { [1] = {label = "4人", value = 4, offset = Vector3.New(10, 0, 0)}, [2] = {label = "3人", value = 3, offset = Vector3.New(10, 0, 0)}, [3] = {label = "2人", value = 2, offset = Vector3.New(10, 0, 0)}},
		ZhiFu_Node = { [1] = {label = "房主支付", value = 1,}, [2] = {label = "平均支付", value = 2}},
		TianDi9_Node = {[1] = {label = "天九王", value = 1, multiple = true}, [2] = {label = "地九王", value = 2, multiple = true}}
	},
	[Common_PlayID.DW_DouDiZhu] = 
	{
		JuShu_Node = {[1] = {label = "8局", subLabel = "(%d张房卡)", value = 8}, [2] = {label = "16局", subLabel = "(%d张房卡)", value = 16}, [3] = {label = "24局", subLabel = "(%d张房卡)", value = 24}},
		ZhiFu_Node = {[1] = {label = "房主支付", value = 1}, [2] = {label = "平均支付", value = 2}},
		JiaBei_Node = {[1] = {label = "加倍", value = true}, [2] = {label = "不加倍", value = false}},
		JiaoFen_Node = {[1] = {label = "必叫3分", value = true}, [2] = {label = "不叫3分", value = false}},
		FengDing_Node = {[1] = {label = "不封顶", value = 0, offset = Vector3.New(15, 0, 0)}, [2] = {label = "3炸", value = 3, offset = Vector3.New(15, 0, 0)}, [3] = {label = "4炸", value = 4, offset = Vector3.New(15, 0, 0)}, [4] = {label = "5炸", value = 5, offset = Vector3.New(15, 0, 0)}, [5] = {label = "6炸", value = 6}, offset = Vector3.New(15, 0, 0)},
	},
	[Common_PlayID.ZhaJinHua] = 
	{
		JuShu_Node = {[1] = {label = "8局", subLabel = "(%d张房卡)", value = 4}, [2] = {label = "16局", subLabel = "(%d张房卡)", value = 8}, [3] = {label = "24局", subLabel = "(%d张房卡)", value = 12}},
		ZhiFu_Node = {[1] = {label = "房主支付", value = 1}, [2] = {label = "平均支付", value = 2}},
		JiaBei_Node = {[1] = {label = "加倍", value = true}, [2] = {label = "不加倍", value = false}},
		JiaoFen_Node = {[1] = {label = "必叫3分", value = true}, [2] = {label = "不叫3分", value = false}},
		FengDing_Node = {[1] = {label = "不封顶", value = 0, offset = Vector3.New(15, 0, 0)}, [2] = {label = "3炸", value = 3, offset = Vector3.New(15, 0, 0)}, [3] = {label = "4炸", value = 4, offset = Vector3.New(15, 0, 0)}, [4] = {label = "5炸", value = 5, offset = Vector3.New(15, 0, 0)}, [5] = {label = "6炸", value = 6}, offset = Vector3.New(15, 0, 0)},
	},
}

-- 默认配置
_s.GameToggleDefValue = 
{
	--[[
		使用位运算
		0, 1, 2, 4, 8, ...
	--]]
	[Common_PlayID.chongRen_MJ] = 
	{
		JuShu_Node = 1,
		RenShu_Node = 1,
		ZhiFu_Node = 1,
		ShaoZhuang_Node = 2,
		FengDing_Node = 1,
		DiHu_Node = 2,
		ZuoMa_Node = 1,
		DianPao_Node = 2,
	},
	[Common_PlayID.chongRen_510K] = 
	{
		JuShu_Node = 1,
		ZhiFu_Node = 1,
		Mode_Node = 2,
	},
	[Common_PlayID.leAn_MJ] = 
	{
		JuShu_Node = 1,
		RenShu_Node = 1,
		ZhiFu_Node = 1,
		ShaoZhuang_Node = 2,
		FengDing_Node = 1,
		ZiMo_Node = 1,
		JiangMa_Node = 8,
	},
	[Common_PlayID.leAn_510K] = 
	{
		JuShu_Node = 1,
		ZhiFu_Node = 1,
		Mode_Node = 2,
	},
	[Common_PlayID.yiHuang_MJ] = 
	{
		JuShu_Node = 1,
		RenShu_Node = 1,
		ZhiFu_Node = 1,
		ShaoZhuang_Node = 2,
		FengDing_Node = 1,
		DiHu_Node = 2,
		JiangMa_Node = 4,
		GangShangHua_Node = 1,
		BangYiPao_Node = 2,
	},
	[Common_PlayID.yiHuang_510K] = 
	{
		JuShu_Node = 1,
		ZhiFu_Node = 1,
		FeiJi_Node = 2,
	},
	[Common_PlayID.ThirtyTwo] = 
	{
		JuShu_Node = 1,
		RenShu_Node = 1,
		ZhiFu_Node = 1,
		TianDi9_Node = 0,
	},
	[Common_PlayID.DW_DouDiZhu] =
	{
		JuShu_Node = 1,
		ZhiFu_Node = 1,
		JiaBei_Node = 1,
		JiaoFen_Node = 1,
		FengDing_Node = 1,
	},
	[Common_PlayID.ZhaJinHua] =
	{
		JuShu_Node = 1,
		ZhiFu_Node = 1,
		JiaBei_Node = 1,
		JiaoFen_Node = 1,
		FengDing_Node = 1,
	},
}

-- 对应服务器字段
_s.GameCreateConfig = 
{
	[Common_PlayID.chongRen_MJ] = 
	{
		JuShu_Node = "gameNum",
		RenShu_Node = "playerNum",
		ZhiFu_Node = "fangkaType",
		ShaoZhuang_Node = "isShaoZhuang",
		FengDing_Node = "fengdingNum",
		DiHu_Node = "dihuStatus",
		ZuoMa_Node = "zhuangJiaZuoMa",
		DianPao_Node = "dianPaoZhenQing",
	},
	[Common_PlayID.chongRen_510K] = 
	{
		JuShu_Node = "games",
		ZhiFu_Node = "payType",
		Mode_Node = "isAllOpen",
	},
	[Common_PlayID.leAn_MJ] = 
	{
		JuShu_Node = "gameNum",
		RenShu_Node = "playerNum",
		ZhiFu_Node = "fangkaType",
		ShaoZhuang_Node = "shaoZhuang",
		FengDing_Node = "fengdingNum",
		ZiMo_Node = "huType",
		JiangMa_Node = "jiangMa",
	},
	[Common_PlayID.leAn_510K] = 
	{
		JuShu_Node = "games",
		ZhiFu_Node = "payType",
		Mode_Node = "isAllOpen",
	},
	[Common_PlayID.yiHuang_MJ] = 
	{
		JuShu_Node = "gameNum",
		RenShu_Node = "playerNum",
		ZhiFu_Node = "fangkaType",
		ShaoZhuang_Node = "shaoZhuang",
		FengDing_Node = "fengdingNum",
		DiHu_Node = "dihuStatus",
		JiangMa_Node = "jiangMa",
		GangShangHua_Node = "gangHu",
		BangYiPao_Node = "bangPao",
	},
	[Common_PlayID.yiHuang_510K] = 
	{
		JuShu_Node = "games",
		ZhiFu_Node = "payType",
		FeiJi_Node = "canPlanes",
	},
	[Common_PlayID.ThirtyTwo] = 
	{
		JuShu_Node = "games",
		RenShu_Node = "playerNum",
		ZhiFu_Node = "payType",
		TianDi9_Node = "point9Type",
	},
	[Common_PlayID.DW_DouDiZhu] = 
	{
		JuShu_Node = "games",
		ZhiFu_Node = "payType",
		JiaBei_Node = "canDouble",
		JiaoFen_Node = "isJackerMax",
		FengDing_Node = "bombLimit",
	},
	[Common_PlayID.ZhaJinHua] = 
	{
		JuShu_Node = "games",
		ZhiFu_Node = "payType",
		JiaBei_Node = "canDouble",
		JiaoFen_Node = "isJackerMax",
		FengDing_Node = "bombLimit",
	},
}