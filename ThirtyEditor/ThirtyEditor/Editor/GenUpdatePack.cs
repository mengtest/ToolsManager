
//========================================================================
// Copyright(C): EZFun
//
// CLR Version : 4.0.30319.34209
// NameSpace : Assets.XGame.Editor
// FileName : GenUpdatePack
//
// Created by : dhf at 2/11/2015 9:28:35 AM
//
// Function : 生成资源更新包
//
//========================================================================

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace XGameUpdate
{
	public class UpdateStruct
	{
		public List<SvrInfo> serverInfo = new List<SvrInfo>();
		public List<PackageURL> packageUpdateInfo = new List<PackageURL>();
		public List<UpdatePackageInfo> dynamicUpdateInfo;
		public UpdateStruct()
		{
			dynamicUpdateInfo = new List<UpdatePackageInfo>();
		}
	}
	
	public class SvrInfo
	{
		public int platform = 1;
		public string version = "";
		public SvrInfo()
		{
		}
	}
	
	public class PackageURL
	{
        public int platform = 0;
		public string version = ""; //最小版本号，当自己版本好号比最小版本号小时，需要强制更新
		public string maxVersion = ""; //最大版本号，当自己版本号比最小版本号大，但比最大版本号小时，建议更新
		public string updateUrl = "";
		public string updateDesc = "";
		public PackageURL()
		{
		}
	}
	
	public class UpdatePackageInfo
	{
		public string baseVersion;                              // 版本号（前台）
		public List<AppUpdateInfo> resVersionInfo;     // 平台更新信息
		public UpdatePackageInfo()
		{
			baseVersion = "";
			resVersionInfo = new List<AppUpdateInfo>();
		}
	}
	
	public class AppUpdateInfo
	{
		public int basePlatform;                                // 平台号
		public int testSvrId;
		public string resVersion;
		public bool forceUpdate;
		public List<resInfo> resInfo;
		public AppUpdateInfo()
		{
			basePlatform = 0; 
			testSvrId = 0;
			resInfo = new List<resInfo>();
		}
	}
	
	public class resInfo
	{
		public string type;
		public int size;
		public string md5;
        public string packageName;
		
		public resInfo()
		{
			type = "";
			size = 0;
			md5 = "";
            packageName = "";
		}
		public resInfo(XGameUpdate.UpdateFileInfo fi)
		{
			type = fi.name;
			size = (int)fi.length;
			md5 = fi.md5;
            packageName = fi.packageName;
		}
	}
	
	public class UpdateFileInfo
	{
		public string name;
		public string md5;
		public long length;
        public string packageName;
	}
}

public class GenUpdatePack : EditorWindow 
{
	public class FolderInfo
	{
		public bool versionFolder;
		public List<bool> platformFolder = new List<bool>();
	}
	
	private List<FolderInfo> m_folders = new List<FolderInfo>();
	private string m_jsonFilePath;
	private XGameUpdate.UpdateStruct m_versionInfo = null;
	
	[MenuItem("EZFun/制作更新包")]
	static void ShowWindow()
	{
		var wr = new Rect(0, 0, 500, 500);
		var w = (GenUpdatePack)EditorWindow.GetWindow(typeof(GenUpdatePack));
		w.title = "制作更新包";
		w.InitData();
		w.Show();
	}
	
	void InitData()
	{
		m_versionInfo = new XGameUpdate.UpdateStruct();
	}
	
	void Save()
	{
		var js = JsonMapper.ToJson(m_versionInfo);
		if (string.IsNullOrEmpty(m_jsonFilePath))
		{
            m_jsonFilePath = OpenFolderPanel("选择保存update_config.json文件路径", "", "");
            m_jsonFilePath += "/update_config.json";
		}
		File.WriteAllText(m_jsonFilePath, js);
	}
	
	protected void LoadVUFile(string path)
	{
		try
		{
			var s = File.ReadAllText(path);
			m_versionInfo = JsonMapper.ToObject<XGameUpdate.UpdateStruct>(s);
			ResetFolders();
		}
		catch (System.Exception)
		{
			m_versionInfo = new XGameUpdate.UpdateStruct();
			ResetFolders();
		}
	}
	
	private void ResetFolders()
	{
		m_folders.Clear();
		for (int i = 0; i < m_versionInfo.dynamicUpdateInfo.Count; ++i)
		{
			var fi = new FolderInfo();
			fi.versionFolder = true;
			m_folders.Add(fi);
			fi.platformFolder = new List<bool>();
			for (int j = 0; j < m_versionInfo.dynamicUpdateInfo[i].resVersionInfo.Count; ++j)
			{
				fi.platformFolder.Add(true);
			}
		}
	}
	
	//绘制窗口时调用
	Vector2 m_ScrollViewPos = Vector2.zero;
	void OnGUI()
	{
		m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos, false, false, GUILayout.Height (Screen.height - 30));
		
