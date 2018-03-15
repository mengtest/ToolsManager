using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using tools.CAssetBundleData;

public class CSceneRessLoadSys
{
	static CSceneRessLoadSys _Instance = null;
	public static CSceneRessLoadSys Instance
	{
		get
		{
			if(_Instance == null)
			{
				_Instance = new CSceneRessLoadSys();
			}
			return _Instance;
		}
	}

	class CSceneRessData
	{
		public string m_path;
		public CAssetBundleDataStruct m_item;

		public CSceneRessData(string path, CAssetBundleDataStruct item)
		{
			m_path = path;
			m_item = item;
		}
	}

	private Dictionary<string, CSceneRessData> m_scenePath = new Dictionary<string, CSceneRessData>();
	private AssetBundle m_currentAB = null;

	public void AddSceneItem(CAssetBundleDataStruct item, string path)
	{
		if(!m_scenePath.ContainsKey(item.m_ressGbName))
		{
			m_scenePath.Add(item.m_ressGbName, new CSceneRessData(path + item.m_storeAssetBundleName + ".lvData", item));
		}
		else
		{
			CAssetBundleDataStruct citem = m_scenePath[item.m_ressGbName].m_item;
			if(item.m_ressVersion > citem.m_ressVersion)
			{
				m_scenePath[item.m_ressGbName] = new CSceneRessData(path + item.m_storeAssetBundleName + ".lvData", item);
			}
		}
	}

	public void LoadSceneAB(string lvName)
	{
		if(m_scenePath.ContainsKey(lvName))
		{
			m_currentAB = AssetBundle.LoadFromFile(m_scenePath[lvName].m_path);
		}
	}

	public void UnloadSceneAB()
	{
		if(m_currentAB != null)
		{
			m_currentAB.Unload(true);
			m_currentAB = null;
		}
	}

}
