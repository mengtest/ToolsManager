using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using LitJson;
using LocalPrefabData;

public enum PreTYPE
{
    none,
    vi,
    kr,
    tw,
    th,
    en,
}


public class LocalPrefabDataEditor : EditorWindow
{

    private static PreTYPE m_pre;

    private GameObject m_gb;

    private GameObject m_srcGb;


    private Transform m_rootTrans;

    Dictionary<string, string> prefabObjs = new Dictionary<string, string>();


    [MenuItem("Local/Prefab运行中处理")]
    static void CompareVIPrefab()
    {
        Rect re = new Rect(500, 500, 500, 300);
        var win = EditorWindow.GetWindow<LocalPrefabDataEditor>();
        win.position = re;
        win.Show();

    }
    protected Dictionary<LocalPrefabData.LabelIDData, UILabel> labelsDic = new Dictionary<LocalPrefabData.LabelIDData, UILabel>();


    protected List<LocalPrefabData.LabelIDData> m_labelIds = new List<LocalPrefabData.LabelIDData>();


    void OnEnable()
    {
        Debug.Log("xuieditor enable");
        prefabObjs.Clear();
        List<FileInfo> list = new List<FileInfo>();
        DirectoryInfo dinfo = new DirectoryInfo(Application.dataPath + "/XGame/Resources/EZFunUI/Item");
        FileInfo[] files = dinfo.GetFiles("*.prefab", SearchOption.AllDirectories);
        list.AddRange(files);
        dinfo = new DirectoryInfo(Application.dataPath + "/XGame/Resources/EZFunUI/Window");
        files = dinfo.GetFiles("*.prefab", SearchOption.AllDirectories);
        list.AddRange(files);
        for (int i = 0; i < list.Count; i++)
        {
            prefabObjs[list[i].Name.Replace(".prefab", "")] = list[i].FullName.Substring((Application.dataPath + "/XGame/Resources/").Length).Replace(".prefab", "");
        }
        if (m_rootTrans == null)
        {
            m_rootTrans = new GameObject("UIRootEditor").transform;
            m_rootTrans.position = new Vector3(1000000, 0, 0);
            GetOrAddComponent<UIRoot>(m_rootTrans.gameObject);
            GetOrAddComponent<UIPanel>(m_rootTrans.gameObject);
            GetOrAddComponent<Rigidbody>(m_rootTrans.gameObject);
        }
    }

    void OnGUI()
    {
        string typeTips = "prefab前缀：";
        if (m_pre == PreTYPE.none)
        {
            typeTips = "prefab前缀(请选择类型)：";
        }
        var oldResType = m_pre;
        m_pre = (PreTYPE)EditorGUILayout.EnumPopup(typeTips, m_pre, GUILayout.Width(400));
        string srcStr = "";
        if (m_gb == null)
        {
            m_gb = EditorGUILayout.ObjectField("拖入编辑UI", m_gb, typeof(GameObject), true) as GameObject;
        }
        else
        {
            m_gb = EditorGUILayout.ObjectField("当前编辑UI", m_gb, typeof(GameObject), true) as GameObject;
            if (!(m_srcGb != null && m_srcGb.name == m_gb.name))
            {
                m_srcGb = FindResourceObj(m_gb.name.Replace("(Clone)", ""), m_gb);
                if (m_srcGb)
                {
                    m_srcGb.name = m_srcGb.name.Replace("(Clone)", "");
                }
                srcStr = "没有找到原gameObject的原Prefab，";
                m_string = "";
            }
        }

        if (m_srcGb == null)
        {
            m_srcGb = EditorGUILayout.ObjectField(srcStr + "拖入原始Prefab", m_srcGb, typeof(GameObject), false) as GameObject;
        }
        else
        {
            m_srcGb = EditorGUILayout.ObjectField("原始Prefab", m_srcGb, typeof(GameObject), false) as GameObject;
        }

        if (m_pre != PreTYPE.none)
        {
            if (GUILayout.Button("保存当前ui的改变") && m_gb != null && m_srcGb != null)
            {
                Debug.Log("save ... " + m_gb.name);
                SaveCompareFile(m_srcGb, m_gb, m_pre.ToString(), m_labelIds);
            }
        }
        if (m_string != "")
        {
            GUILayout.TextField(m_string);
        }
    }



