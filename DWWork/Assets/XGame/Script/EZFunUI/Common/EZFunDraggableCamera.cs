//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// 用ScolllView需要某些拖动后需要居中用，配合EZFunScollView使用
/// </summary>
[RequireComponent(typeof(Camera))]
[AddComponentMenu("EZFUN/Interaction/Draggable Camera")]
public class EZFunDraggableCamera : UIDraggableCamera
{
    /// <summary>
    /// scrollview里各个item的距离
    /// </summary>
    public Vector2 mGap = Vector2.zero;

    /// <summary>
    /// 拽动的总距离，如果曾经超过一格，但后续又低于一格，则将mJumpNextActive置为false
    /// </summary>
    Vector2 mDragDistance = Vector2.zero;
    /// <summary>
    /// 标志：是否启用挪一点点就跳转下一个的逻辑
    /// </summary>
    bool mJumpNextActive = true;

    /// <summary>
    /// 是否是列表头强行对齐
    /// </summary>
    bool _m_bFirstAdapt = false;
    public bool m_bFirstAdpt
    {
        get
        {
            return _m_bFirstAdapt;
        }
        set
        {
            _m_bFirstAdapt = value;
        }
    }
    /// <summary>
    /// 跳到指定的gb的位置.
    /// </summary>
    
    public void JumpTo(GameObject gb)
    {
        onfinished(gb.transform, 0, gb.transform.GetSiblingIndex());

        CenterOn(gb);
    }

    /// <summary>
    /// 直接停住.
    /// </summary>
    
    public override void Stop()
    {
        // Remove all momentum on press
        mMomentum = Vector2.zero;
        mScroll = 0f;
        
        // Disable the spring movement
        SpringPosition sp = GetComponent<SpringPosition>();
        if (sp != null) sp.enabled = false;
    }

    /// <summary>
    /// Constrain the current camera's position set to trans.
    /// </summary>

    public override void CenterOn(GameObject gb)
    {
        //Debug.LogError("CenterOn " + gb);
        //SpringPosition sp = SpringPosition.Begin(mTrans.gameObject, gb.transform.localPosition, 5);
        ////                sp.onFinished = () => {onfinished(closet, 0, closet.GetSiblingIndex());};
        //sp.ignoreTimeScale = true;
        //sp.worldSpace = false;
        CenterOn(gb.transform.localPosition);
    }

    public void CenterOn(Vector3 cPosition)
    {
        //Debug.LogError("CenterOn " + cPosition);
        Transform trans = mTrans;
        if (trans == null)
        {
            trans = transform;
        }
        SpringPosition sp = SpringPosition.Begin(trans.gameObject, CheckPosition(cPosition), 5);
        sp.ignoreTimeScale = true;
        sp.worldSpace = false;
    }

    private Vector3 CheckPosition(Vector3 pos)
    {
        if (!_m_bFirstAdapt || rootForBounds.childCount < 0)
            return pos;

        var firstItem = rootForBounds.GetChild(0);
        Vector3 firstEdge = firstItem.localPosition;

        var lastItem = rootForBounds.GetChild(rootForBounds.childCount - 1);
        Vector3 lastEdge = lastItem.localPosition;

        Rect curr_rect = EZFunUITools.GetRect(lastItem);

        if (scale.y != 0)
        {
            float halfBoxH = mTrans.GetComponent<BoxCollider>().size.y / 2;
            if (pos.y - (halfBoxH - curr_rect.height / 2) < lastEdge.y)
            {
                pos.y = lastEdge.y + (halfBoxH - curr_rect.height / 2);
            }
            else if (pos.y + (halfBoxH - curr_rect.height / 2) > firstEdge.y)
            {
                pos.y = firstEdge.y - (halfBoxH - curr_rect.height / 2);
            }
        }

        if (scale.x != 0)
        {
            float halfBoxW = mTrans.GetComponent<BoxCollider>().size.x / 2;
            if (pos.x + (halfBoxW - curr_rect.width / 2) > lastEdge.x)
            {
                pos.x = lastEdge.x - (halfBoxW - curr_rect.width / 2);
            }
            else if (pos.x - (halfBoxW - curr_rect.width / 2) < firstEdge.x)
            {
                pos.x = firstEdge.x + (halfBoxW - curr_rect.width / 2);
            }
        }

        return pos;
    }

    /// <summary>
    /// Constrain the current camera's position set to closet child.
    /// </summary>

