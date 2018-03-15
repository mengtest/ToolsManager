//     File      : DWSpriteAnimation.cs
//     brief     : �������� ������Ҫ���ŵ����� �����ظ� ���������ѡ������ 
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
    //�ֶ���д������ �Ƚ������ �����ظ��� Ҫ�����
    public string[] spriteNames;
    //˳������� һ��Ҫ��˳����
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

    // ���Ž����ص�
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
