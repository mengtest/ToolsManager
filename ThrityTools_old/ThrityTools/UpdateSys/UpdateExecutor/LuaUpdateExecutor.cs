/************************************************************
//     文件名      : TableUpdateExecutor.cs
//     功能描述    : 
//     负责人      : cai yang
//     参考文档    : 无
//     创建日期    : 05/14/2017
//     Copyright  : Copyright 2017-2018 EZFun.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.IO;

public class LuaUpdateExecutor : BaseUpdateExecutor {
	#region implemented abstract members of BaseUpdateExecutor
	public override string GetUpdateType ()
	{
		return "Lua";
	}
	#endregion
	




}
