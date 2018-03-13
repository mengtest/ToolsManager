using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ezfun_resource;
using System.IO;
using LitJson;

public enum RESTYPE
{
    none,
    zn,
    th,
    en,
    vi,
    tw,
    kr
}


class XUIEditor : EditorWindow
{
    [MenuItem("Local/运行游戏")]
    static void RunGame()
    {
        EditorApplication.OpenScene("Assets/XGame/Scene/Release/GameRoot.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Local/XUIEditor")]
    static void AddXUIEditor()
    {
        Rect re = new Rect(500, 500, 500, 300);
        var win = EditorWindow.GetWindow<XUIEditor>();
        win.position = re;
        win.Show();
    }

    void OnEnable()
    {
        Debug.Log("xuieditor enable");
        m_editResType = RESTYPE.none;
    }

    void OnDisable()
    {
        Debug.Log("xuieditor disable");
    }
    GameObject m_gb;
    GameObject m_editGB;
    string ResourcePath = "";
    string UIRootPath = "Assets/XUIEditorData/XUIRoot.prefab";
    RESTYPE m_editResType = RESTYPE.none;

    protected Dictionary<LocalPrefabData.LabelIDData, UILabel> labelsDic = new Dictionary<LocalPrefabData.LabelIDData, UILabel>();


    protected List<LocalPrefabData.LabelIDData> m_labelIds = new List<LocalPrefabData.LabelIDData>();

    void OnGUI()
    {
        GameObject old_gb = m_gb;
        string typeTips = "当前编辑资源类型：";
        if (m_editResType == RESTYPE.none)
        {
            typeTips = "当前编辑资源类型(请选择类型)：";
        }
        var oldResType = m_editResType;
        m_editResType = (RESTYPE)EditorGUILayout.EnumPopup(typeTips, m_editResType, GUILayout.Width(400));
        if (oldResType != m_editResType)
        {
            ReloadPrefabLableDec();
            m_gb = null;
            m_editGB = null;
        }
        if (m_editResType != RESTYPE.none)
        {

            if (m_gb == null)
            {
                m_gb = EditorGUILayout.ObjectField("拖入编辑UI", m_gb, typeof(GameObject)) as GameObject;
            }
            else
            {
                m_gb = EditorGUILayout.ObjectField("当前编辑UI", m_gb, typeof(GameObject)) as GameObject;
            }
            if (m_gb != old_gb && m_gb != null)
            {
                var tempPrefab = PrefabUtility.CreateEmptyPrefab("Assets/XUIEditorData/XUIEditorTemp.prefab");
                tempPrefab = PrefabUtility.ReplacePrefab(m_gb, tempPrefab);
                ResourcePath = AssetDatabase.GetAssetPath(m_gb);
                AssetDatabase.Refresh();
                m_editGB = (GameObject)PrefabUtility.InstantiateAttachedAsset(tempPrefab);
                m_editGB.SetActive(true);
                GameObject uiRoot = GameObject.Find("UIRoot");
                if (uiRoot == null)
                {
                    var ur = (GameObject)AssetDatabase.LoadAssetAtPath(UIRootPath, typeof(GameObject));
                    uiRoot = (GameObject)GameObject.Instantiate(ur);
                    uiRoot.name = "UIRoot";
                }
                m_editGB.name = m_gb.name + "(UIEdit)";
                var wrTrans = uiRoot.transform.Find("window_root");
                m_editGB.transform.parent = wrTrans;
                m_editGB.transform.position = Vector3.zero;
                m_editGB.transform.rotation = Quaternion.Euler(Vector3.zero);
                m_editGB.transform.localScale = Vector3.one;
                LocalPrefabDataEditor.InitUIJsonData(m_gb.name, m_editGB, m_editResType.ToString(), TableLoader.Instance,m_labelIds);
                ReplaceLabel(m_editGB);
                Debug.Log("path = :" + ResourcePath);
            }

            if (m_editGB != null)
            {
                LocalPrefabDataEditor.ShowLabelGUI(m_labelIds, m_editGB, labelsDic);
            }

            EditorGUILayout.LabelField("资源路径：", ResourcePath);
            if (GUILayout.Button("保存当前UI") && m_gb != null)
            {
                Debug.Log("save ... " + m_gb.name);
                SaveEditOB();
            }


        }
    }





  
    void SaveEditOB()
    {
        if (m_editGB == null || m_gb == null)
        {
            return;
        }
        var editGBClone = (GameObject)GameObject.Instantiate(m_editGB);
        var srcGB = (GameObject)GameObject.Instantiate(m_gb);
        srcGB.name = m_gb.name;
        m_editGB.name = m_gb.name;
        LocalPrefabDataEditor.SaveCompareFile(srcGB, editGBClone, m_editResType.ToString(),m_labelIds, true);

        //var sourcePB = PrefabUtility.GetPrefabObject(m_gb);
        // PrefabUtility.ReplacePrefab(editGBClone, sourcePB);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();

        GameObject.DestroyImmediate(editGBClone);
        GameObject.DestroyImmediate(srcGB);
    }

