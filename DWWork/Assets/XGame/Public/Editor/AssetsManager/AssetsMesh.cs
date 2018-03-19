using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;

public class PopupExample : PopupWindowContent
{
    public static bool Color;
    public static bool UV;
    public static bool isReadable;

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 300);
    }

    public override void OnGUI(Rect rect)
    {
        Color = EditorGUILayout.Toggle("Color", Color); 
        UV = EditorGUILayout.Toggle("UV", UV);
        isReadable = EditorGUILayout.Toggle("Readable", isReadable);
    }
}

public class AssetsMesh : EditorWindow
{

    public AssetsMesh instance;

    public string path = "Assets";
    public string selectPath = "";
    public Rect rect;
    public Rect resTypeRect2;

    public bool showMeshItem = false;

    public List<Mesh> meshList1 = new List<Mesh>();

    public List<string> meshName = new List<string>();
    public List<string> meshName2 = new List<string>();

    public List<string> meshPath = new List<string>();
    public List<string> meshPath2 = new List<string>();

    public List<Texture> meshIcon = new List<Texture>();
    public List<Texture> meshIcon2 = new List<Texture>();

    public List<string> _AllAssetsPaths2 = new List<string>();
    public List<string> _AllTexturePaths = new List<string>();


    private int s_RowCount;
    private int s_ColCount = 3;
    private float m_RowHeight = 30f;
    private float m_ColWidth = 250f;
    private Vector2 m_ScrollPosition;

    //[MenuItem("Assets资源管理器/搜索网格")]

