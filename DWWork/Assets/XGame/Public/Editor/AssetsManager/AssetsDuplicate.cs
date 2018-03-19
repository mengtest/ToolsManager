using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;

public class AssetsDuplicate : EditorWindow
{
    public AssetsDuplicate instance;

    public string path = "Assets";
    public string selectPath = "";
    public Rect rect;

    public bool showDuplicateItem = false;

    public int index = 0;
    public string[] options = new string[] { "None", "AnimationClip", "AudioClip", "AudioMixer", "Font", "GUISkin", "Material", "Mesh", "Model", "PhysicMaterial", "Prefab", "Scene", "C#", "Lua", "Shader", "Texture" };
    public bool[] toggle = new bool[16];
    public string[] resTypeArray = { "", ".anim", ".ogg", ".aa", ".ttf", ".bb", ".mat", ".FBX", ".fbx", ".cc", ".prefab", ".unity", ".cs", ".lua", ".shader", ".png" };

    public List<string> _AllAssetsPaths = new List<string>();
    public List<string> filePaths2 = new List<string>();
    public Dictionary<string, List<string>> dic2 = new Dictionary<string, List<string>>();
    public string[] filePathsArray2;
    public string[] _AllAssetsPathsArray;
  
    List<Texture> texList = new List<Texture>();
    List<string> fileNameList = new List<string>();
    List<string> pathList = new List<string>();

    public int s_RowCount;
    public int s_ColCount = 9;
    private float m_RowHeight = 30f;
    private float m_ColWidth = 200f;
    private Vector2 m_ScrollPosition;

    //[MenuItem("Assets资源管理器/搜索同名")]

    static void Init()
    {
        AssetsDuplicate window = (AssetsDuplicate)EditorWindow.GetWindow(typeof(AssetsDuplicate));
        window.titleContent = new GUIContent("搜索同名");
    }

    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }

    void GetResourceType()
    {
        for (int i = 0; i < 16; i++)
        {
            toggle[i] = false;
        }
        if ((index == -1) || (index == 0))
        {
            return;
        }
        toggle[index] = true;
    }

    void OnSelectProcess()
    {
        texList = new List<Texture>();
        fileNameList = new List<string>();
        pathList = new List<string>();

        string resourceType = "";
        for (int i = 0; i < 16; i++)
        {
            if (toggle[i] == true)
            {
                resourceType = resTypeArray[i];
            }

        }
        resourceType = resTypeArray[index];

        List<string> tempKeys = new List<string>(dic2.Keys);

        for (int i = 0; i < dic2.Count; i++)
        {
            List<string> tempValue = new List<string>();
            dic2.TryGetValue(tempKeys[i], out tempValue);
            if (tempValue.Count > 1)
            {
                for (int j = 0; j < tempValue.Count; j++)
                {
                    string str1 = tempValue[j].Substring(tempValue[j].IndexOf("."));
                    string str2 = "";
                    str2 = str1.ToLower();
                    if (resourceType == "")
                    {
                        texList.Add(AssetDatabase.GetCachedIcon(tempValue[j]));
                        fileNameList.Add(tempKeys[i]);
                        pathList.Add(tempValue[j]);
                        continue;
                    }
                    if (resourceType == ".png")
                    {
                        if (str2 != resourceType)
                        {
                            if (str2 == ".jpg" || str2 == ".tga" || str2 == ".psd" || str2 == ".dds" || str2 == ".bmp" || str2 == ".exr")
                            {
                                texList.Add(AssetDatabase.GetCachedIcon(tempValue[j]));
                                fileNameList.Add(tempKeys[i]);
                                pathList.Add(tempValue[j]);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            texList.Add(AssetDatabase.GetCachedIcon(tempValue[j]));
                            fileNameList.Add(tempKeys[i]);
                            pathList.Add(tempValue[j]);
                        }
                    }
                    else if (str2 != resourceType)
                    {
                        continue;
                    }
                    else
                    {
                        texList.Add(AssetDatabase.GetCachedIcon(tempValue[j]));
                        fileNameList.Add(tempKeys[i]);
                        pathList.Add(tempValue[j]);
                    }
                }
            }
        }
    }

    void OnDuplicateResourceProcess()
    {
        _AllAssetsPaths = new List<string>();
        filePaths2 = new List<string>();
        dic2 = new Dictionary<string, List<string>>();

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
                //filePaths2.Add(Path.GetFileNameWithoutExtension(files[i].ToString()));
                filePaths2.Add(files[i].Name);
                var fullPath = files[i].FullName.Replace("\\", "/");
                _AllAssetsPaths.Add(fullPath.Substring(fullPath.IndexOf("Assets")));
            }
            EditorUtility.ClearProgressBar();
        }

        _AllAssetsPathsArray = _AllAssetsPaths.ToArray();
        filePathsArray2 = filePaths2.ToArray();

        for (int i = 0; i < filePaths2.Count; i++)
        {
            if (dic2.ContainsKey(filePathsArray2[i]) == false)
            {
                List<string> tempValue = new List<string>();
                tempValue.Add(_AllAssetsPathsArray[i]);
                dic2.Add(filePathsArray2[i], tempValue);
            }
            else
            {
                List<string> outputValue2 = new List<string>();
                dic2.TryGetValue(filePathsArray2[i], out outputValue2);
                outputValue2.Add(_AllAssetsPathsArray[i]);
            }
        }

        OnSelectProcess();
    }

    void OnDuplicateButton()
    {
        GetResourceType();
        OnDuplicateResourceProcess();
        showDuplicateItem = true;
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
                    if ((num2 * 4 + j) > (texList.Count - 1))
                    {
                        continue;
                    }
                    colRect.width = 30f;
                    GUI.Label(colRect, new GUIContent(texList[num2 * 4 + j]));
                    colRect.x += 35f;
                    colRect.width = 240f;
                    GUI.Label(colRect, fileNameList[num2 * 4 + j]);
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

    void OnDuplicateGUI()
    {
        s_RowCount = (int)System.Math.Ceiling(texList.Count / 4.0);
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

        rect = EditorGUILayout.GetControlRect(new GUILayoutOption[] { GUILayout.Width((float)(position.width*0.25)), GUILayout.Height(15) });
        path = EditorGUI.TextField(rect, path);

        if (GUILayout.Button("选择文件夹", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) }))
        {
            selectPath = EditorUtility.OpenFolderPanel("选择资源路径", "Assets", "");
            if(selectPath == null)
            {
                path = "Assets";
            }
            else
            {
                path = selectPath;
            }        
        }

        if (GUILayout.Button("预加载全部类型资源", new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(20) }))
        {
            OnDuplicateButton();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();

        index = EditorGUILayout.Popup(index, options, new GUILayoutOption[]{GUILayout.Width((float)(position.width * 0.25)), GUILayout.Height(15)});

        if (GUILayout.Button("按类型筛选", new GUILayoutOption[]{GUILayout.Width(100),GUILayout.Height(20)}))
        {
            OnSelectProcess();
        }

        GUILayout.EndHorizontal();

        if (showDuplicateItem == true)
        {
            OnDuplicateGUI();
        }

        GUILayout.EndVertical();
    }
}
        
