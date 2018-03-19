/************************************************************
     File      : EZFunScollViewNormal.cs
     brief     : 横向列表，功能半完善，已测单行可行  
     author    : JanusLiu   janusliu@ezfun.cn
     version   : 1.0
     date      : 2014/11/3 15:48:52
     copyright : Copyright 2014 EZFun Inc.
**************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;


public class EZFunScollViewNormal
{

    //params
    
    Transform m_DragTrans = null;
    Transform m_ScrollTrans = null;
    Transform m_UIWrapTrans = null;
    List<Transform> m_objList = new List<Transform>();
    GameObject m_fillobj = null;
    int m_uiLayer = 0;
    static int m_NameIndex = 0;
    string m_name;
    Vector3 m_itemVec = Vector3.zero;
    ScrollViewDirection m_direction = ScrollViewDirection.SVD_Horizontal;

    public EZFunScollViewNormal(Transform DragTrans,Vector3 shift,GameObject fillObj,ScrollViewDirection direction,UpdateItemCallBack callBack = null,SpringPanel.OnFinished finishedCallback = null)
    {
        m_uiLayer = DragTrans.gameObject.layer;
        m_DragTrans = DragTrans;
        m_itemVec = shift;
        m_direction = direction;
        UIDragScrollView dragscroll = EZFunTools.GetOrAddComponent<UIDragScrollView>(DragTrans.gameObject); 

        GameObject gb = new GameObject();
        gb.name = "Scroll View"+ m_NameIndex;
        m_name = gb.name;
        m_NameIndex++;
        gb.transform.parent = m_DragTrans;
        gb.transform.localPosition = Vector3.zero;
        gb.transform.localScale = new Vector3(1, 1, 1);
        m_ScrollTrans = gb.transform;
        UIPanel panel = gb.AddComponent<UIPanel>();
        panel.baseClipRegion = new Vector4( 0,0,m_DragTrans.GetComponent<BoxCollider>().size.x,m_DragTrans.GetComponent<BoxCollider>().size.y );
        panel.depth = 100;
        panel.clipping = UIDrawCall.Clipping.SoftClip; 

        UIScrollView scrollview = gb.AddComponent<UIScrollView>();
        SpringPosition sp= gb.AddComponent<SpringPosition>();
        scrollview.restrictWithinPanel = true;
        dragscroll.scrollView = scrollview;
        sp.strength = 13;
        GameObject gbwrap = new GameObject();
        gbwrap.name = "UIWrap Content";
        gbwrap.transform.parent = m_ScrollTrans;
        gbwrap.transform.localPosition = Vector3.zero;
        gbwrap.transform.localScale = new Vector3(1, 1, 1);
        m_UIWrapTrans = gbwrap.transform;
        UIWrapContent wrapContent = gbwrap.AddComponent<UIWrapContent>();
        if(direction == ScrollViewDirection.SVD_Horizontal)
        {
            wrapContent.itemSize = (int)shift.x;
            panel.clipSoftness = new Vector2(200.0f, 0);
        }
        else
        {
            wrapContent.itemSize = (int)shift.y;
            panel.clipSoftness = new Vector2(0, 200.0f);
        }
        wrapContent.m_UpdateItemCallBack = callBack;
        
        UICenterOnChild centerchild = gbwrap.AddComponent<UICenterOnChild>();
        centerchild.enabled = false;
        centerchild.enabled = true;
        centerchild.onFinished = finishedCallback;
        centerchild.springStrength = 13;
        m_fillobj = fillObj;
        
    }

   public  List<Transform> GetScrollView(int count)
    {
        int i = 0;
        if (count > m_objList.Count)
        {
            count -= m_objList.Count;
            for (i = 0; i < count; i++)
            {
                GameObject gb = (GameObject)MonoBehaviour.Instantiate(m_fillobj);
                gb.name = m_fillobj.name + i;
                gb.transform.parent = m_UIWrapTrans;
                gb.transform.localPosition = Vector3.zero;
                gb.transform.localScale = new Vector3(1, 1, 1);
                UIDragScrollView scrollview = gb.AddComponent<UIDragScrollView>();
                scrollview.scrollView = m_ScrollTrans.GetComponent<UIScrollView>();
                gb.AddComponent<UICenterOnClick>();
                UIButtonScale buttonScale = gb.GetComponent<UIButtonScale>();
                if (buttonScale != null)
                    buttonScale.enabled = false;

                m_objList.Add(gb.transform);
            }
        }
        else
        {
            count = m_objList.Count - count;
            for (i = 0; i < count; i++)
            { 
                m_objList.Remove(m_objList[i]);
            }
        }
        m_uiLayer = m_DragTrans.gameObject.layer;

        SetDepth();
        m_UIWrapTrans.GetComponent<UICenterOnChild>().enabled = false;
        m_UIWrapTrans.GetComponent<UICenterOnChild>().enabled = true;
        return m_objList;
    }

    void SetDepth()
   {
       m_DragTrans.gameObject.layer = m_uiLayer;
       m_ScrollTrans.gameObject.layer = m_uiLayer;
       m_UIWrapTrans.gameObject.layer = m_uiLayer;
       for (int i = 0; i < m_objList.Count; i++)
       {
           m_objList[i].gameObject.layer = m_uiLayer;
       }
   }

   public  GameObject GetCenterObj()
    {
        UICenterOnChild centerchild = m_UIWrapTrans.GetComponent<UICenterOnChild>();
        return centerchild.centeredObject;
    }

    public void MoveDirection(Vector3 vec)
   {
       UIScrollView scrollview = m_ScrollTrans.GetComponent<UIScrollView>();
       scrollview.MoveAbsolute(vec);
   }

   public int GetItemCount()
    {
        return m_objList.Count;
    }

   public void DestroyScroll()
   {
       UIWrapContent wrapContent = m_UIWrapTrans.GetComponent<UIWrapContent>();
       wrapContent.m_UpdateItemCallBack = null;

       UICenterOnChild centerchild = m_UIWrapTrans.GetComponent<UICenterOnChild>();
       centerchild.enabled = false;
       centerchild.enabled = true;
       centerchild.onFinished = null;
   }

    public  string GetScrollName()
   {
       return m_name;
   }

    public void MoveToChoose(int chooseIndex)
    {
        if (m_direction == ScrollViewDirection.SVD_Horizontal)
        {
            m_ScrollTrans.localPosition = new Vector3(m_ScrollTrans.localPosition.x - chooseIndex * m_itemVec.x, 0, 0);
            UIPanel panel = m_ScrollTrans.GetComponent<UIPanel>();
            panel.clipOffset = new Vector2(panel.clipOffset.x + chooseIndex * m_itemVec.x, 0);
        }
        else if (m_direction == ScrollViewDirection.SVD_Vertical)
        {
            m_ScrollTrans.localPosition = new Vector3(0, m_ScrollTrans.localPosition.y - chooseIndex * m_itemVec.y, 0);
            UIPanel panel = m_ScrollTrans.GetComponent<UIPanel>();
            panel.clipOffset = new Vector2(0,panel.clipOffset.y + chooseIndex);
        }
    }
}
