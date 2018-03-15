using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// 用ScolllView需要某些拖动后需要居中用，配合EZFunScollView使用
/// </summary>
public class EZFunDraggableCamraNumRow : UIDraggableCamera
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


    private int _curArrow;
    /// <summary>
    /// 如果有箭头的话，这个表示箭头应该的方向，0表示上下都可以,1表示只能向下，2表示只能向上,3表示上下都不可以
    /// </summary>
    public int CurArrow
    {
        get { return _curArrow; }
    }

    public bool m_isCanTouch = true;

    /// <summary>
    /// 跳到指定的gb的位置.
    /// </summary>

    public void JumpTo(GameObject gb)
    {
        if (!m_isCanTouch)
        {
            return;
        }
        onfinished(gb.transform, -1, gb.transform.GetSiblingActiveIndex());

        CenterOn(gb);
    }



    public override void CenterOn(GameObject gb)
    {
        int index = gb.transform.GetSiblingActiveIndex();
        BoxCollider collider = mTrans.GetComponent<BoxCollider>();
        //一行有几个
        int MaxRow = (GetChildCount(rootForBounds) + curNum - 1) / curNum;
        int true_index = (index ) / curNum;
        
        Vector3 targetPosition = Vector3.zero;
        //一个整页都满足不了，直接用中间点呗
        if (MaxRow <= boxContaintRowCount)
        {
            targetPosition = Vector3.zero;
            true_index = 0;
            _curArrow = 3;
        }
        else if (true_index <= (boxContaintRowCount ) / 2)   //点到最小行区域还小的地方了
        {
            targetPosition = Vector3.zero;
            true_index = 0;
            _curArrow = 1;
        }
        else if (MaxRow - true_index <= (boxContaintRowCount + 1) / 2)
        {
            targetPosition = GetActiveChild(rootForBounds,((MaxRow - ((boxContaintRowCount + 1) / 2)) * curNum)).localPosition;
            true_index = (MaxRow - ((boxContaintRowCount + 1) / 2));
            _curArrow = 2;
        }
        else
        {
            targetPosition = GetActiveChild(rootForBounds,true_index * curNum).localPosition;
            _curArrow = 0;
        }

        if (Vector3.Magnitude(targetPosition - mTrans.localPosition) > 0f)
        {
            CenterOn(targetPosition);
        }

    }

    Transform GetActiveChild(Transform trans, int index)
    {
        for (int i = 0; i < trans.childCount; i ++ )
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

    protected  bool CheckDragTooLittle(Vector2 delta)
    {
        if ((Mathf.Abs(scale.x) > 0 && Mathf.Abs(mGap.x) / 2 > Mathf.Abs(delta.x)) ||
            (Mathf.Abs(scale.y) > 0 && Mathf.Abs(mGap.y) / 2 > Mathf.Abs(delta.y)))
        {
            return true;
        }

        return false;
    }

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
            if (!CheckDragTooLittle(mDragDistance))
            {
                mJumpNextActive = false;
            }
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
            if (mJumpNextActive && mDragDistance.magnitude > 0.01f)
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
            else
            {
                mMomentum += scale * (mScroll * 20f);
                mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, delta);

                if (mMomentum.magnitude > 0.01f || mDragDistance.magnitude > 0.01f)
                {
                    // Apply the momentum
                    mTrans.localPosition += (Vector3)NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
                    mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds);

                    if (mMomentum.magnitude < 10f)
                    {
                        if (!CenterOnChild())
                        {
                            SpringPosition sp = GetComponent<SpringPosition>();
                            if (sp != null) sp.enabled = false;
                        }
                    }
                    mDragDistance = Vector2.zero;
                    return;
                }
                else mScroll = 0f;
            }

            //reset state for jump
            mDragDistance = Vector2.zero;
            mJumpNextActive = false;
        }

        // Dampen the momentum
        NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
    }

    /// <summary>
    /// 检查是否滑动距离小于两个子item的距离.
    /// </summary>

    //protected override bool CheckDragTooLittle(Vector2 delta)
    //{
    //    if (
    //        (Mathf.Abs(scale.y) > 0 && Mathf.Abs(mGap.y) / 2 > Mathf.Abs(delta.y)))
    //    {
    //        return true;
    //    }

    //    return false;
    //}


    public int GetChildCount(Transform trans)
    {
        int count = 0;
        for (int i = 0; i < trans.childCount; i++ )
        {
            if (NGUITools.GetActive(trans.GetChild(i).gameObject))
            {
                count++;
            }
        }

        return count;
    }


    public  bool CenterOnChild(int index_offset = 0)
    {
        //        Debug.LogError("CenterOnChild " + index_offset);
        if (mTrans != null && rootForBounds != null)
        {
            Transform t = null;
            Transform closet = null;
            int index = 0;
            if (curNum == 0)
            {
                curNum = 1;
            }
            int MaxRow = (GetChildCount(rootForBounds) + curNum - 1) / curNum;

            for (int i = 0; i < (GetChildCount(rootForBounds) + curNum - 1) / curNum ; i++)
            {
                t = GetActiveChild(rootForBounds, i * curNum);

                if (closet == null)
                {
                    closet = t;
                    index = i;
                }
                else
                {
                    if (Vector3.Magnitude(t.localPosition - mTrans.localPosition) < Vector3.Magnitude(closet.localPosition - mTrans.localPosition))
                    {
                        closet = t;
                        index = i;
                    }
                }
            }

            int true_index = index + index_offset;

            Vector3 targetPosition = Vector3.zero;
            //一个整页都满足不了，直接用中间点呗
            if (MaxRow <= boxContaintRowCount)
            {
                targetPosition = Vector3.zero;
                true_index = 0;
                _curArrow = 3;
            }
            else if (true_index <= (boxContaintRowCount ) / 2)   //点到最小行区域还小的地方了
            {
                targetPosition = Vector3.zero;
                true_index = 0;
                _curArrow = 1;
            }
            else if (MaxRow - (true_index + 1) < (boxContaintRowCount ) / 2)
            {
                targetPosition = GetActiveChild(rootForBounds, (MaxRow - ((boxContaintRowCount + 1) / 2)) * curNum).localPosition;
                true_index = MaxRow - (boxContaintRowCount) / 2;
                _curArrow = 2;
            }
            else
            {
                _curArrow = 0;
                targetPosition = GetActiveChild(rootForBounds, true_index * curNum).localPosition;
            }
            if (Vector3.Magnitude(targetPosition - mTrans.localPosition) > 0f)
            {
                CenterOn(targetPosition);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Constrain the current camera's position set to trans.
    /// </summary>

    public  void CenterOn(Vector3 targetPosition)
    {
        if (scale.y > 0)
        {
            targetPosition.x = 0;
        }
        else
        {
            targetPosition.y = 0;
        }
        //        Debug.LogError("CenterOn " + gb);
        SpringPosition sp = SpringPosition.Begin(mTrans.gameObject, targetPosition, 5);
        //                sp.onFinished = () => {onfinished(closet, 0, closet.GetSiblingIndex());};
        sp.ignoreTimeScale = true;
        sp.worldSpace = false;
    }

}

