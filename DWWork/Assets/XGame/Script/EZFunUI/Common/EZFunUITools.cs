using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;
using System;

public enum PlayerAnimatorType
{
    All,
    Simple,
}
public class EZFunUITools
{
    static Stack<Transform> m_transStack = new Stack<Transform>();
    //Get the first transform named m_need_trans_name in node m_parent_trans
    public static Transform GetTrans(Transform parentTrans, string needTransName)
    {
        if (parentTrans == null || string.IsNullOrEmpty(needTransName))
        {
            return parentTrans;
        }
        //由递归调用改成栈
        m_transStack.Clear();
        m_transStack.Push(parentTrans);
        Transform trans = null;
        while (m_transStack.Count > 0)
        {
            trans = m_transStack.Pop();
            var cTrans = trans.Find(needTransName);
            if (cTrans != null)
            {
                return cTrans;
            }
            for (int i = trans.childCount - 1; i >= 0; i--)
            {
                m_transStack.Push(trans.GetChild(i));
            }
        }
        return null;
    }



    /// <summary>
    /// 查找指定层级就不查了
    /// </summary>
    /// <param name="parentTrans"></param>
    /// <param name="needTransName"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public static Transform GetTransByDepth(Transform parentTrans, string needTransName, int depth = 1)
    {
        if (parentTrans == null)
        {
            return null;
        }
        if (parentTrans.name == needTransName)
        {
            return parentTrans;
        }
        var cTrans = parentTrans.Find(needTransName);
        if (cTrans != null)
        {
            return cTrans;
        }
        if (depth > 0)
        {
            for (int i = 0; i < parentTrans.childCount; i++)
            {
                cTrans = parentTrans.GetChild(i);

                var trans = GetTransByDepth(cTrans, needTransName, depth - 1);
                if (trans != null)
                {
                    return trans;
                }
            }
        }
        return null;
    }


    public static void SetSprite(Transform trans, string name)
    {
        LoadUISprite(trans, name);
        //UISprite sprite = trans.GetComponent<UISprite>();

        //if (sprite == null)
        //{
        //    sprite = trans.GetComponentInChildren<UISprite>();
        //}

        //sprite.spriteName = name;
        //return sprite;
    }

    //适用与npc 与ui界面的模型处理，战斗中副将模型等不适用
    static public bool IsNeedChangeAnimator(string modelName)
    {
        return modelName.CompareTo("monster_zixia") != 0;
    }


    public static void SetTextMesh(Transform trans, string txt, bool IgnoreActive = false, bool outLine = false)
    {
        if (trans == null)
            return;

        if (!NGUITools.GetActive(trans.gameObject) && !IgnoreActive)
        {
            return;
        }

        TextMesh textMesh = (TextMesh)trans.GetComponent<TextMesh>();
        if (textMesh == null)
        {
            //CDebug.LogWarning(trans.name + "[Does not have UILabel component, Try find in child]");
            textMesh = (TextMesh)trans.GetComponentInChildren<TextMesh>();
        }
        if (textMesh == null)
            return;

        txt = txt.Replace("\\n", "\n");
        // textMesh.font = CUIRessLoadSys.Instance.GetFont("ARIALN");
        textMesh.text = txt;

        if (outLine)
        {
            Transform outLineTrans = GetTrans(textMesh.transform, "TextMeshOutline");
            string tempStr = System.Text.RegularExpressions.Regex.Replace(txt, @"\<color=#[0123456789abcdef]{6}>", "");
            string noColorStr = tempStr.Replace("</color>", "");
            if (outLineTrans == null)
            {
                GameObject gb = ResourceMgr.GetInstantiateAsset(RessType.RT_UIItem, "TextMeshOutline") as GameObject;
                NGUITools.SetLayer(gb, textMesh.transform.gameObject.layer);
                outLineTrans = gb.transform;
                SetActive(outLineTrans, true);
                outLineTrans.parent = textMesh.transform;
                outLineTrans.name = "TextMeshOutline";
                outLineTrans.localPosition = Vector3.zero;
                outLineTrans.localScale = Vector3.one;
                outLineTrans.localEulerAngles = Vector3.zero;

                for (int i = 0; i < outLineTrans.childCount; i++)
                {
                    TextMesh tx = outLineTrans.GetChild(i).GetComponent<TextMesh>();

                    tx.font = textMesh.font;
                    tx.fontSize = textMesh.fontSize;
                    tx.characterSize = textMesh.characterSize;
                    tx.anchor = textMesh.anchor;
                    tx.alignment = textMesh.alignment;
                    tx.fontStyle = textMesh.fontStyle;
                    tx.text = noColorStr;
                }
            }
            else
            {
                for (int i = 0; i < outLineTrans.childCount; i++)
                {
                    TextMesh tx = outLineTrans.GetChild(i).GetComponent<TextMesh>();
                    tx.text = noColorStr;
                }
            }
        }

    }

