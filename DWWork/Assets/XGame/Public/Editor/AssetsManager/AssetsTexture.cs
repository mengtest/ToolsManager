using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;

public class PopupExample4 : PopupWindowContent
{
    public static int texWidth;
    public static int texHeight;
    public static bool isReadable;
    public static bool is2Power;
    public static bool isMipmap;

    public static int index = 0;
    public string[] options = new string[] { "None","TrueColor", "ETC2_RGBA8" };

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 300);
    }

    public override void OnGUI(Rect rect)
    {
        texWidth = EditorGUILayout.IntField("Width", texWidth);
        texHeight = EditorGUILayout.IntField("Height", texHeight);
        is2Power = EditorGUILayout.Toggle("Power", is2Power);
        isReadable = EditorGUILayout.Toggle("Readable", isReadable);
        isMipmap = EditorGUILayout.Toggle("Mipmap", isMipmap);
        index = EditorGUILayout.Popup(index, options);
    }
}

public class AssetsTexture : EditorWindow
{
    public AssetsTexture instance;

    public string path = "Assets";
    public string selectPath = "";
    public Rect rect;
    public Vector2 scrollVec2;

    public bool showTextureItem = false;

    public List<string> _AllAssetsPaths2 = new List<string>();

    public List<string> _AllTexturePaths = new List<string>();

    List<string> fileNameList1 = new List<string>();
    List<string> fileNameList2 = new List<string>();
    List<string> fileNameList3 = new List<string>();

    List<int> texWidth = new List<int>();
    List<int> texHeight = new List<int>();
    List<Texture> texList1 = new List<Texture>();
    List<Texture> texList2 = new List<Texture>();
    public List<int> texPos = new List<int>();

    List<string> pathList = new List<string>();

    public Rect resTypeRect2;
    public TextureImporterSettings texImpSet = new TextureImporterSettings();
    public int texWidth2;
    public int texHeight2;

    public int platformMaxTextureSize = 0;
    public TextureImporterFormat platformTextureFmt;
    public int platformCompressionQuality = 0;
    public bool platformAllowsAlphaSplit = false;

    private int s_RowCount;
    private int s_ColCount = 3;
    private float m_RowHeight = 30f;
    private float m_ColWidth = 250f;
    private Vector2 m_ScrollPosition;   

    //[MenuItem("Assets资源管理器/搜索贴图")]

