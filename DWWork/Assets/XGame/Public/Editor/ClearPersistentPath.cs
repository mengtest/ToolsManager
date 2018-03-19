/************************************************************
//     文件名      : ClearPersistentPath.cs
//     功能描述    : 
//     负责人      : shandong   shandong@ezfun.cn
//     参考文档    : 无
//     创建日期    : 2017-08-22 19:44:53.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.IO;
public class ClearPersistentPath  {


	[MenuItem("EZFun/Cleanup/清理更新")]
	public static void ClearPersistentFiles()
	{

		var path = Application.persistentDataPath;
		DeleteFolder (path);
	}


	static void DeleteFolder(string path)
	{
		var dir = new DirectoryInfo (path);
		var files = dir.GetFiles ("*.*", SearchOption.AllDirectories);
		foreach (var file in files) {
			File.Delete (file.FullName);
		}
		var cdirs = dir.GetDirectories ();
		foreach (var cdir in cdirs) {
			DeleteFolder (cdir.FullName);
			Directory.Delete (cdir.FullName);
		}
	}

    [MenuItem("EZFun/Cleanup/Cleanup Selected Missing Scripts")]
    static void CleanupMissingScripts()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            var gameObject = Selection.gameObjects[i];
            RecurseClearMissingComponent(gameObject);
        }
    }

    [MenuItem("EZFun/Cleanup/Cleanup Missing Scripts In All prefab")]
    static void CleanupMissingScriptsInAllprefab()
    {
        //Add by Atin ,遍历目录用队列，层次遍历，遍历文件就一次遍历
        Queue<DirectoryInfo> dirInfoQue = new Queue<DirectoryInfo>();
        if (Directory.Exists(Application.dataPath))
        {
            dirInfoQue.Enqueue(new DirectoryInfo(Application.dataPath));//根目录进队列
        }
        DirectoryInfo[] dirInfosTemp = null;
        DirectoryInfo curDir = null;
        DirectoryInfo dtemp = null;

        FileInfo[] fileInfosTemp = null;
        FileInfo curFile = null;

        IEnumerator fenm = null;
        IEnumerator denm = null;

        GameObject go = null;

        string fileName = null;
        string prefabName = null;

        while (dirInfoQue.Count > 0)
        {
            curDir = dirInfoQue.Dequeue();//设置当前选中的dir

            //将所有子目录进队列
            dirInfosTemp = curDir.GetDirectories();
            denm = dirInfosTemp.GetEnumerator();
            while (denm.MoveNext())
            {
                dtemp = denm.Current as DirectoryInfo;
                if (dtemp != null)
                {
                    dirInfoQue.Enqueue(dtemp);
                }
            }

            //遍历当前目录下所有文件，看有没有*.prefab文件
            fileInfosTemp = curDir.GetFiles("*.prefab");
            fenm = fileInfosTemp.GetEnumerator();
            while (fenm.MoveNext())
            {
                curFile = fenm.Current as FileInfo;
                fileName = curFile.FullName.Replace("\\", "/");
                prefabName = fileName.Substring(fileName.IndexOf("Assets"));
                GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabName, typeof(GameObject)) as GameObject;
                if (prefab != null)
                {
                    go = GameObject.Instantiate(prefab) as GameObject;
                    if (go != null)
                    {
                        bool hasChange = RecurseClearMissingComponent(go);
                        if (hasChange)
                        {
                            PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        GameObject.DestroyImmediate(go);
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Cleanup Missing Scripts In All prefab Finish!");
    }

    static bool RecurseClearMissingComponent(GameObject gameObject)
    {
        bool hasChange = false;
        hasChange |= ClearMissingComponent(gameObject);
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            hasChange |= RecurseClearMissingComponent(gameObject.transform.GetChild(i).gameObject);
        }
        return hasChange;
    }

    static bool ClearMissingComponent(GameObject gameObject)
    {
        bool hasChange = false;
        // We must use the GetComponents array to actually detect missing components
        var components = gameObject.GetComponents<Component>();

        // Create a serialized object so that we can edit the component list
        var serializedObject = new SerializedObject(gameObject);
        // Find the component list property
        var prop = serializedObject.FindProperty("m_Component");

        // Track how many components we've removed
        int r = 0;

        // Iterate over all components
        for (int j = 0; j < components.Length; j++)
        {
            // Check if the ref is null
            if (components[j] == null)
            {
                // If so, remove from the serialized component array
                prop.DeleteArrayElementAtIndex(j - r);
                // Increment removed count
                r++;
                hasChange = true;
            }
        }

        // Apply our changes to the game object
        serializedObject.ApplyModifiedProperties();
        return hasChange;
    }
}
