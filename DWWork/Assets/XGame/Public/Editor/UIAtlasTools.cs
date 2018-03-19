/************************************************************
//     文件名      : UIAtlasTools.cs
//     功能描述    : 给ui将UIAtlas的小碎图全部导出
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-11-19 17:33:08.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class UIAtlasTools
{

    [MenuItem("EZFun/UI/图集导出Sprite")]
    public static void ExportAtlasPNG()
    {
        var mAtlas = Selection.activeGameObject.GetComponent<UIAtlas>();
        string path = EditorUtility.SaveFolderPanel("Save Atlas PNGs", NGUISettings.currentPath, "");
        var pngs = mAtlas.spriteList;
        if (!Directory.Exists(path + "/" + mAtlas.name))
        {
            Directory.CreateDirectory(path + "/" + mAtlas.name);
        }
        for (int i = 0; i < pngs.Count; i++)
        {
            UIAtlasMaker.SpriteEntry se = UIAtlasMaker.ExtractSprite(mAtlas, pngs[i].name);
            if (se != null)
            {
                byte[] bytes = se.tex.EncodeToPNG();
                File.WriteAllBytes(path + "/" + mAtlas.name + "/" + pngs[i].name + ".png", bytes);
                if (se.temporaryTexture) GameObject.DestroyImmediate(se.tex);
            }
        }
    }

}
