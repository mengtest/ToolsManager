/************************************************************
//     文件名      : EZFunCenterPage.cs
//     功能描述    : 不会出现上下没有的剧中
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-11-09 11:26:31.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;

public class EZFunCenterOnPage : UICenterOnChild
{

    // Use this for initialization
    void Start()
    {

    }

    void OnEnable() { mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject); }

    void OnDisable() { }

    /// <summary>
    /// Center the panel on the specified target.
    /// </summary>
    protected override void CenterOn(Transform target, Vector3 panelCenter)
    {
        if (target != null && mScrollView != null && mScrollView.panel != null)
        {
            Transform panelTrans = mScrollView.panel.cachedTransform;
            mCenteredObject = target.gameObject;
            var mPanel = mScrollView.panel;
            // Figure out the difference between the chosen child and the panel's center in local coordinates
            Vector3 cp = panelTrans.InverseTransformPoint(target.position);
            Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
            Vector3 localOffset = cp - cc;


            // Offset shouldn't occur if blocked
            if (!mScrollView.canMoveHorizontally) localOffset.x = 0f;
            if (!mScrollView.canMoveVertically) localOffset.y = 0f;
            localOffset.z = 0f;

            //这里用原来的 uiScrollView居中来中和移动这个位置校正这里的居中，保证移动不会有空的地方
            Vector3 constraint = mPanel.CalculateConstrainOffset(mScrollView.bounds.min - localOffset,
                mScrollView.bounds.max - localOffset);
            localOffset -= constraint;
            if (!mScrollView.canMoveHorizontally) localOffset.x = 0f;
            if (!mScrollView.canMoveVertically) localOffset.y = 0f;
            localOffset.z = 0f;
            // Spring the panel to this calculated position
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                panelTrans.localPosition = panelTrans.localPosition - localOffset;

                Vector4 co = mScrollView.panel.clipOffset;
                co.x += localOffset.x;
                co.y += localOffset.y;
                mScrollView.panel.clipOffset = co;
            }
            else
#endif
            {
                SpringPanel.Begin(mScrollView.panel.cachedGameObject,
                    panelTrans.localPosition - localOffset, springStrength).onFinished = onFinished;
            }
        }
        else mCenteredObject = null;

        // Notify the listener
        if (onCenter != null) onCenter(mCenteredObject);
    }
}