		GUILayout.Label("当前游戏程序版本号:" + UnityEditor.PlayerSettings.bundleVersion);

        if (GUILayout.Button("选择update_config文件路径"))
		{
            m_jsonFilePath = EditorUtility.OpenFilePanel("选择update_config文件路径", "", "json");
			LoadVUFile(m_jsonFilePath);
		}
		
		GUILayout.Label("");
		
		m_drawAppFolder = EditorGUILayout.Foldout(m_drawAppFolder,new GUIContent("App版本信息"));
		if(m_drawAppFolder)
		{
			DrawAppInfo();
		}
		
		m_drawPackUrl = EditorGUILayout.Foldout(m_drawPackUrl,new GUIContent("安装包信息"));
		if(m_drawPackUrl)
		{
			DrawPackageUrl();
		}
		
		var s = new GUIStyle();
		s.richText = true;
		var text = "";
		GUILayout.Label("");
		var delGameVerIndex = new List<int>();
		for (int i = 0; m_versionInfo != null && m_versionInfo.dynamicUpdateInfo != null && i < m_versionInfo.dynamicUpdateInfo.Count; ++i)
		{
			text = "<color=green>删除程序版本</color>";
			if (GUILayout.Button(text, s))
			{
				delGameVerIndex.Add(i);
			}
			var vui = m_versionInfo.dynamicUpdateInfo[i];
			m_folders[i].versionFolder = EditorGUILayout.Foldout(m_folders[i].versionFolder, "游戏程序版本" + vui.baseVersion + " 更新信息");
			vui.baseVersion = EditorGUILayout.TextField("程序版本:", vui.baseVersion);
			if (m_folders[i].versionFolder)
			{
				var delPlatformIndex = new List<int>();
				for (int j = 0; j < vui.resVersionInfo.Count; ++j)
				{
					text = "<color=yellow>  删除资源版本</color>";
					if (GUILayout.Button(text, s))
					{
						delPlatformIndex.Add(j);
					}
					
					var pui = vui.resVersionInfo[j];
					int platform;
					pui.resVersion = EditorGUILayout.TextField("  兼容资源版本：", pui.resVersion);
					pui.forceUpdate = bool.Parse(EditorGUILayout.TextField("    可否跳过资源版本更新：", pui.forceUpdate.ToString()));
					if (int.TryParse(EditorGUILayout.TextField("    平台:", pui.basePlatform.ToString()), out platform))
					{
						pui.basePlatform = platform;
					}
					if (int.TryParse(EditorGUILayout.TextField("    测试服务器ID:", pui.testSvrId.ToString()), out platform))
					{
						pui.testSvrId = platform;
					}
					var delResVersionIndex = new List<int>();
					
					Action<string, string> ressCB = (string buttonName, string ressName)=>{
						EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb,GUILayout.ExpandWidth(true));
						if (GUILayout.Button(buttonName, GUILayout.Width (120)))
						{
							XGameUpdate.UpdateFileInfo fi = null;
							switch(ressName)
							{
							case "Table":
								fi = GenTableData();
								break;
                            case "AssetBundles":
								//fi = GenAssetsBundle(pui.resVersion);
                                fi = GenAssetsBundle_New();
								break;
							case "Lua":
								fi = GenLua();
								break;
							case "DLL":
								fi = GenDLL();
								break;
							case "MapFile":
								fi = GenMapFile();
								break;
                            case "Audio":
                                fi = GenAudio();
                                break;
							}
							if(fi != null)
							{
								AddUpdateInfo(ref pui, new XGameUpdate.resInfo(fi));
							}
						}
						XGameUpdate.resInfo ressInfo = pui.resInfo.Find((XGameUpdate.resInfo findvalue)=>{
							return findvalue.type == ressName;
						});
						
						if(ressInfo != null)
						{
							ressInfo.md5 = EditorGUILayout.TextField("", ressInfo.md5);
							if(string.IsNullOrEmpty(ressInfo.md5))
							{
								pui.resInfo.Remove(ressInfo);
							}
						}
						else
						{
							EditorGUILayout.TextField("", "");
						}
						EditorGUILayout.EndHorizontal();
					};
					
					ressCB("选择TableData文件夹", "Table");
                    ressCB("选择AssetBundles文件夹", "AssetBundles");
					ressCB("选择DLL文件夹", "DLL");
					ressCB("选择Lua文件夹", "Lua");
					ressCB("选择MapFile文件夹", "MapFile");
                    ressCB("选择Audio文件夹", "Audio");
				}
				for (int j = 0; j < delPlatformIndex.Count; ++j)
				{
					vui.resVersionInfo.RemoveAt(delPlatformIndex[j]);
					ResetFolders();
				}
			}
			text = "<color=yellow>添加资源版本</color>";
			if (GUILayout.Button(text, s))
			{
				var pui = new XGameUpdate.AppUpdateInfo();
				vui.resVersionInfo.Add(pui);
				ResetFolders();
			}
		}
		text = "<color=green>添加程序版本</color>";
		if (GUILayout.Button(text, s))
		{
			var vui = new XGameUpdate.UpdatePackageInfo();
			m_versionInfo.dynamicUpdateInfo.Add(vui);
			ResetFolders();
		}
		for (int i = 0; i < delGameVerIndex.Count; ++i)
		{
			m_versionInfo.dynamicUpdateInfo.RemoveAt(delGameVerIndex[i]);
			ResetFolders();
		}
		GUILayout.Label("");
		if (GUILayout.Button("保存"))
		{
			Save();
		}
		
		EditorGUILayout.EndScrollView();
	}
	
	void AddUpdateInfo(ref XGameUpdate.AppUpdateInfo rv, XGameUpdate.resInfo updateInfo)
	{
		XGameUpdate.resInfo value = rv.resInfo.Find((XGameUpdate.resInfo findValue)=>{
			return findValue.type == updateInfo.type;
		});
		
		if(value != null)
		{
			rv.resInfo.Remove(value);
		}
		
		rv.resInfo.Add(updateInfo);
	}
	
	bool m_drawAppFolder = false;
	void DrawAppInfo()
	{
		var s = new GUIStyle();
		s.richText = true;
		var text = "<color=green>    添加App</color>";
		if (GUILayout.Button(text, s))
		{
			var vui = new XGameUpdate.SvrInfo();
			m_versionInfo.serverInfo.Add(vui);
		}
		
		for (int i = 0; m_versionInfo != null && m_versionInfo.serverInfo != null && i < m_versionInfo.serverInfo.Count; ++i)
		{
			var itenText = "<color=red>          删除App</color>";
			if (GUILayout.Button(itenText, s))
			{
				m_versionInfo.serverInfo.RemoveAt(i);
				return;
			}
			m_versionInfo.serverInfo[i].platform = EditorGUILayout.IntField("               App序号", m_versionInfo.serverInfo[i].platform);
			m_versionInfo.serverInfo[i].version = EditorGUILayout.TextField("               App版本号", m_versionInfo.serverInfo[i].version);
		}
	}
	
	bool m_drawPackUrl = false;
	void DrawPackageUrl()
	{
		var s = new GUIStyle();
		s.richText = true;
		var text = "<color=green>    添加安装包</color>";
		if (GUILayout.Button(text, s))
		{
			var vui = new XGameUpdate.PackageURL();
			m_versionInfo.packageUpdateInfo.Add(vui);
		}
		
		for (int i = 0; m_versionInfo != null && m_versionInfo.packageUpdateInfo != null && i < m_versionInfo.packageUpdateInfo.Count; ++i)
		{
			var itenText = "<color=red>          删除安装包</color>";
			if (GUILayout.Button(itenText, s))
			{
				m_versionInfo.packageUpdateInfo.RemoveAt(i);
				return;
			}
			m_versionInfo.packageUpdateInfo[i].platform = EditorGUILayout.IntField("               安装包平台号", m_versionInfo.packageUpdateInfo[i].platform);
			m_versionInfo.packageUpdateInfo[i].version = EditorGUILayout.TextField("               运行最小版本号", m_versionInfo.packageUpdateInfo[i].version);
			m_versionInfo.packageUpdateInfo[i].maxVersion = EditorGUILayout.TextField("               运行最大版本号", m_versionInfo.packageUpdateInfo[i].maxVersion);
			m_versionInfo.packageUpdateInfo[i].updateUrl = EditorGUILayout.TextField("               安装包下载地址", m_versionInfo.packageUpdateInfo[i].updateUrl);
			m_versionInfo.packageUpdateInfo[i].updateDesc = EditorGUILayout.TextField("               更新内容描述", m_versionInfo.packageUpdateInfo[i].updateDesc);
		}
	}
	
	void OnInspectorUpdate()
	{
		//Debug.Log("窗口面板的更新");
		//这里开启窗口的重绘，不然窗口信息不会刷新
		this.Repaint();
	}
	
	void OnSelectionChange()
	{
		//当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
		foreach(Transform t in Selection.transforms)
		{
			//有可能是多选，这里开启一个循环打印选中游戏对象的名称
			Debug.Log("OnSelectionChange" + t.name);
		}
	}
	
	void OnDestroy()
	{
		Debug.Log("当窗口关闭时调用");
	}
	
	//    [MenuItem("EZFun/制作更新包/TableData", false, 1)]
	static XGameUpdate.UpdateFileInfo GenTableData()
	{
		var targetPath = OpenFolderPanel("选择TableData文件夹", "", "");
		return GenFiles(targetPath, false);
	}
	
	//    [MenuItem("EZFun/制作更新包/Lua", false, 1)]
	static XGameUpdate.UpdateFileInfo GenLua()
	{
		var targetPath = OpenFolderPanel("选择Lua文件夹", "", "");
		return GenFiles(targetPath, true);
	}

    static XGameUpdate.UpdateFileInfo GenAudio()
    {
        var targetPath = OpenFolderPanel("选择Audio文件夹", "", "");
        return GenFiles(targetPath, false);
    }

	static XGameUpdate.UpdateFileInfo GenMapFile()
	{
		var targetPath = OpenFolderPanel("选择Lua文件夹", "", "");
		return GenFiles(targetPath, false);
	}
	//    [MenuItem("EZFun/制作更新包/DLL", false, 1)]
	static XGameUpdate.UpdateFileInfo GenDLL()
	{
		var targetPath = OpenFolderPanel("选择DLL文件夹", "", "");
		return GenFiles(targetPath, false);
	}

    static XGameUpdate.UpdateFileInfo GenAssetsBundle_New() 
    {
        var targetPath = OpenFolderPanel("选择AssetsBundle文件夹", "", "");
        return GenFiles(targetPath, false);
    }
	
	//    [MenuItem("EZFun/制作更新包/StreamingAssets", false, 1)]
    //static XGameUpdate.UpdateFileInfo GenAssetsBundle(string version)
    //{
    //    #if UNITY_ANDROID
    //    var targetPath = BuildAssetsBundleEditor.BuildUpdateAssetBundle(0, version);
    //    #elif UNITY_IPHONE
    //    var targetPath = BuildAssetsBundleEditor.BuildUpdateAssetBundle(1, version);
    //    #else
    //    var targetPath = BuildAssetsBundleEditor.BuildUpdateAssetBundle(2, version);
    //    #endif
    //    if (targetPath == null)
    //    {
    //        return null;
    //    }
    //    return GenFiles(targetPath, false);
    //}
	
	public static string OpenFolderPanel(string title, string folder, string defaultName)
	{
		var targetPath = EditorUtility.OpenFolderPanel(title, folder, defaultName);
		if (string.IsNullOrEmpty(targetPath))
		{
			return null;
		}
		return targetPath;
	}
	
	public static void ForeachFile(string path, Action<string> genFile)
	{
		if (!Directory.Exists(path))
			return;
		
		foreach (var fileStr in Directory.GetFiles(path))
		{
			if (fileStr.Substring(fileStr.Length - 4) == "meta")
			{
				File.Delete(fileStr);
				continue;
			}
			genFile(fileStr);
		}
		foreach (var dirStr in Directory.GetDirectories(path))
		{
			if (Directory.Exists(dirStr))
			{
				ForeachFile(dirStr, genFile);
			}
		}
	}
	
	static XGameUpdate.UpdateFileInfo GenFiles(string inputPath, bool isCry)
	{
		if (inputPath == null || inputPath.Equals(""))
		{
			return null;
		}

        var fi = new XGameUpdate.UpdateFileInfo();
        var dir = inputPath.Split('/');
        //压缩出来的文件加个随机数
        fi.name = dir[dir.Length - 1];
        int randomNum = UnityEngine.Random.Range(0,999999999);
        fi.packageName = fi.name + "_" + randomNum;

		ForeachFile(inputPath, (string f) =>
		            {
			Debug.LogWarning("genfile " + f);
			if (Path.GetExtension(f).Equals("meta"))
			{
				File.Delete(f);
			}
			else
			{
				if (isCry)
				{
					CryFile(f);
				}
			}
		});

        var outPath = inputPath + "_" + randomNum +".zip";
		if (File.Exists(outPath))
		{
			if (File.Exists(outPath + "_bk"))
			{
				File.Delete(outPath + "_bk");
			}
			File.Move(outPath, outPath + "_bk");
		}
		Debug.LogError("zipfile " + inputPath);
        DWTools.FastZipCompress(inputPath, outPath);


        fi.md5 = DWTools.GenMD5(outPath);
        fi.length = GetFileLength(outPath);
       
		return fi;
	}
	
	static long GetFileLength(string filePath)
	{
		var fileInfo = new FileInfo(filePath);
		return fileInfo.Length;
	}
	
	static long GenFile(string inputPath, bool isCry)
	{
		if (isCry)
		{
			CryFile(inputPath);
		}
		var filePath = inputPath + ".zip";
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
        DWTools.FastZipCompress(inputPath);
		var fileInfo = new FileInfo(filePath);
		return fileInfo.Length;
	}
	
	static void CryFile(string path)
	{
		Debug.LogWarning("cry path =" + path);
        var b = DWFileUtil.ReadFile(path);
		var cb = GlobalCrypto.Encrypte(b);
		File.WriteAllBytes(path, cb);
	}
}
