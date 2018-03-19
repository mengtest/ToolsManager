/************************************************************
//     文件名      : OpenHighQualityInEditor.cs
//     功能描述    : 
//     负责人      : Administrator   Administrator@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-06-28 15:27:40.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
public class OpenHighQualityInEditor : MonoBehaviour {

    [MenuItem("EZFun/编辑器环境开启高画质")]
    public static void OpenHighQualityInEditorFunc()
    {
        Shader.EnableKeyword("_SceneQuality_Medium");
        Shader.EnableKeyword("_SceneQuality_High");
    }
}
