/************************************************************
//     文件名      : BaseUI.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-08-30 14:26:38.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using ezfun_resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[LuaWrap]
[RequireComponent(typeof(UIEventHandleProxy))]
public class BaseUI : MonoBehaviour
{
    [NonSerialized]
    public EZFunWindowEnum m_currentWindowEnum;
    protected UIEventHandleProxy m_eventProxy;
    protected bool m_ignoreParentClick = false;

    [NonSerialized]
    public string m_windowName;

    public const int CLOSE_STATE = -1002;

    protected WindowRoot m_windowRoot;

    [NonSerialized]
    public bool m_open;

    public void CreateUI()
    {
        m_eventProxy = EZFunTools.GetOrAddComponent<UIEventHandleProxy>(this.gameObject);
        m_eventProxy.m_PreClickHandleCallBack = HandleWidgetBasePress;
        m_eventProxy.m_ClickHandleCallBack = HandleWidgetBaseClick;
    }

	public virtual void OnDestroy()
	{

	}
    #region check click 


    public bool CheckBtnCanClick(GameObject go, bool resetTime = false)
    {
        return true;
    }

    //上面的用的地方太多 放基类会重复监测 所以单独拿这个出来
    public bool CheckBtnCanClickBase(GameObject go, bool resetTime = false)
    {
        if (m_windowRoot != null)
        {
            return m_windowRoot.CheckBtnCanClickSrc(go, resetTime);
        }
        return false;
    }

    #endregion

    #region click handle
    public void HandleWidgetBaseClick(GameObject gameObj)
    {
        //点击监测还是放到基类 人物和input的 会m_windowRoot == null 先放过 到子类中监测
        if (m_windowRoot != null && !CheckBtnCanClickBase(gameObj))
        {
            return;
        }
        HandleWidgetClick(gameObj);
    }


    private void HandleWidgetBasePress(UIButtonMessage.PressObject pressObj)
    {
        if (pressObj.m_IsPressed)
        {
            HandleWidgetPressed(pressObj);
        }
        else
        {
            HandleWidgetRelease(pressObj);
        }
    }

    /// <summary>
    /// ��ť����
    /// </summary>
    /// <param name="pressObj"></param>
    virtual protected void HandleWidgetPressed(UIButtonMessage.PressObject pressObj)
    {
      
    }
    /// <summary>
    /// ��ť�ͷ�
    /// </summary>
    virtual protected void HandleWidgetRelease(UIButtonMessage.PressObject pressObj)
    {

    }

	private static System.Action<GameObject> m_onClickCallback;
	public static void SetOnClickCallback (System.Action<GameObject> cb)
	{
		m_onClickCallback = cb;
	}

    protected virtual void HandleWidgetClick(GameObject gameObj)
    {
		if (m_onClickCallback != null) {
			m_onClickCallback (gameObj);
		}
    }

    protected virtual void HandleWidgetDrag(Vector2 delta)
    {
    }
    #endregion


    #region activity_ui_window & task_ui_window ����Ӧ��Ļ
    UIRoot GetUIRoot(Transform trans)
    {
        UIRoot ui_root = null;

        //������λ�ú�ȡuiroot
        Transform tmp = trans;
        while (tmp != null)
        {
            ui_root = tmp.GetComponent<UIRoot>();

            if (ui_root == null)
            {
                tmp = tmp.parent;
            }
            else
            {
                return ui_root;
            }
        }

        return null;
    }
    //由屏幕尺寸转换成ui尺寸
    public Vector2 TransformScreenSize(int width, int height)
    {
        Vector2 screen_size = new Vector2(width, height);
        UIRoot ui_root = GetUIRoot(transform);

        if (ui_root != null)
        {
            //横向拉伸处理
            screen_size *= ui_root.GetPixelSizeAdjustment(Screen.height);
            //纵向拉伸处理，包含下边的(ui_root.activeHeight - ui_root.manualHeight) * 0.5f 
            screen_size *= ui_root.activeHeight * 1f / ui_root.manualHeight;
        }

        return screen_size;
    }

    //取当前trans的box相对于屏幕中心（0,0），四个边界的像素位置，单位以ui尺寸单位为参照
    public Vector4 GetBoardDistance(Transform trans)
    {
        Vector3 local_position = Vector3.zero;
        UIRoot ui_root = null;

        //算相对位置和取uiroot
        Transform tmp = trans;
        while (tmp != null)
        {
            ui_root = tmp.GetComponent<UIRoot>();

            if (ui_root == null)
            {
                local_position += tmp.localPosition;
                tmp = tmp.parent;
            }
            else
            {
                break;
            }
        }

        if (ui_root == null)
        {
#if UNITY_EDITOR
            Debug.LogError("[WEB VIEW] can't find UIRoot!!!");
#endif
            return Vector4.zero;
        }

        //通过相对位置和box大小算距离边界的距离
        float top = 0, bottom = 0, left = 0, right = 0;
        BoxCollider box = trans.GetComponent<BoxCollider>();

        if (box == null)
        {
#if UNITY_EDITOR
            Debug.LogError("[WEB VIEW] can't find BOX!!!");
#endif
            return Vector4.zero;
        }

        local_position += box.center;
        top = (local_position.y + box.size.y * 0.5f);
        bottom = (local_position.y - box.size.y * 0.5f);
        left = (local_position.x - box.size.x * 0.5f);
        right = (local_position.x + box.size.x * 0.5f);

        return new Vector4(left, top, right, bottom);
    }


    //scrollview的box根据屏幕适配,left,top,right,bottom四个参数代表四个方向的适配类型
    public Vector3 SelfAdaptScale(Transform node, AdaptType left, AdaptType top, AdaptType right, AdaptType bottom)
    {
        Vector4 board = GetBoardDistance(node);
        Vector4 ref_size = new Vector2(1136f, 720f);
        Vector2 screen_size = TransformScreenSize(Screen.width, Screen.height);

        if (screen_size.x >= 1130)
        {
            //Debug.LogError("screen width = " + screen_size.x);
            return Vector3.zero;
        }

        Vector3 size_offset = Vector3.zero;
        Vector3 pos_offset = Vector3.zero;
        if (left == AdaptType.normal && right != AdaptType.rigid)
        {
            size_offset += new Vector3((screen_size.x - ref_size.x) / 2, 0, 0);
            pos_offset += new Vector3(-(screen_size.x - ref_size.x) / 4, 0, 0);
        }
        else if (left == AdaptType.rigid)
        {
            if (right != AdaptType.none)
            {
#if UNITY_EDITOR
                Debug.LogError(TextData.GetText(400052));
#endif
                right = AdaptType.none;
            }

            pos_offset += new Vector3(-(screen_size.x - ref_size.x) / 2, 0, 0);
        }

        if (top == AdaptType.normal && bottom != AdaptType.rigid)
        {
            size_offset += new Vector3(0, (screen_size.y - ref_size.y) / 2, 0);
            pos_offset += new Vector3(0, (screen_size.y - ref_size.y) / 4, 0);
        }
        else if (top == AdaptType.rigid)
        {
            if (bottom != AdaptType.none)
            {
                Debug.LogError(TextData.GetText(400052));
                bottom = AdaptType.none;
            }

            pos_offset += new Vector3(0, (screen_size.y - ref_size.y) / 2, 0);
        }

        if (right == AdaptType.normal && left != AdaptType.rigid)
        {
            size_offset += new Vector3((screen_size.x - ref_size.x) / 2, 0, 0);
            pos_offset += new Vector3((screen_size.x - ref_size.x) / 4, 0, 0);
        }
        else if (right == AdaptType.rigid)
        {
            if (left != AdaptType.none)
            {
                Debug.LogError(TextData.GetText(400052));
                left = AdaptType.none;
            }

            pos_offset += new Vector3((screen_size.x - ref_size.x) / 2, 0, 0);
        }

        if (bottom == AdaptType.normal && top != AdaptType.rigid)
        {
            size_offset += new Vector3(0, (screen_size.y - ref_size.y) / 2, 0);
            pos_offset += new Vector3(0, -(screen_size.y - ref_size.y) / 4, 0);
        }
        else if (bottom == AdaptType.rigid)
        {
            if (top != AdaptType.none)
            {
                Debug.LogError(TextData.GetText(400052));
                top = AdaptType.none;
            }

            pos_offset += new Vector3(0, -(screen_size.y - ref_size.y) / 2, 0);
        }

        BoxCollider box = node.GetComponent<BoxCollider>();

        node.localPosition += pos_offset;
        box.size += size_offset;

        return box.size;
    }

    public void SelfAdaptItem(Transform root, int width)
    {
        BoxCollider box = root.GetComponent<BoxCollider>();

        if (box != null)
        {
            box.size = new Vector3(width, box.size.y, box.size.z);
        }

        Transform node = GetTrans(root, "bgs");
        UIWidget[] bgs = node.GetComponentsInChildren<UIWidget>();

        if (bgs != null)
        {
            for (int i = 0; i < bgs.Length; i++)
            {
                bgs[i].width = width;
            }
        }
    }

    #endregion activity_ui_window & task_ui_window ����Ӧ��Ļ

    
    /// <summary>
    /// 数量超过十万，折合成万为单位，lua使用
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public string GetShowNumberForLua(int num)
    {
        return GetShowNumber(num);
    }

    public string GetShowNumber(int num, int limitNum = 100000)
    {
        if (num < limitNum)
        {
            return num.ToString();
        }
        else
        {
            int TenThousand = num / 10000;
            return TenThousand + TextData.GetText(EnumText.ET_CardWan);
        }
    }

    private bool m_hasRecordTitlePos = false;
    private Vector3 m_guildPos, m_namePos, m_titlePos, m_touxianPos, m_taskItemPos;

    public void SetEffectColor(Transform trans, float colorr, float colorg, float colorb, float colora)
    {
        if (trans == null)
        {
            return;
        }
        var widget = trans.GetComponent<UILabel>();
        if (widget == null)
        {
            return;
        }
        widget.effectColor = new Color(colorr, colorg, colorb, colora);
    }

    public void ResetTweenAnim(Transform trans)
    {
        if (trans == null)
        {
            return;
        }
        UITweener[] animArray = trans.GetComponentsInChildren<UITweener>(true);
        for (int i = 0; i < animArray.Length; i++)
        {
            animArray[i].ResetToBeginning();
        }
    }

    public void PlayTweenAnim(Transform trans)
    {
        if (trans == null)
        {
            return;
        }
        UITweener[] animArray = trans.GetComponentsInChildren<UITweener>(true);
        for (int i = 0; i < animArray.Length; i++)
        {
            animArray[i].enabled = true;
        }
    }


    public static Vector3 WorldToScreenPos(Vector3 pos)
    {
        Camera camera = Camera.main;
        Vector3 screenPosition = camera.WorldToScreenPoint(pos);
        if (screenPosition.z < 0)
        {
            screenPosition.x = int.MaxValue;
        }
        else
        {
            screenPosition.x = screenPosition.x * EZFunWindowMgr.Instance.GetScreenHeight() / Screen.height;
            screenPosition.y = -(Screen.height - screenPosition.y);

            screenPosition.y = screenPosition.y * EZFunWindowMgr.Instance.GetScreenHeight() / Screen.height;
            screenPosition.z = 0;
        }
        return screenPosition;
    }


    public void PlayTweensInChildren(Transform trans, bool playOrNot = true)
    {
        if (trans == null || !trans.gameObject.activeSelf)
        {
            return;
        }

        UITweener[] tweenArray = trans.GetComponentsInChildren<UITweener>();

        if (tweenArray == null)
        {
            return;
        }

        for (int i = 0; i < tweenArray.Length; i++)
        {
            tweenArray[i].ResetToBeginning();
            tweenArray[i].Play(playOrNot);
        }
    }


    public void StopTweensInChildren(Transform trans)
    {
        if (trans == null || !trans.gameObject.activeSelf)
        {
            return;
        }
        UITweener[] tweenArray = trans.GetComponentsInChildren<UITweener>();
        if (tweenArray == null)
        {
            return;
        }
        for (int i = 0; i < tweenArray.Length; i++)
        {
            tweenArray[i].enabled = false;
        }
    }

    //Get transform named m_need_node_trans_name, if hash has this transform,return, or find the first transform, and storage it in hash
    Hashtable m_need_nodes_trans_hash = new Hashtable();
    public Transform GetTrans(string m_need_node_trans_name, bool forceInit)
    {
        Transform m_needNodeTrans = null;
        if (m_need_nodes_trans_hash.ContainsKey(m_need_node_trans_name) && !forceInit)
        {
            m_needNodeTrans = (Transform)m_need_nodes_trans_hash[m_need_node_trans_name];
        }
        else
        {
            if (transform != null)
                m_needNodeTrans = EZFunUITools.GetTrans(m_currentTrans, m_need_node_trans_name);
            if (m_needNodeTrans == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[Can not find transform]" + m_need_node_trans_name);
#endif
                m_need_nodes_trans_hash.Remove(m_need_node_trans_name);
            }
            else
            {
                if (m_need_nodes_trans_hash.ContainsKey(m_need_node_trans_name))
                {
                    m_need_nodes_trans_hash.Remove(m_need_node_trans_name);
                }
                m_need_nodes_trans_hash.Add(m_need_node_trans_name, m_needNodeTrans);
            }
        }
        return m_needNodeTrans;
    }

    public Transform GetTrans(string parentStr, string childStr)
    {
        return GetTrans(GetTrans(parentStr), childStr);
    }
 
    protected Transform _m_currentTrans = null;
    protected Transform m_currentTrans
    {
        get
        {
            if (_m_currentTrans == null)
            {
                _m_currentTrans = transform;
            }
            return _m_currentTrans;
        }
    }

    public void LoadUISprite(Transform trans, string iconName)
    {
        EZFunUITools.LoadUISprite(trans, iconName);
    }

    public virtual void SetActive(Transform trans, bool state)
    {
        if (trans == null)
            return;

        if (trans.gameObject.activeSelf != state)
        {
            NGUITools.SetActive(trans.gameObject, state);
        }
    }

    public void SetActiveChildren(Transform trans, bool state)
    {
        if (trans == null)
            return;

        for (int i = 0; i < trans.childCount; i++)
        {
            SetActive(trans.GetChild(i), state);
        }
    }

    public virtual void SetVisible(Transform trans, bool state)
    {
        if (trans == null)
            return;
        //如果有需要原位置信息 再包一层使用
        if (state)
        {
            SetActive(trans, true);
            trans.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            trans.localPosition = new Vector3(0, -3000, 0);
        }
    }


    public void SetSprite(string transName, string childName, string spriteName)
    {
        if (!string.IsNullOrEmpty(transName))
        {
            if (spriteName == null && !string.IsNullOrEmpty(childName))
            {
                SetSprite(GetTrans(transName), childName);
            }
            else
            {
                SetSprite(GetTrans(transName, childName), spriteName);
            }
        }
    }

    public void SetSprite(Transform trans, string name)
    {
        if (trans == null)
        {
#if UNITY_EDITOR
            Debug.LogError("SetSprite is null , " + name);
#endif
            return;
        }

        UISprite sprite = (UISprite)trans.GetComponent<UISprite>();
        if (sprite == null)
        {
            sprite = trans.GetComponentInChildren<UISprite>();
        }
        if (sprite == null || sprite.atlas == null || sprite.atlas.GetSprite(name) == null || !sprite.enabled)
        {
            LoadUISprite(trans, name);
        }
        else
        {
            if (sprite.spriteName != name)
                sprite.spriteName = name;
        }
    }

    //add by lezen 2017/6/19
    //设置字体的shadow颜色
    public void SetLabelShadow(Transform trans, Color color)
    {
        if (trans == null)
        {
#if UNITY_EDITOR
            Debug.LogError("SetLabelShadow is null , " + name);
#endif
            return;
        }
        UILabel uiLabel = trans.GetComponent<UILabel>();
        if (uiLabel != null)
        {
            uiLabel.effectStyle = UILabel.Effect.Shadow;
            uiLabel.effectColor = color;
        }
    }
    //add by lezen 2017/6/19
    //设置字体基础颜色
    public void SetLabelColor(Transform trans, Color color)
    {
        if (trans == null)
        {
#if UNITY_EDITOR
            Debug.LogError("SetLabelColor is null , " + name);
#endif
            return;
        }
        UILabel uiLabel = trans.GetComponent<UILabel>();
        if (uiLabel != null)
        {
            uiLabel.color = color;
        }
    }

    /// <summary>
    /// 将封装UISprite的MakePixelPerfect方法（还原默认尺寸）
    /// </summary>
    /// <param name="trans">UISprite的节点</param>
    public void MakePixelPerfect(Transform trans)
    {
        if (trans == null)
        {
#if UNITY_EDITOR
            Debug.LogError("SetSprite is null");
#endif
            return;
        }
        if (!NGUITools.GetActive(trans.gameObject))
        {
            return;
        }
        UISprite sprite = (UISprite)trans.GetComponent<UISprite>();
        if (sprite == null)
        {
            sprite = trans.GetComponentInChildren<UISprite>();
        }
        if (sprite == null)
        {
#if UNITY_EDITOR
            Debug.LogError("SetSprite is null");
#endif
            return;
        }
        sprite.MakePixelPerfect();
    }

    public virtual Transform GetTrans(string m_need_node_trans_name)
    {
        return GetTrans(m_need_node_trans_name, false);
    }

    public Transform GetTransByDepth(string m_need_node_trans_name, int depth)
    {
        Transform m_needNodeTrans = null;
        if (m_need_nodes_trans_hash.ContainsKey(m_need_node_trans_name))
        {
            m_needNodeTrans = (Transform)m_need_nodes_trans_hash[m_need_node_trans_name];
        }
        else
        {
            if (transform != null)
                m_needNodeTrans = EZFunUITools.GetTransByDepth(m_currentTrans, m_need_node_trans_name, depth);
            if (m_needNodeTrans == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[Can not find transform]" + m_need_node_trans_name);
#endif
                m_need_nodes_trans_hash.Remove(m_need_node_trans_name);
            }
            else
            {
                if (m_need_nodes_trans_hash.ContainsKey(m_need_node_trans_name))
                {
                    m_need_nodes_trans_hash.Remove(m_need_node_trans_name);
                }
                m_need_nodes_trans_hash.Add(m_need_node_trans_name, m_needNodeTrans);
            }
        }
        return m_needNodeTrans;
    }

    public Transform GetTrans(Transform m_parent_trans, string m_need_trans_name)
    {
        if (m_parent_trans == null)
        {
            return null;
        }
        Transform m_needTrans = null;
        m_needTrans = EZFunUITools.GetTrans(m_parent_trans, m_need_trans_name);

        return m_needTrans;
    }


    public void SetLabel(Transform trans, string txt)
    {
        SetLabel(trans, txt, true);
    }

    public void SetNum(Transform trans, int num)
    {
        string value = null;
        if (num > 10000)
        {
            value = ((int)(num / 10000)).ToString() + TextData.GetText(3159);
        }
        else
        {
            value = num.ToString();
        }
        SetLabel(trans, value, true);
    }

    public void SetLabel(string parentName, string childName, string content)
    {
        if (parentName != "")
        {
            if (content == null)
            {
                SetLabel(GetTrans(parentName), childName);
            }
            else if (childName != "")
            {
                SetLabel(GetTrans(parentName, childName), content);
            }
        }
    }

    public void SetLabel(Transform trans, string txt, bool mustActive)
    {
        if (trans == null)
        {
            return;
        }

        if (mustActive && !NGUITools.GetActive(trans.gameObject))
        {
            return;
        }

        UILabel uilabel = (UILabel)trans.GetComponent<UILabel>();
        if (uilabel == null)
        {
            uilabel = (UILabel)trans.GetComponentInChildren<UILabel>();
        }
        if (uilabel == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(txt))
        {
            uilabel.text = "";
            return;
        }
        txt = txt.Replace("\\n", "\n");
        if (uilabel.text != null && uilabel.ambigiousFont != null)
        {
            uilabel.text = txt;
            uilabel.UpdateNGUIText();
        }
    }

    public void SetLabel(Transform trans, string txt, params string[] stringArray)
    {
        SetLabel(trans, TextData.GetText(txt, stringArray));
    }

    public void SetLabel(Transform trans, int _int)
    {
        SetLabel(trans, _int.ToString());
    }

    public void SetFightLable(Transform trans, int fight)
    {
        SetLabel(trans, "[b]" + fight.ToString());
    }

    /// <summary>
    /// if txt is null, deactive label, otherwise active label
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="txt"></param>
    public void SetLabelActive(Transform trans, string txt)
    {
        SetLabel(trans, txt, false);
        SetActive(trans, txt != null);
    }


    //不尝试去设置，prefab上面就已经挂上去的组件，在打dll状态时GetComponent是获取不到改组件的
    //其代码不在当前dll的程序集里面， add by xiaolong
    public void SetChildTransQueue(Transform trans, int queue)
    {
        for (int i = 0; i < trans.childCount; i++)
        {

            Transform childTrans = trans.GetChild(i);
            if (childTrans.name.Contains("star"))
            {
                EZFunRenderQueueController queueController = EZFunTools.GetOrAddComponent<EZFunRenderQueueController>(childTrans.gameObject);
                if (queueController != null)
                {
                    queueController.m_rendererQueue = queue;
                }
            }
            SetChildTransQueue(childTrans, queue);
        }

    }

    public string GetStarString(int num)
    {
        string s = "";
        if (num <= 5)
        {
            for (int i = 0; i < num; i++)
            {
                s += "s";
            }
        }
        else
        {
            for (int i = 0; i < num - 5; i++)
            {
                s += "j";
            }
        }
        return s;
    }
    // Mingrui Jiang change
    // �����и��ӽڵ���������btn_title�Ļ������������ֶ�������
    public static string grayTitleStr = "btn_title";
    Dictionary<UIWidget, Color> m_grayWidgetDic = new Dictionary<UIWidget, Color>();
    public virtual void SetGray(Transform trans, bool gray, bool Untouched, bool touchInChild)
    {
        SetGrayWork(trans, gray, Untouched, touchInChild);
    }

    public void SetActiveTween(Transform trans, bool isActive)
    {
        if (trans == null)
        {
            return;
        }
        var tween = trans.GetComponent<UITweener>();
        if (tween != null)
        {
            tween.enabled = isActive;
        }
    }

    public virtual void SetGray(Transform trans, bool gray, bool Untouched = false)
    {
        SetGrayWork(trans, gray, Untouched, false);
    }

    void SetGrayWork(Transform trans, bool gray, bool untouchable, bool touchInChild)
    {
        if (trans == null)
        {
            return;
        }
        if (gray)
        {
            var widget = trans.GetComponents<UIWidget>();
            for (int i = 0; i < widget.Length; i++)
            {
                SetGray(widget[i], true);
            }
            UILabel btnTitle = trans.GetComponentInChildren<UILabel>();
            if (btnTitle != null)
            {
                //if (btnTitle.name.Contains(grayTitleStr))
                //{
                btnTitle.prefabEffectColor = btnTitle.effectColor;
                btnTitle.effectColor = new Color(24f / 255, 24f / 255, 24f / 255);
                btnTitle.applyGradient = false;
                //}
                //else if (GetTrans(btnTitle.transform, "_colorText") != null)
                //{
                //    btnTitle.effectColor = new Color(24f / 255, 24f / 255, 24f / 255);
                //    btnTitle.applyGradient = false;
                //}
            }

            UIWidget[] widgetArray = trans.GetComponentsInChildren<UIWidget>(true);
            for (int i = 0; i < widgetArray.Length; i++)
            {
                if (widgetArray[i].gameObject.activeInHierarchy == true)
                {
                    if (widgetArray[i].GetComponent<UILabel>() != null && widgetArray[i].name != "_colorText"
                        && widgetArray[i].transform.Equals(btnTitle.transform) == false)
                    {
                        SetGrayWork(widgetArray[i].transform, true, untouchable, false);
                    }
                    else
                    {
                        SetGray(widgetArray[i], true);
                    }
                }
            }
            if (untouchable)
            {
                SetUntouchable(trans, touchInChild);
            }
        }
        else
        {
            var widget = trans.GetComponents<UIWidget>();
            for (int i = 0; i < widget.Length; i++)
            {
                SetGray(widget[i], false);
            }
            UILabel btnTitle = trans.GetComponentInChildren<UILabel>();
            if (btnTitle != null)
            {
                btnTitle.effectColor = btnTitle.prefabEffectColor;
                btnTitle.applyGradient = true;
            }

            UIWidget[] widgetArray = trans.GetComponentsInChildren<UIWidget>(true);
            for (int i = 0; i < widgetArray.Length; i++)
            {
                SetGray(widgetArray[i], false);
            }
            if (untouchable)
            {
                Settouched(trans, touchInChild);
            }
        }
    }

    //设置是否可点击
    public void SetCanTouched(Transform trans, bool canTouch, bool touchInChild = false) 
    {
        if (canTouch)
        {
            Settouched(trans, touchInChild);
        }
        else 
        {
            SetUntouchable(trans, touchInChild);
        }
    }

    // ��������һ���ڵ�����������NOGRAY ��Ҫ����
    public void SetGray(UIWidget uiwidget, bool gray)
    {
        if (uiwidget == null)
        {
            return;
        }

        if (uiwidget.name.Contains("NOGRAY"))
        {
            return;
        }

        if (gray)
        {
            if (uiwidget.IsGrayNow)
            {
                return;
            }
            if (!m_grayWidgetDic.ContainsKey(uiwidget))
            {
                m_grayWidgetDic.Add(uiwidget, uiwidget.color);
            }
            //������ֱ����unity��font ��ô��ֱ������color�Ϳ����ˣ�������ngui��font ����ʹ��shader���ûҷ�ʽ
            if (uiwidget is UILabel)
            {
                var label = (uiwidget as UILabel);
                if (label.trueTypeFont != null)
                {
                    uiwidget.color = new Color(161f / 255, 161f / 255, 161f / 255);
                }
                else
                {
                    label.symbolStyle = NGUIText.SymbolStyle.Colored;
                    uiwidget.color = new Color(0, 0, 0);
                }
            }
            else
            {
                uiwidget.color = new Color(0, 0, 0);
            }
            uiwidget.IsGrayNow = true;
        }
        else
        {
            if (!uiwidget.IsGrayNow)
            {
                return;
            }
            if (m_grayWidgetDic.ContainsKey(uiwidget))
            {
                uiwidget.color = m_grayWidgetDic[uiwidget];
                m_grayWidgetDic.Remove(uiwidget);
            }
            else
            {
                uiwidget.color = Color.white;
            }
            uiwidget.IsGrayNow = false;
        }
    }

    public void Settouched(Transform trans, bool inchild = false)
    {
        if (trans == null)
            return;
        if (inchild)
        {
            Collider[] boxes = trans.GetComponentsInChildren<Collider>();
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].enabled = true;
            }
        }
        else
        {
            Collider box = (Collider)trans.GetComponent<Collider>();
            if (box != null)
                box.enabled = true;
        }
    }

    public void SetUntouchable(Transform trans, bool inchild = false)
    {
        if (trans == null)
            return;
        if (inchild)
        {
            Collider[] boxes = trans.GetComponentsInChildren<Collider>();
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].enabled = false;
            }
        }
        else
        {
            Collider box = (Collider)trans.GetComponent<Collider>();
            if (box != null)
                box.enabled = false;
        }
    }

    public void SetFillSprite(Transform trans, float fillValue,bool mustActive = true)
    {
        if (trans == null)
        {
            return;
        }

        if (!NGUITools.GetActive(trans.gameObject) && mustActive)
        {
            return;
        }

        UISprite uisprite = (UISprite)trans.GetComponent<UISprite>();
        if (uisprite == null)
        {
            uisprite = trans.GetComponentInChildren<UISprite>();
        }
        if (uisprite == null)
        {
#if UNITY_EDITOR
            Debug.LogError(trans.name + "[Does not have UISprite component]");
#endif
            return;
        }
        if (uisprite.fillAmount != fillValue)
        {
            uisprite.fillAmount = fillValue;
        }
    }

    public void SetColor(Transform trans, Color color)
    {
        UIWidget widget = trans.GetComponent<UIWidget>();
        if (widget != null)
        {
            widget.color = new Color(color.r, color.g, color.b, color.a);
        }
        UIWidget[] widgetArray = trans.GetComponentsInChildren<UIWidget>();
        for (int i = 0; i < widgetArray.Length; i++)
        {
            widgetArray[i].color = new Color(color.r, color.g, color.b, color.a);
        }
    }

    public void SetColor(Transform trans, String colorStr)
    {
        if (trans == null || string.IsNullOrEmpty(colorStr))
            return;

        Color color = EZFunUITools.HexToColor(colorStr);
        UIWidget widget = trans.GetComponent<UIWidget>();
        if (widget != null)
        {
            widget.color = new Color(color.r, color.g, color.b, color.a);
        }
        UIWidget[] widgetArray = trans.GetComponentsInChildren<UIWidget>();
        for (int i = 0; i < widgetArray.Length; i++)
        {
            widgetArray[i].color = new Color(color.r, color.g, color.b, color.a);
        }
    }

    public Color GetColor(Transform trans)
    {
        if (trans == null)
        {
            return new Color(1, 1, 1, 1);
        }
        UIWidget widget = trans.GetComponent<UIWidget>();
        if (widget != null)
        {
            return widget.color;
        }
        UIWidget widgetArray = trans.GetComponentInChildren<UIWidget>();
        if (widgetArray != null)
        {
            return widgetArray.color;
        }
        return new Color(1, 1, 1, 1);
    }

    public void SetSlider(Transform trans, float sliderValue, Transform pointTrans = null)
    {
        if (trans == null)
        {
#if UNITY_EDITOR
            Debug.LogError("[Transform is null]");
#endif
            return;
        }

        UISlider uiSlider = (UISlider)trans.GetComponent<UISlider>();
        if (uiSlider == null)
        {
            uiSlider = trans.GetComponentInChildren<UISlider>();
        }
        if (uiSlider == null)
        {
            trans.localScale = new Vector3(sliderValue, trans.localScale.y, trans.localScale.z);
            if (pointTrans != null)
            {
                if (sliderValue <= 0.0001F)
                {
                    SetActive(pointTrans, false);
                }
                else
                {
                    SetActive(pointTrans, true);
                }
                var sprite = trans.GetComponent<UISprite>();
                if (sprite == null)
                {
                    return;
                }
                var pos = new Vector3((trans.localPosition.x + sprite.width * trans.localScale.x) < 0 ?
                    0 : (trans.localPosition.x + sprite.width * trans.localScale.x),
                    trans.localPosition.y,
                    trans.localPosition.z);
                var parent = pointTrans.parent;
                pointTrans.parent = trans.parent;
                pointTrans.localPosition = pos;
                pointTrans.parent = parent;
            }
            return;
        }
        else
        {
            if (sliderValue == 0)
            {
                SetActive(uiSlider.foregroundWidget.transform, false);
            }
            else
            {
                SetActive(uiSlider.foregroundWidget.transform, true);
            }

            uiSlider.value = sliderValue;
        }
    }

    public void SetAllActive(Transform trans, bool active = true)
    {
        Activate(trans, active);
    }

    void Activate(Transform trans, bool active = true)
    {
        if (trans.name != "defenseEnergy")
        {
            trans.gameObject.SetActive(active);
            for (int i = 0, imax = trans.childCount; i < imax; ++i)
            {
                Transform child = trans.GetChild(i);
                Activate(child, active);
            }
        }
    }

    public void ShowChild(Transform parent, string contain_name, bool show_or_hide = true)
    {
		if (parent == null) 
		{
			Debug.LogError ("ShowChild get null transform, child name is " + contain_name);
			return;
		}
        Transform child;
        for (int i = 0; i < parent.childCount; i++)
        {
            child = parent.GetChild(i);

            if (child != null)
            {
                if (child.name.Contains(contain_name))
                {
                    SetActive(child, show_or_hide);
                }
                else
                {
                    SetActive(child, !show_or_hide);
                }
            }
        }
    }

    public void ShowChild(Transform parent, List<string> child_nameList, bool show_or_hide = true)
    {
        Transform child;
        for (int i = 0; i < parent.childCount; i++)
        {
            child = parent.GetChild(i);

            if (child != null)
            {
                if (child_nameList.Contains(child.name))
                {
                    SetActive(child, show_or_hide);
                }
                else
                {
                    SetActive(child, !show_or_hide);
                }
            }
        }
    }

    public virtual void SetClickable(Transform trans, bool state)
    {
        if (trans == null)
        {
#if UNITY_EDITOR
            Debug.LogError("[Transform is null]");
#endif
            return;
        }

        UIButtonMessage btnMessage = (UIButtonMessage)trans.GetComponent<UIButtonMessage>();
        if (btnMessage == null)
        {
            btnMessage = trans.GetComponentInChildren<UIButtonMessage>();
        }

        if (btnMessage == null)
        {
            return;
        }
        else
        {
            Collider col = (Collider)btnMessage.transform.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = state;
            }
        }
    }

    public static void ShowConfirm(string content, HandleErrorWindow.CallBack okAction = null,
        HandleErrorWindow.CallBack cancelAction = null)
    {
        HandleErrorWindow.m_contentStr = content;
        HandleErrorWindow.m_okCallBack = okAction;
        HandleErrorWindow.m_noCallBack = cancelAction;
        EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.error_ui_window, true, 1);
    }

    static public void ShowTips(string tip)
    {

        if (!String.IsNullOrEmpty(tip))
        {
            HandleErrTipsWindow.m_contentStr = tip;
            EZFunWindowMgr.Instance.SetWindowStatus(EZFunWindowEnum.err_tips_ui_window, RessType.RT_CommonWindow);
        }
    }

    static public void ShowTips(int textID, params string[] strParam)
    {
        ShowTips(TextData.GetText(textID, strParam));
    }

    #region anim

    /// 新加了一个方法，研究了UITweener后，能保证一定从isForward=true，一定是从from到to  isForward=false一定是从to到From
    /// </summary>
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="isForward"></param>
    /// <param name="callBack"></param>
    protected void PlayForceAnimation(Transform trans, EventDelegate.Callback callBack = null, bool isForward = true, bool isPlay = true, Type type = null)
    {
        EZFunUITools.PlayForceAnimation(trans, callBack, isForward, isPlay, type);
    }

    /// 新加了一个方法，研究了UITweener后，能保证一定从isForward=true，一定是从from到to  isForward=false一定是从to到From
    /// </summary>
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="isForward"></param>
    /// <param name="callBack"></param>
    protected void PlayForceAnimationAndActive(Transform trans, EventDelegate.Callback callBack = null, bool isForward = true, bool isPlay = true, Type type = null)
    {
        EZFunUITools.PlayForceAnimation(trans, callBack, isForward, isPlay, type, true);
    }

    public void PlayForceAnimation_Lua(Transform trans, string luaCallBack = null, bool isForward = true, bool isPlay = true)
    {
        PlayForceAnimation(trans, () =>
        {
            if (!string.IsNullOrEmpty(luaCallBack))
            {
                WindowBaseLua.m_luaMgr.CallLuaFunction(luaCallBack);
            }
        }, isForward, isPlay);
    }

    public void PlayForceAnimation_Lua(Transform trans, LuaInterface.LuaFunction luaCallBack = null, bool isForward = true, bool isPlay = true)
    {
        PlayForceAnimation(trans, () =>
        {
            if (luaCallBack != null)
            {
                luaCallBack.Call();
                luaCallBack = null;
            }
        }, isForward, isPlay);
    }

    protected void PlayTweenCallBack(Transform trans, EventDelegate.Callback callBack, bool isForward = true, bool isPlay = true, bool includeChidren = false)
    {
        EZFunUITools.PlayForceAnimation(trans, callBack, isForward, isPlay, null, false, -1, includeChidren);
    }
    #endregion

    #region base function
    public Transform LoadFx(string str, Transform trans = null, bool isAddParticleMask = false)
    {
        return LoadFxWork(str, trans, Vector3.one, Vector3.zero, isAddParticleMask);
    }

    public void AddParticleMask(Transform trans)
    {
        if(trans == null)
        {
            return;
        }
        EZFunTools.GetOrAddComponent<EZFunParticleMask>(trans.gameObject);
    }

    public Transform LoadFxWork(string str, Transform trans = null,
        Vector3 scala = default(Vector3),
        Vector3 localposition = default(Vector3), bool isAddMask = false)
    {
        Transform newTrans = null;
        if (trans == null)
        {
            newTrans = GetTrans(str, true);
            trans = this.transform;
        }
        else
        {
            newTrans = GetTrans(trans, str);
        }
        if (newTrans == null)
        {
            var gb = ResourceMgr.GetInstantiateAsset(RessType.RT_Emoji, str) as GameObject;
            if (gb == null)
            {
                return null;
            }
            gb.name = str;
            WindowRoot.SetLayer(gb, this.gameObject.layer);
            gb.transform.parent = trans;
            gb.transform.localPosition = localposition;
            gb.transform.localScale = scala == default(Vector3) ? Vector3.one : scala;
            newTrans = gb.transform;
        }
        if (isAddMask)
        {
            if (newTrans != null)
            {
                var mask = EZFunTools.GetOrAddComponent<EZFunParticleMask>(newTrans.gameObject);
            }
        }
        return newTrans;
    }

    #endregion

    public Vector2 GetUIWidgetLocalSize(Transform tran = null)
    {
        UIWidget uiw = null;
        if (tran == null)
        {
            uiw = this.GetComponent<UIWidget>();
        }
        else
        {
            uiw = tran.GetComponent<UIWidget>();
        }
        if (uiw == null)
        {
            return new Vector2();
        }
        else
        {
            return uiw.localSize;
        }
    }

    public void RepositionGrid(Transform trans, bool now = false)
    {
        UIGrid grid= trans.GetComponent<UIGrid>();
        if (grid == null)
        {
            grid = trans.GetComponentInChildren<UIGrid>(true);
        }

        if (grid == null)
        {
            return;
        }

        if (now)
        {
            grid.Reposition();
        }
        else
        {
            grid.repositionNow = true;
        }
    }

    #region depth  
    public void SetDepth(Transform trans,int depth)
    {
        if(trans == null)
        {
            return;
        }
        UIWidget[] widgets = trans.GetComponents<UIWidget>();
        for(int i= 0; i < widgets.Length; i ++)
        {
            widgets[i].depth = depth;
        }
    }

	public void ChangeDepth(Transform trans, int offset)
	{
		if(trans == null)
		{
			return;
		}
		UIWidget[] widgets = trans.GetComponentsInChildren<UIWidget>();
		for(int i= 0; i < widgets.Length; i ++)
		{
			widgets[i].depth += offset;
		}
	}

    #endregion
}
