
using System;
using System.Collections.Generic;
using UnityEngine;


public class SpriteAnimation : MonoBehaviourEx
{
    public enum Mode
    {
        Once,
        Loop,
    }

    public UIAtlas atlas;
    public string spriteName;
    public int spriteBegin;
    public int spriteEnd;

    public Mode playMode = Mode.Once;
    public bool playOnStart = false;
    public float playDuration = 1f;
    public float loopInterval = 1f;
    public bool fadeOutOnFinish = true;
    public float fadeOutDuration = 1f;


    // 播放结束回调
    public Action OnFinish { get; set; }


    SpriteSequence mSpriteSequence;
    int mFrameIndex;
    float mFrameTime;
    float mAccumTime;
    // 动画状态，0 帧序列、1 循环延迟、2 隐藏过程
    int mAnimState;

    SpriteRenderer mSpriteRenderer;


    protected override void DoAwake()
    {
        mSpriteSequence = null;
        mFrameIndex = 0;
        mFrameTime = 0;
        mAccumTime = 0;
        mAnimState = 0;

        mSpriteRenderer = GetComponent<SpriteRenderer>();


        if (atlas != null && !string.IsNullOrEmpty(spriteName))
        {
            SpriteSequence seq = SpriteSequence.Create(atlas, spriteName, spriteBegin, spriteEnd);
            if (seq != null && seq.Count > 0)
            {
                mSpriteSequence = seq;
                mFrameTime = playDuration / seq.Count;

                Memento.Store(this);
            }
        }
    }

    void Start()
    {
        //enabled = playOnStart;

        if (mSpriteSequence != null)
            SetSprite(mSpriteSequence.GetSprite(0));
    }

    void Update()
    {
        if (mSpriteSequence == null)
            return;

        mAccumTime += Time.deltaTime;

        if (mAnimState == 0)
        {
            if (mAccumTime < mFrameTime)
                return;

            mAccumTime = 0f;

            int frameCount = mSpriteSequence.Count;
            int frameIndex = mFrameIndex + 1;

            if (frameIndex == frameCount)
            {
                if (playMode == Mode.Once)
                {
                    if (fadeOutOnFinish)
                        mAnimState = 2;
                    else
                        Stop(true);
                }
                else if (playMode == Mode.Loop)
                {
                    mAnimState = 1;
                }
            }
            else
            {
                mFrameIndex = frameIndex;

                SetSprite(mSpriteSequence.GetSprite(frameIndex));
            }
        }
        else if (mAnimState == 1)
        {
            if (mAccumTime >= loopInterval)
                Play();
        }
        else if (mAnimState == 2)
        {
            if (mAccumTime >= fadeOutDuration)
            {
                mAccumTime = fadeOutDuration;
                Stop(true);
            }

            float alpha = (fadeOutDuration - mAccumTime) / fadeOutDuration;
            SetAlpha(alpha);
        }
    }


    protected virtual void SetSprite(Sprite sprite)
    {
        if (mSpriteRenderer != null)
            mSpriteRenderer.sprite = sprite;
    }

    protected virtual void SetAlpha(float alpha)
    {
        if (mSpriteRenderer != null)
        {
            Color color = mSpriteRenderer.color;
            color.a = alpha;
            mSpriteRenderer.color = color;
        }
    }


    public void Play(string spriteName = null)
    {
        if (!string.IsNullOrEmpty(spriteName))
        {
            Mode oldMode = playMode;
            this.spriteName = spriteName;
            Memento.Restore(this);
            playMode = oldMode;
        }

        if (mSpriteSequence == null)
            return;

        enabled = true;
        mFrameIndex = 0;
        mAccumTime = 0;
        mAnimState = 0;

        SetSprite(mSpriteSequence.GetSprite(0));
        SetAlpha(1f);
    }

    public void Stop(bool isFinish = false)
    {
        enabled = false;

        Action callback = OnFinish;
        if (isFinish && callback != null)
            callback();
    }


    static public void ClearMemento()
    {
        Memento.Clear();
    }


    class Memento
    {
        public Mode playMode;
        public float playDuration;
        public float loopInterval;
        public bool fadeOutOnFinish;
        public float fadeOutDuration;
        public SpriteSequence spriteSequence;
        public float frameTime;


        static Dictionary<string, Memento> sMemento = new Dictionary<string, Memento>();

        static public void Store(SpriteAnimation sa)
        {
            if (sMemento.ContainsKey(sa.spriteName))
                return;

            Memento m = new Memento();
            m.playMode = sa.playMode;
            m.playDuration = sa.playDuration;
            m.loopInterval = sa.loopInterval;
            m.fadeOutOnFinish = sa.fadeOutOnFinish;
            m.fadeOutDuration = sa.fadeOutDuration;
            m.spriteSequence = sa.mSpriteSequence;
            m.frameTime = sa.mFrameTime;

            sMemento.Add(sa.spriteName, m);
        }

        static public void Restore(SpriteAnimation sa)
        {
            Memento m = null;
            if (!sMemento.TryGetValue(sa.spriteName, out m))
                return;

            sa.playMode = m.playMode;
            sa.playDuration = m.playDuration;
            sa.loopInterval = m.loopInterval;
            sa.fadeOutOnFinish = m.fadeOutOnFinish;
            sa.fadeOutDuration = m.fadeOutDuration;
            sa.mSpriteSequence = m.spriteSequence;
            sa.mFrameTime = m.frameTime;
        }

        static public void Clear()
        {
            sMemento.Clear();
        }
    }
}
