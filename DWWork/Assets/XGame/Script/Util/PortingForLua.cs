/************************************************************
    File      : PortingForLua.cs
    brief     : 一些公共参数,公共函数
    author    : Jason   jason@ezfun.cn 
    version   : 1.0
    date      : 2015-10-31 12:18:55
    copyright : Copyright 2015 EZFun Inc.
**************************************************************/


using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;

public class PortingForLua
{
    public delegate void lua_call_back();

    public static int BinaryMoveLeft(int value, int count)
    {
        return value << count;
    }
    
    public static int BinaryMoveRight(int value, int count)
    {
        return value >> count;
    }
    
    public static Camera GetOrAddCamera(Transform camera_trans)
    {
        Camera bgCamera = EZFunTools.GetOrAddComponent<Camera>(camera_trans.gameObject);

        return bgCamera;
    }

    public static UICamera GetOrAddUICamera(Transform camera_trans)
    {
        UICamera bgCamera = EZFunTools.GetOrAddComponent<UICamera>(camera_trans.gameObject);
        
        return bgCamera;
    }

    public static void PlayAnimInAnimator(Transform trans, string name)
    {
        Animator anim = trans.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            anim.Play(name, 0, 0);
        }
    }

    public static void PlayAnimEndPlayIdle(Transform trans)
    {
        Animator anim = trans.GetComponentInChildren<Animator>();
        
        if (anim != null)
        {
            AnimatorStateInfo anim_info = anim.GetCurrentAnimatorStateInfo(0);

            if (!anim_info.IsNull())
            {
                if (!anim_info.IsName("idle") && anim_info.normalizedTime >= 0.99f)
                {
                    anim.Play("idle", 0, 0);
                }
            }
        }
        
        
    }

    public static void SetTweenerEndCallBack(Transform root, lua_call_back callbackfunc)
    {
        UITweener[] tweener = root.GetComponentsInChildren<UITweener>();

        if (tweener.Length > 0)
        {
            tweener[0].onFinished.Clear();
            tweener[0].onFinished.Add(new EventDelegate(() => {callbackfunc();}));
        }
    }

    public static void SetTweenerEnCallBackByName(Transform root, string tweener, lua_call_back callbackfunc)
    {
        var tween = (UITweener)root.GetComponent(tweener);
        if(null != tween)
        {
            tween.onFinished.Clear();
            tween.onFinished.Add(new EventDelegate(() => {
                var aa = root;
                callbackfunc(); 
            }));
        }
    }

    public static void PlayTweensInChildren(Transform root)
    {
        UITweener[] tweeners = root.GetComponentsInChildren<UITweener>();
        foreach (var tw in tweeners)
        {
            tw.enabled = true;
            tw.ResetToBeginning();
            tw.PlayForward();
        }
    }

    public static void PlayAnimationByName(Transform target, string aniName)
    {
        Animator[] anis = target.GetComponentsInChildren<Animator>();
        foreach (var ani in anis)
        {
            ani.enabled = true;
            var aa = ani.GetCurrentAnimatorStateInfo(0);
            ani.Play(aniName);
        }
    }
}