    static void Init()
    {
        AssetsTexture window = (AssetsTexture)EditorWindow.GetWindow(typeof(AssetsTexture));
        window.titleContent = new GUIContent("搜索贴图");
    }

    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; } 

    void OnSelectProcess()
    {
        texList2 = new List<Texture>();
        fileNameList3 = new List<string>();
        pathList = new List<string>();

        for (int i = 0; i < _AllTexturePaths.Count; i++)
        {
            EditorUtility.DisplayProgressBar("正在筛选", _AllTexturePaths[i], (float)i / (float)_AllTexturePaths.Count);
            TextureImporter textureImporter = AssetImporter.GetAtPath(_AllTexturePaths[i]) as TextureImporter;
            textureImporter.GetPlatformTextureSettings("", out platformMaxTextureSize, out platformTextureFmt, out platformCompressionQuality, out platformAllowsAlphaSplit);
            texWidth2 = texWidth[i];
            texHeight2 = texHeight[i];
            bool width = (texWidth2 > 0) && ((texWidth2 & (texWidth2 - 1)) == 0);
            bool height = (texHeight2 > 0) && ((texHeight2 & (texHeight2 - 1)) == 0);
            if ((texWidth2 >= PopupExample4.texWidth) && (texHeight2 >= PopupExample4.texHeight))
            {
                if (PopupExample4.is2Power == true)
                {
                    if (width && height)
                    {
                        continue;
                    }
                }               
                if (PopupExample4.isReadable == true)
                {
                    if (textureImporter.isReadable != true)
                    {
                        continue;
                    }
                }
                if (PopupExample4.isMipmap == true)
                {
                    if (textureImporter.mipmapEnabled != true)
                    {
                        continue;
                    }
                }
                if (PopupExample4.index == 1)
                {
                    if (platformTextureFmt != TextureImporterFormat.AutomaticTruecolor)
                    {
                        continue;
                    }
                }
                if (PopupExample4.index == 2)
                {
                    if (platformTextureFmt != TextureImporterFormat.ETC2_RGBA8)
                    {
                        continue;
                    }
                }
                texList2.Add(texList1[i]);
                fileNameList3.Add(fileNameList2[i]);
                pathList.Add(_AllTexturePaths[i]);
            }
            else
            {
                continue;
            }
        }
        EditorUtility.ClearProgressBar();
    }

    void OnTextureResourceProcess()
    {
        _AllAssetsPaths2 = new List<string>();
        _AllTexturePaths = new List<string>();

        fileNameList1 = new List<string>();
        fileNameList2 = new List<string>();

        texPos = new List<int>();
        texWidth = new List<int>();
        texHeight = new List<int>();
        texList1 = new List<Texture>();      

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
                //fileNameList1.Add(Path.GetFileNameWithoutExtension(files[i].ToString()));
                fileNameList1.Add(files[i].Name);
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
            if (str2 == ".jpg" || str2 == ".tga" || str2 == ".psd" || str2 == ".dds" || str2 == ".bmp" || str2 == ".png" || str2 == ".exr")
            {               
                fileNameList2.Add(fileNameList1[i]);
                _AllTexturePaths.Add(_AllAssetsPaths2[i]);
            }
        }
        for (int i = 0; i < _AllTexturePaths.Count; i++)
        {
            EditorUtility.DisplayProgressBar("正在获取贴图大小", _AllTexturePaths[i], (float)i / (float)_AllTexturePaths.Count);
            Texture tex = (Texture)AssetDatabase.LoadAssetAtPath(_AllTexturePaths[i], typeof(Texture));
            //Texture2D tex = EditorGUIUtility.FindTexture(assetsFullPath2[i]);
            if(tex == null)
            {
                texPos.Add(i);
                continue;
            }
            texWidth.Add(tex.width);
            texHeight.Add(tex.height);
            texList1.Add(tex);
            Resources.UnloadAsset(tex);
        }
        for(int i = 0;i < texPos.Count;i++)
        {          
            fileNameList2.RemoveAt(texPos[i]);
            _AllTexturePaths.RemoveAt(texPos[i]);
        }
        EditorUtility.ClearProgressBar();

        OnSelectProcess();
    }

    void OnTextureButton()
    {
        OnTextureResourceProcess();
        showTextureItem = true;
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
                    if ((num2 * 4 + j) > (texList2.Count - 1))
                    {
                        continue;
                    }
                    colRect.width = 30f;
                    GUI.Label(colRect, new GUIContent(texList2[num2 * 4 + j]));
                    colRect.x += 35f;
                    colRect.width = 240f;
                    GUI.Label(colRect, fileNameList3[num2 * 4 + j]);
                    colRect.x += 245f;
                    colRect.width = 30f;
                    if (GUI.Button(colRect, "S"))
                    {
                        OnSelectButton(pathList[num2 * 4 + j]);
                    }
                    colRect.x += 60;
                }

            }
            i++;
        }
    }

    void OnTextureGUI()
    {
        s_RowCount = (int)System.Math.Ceiling(texList2.Count / 4.0);
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

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        rect = EditorGUILayout.GetControlRect(new GUILayoutOption[] { GUILayout.Width((float)(position.width * 0.25)), GUILayout.Height(20) });
        path = EditorGUI.TextField(rect, path);

        if (GUILayout.Button("选择文件夹", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            selectPath = EditorUtility.OpenFolderPanel("选择资源路径", "Assets", "");
            if (selectPath == null)
            {
                path = "Assets";
            }
            else
            {
                path = selectPath;
            }
        }

        if (GUILayout.Button("预加载全部属性资源", new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(20) }))
        {
            OnTextureButton();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("贴图属性", new GUILayoutOption[] { GUILayout.Width((float)(position.width * 0.25)), GUILayout.Height(20) }))
        {
            PopupWindow.Show(resTypeRect2, new PopupExample4());
        }

        if (Event.current.type == EventType.Repaint)
            resTypeRect2 = GUILayoutUtility.GetLastRect();

        if (GUILayout.Button("按属性筛选", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            OnSelectProcess();
        }

        GUILayout.EndHorizontal();

        if (showTextureItem == true)
        {
            OnTextureGUI();
        }

        GUILayout.EndVertical();
    }
}