    public static void ShowLabelGUI(List<LabelIDData> m_labelIds, GameObject m_editGB, Dictionary<LocalPrefabData.LabelIDData, UILabel> labelsDic)
    {
        NGUIEditorTools.DrawHeader("labels", true);
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(3f);
            GUILayout.BeginVertical();
            Vector2 mScroll = GUILayout.BeginScrollView(new Vector2(50, 100));
            GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
            int lenth = EditorGUILayout.IntField("设置文本id的数目", m_labelIds.Count);
            if (lenth < m_labelIds.Count)
            {
                lenth = m_labelIds.Count;
            }
            GUILayout.EndHorizontal();
            for (int i = m_labelIds.Count; i < lenth; i++)
            {
                m_labelIds.Add(new LocalPrefabData.LabelIDData());
            }

            for (int i = 0; i < m_labelIds.Count; i++)
            {
                if (m_labelIds[i] != null && !string.IsNullOrEmpty(m_labelIds[i].labelPath))
                {
                    if (m_editGB.transform.Find(m_labelIds[i].labelPath) != null)
                    {
                        UILabel trans = m_editGB.transform.Find(m_labelIds[i].labelPath).GetComponent<UILabel>();
                        if (trans != null)
                        {
                            labelsDic[m_labelIds[i]] = trans;
                        }
                    }

                }
            }

            for (int i = 0; i < lenth; i++)
            {
                GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                if (labelsDic.ContainsKey(m_labelIds[i]))
                {
                    labelsDic[m_labelIds[i]] = EditorGUILayout.ObjectField(string.IsNullOrEmpty(m_labelIds[i].labelPath) ? "Null" : m_labelIds[i].labelPath, labelsDic[m_labelIds[i]], typeof(UILabel)) as UILabel;
                }
                else
                {
                    labelsDic[m_labelIds[i]] = EditorGUILayout.ObjectField("Null", null, typeof(UILabel)) as UILabel;
                }
                if (labelsDic[m_labelIds[i]] != null)
                {
                    m_labelIds[i].labelPath = FindToParentPath(m_editGB.transform, labelsDic[m_labelIds[i]].transform);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                m_labelIds[i].labelID = EditorGUILayout.IntField("设置文本id", m_labelIds[i].labelID);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
        }
    }




    private static string m_string = "";
    public static void SaveCompareFile(GameObject srcGb, GameObject dstGb, string pre, List<LabelIDData> labels, bool isRecordSprite = false)
    {
        List<NodeData> list = new List<NodeData>();
        ComparePrefab(srcGb.transform, dstGb.transform, 0, "", list, isRecordSprite);

        if (list.Count != 0 || labels.Count != 0)
        {
            StreamWriter sw = null;
            try
            {
                string filePaht = Application.dataPath + "/StreamingAssets/Table/" + pre + "_" + srcGb.name + ".bytes";
                sw = new StreamWriter(filePaht);
                PrefabData prefabData = new PrefabData();
                prefabData.nodes = list;
                prefabData.labelIds = labels;
                JsonWriter jsonWrite = new JsonWriter();
                jsonWrite.PrettyPrint = true;
                JsonMapper.ToJson(prefabData, jsonWrite);
                var js = jsonWrite.ToString();
                sw.Write(js);
                sw.Flush();
                m_string = "保存路径:" + "/StreamingAssets/Table/" + pre + "_" + srcGb.name + ".bytes";
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                }
            }
        }
        else
        {
            m_string = "没有什么保存";
        }
    }

    #region 初始化内容

    public static void InitUIJsonData(string ressName, GameObject ob, string pre, TableLoader resourceSys, List<LabelIDData> m_labelIds)
    {
        string dataName = pre + "_" + ressName + ".bytes";
        string use_path = Application.streamingAssetsPath + "/Table/" + dataName;
        m_labelIds.Clear();
        if (File.Exists(use_path))
        {
            byte[] b = ReadFileStream(use_path);
            if (b != null)
            {
                LocalPrefabData.PrefabData data = JsonMapper.ToObject<LocalPrefabData.PrefabData>(System.Text.Encoding.UTF8.GetString(b));
                if (data != null)
                {
                    LocalPrefabData.PrefabDataTools.ApplyPrefabData(data, ob.transform, resourceSys);
                    if (data.labelIds != null)
                    {
                        m_labelIds.AddRange(data.labelIds);
                    }
                }
            }
        }
    }


    public static byte[] ReadFileStream(string path)
    {
        byte[] b = null;
        using (Stream file = File.OpenRead(path))
        {
            b = new byte[(int)file.Length];
            file.Read(b, 0, b.Length);
            file.Close();
            file.Dispose();
        }
        return b;
    }

    #endregion


