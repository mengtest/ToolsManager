using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using tools.CAssetBundleData;

public delegate void LoadSucCB(AssetBundle ab, string abName);
public delegate void ReadFileCB(byte[] b);
public delegate void ReadWWW(string txt);
public delegate void LoadDsyncCB(); //异步加载完成回调

public class CABItemLoad
{
    public RessKey m_ressKey;
    public RessStorgeType m_ressStore;
    public int m_sort = 0;

    public CABItemLoad(RessKey ressKey, RessStorgeType ressStore)
    {
        m_ressKey = ressKey;
        m_ressStore = ressStore;
    }

    public CABItemLoad(RessKey ressKey, RessStorgeType ressStore, int sort)
    {
        m_ressKey = ressKey;
        m_ressStore = ressStore;
        m_sort = sort;
    }
}


[RegisterSystem(typeof(CABLoadSys), true)]
public class CABLoadSys : TCoreSystem<CABLoadSys>, IInitializeable
{
    bool isLoad = false;
	public void Init ()
	{
        if (!isLoad)
        {
            ResourceManager.m_readFileCallback = EZFunTools.ReadFile;
            ResourceManager.Instance.Init();

            GameRoot.Instance.StartCoroutine(InitABCoroutine());
            isLoad = true;
        }
	}

    public void Release()
    {

    }

	//当前游戏所有的assetsbundle对应的加载路径
	private Dictionary<string, string> m_abPathDic = new Dictionary<string, string>();
	private Dictionary<string, AssetBundle> m_assetBundleDic = new Dictionary<string, AssetBundle>() ;
    private Dictionary<string, CAssetBundleDataStruct> m_ressPostionDic = new Dictionary<string, CAssetBundleDataStruct>();
	private bool m_initRessPostionDicDone = false;
	private float _m_unpackRate = 0f;

	private bool _m_initLoginDone = false;
	public bool m_initLoginDone {get{return _m_initLoginDone;}}//登陆界面所需是否加载完成
	public float m_unpackRate {get{return _m_unpackRate;}} //本地ab解压进度

	public static void InitUpdateSys(bool useAB)
	{
		Constants.FORCE_LOAD_AB = useAB;
	}

	public bool CheckNeedUnpack()
	{
		return EZFunTools.CheckLoadAllAB() && Application.platform == RuntimePlatform.Android;
	}

	//解压本地ab
	public void UnpackResources()
	{
		if(CheckNeedUnpack())
		{
			string M_ASSETS_BUNDLE_LOCAL_PATH = EZFunTools.StreamPath + "/AssetsBundle/";
			string path = EZFunTools.StreamPath + "/AssetsBundle/localAB.assetbundle";
            GameRoot.Instance.StartCoroutine(LoadABCoroutine(path, "localAB", (AssetBundle ab, string name) => {
				ab.Unload(true);
				_m_unpackRate = 1f;
			}));
		}
		else
		{
			_m_unpackRate = 1;
		}
	}

	//初始化AB，先读取RessPos，在读取所有的AB
	IEnumerator InitABCoroutine()
	{
		m_abPathDic.Clear();
		m_assetBundleDic.Clear();
		InitRessPosCoroutine();
		while(!m_initRessPostionDicDone)
		{
			yield return null;
		}	

		if(m_abPathDic.Count == 0)
		{
			InitLoginNeed();
		}
		else
		{
			foreach(var kv in m_abPathDic)
			{
		
					AssetBundle ab = AssetBundle.LoadFromFile(kv.Value);
					m_assetBundleDic.Add(kv.Key, ab);
					if(m_abPathDic.Count == m_assetBundleDic.Count)
					{
						InitLoginNeed();
					}
				
			}
		}	
	}

