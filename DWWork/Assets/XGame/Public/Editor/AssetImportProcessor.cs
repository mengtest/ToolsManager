/************************************************************
//     文件名      : AssetImportProcessor.cs
//     功能描述    : 美术资源导入处理器
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/12/19.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class AssetImportProcessor : AssetPostprocessor
{
    public const string TargetMonsterPath = "Data/Animation/monster/";
    public const string TargetMalePlayerPath = "Data/Animation/male/";
    public const string TargetFemalePlayerPath = "Data/Animation/female/";
    public const string TargetMountPath = "Data/Animation/mount/";
    public const string TargetPetPath = "Data/Animation/pet/";
    public const string TargetWeaponPath = "Data/Animation/weapon/";
    public const string SceneModelPath = "Data/models/";


    public void OnPreprocessTexture()
    {
        OnTextureCommmonProcess();
    }

    private void OnTextureCommmonProcess()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        //importer.isReadable = false;
        if (importer.assetPath.Contains("Resources"))
        {
           // EZFunTextureTools.HandleTexturesToPrefab();
        }
        //importer.maxTextureSize = Mathf.Min(2048, importer.maxTextureSize);
    }

    public void OnPreprocessModel()
    {
        OnModelCommonProcess();
        OnModelCompressAnimation();
    }

    private void OnPostprocessModel(GameObject gb)
    {
        OnModelDeleteScaleCurve(gb);
        ClearMeshUVAndColorChannel(gb);
    }

    private void ClearMeshUVAndColorChannel(GameObject rImportModel)
    {
        List<Vector2> rNewUV = null;
        List<Color32> rNewColor = null;
        var rFilters = rImportModel.GetComponentsInChildren<MeshFilter>();
        for (int filter_index = 0; filter_index < rFilters.Length; filter_index++)
        {
            //rFilters[filter_index].sharedMesh.SetColors(rNewColor);
            rFilters[filter_index].sharedMesh.SetUVs(2, rNewUV);
            rFilters[filter_index].sharedMesh.SetUVs(3, rNewUV);
        }
    }

    private void OnModelCommonProcess()
    {
        ModelImporter modelImporter = (ModelImporter)assetImporter;
        if (modelImporter.isReadable)
        {
            modelImporter.isReadable = false;
            modelImporter.SaveAndReimport();
        }
        modelImporter.importBlendShapes = false;
        //modelImporter.meshCompression = ModelImporterMeshCompression.Off;
    }

    /// <summary>
    /// 所有导入动画在导入的时候进行压缩减帧
    /// </summary>
    /// 
    private void OnModelCompressAnimation()
    {
        ModelImporter modelImporter = (ModelImporter)assetImporter;
        modelImporter.animationCompression = ModelImporterAnimationCompression.KeyframeReduction;
        string pathName = modelImporter.assetPath;
        float compressScale = 0.5f;
        if (pathName.Contains("male_kan"))
        {
            compressScale = 0.5f;
        }
        else
        if (pathName.Contains(TargetMonsterPath))
        {
            if (!(pathName.Contains("idle") || pathName.Contains("collapseup")))
            {
                compressScale = 2;
            }
            else
            {
                compressScale = 0.5f;
            }
        }
        else if (pathName.Contains(TargetMalePlayerPath) || pathName.Contains(TargetFemalePlayerPath))
        {
            if (pathName.Contains("xuanjue"))
            {
                compressScale = 0;
            }else
            if (pathName.Contains("idle") || pathName.Contains("stand") ||
                pathName.Contains("standby") || pathName.Contains("standup") ||
                pathName.Contains("run") || pathName.Contains("victory") ||
                pathName.Contains("run"))
            {
                compressScale = 0.5f;
            }
            else
            {
                compressScale = 2.0f;
            }
        }
        else if (pathName.Contains(TargetMountPath))
        {
            compressScale = 0.5f;
        }
        else if (!pathName.Contains(SceneModelPath) &&
                !pathName.Contains("jingmai"))
        {
            compressScale = 2.0f;
        }

        modelImporter.animationRotationError = compressScale;
        modelImporter.animationPositionError = compressScale;
        modelImporter.animationScaleError = compressScale;

    }
    /// <summary>
    ///   所有导入动画在导入的时候删除ScaleCurve
    /// </summary>
    /// 
    private void OnModelDeleteScaleCurve(GameObject gb)
    {
        //处理导入的模型中的动画
        if (AssetImportTools.IsOptiamizeAnimationPath(assetPath))
        {
            List<AnimationClip> animationClipList = new List<AnimationClip>(AnimationUtility.GetAnimationClips(gb));
            if (animationClipList.Count == 0)
            {
                AnimationClip[] objectList = UnityEngine.Object.FindObjectsOfType(typeof(AnimationClip)) as AnimationClip[];
                animationClipList.AddRange(objectList);
            }

            foreach (AnimationClip theAnimation in animationClipList)
            {
                AssetImportTools.DeleteScaleCurve(theAnimation);
            }
        }
    }

}