    public static void SetLabel(Transform trans, string txt)
    {
        if (trans == null)
        {
            Debug.LogError("[Transform is null]");
            return;
        }

        UILabel uilabel = trans.GetComponent<UILabel>();
        if (uilabel == null)
        {
            UILabel[] uilabels = trans.GetComponentsInChildren<UILabel>(true);
            if (uilabels.Length > 0)
            {
                uilabel = uilabels[0];
            }
        }
        if (uilabel == null || uilabel.ambigiousFont != null)
        {
            Debug.LogError(trans.name + "[Does not have UILabel component]");
            return;
        }

        uilabel.text = txt;
        uilabel.UpdateNGUIText();
    }

    public static void SetLabel(Transform trans, int _int)
    {
        SetLabel(trans, _int.ToString());
    }

    public static void SetLabel(Transform trans, string fontName, int fontSize, string text)
    {
        if (trans == null)
        {
            Debug.LogError("[Transform is null]");
            return;
        }

        if (!NGUITools.GetActive(trans.gameObject))
        {
            //			return;
        }

        UILabel uilabel = trans.GetComponent<UILabel>();
        if (uilabel == null)
        {
            //			Debug.LogWarning(trans.name + "[Does not have UILabel component, Try  in child]");
            uilabel = trans.GetComponentInChildren<UILabel>();
        }
        if (uilabel == null)
        {
            Debug.LogError(trans.name + "[Does not have UILabel component]");
            return;
        }
        uilabel.bitmapFont = TextureManager.Instance.GetUIFont(fontName);
        uilabel.fontSize = fontSize;
        uilabel.text = text;
    }

    public static void SetFillSprite(Transform trans, float fillValue)
    {
        if (trans == null)
        {
            //	Debug.LogError("[Transform is null]");
            return;
        }

        if (!NGUITools.GetActive(trans.gameObject))
        {
            //			return;
        }

        UISprite uisprite = trans.GetComponent<UISprite>();
        if (uisprite == null)
        {
            //			Debug.LogWarning(trans.name + "[Does not have UISprite component, Try find in child]");
            uisprite = trans.GetComponentInChildren<UISprite>();
        }
        if (uisprite == null)
        {
            Debug.LogError(trans.name + "[Does not have UISprite component]");
            return;
        }

        uisprite.fillAmount = fillValue;
    }

    public static void SetStar(Transform starTrans, Transform moonTrans, int star)
    {
        List<Transform> starTransList = sortTransChildsByName(starTrans);
        List<Transform> moonTransList = sortTransChildsByName(moonTrans);

        SetActive(starTrans, true);
        if (star <= 5)
        {
            SetActive(moonTrans, false);
            for (int i = 0; i < star; i++)
            {
                SetActive(GetTrans(starTransList[i], "starlight"), true);
                SetActive(GetTrans(starTransList[i], "starBack"), false);
            }
            for (int i = star; i < 5; i++)
            {
                SetActive(GetTrans(starTransList[i], "starBack"), true);
                SetActive(GetTrans(starTransList[i], "starlight"), false);
            }
        }
        else
        {
            SetActive(moonTrans, true);
            for (int i = 0; i < star - 5; i++)
            {
                SetActive(moonTransList[i], true);
                SetActive(starTransList[i], false);
            }
            for (int i = star - 5; i < 5; i++)
            {
                SetActive(starTransList[i], true);
                SetActive(GetTrans(starTransList[i], "starBack"), false);
                SetActive(moonTransList[i], false);
            }
        }
    }
    public static List<Transform> sortTransChildsByName(Transform trans)
    {
        List<Transform> childTransList = new List<Transform>();
        for (int i = 0; i < trans.childCount; i++)
        {
            childTransList.Add(trans.GetChild(i));
        }
        childTransList.Sort(delegate (Transform x, Transform y)
        {
            return x.name.CompareTo(y.name);
        });

        return childTransList;
    }
    public static void SetActive(Transform trans, bool state)
    {
        if (trans == null)
        {
            return;
        }
        NGUITools.SetActive(trans.gameObject, state);
    }

