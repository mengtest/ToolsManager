using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Threading;
using System;

public static class AssetImportTools
{
    public const string AnimaitonPath = "Assets/XGame/Data/Animation/";
    public static string[] expanPath = { "male", "female", "monster", "mount", "pet", "weapon", "Camera", "cutscene" };

    [MenuItem("EZFun/优化动画尺寸")]
    public static void deleteFunc()
    {
        List<string> dirs = new List<string>();
        for (int i = 0; i < expanPath.Length; i++)
        {
            GetDirs(AnimaitonPath + expanPath[i], ref dirs);
        }
        AssetDatabase.StartAssetEditing();
        for (int i = 0; i < dirs.Count; i++)
        {
            HandleScaleCurve(dirs[i]);
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
        Debug.Log("success");
    }
    /// <summary>
    ///  获取文件路径
    /// </summary>
    /// 
    public static void GetDirs(string dirPath, ref List<string> dirs)
    {
        DirectoryInfo di = new DirectoryInfo(dirPath);
        var files = di.GetFiles("*?.anim", SearchOption.AllDirectories);
        foreach (var fi in files)
        {
            string path = fi.FullName;
            dirs.Add(path.Substring(path.IndexOf("Assets")));
        }
    }
    /// <summary>
    ///  执行删除操作
    /// </summary>
    /// 
    public static void HandleScaleCurve(string path)
    {
        AnimationClip animclip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        DeleteScaleCurve(animclip);
    }

    public static void DeleteScaleCurve(AnimationClip clip)
    {
        foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(clip))
        {
            string name = theCurveBinding.propertyName.ToLower();
            if (name.Contains("scale"))
            {
                AnimationUtility.SetEditorCurve(clip, theCurveBinding, null);
            }
            else
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, theCurveBinding);
                if (curve != null && curve.keys != null && curve.length > 0)
                {
                    Keyframe[] keys = new Keyframe[curve.length];
                    for (int i = 0; i < curve.length; ++i)
                    {
                        keys[i].value = (float)Math.Round((double)curve.keys[i].value, 3);
                        keys[i].inTangent = (float)Math.Round((double)curve.keys[i].inTangent, 3);
                        keys[i].outTangent = (float)Math.Round((double)curve.keys[i].outTangent, 3);
                        keys[i].time = (float)Math.Round((double)curve.keys[i].time, 3);
                        keys[i].value = (float)Math.Round((double)curve.keys[i].value, 3);
                        keys[i].tangentMode = curve.keys[i].tangentMode;
                    }
                    AnimationUtility.SetEditorCurve(clip, theCurveBinding, new AnimationCurve(keys));
                }
            }
        }
        //删掉编辑器绑定关系部分
        //编辑器部分动画文件打包Unity会删除掉，这里不用删除，先注释
        var so = new SerializedObject(clip);
        SerializedProperty m_EulerEditorCurves = so.FindProperty("m_EulerEditorCurves");
        SerializedProperty m_EditorCurves = so.FindProperty("m_EditorCurves");
        m_EditorCurves.ClearArray();
        so.ApplyModifiedProperties();
        m_EulerEditorCurves.ClearArray();
        so.ApplyModifiedProperties();
        so.Dispose();
    }

    public static bool IsOptiamizeAnimationPath(string str)
    {
        string lowerStr = str.ToLower();
        for (int i = 0; i < expanPath.Length; ++i)
        {
            string path = AnimaitonPath + expanPath[i];
            path = path.ToLower();
            if (lowerStr.Contains(path))
                return true;
        }
        return false;
    }
}