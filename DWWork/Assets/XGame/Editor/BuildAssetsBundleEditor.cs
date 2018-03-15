using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using ProtoBuf;
using tools.CAssetBundleData;

public class BuildAssetsBundleEditor 
{
	static BuildTarget m_target = BuildTarget.StandaloneWindows;
	static BuildAssetBundleOptions m_options = BuildAssetBundleOptions.DeterministicAssetBundle;
	static List<UnityEngine.Object> m_selectedOBLs = new List<UnityEngine.Object>();
	static List<string> m_itemNameLs = new List<string>();
	static ResCABDataStructLs m_abDataLs = new ResCABDataStructLs();
	//[MenuItem("EZFun/build/Build XGame AssetsBundle &%#x")]
	static void HandBuildAB()
	{
		BuildAssetBundle(2);
	}

	//[MenuItem("EZFun/Assets/AssetsBundle")]
	static void BuildSingleAB()
	{
		m_options = BuildAssetBundleOptions.UncompressedAssetBundle;
		m_target = BuildTarget.StandaloneWindows;

		BuildPipeline.BuildAssetBundle(Selection.activeObject, null, Application.streamingAssetsPath + "/" + Selection.activeObject.name,
		                               m_options, m_target);
	}

	//[MenuItem("EZFun/Assets/UI AssetsBundle")]
	static void BuildUISingleAB()
	{
		m_options = BuildAssetBundleOptions.UncompressedAssetBundle;
		m_target = BuildTarget.StandaloneWindows;

		RemoveUIRess("Altas");
		RemoveUIRess("Font");
		AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundle(Selection.activeObject, null, Application.streamingAssetsPath + "/" + Selection.activeObject.name + ".assetbundle",
		                               m_options, m_target);
		RecoveryUIRess("Altas");
		RecoveryUIRess("Font");
		AssetDatabase.Refresh();
	}

	static void RemoveUIRess(string name)
	{
		string path = Application.dataPath + "/XGame/Resources/EZFunUI/" + name;
        var disk = Application.dataPath.Substring(0, 3);
		string destPath = disk + name;

		DirectoryInfo direct = new DirectoryInfo(path);
		direct.MoveTo(destPath);
	}

	static void RecoveryUIRess(string name)
	{
        var disk = Application.dataPath.Substring(0, 3);
		string path = disk + name;
		string destPath = Application.dataPath + "/XGame/Resources/EZFunUI/" + name;
		
		DirectoryInfo direct = new DirectoryInfo(path);
		direct.MoveTo(destPath);
	}

	public static void BuildAndrokdAB()
	{
		BuildAssetBundle(0);
	}

    static void InitArgs(int state)
    {
		m_abDataLs.list.Clear();
		
		m_options  = BuildAssetBundleOptions.DeterministicAssetBundle;
		if(state == 0)
		{	
//			m_options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.UncompressedAssetBundle;
			m_target = BuildTarget.Android;
		}
		else
		{
			m_options = BuildAssetBundleOptions.UncompressedAssetBundle;
			if(state == 1)
			{
				m_target = BuildTarget.iOS;
			}
			else if(state == 2)
			{
				m_target = BuildTarget.StandaloneWindows;
			}
		}
    }

	public static void BuildAssetBundle(int state = 0)//0:apk 1: ios: 2: editor
	{
		DirectoryInfo direc =  new DirectoryInfo(Application.streamingAssetsPath + "/AssetsBundle");
		if(direc.Exists)
		{
			direc.Delete(true);
		}

        InitArgs(state);
		if(state == 0 || state == 1)
		{
			AutoBuild.RemoveRessToTmp();
        }
		BuildAB(state);
		
		AssetDatabase.Refresh();
	}

	public static string BuildUpdateAssetBundle(int state, string version)//0:apk 1: ios: 2: editor
	{
        InitArgs(state);
		m_options = BuildAssetBundleOptions.UncompressedAssetBundle;
		 
        var ABPath = GenUpdatePack.OpenFolderPanel("选择AssetsBundle文件夹", Application.dataPath, "");
        if (ABPath == null)
        {
            return null;
        }
        var targetPath = Application.streamingAssetsPath + "/AssetsBundle/Update/" + version;
        targetPath = targetPath + "/AssetsBundle";

		DirectoryInfo abDirec = new DirectoryInfo(targetPath);
		if(!abDirec.Exists)
		{
			abDirec.Create();
		}

		RemoveUIRess("Altas");
		RemoveUIRess("Font");
		AssetDatabase.Refresh();

        BuildAB(state, ABPath, targetPath + "/" + version + ".assetbundle", version.ToLower());

		if(m_selectedOBLs != null && m_selectedOBLs.Count > 0)
		{
			SaveABDataFile(version, targetPath + "/");
		}

		RecoveryUIRess("Altas");
		RecoveryUIRess("Font");
		AssetDatabase.Refresh();
		
		BuildUpdateScene(state, version, ABPath, targetPath);

        return targetPath;
	}