	//保存一份字典<资源名字，所在AssetsBundle名字>
	//先读取本地的，然后再读取更新下来的map，通过版本号比较看用哪一份Assetsbundle
	void InitRessPosCoroutine()
	{
        //本地读取完
		bool loadLocalDone = false;
		//更新下来的配置读取完
		bool loadUpateDone = false;
		int seq = 0;

		if(EZFunTools.CheckLoadAllAB())
		{
			string M_ASSETS_BUNDLE_LOCAL_PATH = EZFunTools.StreamPath + "/AssetsBundle/";
			var abMapFileDir = "abDataStruct.abMap";
			string path = M_ASSETS_BUNDLE_LOCAL_PATH + abMapFileDir;
			StartStreamFile(path, (byte[] b)=>{
				InitRessPos(b, EZFunTools.StreamPath + "/AssetsBundle/localAB.assetbundle");
				loadLocalDone = true;
				if(loadLocalDone && loadUpateDone)
				{
					m_initRessPostionDicDone = true;
				}
			});
		}
		else
		{
			loadLocalDone = true;
		}
        
        string M_ASSETS_BUNDLE_UPDATE_PATH = EZFunTools.AvailablePath + "/AssetsBundle/";
        if (Directory.Exists(M_ASSETS_BUNDLE_UPDATE_PATH))
        {
            // 取更新
           	Dictionary<string, string> dic = new Dictionary<string, string>();
			DirectoryInfo direct = new DirectoryInfo(M_ASSETS_BUNDLE_UPDATE_PATH);
			FileInfo[] fileArray = direct.GetFiles();

			for(int fileIndex = 0; fileIndex < fileArray.Length; fileIndex ++)
			{
				if(fileArray[fileIndex].Name.EndsWith("abMap"))
				{
					StartStreamFile(fileArray[fileIndex].FullName, (byte[] b) => {
						InitRessPos(b, M_ASSETS_BUNDLE_UPDATE_PATH, true);
						seq ++;
						if(seq == fileArray.Length/2)
						{
							loadUpateDone = true;
							if(loadLocalDone && loadUpateDone)
							{
								m_initRessPostionDicDone = true;
							}
						}
					});
				}
			}
        }
		else
		{
			loadUpateDone = true;
			if(loadLocalDone && loadUpateDone)
			{
				m_initRessPostionDicDone = true;
			}
		}
	}

    private void InitRessPos(byte[] b, string path, bool isUpdate = false)
    {
        ResCABDataStructLs cabDSLs = new ResCABDataStructLs();
        Stream file = new MemoryStream(b);
        cabDSLs = (ResCABDataStructLs)Serializer.Deserialize(file,typeof(ResCABDataStructLs));
        for (int lsIndex = 0; cabDSLs.list != null && lsIndex < cabDSLs.list.Count; lsIndex++)
        {
            CAssetBundleDataStruct item = cabDSLs.list[lsIndex];

			if(item.m_storeAssetBundleName.StartsWith("levelScene"))
			{
				CSceneRessLoadSys.Instance.AddSceneItem(item, path);
			}
			else
			{
				if(!m_abPathDic.ContainsKey(item.m_storeAssetBundleName))
				{
					if(isUpdate)
					{
						m_abPathDic.Add(item.m_storeAssetBundleName, path + item.m_storeAssetBundleName + ".assetbundle");
					}
					else
					{
						m_abPathDic.Add(item.m_storeAssetBundleName, path);
					}
				}
				
				if (!m_ressPostionDic.ContainsKey(item.m_ressGbName))
				{
					
					if(item.m_ressGbName.Contains("no_use_ref"))
					{
					}
					m_ressPostionDic.Add(item.m_ressGbName, item);
				}
				else
				{
					var existedItem = m_ressPostionDic[item.m_ressGbName];
					if (existedItem.m_ressVersion < item.m_ressVersion)
					{
						m_ressPostionDic[item.m_ressGbName] = item;
					}
				}
			}
		}
	}
	
	IEnumerator LoadABCoroutine(string path, string abName, LoadSucCB cb)
	{
		if(Application.platform != RuntimePlatform.Android)
		{
			path = "file:///" + path;
		}

		WWW www = WWW.LoadFromCacheOrDownload(path, Version.Instance.GetRessVersion());
		while(!www.isDone)
		{
			_m_unpackRate = (int)(www.progress * 100) / 100f - 0.01f;
			if(_m_unpackRate < 0)
			{
				_m_unpackRate = 0;
			}
			yield return null;
		}
		cb(www.assetBundle, abName);
		www.Dispose();
	}

	private void InitLoginNeed()
	{
		EZFunWindowMgr.Instance.InitWindowDic(EZFunWindowEnum.luaWindow, RessType.RT_CommonWindow, RessStorgeType.RST_Never, "login_ui_windnow");
		_m_initLoginDone = true;
	}

	#region tool

	public void StartStreamFile(string path, ReadFileCB callBack)
	{		

			byte[] b = EZFunTools.ReadFileStream(path);
			callBack(b);
		
	}

	IEnumerator ReadAndroidFile(string path, ReadFileCB callBack)
	{
		WWW www = new WWW(path);
		while(!www.isDone)
		{
			yield return null;
		}
		callBack(www.bytes);
	}