    #region 节点对比
    static void ComparePrefab(Transform srcTrans, Transform dstTrans, int depth, string parentName, List<NodeData> list, bool isRecordSprite)
    {
        for (int i = 0; i < srcTrans.transform.childCount; i++)
        {
            if (srcTrans.GetChild(i).name.Contains("fx_"))
            {
                continue;
            }
            NodeData childNodeData = CompareTrans(srcTrans.GetChild(i), dstTrans.Find(srcTrans.GetChild(i).name), isRecordSprite);// ComparePrefab(srcTrans.GetChild(i), dstTrans.FindChild(srcTrans.GetChild(i).name));
          
            if (childNodeData != null)
            {
                childNodeData.depth = depth;
                childNodeData.name = (parentName != "" ? parentName + "/" : "") + srcTrans.GetChild(i).name;
                list.Add(childNodeData);
            }
        }
        //广度递归
        for (int i = 0; i < srcTrans.transform.childCount; i++)
        {
            if (srcTrans.GetChild(i).name.Contains("fx_"))
            {
                continue;
            }
            ComparePrefab(srcTrans.GetChild(i), dstTrans.Find(srcTrans.GetChild(i).name), depth + 1, (parentName != "" ? parentName + "/" : "") + srcTrans.GetChild(i).name, list, isRecordSprite);// ComparePrefab(srcTrans.GetChild(i), dstTrans.FindChild(srcTrans.GetChild(i).name));
        }
    }


    static NodeData CompareTrans(Transform srcTrans, Transform dstTrans, bool isRecordSprite)
    {
        if (srcTrans == null || dstTrans == null)
        {
            return null;
        }


        UILabelFixData LabelData = CompareUILabel(srcTrans.GetComponent<UILabel>(), dstTrans.GetComponent<UILabel>());

        UIFixSpriteData spriteData = CompareUISprite(srcTrans.GetComponent<UISprite>(), dstTrans.GetComponent<UISprite>(), isRecordSprite);
        //强制记住位置
        TransformFixData transData = CompareTransCom(srcTrans, dstTrans, (LabelData != null || spriteData != null));
        //有锚点的话，就不要去改位置了
        UIRect rect = srcTrans.GetComponent<UIRect>();
        if ((rect != null && rect.isAnchored))
        {
            transData = null;
        }

        NodeData nodeData = null;
        if (transData != null || LabelData != null || spriteData != null)
        {
            nodeData = new NodeData();
            nodeData.name = srcTrans.name;
            if (transData != null)
            {
                nodeData.transformData = transData;
            }
            if (LabelData != null)
            {
                nodeData.uiLableData = LabelData;
            }
            if (spriteData != null)
            {
                nodeData.uiSpriteData = spriteData;
            }
        }
        return nodeData;
    }

    static UILabelFixData CompareUILabel(UILabel srcLabel, UILabel dstLable)
    {
        if (srcLabel == null || dstLable == null)
        {
            return null;
        }
        UILabelFixData data = null;
        if (srcLabel.width != dstLable.width || srcLabel.height != dstLable.height
        || srcLabel.color != dstLable.color || srcLabel.depth != dstLable.depth
            || srcLabel.color != dstLable.color
            || srcLabel.aspectRatio != dstLable.aspectRatio
            || srcLabel.pivot != dstLable.pivot
            || srcLabel.keepAspectRatio != dstLable.keepAspectRatio)
        {
            data = new UILabelFixData();
            data.width = dstLable.width;
            data.height = dstLable.height;
            data.depth = dstLable.depth;
            data.color = PrefabDataTools.ColorToDouble(dstLable.color);
            data.aspectRatio = dstLable.aspectRatio;
            data.pivot = (int)dstLable.pivot;
            data.AspectType = (int)dstLable.keepAspectRatio;
            data.hasWidget = true;
        }
        if (srcLabel.fontSize != dstLable.fontSize
            || srcLabel.overflowMethod != dstLable.overflowMethod
            || srcLabel.spacingX != dstLable.spacingX
            || srcLabel.spacingY != dstLable.spacingY
            || srcLabel.gradientTop != dstLable.gradientTop
            || srcLabel.gradientBottom != dstLable.gradientBottom
            || srcLabel.keepCrispWhenShrunk != dstLable.keepCrispWhenShrunk
            || srcLabel.alignment != dstLable.alignment
            || data != null)
        {
            if (data == null)
            {
                data = new UILabelFixData();
            }
            data.fontSize = dstLable.fontSize;
            data.mSpacingX = dstLable.spacingX;
            data.mSpacingY = dstLable.spacingY;
            data.mOverflow = (int)dstLable.overflowMethod;
            if (srcLabel.gradientTop != dstLable.gradientTop)
                data.gradientTop = PrefabDataTools.ColorToDouble(dstLable.gradientTop);
            if (srcLabel.gradientBottom != dstLable.gradientBottom)
                data.gradientBottom = PrefabDataTools.ColorToDouble(dstLable.gradientBottom);
            data.keepCrisp = (int)dstLable.keepCrispWhenShrunk;
            data.alignMent = (int)dstLable.alignment;
            data.isGradient = dstLable.applyGradient;
            data.hasUILable = true;
        }
        return data;
    }