	public static void BuildUpdateScene(int state, string version, string path, string targetPath)
	{
		BuildTargetGroup m_group = BuildTargetGroup.Standalone;
		switch(state)
		{
		case 0:
			m_group = BuildTargetGroup.Android;
			break;
		case 1:
			m_group = BuildTargetGroup.iOS;
			break;
		case 2:
			m_group = BuildTargetGroup.Standalone;
			break;
		}
		PlayerSettings.SetScriptingDefineSymbolsForGroup(m_group, "BUILD_EXP");
		AssetDatabase.Refresh();

		DirectoryInfo direc = new DirectoryInfo(path);	
		FileInfo[] fileArray = direc.GetFiles();
		for(int fileIndex = 0; fileArray != null && fileIndex < fileArray.Length; fileIndex ++)
		{
			FileInfo file = fileArray[fileIndex];
			string fileName = file.Name;
			if(!fileName.EndsWith("unity"))
			{
				continue;
			}

			fileName = fileName.Replace(".unity", "");
			string mapFilePath = targetPath + "/" + fileName + ".abMap";
			ResCABDataStructLs itemData = new ResCABDataStructLs();
			CAssetBundleDataStruct item = new CAssetBundleDataStruct();
			item.m_ressGbName = fileName;
			item.m_storeAssetBundleName = "levelScene" + fileName;
			item.m_ressVersion = Version.GetVersion(version);
			itemData.list.Add(item);
			using(Stream abFile = File.Create(mapFilePath))
			{
				Serializer.Serialize(abFile, itemData);
				abFile.Close();
			}	

			string[] folderArray = file.FullName.Split(Path.DirectorySeparatorChar);
			string sceneName = "";
			bool inAsset = false;
			for(int folderIndex = 0; folderIndex < folderArray.Length; folderIndex ++)
			{
				if(folderArray[folderIndex] == "Assets")
				{
					inAsset = true;
				}

				if(inAsset)
				{
					sceneName += folderArray[folderIndex];
					if(!folderArray[folderIndex].EndsWith("unity"))
					{
						sceneName += "/";	
					}
				}
			}

			string saveSceneName = targetPath + "/" + item.m_storeAssetBundleName + ".lvData";
			BuildPipeline.BuildStreamedSceneAssetBundle(new string[1]{sceneName}, saveSceneName, m_target, BuildOptions.UncompressedAssetBundle);
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(m_group, "");
		AssetDatabase.Refresh();
	}

    static void BuildAB(int state, string fromPath, string toPath, string version)
    {
		//BuildPipeline.PushAssetDependencies();
		BuildUpdateAB(fromPath, toPath, version);
		//BuildPipeline.PopAssetDependencies();
    }

	static void BuildAB(int state)
	{
		string path = "";
		if(state == 2)
		{
			path = "XGame/Resources/Prefab";
		}
		else
		{
			path = "XGame/Temp";
		}

		m_selectedOBLs.Clear();
		m_itemNameLs.Clear();
		GetOBFromDirectinfo(path, false);
		SaveAssetsBundle(m_selectedOBLs, "localAB", m_itemNameLs);
		
		SaveABDataFile("abDataStruct");
	}

	static void BuildUpdateAB(string fromPath, string toPath, string version)
    {
		m_selectedOBLs.Clear();
		m_itemNameLs.Clear();
		GetOBFromDirectinfo(fromPath, true);
        ActiveAllNodeOfPrefab(ref m_selectedOBLs);
		
		SaveAssetsBundle(m_selectedOBLs, version, m_itemNameLs, toPath, Version.GetVersion(version));
    }

    // 制作ab前要将prefab所有的节点激活一次
    static private void ActiveAllNodeOfPrefab(ref List<UnityEngine.Object> obLs)
    {
        var activedList = new List<Transform>();
        for (int i = 0; i < obLs.Count; ++i)
        {
            var ob = obLs[i] as GameObject;
            if (ob == null)
            {
                Debug.LogError("active all nodes error " + obLs[i].name);
                continue;
            }
            ActiveAllNodeOfPrefab(ref ob, ref activedList);
        }
        for (int i = 0; i < activedList.Count; ++i)
        {
            activedList[i].gameObject.SetActive(false);
        }
    }

    static private void ActiveAllNodeOfPrefab(ref GameObject ob, ref List<Transform> activedList)
    {
        for (int i = 0; i < ob.transform.childCount; ++i)
        {
            var child = ob.transform.GetChild(i);
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                activedList.Add(child);
                if (child.childCount > 0)
                {
                    var go = child.gameObject;
                    ActiveAllNodeOfPrefab(ref go, ref activedList);
                }
            }
            else
            {
                var go = child.gameObject;
                ActiveAllNodeOfPrefab(ref go, ref activedList);
            }
        }
    }

