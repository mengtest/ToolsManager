//========================================================================
// Copyright(C): EZFun
//
// CLR Version : 4.0.30319.42000
// Machinename : USER-20160222CX
// NameSpace   : Assets.XGame.Script.Util
// FileName    : EZFunString
//
// Created by  : dhf at 2016/7/5 16:34:57
//
// Function    : 
//
/*
使用方法：
1.申请string
var strID = EZFunString.Alloc();
2.使用string
EZFunString.Append(strID, clear, params...);
3.释放string
EZFunString.Free(strID);
*/
//========================================================================


using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class EZFunStringStruct
{
    public StringBuilder m_sb;
    public bool m_inUse;
    public EZFunStringStruct()
    {
        m_sb = new StringBuilder();
        m_inUse = true;
    }
}
class EZFunString
{
    private static Dictionary<int, EZFunStringStruct> m_sbs = new Dictionary<int, EZFunStringStruct>();
    private static int m_key = 0;

    public static int Alloc(bool clear = false)
    {
        int allocKey = -1;
        var enumerator = m_sbs.GetEnumerator();
        while(enumerator.MoveNext())
        {
            var key = enumerator.Current.Key;
            var value = enumerator.Current.Value;
            if(value != null &&
            value.m_inUse == false)
            {
                allocKey = key;
                value.m_inUse = true;
                if(clear)
                {
                    value.m_sb.Remove(0, value.m_sb.Length);
                }
                break;
            }
        }
        if (allocKey == -1)
        {
            var sb = new EZFunStringStruct();
            m_sbs.Add(m_key, sb);
            allocKey = m_key;
            m_key++;
        }
        return allocKey;
    }

    public static void Free(int key)
    {
        EZFunStringStruct sb;
        if(!m_sbs.TryGetValue(key, out sb))
        {
            return;
        }
        sb.m_inUse = false;
    }

    public static string GetString(int key)
    {
        EZFunStringStruct sb;
        if(m_sbs.TryGetValue(key, out sb))
        {
            return sb.m_sb.ToString();
        }
        return "";
    }

    /// <summary>
    /// 拼字符串
    /// </summary>
    /// <param name="key">用于索引string</param>
    /// <param name="clear">使用string前是否要清空string的内容</param>
    /// <param name="objs">需要拼的字符容器</param>
    /// <returns></returns>
    public static string Append(int key, bool clear, params object[] objs)
    {
        if (m_sbs == null)
        {
            return "";
        }
        EZFunStringStruct sb;
        if (!m_sbs.TryGetValue(key, out sb))
        {
            return "";
        }
        if(clear)
        {
            sb.m_sb.Remove(0, sb.m_sb.Length);
        }
        for (int i = 0; i < objs.Length; ++i)
        {
            sb.m_sb.Append(objs[i]);
        }
        return sb.m_sb.ToString();
    }

    public static string LinkString(params object[] objects)
    {
        var strID = Alloc();
        Append(strID, true, objects);
        Free(strID);
        return GetString(strID);
    }

    public static string CommonStringBuilderString(params string[] objects)
    {
        CommonStringBuilder.Clear();
        for (int i = 0; i < objects.Length; ++i)
        {
            CommonStringBuilder.Append(objects[i]);
        }
        return CommonStringBuilder.GetString();
    }
}
