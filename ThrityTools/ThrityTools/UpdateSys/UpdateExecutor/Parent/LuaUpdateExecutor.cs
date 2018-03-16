/************************************************************
//     文件名      : LuaUpdateExecutor.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 03/15/2018
//     Copyright  : 
**************************************************************/

using UnityEngine;
using System.Collections;
using System.IO;

public class LuaUpdateExecutor : ParentUpdateExecutor {
	#region implemented abstract members of BaseUpdateExecutor
	public override string GetUpdateType ()
	{
		return "Lua";
	}
	#endregion
	




}