	public void StartWWW(string url, ReadWWW cb)
	{
        GameRoot.Instance.StartCoroutine(WWWCoroutine(url, cb));
	}

	private IEnumerator WWWCoroutine(string url, ReadWWW cb)
	{
		float startTime = Time.realtimeSinceStartup;
		WWW www = new WWW(url);
		while(!www.isDone && (Time.realtimeSinceStartup - startTime) < 10)
		{
			yield return null;
		}

		if((Time.realtimeSinceStartup - startTime) >= 10)
		{
			cb("-1");
		}
		else if(string.IsNullOrEmpty(www.error))
		{
			cb(www.text);
		}
		else
		{
			cb("-1");
		}
	}

	public Object GetOB(string ressName)
	{
		float time = Time.realtimeSinceStartup;

		ressName = ressName.ToLower();
		if(m_ressPostionDic.ContainsKey(ressName))
		{
			string abName = m_ressPostionDic[ressName].m_storeAssetBundleName;
			if(m_assetBundleDic.ContainsKey(abName))
			{
				AssetBundle ab = m_assetBundleDic[abName];
				Object ob = ab.LoadAsset("AssetsBundle_" + ressName);
			
				return ob;
			}
		}

        return null;
	}

	public bool CheckContain(string ressName)
	{
		ressName = ressName.ToLower();
		if(m_ressPostionDic.ContainsKey(ressName))
		{
			string abName = m_ressPostionDic[ressName].m_storeAssetBundleName;
			if(m_assetBundleDic.ContainsKey(abName))
			{
				return true;
			}
		}
		return false;
	}

    public void StartLoadAync(List<CABItemLoad> itemLs, LoadDsyncCB cb = null)
	{
        GameRoot.Instance.StartCoroutine(LoadAyncCoroutine(itemLs, cb));
	}

	public void StartLoadAync(string name, RessType ressType, LoadDsyncCB cb)
	{
		if(!EZFunTools.CheckLoadAllAB())
		{
			cb();
		}
		else
		{
            List<CABItemLoad> itemLs = new List<CABItemLoad>();
            itemLs.Add(new CABItemLoad(new RessKey(ressType,name), RessStorgeType.RST_Never));
            GameRoot.Instance.StartCoroutine(LoadAyncCoroutine(itemLs, cb));
		}
	}

    IEnumerator LoadAyncCoroutine(List<CABItemLoad> itemLs, LoadDsyncCB cb = null)
	{
		int seq = 0;
        for(int i = 0; i < itemLs.Count; i ++)
        {
            string ressName = itemLs[i].m_ressKey.m_ressName.ToLower();
            if(m_ressPostionDic.ContainsKey(ressName))
            {
                string abName = m_ressPostionDic[ressName].m_storeAssetBundleName;
                
                if(ResourceMgr.m_ressDic.ContainsKey(itemLs[i].m_ressKey))
                {
                    seq = CheckLoadDone(seq, itemLs.Count, cb);
                    continue;
                }
                
                if(m_assetBundleDic.ContainsKey(abName))
                {
                    AssetBundle ab = m_assetBundleDic[abName];
                    AssetBundleRequest abr = ab.LoadAssetAsync("AssetsBundle_" + ressName, typeof(GameObject));
                    yield return abr;
                  //  ResourceMgr.InsertAsset(abr.asset, itemLs[i].m_ressKey.m_type, itemLs[i].m_ressKey.m_ressName, itemLs[i].m_ressStore);
                    seq = CheckLoadDone(seq, itemLs.Count, cb);
                }
                else
                {
                    seq = CheckLoadDone(seq, itemLs.Count, cb);
                }
            }
            else
            {
				ResourceRequest rq = Resources.LoadAsync(ResourceMgr.GetRessPath(itemLs[i].m_ressKey));
				yield return rq;
			//	ResourceMgr.InsertAsset(rq.asset, itemLs[i].m_ressKey.m_type, itemLs[i].m_ressKey.m_ressName, itemLs[i].m_ressStore);
                seq = CheckLoadDone(seq, itemLs.Count, cb);
            }
        }
	}

	private int CheckLoadDone(int seq, int total, LoadDsyncCB cb)
	{
		seq ++;
		if(total == seq)
		{
//            Debug.LogError("load done");
            if(cb != null)
            {
                cb();
            }
		}

		return seq;
	}
	#endregion
}
