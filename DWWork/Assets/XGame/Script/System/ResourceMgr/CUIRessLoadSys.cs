// /************************************************************
//     File      : CUIRessLoadSys.cs
//     brief     : UI动态更新,主要包括更新下来的UI自动找到本地的字体,图集和更新图集相关功能
//     author    : JanusLiu   janusliu@ezfun.cn
//     version   : 1.0
//     date      : 2015/6/23 20:19:6
//     copyright : Copyright 2014 EZFun Inc.
// **************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CUIRessLoadSys
{
    static CUIRessLoadSys _Instance = null;
    public static CUIRessLoadSys Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new CUIRessLoadSys();
            }
            return _Instance;
        }
    }

    public void Init()
    {
        NGUITools.ClearDynamicUIDic();

        NGUITools.mGetFontCB = GetFont;

        NGUITools.mGetUIFontCB = GetUIFont;

        NGUITools.mGetAtalsCB = GetUIAtlas;

        NGUITools.mGetTextureCB = GetUITexture;

        // Mingrui Jiang cgg
        // texture的文件夹很多，检查是不是都加上了!!!
        // 这个在ios上用不了，只是为了防止出错，所有的文件夹名字还是得手动填写
        //var hash = new HashSet<string>(textureFolders);
        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    DirectoryInfo resourcesInfo = new DirectoryInfo(Application.dataPath + "/XGame/Resources/EZFunUI/Texture/BgImg");
        //    DirectoryInfo[] textureSubdirectories = resourcesInfo.GetDirectories();
        //    var e = from s in textureSubdirectories select s;
        //    foreach (DirectoryInfo di in e)
        //    {
        //        if (!hash.Contains(di.Name))
        //        {
        //            Debug.Log(string.Format("<color=yellow>IMPORTANT missing texture folder: {0}</color>", di.Name));
        //            Debug.Log(string.Format("<color=yellow>IMPORTANT missing texture folder: {0}</color>", di.Name));
        //        }
        //    }
        //}
    }

    //先看有没有更新,如果没有更新,直接从本地读
    public Font GetFont(string name)
    {
        object ob = null;
        if (ResourceManager.Instance != null)
        {
            ob = ResourceManager.Instance.LoadResource("Assets/XGame/Resources/EZFunUI/Font/" + name + ".ttf", typeof(GameObject));
        }

        if (ob == null)
        {
            ob = Resources.Load("EZFunUI/Font/" + name);
        }

        if (ob != null)
        {
            return ob as Font;
        }
        else
        {
            return null;
        }
    }

    //先看有没有更新,如果没有更新,直接从本地读
    public UIFont GetUIFont(string name)
    {
        object ob = null;
        if (ResourceManager.Instance != null)
        {
            ob = ResourceManager.Instance.LoadResource("Assets/XGame/Resources/EZFunUI/Font/" + name + ".prefab", typeof(GameObject));
        }

        if (ob == null)
        {
            ob = Resources.Load("EZFunUI/Font/" + name);
        }

        if (ob != null)
        {
            UIFont font = (ob as GameObject).GetComponent<UIFont>();
            return font;
        }
        else
        {
            return null;
        }
    }

    public UIAtlas GetUIAtlas(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        return TextureManager.Instance.GetUIAtlas(name);
    }

    public Texture GetUITexture(string name)
    {
        //Debug.LogError("GetUITexture:" + name);
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        return TextureManager.Instance.LoadTexture(name);
    }

    //保存需要更新的引用图集列表
    List<string> m_needUpdateAtlasRefNameLs = new List<string>();
    public void AddNeedUpdateAtlasName(string name)
    {
        if (!m_needUpdateAtlasRefNameLs.Contains(name))
        {
            m_needUpdateAtlasRefNameLs.Add(name);
        }
    }

    public void CheckNeedUpdateAtlas()
    {
        for (int i = 0; i < m_needUpdateAtlasRefNameLs.Count; i++)
        {
            string refName = m_needUpdateAtlasRefNameLs[i];
            Object ob = null;
            if (ResourceManager.Instance != null)
            {
                ob = ResourceManager.Instance.LoadResource(refName, typeof(GameObject));
            }
            //更新Assetsbundle中没有找到更新的图集
            if (ob == null)
            {
                continue;
            }

            GameObject gb = ob as GameObject;
            UIAtlas refAtlas = gb.GetComponent<UIAtlas>();
            //更新Assetsbundle中没有找到更新的图集
            if (refAtlas == null)
            {
                continue;
            }

            string atlasName = refAtlas.spriteMaterial.name;
            UIAtlas atlas = GetUIAtlas(atlasName);
            //没有找到原图集
            if (atlas == null)
            {
                continue;
            }

            atlas.replacement = refAtlas;
        }
    }
}