    static TransformFixData CompareTransCom(Transform srcTrans, Transform dstTrans, bool isForce)
    {
        if (srcTrans == null || dstTrans == null)
        {
            return null;
        }
        TransformFixData data = null;
        if (srcTrans.localPosition != dstTrans.localPosition
            || srcTrans.localRotation != dstTrans.localRotation
            || srcTrans.localScale != dstTrans.localScale
            || isForce)
        {
            data = new TransformFixData();
            if (dstTrans.localPosition != srcTrans.localPosition || isForce)
                data.localPos = PrefabDataTools.Vector3ToDouble(dstTrans.localPosition);
            if (dstTrans.localRotation != srcTrans.localRotation)
                data.localRotation = PrefabDataTools.Quaternion3ToDouble(dstTrans.localRotation);
            if (dstTrans.localScale != srcTrans.localScale)
                data.localScale = PrefabDataTools.Vector3ToDouble(dstTrans.localScale);
        }
        return data;
    }


    static UIFixSpriteData CompareUISprite(UISprite srcSprite, UISprite dstSprite, bool isRecordSprite)
    {
        if (srcSprite == null || dstSprite == null)
        {
            return null;
        }

        UIFixSpriteData comData = null;
        if (srcSprite.width != dstSprite.width || srcSprite.height != dstSprite.height
           || srcSprite.color != dstSprite.color || srcSprite.depth != dstSprite.depth
            || srcSprite.aspectRatio != dstSprite.aspectRatio
            || srcSprite.pivot != dstSprite.pivot
            || srcSprite.keepAspectRatio != dstSprite.keepAspectRatio)
        {
            comData = new UIFixSpriteData();
            comData.width = dstSprite.width;
            comData.height = dstSprite.height;
            comData.depth = dstSprite.depth;
            comData.color = PrefabDataTools.ColorToDouble(dstSprite.color);
            comData.aspectRatio = dstSprite.aspectRatio;
            comData.pivot = (int)dstSprite.pivot;
            comData.AspectType = (int)dstSprite.keepAspectRatio;
            comData.hasWidget = true;
        }
        if (srcSprite.type != dstSprite.type || (isRecordSprite && srcSprite.atlas != null &&
            dstSprite.atlas != null && (srcSprite.spriteName != dstSprite.spriteName || srcSprite.atlas.name != dstSprite.atlas.name))//两个都有，别且不一样
            || (isRecordSprite && srcSprite.atlas == null && dstSprite.atlas != null)//目标图片有sprite，而原始没有
            || comData != null)
        {
            if (comData == null)
            {
                comData = new UIFixSpriteData();
            }
            comData.mType = (int)dstSprite.type;
            comData.hasUISprite = true;
            if (isRecordSprite && ((srcSprite.atlas != null && dstSprite.atlas != null && (srcSprite.spriteName != dstSprite.spriteName || srcSprite.atlas.name != dstSprite.atlas.name))
                    || (dstSprite.atlas != null && srcSprite.atlas == null))
                )
            {
                comData.mAtlas = dstSprite.atlas.name;
                comData.mSprite = dstSprite.spriteName;
            }
        }
        return comData;
    }
    #endregion

    #region 工具方法
    /// <summary>
    /// 通过名字找到对应的prefab
    /// </summary>
    /// <param name="m_gbName"></param>
    /// <param name="dstGb"></param>
    /// <returns></returns>
    GameObject FindResourceObj(string m_gbName, GameObject dstGb)
    {
        if (prefabObjs.ContainsKey(m_gbName))
        {
            string PrefabName = prefabObjs[m_gbName];
            PrefabName = PrefabName.Replace("\\", "/");
            Debug.Log(PrefabName);
            GameObject resource = Resources.Load<GameObject>(PrefabName);
            GameObject gb = (GameObject)GameObject.Instantiate(resource);
            if (m_rootTrans == null)
            {
                m_rootTrans = new GameObject("UIRootEditor").transform;
                m_rootTrans.position = new Vector3(1000000, 0, 0);
                GetOrAddComponent<UIRoot>(m_rootTrans.gameObject);
                GetOrAddComponent<UIPanel>(m_rootTrans.gameObject);
                GetOrAddComponent<Rigidbody>(m_rootTrans.gameObject);
            }
            gb.transform.parent = m_rootTrans;
            return gb;
        }
        else if (true)
        {
            return null;
        }
    }


    static T GetOrAddComponent<T>(GameObject gb)
       where T : UnityEngine.Component
    {
        T t = gb.GetComponent<T>();
        if (t == null)
        {
            t = gb.AddComponent<T>();
        }

        return t;
    }


    public static string FindToParentPath(Transform parent, Transform child)
    {
        if (child == null || parent == null)
        {
            return "";
        }
        if (child == parent)
        {
            return "";
        }
        string path = child.name;
        while (child.parent != parent)
        {
            if (child.parent == null)
            {
                return "";
            }
            child = child.parent;
            path = child.name + "/" + path;
        }
        return path;
    }

    #endregion
}
