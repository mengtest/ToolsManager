/************************************************************
     File      : UITab.cs
     brief     :   
     author    : Jason   Jason@ezfun.cn
     version   : 1.0
     date      : 2014/11/3 16:19:54
     copyright : Copyright 2014 EZFun Inc.
**************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//prefab tree
//tabroot
//	btn                //has a component as UIButtonMessage
//		high_light     //showed when active
//	tab
//
//override your own HandleWidgetClick, eg: HandleVGWindow.cs

public class EZFunTab
{
	public int m_actIndex = 1;
	public  Dictionary<Transform, Transform> m_tabList = new Dictionary<Transform, Transform>();
	private Transform m_actBtn = null;

	protected Transform GetTrans(Transform parent, string name)
	{
		return EZFunUITools.GetTrans(parent, name);
	}

	protected void SetActive(Transform trans, bool state)
	{
        NGUITools.SetActive(trans.gameObject, state, false);
	}

	public void AddTab(Transform trans)
	{
		Transform btn = GetTrans(trans, "btn");
		Transform tab = GetTrans(trans, "tab");

		if (m_tabList.ContainsKey(btn))
		{
			return;
		}
		else
		{
			m_tabList.Add(btn, tab);

            if (m_actBtn == null)
			{
				m_actBtn = btn;
				ShowTheObj(btn);
			}
			else
			{
				SetActive(GetTrans(btn, "high_light"), false);
				SetActive(tab, false);
            }
		}

		if (btn.transform.GetComponent<UIButtonMessage>() == null)
		{
			Debug.LogError("Every btn in UITab should have a component as UIButtonMessage");
		}
	}

	void ShowTheObj(Transform trans)
	{
        var enm = m_tabList.GetEnumerator();
        while(enm.MoveNext())
		{
			if (enm.Current.Key == trans)
			{
				SetActive(GetTrans(enm.Current.Key, "high_light"), true);
				SetActive(enm.Current.Value, true);
			}
			else if (enm.Current.Value.gameObject.activeSelf)
			{
				SetActive(GetTrans(enm.Current.Key, "high_light"), false);
				SetActive(enm.Current.Value, false);
			}
		}
	}

	public virtual void HandleWidgetClick(GameObject gb)
	{
		Transform trans = gb.transform;
		
		if (trans == m_actBtn)
		{
			return;
		}
		
		foreach (var kv in m_tabList)
		{
			if (kv.Key == trans)
			{
				ShowTheObj(kv.Key);
				m_actBtn = kv.Key;
				SetActIndex();
			}
		}

	}

	void SetActIndex()
	{
		int i = 1;

		foreach (var kv in m_tabList)
		{
			if (kv.Key == m_actBtn)
			{
				m_actIndex = i;
			}
			else 
			{
				i++;
			}
		}
	}
}
