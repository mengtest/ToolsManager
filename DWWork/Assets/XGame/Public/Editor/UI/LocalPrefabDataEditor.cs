using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using LitJson;
using LocalPrefabData;




public class UIPrefabLabelEditor : EditorWindow
{
    private GameObject m_gb;

    [MenuItem("UI/Prefab Label运行中处理")]
    static void CompareVIPrefab()
    {
        Rect re = new Rect(500, 500, 500, 300);
        var win = EditorWindow.GetWindow<UIPrefabLabelEditor>();
        win.position = re;
        win.Show();

    }


    protected List<UILabel> m_labelIds = new List<UILabel>();


    void OnEnable()
    {
        Debug.Log("xuieditor enable");
        m_labelIds.Clear();
    }

    void OnGUI()
    {


        var gb = EditorGUILayout.ObjectField("拖入查找label prefab", m_gb, typeof(GameObject), true) as GameObject;
        if (gb == null)
        {
            return;
        }
        if (gb != m_gb)
        {
            m_gb = gb;
            m_labelIds.Clear();
            var labels = m_gb.GetComponentsInChildren<UILabel>(true);
            for (int i = 0; i < labels.Length; i++)
            {
                if (labels[i].trueTypeFont == null || labels[i].trueTypeFont.name == "Arial")
                {
                    m_labelIds.Add(labels[i]);
                }
            }
        }
        for (int i = 0; i < m_labelIds.Count; i++)
        {
            EditorGUILayout.ObjectField("有需要替换的label", m_labelIds[i], typeof(UILabel), true);
        }
    }


}
