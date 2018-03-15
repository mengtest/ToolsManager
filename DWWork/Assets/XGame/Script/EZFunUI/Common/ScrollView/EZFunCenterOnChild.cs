/************************************************************
//     文件名      : EZFunCenterOnChild.cs
//     功能描述    : 这个剧中是点哪儿，哪儿居中，不考虑上下或者左右有空白
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-11-08 17:07:27.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

public class EZFunCenterOnChild : UICenterOnChild {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnEnable() { mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject); }

    void OnDisable() { }
}
