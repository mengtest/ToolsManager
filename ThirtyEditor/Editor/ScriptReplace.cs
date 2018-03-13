/************************************************************
//     文件名      : ScriptReplace.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2016-12-29 20:30:24.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
public class ScriptReplace
{


   // [MenuItem("EZFun/Prefab/ReplaceScript")]
    public static void ReplaceScriptEditor()
    {
        var dir = new DirectoryInfo(Application.dataPath + "/XGame/Resources/Prefab/SkillItem");
        var files = dir.GetFiles("*.prefab", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            var ps = files[i].FullName.Split(new string[] { "\\Resources\\" }, StringSplitOptions.None);
            var ob = Resources.Load(ps[1].Split('.')[0]);
            GameObject gb = GameObject.Instantiate(ob) as GameObject;
            ReplaceScript(gb, ob);
            GameObject.DestroyImmediate(gb);
        }
    }

   // [MenuItem("EZFun/Prefab/ReplaceCurSceneScript")]
    public static void ReplaceCurSceneScriptEditor()
    {
        var objs = Selection.gameObjects;
        for (int i = 0; i < objs.Length; i++)
        {
            ReplaceScript(objs[i], null);
        }
    }


    private static void ReplaceScript(GameObject gb, UnityEngine.Object prefab)
    {
        List<Type> oldList = new List<Type>();
        List<Type> newList = new List<Type>();
        //oldList.Add(typeof(BadUVController));
        //newList.Add(typeof(EZFunBadUVController));


        ////oldList.Add(typeof(ParticleMask));
        ////newList.Add(typeof(EZFunParticleMask));

        //oldList.Add(typeof(DissolveController));
        //newList.Add(typeof(EZFunDissolveController));

        //oldList.Add(typeof(GlobalFog));
        //newList.Add(typeof(EZFunGlobalFog));

        //oldList.Add(typeof(EZFunRenderQueueController));
        //newList.Add(typeof(EZFunRenderQueueController));

        //oldList.Add(typeof(UVController));
        //newList.Add(typeof(EZFunUVController));

        //oldList.Add(typeof(WaterFoamAnimation));
        //newList.Add(typeof(EZFunWaterFoamAnimation));

        //oldList.Add(typeof(WaterMoveTime));
        //newList.Add(typeof(EZFunWaterMoveTime));

        //oldList.Add(typeof(EZFunDelayShowFx));
        //newList.Add(typeof(EZFunDelayShowFx));

        //oldList.Add(typeof(SpriteAnimator));
        //newList.Add(typeof(EZFunSpriteAnimator));

        bool isReplace = false;
        for (int i = 0; i < oldList.Count; i++)
        {
            var hasReplace = ReplaceType(gb, oldList[i], newList[i]);
            if (hasReplace)
            {
                isReplace = true;
            }
        }
        if (isReplace && prefab != null)
        {
            PrefabUtility.ReplacePrefab(gb, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }
    }

    private static bool ReplaceType(GameObject gb, Type type1, Type type2)
    {
        var values = gb.GetComponentsInChildren(type1, true);
        bool isReplace = false;
        for (int i = 0; i < values.Length; i++)
        {
            var ezBadUV = values[i].gameObject.AddComponent(type2);
            var newValue = (ezBadUV);
            var oldValue = values[i];
            var fileds = type1.GetFields();
            var newFileds = type2.GetFields();
            for (int f = 0; f < fileds.Length; f++)
            {
                object oldFieldValue = fileds[f].GetValue(oldValue);
                newFileds[f].SetValue(newValue, oldFieldValue);
            }
            Debug.LogError("Replace script :" + gb.name + " script:" + type1.Name);
            isReplace = true;
            GameObject.DestroyImmediate(oldValue);
        }
        return isReplace;
    }

    private void ReplaceBadUVController(GameObject gb)
    {
        //var values = gb.GetComponentsInChildren<BadUVController>(true);
        //for (int i = 0; i < values.Length; i++)
        //{
        //    var ezBadUV = values[i].gameObject.AddComponent<EZFunBadUVController>();
        //    ezBadUV.targetMaterialSlot = values[i].targetMaterialSlot;
        //    ezBadUV.speedX = values[i].speedX;
        //    ezBadUV.speedY = values[i].speedY;
        //    ezBadUV.speedX2 = values[i].speedX2;
        //    ezBadUV.speedY2 = values[i].speedY2;
        //}
    }

    private void ReplaceEZFunDissolveController(GameObject gb)
    {
        //var values = gb.GetComponentsInChildren<DissolveController>(true);
        //for (int i = 0; i < values.Length; i++)
        //{
        //    var ezBadUV = values[i].gameObject.AddComponent<EZFunDissolveController>();
        //    ezBadUV.m_fDestruktionSpeed = values[i].m_fDestruktionSpeed;
        //    ezBadUV.m_Mat = values[i].m_Mat;
        //    ezBadUV.m_fTime = values[i].m_fTime;
        //}
    }

    private void ReplaceEZFunGlobalFog(GameObject gb)
    {
        //var values = gb.GetComponentsInChildren<GlobalFog>(true);
        //for (int i = 0; i < values.Length; i++)
        //{
        //    var ezBadUV = values[i].gameObject.AddComponent<EZFunGlobalFog>();
        //    ezBadUV.distanceFog = values[i].distanceFog;
        //    ezBadUV.useRadialDistance = values[i].useRadialDistance;
        //    ezBadUV.heightFog = values[i].heightFog;
        //    ezBadUV.height = values[i].heightDensity;
        //    ezBadUV.distanceFog = values[i].distanceFog;
        //    ezBadUV.fogShader = values[i].fogShader;
        //}
    }

}
