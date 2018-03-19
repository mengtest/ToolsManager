using UnityEngine;
using System.Collections;

public class HandleErrTipsWindow : WindowRoot {

	public static string m_contentStr = "";

    private Transform m_position = null;
    private Transform m_spTipsRoot = null;
    private Transform m_footRoot = null;

	private UITweener[] m_tweenArray = null;

    private UITweener m_footRootTween = null;

	public override void InitWindow (bool open=true, int state=0)
	{
        m_IsTipWindow = true;
        InitCamera(open, false, (int)SpecialWindowDepthEnum.SWE_ErrTipsWindow);
		base.InitWindow (open, state);

		if(open)
		{
			InitWindowDetail();
		}
	}

	protected override void CreateWindow ()
	{
		base.CreateWindow ();

        m_position = GetTrans("position");
        m_spTipsRoot = GetTrans("spTipsRoot");
        m_footRoot = GetTrans("footRoot");

		m_tweenArray = m_position.GetComponents<UITweener>();
		if(m_tweenArray != null && m_tweenArray.Length > 0)
		{
			m_tweenArray[0].onFinished.Add(new EventDelegate( ()=> InitWindow(false)));
		}

        m_footRootTween = m_footRoot.GetComponent<UITweener>();
        m_footRootTween.onFinished.Add(new EventDelegate(() => InitWindow(false)));
	}

	void InitWindowDetail()
	{
        if(m_selfOpenState == 0)
        {
            SetActive(m_position, true);
            SetActive(m_spTipsRoot, false);
            SetActive(m_footRoot, false);
            SetLabel(GetTrans(m_position, "valueLabel"), m_contentStr,false);
		    //AutoSetBgScale(GetTrans(m_position, "bg"), GetTrans("valueLabel"), new Vector4(40, 12, 40, 14));
		    //AutoSetBgScale(GetTrans(m_position, "bg1"), GetTrans("valueLabel"), new Vector4(25, 18, 25, 18));
		    //AutoSetBgScale(GetTrans(m_position, "bg2"), GetTrans("valueLabel"), new Vector4(25, 18, 25, 18));

		    if(m_tweenArray != null)
		    {
			    for(int i = 0; i < m_tweenArray.Length; i ++)
			    {
				    m_tweenArray[i].ResetToBeginning();
				    m_tweenArray[i].enabled = true;
			    }
		    }
        }
        else if(m_selfOpenState == 1)
        {
            SetActive(m_position, false);
            SetActive(m_spTipsRoot, true);
            SetActive(m_footRoot, false);
            GenTipsIndependently(m_spTipsRoot, m_contentStr);
        }
        else if (m_selfOpenState == 3)
        {
            SetActive(m_position, false);
            SetActive(m_spTipsRoot, false);
            SetActive(m_footRoot, true);
            SetLabel(GetTrans(m_footRoot, "footLabel"), m_contentStr, false);
            AutoSetBgScale(GetTrans(m_footRoot, "bg"), GetTrans("footLabel"), new Vector4(40, 12, 40, 14));
            m_footRootTween.ResetToBeginning();
            m_footRootTween.enabled = true;
        }
		
	}

    private int m_tipsTotalCount = 0;
    private int m_expTipsDepth = 10;
    private void GenTipsIndependently(Transform rootTrans, string text)
    {
        GameObject exp_tip = (GameObject)ResourceMgr.GetInstantiateAsset(RessType.RT_UIItem, "constellation_exp_tip");
        SetActive(exp_tip.transform, true);
        SetParentAndReset(exp_tip.transform, rootTrans);

        SetLabel(GetTrans(exp_tip.transform, "valueLabel"), text, false);
        PlayTweensInChildren(exp_tip.transform);

        exp_tip.GetComponent<UIPanel>().depth = m_expTipsDepth++;
        m_tipsTotalCount++;

        TweenPosition tal = exp_tip.transform.GetComponent<TweenPosition>();
        tal.onFinished.Clear();
        tal.onFinished.Add(new EventDelegate(() =>
        {
            SetActive(exp_tip.transform, false);
            m_tipsTotalCount--;

            if (m_tipsTotalCount <= 0)
            {
                m_expTipsDepth = 10;
            }
        }));
    }


}