    public bool CenterOnChild (int index_offset = 0)
    {
//        Debug.LogError("CenterOnChild " + index_offset);
        if (mTrans != null && rootForBounds != null)
        {
            Transform t = null;
            Transform closet = null;
            int index = 0;
            //Debug.LogError("mTrans.localPosition = " + mTrans.localPosition);
            for (int i = 0; i < rootForBounds.childCount; i++)
            {
                t = rootForBounds.GetChild(i);

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

            if (true_index >= rootForBounds.childCount)
            {
                true_index = rootForBounds.childCount - 1;
            }
            else if (true_index < 0)
            {
                true_index = 0;
            }

            closet = rootForBounds.GetChild(true_index);
            onfinished(closet, 0, true_index);

            if (Vector3.Magnitude(closet.localPosition - mTrans.localPosition) > 0f)
            {
                CenterOn(closet.gameObject);
                return true;
            }
        }

        return false;
    }

    //private static int req  = 0;
    /// <summary>
    /// 仅仅对水平方向有效
    /// </summary>
    /// <param name="index_offset"></param>
    /// <returns></returns>
    public bool ForceFirstAdapt(int index_offset = 0)
    {
        if (mTrans != null && rootForBounds != null)
        {
            Transform t = null;
            Transform closet = null;
            int index = 0;
            float halfBoxWidth = mTrans.GetComponent<BoxCollider>().size.x / 2;
            Vector3 leftPosition = mTrans.localPosition;
            //req++;
            //Debug.LogError("mTrans.localPosition = " + leftPosition + "  " + req);
            leftPosition.x -= mTrans.GetComponent<BoxCollider>().size.x/2;
            //Debug.LogError("leftPosition = " + leftPosition);
            for (int i = 0; i < rootForBounds.childCount; i++)
            {
                t = rootForBounds.GetChild(i);

                if (closet == null)
                {
                    closet = t;
                    index = i;
                }
                else
                {
                    if (t.localPosition.x > leftPosition.x && Vector3.Magnitude(t.localPosition - leftPosition) < Vector3.Magnitude(closet.localPosition - leftPosition))
                    {
                        closet = t;
                        index = i;
                    }
                }
            }

            int true_index = index + index_offset;

            if (true_index >= rootForBounds.childCount)
            {
                true_index = rootForBounds.childCount - 1;
            }
            else if (true_index < 0)
            {
                true_index = 0;
            }

            closet = rootForBounds.GetChild(true_index);
            onfinished(closet, 0, true_index);
            Rect curr_rect = EZFunUITools.GetRect(closet);
            Vector3 centerPosition = closet.localPosition + new Vector3(halfBoxWidth - curr_rect.width / 2, 0, 0);
            LearnFFMaxPosition();
            if (centerPosition.x > m_FFMaxPosition.x)
            {
                centerPosition = m_FFMaxPosition;
            }
            if (Vector3.Magnitude(centerPosition - mTrans.localPosition) > 0f)
            {
                CenterOn(centerPosition);
                return true;
            }
        }

        return false;
    }

    private Vector3 m_FFMaxPosition;
    private bool bHasLearn = false;
    private void LearnFFMaxPosition()
    {
        if (bHasLearn)
        {
            return;
        }
        bHasLearn = true;
        float halfBoxWidth = mTrans.GetComponent<BoxCollider>().size.x / 2;
        var rightT = rootForBounds.GetChild(rootForBounds.childCount - 1);
        Vector3 rightEdge = rightT.localPosition;
        Rect curr_rect = EZFunUITools.GetRect(rightT);
        rightEdge.x += curr_rect.width / 2;
        for (int i = 0; i < rootForBounds.childCount; i++)
        {
            var closet = rootForBounds.GetChild(i);
            curr_rect = EZFunUITools.GetRect(closet);
            Vector3 centerPosition = closet.localPosition + new Vector3(halfBoxWidth - curr_rect.width / 2, 0, 0);
            if (centerPosition.x + halfBoxWidth > rightEdge.x)
            {
                m_FFMaxPosition = centerPosition;
                return;
            }
        }
    }

	/// <summary>
	/// Calculate the bounds of all widgets under this game object.
	/// </summary>

	public override void Press (bool isPressed)
	{
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

    bool CheckDragTooLittle(Vector2 delta)
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

	public override void Drag (Vector2 delta)
	{
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

	public override void Scroll (float delta)
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

	void Update ()
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

                if (_m_bFirstAdapt)
                {
                    ForceFirstAdapt(jumpOffset);
                }
                else
                {
                    CenterOnChild(jumpOffset);
                }
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
                        if (_m_bFirstAdapt)
                        {
                            if (!ForceFirstAdapt())
                            {
                                SpringPosition sp = GetComponent<SpringPosition>();
                                if (sp != null) sp.enabled = false;
                            }
                        }
                        else
                        {
                            if (!CenterOnChild())
                            {
                                SpringPosition sp = GetComponent<SpringPosition>();
                                if (sp != null) sp.enabled = false;
                            }
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
}
