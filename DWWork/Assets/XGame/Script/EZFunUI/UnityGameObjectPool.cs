/************************************************************
//     文件名      : UnityGameObjectPool.cs
//     功能描述    : 用于回收Unity内GameObject对象（移除窗口外等待下一次复用，切换场景后移除）
//     负责人      : lezen
//     参考文档    : 无
//     创建日期    : 2017/06/20.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class UnityGameObjectPool
{
    private Dictionary<string, Stack<GameObject>> m_objStack = new Dictionary<string, Stack<GameObject>>();
    private int m_maxCapacity = 100;
    private string tagName = "No_Use_GameObject";
    private Transform _m_parentTrans = null;
    //NGUI里一个节点的父节点置null或者新建时未设置父节点时，本身或者子节点有uiwidet时，必须父节点中有一个uipanel，不然会自动去找一个有uipanel的节点挂载上去
    //解决方案：指定一个父节点，添加一个uipanel
    private Transform ParentTrans
    {
        get
        {
            if (_m_parentTrans == null)
            {
                GameObject gb = GameObject.Find("No_Use_GameObject_Root");
                if (gb == null)
                {
                    gb = new GameObject("No_Use_GameObject_Root");
                    gb.transform.position = Vector3.zero;
                    EZFunTools.GetOrAddComponent<UIPanel>(gb);
                    gb.transform.parent = null;
                }
                _m_parentTrans = gb.transform;
            }
            return _m_parentTrans;
        }
    }

    public UnityGameObjectPool(int maxCapacity)
    {
        m_maxCapacity = maxCapacity;
    }

    public GameObject PopObject(string name)
    {
        if (m_objStack.ContainsKey(name))
        {
            Stack<GameObject> t_stack = m_objStack[name];
            if (t_stack.Count > 0)
            {
                return t_stack.Pop();
            }
        }
        return null;
    }

    public bool PushObject(string name,GameObject p_obj)
    {
        if (p_obj == null)
        {
            return false;
        }
        if (!m_objStack.ContainsKey(name))
        {
            Stack<GameObject> t_stack = new Stack<GameObject>();
            m_objStack.Add(name, t_stack);
        }
        if (m_objStack[name].Count <= m_maxCapacity)
        {
            p_obj.name = tagName;
            p_obj.transform.position = new Vector3(100000, 100000, 100000);
            p_obj.transform.parent = ParentTrans;
            m_objStack[name].Push(p_obj);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        m_objStack.Clear();
        if (_m_parentTrans != null)
        {
            MonoBehaviour.Destroy(_m_parentTrans.gameObject);
            _m_parentTrans = null;
        }
    }
}

