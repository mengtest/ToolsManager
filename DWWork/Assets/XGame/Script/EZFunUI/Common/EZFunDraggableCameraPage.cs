using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// 用ScolllView需要某些拖动后需要居中用，配合EZFunScollView使用
/// </summary>
public class EZFunDraggableCameraPage : UIDraggableCamera
{
    /// <summary>
    /// scrollview里各个item的距离
    /// </summary>
    public Vector2 mGap = Vector2.zero;
    /// <summary>
    /// 如果是横向的话，这个就是行的数据，如果是纵向的刷，这个就是列的数据
    /// </summary>
    public int curNum = 0;
    /// <summary>
    /// 如果是横向的话，这个就是一个摄像机能看到多列
    /// </summary>
    public int boxContaintRowCount = 0;
    /// <summary>
    /// 拽动的总距离，如果曾经超过一格，但后续又低于一格，则将mJumpNextActive置为false
    /// </summary>
    protected Vector2 mDragDistance = Vector2.zero;
    /// <summary>
    /// 标志：是否启用挪一点点就跳转下一个的逻辑
    /// </summary>
    protected bool mJumpNextActive = true;
    public bool m_isCanTouch = true;

    public Vector2 ContentVec;
    public string m_luaChangePage = "";

    private int m_maxPage = 0;
    public int nowPage = 0;

    public int maxPage
    {
        get {
            m_maxPage = (int)Mathf.Ceil((GetChildCount(rootForBounds) / (curNum * (float)boxContaintRowCount)));
            return m_maxPage;
        }
    }

    Transform GetActiveChild(Transform trans, int index)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            if (NGUITools.GetActive(trans.GetChild(i).gameObject))
            {
                index--;
            }
            if (index <= -1)
            {
                return trans.GetChild(i);
            }
        }
        return null;
    }


    /// <summary>
    /// Calculate the bounds of all widgets under this game object.
    /// </summary>
    public override void Press(bool isPressed)
    {
        if (!m_isCanTouch)
        {
            return;
        }
        if (isPressed) mDragStarted = false;

        if (rootForBounds != null)
        {
            mPressed = isPressed;

            if (isPressed)
            {
                // Update the bounds
                mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds);

                // Remove all momentum on press
                mMomentum = Vector2.zero;
                mScroll = 0f;

                // Disable the spring movement
                SpringPosition sp = GetComponent<SpringPosition>();
                if (sp != null) sp.enabled = false;

                //init state for jump
                mJumpNextActive = true;
            }
            else
            {
            }
        }
    }

    /// <summary>
    /// 检查是否滑动距离小于两个子item的距离.
    /// </summary>

    //protected bool CheckDragTooLittle(Vector2 delta)
    //{
    //    if ((Mathf.Abs(scale.x) > 0 && Mathf.Abs(mGap.x) / 2 > Mathf.Abs(delta.x)) ||
    //        (Mathf.Abs(scale.y) > 0 && Mathf.Abs(mGap.y) / 2 > Mathf.Abs(delta.y)))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    /// <summary>
    /// Drag event receiver.
    /// </summary>
    public override void Drag(Vector2 delta)
    {
        if (!m_isCanTouch)
        {
            return;
        }
        // Prevents the initial jump when the drag threshold gets passed
        if (smoothDragStart && !mDragStarted)
        {
            mDragStarted = true;
            return;
        }

        UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
        if (mRoot != null) delta *= mRoot.pixelSizeAdjustment;

        Vector2 offset = Vector2.Scale(delta, -scale);
        mTrans.localPosition += (Vector3)offset;

        //
        mDragDistance += offset;

        if (mJumpNextActive)
        {
            //if (!CheckDragTooLittle(mDragDistance))
            //{
            //    mJumpNextActive = false;
            //}
        }
        else
        {
            // Adjust the momentum
            mMomentum = Vector2.Lerp(mMomentum, mMomentum + offset * (0.01f * momentumAmount), 0.67f);

            // Constrain the UI to the bounds, and if done so, eliminate the momentum
            if (dragEffect != UIDragObject.DragEffect.MomentumAndSpring && ConstrainToBounds(true))
            {
                mMomentum = Vector2.zero;
                mScroll = 0f;
            }
        }
    }

    /// <summary>
    /// If the object should support the scroll wheel, do it.
    /// </summary>
    public override void Scroll(float delta)
    {
        if (enabled && NGUITools.GetActive(gameObject))
        {
            if (Mathf.Sign(mScroll) != Mathf.Sign(delta)) mScroll = 0f;
            mScroll += delta * scrollWheelFactor;
        }
    }

    /// <summary>
    /// Apply the dragging momentum.
    /// </summary>
    void Update()
    {
        float delta = RealTime.deltaTime;
        if (mPressed)
        {
            // Disable the spring movement
            SpringPosition sp = GetComponent<SpringPosition>();
            if (sp != null) sp.enabled = false;
            mScroll = 0f;
        }
        else
        {
            if(mJumpNextActive)
            {
                int jumpOffset = 0;
                if (mDragDistance.x > 0 || mDragDistance.y < 0)
                {
                    jumpOffset = 1;
                }
                else if (mDragDistance.x < 0 || mDragDistance.y > 0)
                {
                    jumpOffset = -1;
                }

                CenterOnChild(jumpOffset);
            }

            //reset state for jump
            mDragDistance = Vector2.zero;
            mJumpNextActive = false;
        }

        // Dampen the momentum
        NGUIMath.SpringDampen(ref mMomentum, 6f, delta);
    }

    public int GetChildCount(Transform trans)
    {
        int count = 0;
        for (int i = 0; i < trans.childCount; i++)
        {
            if (NGUITools.GetActive(trans.GetChild(i).gameObject))
            {
                count++;
            }
        }
        return count;
    }

    public void SetPageSize(Vector2 vect)
    {
        if (vect == Vector2.zero)
        {
            if (scale.y > 0)
            {
                vect = new Vector2(mGap.x * boxContaintRowCount, mGap.y * curNum);
            }
            else
            {
                vect = new Vector2(mGap.x * curNum, mGap.y * boxContaintRowCount);
            }
        }
        ContentVec = vect;
    }

    public void CenterOnChild(int index_offset = 0)
    {
        if (mTrans != null && rootForBounds != null)
        {
            m_maxPage = (int)Mathf.Ceil((GetChildCount(rootForBounds) / (curNum * (float)boxContaintRowCount)));

            nowPage += index_offset;
            if (nowPage > m_maxPage - 1)
                nowPage = m_maxPage - 1;
            if (nowPage < 0)
                nowPage = 0;

            Vector3 targetPosition = Vector3.zero;
            if (scale.y > 0)
            {
                targetPosition.y = ContentVec.y * nowPage;
            }
            else
            {
                targetPosition.x = ContentVec.x * nowPage;
            }
            CenterOn(targetPosition);
        }
    }

    /// <summary>
    /// Constrain the current camera's position set to trans.
    /// </summary>
    public void CenterOn(Vector3 targetPosition)
    {
        if (scale.y > 0)
        {
            targetPosition.x = 0;
        }
        else
        {
            targetPosition.y = 0;
        }
        SpringPosition sp = SpringPosition.Begin(mTrans.gameObject, targetPosition, 5);
        sp.ignoreTimeScale = true;
        sp.worldSpace = false;

        if (!string.IsNullOrEmpty(m_luaChangePage))
        {
            WindowBaseLua.m_luaMgr.CallLuaFunction(m_luaChangePage, nowPage);
        }
    }

}

