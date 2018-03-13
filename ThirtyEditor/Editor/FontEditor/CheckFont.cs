using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class PrefabCheck : MonoBehaviour
{
    [MenuItem("EZFun/Font/Check Unity System  Font")]
    static void CheckFont()
    {
        List<string> dirs = new List<string>();
        CheckSystemFontInDirs(Application.dataPath + "/XGame/Resources/EZFunUI", ref dirs);
    }
    [MenuItem("EZFun/Font/Replace Unity System Font")]
    static void ReplaceFont()
    {
        List<string> dirs = new List<string>();
        ReplaceSystemFontInDirs(Application.dataPath + "/XGame/Resources/EZFunUI", ref dirs);
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 检查路径里是否有UILabel引用系统字体库
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="dirs"></param>
  
    private static void CheckSystemFontInDirs(string dirPath, ref List<string> dirs)
    {
        foreach (string path in Directory.GetFiles(dirPath))
        {
            //获取所有文件夹中包含后缀为 .prefab 的路径    
            if (System.IO.Path.GetExtension(path) == ".prefab")
            {
                dirs.Add(path.Substring(path.IndexOf("Assets")));
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(path.Substring(path.IndexOf("Assets")), typeof(GameObject));
                if (obj != null)
                {
                    Debug.Log("Connect Font Success=" + path);
                    GameObject instance = (GameObject)GameObject.Instantiate(obj);
                    UILabel[] labels = instance.GetComponentsInChildren<UILabel>(true);
                    foreach (UILabel label in labels)
                    {
                        if (label.trueTypeFont != null && (label.trueTypeFont.name.Contains("Lucida") || label.trueTypeFont.name.Contains("Arial")))
                        {
                            Debug.LogError(label.trueTypeFont.name);
                            Debug.LogError(path.Substring(path.IndexOf("Assets")));
                        }
                        if (label.bitmapFont != null && (label.bitmapFont.name.Contains("Lucida") || label.bitmapFont.name.Contains("Arial")))
                        {
                            Debug.LogError(label.bitmapFont.name);
                            Debug.LogError(path.Substring(path.IndexOf("Assets")));
                        }
                    }
                    DestroyImmediate(instance);
                }
                else
                {
                    Debug.LogError(path.Substring(path.IndexOf("Assets")));
                }


            }
        }

        if (Directory.GetDirectories(dirPath).Length > 0)  //遍历所有文件夹    
        {
            foreach (string path in Directory.GetDirectories(dirPath))
            {
                CheckSystemFontInDirs(path, ref dirs);
            }
        }
    }



    /// <summary>
    /// 替换路径里prefab中的UILabel引用系统字体为ARIALN
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="dirs"></param>
    private static void ReplaceSystemFontInDirs(string dirPath, ref List<string> dirs)
    {
        Font defaultFont = Resources.Load<Font>("EZFunUI/Font/ARIALN");
        foreach (string path in Directory.GetFiles(dirPath))
        {
            //获取所有文件夹中包含后缀为 .prefab 的路径    
            if (System.IO.Path.GetExtension(path) == ".prefab")
            {
                dirs.Add(path.Substring(path.IndexOf("Assets")));
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(path.Substring(path.IndexOf("Assets")), typeof(GameObject));
                if (obj != null)
                {
                    Debug.Log("Connect Font Success=" + path);
                    bool neddReplace = false;
                    GameObject instance = (GameObject)GameObject.Instantiate(obj);
                    UILabel[] labels = instance.GetComponentsInChildren<UILabel>(true);
                    foreach (UILabel label in labels)
                    {
                        if (label.trueTypeFont != null && (label.trueTypeFont.name.Contains("Lucida") || label.trueTypeFont.name.Contains("Arial")))
                        {
                            label.trueTypeFont = defaultFont;
                            neddReplace = true;
                        }
                    }
                    if (neddReplace)
                    {
                        SaveDealFinishPrefab(instance, path.Substring(path.IndexOf("Assets")));
                        Debug.LogError("repalce Font Success=" + path);
                    }

                    DestroyImmediate(instance);
                }
                else
                {
                    Debug.LogError(path.Substring(path.IndexOf("Assets")));
                }
            }
        }

        if (Directory.GetDirectories(dirPath).Length > 0)  //遍历所有文件夹    
        {
            foreach (string path in Directory.GetDirectories(dirPath))
            {
                ReplaceSystemFontInDirs(path, ref dirs);
            }
        }
    }


    private static void SaveDealFinishPrefab(GameObject go, string path)
    {
        if (File.Exists(path) == true)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            PrefabUtility.ReplacePrefab(go, prefab);
        }
        else
        {
            PrefabUtility.CreatePrefab(path, go);
        }
    }

}