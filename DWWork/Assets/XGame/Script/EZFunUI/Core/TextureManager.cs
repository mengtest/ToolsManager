/************************************************************
     File      : TextureManager.cs
     author    : blackzhou blackzhou@ezfun.cn
     version   : 1.0
     date      : 2017-05-31 16:25:16.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections.Generic;
using System;

public class TextureManager
{
    private Dictionary<string, string> m_UITextureDic = new Dictionary<string, string>();
    private Dictionary<string, string> m_UIAtlasDic = new Dictionary<string, string>();
    private Dictionary<string, UIAtlas> m_UIAtlas = new Dictionary<string, UIAtlas>();
    public Texture DefaultIconTexture = null;
    private static TextureManager m_Instance = null;

    public static TextureManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new TextureManager();
                m_Instance.Init();
            }
            return m_Instance;
        }
    }

    private void Init()
    {
        //打包之前都会去生成一个碎图的文件列表
        var textureTrans = ResourceManager.Instance.LoadResource("Assets/XGame/Resources/EZFunUI/Altas/textureList.prefab", typeof(GameObject)) as GameObject;
        if (textureTrans != null)
        {
            var textureList = textureTrans.GetComponent<UITextureList>();
            if (textureList != null)
            {
                var sprites = textureList.m_sprites;
                var spritePaths = textureList.m_spritePath;
                for (int i = 0; i < sprites.Count; i++)
                {
                    if (!m_UITextureDic.ContainsKey(sprites[i]))
                    {
                        m_UITextureDic.Add(sprites[i], spritePaths[i].ToLower());
                    }
                }

                var atlas = textureList.m_atlas;
                var atlasPath = textureList.m_atlasPath;
                for (int i = 0; i < atlas.Count; ++i)
                {
                    if (!m_UIAtlasDic.ContainsKey(atlas[i]))
                    {
                        m_UIAtlasDic.Add(atlas[i], atlasPath[i]);
                    }
                }
            }
        }
        DefaultIconTexture = ResourceManager.Instance.LoadResource("Assets/XGame/Resources/EZFunUI/Texture/Icons/icon_common_default.png", typeof(Texture)) as Texture;

        NGUITools.ClearDynamicUIDic();

        NGUITools.mGetFontCB = GetFont;

        NGUITools.mGetUIFontCB = GetUIFont;

        NGUITools.mGetAtalsCB = GetUIAtlas;

        NGUITools.mGetTextureCB = LoadTexture;
    }

    public Texture LoadTexture(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        string path = null;
        Texture tex = null;
        if (m_UITextureDic.TryGetValue(name, out path))
        {
            tex = ResourceManager.Instance.LoadResource(path, typeof(Texture)) as Texture;
        }
        return tex;
    }

    public UIAtlas LoadUIAtlas(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        string path = null;
        UIAtlas atlas = null;
        if (m_UIAtlasDic.TryGetValue(name, out path))
        {
            GameObject ob = ResourceManager.Instance.LoadResource(path, typeof(GameObject)) as GameObject;
            if (ob != null)
            {
                atlas = ob.GetComponent<UIAtlas>();
            }
        }
        return atlas;
    }

    public UIAtlas GetUIAtlas(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        UIAtlas atlas = null;
        if (m_UIAtlas.TryGetValue(name, out atlas))
        {
            return atlas;
        }
        atlas = LoadUIAtlas(name);
        if (atlas != null)
        {
            m_UIAtlas.Add(name, atlas);
        }
        return atlas;
    }

    public void LoadAtlasToSprite(UISprite uisprice, string spriteName)
    {
        var iter = m_UIAtlasDic.GetEnumerator();
        while(iter.MoveNext())
        {
            UIAtlas atlas = GetUIAtlas(iter.Current.Key);
            if (atlas == null)
                continue;
            if (atlas.GetSprite(spriteName) != null)
            {
                uisprice.atlas = atlas;
                return;
            }
        }
    }

    public UIAtlas FindAtlasOwnsSprite(string spriteName)
    {
        var iter = m_UIAtlasDic.GetEnumerator();
        while (iter.MoveNext())
        {
            UIAtlas atlas = GetUIAtlas(iter.Current.Key);
            if (atlas == null)
                continue;
            if (atlas.GetSprite(spriteName) != null)
            {
                return atlas;
            }
        }
        return null;
    }

    //先看有没有更新,如果没有更新,直接从本地读
    public Font GetFont(string name)
    {
        object ob = null;
        if (ResourceManager.Instance != null)
        {
            CommonStringBuilder.Clear();
            CommonStringBuilder.AppendFormat("Assets/XGame/Resources/EZFunUI/Font/{0}.ttf", name);
            ob = ResourceManager.Instance.LoadResource(CommonStringBuilder.GetString(), typeof(GameObject));
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
            CommonStringBuilder.Clear();
            CommonStringBuilder.AppendFormat("Assets/XGame/Resources/EZFunUI/Font/{0}.prefab", name);
            ob = ResourceManager.Instance.LoadResource(CommonStringBuilder.GetString(), typeof(GameObject));
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
}
