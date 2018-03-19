/************************************************************
//     文件名      : DLLUpdateExecutor.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 03/15/2018
//     Copyright  : 
**************************************************************/

using UnityEngine;
using System.Collections;
using System.IO;

public class DLLUpdateExecutor : ParentUpdateExecutor
{
	#region implemented abstract members of BaseUpdateExecutor
	public override string GetUpdateType ()
	{
		return "DLL";
	}

    protected override bool IsNeedReloadDLL()
    {
        return true;
    }
    #endregion





}
