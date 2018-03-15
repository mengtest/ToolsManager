/************************************************************
     File      : EZFunButtonGroup.cs
     brief     :   
     author    : JanusLiu   janusliu@ezfun.cn
     version   : 1.0
     date      : 2014/11/4 15:14:47
     copyright : Copyright 2014 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EZFunButtonGroupItem
{
	public Transform m_selectTrans;
	public Transform m_unselectTrans;

	public EZFunButtonGroupItem(Transform selectTrans, Transform unselectTrans)
	{
		m_selectTrans = selectTrans;
		m_unselectTrans = unselectTrans;
	}
}

[LuaWrap]
public class EZFunButtonGroup {

	private Dictionary<Transform, EZFunButtonGroupItem> m_buttonGroupDic = new Dictionary<Transform, EZFunButtonGroupItem>();
	private Transform m_lastClickButton = null;
		
	public EZFunButtonGroup(Transform rootTrans, bool autoAddChild, bool includDisable = false)
	{
		if(rootTrans == null)
		{
			Debug.LogError("[RootTrans is Null]");
			return;
		}

		m_buttonGroupDic.Clear();
		m_lastClickButton = null;

		if(autoAddChild)
		{
			for(int childIndex = 0; childIndex < rootTrans.childCount; childIndex ++)
			{
				Transform childTrans = rootTrans.GetChild(childIndex);

				if(!includDisable && !childTrans.gameObject.activeSelf)
				{
					continue;
				}

				Transform selectTrans = null;
				Transform unSelectTrans = null;

				for(int index = 0; index < childTrans.childCount; index ++)
				{
					Transform trans = childTrans.GetChild(index);
					if(trans.name.Contains("unSelect") || trans.name.Contains("unselect"))
					{
						unSelectTrans = trans;
					}
					else if(trans.name.Contains("select"))
					{
						selectTrans = trans;
					}
				}

				if(selectTrans != null && unSelectTrans != null)
				{
					if(m_buttonGroupDic.ContainsKey(childTrans))
					{
						m_buttonGroupDic.Remove(childTrans);
					}
					m_buttonGroupDic.Add(childTrans, new EZFunButtonGroupItem(selectTrans, unSelectTrans));
				}
			}
		}
	}

	public EZFunButtonGroup()
	{
		m_buttonGroupDic.Clear();
		m_lastClickButton = null;
	}


	public void AddItem(Transform trans)
	{
		Transform selectTrans = null;
		Transform unSelectTrans = null;
		
		for(int index = 0; index < trans.childCount; index ++)
		{
			Transform childTrans = trans.GetChild(index);
			if(childTrans.name.Contains("unSelect") || childTrans.name.Contains("unselect"))
			{
				unSelectTrans = trans;
			}
			else if(childTrans.name.Contains("select"))
			{
				selectTrans = trans;
			}
		}

		if(selectTrans == null || unSelectTrans == null)
		{
			Debug.LogError("[SelectTrans Or UnselectTrans is null]");
		}
		else
		{
			if(m_buttonGroupDic.ContainsKey(trans))
			{
				m_buttonGroupDic.Remove(trans);
			}
			m_buttonGroupDic.Add(trans, new EZFunButtonGroupItem(selectTrans, unSelectTrans));
		}
	}

	public void AddItem(Transform selectTrans, Transform unselectTrans)
	{
		if(selectTrans == null || unselectTrans == null)
		{
			Debug.LogError("[SelectTrans Or UnselectTrans is null]");
		}
		else if(selectTrans.parent == null || unselectTrans.parent == null || selectTrans.parent != unselectTrans.parent)
		{
			Debug.LogError("[SelectTrans and UnselectTrans 's parent is invaild]");
		}
		else
		{
			if(m_buttonGroupDic.ContainsKey(selectTrans.parent))
			{
				m_buttonGroupDic.Remove(selectTrans.parent);
			}
			else
			{
				m_buttonGroupDic.Add(selectTrans.parent, new EZFunButtonGroupItem(selectTrans, unselectTrans));
			}
		}
	}
	
	public bool ClickButton(Transform trans, bool forceClick = false)
	{
		bool click = false;

		if(forceClick)
		{
			click = true;
		}

		if(trans != m_lastClickButton || forceClick)
		{
			click = true;
			if(trans != null)
			{
				SetButton(trans, true);
			}
            var enm = m_buttonGroupDic.GetEnumerator();
            while(enm.MoveNext())
			{
				if(enm.Current.Key == trans)
				{
					continue;
				}
				SetButton(enm.Current.Key, false);
			}

			m_lastClickButton = trans;
		}

		return click;
	}

	public Transform GetLastClick()
	{
		return m_lastClickButton;
	}

	void SetButton(Transform trans, bool select)
	{
        if (!m_buttonGroupDic.ContainsKey(trans))
        {
            return;
        }
		EZFunButtonGroupItem Item = m_buttonGroupDic[trans];

		NGUITools.SetActive(Item.m_selectTrans.gameObject, select);
        NGUITools.SetActive(Item.m_unselectTrans.gameObject, !select);
	}	
}