    #region 文本相关处理
    void ReloadPrefabLableDec()
    {
        m_prefabLableDec.Clear();
        InitPrefabLableDec();
    }


    Dictionary<string, int> m_prefabLableDec = new Dictionary<string, int>();

    void InitPrefabLableDec()
    {
        if (m_editResType == RESTYPE.none)
        {
            return;
        }
        if (m_prefabLableDec.Count == 0)
        {
            var m_TextMessage = TableLoader.Instance.GetTable<ezfun_resource.ResTextList>();
            Dictionary<int, ezfun_resource.ResText> tempDic = new Dictionary<int, ezfun_resource.ResText>();
            var enumerator = m_TextMessage.GetEnumerator();
            while (enumerator.MoveNext())
            {
                tempDic[(int)enumerator.Current.Key] = enumerator.Current.Value as ezfun_resource.ResText;

            }
            //for (int i = 300000; i < 310000; i++)
            //{
            //    if (tempDic.ContainsKey(i))
            //    {
            //        m_prefabLableDec[tempDic[i].constantText] = i;
            //    }
            //}
        }
    }

    bool ReplaceLabel(GameObject editGB)
    {
        if (editGB == null)
        {
            return false;
        }

        UILabel[] labels = editGB.GetComponentsInChildren<UILabel>(true);
        InitPrefabLableDec();
        string text = "";

        bool needChangeTrueTypeFont = false;
        bool needChangeText = true;
        string fontName = "";
        string ORGIN_LANGUAGE_FONT = "ARIALN";
        if (m_editResType == RESTYPE.vi)
        {
            fontName = "UTM Cafeta";
            needChangeTrueTypeFont = true;
        }

        if (m_editResType != RESTYPE.zn)
        {
            needChangeText = true;
        }

        for (int i = 0; i < labels.Length; i++)
        {
            text = labels[i].text;
            text = text.Replace("\n", "\\n");

            if (needChangeText && m_prefabLableDec.ContainsKey(text))
            {
                if (!string.IsNullOrEmpty(GetText(m_prefabLableDec[text])))
                {
                    labels[i].text = GetText(m_prefabLableDec[text]);
                }
            }
            else if (needChangeText)
            {
                string removeSpaceStr = text.Trim().Replace("  ", " ");
                if (m_prefabLableDec.ContainsKey(removeSpaceStr))
                {
                    if (!string.IsNullOrEmpty(GetText(m_prefabLableDec[removeSpaceStr])))
                    {
                        labels[i].text = GetText(m_prefabLableDec[removeSpaceStr]);
                    }
                }
            }
            //字体替换
            if (((needChangeTrueTypeFont && labels[i].trueTypeFont != null && labels[i].trueTypeFont.name.Contains(ORGIN_LANGUAGE_FONT))
                || (labels[i].trueTypeFont == null && labels[i].bitmapFont == null)) && fontName != "")
            {
                labels[i].trueTypeFont = Resources.Load<Font>("EZFunUI/Font/" + fontName);
            }
        }
        return true;
    }


    string GetText(int id)
    {
        string text = "";
        ezfun_resource.ResText resText = (ezfun_resource.ResText)TableLoader.Instance.GetEntry<ezfun_resource.ResTextList>(id);
        if (resText != null)
        {
            text = resText.text.Replace("\\n", "\n");
        }
        else
        {
            text = "[NO LABEL]" + id;
        }
        return text;
    }
    #endregion
}