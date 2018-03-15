/************************************************************
//     文件名      : MonsterAnimControllerToPrefab.cs
//     功能描述    : 根据怪物动画控制器转化prefab引用
//     负责人      : lezen   lezenzeng@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-08-04 18:11:43.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor.Animations;
public class MonsterAnimControllerToPrefab {

    [MenuItem("EZFun/Animator/生成 Monster AnimatorController prefab 引用")]
    public static void MainFunc()
    {
        Debug.Log("Start Departing.");

        string animatorControllerPath = "Assets/XGame/Data/AnimatorController/monster/";
        string animatorControllerPrefabPath = Application.dataPath + "/XGame/Resources/Prefab/AnimatorController/Monster/";
        string animatorControllerPrefabAssetPath = "Assets/XGame/Resources/Prefab/AnimatorController/Monster/";
        string[] paths = Directory.GetFiles(animatorControllerPath, "*.*", SearchOption.AllDirectories);
        List<AnimatorController> list = new List<AnimatorController>();
        for (int i = 0; i < paths.Length; i++)
        {
            if (IsAnimatorController(paths[i]))
            {
                var obj = AssetDatabase.LoadAssetAtPath<AnimatorController>(paths[i]);
                if (obj != null)
                {
                    list.Add(obj);
                }
            }
        }
        DirectoryInfo direct = new DirectoryInfo(animatorControllerPrefabPath);
        if (direct.Exists)
        {
            direct.Delete(true);
        }
        direct.Create();
        for (int i = 0; i < list.Count; i ++)
        {
            GameObject acGB = new GameObject(list[i].name);
            var animtor = acGB.AddComponent<Animator>();
            animtor.runtimeAnimatorController = list[i];
            animtor.updateMode = AnimatorUpdateMode.Normal;
            animtor.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            GameObject prefab = PrefabUtility.CreatePrefab(animatorControllerPrefabAssetPath + acGB.name + ".prefab", acGB);
            MonoBehaviour.DestroyImmediate(acGB);
        }
      
        AssetDatabase.Refresh();    //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file  
        Debug.Log("Finish Departing.");
    }


    [MenuItem("EZFun/Animator/指定 Monster prefab 默认简单动画控制器")]
    public static void SetMonsterSimpleAnimator()
    {
        Debug.Log("Start Departing.");

        string monsterPrefabPath = "Assets/XGame/Resources/Prefab/monster/";
        string animatorControllerPrefabAssetPath = "Assets/XGame/Data/AnimatorController/monster/";
        string[] paths = Directory.GetFiles(monsterPrefabPath, "*.*", SearchOption.AllDirectories);
        List<Animator> list = new List<Animator>();
        for (int i = 0; i < paths.Length; i++)
        {
            if (IsPrefab(paths[i]))
            {
                //string assetPath = paths[i].Substring(paths[i].IndexOf("Assets"));
                var obj = AssetDatabase.LoadAssetAtPath(paths[i], typeof(GameObject)) as GameObject;
                if (obj != null)
                {
                    var animator = obj.GetComponent<Animator>();
                    if(animator != null)
                    {
                        list.Add(animator);
                    }

                }
            }
        }

        for(int i = 0; i < list.Count; i ++)
        {
            var animCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorControllerPrefabAssetPath + list[i].gameObject.name + "_simple.controller");
            if (animCtrl != null)
                list[i].runtimeAnimatorController = animCtrl;
            else if(list[i].gameObject.name.Contains("_"))
            {
                string gbName = list[i].gameObject.name.Substring(0,list[i].gameObject.name.LastIndexOf("_"));
                animCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorControllerPrefabAssetPath + gbName + "_simple.controller");
                if(animCtrl != null)
                    list[i].runtimeAnimatorController = animCtrl;
                else
                {
                    animCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorControllerPrefabAssetPath + gbName + ".controller");
                    if (animCtrl != null)
                        list[i].runtimeAnimatorController = animCtrl;
                }
            }
            else
            {
                animCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorControllerPrefabAssetPath + list[i].gameObject.name + ".controller");
                if (animCtrl != null)
                    list[i].runtimeAnimatorController = animCtrl;
            }
        }
        AssetDatabase.Refresh();    //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file  
        Debug.Log("Finish Departing.");

    }

    public static bool IsAnimatorController(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".controller");
    }

    public static bool IsPrefab(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".prefab") && !path.EndsWith(".meta");
    }
}