	//ingoreDirecName 忽略的文件夹名
	static void GetOBFromDirectinfo(string path, bool isUpdate = false)
	{
        DirectoryInfo direc;
        if (isUpdate)
        {
            direc = new DirectoryInfo(path);
        }
        else
        {
            direc = new DirectoryInfo(Application.dataPath + "/" + path);
        }

		FileInfo[] fileArray = direc.GetFiles();
		for(int fileIndex = 0; fileArray != null && fileIndex < fileArray.Length; fileIndex ++)
		{
			FileInfo file = fileArray[fileIndex];
			string fileName = file.Name;
			if(!fileName.EndsWith("prefab") && !fileName.EndsWith("exr"))
			{
				continue;
			}

            if (isUpdate)
            {
                var filePath = path + "/" + fileName;
                var ps = filePath.Split(new string[]{"/Assets/"}, StringSplitOptions.None);
                m_selectedOBLs.Add(AssetDatabase.LoadMainAssetAtPath("Assets/" + ps[1]));
            }
            else
            {
                m_selectedOBLs.Add(AssetDatabase.LoadMainAssetAtPath("Assets/" + path + "/" + fileName));
            }
			m_itemNameLs.Add(fileName.Split('.')[0]);
		}
		
		DirectoryInfo[] childDirect = direc.GetDirectories();
		for(int childIndex = 0; childDirect != null && childIndex < childDirect.Length; childIndex ++)
		{
			string childPath = path + "/" + childDirect[childIndex].Name;
			GetOBFromDirectinfo(childPath);
		}
	}
	
	static void SaveAssetsBundle(List<UnityEngine.Object> obLs, string saveName, List<string> obNameLs, string toPath = "", int version = -1)
	{
        string savePath;
        if (toPath.Equals(""))
        {
            DirectoryInfo direc = new DirectoryInfo(Application.streamingAssetsPath + "/AssetsBundle");
            if (!direc.Exists)
            {
                direc.Create();
            }
            savePath = Application.streamingAssetsPath + "/AssetsBundle/" + saveName + ".assetbundle";
        }
        else
        {
            savePath = toPath;
        }

		UnityEngine.Object[] SelectedAsset = obLs.ToArray();
		List<string> nameLs = new List<string>();
		for(int i = 0; i < obNameLs.Count; i ++)
		{
			nameLs.Add("AssetsBundle_" + obNameLs[i].ToLower());
		}

		if(SelectedAsset == null || SelectedAsset.Length == 0)
		{
			return;
		}

		BuildPipeline.BuildAssetBundleExplicitAssetNames(SelectedAsset, nameLs.ToArray(), savePath, m_options, m_target);
		
		for(int i = 0; i < obNameLs.Count; i ++)
		{
			string ressName = obNameLs[i].ToLower();
			CAssetBundleDataStruct cAB = m_abDataLs.list.Find((CAssetBundleDataStruct findValue)=>{
				return findValue.m_ressGbName == ressName ;
			});
			
			if(cAB == null)
			{
				CAssetBundleDataStruct item = new CAssetBundleDataStruct();
				item.m_ressGbName = ressName;
				item.m_storeAssetBundleName = saveName;
				if(version == -1)
				{
					item.m_ressVersion = Version.Instance.GetRessVersion();
				}
				else
				{
					item.m_ressVersion = version;
				}
				m_abDataLs.list.Add(item);
			}
		}
	}
	
	static void SaveABDataFile(string fileName, string savePath = "")
	{
        string path;
        if (savePath.Equals(""))
        {
            path = Application.streamingAssetsPath + "/AssetsBundle/" + fileName + ".abMap";
        }
        else
        {
            path = savePath + fileName + ".abMap";
        }
		using(Stream file = File.Create(path))
		{
			Serializer.Serialize(file, m_abDataLs);
			file.Close();
		}
	}
    [MenuItem("EZFun/Lua/GenCrypteLua")]
	public static void GenLuaFiles()
	{
		var sPath = Application.dataPath + Path.DirectorySeparatorChar +
			"XGame" + Path.DirectorySeparatorChar +
				"Script" + Path.DirectorySeparatorChar +
				"Lua" + Path.DirectorySeparatorChar;
		
		var dPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar +
			"Lua" + Path.DirectorySeparatorChar;
		
		var splitChrs = new string[] {"/", "\\", "//"};
        GlobalCrypto.InitCry(Constants.CryptoIV, Constants.CryptoKey);

        GenUpdatePack.ForeachFile(sPath, (string path) =>
		 {
			Debug.LogWarning("genfile " + path);
			if (!Path.GetExtension(path).Equals("meta"))
			{
				
                var fileName = path.Substring(path.IndexOf("Lua") + "Lua".Length);
                 var strArray = fileName.Split(splitChrs, StringSplitOptions.None);

                 var directStr = dPath;
                 for (int i = 0; i < strArray.Length - 1; i++)
                 {
                     directStr += strArray[i] + Path.DirectorySeparatorChar;
                     if (!Directory.Exists(directStr))
                     {
                         Directory.CreateDirectory(directStr);
                     }
                 }

                 var b = File.ReadAllBytes(path);
                 var cb = GlobalCrypto.Encrypte(b);
                 File.WriteAllBytes(dPath + fileName, cb);
             }
		});
	}
}