    public static void SetActive(GameObject ob, bool state)
    {
        if (ob == null)
        {
            return;
        }
        NGUITools.SetActive(ob, state);
    }

    //要忽略计算rect的项目名称
    public static string ignoreRect = "ignoreRect";
    //获取以localPosition坐标为基础，当前节点与子节点里所有UIWidget所占的矩形区域
    //例如localposition为【1000,1000】，宽高各100，rect就是x,y范围[950,1050]的区域，坐标基于父节点
    public static Rect GetRect(Transform trans)
    {
        UIWidget[] box_list = trans.GetComponentsInChildren<UIWidget>();
        float left = 65536, right = -65536, top = -65536, bottom = 65536;
        float tmp_left = 0, tmp_right = 0, tmp_top = 0, tmp_bottom = 0;
        float center_x = 0, center_y = 0;
        Transform tmp = null;

        foreach (var k in box_list)
        {
            //特殊要求 scrollView经常有尺寸太大的底图 撑大界面 不好调整位置
            if (k.name.Contains(ignoreRect))
                continue;

            tmp = k.transform;
            center_x = tmp.localPosition.x;
            center_y = tmp.localPosition.y;
            while (tmp != trans)
            {
                tmp = tmp.parent;
                center_x += tmp.localPosition.x;
                center_y += tmp.localPosition.y;
            }

            if (k.pivot == UIWidget.Pivot.Bottom
                || k.pivot == UIWidget.Pivot.BottomLeft
                || k.pivot == UIWidget.Pivot.BottomRight)
            {
                tmp_top = center_y + k.height;
                tmp_bottom = center_y;
            }
            else if (k.pivot == UIWidget.Pivot.Top
                     || k.pivot == UIWidget.Pivot.TopLeft
                     || k.pivot == UIWidget.Pivot.TopRight)
            {
                tmp_top = center_y;
                tmp_bottom = center_y - k.height;
            }
            else
            {
                tmp_top = center_y + k.height / 2f;
                tmp_bottom = center_y - k.height / 2f;
            }

            if (k.pivot == UIWidget.Pivot.Left
                || k.pivot == UIWidget.Pivot.TopLeft
                || k.pivot == UIWidget.Pivot.BottomLeft)
            {
                tmp_left = center_x;
                tmp_right = center_x + k.width;
            }
            else if (k.pivot == UIWidget.Pivot.Right
                     || k.pivot == UIWidget.Pivot.TopRight
                     || k.pivot == UIWidget.Pivot.BottomRight)
            {
                tmp_left = center_x - k.width;
                tmp_right = center_x;
            }
            else
            {
                tmp_left = center_x - k.width / 2f;
                tmp_right = center_x + k.width / 2f;
            }

            if (tmp_top > top)
            {
                top = tmp_top;
            }
            if (tmp_bottom < bottom)
            {
                bottom = tmp_bottom;
            }
            if (tmp_right > right)
            {
                right = tmp_right;
            }
            if (tmp_left < left)
            {
                left = tmp_left;
            }
        }

        //rect的yMax在下边，Rect.MinMaxRect里的top其实就是bottom
        return Rect.MinMaxRect(left, bottom, right, top);
    }
    public static void ShowAllSameNameTrans(Transform parentTrans, string needTransName, bool isShow)
    {
        for (int i = 0; i < parentTrans.childCount; i++)
        {
            Transform childTrans = parentTrans.GetChild(i);
            if (childTrans.name.Equals(needTransName))
            {
                SetActive(childTrans, isShow);
            }
        }

        for (int i = 0; i < parentTrans.childCount; i++)
        {
            Transform childTrans = parentTrans.GetChild(i);
            ShowAllSameNameTrans(childTrans, needTransName, isShow);
        }
    }

    public static Transform GetActiveChildTrans(Transform parentTrans, string transName)
    {
        for (int i = 0; i < parentTrans.childCount; i++)
        {
            Transform childTrans = parentTrans.GetChild(i);
            if (NGUITools.GetActive(childTrans.gameObject) && childTrans.name.Equals(transName))
            {
                return childTrans;
            }
        }

        for (int i = 0; i < parentTrans.childCount; i++)
        {
            Transform childTrans = parentTrans.GetChild(i);

            Transform getTrans = GetActiveChildTrans(childTrans, transName);
            if (getTrans != null)
            {
                return getTrans;
            }
        }
        return null;
    }



