//     File      : DWSpriteAnimation.cs
//     brief     : 动画播放 拖入需要播放的序列 可以重复 播放完可以选择销毁 
//     author    : jianing
//     version   : 1.0
//     date      : 2017/10/25 15:17:58
//     copyright : Copyright 2017 DW Inc.
// **************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DWSpriteAnimation : MonoBehaviour
{
    public int Framerate = 2;
    public bool Loop = true;
    //手动填写的序列 比较特殊的 或者重复的 要用这个
    public string[] spriteNames;
    //顺序的序列 一定要按顺序来
    public string spriteName;
    public int spriteBegin;
    public int spriteEnd;
    public bool isEndDstory = false;
    public float delayTime = 0.5f;

    private UISprite mySprite;
    private int tickTime = 0;
    private int nowIndex = 0;
    private int totalFramerate = 0;
    private List<string> spriteNameList;
    private bool isPlay = true;
    private float tickDelayTime = 0.5f;

    // 播放结束回调
    public Action OnFinish { get; set; }

    void Start() 
    {
        spriteNameList = new List<string>();

        if (!string.IsNullOrEmpty(spriteName) && spriteEnd > spriteBegin)
        {
            spriteNames = new String[spriteEnd - spriteBegin];
            for (int i = spriteBegin; i <= spriteEnd; i++)
            {
                spriteNameList.Add(spriteName + i);
            }
        }
        else if (spriteNames != null)
        {
            for (int i = 0; i < spriteNames.Length; i++)
            {
                 spriteNameList.Add(spriteNames[i]);
            }
        }

        mySprite = GetComponent<UISprite>();
        totalFramerate = spriteNameList.Count;
        isPlay = true;
        nowIndex = 0;
        tickTime = 0;
        tickDelayTime = delayTime;
    }

    void OnEnable() 
    {
        isPlay = true;
        nowIndex = 0;
        tickTime = 0;
        tickDelayTime = delayTime;
    }

    void Update()
    {
        if(mySprite == null || totalFramerate == 0 || !isPlay)
            return;

        tickTime++;
        if (tickTime > Framerate) 
        {
            tickTime = 0;
            nowIndex++;
            if (nowIndex < totalFramerate)
            {
                mySprite.spriteName = spriteNameList[nowIndex];
            }
            else 
            {
                if (Loop)
                {
                    nowIndex = 0;
                    mySprite.spriteName = spriteNameList[nowIndex];
                }
                else 
                {
                    tickDelayTime -= Time.deltaTime;
                    if(tickDelayTime <= 0)
                        Stop(true);
                }
            }
        }
    }

    public void Stop(bool sendFinish) 
    {
        isPlay = false;
        if (isEndDstory)
            gameObject.SetActive(false);

        if (OnFinish != null && sendFinish)
            OnFinish();

        OnFinish = null;
    }

    void OnDestory() 
    {
        OnFinish = null;
    }
}