    static void Init()
    {
        AssetsMesh window = (AssetsMesh)EditorWindow.GetWindow(typeof(AssetsMesh));
        window.titleContent = new GUIContent("搜索网格");
    }

    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }

    void OnSelectProcess()
    {
        meshName2 = new List<string>();
        meshPath2 = new List<string>();
        meshIcon2 = new List<Texture>();

        for (int i = 0; i < meshName.Count; i++)
        {
            EditorUtility.DisplayProgressBar("正在筛选", meshName[i], (float)i / (float)meshName.Count);
            if (PopupExample.Color == true)
            {
                if ((meshList1[i].colors == null) || (meshList1[i].colors32 == null))
                {
                    continue;
                }
            }
            if (PopupExample.UV == true)
            {
                if ((meshList1[i].uv2 != null) || (meshList1[i].uv3 != null) || (meshList1[i].uv4 != null))
                {
                    continue;
                }
            }          
            if (PopupExample.isReadable == true)
            {
                if (meshList1[i].isReadable == true)
                {
                    continue;
                }
            }
            meshName2.Add(meshName[i]);
            meshPath2.Add(meshPath[i]);
            meshIcon2.Add(meshIcon[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    void OnMeshResourceProcess()
    {
        _AllTexturePaths = new List<string>();
        _AllAssetsPaths2 = new List<string>();

        meshList1 = new List<Mesh>();
        meshName = new List<string>();
        meshPath = new List<string>();
        meshIcon = new List<Texture>();

        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                EditorUtility.DisplayProgressBar("正在查找", files[i].FullName, (float)i / (float)files.Length);
                if (files[i].Name.EndsWith(".meta") || files[i].FullName.Contains("Plugins") || files[i].FullName.Contains("StreamingAssets"))
                {
                    continue;
                }
                var fullPath = files[i].FullName.Replace("\\", "/");
                _AllAssetsPaths2.Add(fullPath.Substring(fullPath.IndexOf("Assets")));
            }
            EditorUtility.ClearProgressBar();
        }

        for (int i = 0; i < _AllAssetsPaths2.Count; i++)
        {
            string str1 = _AllAssetsPaths2[i].Substring(_AllAssetsPaths2[i].IndexOf("."));
            string str2 = "";
            str2 = str1.ToLower();
            if (str2 == ".fbx")
            {
                _AllTexturePaths.Add(_AllAssetsPaths2[i]);
            }
        }

        for (int i = 0; i < _AllTexturePaths.Count; i++)
        {
            EditorUtility.DisplayProgressBar("正在获取网格属性", _AllTexturePaths[i], (float)i / (float)_AllTexturePaths.Count);
            GameObject objPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(_AllTexturePaths[i], typeof(GameObject));
            if (objPrefab.GetComponents<MeshFilter>().Length == 0)
            {
                continue;
            }
            Mesh mesh = null;
            MeshFilter[] meshFilter = objPrefab.GetComponents<MeshFilter>();
            for(int j = 0;j < meshFilter.Length;j++)
            {
                mesh = meshFilter[j].sharedMesh;
                if (mesh == null)
                {
                    continue;
                }
                var meshpath = AssetDatabase.GetAssetPath(mesh);
                var icon = AssetDatabase.GetCachedIcon(meshpath);
                //var icon = AssetPreview.GetAssetPreview(mesh);
                meshList1.Add(mesh);
                meshName.Add(mesh.name);
                meshPath.Add(meshpath);
                meshIcon.Add(icon);
            }
           
        }               
        EditorUtility.ClearProgressBar();

        OnSelectProcess();
    }


    void GetRowPosition(out int firstRowVisible, out int lastRowVisible, float viewHeight)
    {
        if (s_RowCount == 0)
        {
            firstRowVisible = lastRowVisible = -1;
        }
        else
        {
            float y = m_ScrollPosition.y;
            float height = viewHeight;
            firstRowVisible = (int)Mathf.Floor(y / m_RowHeight);
            lastRowVisible = firstRowVisible + (int)Mathf.Ceil(height / m_RowHeight);
            firstRowVisible = Mathf.Max(firstRowVisible, 0);
            lastRowVisible = Mathf.Min(lastRowVisible, s_RowCount - 1);
            if (firstRowVisible >= s_RowCount && firstRowVisible > 0)
            {
                m_ScrollPosition.y = 0f;
                GetRowPosition(out firstRowVisible, out lastRowVisible, viewHeight);
            }
        }
    }


    void OnSelectButton(string _PathValue)
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(_PathValue);
    }

    void ShowSelectItems(int firstRow, int numVisibleRows, float rowWidth, float viewHeight)
    {
        int i = 0;
        while (i < numVisibleRows)
        {
            int num2 = firstRow + i;
            Rect rowRect = new Rect(0f, (float)num2 * m_RowHeight, rowWidth, m_RowHeight);
            float num3 = rowRect.y - m_ScrollPosition.y;
            if (num3 <= viewHeight)
            {
                Rect colRect = new Rect(rowRect);
                colRect.width = 0;
                for (int j = 0; j < 4; j++)
                {
                    if ((num2 * 4 + j) > (meshName2.Count - 1))
                    {
                        continue;
                    }
                    colRect.width = 30f;
                    GUI.Label(colRect, new GUIContent((meshIcon2[num2 * 4 + j])));
                    colRect.x += 35f;
                    colRect.width = 240f;
                    GUI.Label(colRect, meshName2[num2 * 4 + j]);
                    colRect.x += 245f;
                    colRect.width = 30f;
                    if (GUI.Button(colRect, "S"))
                    {
                        OnSelectButton(meshPath2[num2 * 4 + j]);
                    }
                    colRect.x += 60;
                }

            }
            i++;
        }
    }

    void OnMeshGUI()
    {
        s_RowCount = (int)System.Math.Ceiling(meshName2.Count / 4.0);
        GUILayout.Space(5f);

        Rect totalRect = new Rect(0, 80, position.width, position.height - 80);
        Rect contentRect = new Rect(0, 0, 340 * 4, s_RowCount * m_RowHeight);
        m_ScrollPosition = GUI.BeginScrollView(totalRect, m_ScrollPosition, contentRect);

        int num;
        int num2;
        GetRowPosition(out num, out num2, totalRect.height);
        if (num2 >= 0)
        {
            int numVisibleRows = num2 - num + 1;
            ShowSelectItems(num, numVisibleRows, contentRect.width, totalRect.height);
        }

        GUI.EndScrollView(true);
    }

    void OnMeshButton()
    {
        OnMeshResourceProcess();
        showMeshItem = true;
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        rect = EditorGUILayout.GetControlRect(new GUILayoutOption[] { GUILayout.Width((float)(position.width * 0.25)), GUILayout.Height(20) });
        path = EditorGUI.TextField(rect, path);

        if (GUILayout.Button("选择文件夹", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            selectPath = EditorUtility.OpenFolderPanel("选择资源路径", "Assets", "");
            path = selectPath;
        }

        if (GUILayout.Button("预加载全部属性资源", new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(20) }))
        {
            OnMeshButton();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("网格属性", new GUILayoutOption[] { GUILayout.Width((float)(position.width * 0.25)), GUILayout.Height(20) }))
        {
            PopupWindow.Show(resTypeRect2, new PopupExample());
        }

        if (Event.current.type == EventType.Repaint)
            resTypeRect2 = GUILayoutUtility.GetLastRect();

        if (GUILayout.Button("按属性筛选", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            OnSelectProcess();
        }

        GUILayout.EndHorizontal();

        if (showMeshItem == true)
        {
            OnMeshGUI();
        }

        GUILayout.EndVertical();
    }
}