    public static string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, a);
    }

    public static void QuadLoadUISprite(Transform trans, string iconName)
    {
        EZFunMeshSprite meshSprite = EZFunTools.GetOrAddComponent<EZFunMeshSprite>(trans.gameObject);
        if (meshSprite)
        {
            bool needFindAtlas = true;
            if (meshSprite.atlas != null)
            {
                UISpriteData sd = meshSprite.atlas.GetSprite(iconName);
                if (sd != null)
                {
                    needFindAtlas = false;
                }
            }

            if (needFindAtlas)
            {
                meshSprite.atlas = TextureManager.Instance.FindAtlasOwnsSprite(iconName);
            }

            meshSprite.spriteName = iconName;
        }
    }

    public static void LoadUISprite(Transform trans, string iconName)
    {
        if (string.IsNullOrEmpty(iconName) || trans == null)
        {
            return;
        }

        //现在图集会拆成小图 防止内存过大
        //这里方法 一般是加载小图标 所以优先监测小图
        UITexture uitex = trans.GetComponent<UITexture>();
        UISprite uiSpri = trans.GetComponent<UISprite>();
        if (uitex == null && uiSpri == null)
        {
            return;
        }

        Texture texture = TextureManager.Instance.LoadTexture(iconName);
        if (texture != null)
        {
            if (uitex == null)
            {
                uitex = trans.gameObject.AddComponent<UITexture>();
                uitex.depth = uiSpri.depth;
                uitex.width = uiSpri.width;
                uitex.height = uiSpri.height;
            }

            uitex.mainTexture = texture;
            uitex.enabled = false;
            uitex.enabled = true;

            if (uiSpri != null)
                uiSpri.enabled = false;
        }
        else
        {
            if (uiSpri == null)
            {
                uiSpri = trans.gameObject.AddComponent<UISprite>();
                uiSpri.depth = uitex.depth;
                uiSpri.width = uitex.width;
                uiSpri.height = uitex.height;
            }

            if (uitex != null)
                uitex.enabled = false;

            uiSpri.enabled = true;
            TextureManager.Instance.LoadAtlasToSprite(uiSpri, iconName);
            uiSpri.spriteName = iconName;
        }
    }

    public static Transform FindWindowTrans(Transform m_rootTrans)
    {
        Transform trans = m_rootTrans;
        while (trans.parent != null)
        {
            if (trans.name.Contains("ui_window") && trans.GetComponent<WindowRoot>() != null)
            {
                return trans;
            }
            trans = trans.parent;
        }
        return trans;
    }

    public static Color GetColorByStr(string strColor)
    {
        Color color = new Color(1, 1, 1);
        string[] splitArray = strColor.Split(',');
        if (splitArray.Length == 3)
        {
            int r = int.Parse(splitArray[0]);
            int g = int.Parse(splitArray[1]);
            int b = int.Parse(splitArray[2]);
            color = new Color(r / 255f, g / 255f, b / 255f);
        }

        return color;
    }

    public static string GetColorBBCodeByStr(string strColor, bool isNGUI = true)
    {
        string colorStr = "";
        string[] splitArray = strColor.Split(',');
        if (splitArray.Length == 3)
        {
            int r = int.Parse(splitArray[0]);
            int g = int.Parse(splitArray[1]);
            int b = int.Parse(splitArray[2]);
            string rStr = String.Format("{0:X2}", r);
            string gStr = String.Format("{0:X2}", g);
            string bStr = String.Format("{0:X2}", b);
            if (isNGUI)
                colorStr = "[" + rStr + gStr + bStr + "]";
            else
                colorStr = "<c=" + rStr + gStr + bStr + ">";
        }

        return colorStr;
    }

    public static void SetParent(Transform pTrans, Transform trans)
    {
        if (trans != null && pTrans != null)
        {
            trans.parent = pTrans;
            trans.localPosition = Vector3.zero;
            trans.localEulerAngles = Vector3.zero;
            trans.localScale = Vector3.one;
        }
    }

    public static void SetPopupPos(Transform trans, float startY)
    {
        Transform dropDownList = EZFunUITools.GetTrans(trans, "Drop-down List");
        if (dropDownList != null)
        {
            UILabel[] labels = dropDownList.GetComponentsInChildren<UILabel>();
            int height = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                height += labels[i].height;
            }
            dropDownList.localPosition = new Vector3(dropDownList.localPosition.x, startY + height, dropDownList.localPosition.z);
        }
    }




    static public void ShowUIModelStand(Transform trans)
    {
        if (null == trans || null == trans.gameObject)
        {
            return;
        }

        //var animMgr = EZFunTools.GetOrAddComponent<AnimMgr>(trans.gameObject);
        //if (null != animMgr)
        //{
        //    animMgr.PlayUIStand();
        //}
    }

    public static void SetSpriteFromSelfAtlas(Transform trans, string name)
    {
        if (name == null)
        {
            name = "";
        }

        if (trans == null)
        {
            //CDebug.LogError("SetSprite is null , " + name);
            return;
        }

        UISprite sprite = (UISprite)trans.GetComponent<UISprite>();

        if (sprite == null)
        {
            sprite = trans.GetComponentInChildren<UISprite>();
        }

        if (sprite == null)
        {
            //CDebug.LogError("Sprite is null");
            return;
        }

        if (sprite.atlas == null || sprite.atlas.GetSprite(name) == null)
        {
            //CDebug.LogError("[no such sprite]" + name);
            sprite.spriteName = name;
            return;
        }
        else if (sprite.spriteName != name)
        {
            sprite.spriteName = name;
        }
    }

    public static string ReadableStrFromTime(int start)
    {
        long now = TimerSys.Instance.GetCurrentDateTimeLong();
        int mouth = 60 * 60 * 24 * 7 * 4;
        int week = 60 * 60 * 24 * 7;
        int day = 60 * 60 * 24;
        int hour = 60 * 60;
        int minute = 60;
        string timeStr = "";
        int time = (int)now - start;
        if (time / mouth >= 1)
        {
            timeStr = (time / mouth).ToString() + TextData.GetText(400269);
        }
        else if (time / week >= 1)
        {
            timeStr = (time / week).ToString() + TextData.GetText(400270);
        }
        else if (time / day >= 1)
        {
            timeStr = (time / day).ToString() + TextData.GetText(400271);
        }
        else if (time / hour >= 1)
        {
            timeStr = (time / hour).ToString() + TextData.GetText(400272);
        }
        else if (time / minute >= 1)
        {
            timeStr = (time / minute).ToString() + TextData.GetText(400273);
        }
        else
        {
            if (time <= 0)
            {
                timeStr = TextData.GetText(400274);
            }
            else
            {
                timeStr = time.ToString() + TextData.GetText(400275);
            }
        }

        return timeStr;
    }

    #region Tween Anim
    public static void PlayForceAnimation(Transform trans, EventDelegate.Callback callBack = null,
        bool isForward = true, bool isPlay = true, Type type = null, bool isForceActive = false, int group = -1, bool includeChidren = false)
    {
        if (trans == null)
        {
            if (callBack != null)
            {
                callBack();
            }
            return;
        }
        UITweener[] twPosition;
        if (includeChidren)
        {
            twPosition = trans.GetComponentsInChildren<UITweener>();
        }
        else
        {
            twPosition = trans.GetComponents<UITweener>();
        }
        if (twPosition == null || twPosition.Length == 0)
        {
            if (callBack != null)
            {
                callBack();
            }
            return;
        }

        bool isFirst = false;
        for (int i = 0; i < twPosition.Length; i++)
        {
            if ((type != null && (twPosition[i].GetType() != type)) || (twPosition[i].tweenGroup != group && group != -1))
            {
                continue;
            }
            if (isForceActive)
            {
                twPosition[i].enabled = true;
            }
            if (!isFirst)
            {
                isFirst = true;
                twPosition[i].onFinished.Clear();
                if (callBack != null)
                {
                    var del = new EventDelegate(callBack);
                    del.oneShot = true;
                    twPosition[i].onFinished.Add(del);
                }
            }
            //原来方向为反方向，那么先逆转一下
            if ((twPosition[i].amountPerDelta <= 0 && isForward) || (twPosition[i].amountPerDelta > 0 && !isForward))
            {
                twPosition[i].Toggle();
            }
            twPosition[i].ResetToBeginning();

            if (!isPlay)
            {
                //不需要播放过程，只需要最后的结果
                if (isForward)
                {
                    twPosition[i].tweenFactor = 1;
                }
                else
                {
                    twPosition[i].tweenFactor = 0;
                }
            }
            twPosition[i].Play(isForward);
        }
        if (!isFirst)
        {
            if (callBack != null)
            {
                callBack();
            }
        }

    }

    public static void PlayTweenCallBack(Transform trans, EventDelegate.Callback callBack = null, bool isForward = true, bool isPlay = true)
    {
        PlayForceAnimation(trans, callBack, isForward, isPlay);
    }
    #endregion
}